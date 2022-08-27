using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace Models
{
    [Table("Sastojak")]
    public class Sastojak
    {
        [Key]
        public int ID { get; set; }

        public int? Cena { get; set; }

        public int? Kalorije { get; set; }

        public String? StringZaKalorije { get; set; }

        [NotMapped]
        public double[] KalorijePoTipu
        {
            get
            {
                return Array.ConvertAll(StringZaKalorije!.Split(';'), Double.Parse);
            }
            set
            {
                var _KalorijePoTipu = value;
                StringZaKalorije = String.Join(";", _KalorijePoTipu.Select(p => p.ToString()).ToArray());
            }
        }

        public bool? Odobren { get; set; }

        public String? Naziv { get; set; }

        [JsonIgnore]
        public List<Sadrzi>? SastojakSadrzi { get; set; }
    }
}