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
using System.Text.RegularExpressions;
using System.Globalization;
using Telecomunicaciones_Sistema.Clases.Módulo_de_Transporte;
using System.Windows.Markup;

namespace Telecomunicaciones_Sistema
{
    /// <summary>
    /// Lógica de interacción para Registro_Orden.xaml
    /// </summary>
    public partial class Registro_Orden : Window
    {
        private SqlConnection Conn;

        public Registro_Orden()
        {
            InitializeComponent();
            Conn = BD.ObtenerConexion(); // Conexión a la base de datos
            CargarDatos(); // Al inicializar la ventana, carga los datos en el DataGrid

            CargarEmpleadosTecnicos();

            CargarTransportesActivos();
        }

        // Propiedad para almacenar los datos de la orden
        public Ordenes DatosOrden { get; set; }

        private bool isInicio_Sesión; // Variable para identificar si esta ventana es la ventana principal

        // Método para cargar los datos en el DataGrid al iniciar la ventana
        private void CargarDatos()
        {
            try
            {
                DataTable dataTable = OrdenDAL.ObtenerOrdenes(); // Obtiene los datos de las órdenes y los muestra en el DataGrid
                DatGridOT.ItemsSource = dataTable.DefaultView; 
                DatGridOT.IsReadOnly = true; // Establecer el DataGrid como solo lectura
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los datos: " + ex.Message);
            }
        }

