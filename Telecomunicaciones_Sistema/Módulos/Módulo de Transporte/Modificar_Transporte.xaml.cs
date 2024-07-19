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
    public partial class Modificar_Transporte : Window
    {
        public event EventHandler TransporteModificado;
        private SqlConnection Conn;
        private Registro_Transporte.Transportes TransporteSeleccionado;
        private bool esModificacion;
        public Transporte NuevoTransporte { get; private set; }

        public Modificar_Transporte(Registro_Transporte.Transportes TransporteSeleccionado, bool esModificacion)
        {
            InitializeComponent();
            this.esModificacion = esModificacion;
            this.TransporteSeleccionado = TransporteSeleccionado;
            MostrarDetallesTransporte();
        }

        private void MostrarDetallesTransporte()
        {
            txtIDP.Text = TransporteSeleccionado.ID_Placa;
            txtMarca.Text = TransporteSeleccionado.Marca_Carro;
            txtModelo.Text = TransporteSeleccionado.Modelo_Carro;
            txtColor.Text = TransporteSeleccionado.Color;
            txtAño.Text = TransporteSeleccionado.Año_Carro.ToString();

            // Asignar la fecha al DatePicker
            Fecha_Pago.SelectedDate = TransporteSeleccionado.Fecha_Pago;

            // Obtener el estado del transporte seleccionado
            string estadoTransporte = TransporteSeleccionado.ID_Estado;

            // Buscar el estado en los elementos del ComboBox
            foreach (ComboBoxItem item in cmbEstado.Items)
            {
                string estado = item.Content?.ToString().Split('-')[0].Trim(); // Obtener solo el número de estado
                if (estado == estadoTransporte)
                {
                    // Establecer el elemento correspondiente como seleccionado en el ComboBox cmbEstado
                    cmbEstado.SelectedItem = item;
                    break;
                }
            }
        }

        private void OnTransporteModificado()
        {
            TransporteModificado?.Invoke(this, EventArgs.Empty);
        }

        private void btnRegresar_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();

            // Crea una nueva instancia de Registro_Transporte (ventana principal de transportes) y la muestra
            Registro_Transporte frmTP = new Registro_Transporte();
        }

        private void btnAceptar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!Validaciones.NoContieneEspaciosEnBlanco(txtColor.Text) || !Validaciones.NoContieneEspaciosEnBlanco(Fecha_Pago.Text))
                {
                    MessageBox.Show("Todos los campos del transporte deben llenarse.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string color = txtColor.Text;

                if (!Validaciones.MMValido(txtColor.Text))
                {
                    MessageBox.Show("El color no es válido. No se permiten espacios en blanco al inicio ni entre caracteres.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!Validaciones.TresVecesSeguidas(txtColor.Text))
                {
                    MessageBox.Show("El color no es válido. No se permiten más de 3 veces seguidas la misma letra.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                DateTime fechaPago = Fecha_Pago.SelectedDate.HasValue ? Fecha_Pago.SelectedDate.Value : DateTime.MinValue;

                string estadoSeleccionado = ((ComboBoxItem)cmbEstado.SelectedItem)?.Content.ToString();
                string[] partesEstado = estadoSeleccionado?.Split('-');
                string numeroEstado = partesEstado[0].Trim();
                if (!int.TryParse(numeroEstado, out int idEstado))
                {
                    MessageBox.Show("El estado seleccionado no es válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                NuevoTransporte = new Transporte
                {
                    ID_Placa = TransporteSeleccionado.ID_Placa, 
                    Color = txtColor.Text,
                    Fecha_Pago = fechaPago,
                    ID_Estado = numeroEstado
                };

                TransporteDAL.ActualizarTransporte(NuevoTransporte);
                MessageBox.Show("Transporte modificado correctamente.");
                OnTransporteModificado();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al modificar el transporte: " + ex.Message);
            }

            this.Close();
        }

        private void txtColor_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            // Combinar el texto actual con el texto que se está ingresando
            string nuevoTexto = textBox.Text + e.Text;

            if (!Validaciones.ValidarColor(nuevoTexto, out string mensajeError))
            {
                e.Handled = true; // Detener la entrada de texto no válida
                MessageBox.Show(mensajeError, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InputControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Llama al método de Validaciones para bloquear copiar, pegar y cortar
            Validaciones.BloquearControles(e);
        }

        private void txtColor_LostFocus(object sender, RoutedEventArgs e)
        {
            // Formatea el texto del control de texto (txtColor)
            txtColor.Text = Validaciones.FormatearTexto(txtColor.Text);
        }
    }
}

