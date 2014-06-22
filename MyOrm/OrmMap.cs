using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using MyOrm.Attributes;
using MyOrm.OrmMapClasses;

namespace MyOrm
{
    internal enum ColumnType { Simple, Relation };

    internal class OrmMap
    {
        private IDictionary<string, SimpleColumnInfo> _simpleColumnToProperty;
        private ICollection<PropertyInfo> _oneAttributeProperty;
        private ICollection<RelationManyColumnInfo> _manyAttributeProperty;


        public TableInfo TableInfo { get; private set; }

        public IMappingPool Pool { get; set; }

        public SimpleColumnInfo Id
        {
            get
            {
                return (from sci in _simpleColumnToProperty.Values where sci.IsId select sci).First();
            }
        }

        public IEnumerable<string> Columns
        {
            get { return _simpleColumnToProperty.Keys; }
        }

        public IEnumerable<PropertyInfo> OneRelations
        {
            get { return _oneAttributeProperty; }
        }

        public IEnumerable<RelationManyColumnInfo> ManyRelations
        {
            get { return _manyAttributeProperty; }
        }


        private OrmMap()
        {
            _simpleColumnToProperty = new Dictionary<string, SimpleColumnInfo>();

            _oneAttributeProperty = new List<PropertyInfo>();
            _manyAttributeProperty = new List<RelationManyColumnInfo>();
        }


        public SimpleColumnInfo this[string columnName]
        {
            get
            {
                if(_simpleColumnToProperty.ContainsKey(columnName))
                    return _simpleColumnToProperty[columnName];
                else
                {                  
                    string tmp = columnName.Replace(TableInfo.DbTableName + ".", String.Empty);
                    if (_simpleColumnToProperty.ContainsKey(tmp))
                        return _simpleColumnToProperty[tmp];
                    else
                        throw new ArgumentException("Column not found.");
                }
            }
        }


        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.Append("Table name: ");
            b.Append(TableInfo.DbTableName);
            b.Append("\nColumns: \n");
            foreach (var c in _simpleColumnToProperty)
            {
                b.Append("  ");
                b.Append(c.Key);
                b.Append(" => ");
                b.Append(c.Value.PropertyInfo.Name);
                b.Append("\n");
            }
            return b.ToString();
        }


        public static OrmMap FromType(Type type)
        {          
            OrmMap map = new OrmMap();
            map.TableInfo = TableInfo.FromType(type);

            foreach (PropertyInfo p in type.GetProperties())
            {
                ColumnAttribute ca = p.GetCustomAttribute<ColumnAttribute>();
                if (ca != null)
                {
                    map._simpleColumnToProperty[ca.ColumnName] = SimpleColumnInfo.FromPropertyInfo(p);
                }
                else
                {
                    RelationAttribute ra = p.GetCustomAttribute<RelationAttribute>();
                    if (ra != null)
                    {
                        if (ra.Type == RelationType.One)
                        {
                            // TODO
                            map._oneAttributeProperty.Add(p);
                        }
                        else if(ra.Type == RelationType.Many)
                        {
                            var tmp = RelationManyColumnInfo.FromPropertyInfo(p);
                            map._manyAttributeProperty.Add(tmp);
                        }
                    }
                }
            }
            return map;
        }
    }
}
