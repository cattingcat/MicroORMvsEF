using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyOrm
{
    public interface IOrm
    {
        IEnumerable<T> SelectAll<T>() where T : class, new();

        T SelectById<T>(object id) where T : class, new();
        
        int Insert(object o);

        int Delete<T>(object id) where T : class, new();
    }
}
