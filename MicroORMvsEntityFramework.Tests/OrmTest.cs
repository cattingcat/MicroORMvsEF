using MicroORMvsEntityFramework.Domain;
using MyOrm;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroORMvsEntityFramework.Tests
{
    [TestFixture]
    public class OrmTest
    {
        MyORM _orm;

        [SetUp]
        public void SetUp()
        {
            _orm = new MyORM("myDbServer", typeof(Person), typeof(Phone));           
        }

        [Test]
        public void SelectX1000_MicroOrm()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < 1000; ++i)
            {
                _orm.Lazy = true;
                var persons = _orm.SelectAll<Person>();
                _orm.Lazy = false;
            }

            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
        }

        [Test]
        public void SelectWithRelationsX1000_MicroOrm()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < 1000; ++i)
            {
                _orm.Lazy = false;
                var persons = _orm.SelectAll<Person>();                
            }

            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
        }
    }
}
