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
using System.Diagnostics;

namespace Telecomunicaciones_Sistema
{
    /// <summary>
    /// Lógica de interacción para Agregar_Pago.xaml
    /// </summary>
    public partial class Agregar_Pago : Window
    {
        // Declaración del evento PagoModificado
        public event EventHandler PagoModificado;

        // Propiedad para obtener el nuevo pago
        public Pagos NuevoPago { get; private set; }

        // Esto asegura que la instancia solo pueda ser asignada dentro de la clase y se acceda a ella de manera controlada.
        public static Agregar_Pago Instance { get; private set; }

        // Objeto de conexión a la base de datos
        private SqlConnection Conn;

        public Agregar_Pago()
        {
            InitializeComponent();
           // pagos = new List<Pagos>(); // Inicialización de la lista de pagos
            Conn = BD.ObtenerConexion(); // Inicialización de la conexión a la base de datos
            txtIDP.Text = PagoDAL.ObtenerUltimoIDPago().ToString(); // Obtener el último ID_Pago de la base de datos y mostrarlo en txtIDP
            DateTime fechaActual = DateTime.Now; // Obtener la fecha actual y asignarla al campo txtFecha
            txtFecha.Text = fechaActual.ToString("yyyy-MM-dd");
            txtNombreE.Text = Inicio_Sesión.IdUsuario.ToString(); // Asignar el ID del usuario al campo txtNombreE
            cmbIDS.SelectionChanged += CmbIDS_SelectionChanged; // Agregar un manejador de eventos para el evento SelectionChanged del ComboBox cmbIDS
            Instance = this; // Almacena la instancia actual
            // Actualizar ComboBox de meses pagados para el cliente seleccionado
            if (!string.IsNullOrEmpty(txtIDC.Text) && cmbIDS.SelectedItem != null)
            {
                string idServicio = ObtenerNumeroServicioSeleccionado();
                ActualizarMesesPagados(txtIDC.Text, idServicio);
            }
        }

        private void ActualizarMesesPagados(string idCliente, string idServicio)
        {
            // Obtiene la lista de meses que han sido pagados por el cliente para el servicio especificado
            List<string> mesesPagados = PagoDAL.ObtenerMesesPagados(idCliente, idServicio);

            // Recorre cada elemento en el ComboBox 'cmbMes'
            foreach (ComboBoxItem item in cmbMes.Items)
            {
                // Obtiene el contenido del ComboBoxItem, que es el nombre del mes
                string mes = item.Content.ToString();

                // Verifica si el mes actual está en la lista de meses pagados
                if (mesesPagados.Contains(mes))
                {
                    // Si el mes está en la lista de meses pagados, oculta el elemento en el ComboBox
                    item.Visibility = Visibility.Collapsed;
                }
                else
                {
                    // Si el mes no está en la lista de meses pagados, muestra el elemento en el ComboBox
                    item.Visibility = Visibility.Visible;
                }
            }

            // Imprime en la consola de depuración cada mes que ha sido pagado, para facilitar la depuración
            foreach (var mes in mesesPagados)
            {
                Debug.WriteLine("Mes pagado: " + mes);
            }
        }

