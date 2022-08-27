using Models;
public class MeniDodaj
{
    public String? Naziv { get; set; }
    public String? Opis { get; set; }
    public List<ReceptZaMeni>? receptiZaMeni { get; set; }
    public DateTime datumPostavljanja { get; set; }
}