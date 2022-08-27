using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    [Table("Meni")]

    public class Meni
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [MaxLength(30)]
        public string? Naziv { get; set; }

        [MaxLength(255)]
        public string? Opis { get; set; }

        public int? Kalorije { get; set; }

        public Boolean? Prijava { get; set; }

        public List<Jelo>? Jela { get; set; }
        public Korisnik? Korisnik { get; set; }

        [Required]
        public DateTime DatumPostavljanja { get; set; }
    }
}
