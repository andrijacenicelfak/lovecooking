using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace Models
{
    [Table("Korisnik")]
    public class Korisnik
    {

        [Key]
        public int ID { get; set; }

        [Required]
        [MaxLength(30)]
        public string? Username { get; set; }

        [MaxLength(30)]
        public string? Ime { get; set; }

        [MaxLength(30)]
        public string? Prezime { get; set; }

        [Required]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
        [MaxLength(320)]
        public string? Email { get; set; }

        [Required]
        public byte[]? PasswordHash { get; set; }

        [Required]
        public byte[]? PasswordSalt { get; set; }

        [MaxLength(13)]
        [RegularExpression("\\d+")]
        public string? BrojTelefona { get; set; }

        [MaxLength(255)]
        public string? Opis { get; set; }

        public int? KonformacioniBroj { get; set; }

        [Required]
        public string? Role { get; set; }

        [JsonIgnore]
        public List<Komentar>? Komentari { get; set; }

        [JsonIgnore]
        public List<Prati>? Pratioci { get; set; }

        [JsonIgnore]
        public List<Prati>? Prati { get; set; }
        [JsonIgnore]

        public List<Meni>? Meniji { get; set; }
        public Slika? ProfilaSlika { get; set; }
        [JsonIgnore]

        public List<Ucestvovanje>? Glasanja { get; set; }
    }

}