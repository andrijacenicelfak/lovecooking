using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace Models
{
    [Table("Korak")]
    public class Korak
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public int? BrojKoraka { get; set; }

        [MaxLength(255)]
        public string? Opis { get; set; }

        [JsonIgnore]
        public Recept? Recept { get; set; }
        public Slika? Slike { get; set; }
    }
}
