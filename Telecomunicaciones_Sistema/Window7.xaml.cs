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

        // Lista para almacenar clientes 
        private List<Clientes> clientes;

        // Propiedad para acceder al nuevo cliente desde fuera de la clase
        public Clientes NuevoCliente { get; private set; }

        // Variable para indicar si se está modificando un cliente existente
        private bool esModificacion;

        // Constructor para la ventana de agregar o modificar cliente
        public Window7(bool esModificacion)
        {
            InitializeComponent();
            MostrarNuevoID();
            this.esModificacion = esModificacion;
            if (esModificacion)
            {
                lblNom.Content = "Modificar cliente"; // Cambia el título de la ventana si se está modificando
            }
            else
            {
                lblNom.Content = "Agregar un nuevo cliente"; // Cambia el título de la ventana si se está agregando
            }

            // Inicializar NuevoCliente
            NuevoCliente = new Clientes();
        }

        // Constructor sobrecargado para la ventana de agregar o modificar cliente, recibiendo el cliente a modificar
        public Window7(Window2.Clientes clienteSeleccionado, bool esModificacion)
        {
            InitializeComponent();
            this.esModificacion = esModificacion;
            Conn = BD.ObtenerConexion();
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

        private void MostrarNuevoID()
        {
            if (!esModificacion)
            {
                // Obtener el nuevo ID generado
                int nuevoID = GenerarNuevoID();
                // Asignar el nuevo ID al campo txtIDC
                txtIDC.Text = nuevoID.ToString();
            }
        }


        // Constructor 
        public Window7(bool esModificacion, bool esOtraModificacion)
        {
            InitializeComponent();
            if (esOtraModificacion)
            {
                // Haz algo con esOtraModificacion si es necesario
            }
            Conn = BD.ObtenerConexion();
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
            try
            {
                string idCliente = txtIDC.Text;

                // Verificar si el ID del cliente ya existe en la base de datos
                bool clienteExistente = ClienteDAL.ClienteExiste(idCliente);

                // Si estamos en modo modificación y el cliente no existe, mostrar un mensaje de error
                if (esModificacion && !clienteExistente)
                {
                    MessageBox.Show("El cliente con este ID no existe en la base de datos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!esModificacion && ClienteDAL.ClienteDI(txtIDC.Text, txtNombreC.Text, txtApellidoC.Text, txtCorreoC.Text, txtTelefonoC.Text, txtDireccionC.Text))
                {
                    MessageBox.Show("Ya existe un cliente con la misma información en la base de datos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Verificar si algún campo del cliente está vacío
                // Verificar si algún campo del cliente está vacío
                if (Validaciones.CamposClienteVacios(txtNombreC.Text, txtApellidoC.Text, txtTelefonoC.Text, txtCorreoC.Text, txtDireccionC.Text))
                {
                    MessageBox.Show("Todos los campos del cliente deben llenarse.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Si estamos en modo modificación y el cliente existe, actualizar los datos del cliente
                if (esModificacion && clienteExistente)
                {
                    // Crear el objeto NuevoCliente con los datos modificados
                    NuevoCliente = new Clientes
                    {
                        ID_Cliente = idCliente,
                        Nombre = txtNombreC.Text,
                        Apellido = txtApellidoC.Text,
                        Correo = txtCorreoC.Text,
                        ID_Dirección = txtDireccionC.Text
                    };

                    // Verificar si el texto del campo de teléfono es un número válido
                    if (!decimal.TryParse(txtTelefonoC.Text, out decimal telefonoDecimal))
                    {
                        MessageBox.Show("El teléfono debe ser un número válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (!Validaciones.EsTelefonoValido(txtTelefonoC.Text))
                    {
                        MessageBox.Show("El teléfono debe tener 8 dígitos y comenzar con 3, 8 o 9.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Verificar si el texto del campo de correo electrónico es válido
                    if (!Validaciones.CorreoValido(txtCorreoC.Text))
                    {
                        MessageBox.Show("El formato del correo electrónico no es válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Asignar el valor convertido a decimal al Teléfono del NuevoCliente
                    NuevoCliente.Teléfono = telefonoDecimal;

                    // Actualizar el cliente existente en la base de datos
                    ClienteDAL.ActualizarCliente(NuevoCliente);
                    MessageBox.Show("Cliente modificado correctamente.");
                }
                // Si estamos en modo agregado y el cliente no existe, agregar el nuevo cliente
                else if (!esModificacion && !clienteExistente)
                {
                    // Crear el objeto NuevoCliente con los datos del nuevo cliente
                    NuevoCliente = new Clientes
                    {
                        ID_Cliente = idCliente,
                        Nombre = txtNombreC.Text,
                        Apellido = txtApellidoC.Text,
                        Correo = txtCorreoC.Text,
                        ID_Dirección = txtDireccionC.Text
                    };

                    // Verificar si el texto del campo de teléfono es un número válido
                    if (!decimal.TryParse(txtTelefonoC.Text, out decimal telefonoDecimal))
                    {
                        MessageBox.Show("El teléfono debe ser un número válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Verificar si el teléfono cumple con los criterios de validación
                    if (!Validaciones.EsTelefonoValido(txtTelefonoC.Text))
                    {
                        MessageBox.Show("El teléfono debe tener 8 dígitos y comenzar con 3, 8 o 9.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Verificar si el texto del campo de correo electrónico es válido
                    if (!Validaciones.CorreoValido(txtCorreoC.Text))
                    {
                        MessageBox.Show("El formato del correo electrónico no es válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Asignar el valor convertido a decimal al Teléfono del NuevoCliente
                    NuevoCliente.Teléfono = telefonoDecimal;

                    // Agregar el nuevo cliente a la base de datos
                    ClienteDAL.AgregarCliente(NuevoCliente);
                    MessageBox.Show("Cliente agregado correctamente.");

                    // Llama al evento ClienteAgregado antes de cerrar la ventana
                    OnClienteAgregado();
                }
                // Si estamos en modo agregado y el cliente ya existe, mostrar un mensaje de error
                else if (!esModificacion && clienteExistente)
                {
                    MessageBox.Show("El cliente con este ID ya existe en la base de datos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
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
        private int GenerarNuevoID()
        {
            int nuevoID = 0;
            try
            {
                // Simplemente puedes obtener la cantidad actual de clientes y sumarle 1 para generar el nuevo ID
                int cantidadClientes = ClienteDAL.ObtenerCantidadClientes();
                nuevoID = cantidadClientes + 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar el nuevo ID: " + ex.Message);
            }
            return nuevoID;
        }
    }
}