        private void BtnAceptar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Verifica si alguno de los campos requeridos está vacío usando la clase Validaciones.
                if (!Validaciones.CamposPagoVacios(txtIDP.Text, txtIDC.Text, txtNombreE.Text, cmbMes, txtMonto.Text))
                {
                    MessageBox.Show("Todos los campos del pago deben llenarse.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Verifica si se ha seleccionado un mes en el ComboBox cmbMes.
                if (cmbMes.SelectedItem == null)
                {
                    MessageBox.Show("Por favor, seleccione un mes.");
                    return;
                }

                // Obtiene el contenido del mes seleccionado del ComboBox cmbMes.
                string mesPagado = cmbMes.SelectedItem.ToString();
                ComboBoxItem selectedComboBoxItem = cmbMes.SelectedItem as ComboBoxItem;
                if (selectedComboBoxItem != null)
                {
                    mesPagado = selectedComboBoxItem.Content.ToString();
                }

                // Obtiene el ID del servicio seleccionado llamando al método ObtenerNumeroServicioSeleccionado.
                string idServicio = ObtenerNumeroServicioSeleccionado();

                // Verifica si el mes seleccionado ya ha sido pagado para el servicio seleccionado.
                // Obtiene una lista de meses ya pagados para el cliente y el servicio específico desde la base de datos.
                List<string> mesesPagados = PagoDAL.ObtenerMesesPagados(txtIDC.Text, idServicio);
                if (mesesPagados.Contains(mesPagado))
                {
                    MessageBox.Show("El mes seleccionado ya ha sido pagado.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Intenta convertir el monto ingresado a un tipo decimal.
                decimal monto;
                if (decimal.TryParse(txtMonto.Text, out monto))
                {
                    NuevoPago = new Pagos
                    {
                        ID_Pago = txtIDP.Text,
                        ID_Cliente = txtIDC.Text,
                        ID_Servicio = idServicio,
                        Monto = monto,
                        MesPagado = mesPagado,
                        ID_Empleado = txtNombreE.Text,
                        Fecha = DateTime.Now
                    };
                }
                else
                {
                    // Si la conversión del monto falla, muestra un mensaje de error.
                    MessageBox.Show("El monto ingresado no es válido.");
                    return;
                }

                // Llama al método AgregarPago de PagoDAL para agregar el nuevo pago a la base de datos.
                PagoDAL.AgregarPago(NuevoPago);
                MessageBox.Show("Pago agregado correctamente.");

                // Actualiza el ComboBox de meses pagados para el cliente seleccionado.
                ActualizarMesesPagados(txtIDC.Text, idServicio);

                // Si existe algún manejador para el evento PagoModificado, lo invoca.
                // Esto podría actualizar la UI u otros elementos relacionados con el pago.
                if (PagoModificado != null)
                {
                    PagoModificado(this, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                // Muestra un mensaje de error si ocurre una excepción durante el proceso.
                MessageBox.Show("Error al agregar el pago: " + ex.Message);
            }

            // Cierra la ventana actual después de que el proceso de agregar el pago ha terminado.
            this.Close();
        }

        private void BtnBusC_Click(object sender, RoutedEventArgs e)
        {
            // Crea una instancia de Registro_Cliente
            Registro_Cliente ventana2 = new Registro_Cliente();
            ventana2.SeleccionDesdeVentana9 = true; // Establecer la bandera

            // Deshabilitar los botones en la ventana 2
            ventana2.btnAgregar.IsEnabled = false; // Deshabilita el botón de agregar
            ventana2.BtnModificar.IsEnabled = false;

            // Muestra un mensaje para instruir al usuario a seleccionar un cliente
            MessageBox.Show("Por favor, seleccione un cliente de la lista en la ventana que se abrirá a continuación.", "Instrucción", MessageBoxButton.OK, MessageBoxImage.Information);

            // Muestra la ventana 2
            ventana2.ShowDialog();

            // Verifica si se seleccionó un cliente en la ventana 2
            if (!string.IsNullOrEmpty(ventana2.ClienteSeleccionado.ID_Cliente))
            {
                // Copia el ID del cliente seleccionado en txtIDC
                txtIDC.Text = ventana2.ClienteSeleccionado.ID_Cliente;

                // Obtener los servicios del cliente y llenar el ComboBox
                List<Servicio> servicios = PagoDAL.ObtenerServicioCliente(ventana2.ClienteSeleccionado.ID_Cliente);
                cmbIDS.Items.Clear();
                foreach (var servicio in servicios)
                {
                    ComboBoxItem item = new ComboBoxItem
                    {
                        Content = $"{servicio.ID_Servicio} - {servicio.Nombre}"
                    };
                    cmbIDS.Items.Add(item);
                }
            }
            else
            {
                // Si no se seleccionó un cliente, muestra un mensaje al usuario
                MessageBox.Show("No se seleccionó ningún cliente. Por favor, vuelva a la lista y seleccione un cliente.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnRegresar_Click(object sender, RoutedEventArgs e)
        {
            // Ocultar la ventana actual y mostrar la ventana3
            this.Hide();
            Registro_Pago frmPr = new Registro_Pago();
        }

        private string ObtenerNumeroServicioSeleccionado()
        {
            // Obtener el texto seleccionado en el ComboBox
            string servicioSeleccionado = (cmbIDS.SelectedItem as ComboBoxItem)?.Content.ToString();

            // Dividir el texto en partes utilizando el carácter '-'
            string[] partes = servicioSeleccionado.Split('-');

            // Obtener el número (primer elemento después de dividir)
            if (partes.Length > 0)
            {
                return partes[0].Trim(); // Retorna el número eliminando espacios en blanco al inicio y al final
            }

            return null; // En caso de que no haya texto seleccionado
        }

        private void CmbIDS_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Obtener el ComboBox actual
            ComboBox cmb = sender as ComboBox;

            // Verificar si se ha seleccionado un elemento
            if (cmb.SelectedItem != null)
            {
                // Obtener el contenido del elemento seleccionado
                string selectedItem = (cmb.SelectedItem as ComboBoxItem).Content.ToString();

                // Dependiendo de la opción seleccionada, asignar un monto al TextBox txtMonto
                switch (selectedItem)
                {
                    case "1 - Cable":
                        txtMonto.Text = "250"; // Asignar el monto correspondiente al servicio de cable
                        break;
                    case "2 - Internet 5MB":
                        txtMonto.Text = "650"; // Asignar el monto correspondiente al servicio de Internet 5MB
                        break;
                    case "3 - Internet 8MB":
                        txtMonto.Text = "850"; // Asignar el monto correspondiente al servicio de Internet 8MB
                        break;
                    case "4 - Internet 10MB":
                        txtMonto.Text = "1050"; // Asignar el monto correspondiente al servicio de Internet 10MB
                        break;
                    case "5 - Internet 15MB":
                        txtMonto.Text = "1200"; // Asignar el monto correspondiente al servicio de Internet 15MB
                        break;
                    default:
                        // Limpiar el TextBox si no se selecciona ninguna opción válida
                        txtMonto.Text = "";
                        break;
                }
            }
        }
    }
}


