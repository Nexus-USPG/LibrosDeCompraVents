using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
            string fecha = dateTimePicker1.Text;
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

            if (radioButton1.Checked)
            {
                dgvCompra.Rows.Add(fecha, serie, DTE, NitReceptor, nombreReceptor, moneda, observaciones);
            }

            if(radioButton2.Checked)
            {
                
                dgvCompra.Rows.Add(fecha, serie, DTE, NitEmisor, NombreEmisor,moneda,observaciones);
            }
        }
    }
}
