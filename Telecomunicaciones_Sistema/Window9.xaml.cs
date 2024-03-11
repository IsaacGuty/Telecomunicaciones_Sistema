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
        //public event EventHandler PagoAgregado;

        public event EventHandler PagoModificado;


        private List<Pagos> pagos;

        public Pagos NuevoPago { get; private set; }

        public Window9()
        {
            InitializeComponent();
            Conn = new SqlConnection("Data source = DESKTOP-KIBLMD6\\SQLEXPRESS; Initial catalog = TelecomunicacionesBD; Integrated security = true");
            pagos = new List<Pagos>();
        }

        public Window9(Window3.Pagos pagoSeleccionado)
        {
            InitializeComponent();
            Conn = new SqlConnection("Data source = DESKTOP-KIBLMD6\\SQLEXPRESS; Initial catalog = TelecomunicacionesBD; Integrated security = true");
            pagos = new List<Pagos>();
            this.pagoSeleccionado = pagoSeleccionado;
            MostrarDetallesPago();
        }

        private SqlConnection Conn;
        private Window3.Pagos pagoSeleccionado;

        private void BtnAceptar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Pagos nuevoPago = new Pagos
                {
                    ID_Cliente = txtIDC.Text,
                    MesPagado = txtMesP.Text,
                    Servicio = txtServicio.Text,
                };

                PagoDAL.ActualizarPago(nuevoPago);
                MessageBox.Show("Pago modificado correctamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al modificar el pago: " + ex.Message);
            }

            OnPagoModificado();
            this.Close();
        }


        private void OnPagoModificado()
        {
            PagoModificado?.Invoke(this, EventArgs.Empty);
        }

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
            this.Hide();

            Window3 frmPr = new Window3();
        }
    }
}
