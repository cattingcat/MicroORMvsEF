using MyOrm.Attributes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroORMvsEntityFramework.Domain
{
    [Table(TableName="People")]
    public class Person
    {
        [Id(ColumnName = "PersonId", ColumnType = DbType.Int32)]
        public int PersonId { get; set; }

        [Column(ColumnName = "Name", ColumnType = DbType.String)]
        public string Name { get; set; }

        [Many(SecondColumn = "Person_PersonId")]
        public virtual ICollection<Phone> Phones { get; set; }
    }
}
