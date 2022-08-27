using Models;

public class ReceptDodaj
{
    public String? Naziv { get; set; }
    public String? Opis { get; set; }
    public List<String>? Slike { get; set; }
    public int? brojPoricja { get; set; }
    public int VremePripreme { get; set; }
    public TipRecepta TipRecepta { get; set; }
    public List<KorakDodaj>? Koraci { get; set; }
    public List<SastojakDodaj>? Sastojci { get; set; }
    public int? PiceID { get; set; }
    public int? OriginalniID { get; set; }

}