using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    [Table("Slika")]
    public class Slika
    {
        [Key]
        public int ID { get; set; }

        [MaxLength(100)]
        public string? Path { get; set; }

        public Boolean? Prijava { get; set; }
    }
}