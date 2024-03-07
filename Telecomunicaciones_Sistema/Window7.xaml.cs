using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Data;

namespace Telecomunicaciones_Sistema
{
    public partial class Window7 : Window
    {
        public event EventHandler ClienteAgregado;

        public event EventHandler ClienteModificado;

        private List<Clientes> clientes;

        public Clientes NuevoCliente { get; private set; }

        public Window7()
        {
            InitializeComponent();
            Conn = new SqlConnection("Data source = DESKTOP-KIBLMD6\\SQLEXPRESS; Initial catalog = TelecomunicacionesBD; Integrated security = true");
            clientes = new List<Clientes>();
        }

        public Window7(Window2.Clientes clienteSeleccionado)
        {
            InitializeComponent();
            Conn = new SqlConnection("Data source = DESKTOP-KIBLMD6\\SQLEXPRESS; Initial catalog = TelecomunicacionesBD; Integrated security = true");
            clientes = new List<Clientes>();
            this.clienteSeleccionado = clienteSeleccionado;
            MostrarDetallesCliente();
        }

        public Window7(Clientes clienteSeleccionado1)
        {
        }

        private SqlConnection Conn;
        private Window2.Clientes clienteSeleccionado;

        private void BtnAceptar_Click(object sender, RoutedEventArgs e)
        {
            NuevoCliente = new Clientes
            {
                ID_Cliente = txtIDC.Text,
                Nombre = txtNombreC.Text,
                Apellido = txtApellidoC.Text,
                Teléfono = Convert.ToDecimal(txtTelefonoC.Text),
                Correo = txtCorreoC.Text,
                ID_Dirección = txtDireccionC.Text
            };
            try
            {
                Conn.Open();

                SqlCommand cmd = new SqlCommand("NombreProcedimientoAlmacenado", Conn);
                cmd.CommandType = CommandType.StoredProcedure;
                int count = (int)cmd.ExecuteScalar();

                if (count > 0)
                {
                    cmd = new SqlCommand("UPDATE proal_Cliente", Conn);
                }
                else
                {
                    cmd = new SqlCommand("INSERT INTO Clientes (ID_Cliente, Nombre, Apellido, Teléfono, Correo, ID_Dirección) VALUES (@ID_Cliente, @Nombre, @Apellido, @Teléfono, @Correo, @ID_Dirección)", Conn);
                    cmd = new SqlCommand("INSERT INTO proal_Cliente", Conn);
                }

                cmd.Parameters.AddWithValue("@Nombre", NuevoCliente.Nombre);
                cmd.Parameters.AddWithValue("@Apellido", NuevoCliente.Apellido);
                cmd.Parameters.AddWithValue("@Teléfono", NuevoCliente.Teléfono);
                cmd.Parameters.AddWithValue("@Correo", NuevoCliente.Correo);
                cmd.Parameters.AddWithValue("@ID_Dirección", NuevoCliente.ID_Dirección);
                cmd.Parameters.AddWithValue("@ID_Cliente", NuevoCliente.ID_Cliente);
                cmd.ExecuteNonQuery();

                if (count > 0)
                {
                    MessageBox.Show("Cliente modificado correctamente.");
                }
                else
                {
                    MessageBox.Show("Cliente agregado correctamente.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al modificar/agregar el cliente: " + ex.Message);
            }
            finally
            {
                Conn.Close(); 
            }

            OnClienteModificado();

            this.Close();
        }

        private void OnClienteAgregado()
        {
            ClienteAgregado?.Invoke(this, EventArgs.Empty);
        }

        private void GuardarCambios()
        {
            
        }

        private void OnClienteModificado()
        {
            ClienteModificado?.Invoke(this, EventArgs.Empty);
        }

        private void MostrarDetallesCliente()
        {
            txtIDC.Text = clienteSeleccionado.ID_Cliente;
            txtNombreC.Text = clienteSeleccionado.Nombre;
            txtApellidoC.Text = clienteSeleccionado.Apellido;
            txtCorreoC.Text = clienteSeleccionado.Correo;
            txtTelefonoC.Text = clienteSeleccionado.Teléfono;
            txtDireccionC.Text = clienteSeleccionado.ID_Dirección;
        }

        private void BtnRegresar_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();

            Window2 frmPr = new Window2();
        }
    }
}
