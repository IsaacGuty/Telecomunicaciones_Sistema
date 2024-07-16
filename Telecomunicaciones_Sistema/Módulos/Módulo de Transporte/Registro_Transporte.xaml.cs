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
        private Agregar_Transporte ventanaAgr;

        public static Transportes TransporteSeleccionado { get; set; }

        public Modificar_Transporte Modificar_Transporte { get; private set; }

        private SqlConnection Conn;

        public Registro_Transporte()
        {
            InitializeComponent();
            Conn = BD.ObtenerConexion(); // Establecer conexión a la base de datos           
            CargarDatos();  // Cargar los datos de los clientes en el DataGrid

            ventanaAgr = new Agregar_Transporte(); // Crear una instancia de Agregar_Transporte

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
            try
            {
                DataTable dataTable = TransporteDAL.ObtenerTodosTransportes();
                DataGridTP.ItemsSource = dataTable.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los datos: " + ex.Message);
            }
        }

        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Por favor, ingrese la información del nuevo transporte.", "Nuevo Transporte", MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                // Mostrar la ventana para agregar un nuevo transporte
                Agregar_Transporte frmAg = new Agregar_Transporte();
                frmAg.TransporteAgregado += (s, args) => CargarDatos(); // Refrescar los datos del DataGrid cuando se agregue un nuevo transporte
                frmAg.Show();
            }
        }

        private void btnRegresar_Click(object sender, RoutedEventArgs e)
        {
            // Regresar a la ventana principal
            Menú frmPr = new Menú(isInicio_Sesión: true);
            frmPr.Show();

            this.Close();
        }

        private void BtnModificar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Por favor, ingrese la modificación del transporte.", "Modificación", MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                if (!TransporteSeleccionado.Equals(default(Transportes)))
                {
                    // Crear una nueva ventana para modificar el transporte seleccionado
                    Modificar_Transporte = new Modificar_Transporte(TransporteSeleccionado, true);
                    Modificar_Transporte.TransporteModificado += ActualizarDatosTransporte;
                    Modificar_Transporte.Closed += (s, args) => CargarDatos(); // Refrescar los datos del DataGrid cuando se cierre la ventana
                    Modificar_Transporte.Show();
                }
                else
                {
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
            if (txtBuscar.Text == "Placa, Marca")
            {
                txtBuscar.Text = "";
                txtBuscar.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void txtBuscar_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBuscar.Text))
            {
                txtBuscar.Text = "Placa, Marca";
                txtBuscar.Foreground = new SolidColorBrush(Colors.Gray);
            }
            else
            {
                txtBuscar.Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(txtBuscar.Text.ToLower()); // Convierte la primera letra de cada palabra a mayúscula
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