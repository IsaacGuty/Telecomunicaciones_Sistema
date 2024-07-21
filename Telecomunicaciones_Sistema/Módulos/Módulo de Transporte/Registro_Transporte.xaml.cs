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
using Telecomunicaciones_Sistema.Clases.Módulo_de_Transporte;

namespace Telecomunicaciones_Sistema
{
    /// <summary>
    /// Lógica de interacción para Registro_Empleado.xaml
    /// </summary>
    public partial class Registro_Transporte : Window
    {
        // Declaración de un campo privado para almacenar una instancia de la ventana Agregar_Transporte.
        private Agregar_Transporte ventanaAgr;

        // Propiedad estática que almacena el transporte seleccionado. Puede ser accedido y modificado desde cualquier lugar.
        public static Transportes TransporteSeleccionado { get; set; }

        // Propiedad que almacena una instancia de la ventana Modificar_Transporte. Esta propiedad es de solo lectura fuera de la clase.
        public Modificar_Transporte Modificar_Transporte { get; private set; }

        // Campo privado para almacenar la conexión a la base de datos SQL.
        private SqlConnection Conn;

        // Constructor de la clase Registro_Transporte. Se llama cuando se crea una nueva instancia de la clase.
        public Registro_Transporte()
        {
            // Inicializa los componentes de la interfaz de usuario definidos en el archivo XAML asociado.
            InitializeComponent();

            // Establece una conexión a la base de datos utilizando el método ObtenerConexion de la clase BD.
            Conn = BD.ObtenerConexion(); // Establecer conexión a la base de datos

            // Llama al método CargarDatos para cargar los datos de los clientes en el DataGrid.
            CargarDatos(); // Cargar los datos de los clientes en el DataGrid

            // Crea una nueva instancia de la ventana Agregar_Transporte.
            ventanaAgr = new Agregar_Transporte(); // Crear una instancia de Agregar_Transporte

            // Suscribe el evento TransporteAgregado de la ventana Agregar_Transporte al método ActualizarDatosTransporte.
            // Esto asegura que cada vez que se agregue un nuevo transporte, se actualizarán los datos en la ventana actual.
            ventanaAgr.TransporteAgregado += ActualizarDatosTransporte; // Suscribir al evento TransporteAgregado de Agregar_Transporte para actualizar los datos en esta ventana
        }

        public struct Transportes
        {
            public string ID_Placa;
            public string Marca_Carro;
            public string Modelo_Carro;
            public string Color;
            public DateTime Fecha_Pago;
            public int Año_Carro;
            public string ID_Estado;
        }

        public void CargarDatos()
        {
            // Inicia un bloque de código que puede lanzar excepciones para que puedan ser manejadas
            try
            {
                // Llama al método ObtenerTodosTransportes de la clase TransporteDAL para obtener todos los registros de transporte
                // y almacena el resultado en un objeto DataTable
                DataTable dataTable = TransporteDAL.ObtenerTodosTransportes();

                // Asigna la vista predeterminada del DataTable (DataView) como la fuente de datos para el DataGridTP
                DataGridTP.ItemsSource = dataTable.DefaultView;
            }
            // Captura cualquier excepción que pueda ocurrir durante la ejecución del bloque try
            catch (Exception ex)
            {
                // Muestra un mensaje de error con la información del mensaje de la excepción
                MessageBox.Show("Error al cargar los datos: " + ex.Message);
            }
        }

        // Evento manejador para el clic del botón "Agregar" en la interfaz de usuario
        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            // Muestra un cuadro de mensaje al usuario solicitando la información del nuevo transporte.
            // El cuadro de mensaje tiene un botón "OK" y un botón "Cancelar".
            MessageBoxResult result = MessageBox.Show("Por favor, ingrese la información del nuevo transporte.", "Nuevo Transporte", MessageBoxButton.OKCancel);

            // Verifica si el usuario hizo clic en el botón "OK" del cuadro de mensaje.
            if (result == MessageBoxResult.OK)
            {
                // Crea una instancia de la ventana "Agregar_Transporte", que permite al usuario ingresar la información del nuevo transporte.
                Agregar_Transporte frmAg = new Agregar_Transporte();

                // Suscribe el método `CargarDatos` al evento `TransporteAgregado` de la ventana `Agregar_Transporte`.
                // Esto asegura que el DataGrid se actualice (refresque) cuando se agregue un nuevo transporte.
                frmAg.TransporteAgregado += (s, args) => CargarDatos();

                // Muestra la ventana "Agregar_Transporte" al usuario.
                frmAg.Show();
            }
        }

        // Método manejador de eventos para el botón "Regresar" en una ventana WPF
        private void btnRegresar_Click(object sender, RoutedEventArgs e)
        {
            // Crear una instancia de la ventana principal "Menú".
            // El parámetro "isInicio_Sesión: true" indica que se está iniciando la sesión.
            Menú frmPr = new Menú(isInicio_Sesión: true);

            // Mostrar la ventana principal.
            frmPr.Show();

            // Cerrar la ventana actual (la ventana desde la que se hizo clic en el botón "Regresar").
            this.Close();
        }

