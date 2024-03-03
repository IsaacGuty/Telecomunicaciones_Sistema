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

        private List<Clientes> clientes;

        public Clientes NuevoCliente { get; private set; }

        public Window7()
        {
            InitializeComponent();
            Conn = new SqlConnection("Data source = DESKTOP-KIBLMD6\\SQLEXPRESS; Initial catalog = TelecomunicacionesBD; Integrated security = true");
            clientes = new List<Clientes>();
        }

        private SqlConnection Conn;

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
                    SqlCommand cmd = new SqlCommand("INSERT INTO Clientes (ID_Cliente, Nombre, Apellido, Teléfono, Correo, ID_Dirección) VALUES (@ID_Cliente, @Nombre, @Apellido, @Teléfono, @Correo, @ID_Dirección)", Conn);
                    cmd.Parameters.AddWithValue("@ID_Cliente", NuevoCliente.ID_Cliente);
                    cmd.Parameters.AddWithValue("@Nombre", NuevoCliente.Nombre);
                    cmd.Parameters.AddWithValue("@Apellido", NuevoCliente.Apellido);
                    cmd.Parameters.AddWithValue("@Teléfono", NuevoCliente.Teléfono);
                    cmd.Parameters.AddWithValue("@Correo", NuevoCliente.Correo);
                    cmd.Parameters.AddWithValue("@ID_Dirección", NuevoCliente.ID_Dirección);
                    cmd.ExecuteNonQuery();
                MessageBox.Show("Cliente agregado correctamente.");
            }
            catch (Exception ex)
            {
                 MessageBox.Show("Error al agregar el cliente: " + ex.Message);
            }
            OnClienteAgregado();

            this.Close();
        }

        private void OnClienteAgregado()
        {
            ClienteAgregado?.Invoke(this, EventArgs.Empty);
        }

    }
}
