using System;

namespace MyOrm.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class OneAttribute: RelationAttribute
    {
        public OneAttribute()
        {
            this.Type = RelationType.One;
        }
    }
}
