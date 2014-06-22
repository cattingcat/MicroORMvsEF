using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MicroOrm

namespace FillDatabaseApp
{
    class Program
    {
        static void Main(string[] args)
        {
            using (EFContext context = new EFContext("myDbServer"))
            {
                context.Database.Log = Console.WriteLine;
                for (int i = 0; i < 100; ++i)
                {
                    Person p = context.Persons.Add(new Person() { Name = "person" });
                    for (int j = 0; j < 5; ++j)
                    {
                        context.Phones.Add(new Phone() { Number = "phone", Person = p });
                    }
                }
                context.SaveChanges();
            }
        }
    }
}
