using MicroORMvsEntityFramework.Domain;
using MyOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FillDatabase
{
    class Program
    {
        static void Main(string[] args)
        {
            MyORM orm = new MyORM("myDbServer", typeof(Person), typeof(Phone));
            orm.Log = Console.WriteLine;

            // non lazy
            orm.Lazy = false;
            var persons = orm.SelectAll<Person>();

            // lazy
            orm.Lazy = true;
            var personsL = orm.SelectAll<Person>();

            Console.WriteLine("----------------------------------------------------------------------------------");

            using (EFContext c = new EFContext("myDbServer"))
            {
                c.Database.Log = Console.WriteLine;
                c.Persons.Include("Phones").ToList();
            }

            Console.ReadKey();
        }
    }
}