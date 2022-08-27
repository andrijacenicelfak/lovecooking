using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    [Table("Prati")]
    public class Prati
    {
        [Key]
        public int ID { get; set; }

        public Korisnik? Pratioc { get; set; }
        public Korisnik? Pracenik { get; set; }
    }
}