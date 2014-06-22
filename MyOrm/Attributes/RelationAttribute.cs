using System;

namespace MyOrm.Attributes
{
    public enum RelationType { Many, One };

    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false, Inherited=true)]
    public class RelationAttribute: Attribute
    {
        public RelationType Type { get; set; }
        public string SecondTable { get; set; }
        public string SecondColumn { get; set; }
        public string ThisColumn { get; set; }
    }
}
