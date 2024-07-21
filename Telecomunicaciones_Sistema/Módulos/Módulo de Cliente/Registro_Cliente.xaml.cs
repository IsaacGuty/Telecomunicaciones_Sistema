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
using System.Globalization;

namespace Telecomunicaciones_Sistema
{
    /// <summary>
    /// Lógica de interacción para Registro_Cliente.xaml
    /// </summary>
    public partial class Registro_Cliente : Window
    {
        // Propiedad que indica si la ventana fue abierta desde Ventana9
        public bool SeleccionDesdeVentana9 { get; set; }

        private bool SeleccionDesdeVentana10 = false; // Variable privada para manejar la selección desde Ventana10

        private Agregar_Cliente ventana7; // Instancia de la ventana para agregar o modificar clientes

        // Propiedad para almacenar el cliente actualmente seleccionado
        public Clientes ClienteSeleccionado { get; set; }

        // Propiedad para almacenar la ventana de modificación del cliente
        public Modificar_Cliente Modificar_Cliente { get; private set; }

        // Conexión a la base de datos
        private SqlConnection Conn;

        // Indicador de si esta ventana es la ventana principal del sistema
        private bool isInicio_Sesión;

        // Constructor de la ventana Registro_Cliente
        public Registro_Cliente()
        {
            InitializeComponent(); // Inicializa los componentes de la ventana
            Conn = BD.ObtenerConexion(); // Establece la conexión a la base de datos
            CargarDatos();  // Carga los datos de clientes en el DataGrid

            ventana7 = new Agregar_Cliente(); // Inicializa la instancia de Agregar_Cliente

            // Suscribe el evento ClienteAgregado para actualizar los datos en esta ventana
            ventana7.ClienteAgregado += ActualizarDatosCliente;
        }

        // Estructura que representa los datos de un cliente
        public struct Clientes
        {
            public string ID_Cliente; // ID del cliente
            public string Nombre; // Nombre del cliente
            public string Apellido; // Apellido del cliente
            public string Teléfono; // Teléfono del cliente
            public string Correo; // Correo electrónico del cliente
            public string ID_Dirección; // ID de la dirección del cliente
        }

        // Método para cargar los datos de clientes desde la base de datos al DataGrid
        private void CargarDatos()
        {
            try
            {
                DataTable dataTable = ClienteDAL.ObtenerTodosClientes(); // Obtiene todos los clientes de la base de datos
                DatGridRC.ItemsSource = dataTable.DefaultView; // Asigna los datos al DataGrid
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los datos: " + ex.Message); // Muestra un mensaje de error en caso de excepción
            }
        }

