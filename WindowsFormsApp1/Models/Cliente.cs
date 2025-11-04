using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1.Models
{
    public class Cliente
    {
        public decimal Id { get; set; }
        public string Nombre { get; set; }
        public string Telefono { get; set; }  // opcional
        public string Correo { get; set; }    // opcional
    }
}
