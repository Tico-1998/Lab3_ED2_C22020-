using System;

namespace Lab_3.Models
{
    public class nodoarbol : IComparable
    {
        public byte caracter { get; set; }
        public double Frecuencia { get; set; }
        public nodoarbol nodoizq { get; set; }
        public nodoarbol nododer { get; set; }

        public int CompareTo(object obj)
        {
            return Frecuencia.CompareTo(((nodoarbol)obj).Frecuencia);
        }
    }
}