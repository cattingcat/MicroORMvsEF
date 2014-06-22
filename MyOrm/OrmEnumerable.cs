using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyOrm
{
    class OrmEnumerable<T> : IEnumerable<T> where T : class, new()
    {
        private MyORM _orm;
        private DbCommand _selectCommand;
        private IEnumerable<T> _result;


        public OrmEnumerable(MyORM orm)
        {
            _orm = orm;

            OrmMap map = _orm.MappingPool.GetMap<T>();
            string selectQuery = map.BuildSelectAllQuery();
            _selectCommand = _orm.ProviderFactory.CreateCommand();
            _selectCommand.CommandText = selectQuery;
        }

        public OrmEnumerable(MyORM orm, DbCommand command)
        {
            _orm = orm;
            _selectCommand = command;
        }


        public IEnumerator<T> GetEnumerator()
        {
            if (_result != null)
                return _result.GetEnumerator();

            using (DbConnection connection = _orm.GetOpenConnection())
            {
                _orm.DoLog(_selectCommand.CommandText);

                _selectCommand.Connection = connection;
                using (DbDataReader reader = _selectCommand.ExecuteReader())
                {
                    DbReaderAdapter<T> adapter = new DbReaderAdapter<T>(reader, _orm.MappingPool);
                    _result = adapter.GetResult();
                }
            }
            return _result.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
