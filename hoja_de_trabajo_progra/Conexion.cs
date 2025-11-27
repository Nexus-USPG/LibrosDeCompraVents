using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace hoja_de_trabajo_progra
{
    class Conexion
    {
        private MySqlConnection cnx;

        public Conexion()
        {
            try
            {
                cnx = new MySqlConnection("Server=localhost;Port=3306;Database=libro_compras_ventas;Uid=root;Pwd=SpikeOso;SslMode=Required;TlsVersion=Tls12,Tls13;");

                cnx.Open();

                MessageBox.Show("Conexion Exitosa", "Si compila", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
        public MySqlConnection ObtenerConexion()
        {
            return cnx;
        }
    }

         
    }        
 