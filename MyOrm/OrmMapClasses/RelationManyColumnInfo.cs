using MyOrm.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyOrm.OrmMapClasses
{
    internal class RelationManyColumnInfo
    {
        private RelationManyColumnInfo() { }

        public string SecondTable { get; private set; }
        public string ForeignKey { get; private set; }
        public PropertyInfo PropertyInfo { get; private set; }

        public Type CollectionType { get; private set; }
        public Type CollectionGenericArgument { get; private set; }

        public static RelationManyColumnInfo FromPropertyInfo(PropertyInfo info)
        {
            RelationAttribute ra = info.GetCustomAttribute<RelationAttribute>();
            if (ra != null)
            {
                RelationManyColumnInfo res = new RelationManyColumnInfo();
                res.ForeignKey = ra.SecondColumn;
                res.SecondTable = ra.SecondTable;
                res.PropertyInfo = info;

                Type genericPropertyType = info.PropertyType.GetGenericTypeDefinition();

                if (genericPropertyType == typeof(IEnumerable<>) || genericPropertyType == typeof(ICollection<>))
                {
                    Type genericArg = info.PropertyType.GetGenericArguments().First();
                    var tblAttr = genericArg.GetCustomAttribute<TableAttribute>();
                    if(String.IsNullOrEmpty(ra.SecondTable))
                    {
                        res.SecondTable = tblAttr.TableName;
                    }

                    res.CollectionType = genericPropertyType;
                    res.CollectionGenericArgument = genericArg;
                }
                else
                {
                    throw new ArgumentException("Many-property should be IEnumerable or ICollection type");
                }

                return res;
            }
            else
                throw new ArgumentException("Property without attribute");
        }
    }
}
