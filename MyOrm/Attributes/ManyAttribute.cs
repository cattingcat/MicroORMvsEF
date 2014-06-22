using System;

namespace MyOrm.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ManyAttribute: RelationAttribute
    {
        public ManyAttribute()
        {
            this.Type = RelationType.Many;
        }
    }
}
