using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyOrm
{
    internal interface IMappingPool: IEnumerable<OrmMap>
    {
        void RegisterType(Type type);

        OrmMap GetMap<T>();

        OrmMap GetMap(Type type);

        OrmMap GetMap(string tableName);
    }
}
