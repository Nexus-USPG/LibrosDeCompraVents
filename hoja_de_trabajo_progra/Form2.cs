using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace hoja_de_trabajo_progra
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int mesActual = dateTimePicker1.Value.Month;
            string nombreMes = dateTimePicker1.Value.ToString("MMMM");

            string serie = txtSerie.Text;
            string DTE = txtDTE.Text;
            string autorizacion = txtAutorizacion.Text;
            string acceso = txtAcceso.Text;
            string NitEmisor = txtNitEmisor.Text;
            string direEmisor = txtDireccionEmisor.Text;
            string NitReceptor = txtNitReceptor.Text;
            string nombreReceptor = txtNombreReceptor.Text;
            string impuestos = txtImpuestos.Text;
            string descuentos = txtDescuentos.Text;
            string observaciones = txtObservaciones.Text;
            string NombreEmisor = txtNombreEmisor.Text;
            double parcial = 0;

            string moneda = cmbMoneda.SelectedItem.ToString();
            string pago = cmbPago.SelectedItem.ToString();

            private void CambiarMes(int nuevoMes)
            {
                DateTime fechaActual = dateTimePicker1.Value;
            try
                {
                DateTime nuevaFecha = new DateTime(fechaActual.Year, nuevoMes, fechaActual.Day);
                dateTimePicker1.Value = nuevaFecha;
            }
            catch (ArgumentOutOfRangeException)
            {
                DateTime nuevaFechaSegura = new DateTime(fechaActual.Year, nuevoMes, 1);
                dateTimePicker1.Value = nuevaFechaSegura;
            }
                

            if (radioButton1.Checked)
            {
                dgvCompra.Rows.Add(fecha, serie, DTE, NitReceptor, nombreReceptor, moneda, observaciones);
            }

            if(radioButton2.Checked)
            {
                
                DateTime fecha = dateTimePicker1.Value;
                dgvCompra.Rows.Add(fecha, serie, DTE, NitEmisor, NombreEmisor, moneda, observaciones);
            }
        }
    }
}
