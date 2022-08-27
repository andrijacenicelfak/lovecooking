using System;
using System.Collections.Generic;
using Models;
namespace LoveCooking
{
    public class ReceptSortiranje : IComparer<Recept>
    {

        public int Compare(Recept? x, Recept? y)
        {
            if (x == null || y == null) return 0;
            return x.DatumKreiranja.CompareTo(y.DatumKreiranja) * (-1);
        }
    }
}