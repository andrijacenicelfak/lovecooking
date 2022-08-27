using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace Models
{
    [Table("Komentar")]

    public class Komentar
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [MaxLength(255)]
        public string? Sadrzaj { get; set; }

        public int? Ocena { get; set; }

        [Required]
        public DateTime Datum { get; set; }

        [Required]
        public Boolean? Prijava { get; set; }

        [JsonIgnore]
        public Korisnik? Korisnik { get; set; }

        [JsonIgnore]
        public Recept? Recept { get; set; }
    }
}