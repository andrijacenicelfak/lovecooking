using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace Models
{
    [Table("Jelo")]
    public class Jelo
    {
        [Key]
        public int ID { get; set; }
        public TipJela TipJela { get; set; }
        public Recept? Recept { get; set; }

        public Meni? Meni { get; set; }
    }
}