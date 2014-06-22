using MyOrm.Attributes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyOrm.OrmMapClasses
{
    internal class SimpleColumnInfo
    {
        private SimpleColumnInfo() { }

        public string DbColumnName { get; private set; }
        public PropertyInfo PropertyInfo { get; private set; }
        public bool IsId { get; private set; }
        public DbType DbType { get; private set; }

        public static SimpleColumnInfo FromPropertyInfo(PropertyInfo info)
        {
            SimpleColumnInfo res = new SimpleColumnInfo();
            ColumnAttribute ca = info.GetCustomAttribute<ColumnAttribute>();
            res.DbColumnName = ca.ColumnName;
            res.PropertyInfo = info;
            res.IsId = (ca as IdAttribute) != null;
            res.DbType = ca.ColumnType;
            return res;
        }
    }
}
