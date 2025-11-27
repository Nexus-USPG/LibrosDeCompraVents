using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hoja_de_trabajo_progra.Dominio
{
    public class Factura
    {
        public string Tipo { get; set; } = "";

        public DateTime FechaEmision { get; set; } = DateTime.Now;

        public string Serie { get; set; } = "";
        public string NumeroDTE { get; set; } = "";

        public string NumeroAutorizacion { get; set; } = "";
        public string NumeroAcceso { get; set; } = "";

        public string Moneda { get; set; } = "GTQ";
        public string NITEmisor { get; set; } = "";
        public string NombreEmisor { get; set; } = "";
        public string DireccionEmisor { get; set; } = "";
        public string NITReceptor { get; set; } = "";
        public string NombreReceptor { get; set; } = "";
        public decimal TotalQ { get; set; } = 0m;
        public decimal Impuestos { get; set; } = 0m;
        public decimal Descuentos { get; set; } = 0m;
        public string FormaPago { get; set; } = "";
    }
}
