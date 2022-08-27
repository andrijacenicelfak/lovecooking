using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Models
{
    [Table("Sadrzi")]
    public class Sadrzi
    {
        [Key]
        public int ID { get; set; }
        public int? Kolicina { get; set; }
        public TipJedinice TipJedinice { get; set; }

        [JsonIgnore]
        public Recept? Recept { get; set; }
        public Sastojak? Sastojci { get; set; }
    }
}