        private void CargarEmpleadosTecnicos()
        {
            try
            {
                DataTable dataTable = EmpleadoDAL.ObtenerEmpleadosTecnicos(); // Obtener los empleados técnicos
                cmbNombreE.Items.Clear(); // Limpiar ComboBox cmbNombreE
                // Agregar empleados técnicos al ComboBox cmbNombreE
                foreach (DataRow row in dataTable.Rows)
                {
                    string nombreCompleto = row["Nombre_E"].ToString() + " " + row["Apellido_E"].ToString();
                    cmbNombreE.Items.Add(nombreCompleto);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los empleados técnicos: " + ex.Message);
            }
        }

        private void CargarTransportesActivos()
        {
            try
            {
                DataTable dataTable = TransporteDAL.ObtenerTransportesActivos(); // Obtener los transportes activos
                cmbTransporte.Items.Clear(); // Limpiar ComboBox cmbTransporte
                // Agregar transportes actvios al ComboBox cmbTransporte
                foreach (DataRow row in dataTable.Rows)
                {
                    string marcaCompleta = row["Modelo_Carro"].ToString();
                    cmbTransporte.Items.Add(marcaCompleta);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los transportes activos: " + ex.Message);
            }
        }

        private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            // Limpia los campos 
            txtNombre.Clear();
            txtApellido.Clear();
            txtDirección.Clear();
            txtNumT.Clear();
            txtTpServicio.Clear();
            txtBuscar.Clear();

            // Restablece el placeholder y color del campo de búsqueda
            txtBuscar.Text = "Nombre, apellido";
            txtBuscar.Foreground = new SolidColorBrush(Colors.Gray);

            // Limpia el ComboBox de TipoT
            cmbTipoT.SelectedIndex = -1;

            // Limpia el ComboBox del Nombre del Empleado
            cmbNombreE.SelectedIndex = -1;

            // Limpia el ComboBox de Transporte
            cmbTransporte.SelectedIndex = -1;

            // Recarga los datos en el DataGrid
            CargarDatos();
        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            // Realizar la validación del texto de búsqueda
            if (!Validaciones.BusquedaOValida(txtBuscar.Text, out string mensaje))
            {
                // Mostrar un mensaje informando al usuario que debe ingresar un criterio de búsqueda
                MessageBox.Show(mensaje, "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // Detener la ejecución de la función si no se ha ingresado un criterio de búsqueda
            }

            // Imprimir el criterio de búsqueda
            Console.WriteLine("Criterio de búsqueda: " + txtBuscar.Text);

            // Obtener los datos de las órdenes que coinciden con el criterio de búsqueda y mostrarlos en el DataGrid
            DataTable dataTable = OrdenDAL.BuscarOrden(txtBuscar.Text);
            DataView dataView = dataTable.DefaultView;
            DatGridOT.ItemsSource = dataView;

            // Verificar si la búsqueda no devolvió ningún resultado después de mostrar los datos
            if (dataTable.Rows.Count == 0)
            {
                // Mostrar mensaje indicando que no se encontró ninguna orden que coincida con el criterio de búsqueda
                MessageBox.Show("No se encontró ninguna orden que coincida con el criterio de búsqueda.", "Búsqueda sin resultados", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // Evento que se ejecuta cuando se selecciona una fila en el DataGrid
        private void DatGridOT_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Muestra los detalles de la orden seleccionada en los campos correspondientes
            if (DatGridOT.SelectedItem != null)
            {
                DataRowView rowView = DatGridOT.SelectedItem as DataRowView;

                if (rowView != null)
                {
                    txtNombre.Text = rowView["Nombre"].ToString();
                    txtApellido.Text = rowView["Apellido"].ToString();
                    txtDirección.Text = rowView["Dirección"].ToString();
                    txtNumT.Text = rowView["Teléfono"].ToString();
                    txtTpServicio.Text = rowView["Tipo_Servicio"].ToString();
                }
            }
        }

        // Variables para almacenar la selección en los ComboBox
        private string valorSeleccionadoTipoT;
        private string valorSeleccionadoNombreE;
        private string valorSeleccionadoTransporte;

        // Evento que se ejecuta al cambiar la selección en el ComboBox de Tipo de Trabajo
        private void CmbTipoT_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Obtener el tipo de trabajo seleccionado del ComboBox cmbTipoT
            if (cmbTipoT.SelectedItem != null)
            {
                valorSeleccionadoTipoT = ((ComboBoxItem)cmbTipoT.SelectedItem).Content.ToString();
            }
        }

        // Evento que se ejecuta al cambiar la selección en el ComboBox de Nombre de Empleado
        private void CmbNombreE_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Actualiza la selección en el ComboBox de Tipo de Trabajo en otra ventana
            if (cmbNombreE.SelectedItem != null)
            {
                valorSeleccionadoNombreE = cmbNombreE.SelectedItem.ToString();

                // Obtener el ID del empleado seleccionado
                string idEmpleado = ObtenerIdEmpleadoSeleccionado(valorSeleccionadoNombreE);
                string idPlaca = ObtenerIdPlacaSeleccionado(valorSeleccionadoTransporte);

                // Crear una instancia de Mostrar_Orden si no existe una instancia previa
                Mostrar_Orden ventana5 = Application.Current.Windows.OfType<Mostrar_Orden>().FirstOrDefault();
                if (ventana5 == null)
                {
                    ventana5 = new Mostrar_Orden();
                }

                // Asignar los datos de la orden a la ventana5
                ventana5.DatosOrden = DatosOrden;

                // Actualizar los datos en la ventana5
                ventana5.ActualizarDatos(valorSeleccionadoTipoT, valorSeleccionadoNombreE, valorSeleccionadoTransporte, idEmpleado, idPlaca);
            }
        }

        // Evento que se ejecuta al cambiar la selección en el ComboBox 
        private void CmbTransporte_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Actualiza la selección en el ComboBox de transporte en otra ventana
            if (cmbTransporte.SelectedItem != null)
            {
                valorSeleccionadoTransporte = cmbTransporte.SelectedItem.ToString();

                // Obtener el ID del empleado seleccionado
                string idPlaca = ObtenerIdPlacaSeleccionado(valorSeleccionadoTransporte);
                string idEmpleado = ObtenerIdEmpleadoSeleccionado(valorSeleccionadoNombreE);

                // Crear una instancia de Mostrar_Orden si no existe una instancia previa
                Mostrar_Orden ventana5 = Application.Current.Windows.OfType<Mostrar_Orden>().FirstOrDefault();
                if (ventana5 == null)
                {
                    ventana5 = new Mostrar_Orden();
                }

                // Asignar los datos de la orden a la ventana5
                ventana5.DatosOrden = DatosOrden;

                // Actualizar los datos en la ventana5
                ventana5.ActualizarDatos(valorSeleccionadoTipoT, valorSeleccionadoNombreE, valorSeleccionadoTransporte, idPlaca, idEmpleado);
            }
        }

        private void BtnMostrar_Click(object sender, RoutedEventArgs e)
        {
            if (!Validaciones.CamposOrdenVacios(txtNombre.Text, txtApellido.Text, txtDirección.Text, txtNumT.Text, txtTpServicio.Text, cmbTipoT.SelectedItem, cmbNombreE.SelectedItem, cmbTransporte.SelectedItem))
            {
                MessageBox.Show("Por favor, complete todos los campos antes de imprimir.", "Datos Incompletos", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Obtener el ID del empleado seleccionado
                string idEmpleado = ObtenerIdEmpleadoSeleccionado(cmbNombreE.SelectedItem.ToString());
                string idPlaca = ObtenerIdPlacaSeleccionado(cmbTransporte.SelectedItem.ToString());

                // Guardar la orden en la base de datos
                OrdenDAL.GuardarOrden(new Ordenes
                {
                    Nombre = txtNombre.Text,
                    Apellido = txtApellido.Text,
                    Dirección = txtDirección.Text,
                    Teléfono = Convert.ToDecimal(txtNumT.Text),
                    Servicio = txtTpServicio.Text,
                    Tipo_Trabajo = valorSeleccionadoTipoT,
                    Nombre_E = cmbNombreE.SelectedItem.ToString(),
                    Modelo_Carro = cmbTransporte.SelectedItem.ToString(),
                    ID_Empleado = idEmpleado,
                    ID_Placa = idPlaca,
                    Fecha_Orden = DateTime.Now
                });

                // Crear una instancia de Mostrar_Orden si no existe una instancia previa
                Mostrar_Orden ventana5 = Application.Current.Windows.OfType<Mostrar_Orden>().FirstOrDefault();
                if (ventana5 == null)
                {
                    ventana5 = new Mostrar_Orden();
                }

                // Asignar los datos de la orden a la ventana5
                ventana5.DatosOrden = new Ordenes
                {
                    Nombre = txtNombre.Text,
                    Apellido = txtApellido.Text,
                    Dirección = txtDirección.Text,
                    Teléfono = Convert.ToDecimal(txtNumT.Text),
                    Servicio = txtTpServicio.Text,
                    Fecha_Orden = DateTime.Now
                };

                // Obtener los valores seleccionados de los ComboBoxes
                string valorSeleccionadoNombreE = cmbNombreE.SelectedItem.ToString();
                string valorSeleccionadoTransporte = cmbTransporte.SelectedItem.ToString();

                // Actualizar los datos en la ventana5
                ventana5.ActualizarDatos(valorSeleccionadoTipoT, valorSeleccionadoNombreE, valorSeleccionadoTransporte, idEmpleado, idPlaca);


                ventana5.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar la orden: " + ex.Message);
            }
        }

        // Método para obtener el ID del empleado seleccionado en el ComboBox
        private string ObtenerIdEmpleadoSeleccionado(string nombreCompleto)
        {
            try
            {
                // Buscar el ID del empleado en base al nombre completo
                DataTable dataTable = EmpleadoDAL.BuscarEmpleadoNombreCompleto(nombreCompleto);
                if (dataTable.Rows.Count > 0)
                {
                    // Obtener el ID del primer empleado encontrado
                    return dataTable.Rows[0]["ID_Empleado"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener el ID del empleado: " + ex.Message);
            }

            // Si no se pudo encontrar el empleado, devolver null
            return null;
        }

        private string ObtenerIdPlacaSeleccionado(string modelo)
        {
            try
            {
                // Verificar que el modelo no sea nulo o vacío
                if (string.IsNullOrEmpty(modelo))
                {
                    return null;
                }

                // Buscar el ID de la placa en base al modelo
                DataTable dataTable = TransporteDAL.BuscarPlacaCompleto(modelo);
                if (dataTable.Rows.Count > 0)
                {
                    // Obtener el ID de la primera placa encontrada
                    return dataTable.Rows[0]["ID_Placa"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener el ID placa: " + ex.Message);
            }

            // Si no se pudo encontrar la placa, devolver null
            return null;
        }

        private void txtBuscar_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtBuscar.Text == "Nombre, apellido")
            {
                txtBuscar.Text = "";
                txtBuscar.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void txtBuscar_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBuscar.Text))
            {
                txtBuscar.Text = "Nombre, apellido";
                txtBuscar.Foreground = new SolidColorBrush(Colors.Gray);
            }
            else
            {
                // Convierte la primera letra de cada palabra a mayúscula
                txtBuscar.Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(txtBuscar.Text.ToLower());
            }
        }

        private void BtnRegresar_Click(object sender, RoutedEventArgs e)
        {
            // Crea una nueva instancia de la ventana principal y la muestra
            Menú frmPr = new Menú(isInicio_Sesión: true);
            frmPr.Show();

            // Cierra esta ventana si no es la ventana principal
            if (!isInicio_Sesión)
            {
                this.Close();
            }
        }
    }
}

