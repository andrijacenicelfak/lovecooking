
namespace LoveCooking
{

    public class TakmicenjeVrati
    {
        public int id { get; set; }
        public String? naziv { get; set; }
        public String? opis { get; set; }
        public DateTime datumOd { get; set; }
        public DateTime datumDo { get; set; }
        public String? slikaPath { get; set; }

        public int? pobednik { get; set; }

    }
}