using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;

using MyOrm.Attributes;
using MyOrm.OrmMapClasses;
using System.Configuration;

namespace MyOrm
{
    public class MyORM: IOrm
    {        
        internal DbProviderFactory ProviderFactory { get; private set; }
        internal IMappingPool MappingPool { get; private set; }

        private string _connectionString;        

        public bool Lazy { get; set; }
        public Action<string> Log { get; set; }


        public MyORM(string connectionStringName, params Type[] types)
        {
            var connectionStringSettings = ConfigurationManager.ConnectionStrings[connectionStringName];

            ProviderFactory = DbProviderFactories.GetFactory(connectionStringSettings.ProviderName);
            MappingPool = new DefaultMappingPool();            
            
            _connectionString = connectionStringSettings.ConnectionString;

            Lazy = false;

            foreach (Type type in types)
            {
                MappingPool.RegisterType(type);
            }
        }
        

        #region IOrm
        public IEnumerable<T> SelectAll<T>() where T : class, new()
        {
            OrmMap map = MappingPool.GetMap<T>();

            string selectQuery = String.Empty;
            if (!Lazy)
            {
                selectQuery = map.BuildSelectAllQuery();
            }
            else
            {
                selectQuery = map.BuildSelectAllQueryLazy();
            }
            
            DbCommand command = ProviderFactory.CreateCommand();
            command.CommandText = selectQuery;

            IEnumerable<T> result = null;
            using (DbConnection connection = GetOpenConnection())
            {
                DoLog(command.CommandText);

                command.Connection = connection;
                using (DbDataReader reader = command.ExecuteReader())
                {
                    DbReaderAdapter<T> adapter = new DbReaderAdapter<T>(reader, MappingPool);
                    if(!Lazy)
                    {
                        result = adapter.GetResult();
                    }
                    else
                    {
                        result = adapter.GetResultLazy(this);
                    }
                }
            }

            return result;
        }

        public T SelectById<T>(object id) where T: class, new()
        {            
            OrmMap map = MappingPool.GetMap<T>();            
            string whereStatement = String.Format("{0}.{1}=@id", map.TableInfo.DbTableName, map.Id.DbColumnName);
            string selectQuery = map.BuildSelectAllQuery(whereStatement);        

            using (DbConnection connection = GetOpenConnection())
            {
                DbCommand command = connection.CreateCommand();
                command.CommandText = selectQuery;

                DbParameter param = command.CreateParameter();
                param.DbType = map.Id.DbType;
                param.ParameterName = "@id";
                param.Value = id;
                command.Parameters.Add(param);

                DoLog(command.CommandText);

                T result = null;
                using (DbDataReader reader = command.ExecuteReader())
                {
                    DbReaderAdapter<T> adapter = new DbReaderAdapter<T>(reader, MappingPool);
                    result = adapter.GetResult().First();
                }

                ResolveManyAttribute(result);

                return result;
            }
        }

        public int Insert(object o)
        {
            Type type = o.GetType();
            OrmMap map = MappingPool.GetMap(type);            
            StringBuilder argListBuilder = new StringBuilder();
            List<KeyValuePair<string, string>> colNameToNamedParam = new List<KeyValuePair<string, string>>();

            foreach (string col in map.Columns)
            {
                string namedParam = '@' + col;
                colNameToNamedParam.Add(new KeyValuePair<string, string>(col, namedParam));
                argListBuilder.Append(namedParam);
                argListBuilder.Append(',');
            }
            argListBuilder.Remove(argListBuilder.Length - 1, 1);

            string insertQuery = map.BuildInsertQuery(argListBuilder.ToString());

            using (DbConnection connection = GetOpenConnection())
            {
                DbCommand command = connection.CreateCommand();
                command.CommandText = insertQuery;
                foreach (var pair in colNameToNamedParam)
                {
                    DbParameter param = command.CreateParameter();
                    param.DbType = map[pair.Key].DbType;
                    param.ParameterName = pair.Value;
                    param.Value = map[pair.Key].PropertyInfo.GetValue(o) ?? DBNull.Value;                    
                    command.Parameters.Add(param);
                }

                DoLog(command.CommandText);

                return command.ExecuteNonQuery();
            }
        }

        public int Delete<T>(object id) where T : class, new()
        {
            OrmMap map = MappingPool.GetMap<T>();            
            string whereStatement = String.Format("{0}=@id", map.Id);

            string deleteQuery = map.BuildDeleteQuery(whereStatement);

            using (DbConnection connection = GetOpenConnection())
            {
                DbCommand command = connection.CreateCommand();
                command.CommandText = deleteQuery;

                DbParameter param = command.CreateParameter();
                param.DbType = map.Id.DbType;
                param.ParameterName = "@id";
                param.Value = id;
                command.Parameters.Add(param);

                DoLog(command.CommandText);

                return command.ExecuteNonQuery();
            }
        }
        #endregion

        #region Helpers
        internal DbConnection GetOpenConnection()
        {
            DoLog("Connection opened!");

            DbConnection connection = ProviderFactory.CreateConnection();
            connection.ConnectionString = _connectionString;
            connection.Open();
            return connection;
        }

        internal void DoLog(string message)
        {
            if (Log != null)
                Log(message);
        }


        private IEnumerable<T> GetEnumerable<T>() where T : class, new()
        {
            return new OrmEnumerable<T>(this);
        }


        internal void ResolveManyAttribute(object o)
        {
            if (!Lazy)
                return;

            OrmMap map = MappingPool.GetMap(o.GetType());
            foreach (RelationManyColumnInfo relation in map.ManyRelations)
            {
                string secondTable = relation.SecondTable;
                string secondTbsFK = relation.ForeignKey;
                // SELECT * FROM {{secondTable}} where {{secondTbsFK}} == %firstTableKey%;

                object firstTableKeyValue = map.GetId(o);
                OrmMap secondTableMap = 
                    (from m in MappingPool
                        where m.TableInfo.DbTableName == secondTable select m).First();

                string selectQuery = secondTableMap.BuildSelectWhereQuery(String.Format("{0}=@pk", secondTbsFK));

                DbCommand comm = ProviderFactory.CreateCommand();
                comm.CommandText = selectQuery;
                DbParameter param = comm.CreateParameter();
                param.DbType = map.Id.DbType;
                param.Value = firstTableKeyValue;
                param.ParameterName = "@pk";
                comm.Parameters.Add(param);

                Type enumerableType = typeof(OrmEnumerable<>).MakeGenericType(secondTableMap.TableInfo.Type);
                object enumerable = enumerableType.GetConstructor(new[] { typeof(MyORM), typeof(DbCommand) }).Invoke(new object[]{ this, comm});                

                if (relation.CollectionType == typeof(IEnumerable<>))
                {
                    relation.PropertyInfo.SetValue(o, enumerable);
                }
                else if (relation.CollectionType == typeof(ICollection<>))
                {          
                    MethodInfo method = typeof(Enumerable).GetMethod("ToList");
                    object icollection = method.MakeGenericMethod(relation.CollectionGenericArgument).Invoke(null, new object[] { enumerable });

                    relation.PropertyInfo.SetValue(o, icollection);
                }                
            }
        }
        #endregion
    }
}
