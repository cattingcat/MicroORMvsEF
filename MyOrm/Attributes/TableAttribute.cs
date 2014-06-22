using System;

namespace MyOrm.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class TableAttribute: Attribute
    {
        public string TableName { get; set; }
    }
}
