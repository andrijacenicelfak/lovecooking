using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    [Table("Glasanje")]
    public class Glasanje
    {
        //spoj
        public List<Korisnik>? Korisnici { get; set; }
        public Ucestvovanje? Ucestvovanje { get; set; }
    }
}