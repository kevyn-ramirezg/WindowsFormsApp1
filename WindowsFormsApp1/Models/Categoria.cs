using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1.Models
{
    public class Categoria
    {
        public decimal Id { get; set; }
        public string Nombre { get; set; }
        public decimal Iva { get; set; }           // porcentaje 0..100
        public decimal UtilidadPct { get; set; }   // porcentaje 0..100
    }
}
