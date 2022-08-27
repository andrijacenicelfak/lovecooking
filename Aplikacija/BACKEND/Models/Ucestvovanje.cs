using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    [Table("Ucestvovanje")]
    public class Ucestvovanje
    {
        [Key]
        public int? ID { get; set; }
        public List<Korisnik>? Glasanja { get; set; }
        public Recept? Recept { get; set; }
        public Takmicenje? Takmicenje { get; set; }
    }
}