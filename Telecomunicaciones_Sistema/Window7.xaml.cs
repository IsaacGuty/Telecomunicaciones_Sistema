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
        // Eventos para notificar cuando se agrega o modifica un cliente
        public event EventHandler ClienteAgregado;
        public event EventHandler ClienteModificado;

        // Lista para almacenar clientes (aunque no se utiliza en este fragmento de código)
        private List<Clientes> clientes;

        // Propiedad para acceder al nuevo cliente desde fuera de la clase
        public Clientes NuevoCliente { get; private set; }

        // Variable para indicar si se está modificando un cliente existente
        private bool esModificacion;

        // Constructor para la ventana de agregar o modificar cliente
        public Window7(bool esModificacion)
        {
            InitializeComponent();
            this.esModificacion = esModificacion;
            if (esModificacion)
            {
                lblNom.Content = "Modificar cliente"; // Cambia el título de la ventana si se está modificando
            }
            else
            {
                lblNom.Content = "Agregar un nuevo cliente"; // Cambia el título de la ventana si se está agregando
            }
        }

        // Constructor sobrecargado para la ventana de agregar o modificar cliente, recibiendo el cliente a modificar
        public Window7(Window2.Clientes clienteSeleccionado, bool esModificacion)
        {
            InitializeComponent();
            this.esModificacion = esModificacion;
            Conn = new SqlConnection("Data source = DESKTOP-KIBLMD6\\SQLEXPRESS; Initial catalog = TelecomunicacionesBD; Integrated security = true");
            clientes = new List<Clientes>();
            this.clienteSeleccionado = clienteSeleccionado;
            MostrarDetallesCliente(); // Muestra los detalles del cliente a modificar en los campos correspondientes
            ActualizarLabel();
        }

        // Método para actualizar el contenido de la etiqueta según si se está modificando o agregando un cliente
        private void ActualizarLabel()
        {
            if (esModificacion)
            {
                lblNom.Content = "Modificar cliente";
            }
            else
            {
                lblNom.Content = "Agregar un nuevo cliente";
            }
        }

        // Constructor sobrecargado (sin uso en este fragmento de código)
        public Window7(bool esModificacion, bool esOtraModificacion)
        {
            InitializeComponent();
            if (esOtraModificacion)
            {
                // Haz algo con esOtraModificacion si es necesario
            }
            Conn = new SqlConnection("Data source = DESKTOP-KIBLMD6\\SQLEXPRESS; Initial catalog = TelecomunicacionesBD; Integrated security = true");
            clientes = new List<Clientes>();
        }

        // Constructor por defecto 
        public Window7()
        {
        }

        // Conexión a la base de datos SQL Server 
        private SqlConnection Conn;

        // Cliente seleccionado para modificar 
        private Window2.Clientes clienteSeleccionado;

        private void BtnAceptar_Click(object sender, RoutedEventArgs e)
        {
            NuevoCliente = new Clientes
            {
                ID_Cliente = txtIDC.Text, // Mantenemos el ID_Cliente original
                Nombre = txtNombreC.Text,
                Apellido = txtApellidoC.Text,
                Teléfono = Convert.ToDecimal(txtTelefonoC.Text),
                Correo = txtCorreoC.Text,
                ID_Dirección = txtDireccionC.Text
            };

            try
            {
                if (ClienteDAL.ClienteExiste(NuevoCliente.ID_Cliente))
                {
                    // Cliente existente, actualiza los datos excepto el ID_Cliente
                    ClienteDAL.ActualizarCliente(NuevoCliente);
                    MessageBox.Show("Cliente modificado correctamente.");
                }
                else
                {
                    // Cliente no existente, agrega un nuevo cliente
                    ClienteDAL.AgregarCliente(NuevoCliente);
                    MessageBox.Show("Cliente agregado correctamente.");

                    // Llama al evento ClienteAgregado antes de cerrar la ventana
                    OnClienteAgregado();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al modificar/agregar el cliente: " + ex.Message);
            }

            // Cierra la ventana después de procesar el cliente
            this.Close();
        }

        // Método invocado cuando se agrega un cliente, activa el evento ClienteAgregado
        private void OnClienteAgregado()
        {
            ClienteAgregado?.Invoke(this, EventArgs.Empty);
        }

        // Método para mostrar los detalles del cliente seleccionado para modificar
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

