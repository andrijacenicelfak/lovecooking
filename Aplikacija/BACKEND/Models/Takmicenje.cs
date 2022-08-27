using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace Models
{
    [Table("Takmicenje")]
    public class Takmicenje
    {
        [Key]
        public int ID { get; set; }

        [MaxLength(30)]
        [Required]
        public string? Naziv { get; set; }

        [MaxLength(255)]
        public string? Opis { get; set; }

        public DateTime DatumOd { get; set; }

        public DateTime DatumDo { get; set; }

        [JsonIgnore]
        public List<Ucestvovanje>? Ucestvovanja { get; set; }

        public Recept? Pobednik { get; set; }

        public Slika? Slika { get; set; }

        public Boolean Zavrseno { get; set; }
    }
}