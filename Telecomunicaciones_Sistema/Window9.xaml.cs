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
    /// <summary>
    /// Lógica de interacción para Window9.xaml
    /// </summary>
    public partial class Window9 : Window
    {
        // Declaración del evento PagoModificado
        public event EventHandler PagoModificado;

        // Lista para almacenar los pagos
        private List<Pagos> pagos;

        // Propiedad para obtener el nuevo pago
        public Pagos NuevoPago { get; private set; }

        // Lista de meses completos
        private List<string> mesesCompletos = new List<string>
        {
            "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio",
            "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"
        };

        // Constructor sin parámetros
        public Window9()
        {
            InitializeComponent();
            // Inicialización de la conexión a la base de datos
            Conn = BD.ObtenerConexion();
            // Inicialización de la lista de pagos
            pagos = new List<Pagos>();
        }

        // Constructor con parámetro de pago seleccionado
        public Window9(Window3.Pagos pagoSeleccionado)
        {
            InitializeComponent();
            // Inicialización de la conexión a la base de datos
            Conn = BD.ObtenerConexion();
            // Inicialización de la lista de pagos
            pagos = new List<Pagos>();
            // Asignación del pago seleccionado
            this.pagoSeleccionado = pagoSeleccionado;
            // Mostrar los detalles del pago seleccionado
            MostrarDetallesPago();
        }

        // Objeto de conexión a la base de datos
        private SqlConnection Conn;

        // Objeto para almacenar el pago seleccionado
        private Window3.Pagos pagoSeleccionado;

        private void BtnAceptar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validar que el mes ingresado sea completo
                if (!mesesCompletos.Contains(txtMesP.Text))
                {
                    MessageBox.Show("Por favor, ingrese el nombre completo del mes.");
                    return;
                }

                // Crear un nuevo objeto de pago con los datos ingresados en el formulario
                Pagos nuevoPago = new Pagos
                {
                    ID_Cliente = txtIDC.Text,
                    MesPagado = txtMesP.Text,
                    Servicio = txtServicio.Text,
                };

                // Actualizar el pago en la base de datos
                PagoDAL.ActualizarPago(nuevoPago);
                // Mostrar un mensaje de éxito
                MessageBox.Show("Pago modificado correctamente.");
            }
            catch (Exception ex)
            {
                // Mostrar un mensaje de error en caso de excepción
                MessageBox.Show("Error al modificar el pago: " + ex.Message);
            }

            // Invocar el evento PagoModificado
            OnPagoModificado();
            // Cerrar la ventana
            this.Close();
        }

        // Método para invocar el evento PagoModificado
        private void OnPagoModificado()
        {
            PagoModificado?.Invoke(this, EventArgs.Empty);
        }

        // Método para mostrar los detalles del pago seleccionado en los campos del formulario
        private void MostrarDetallesPago()
        {
            txtIDC.Text = pagoSeleccionado.ID_Cliente;
            txtNombreC.Text = pagoSeleccionado.Nombre;
            txtApellidoC.Text = pagoSeleccionado.Apellido;
            txtDirecciónC.Text = pagoSeleccionado.Dirección;
            txtTeléfonoC.Text = pagoSeleccionado.Teléfono;
            txtServicio.Text = pagoSeleccionado.Servicio;
            txtMonto.Text = pagoSeleccionado.Monto;
            txtMesP.Text = pagoSeleccionado.MesPagado;
            txtNombreE.Text = pagoSeleccionado.Nombre_E;
        }

        private void BtnRegresar_Click(object sender, RoutedEventArgs e)
        {
            // Ocultar la ventana actual y mostrar la ventana3
            this.Hide();
            Window3 frmPr = new Window3();
        }
    }
}

