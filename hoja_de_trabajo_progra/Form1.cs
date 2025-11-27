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
            string sql ="SELECT * FROM clientes WHERE Nit='" + bus + "'";
            List<string> lista = new List<string>();
            if(!string.IsNullOrEmpty(bus))
            {
                Conexion con = new Conexion();
                SqlCommand cmd = new SqlCommand(sql, con.ObtenerConexion());
                SqlDataReader reader = cmd.ExecuteReader();
                if(reader.HasRows)
                {
                    while(reader.Read())
                    {
                        lista.Add(reader.GetString(0));
                        lista.Add(reader.GetString(1));
                        lista.Add(reader.GetString(2));
                    }
                    textBox1.Text = lista[1];
                    textBox2.Text = lista[0];
                    textBox3.Text = lista[2];
                }
                else
                {
                    MessageBox.Show("No se encontraron registros");
                }
            }
            else
            {
                MessageBox.Show("Ingrese un Nit valido");
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Conexion con = new Conexion();
        }
    }
}
