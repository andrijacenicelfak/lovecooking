using Models;

public class ReceptVrati
{
    public int? id { get; set; }
    public int? idoriginala { get; set; }
    public int? Cena { get; set; }
    public double? Ocena { get; set; }
    public int? Kalorije { get; set; }
    public DateTime? Datum { get; set; }
    public String? Naziv { get; set; }
    public String? Opis { get; set; }
    public String? Autor { get; set; }
    public int? idautora { get; set; }

    public Boolean? slideshow { get; set; }
    public List<String>? Slike { get; set; }
    public int? BrojPorcija { get; set; }
    public int VremePripreme { get; set; }
    public String? TipRecepta { get; set; }
    public List<KorakDodaj>? Koraci { get; set; }
    public List<SastojakVrati>? Sastojci { get; set; }
    public String? Pice { get; set; }
}