        // Manejador del evento de cambio de selección en el DataGrid
        private void DatGridRC_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DatGridRC.SelectedItem != null && DatGridRC.SelectedItem is DataRowView)
            {
                DataRowView rowView = DatGridRC.SelectedItem as DataRowView;

                // Almacena los datos del cliente seleccionado en la estructura Clientes
                ClienteSeleccionado = new Clientes
                {
                    ID_Cliente = rowView["ID_Cliente"].ToString(),
                    Nombre = rowView["Nombre"].ToString(),
                    Apellido = rowView["Apellido"].ToString(),
                    Teléfono = rowView["Teléfono"].ToString(),
                    Correo = rowView["Correo"].ToString(),
                    ID_Dirección = rowView["ID_Dirección"].ToString()
                };
            }
            else
            {
                ClienteSeleccionado = default(Clientes); // Restablece el cliente seleccionado si no hay selección
            }
        }

        // Manejador del evento de clic en el botón de agregar cliente
        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Por favor, ingrese la información del nuevo cliente.", "Nuevo Cliente", MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                // Muestra la ventana para agregar un nuevo cliente
                Agregar_Cliente frmAg = new Agregar_Cliente();
                frmAg.ClienteAgregado += (s, args) => CargarDatos(); // Refresca el DataGrid cuando se agrega un nuevo cliente
                frmAg.Show();
            }
        }

        // Manejador del evento de clic en el botón de buscar cliente
        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            // Realiza la validación del texto de búsqueda
            if (!Validaciones.BusquedaCValida(txtBuscar.Text, out string mensaje))
            {
                // Muestra un mensaje si no se ha ingresado un criterio de búsqueda
                MessageBox.Show(mensaje, "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // Detiene la ejecución de la función si el criterio de búsqueda es inválido
            }

            // Imprime el criterio de búsqueda en la consola
            Console.WriteLine("Criterio de búsqueda: " + txtBuscar.Text);

            // Busca clientes según el texto ingresado en el campo de búsqueda
            DataTable dataTable = ClienteDAL.BuscarCliente(txtBuscar.Text);
            DataView dataView = new DataView(dataTable);
            DatGridRC.ItemsSource = dataView;

            // Verifica si no se encontraron resultados
            if (dataTable.Rows.Count == 0)
            {
                MessageBox.Show("No se encontraron clientes que coincidan con la búsqueda.", "Búsqueda sin resultados", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // Manejador del evento de clic en el botón de limpiar búsqueda
        private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            txtBuscar.Clear(); // Limpia el campo de búsqueda
            txtBuscar.Foreground = new SolidColorBrush(Colors.Gray); // Restaura el color del texto a gris
            txtBuscar.Text = "ID, nombre, apellido"; // Restaura el texto de placeholder
            CargarDatos(); // Recarga todos los datos de clientes en el DataGrid
        }

        // Manejador del evento de clic en el botón de modificar cliente
        private void BtnModificar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Por favor, ingrese la modificación del cliente.", "Modificación", MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                // Verifica si hay un cliente seleccionado
                if (!ClienteSeleccionado.Equals(default(Clientes)))
                {
                    if (ventana7 == null || !ventana7.IsVisible) // Verifica si la ventana de modificación está abierta
                    {
                        // Abre la ventana para modificar el cliente seleccionado
                        Modificar_Cliente = new Modificar_Cliente(ClienteSeleccionado);
                        Modificar_Cliente.ClienteModificado += ActualizarDatosCliente; // Actualiza los datos cuando se modifique el cliente
                        Modificar_Cliente.Closed += (s, args) => CargarDatos(); // Refresca los datos cuando se cierre la ventana de modificación
                        Modificar_Cliente.Show();
                    }
                    else
                    {
                        ventana7.Activate(); // Muestra la ventana si ya está abierta
                    }
                }
                else
                {
                    // Muestra un mensaje si no se ha seleccionado ningún cliente
                    MessageBox.Show("No se ha seleccionado ningún cliente.");
                }
            }
        }

        // Método para actualizar los datos del cliente en el DataGrid
        private void ActualizarDatosCliente(object sender, EventArgs e)
        {
            CargarDatos(); // Recarga los datos del DataGrid para reflejar los cambios
        }

        // Manejador del evento de clic en el botón de regresar
        private void BtnRegresar_Click(object sender, RoutedEventArgs e)
        {
            if (Vista_Pago.SeleccionDesdeVentana10)
            {
                // Oculta la ventana actual y muestra la ventana Vista_Pago
                this.Hide();
                Vista_Pago ventana10 = Vista_Pago.Instance;
            }
            else if (SeleccionDesdeVentana9)
            {
                // Oculta la ventana actual y muestra la ventana Agregar_Pago
                this.Hide();
                Agregar_Pago ventana9 = Agregar_Pago.Instance;
                ventana9.Show();
            }
            else
            {
                // Oculta la ventana actual y muestra la ventana Menú
                this.Hide();
                Menú frmPr = new Menú(isInicio_Sesión: true);
                frmPr.Show();

                if (!isInicio_Sesión)
                {
                    this.Close(); // Cierra la ventana si no es la ventana principal
                }
            }
        }

        // Manejador del evento de obtención de foco en el campo de búsqueda
        private void txtBuscar_GotFocus(object sender, RoutedEventArgs e)
        {
            // Verifica si el texto en el campo de búsqueda es el texto de placeholder
            if (txtBuscar.Text == "ID, nombre, apellido")
            {
                txtBuscar.Text = ""; // Limpia el campo de búsqueda
                txtBuscar.Foreground = new SolidColorBrush(Colors.Black); // Cambia el color del texto a negro
            }
        }

        // Manejador del evento de pérdida de foco en el campo de búsqueda
        private void txtBuscar_LostFocus(object sender, RoutedEventArgs e)
        {
            // Verifica si el campo de búsqueda está vacío
            if (string.IsNullOrWhiteSpace(txtBuscar.Text))
            {
                txtBuscar.Text = "ID, nombre, apellido"; // Restaura el texto de placeholder
                txtBuscar.Foreground = new SolidColorBrush(Colors.Gray); // Restaura el color del texto a gris
            }
            else
            {
                // Convierte la primera letra de cada palabra a mayúscula
                txtBuscar.Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(txtBuscar.Text.ToLower());
            }
        }
    }
}



