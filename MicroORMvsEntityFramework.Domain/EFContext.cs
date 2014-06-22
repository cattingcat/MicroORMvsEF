using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroORMvsEntityFramework.Domain
{
    public class EFContext : DbContext
    {
        public EFContext(string nameOrConnectionString):
            base(nameOrConnectionString) { }

        public DbSet<Person> Persons { get; set; }
        public DbSet<Phone> Phones { get; set; }
    }
}
