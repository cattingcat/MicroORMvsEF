using MyOrm.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyOrm.OrmMapClasses
{
    internal class TableInfo
    {
        private TableInfo() { }

        public string DbTableName { get; private set; }
        public Type Type { get; private set; }

        public static TableInfo FromType(Type type)
        {
            TableInfo res = new TableInfo();
            res.Type = type;
            TableAttribute attr = (TableAttribute)type.GetCustomAttributes(typeof(TableAttribute), true).SingleOrDefault();
            if (attr == null)
            {
                throw new Exception("this type have no TableAttribute");
            }
            res.DbTableName = attr.TableName;
            return res;            
        }
    }
}
