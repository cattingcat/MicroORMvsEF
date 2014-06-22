using System;
using System.Data;

namespace MyOrm.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false, Inherited=true)]
    public class ColumnAttribute: Attribute
    {
        public string ColumnName { get; set; }
        public DbType ColumnType { get; set; }
    }
}
