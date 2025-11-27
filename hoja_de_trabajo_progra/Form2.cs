using hoja_de_trabajo_progra; // Para usar la clase Factura
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO; // Necesario para Exportar a CSV
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace hoja_de_trabajo_progra
{
    public partial class Form2 : Form
    {
        private DataTable dtVentas = new DataTable();
        private DataTable dtCompras = new DataTable();

        public Form2()
        {
            InitializeComponent();
            InicializarDataGrids();
            CargarPorcentajeImpuesto();
            CargarFacturasMes();
        }


        // 💡 AGREGADO/MODIFICADO
        private void InicializarDataGrids()
        {
            // Función auxiliar para inicializar la estructura de una grilla
            Action<DataGridView> configurarGrid = (dgv) =>
            {
                if (dgv.Columns.Count == 0)
                {
                    dgv.Columns.Add("Fecha", "Fecha");
                    dgv.Columns.Add("Serie", "Serie");
                    dgv.Columns.Add("DTE", "Número DTE");
                    dgv.Columns.Add("Nit", "NIT");
                    dgv.Columns.Add("Nombre", "Nombre");
                    dgv.Columns.Add("Moneda", "Moneda");
                    dgv.Columns.Add("TotalQ", "Total");
                    dgv.Columns.Add("Observaciones", "Observaciones");

                    // 💡 CORRECCIÓN APLICADA: 
                    // 1. Agregar la columna (esto devuelve el índice)
                    dgv.Columns.Add("Tipo", "Tipo");

                    // 2. Acceder a la columna por su nombre para establecer la propiedad Visible
                    dgv.Columns["Tipo"].Visible = false;
                }
            };

            // Aplicar configuración a los DataGridViews
            configurarGrid(dgvVenta); // ASUMO la existencia de dgvVenta
            configurarGrid(dgvCompra);
        }

        private decimal porcentajeImpuesto = 0.00m;

        private void CargarDatosEnGrid(DataGridView dgv, string tipo, int mes, int anio)
        {
            dgv.Rows.Clear();

            // 💡 MODIFICADO: Consulta para obtener solo un tipo de factura
            string sql = "SELECT * FROM facturas WHERE tipo = @tipo AND MONTH(fecha_emision) = @mes AND YEAR(fecha_emision) = @anio";

            try
            {
                Conexion cnx = new Conexion();
                using (MySqlCommand cmd = new MySqlCommand(sql, cnx.ObtenerConexion()))
                {
                    cmd.Parameters.AddWithValue("@tipo", tipo);
                    cmd.Parameters.AddWithValue("@mes", mes);
                    cmd.Parameters.AddWithValue("@anio", anio);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // La lógica para determinar qué NIT/Nombre usar depende de si es VENTA o COMPRA.
                            // Para VENTAS: se muestra NIT_Receptor y Nombre_Receptor.
                            // Para COMPRAS: se muestra NIT_Emisor y Nombre_Emisor.
                            string nit = tipo == "VENTA" ? reader["nit_receptor"].ToString() : reader["nit_emisor"].ToString();
                            string nombre = tipo == "VENTA" ? reader["nombre_receptor"].ToString() : reader["nombre_emisor"].ToString();

                            dgv.Rows.Add(
                                reader.GetDateTime("fecha_emision").ToShortDateString(),
                                reader["serie"].ToString(),
                                reader["dte"].ToString(),
                                nit,
                                nombre,
                                reader["moneda"].ToString(),
                                reader["total"].ToString(),
                                reader["observaciones"].ToString(),
                                tipo
                            );
                        }
                    }
                }
            }
            catch (MySqlException mysqlEx)
            {
                MessageBox.Show($"Error al cargar facturas de {tipo}: " + mysqlEx.Message, "Error MySQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error general al cargar facturas de {tipo}: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarPorcentajeImpuesto()
        {
            string sql = "SELECT porcentaje_impuesto FROM configuraciones WHERE activo = 1 ORDER BY vigente_desde DESC LIMIT 1";
            try
            {
                Conexion cnx = new Conexion();
                using (MySqlCommand cmd = new MySqlCommand(sql, cnx.ObtenerConexion()))
                {
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {

                        porcentajeImpuesto = Convert.ToDecimal(result);
                    }
                }
            }
            catch (MySqlException mysqlEx)
            {
                MessageBox.Show("Error al cargar configuración: " + mysqlEx.Message, "Error MySQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error general al cargar configuración: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarFacturasMes()
        {
            int mes = dateTimePicker1.Value.Month;
            int anio = dateTimePicker1.Value.Year;

            // 1. Cargar Libro de Ventas en dgvVenta
            // Esto debería cargar SOLO VENTAS
            CargarDatosEnGrid(dgvVenta, "VENTA", mes, anio);

            // 2. Cargar Libro de Compras en dgvCompra
            // Esto debería cargar SOLO COMPRAS
            CargarDatosEnGrid(dgvCompra, "COMPRA", mes, anio);

            GenerarReporteMensual();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            CargarFacturasMes();

            GenerarReporteMensual();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string serie = txtSerie.Text.Trim();
            string DTE = txtDTE.Text.Trim();
            string autorizacion = txtAutorizacion.Text.Trim(); // 💡 ASUMO el nombre del control
            string acceso = txtAcceso.Text.Trim(); // 💡 ASUMO el nombre del control
            string NitEmisor = txtNitEmisor.Text.Trim();
            string direEmisor = txtDireccionEmisor.Text.Trim();
            string NitReceptor = txtNitReceptor.Text.Trim();
            string nombreReceptor = txtNombreReceptor.Text.Trim();
            string NombreEmisor = txtNombreEmisor.Text.Trim();
            string observaciones = txtObservaciones.Text.Trim();

            // 💡 ASUMO un TextBox para el Total (obligatorio en DB). Si no existe, debes crearlo.
            string total_str = txtTotal.Text.Trim(); // ASUMO txtTotal
            string impuestos_str = txtImpuestos.Text.Trim();
            string descuentos_str = txtDescuentos.Text.Trim();

            // Validar ComboBoxes y RadioButtons
            if (cmbMoneda.SelectedItem == null || cmbPago.SelectedItem == null || (!radioButton1.Checked && !radioButton2.Checked))
            {
                MessageBox.Show("Por favor, complete todos los campos obligatorios (Moneda, Pago, Tipo de Factura).", "Error de Entrada", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 💡 MODIFICADO: Uso de TryParse para datos numéricos y decimal para precisión
            if (!decimal.TryParse(total_str, out decimal total))
            {
                MessageBox.Show("El campo Total debe ser un valor numérico válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            decimal impuestos = decimal.TryParse(impuestos_str, out decimal imp) ? imp : 0.00m;
            decimal descuentos = decimal.TryParse(descuentos_str, out decimal desc) ? desc : 0.00m;

            string moneda = cmbMoneda.SelectedItem.ToString(); // Esto debe ser GTQ, USD, EUR (3 caracteres)
            string pago = cmbPago.SelectedItem.ToString();

            // Determinar el tipo de factura (VENTA o COMPRA)
            string tipoFactura;
            if (radioButton2.Checked) // 💡 ASUMO que el RadioButton Venta se llama radioButtonVenta
            {
                tipoFactura = "VENTA";
            }
            else if (radioButton1.Checked) // 💡 ASUMO que el RadioButton Compra se llama radioButtonCompra
            {
                tipoFactura = "COMPRA";
            }
            else
            {
                // Esta validación ya se hizo arriba, pero por seguridad
                MessageBox.Show("Debe seleccionar si es una Venta o una Compra.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DateTime fechaEmision = dateTimePicker1.Value.Date; // Obtener la fecha del control

            // --- 2. Lógica de Inserción en la DB ---
            string sql = @"
        INSERT INTO facturas 
        (tipo, fecha_emision, serie, dte, numero_autorizacion, numero_acceso, moneda, 
         nit_emisor, nombre_emisor, direccion_emisor, nit_receptor, nombre_receptor, 
         total, impuestos, descuentos, forma_pago, observaciones)
        VALUES 
        (@tipo, @fecha_emision, @serie, @dte, @autorizacion, @acceso, @moneda, 
         @nit_emisor, @nombre_emisor, @direccion_emisor, @nit_receptor, @nombre_receptor, 
         @total, @impuestos, @descuentos, @pago, @observaciones)";

            try
            {
                Conexion cnx = new Conexion();
                using (MySqlCommand cmd = new MySqlCommand(sql, cnx.ObtenerConexion()))
                {
                    cmd.Parameters.AddWithValue("@tipo", tipoFactura);
                    cmd.Parameters.AddWithValue("@fecha_emision", fechaEmision);
                    cmd.Parameters.AddWithValue("@serie", serie);
                    cmd.Parameters.AddWithValue("@dte", DTE);
                    cmd.Parameters.AddWithValue("@autorizacion", autorizacion);
                    cmd.Parameters.AddWithValue("@acceso", acceso);
                    cmd.Parameters.AddWithValue("@moneda", moneda);
                    cmd.Parameters.AddWithValue("@nit_emisor", NitEmisor);
                    cmd.Parameters.AddWithValue("@nombre_emisor", NombreEmisor);
                    cmd.Parameters.AddWithValue("@direccion_emisor", direEmisor);
                    cmd.Parameters.AddWithValue("@nit_receptor", NitReceptor);
                    cmd.Parameters.AddWithValue("@nombre_receptor", nombreReceptor);
                    cmd.Parameters.AddWithValue("@total", total);
                    cmd.Parameters.AddWithValue("@impuestos", impuestos);
                    cmd.Parameters.AddWithValue("@descuentos", descuentos);
                    cmd.Parameters.AddWithValue("@pago", pago);
                    cmd.Parameters.AddWithValue("@observaciones", observaciones);

                    int filasAfectadas = cmd.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                    {
                        MessageBox.Show("Factura registrada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // 💡 AGREGADO: Limpiar campos de entrada
                        LimpiarCampos();

                        // 💡 AGREGADO: Recargar ambas grillas para mostrar el nuevo registro en su lugar correcto
                        CargarFacturasMes();
                    }
                    else
                    {
                        MessageBox.Show("No se pudo registrar la factura.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (MySqlException mysqlEx)
            {
                MessageBox.Show("Error de MySQL al registrar factura: " + mysqlEx.Message, "Error MySQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error general al registrar factura: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LimpiarCampos()
        {
            // Limpia los TextBoxes principales
            txtSerie.Clear();
            txtDTE.Clear();
            txtAutorizacion.Clear(); // ASUMO nombre del control
            txtAcceso.Clear();      // ASUMO nombre del control

            // Datos de Emisor y Receptor
            txtNitEmisor.Clear();
            txtNombreEmisor.Clear();
            txtDireccionEmisor.Clear();
            txtNitReceptor.Clear();
            txtNombreReceptor.Clear();

            // Totales y Observaciones
            txtTotal.Clear();             // ASUMO txtTotal
            txtImpuestos.Clear();
            txtDescuentos.Clear();
            txtObservaciones.Clear();

            // Opcional: Deseleccionar ComboBoxes y RadioButtons si es necesario
            // cmbMoneda.SelectedIndex = -1;
            // cmbPago.SelectedIndex = -1;
            // radioButtonVenta.Checked = false;
            // radioButtonCompra.Checked = false;
        }

        private void GenerarReporteMensual()
        {

            int mes = dateTimePicker1.Value.Month;
            int anio = dateTimePicker1.Value.Year;

            // 2. Total de ventas del mes
            decimal totalVentas = CalcularTotalPorTipo("VENTA", mes, anio);
            // 3. Total de compras del mes
            decimal totalCompras = CalcularTotalPorTipo("COMPRA", mes, anio);

            // 4. Porcentaje de impuesto: ya está cargado en 'porcentajeImpuesto'

            // 5. Impuesto del Mes: ImpuestoDelMes = TotalVentasDelMes × (PorcentajeImpuesto / 100)
            // 💡 AGREGADO: Variable para el cálculo del impuesto
            decimal impuestoDelMes = totalVentas * (porcentajeImpuesto / 100);

            // 6. Declaración/Pago: Mes inmediatamente siguiente
            // 💡 AGREGADO: Cálculo del mes de declaración
            DateTime fechaDeclaracion = new DateTime(anio, mes, 1).AddMonths(1);
            string declaracionEn = fechaDeclaracion.ToString("MMMM yyyy"); // Eje: noviembre 2025

            // --- Mostrar en la UI (Se asume la existencia de Labels/TextBoxes para el resumen) ---
            // 💡 AGREGADO: Se asume que existen controles (ej. txtResumenVentas, txtResumenCompras, etc.)
            // lblMesAnio.Text = $"{new DateTime(anio, mes, 1).ToString("MMMM")} {anio}"; // Eje: octubre 2025
            // txtTotalVentasQ.Text = totalVentas.ToString("N2");
            // txtTotalComprasQ.Text = totalCompras.ToString("N2");
            // txtPorcentajeImpuesto.Text = porcentajeImpuesto.ToString("N2") + "%";
            // txtImpuestoDelMesQ.Text = impuestoDelMes.ToString("N2");
            // txtDeclaracionEn.Text = declaracionEn; 

            // Mostrar Totales en el DataGrid (asumiendo que dgvCompra muestra la lista)
            // Puedes agregar filas de totales al final del dgvCompra o usar otros controles.
            // Ejemplo de totales al pie:
            // dgvCompra.Rows.Add("", "", "", "", "TOTAL VENTAS:", "", totalVentas.ToString("N2"), "");
            // dgvCompra.Rows.Add("", "", "", "", "TOTAL COMPRAS:", "", totalCompras.ToString("N2"), "");

            // 💡 AGREGADO: También se debe generar el contenido para exportar a CSV si es necesario.
        }
        private decimal CalcularTotalPorTipo(string tipo, int mes, int anio)
        {
            // 💡 MODIFICADO: Consulta para SUMAR el campo total
            string sql = "SELECT SUM(total) FROM facturas WHERE tipo = @tipo AND MONTH(fecha_emision) = @mes AND YEAR(fecha_emision) = @anio";
            decimal total = 0.00m;

            try
            {
                Conexion cnx = new Conexion();
                using (MySqlCommand cmd = new MySqlCommand(sql, cnx.ObtenerConexion()))
                {
                    cmd.Parameters.AddWithValue("@tipo", tipo);
                    cmd.Parameters.AddWithValue("@mes", mes);
                    cmd.Parameters.AddWithValue("@anio", anio);

                    object result = cmd.ExecuteScalar();
                    if (result != DBNull.Value && result != null)
                    {
                        total = Convert.ToDecimal(result);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al calcular total de {tipo}: {ex.Message}", "Error de Cálculo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return total;
        }

        private void ExportarALibroCSV(string tipo)
        {
            // Se asume la existencia de un botón (ej. btnExportarVentas) que llama a este método.

            // 1. Obtener los datos del mes/año
            int mes = dateTimePicker1.Value.Month;
            int anio = dateTimePicker1.Value.Year;
            string sql = "SELECT fecha_emision, serie, dte, nit_receptor, nombre_receptor, moneda, total, observaciones, nit_emisor, nombre_emisor FROM facturas WHERE tipo = @tipo AND MONTH(fecha_emision) = @mes AND YEAR(fecha_emision) = @anio";

            try
            {
                Conexion cnx = new Conexion();
                using (MySqlCommand cmd = new MySqlCommand(sql, cnx.ObtenerConexion()))
                {
                    cmd.Parameters.AddWithValue("@tipo", tipo);
                    cmd.Parameters.AddWithValue("@mes", mes);
                    cmd.Parameters.AddWithValue("@anio", anio);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        // 2. Definir el encabezado del CSV
                        StringBuilder sb = new StringBuilder();
                        if (tipo == "VENTA")
                        {
                            sb.AppendLine("Fecha,Serie,NumeroDTE,NITReceptor,NombreReceptor,Moneda,TotalQ,Observaciones");
                        }
                        else // COMPRA
                        {
                            sb.AppendLine("Fecha,Serie,NumeroDTE,NITEmisor,NombreEmisor,Moneda,TotalQ,Observaciones");
                        }

                        // 3. Llenar las filas
                        while (reader.Read())
                        {
                            string nit = tipo == "VENTA" ? reader["nit_receptor"].ToString() : reader["nit_emisor"].ToString();
                            string nombre = tipo == "VENTA" ? reader["nombre_receptor"].ToString() : reader["nombre_emisor"].ToString();

                            sb.AppendLine(
                                $"{reader.GetDateTime("fecha_emision").ToShortDateString()}," +
                                $"{reader["serie"]}," +
                                $"{reader["dte"]}," +
                                $"{nit}," +
                                $"{nombre}," +
                                $"{reader["moneda"]}," +
                                $"{reader["total"]}," +
                                $"\"{reader["observaciones"]}\"" // Usar comillas para observaciones con comas
                            );
                        }

                        // 4. Agregar el total al final
                        decimal total = CalcularTotalPorTipo(tipo, mes, anio);
                        sb.AppendLine($",,,,,TOTAL: {total},");


                        // 5. Guardar el archivo (se necesita un SaveFileDialog)
                        SaveFileDialog saveDialog = new SaveFileDialog
                        {
                            Filter = "CSV files (*.csv)|*.csv",
                            FileName = $"Libro_de_{tipo}_{mes}_{anio}.csv"
                        };

                        if (saveDialog.ShowDialog() == DialogResult.OK)
                        {
                            System.IO.File.WriteAllText(saveDialog.FileName, sb.ToString(), Encoding.UTF8);
                            MessageBox.Show($"Libro de {tipo} exportado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al exportar a CSV: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            CargarFacturasMes();
            GenerarReporteMensual();
        }

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
            // Esto llamará a dateTimePicker1_ValueChanged para recargar.
        }

    }
}