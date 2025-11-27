using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace hoja_de_trabajo_progra
{
    
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string nombre = textBox1.Text;
            string Nit = textBox2.Text;
            int telefono = int.Parse(textBox3.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string bus = textBox2.Text;

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Conexion con = new Conexion();
        }
    }
}
