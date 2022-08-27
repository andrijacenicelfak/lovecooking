using Models;
namespace LoveCooking

{
    public class Funkcije
    {
        public static int konverzijaKalorija(Sastojak sastojak, TipJedinice tipJedinice, int kolicina)
        {
            int kalorije = 0;
            switch (tipJedinice)
            {
                case TipJedinice.g:
                    kalorije = (int)sastojak!.Kalorije! / 100 * kolicina;
                    break;
                case TipJedinice.dl:
                    kalorije = (int)sastojak.Kalorije! * 2 * kolicina;
                    break;
                case TipJedinice.kasicica:
                    kalorije = (int)sastojak.Kalorije! * 9 * kolicina;
                    break;
                case TipJedinice.kg:
                    kalorije = (int)sastojak.Kalorije! / 10 * kolicina;
                    break;
                case TipJedinice.kom:
                    kalorije = (int)sastojak.Kalorije! * 2 * kolicina;
                    break;
                case TipJedinice.l:
                    kalorije = (int)sastojak.Kalorije! / 10 * kolicina;
                    break;
                case TipJedinice.ml:
                    kalorije = (int)sastojak.Kalorije! / 100 * kolicina;
                    break;
                case TipJedinice.po_potrebi:
                    kalorije = (int)sastojak.Kalorije! / 1 * 0;
                    break;
                case TipJedinice.prstohvat:
                    kalorije = (int)sastojak.Kalorije! / 200 * kolicina;
                    break;
                case TipJedinice.solja:
                    kalorije = (int)sastojak.Kalorije! * 2 * kolicina;
                    break;
                case TipJedinice.supena_kasika:
                    kalorije = (int)sastojak.Kalorije! / 100 * 15 * kolicina;
                    break;
                default:
                    kalorije = 0;
                    break;
            }

            return kalorije;
        }

        public static double konverzijaCene(Sastojak sastojak, TipJedinice tipJedinice, int kolicina)
        {
            double cena = 0;
            switch (tipJedinice)
            {
                case TipJedinice.g:
                    cena = ((int)sastojak!.Cena!) / 1000.0 * kolicina;
                    break;
                case TipJedinice.dl:
                    cena = ((int)sastojak!.Cena!) / 10.0 * kolicina;
                    break;
                case TipJedinice.kasicica:
                    cena = ((int)sastojak!.Cena!) / 1000.0 * 5 * kolicina;
                    break;
                case TipJedinice.kg:
                    cena = ((int)sastojak!.Cena!) * kolicina;
                    break;
                case TipJedinice.kom:
                    cena = ((int)sastojak!.Cena!) / 20.0 * kolicina;
                    break;
                case TipJedinice.l:
                    cena = ((int)sastojak!.Cena!) * 1.0 * kolicina;
                    break;
                case TipJedinice.ml:
                    cena = ((int)sastojak!.Cena!) / 1000.0 * kolicina;
                    break;
                case TipJedinice.po_potrebi:
                    cena = 0;
                    break;
                case TipJedinice.prstohvat:
                    cena = 0;
                    break;
                case TipJedinice.solja:
                    cena = ((int)sastojak!.Cena!) / 50.0 * kolicina;
                    break;
                case TipJedinice.supena_kasika:
                    cena = ((int)sastojak!.Cena!) / 1000.0 * 15 * kolicina;
                    break;
                default:
                    cena = 0;
                    break;
            }

            return cena;
        }
    }
}