        private void BtnModificar_Click(object sender, RoutedEventArgs e)
        {
            // Muestra un cuadro de mensaje solicitando la confirmación del usuario para modificar el transporte.
            MessageBoxResult result = MessageBox.Show("Por favor, ingrese la modificación del transporte.", "Modificación", MessageBoxButton.OKCancel);

            // Verifica si el usuario ha hecho clic en el botón OK.
            if (result == MessageBoxResult.OK)
            {
                // Verifica si se ha seleccionado un transporte válido (no es el valor predeterminado).
                if (!TransporteSeleccionado.Equals(default(Transportes)))
                {
                    // Crea una nueva instancia de la ventana Modificar_Transporte para permitir la modificación del transporte seleccionado.
                    Modificar_Transporte = new Modificar_Transporte(TransporteSeleccionado, true);

                    // Suscribe el evento TransporteModificado a la función ActualizarDatosTransporte.
                    Modificar_Transporte.TransporteModificado += ActualizarDatosTransporte;

                    // Suscribe el evento Closed a una función anónima que llama a CargarDatos.
                    Modificar_Transporte.Closed += (s, args) => CargarDatos();

                    // Muestra la ventana Modificar_Transporte.
                    Modificar_Transporte.Show();
                }
                else
                {
                    // Muestra un mensaje de advertencia si no se ha seleccionado ningún transporte.
                    MessageBox.Show("No se ha seleccionado ningún transporte.");
                }
            }
        }

        private void ActualizarDatosTransporte(object sender, EventArgs e)
        {
            CargarDatos(); // Cargar los datos nuevamente para reflejar el nuevo transporte agregado o modificado
        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            // Realizar la validación del texto de búsqueda
            if (!Validaciones.BusquedaTValida(txtBuscar.Text, out string mensaje))
            {
                // Mostrar un mensaje informando al usuario que debe ingresar un criterio de búsqueda
                MessageBox.Show(mensaje, "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // Detener la ejecución de la función si no se ha ingresado un criterio de búsqueda
            }

            // Imprimir el criterio de búsqueda
            Console.WriteLine("Criterio de búsqueda: " + txtBuscar.Text);

            // Buscar clientes según el texto ingresado en el campo de búsqueda
            DataTable dataTable = TransporteDAL.BuscarTransporte(txtBuscar.Text);
            DataView dataView = new DataView(dataTable);
            DataGridTP.ItemsSource = dataView;

            // Verificar si el DataTable está vacío
            if (dataTable.Rows.Count == 0)
            {
                MessageBox.Show("No se encontraron transportes que coincidan con la búsqueda.", "Búsqueda sin resultados", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            txtBuscar.Clear(); // Limpiar el campo de búsqueda
            txtBuscar.Foreground = new SolidColorBrush(Colors.Gray); // Restablecer el color del texto a gris
            txtBuscar.Text = "Placa, Marca"; // Restablecer el placeholder
            CargarDatos(); // Recargar todos los datos de los clientes en el DataGrid
        }

        private void txtBuscar_GotFocus(object sender, RoutedEventArgs e)
        {
            // Verifica si el texto actual en el control txtBuscar es igual a "Placa, Marca".
            if (txtBuscar.Text == "Placa, Marca")
            {
                // Si el texto es igual a "Placa, Marca", borra el texto del control txtBuscar.
                txtBuscar.Text = "";

                // Cambia el color del texto del control txtBuscar a negro (Color.Black).
                txtBuscar.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void txtBuscar_LostFocus(object sender, RoutedEventArgs e)
        {
            // Verifica si el contenido del TextBox 'txtBuscar' está vacío o contiene solo espacios en blanco.
            if (string.IsNullOrWhiteSpace(txtBuscar.Text))
            {
                // Si el contenido es vacío o solo espacios, establece el texto del TextBox a "Placa, Marca".
                // Esto indica al usuario el texto predeterminado que debería ingresar.
                txtBuscar.Text = "Placa, Marca";

                // Cambia el color del texto del TextBox a gris para indicar que es un texto de sugerencia o placeholder.
                txtBuscar.Foreground = new SolidColorBrush(Colors.Gray);
            }
            else
            {
                // Si el contenido del TextBox no está vacío, convierte el texto a minúsculas y luego capitaliza la primera letra de cada palabra.
                // Esto asegura que el texto ingresado se muestre con un formato estético y consistente.
                txtBuscar.Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(txtBuscar.Text.ToLower());
            }
        }

        // Manejador de eventos para el cambio de selección en el DataGrid
        private void DataGridTP_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGridTP.SelectedItem != null && DataGridTP.SelectedItem is DataRowView)
            {
                DataRowView rowView = DataGridTP.SelectedItem as DataRowView;

                // Almacena los datos del transporte seleccionado
                TransporteSeleccionado = new Transportes
                {
                    ID_Placa = rowView["ID_Placa"].ToString(),
                    Marca_Carro = rowView["Marca_Carro"].ToString(), // Ajusta aquí según el nombre de la columna en tu DataGrid
                    Modelo_Carro = rowView["Modelo_Carro"].ToString(),
                    Color = rowView["Color"].ToString(),
                    Fecha_Pago = Convert.ToDateTime(rowView["Fecha_Pago_Matrícula"]),
                    Año_Carro = Convert.ToInt32(rowView["Año_Carro"]),
                    ID_Estado = rowView["ID_Estado"].ToString()
                };
            }
            else
            {
                TransporteSeleccionado = default(Transportes); // Restablecer el transporte seleccionado si no hay selección
            }
        }
    }
}