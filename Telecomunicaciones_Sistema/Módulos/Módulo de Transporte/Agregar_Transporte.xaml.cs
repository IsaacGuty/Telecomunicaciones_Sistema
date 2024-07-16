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
    public partial class Agregar_Transporte : Window
    {
        public event EventHandler TransporteAgregado;

        public Transporte NuevoTransporte { get; private set; }

        public Agregar_Transporte()
        {
            InitializeComponent();
            cmbEstado.Items.Clear(); // Limpiar cualquier elemento existente
            cmbEstado.Items.Add("1 - Activo"); // Agregar solo la opción "Activo"
            cmbEstado.SelectedIndex = 0; // Establecer "Activo" como seleccionado
        }

        private void btnRegresar_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            Registro_Transporte frmPr = new Registro_Transporte();
        }

        private void btnAceptar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!Validaciones.NoContieneEspaciosEnBlanco(txtIDP.Text) || !Validaciones.NoContieneEspaciosEnBlanco(txtMarca.Text) || !Validaciones.NoContieneEspaciosEnBlanco(txtModelo.Text) || !Validaciones.NoContieneEspaciosEnBlanco(txtAño.Text))
                {
                    MessageBox.Show("Todos los campos del cliente deben llenarse.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string ID_Placa = txtIDP.Text;
                string marca = txtMarca.Text;
                string modelo = txtModelo.Text;
                string color = txtColor.Text;
                string año = txtAño.Text;

                if (!Validaciones.MMValido(txtMarca.Text))
                {
                    MessageBox.Show("La marca no es válida. No se permiten espacios en blanco al inicio ni entre caracteres.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!Validaciones.MMValido(txtModelo.Text))
                {
                    MessageBox.Show("La marca no es válida. No se permiten espacios en blanco al inicio ni entre caracteres.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string mensajeError;
                if (!Validaciones.ValidarPlaca(txtIDP.Text, out mensajeError))
                {
                    MessageBox.Show(mensajeError, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!Validaciones.TresVecesSeguidas(txtMarca.Text))
                {
                    MessageBox.Show("La marca no es válida. No se permiten más de 3 veces seguidas la misma letra.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!Validaciones.TresVecesSeguidas(txtModelo.Text))
                {
                    MessageBox.Show("El modelo no es válida. No se permiten más de 3 veces seguidas la misma letra.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                bool TransporteExistente = TransporteDAL.TransporteExiste(txtIDP.Text);

                // Si el transporte ya existe, mostrar un mensaje de error
                if (TransporteExistente)
                {
                    MessageBox.Show("El transporte con este ID ya existe en la base de datos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
                DateTime fechaPago = Fecha_Pago.SelectedDate.HasValue ? Fecha_Pago.SelectedDate.Value : DateTime.MinValue;

                string estadoSeleccionado = cmbEstado.SelectedItem.ToString();
                string[] partesEstado = estadoSeleccionado.Split('-');
                string numeroEstado = partesEstado[0].Trim();

                NuevoTransporte = new Transporte
                {
                    ID_Placa = txtIDP.Text,
                    Marca_Carro = txtMarca.Text,
                    Modelo_Carro = txtModelo.Text,
                    Color = txtColor.Text,
                    Fecha_Pago = fechaPago,
                    Año_Carro = txtAño.Text,
                    ID_Estado = numeroEstado
                };

                // Agregar el nuevo transporte a la base de datos
                TransporteDAL.AgregarTransporte(NuevoTransporte);
                MessageBox.Show("Transporte agregado correctamente.");

                // Llama al evento ClienteTransporte antes de cerrar la ventana
                OnTransporteAgregado();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar el transporte: " + ex.Message);
            }

            // Cierra la ventana después de procesar el transporte
            this.Close();
        }

        private void OnTransporteAgregado()
        {
            TransporteAgregado?.Invoke(this, EventArgs.Empty);
        }

        private void txtIDP_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!Validaciones.ValidarTeclaEspacioIDP(e, out string mensajeError))
            {
                e.Handled = true;
                MessageBox.Show(mensajeError, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Validaciones.BloquearControles(e);
        }

        private void txtAño_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!Validaciones.ValidarTeclaEspacioIDP(e, out string mensajeError))
            {
                e.Handled = true;
                MessageBox.Show(mensajeError, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Validaciones.BloquearControles(e);
        }

        private void txtIDP_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            // Combinar el texto actual con el texto que se está ingresando
            string nuevoTexto = textBox.Text + e.Text;

            if (!Validaciones.ValidarIDPC(nuevoTexto, out string mensajeError))
            {
                e.Handled = true; // Detener la entrada de texto no válida
                MessageBox.Show(mensajeError, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

        private void txtAño_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            // Combinar el texto actual con el texto que se está ingresando
            string nuevoTexto = textBox.Text + e.Text;

            if (!Validaciones.ValidarAño(nuevoTexto, out string mensajeError))
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

        private void txtIDP_LostFocus(object sender, RoutedEventArgs e)
        {
            // Formatea el texto del control de texto (txtIDP)
            txtIDP.Text = Validaciones.FormatearTexto(txtIDP.Text);

        }

        private void txtMarca_LostFocus(object sender, RoutedEventArgs e)
        {
            // Formatea el texto del control de texto (txtMarca)
            txtMarca.Text = Validaciones.FormatearTexto(txtMarca.Text);
        }

        private void txtModelo_LostFocus(object sender, RoutedEventArgs e)
        {
            // Formatea el texto del control de texto (txtModelo)
            txtModelo.Text = Validaciones.FormatearTexto(txtModelo.Text);
        }

        private void txtColor_LostFocus(object sender, RoutedEventArgs e)
        {
            // Formatea el texto del control de texto (txtColor)
            txtColor.Text = Validaciones.FormatearTexto(txtColor.Text);
        }
    }
}

