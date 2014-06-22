using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyOrm
{
    internal class DefaultMappingPool: IMappingPool
    {
        private ICollection<OrmMap> _mappingPool = new List<OrmMap>();

        public void RegisterType(Type type)
        {
            OrmMap map = OrmMap.FromType(type);
            _mappingPool.Add(map);
            map.Pool = this;
        }

        public OrmMap GetMap<T>()
        {
            return (from map in _mappingPool where map.TableInfo.Type == typeof(T) select map).First();
        }

        public OrmMap GetMap(Type type)
        {
            return (from map in _mappingPool where map.TableInfo.Type == type select map).First();
        }

        public OrmMap GetMap(string tableName)
        {
            return (from map in _mappingPool where map.TableInfo.DbTableName == tableName select map).First();
        }


        public IEnumerator<OrmMap> GetEnumerator()
        {
            return _mappingPool.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _mappingPool.GetEnumerator();
        }
    }
}
