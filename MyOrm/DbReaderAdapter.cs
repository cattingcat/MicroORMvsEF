using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;

namespace MyOrm
{
    internal class DbReaderAdapter<T> where T : class, new()
    {
        private DbDataReader _reader;
        private OrmMap _mainMap;
        private IMappingPool _pool;

        public DbReaderAdapter(DbDataReader reader, IMappingPool pool)
        {
            _reader = reader;
            _mainMap = pool.GetMap<T>();
            _pool = pool;
        }

        private object ReadFields(OrmMap map, ref int readerOffset)
        {
            Type resultType = map.TableInfo.Type;
            object res = Activator.CreateInstance(resultType);            
            
            foreach (string column in map.Columns)
            {
                PropertyInfo info = map[column].PropertyInfo;
                object value = _reader.GetValue(readerOffset);
                if (value != DBNull.Value)
                {
                    info.SetValue(res, value);
                }
                else
                {
                    info.SetValue(res, null);
                }
                ++readerOffset;
            }
            return res;
        }

        public IEnumerable<T> GetResult()
        {
            ICollection<T> result = new List<T>();
            int readerOffset = 0;

            T currentResult = null;

            while (_reader.Read())
            {
                T obj = (T)ReadFields(_mainMap, ref readerOffset);

                if (currentResult == null)
                {
                    currentResult = obj;
                }
                else if (!_mainMap.Id.PropertyInfo.GetValue(currentResult).Equals(_mainMap.Id.PropertyInfo.GetValue(obj)))
                {
                    result.Add(currentResult);
                    currentResult = obj;
                }

                foreach (var relation in _mainMap.ManyRelations)
                {
                    OrmMap innerMap = _pool.GetMap(relation.SecondTable);
                    Type collectionType = typeof(List<>).MakeGenericType(relation.CollectionGenericArgument);

                    object innerObj = ReadFields(innerMap, ref readerOffset);

                    object collection = relation.PropertyInfo.GetValue(currentResult);
                    if (collection == null)
                    {                        
                        collection = Activator.CreateInstance(collectionType);
                        relation.PropertyInfo.SetValue(obj, collection);
                    }                    

                    collectionType.GetMethod("Add").Invoke(collection, new[] { innerObj });
                }
                readerOffset = 0;
            }

            result.Add(currentResult);

            return result;
        }

        public IEnumerable<T> GetResultLazy(MyORM orm)
        {
            ICollection<T> result = new List<T>();
            int readerOffset = 0;

            while (_reader.Read())
            {
                T obj = (T)ReadFields(_mainMap, ref readerOffset);

                result.Add(obj);
                readerOffset = 0;

                foreach (var relation in _mainMap.ManyRelations)
                {
                    string where = String.Format("{0}.{1}=@id", relation.SecondTable, relation.ForeignKey);
                    string strCommand = _pool.GetMap(relation.SecondTable).BuildSelectWhereQuery(where);

                    DbCommand command = orm.ProviderFactory.CreateCommand();
                    command.CommandText = strCommand;
                    DbParameter param = command.CreateParameter();
                    param.ParameterName = "@id";
                    param.DbType = _mainMap.Id.DbType;
                    param.Value = _mainMap.Id.PropertyInfo.GetValue(obj);
                    command.Parameters.Add(param);

                    object collection = typeof(OrmEnumerable<>).MakeGenericType(_pool.GetMap(relation.SecondTable).TableInfo.Type).
                        GetConstructor(new[] { typeof(MyORM), typeof(DbCommand) }).Invoke(new object[] { orm, command });

                    if (relation.CollectionType == typeof(IEnumerable<>))
                    {
                        relation.PropertyInfo.SetValue(obj, collection);
                    }
                    else
                    {
                        // TODO not IEnumerable lazy collection
                    }
                }
            }
            return result;
        }
    }
}
