using System;
using System.Collections.Generic;
using Models;
namespace LoveCooking
{
    public class MeniSortiranje : IComparer<Meni>
    {
        public int Compare(Meni? x, Meni? y)
        {
            if (x == null || y == null) return 0;
            return x.DatumPostavljanja.CompareTo(y.DatumPostavljanja) * (-1);
        }
    }
}