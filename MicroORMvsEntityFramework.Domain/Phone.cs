using MyOrm.Attributes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroORMvsEntityFramework.Domain
{
    [Table(TableName = "Phones")]
    public class Phone
    {
        [Id(ColumnName = "PhoneId", ColumnType = DbType.Int32)]
        public int PhoneId { get; set; }

        [Column(ColumnName = "Number", ColumnType = DbType.String)]
        public string Number { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        [Column(ColumnName = "Person_PersonId", ColumnType = DbType.Int32)]
        public int PersonId { get; set; }

        public virtual Person Person { get; set; }
    }
}
