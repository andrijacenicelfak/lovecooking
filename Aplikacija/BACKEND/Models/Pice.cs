using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace Models
{
    [Table("Pice")]
    public class Pice
    {
        [Key]
        public int ID { get; set; }

        [MaxLength(30)]
        [Required]
        public string? Naziv { get; set; }

        public int? Cena { get; set; }

        public int? Kalorije { get; set; }

        public int? ProcenatAlkohola { get; set; }

        [JsonIgnore]
        public List<Recept>? Recepti { get; set; }
    }
}