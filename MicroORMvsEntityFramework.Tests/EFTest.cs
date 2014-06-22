using MicroORMvsEntityFramework.Domain;
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
    public class EFTest
    {
        EFContext _ef;

        [SetUp]
        public void SetUp()
        {
            _ef = new EFContext("myDbServer");
            // first query 
            var persons = _ef.Persons.ToList();
            //_ef.Database.Log = Console.WriteLine;
        }

        [TearDown]
        public void TearDown()
        {
            _ef.Dispose();
        }

        [Test]
        public void SelectX1000_EntityFramework()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < 1000; ++i)
            {
                var persons = _ef.Persons.ToList();
            }

            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
        }

        [Test]
        public void SelectWithRelationsX1000_EntityFramework()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < 1000; ++i)
            {
                var persons = _ef.Persons.Include("Phones").ToList();
            }

            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
        }
    }

}
