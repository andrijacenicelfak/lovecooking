using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace Models
{
    [Table("Recept")]

    public class Recept
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public int? BrojPorcija { get; set; }

        [MaxLength(255)]
        public string? Opis { get; set; }

        [MaxLength(30)]
        public string? Naziv { get; set; }

        public int? Cena { get; set; }

        public int? Kalorije { get; set; }

        public int? VremePripremeMinuti { get; set; }

        [Required]
        public DateTime DatumKreiranja { get; set; }

        [Required]
        public Boolean? Prijava { get; set; }
        public Korisnik? Korisnik { get; set; }
        public List<Komentar>? Komentari { get; set; }

        [JsonIgnore]
        public List<Ucestvovanje>? Ucestvovanja { get; set; }
        public List<Korak>? Koraci { get; set; }
        public List<Slika>? Slike { get; set; }
        public Pice? Pice { get; set; }
        public List<Sadrzi>? SadrziSastojke { get; set; }
        [JsonIgnore]
        public List<Jelo>? Jela { get; set; }
        public Boolean? DaLiJeDodatNaSlajd { get; set; }
        public TipRecepta TipRecepta { get; set; }
        public Recept? OriginalniRecept { get; set; }
    }
}