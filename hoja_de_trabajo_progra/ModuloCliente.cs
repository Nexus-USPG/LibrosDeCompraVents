using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace hoja_de_trabajo_progra
{
    
    public partial class ModuloCliente : Form
    {
        public ModuloCliente()
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

            string sql = "INSERT INTO clientes (nombre, Nit, telefono) VALUES (@nombre, @Nit, @telefono)";
            try
            {
                Conexion cnx = new Conexion();
                using (MySqlCommand cmd = new MySqlCommand(sql, cnx.ObtenerConexion()))
                {
                    cmd.Parameters.AddWithValue("@nombre", nombre);
                    cmd.Parameters.AddWithValue("@Nit", Nit);
                    cmd.Parameters.AddWithValue("@telefono", telefono);
                    int filasAfectadas = cmd.ExecuteNonQuery();
                    if (filasAfectadas > 0)
                    {
                        MessageBox.Show("Registro insertado correctamente.", "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        textBox1.Clear();
                        textBox2.Clear();
                        textBox3.Clear();
                    }
                    else
                    {
                        MessageBox.Show("No se pudo insertar el registro.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (MySqlException mysqlEx)
            {
                MessageBox.Show("Error de Conexion: " + mysqlEx.Message, "Error de MySQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error de Conexion: " + ex.Message, "Error Humano", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string nombre = textBox1.Text.Trim();
            string Nit = textBox2.Text.Trim();
            string telefono = textBox3.Text.Trim();

            List<string> condiciones = new List<string>();

            if (!string.IsNullOrEmpty(nombre))
            {
                condiciones.Add("nombre = '" + nombre + "'");
            }

            if (!string.IsNullOrEmpty(Nit))
            {
                condiciones.Add("Nit = '" + Nit + "'");
            }

            if (!string.IsNullOrEmpty(telefono) && int.TryParse(telefono, out int tel))
            {
                condiciones.Add("telefono = " + tel);
            }

            if (condiciones.Count == 0)
            {
                MessageBox.Show("Por favor ingrese al menos un criterio de busqueda.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string sql = "SELECT Nit, nombre, telefono FROM clientes WHERE " + string.Join(" AND ", condiciones);

            try
            {
                Conexion cnx = new Conexion();
                using (MySqlCommand cmd = new MySqlCommand(sql, cnx.ObtenerConexion()))
                {
                    if (!string.IsNullOrEmpty(nombre))
                    {
                        cmd.Parameters.AddWithValue("@nombre", nombre);
                    }
                    if (!string.IsNullOrEmpty(Nit))
                    {
                        cmd.Parameters.AddWithValue("@Nit", Nit);
                    }
                    if (!string.IsNullOrEmpty(telefono) && int.TryParse(telefono, out int tele))
                    {
                        cmd.Parameters.AddWithValue("@telefono", tele);
                    }

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            textBox1.Clear();
                            textBox2.Clear();
                            textBox3.Clear();

                            if (reader.Read())
                            {
                                textBox1.Text = reader["nombre"].ToString();
                                textBox2.Text = reader["Nit"].ToString();
                                textBox3.Text = reader["telefono"].ToString();
                            }
                        }
                        else
                        {
                            MessageBox.Show("No se encontraron registros que coincidan con los criterios de busqueda.", "Sin Resultados", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (MySqlException mysqlEx)
            {
                MessageBox.Show("Error de Conexion: " + mysqlEx.Message, "Error de MySQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error de Conexion: " + ex.Message, "Error Humano", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        

        private void Form1_Load(object sender, EventArgs e)
        {
            Conexion con = new Conexion();
        }
    }
}
