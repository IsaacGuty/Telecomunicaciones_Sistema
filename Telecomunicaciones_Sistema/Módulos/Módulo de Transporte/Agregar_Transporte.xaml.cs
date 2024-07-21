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
        // Declara un evento llamado TransporteAgregado. Este evento es del tipo EventHandler.
        public event EventHandler TransporteAgregado;

        // Propiedad pública llamada NuevoTransporte de tipo Transporte. 
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
            this.Hide(); // Oculta el formulario actual.
            Registro_Transporte frmPr = new Registro_Transporte(); // Crea una nueva instancia del formulario 'Registro_Transporte'.
            //frmPr.Show(); // Muestra el formulario 'Registro_Transporte'.
        }

        private void btnAceptar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Verifica que ninguno de los campos de entrada contenga espacios en blanco
                if (!Validaciones.NoContieneEspaciosEnBlanco(txtIDP.Text) ||
                    !Validaciones.NoContieneEspaciosEnBlanco(txtMarca.Text) ||
                    !Validaciones.NoContieneEspaciosEnBlanco(txtModelo.Text) ||
                    !Validaciones.NoContieneEspaciosEnBlanco(txtAño.Text) ||
                    !Validaciones.NoContieneEspaciosEnBlanco(txtColor.Text))
                {
                    // Muestra un mensaje de error si alguno de los campos tiene espacios en blanco
                    MessageBox.Show("Todos los campos del transporte deben llenarse.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Asigna los valores de los campos de entrada a variables locales
                string ID_Placa = txtIDP.Text;
                string marca = txtMarca.Text;
                string modelo = txtModelo.Text;
                string color = txtColor.Text;
                string año = txtAño.Text;

                // Verifica si la marca es válida, no debe tener espacios en blanco al inicio ni entre caracteres
                if (!Validaciones.MMValido(txtMarca.Text))
                {
                    // Muestra un mensaje de error si la marca no es válida
                    MessageBox.Show("La marca no es válida. No se permiten espacios en blanco al inicio ni entre caracteres.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Verifica si el modelo es válido, no debe tener espacios en blanco al inicio ni entre caracteres
                if (!Validaciones.MMValido(txtModelo.Text))
                {
                    // Muestra un mensaje de error si el modelo no es válido
                    MessageBox.Show("El modelo no es válido. No se permiten espacios en blanco al inicio ni entre caracteres.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Variable para almacenar un mensaje de error relacionado con la placa
                string mensajeError;

                // Verifica si la placa es válida utilizando el método ValidarPlaca
                if (!Validaciones.ValidarPlaca(txtIDP.Text, out mensajeError))
                {
                    MessageBox.Show(mensajeError, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Verifica si la marca contiene más de tres letras seguidas iguales
                if (!Validaciones.TresVecesSeguidas(txtMarca.Text))
                {
                    // Muestra un mensaje de error si la marca tiene más de tres letras seguidas iguales
                    MessageBox.Show("La marca no es válida. No se permiten más de 3 veces seguidas la misma letra.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Verifica si el modelo contiene más de tres letras seguidas iguales
                if (!Validaciones.TresVecesSeguidas(txtModelo.Text))
                {
                    // Muestra un mensaje de error si el modelo tiene más de tres letras seguidas iguales
                    MessageBox.Show("El modelo no es válida. No se permiten más de 3 veces seguidas la misma letra.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Verifica si el transporte ya existe en la base de datos utilizando el ID de la placa
                bool TransporteExistente = TransporteDAL.TransporteExiste(txtIDP.Text);

                // Si el transporte ya existe, muestra un mensaje de error
                if (TransporteExistente)
                {
                    MessageBox.Show("El transporte con este ID ya existe en la base de datos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Obtiene la fecha de pago seleccionada del control Fecha_Pago
                DateTime fechaPago = Fecha_Pago.SelectedDate.HasValue ? Fecha_Pago.SelectedDate.Value : DateTime.MinValue;

                // Obtiene el estado seleccionado del combo box cmbEstado y lo divide en partes
                string estadoSeleccionado = cmbEstado.SelectedItem.ToString();
                string[] partesEstado = estadoSeleccionado.Split('-');
                string numeroEstado = partesEstado[0].Trim();

                // Crea un nuevo objeto Transporte con los valores ingresados y la fecha de pago
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

                // Agrega el nuevo transporte a la base de datos utilizando el método AgregarTransporte
                TransporteDAL.AgregarTransporte(NuevoTransporte);

                // Muestra un mensaje de éxito indicando que el transporte se ha agregado correctamente
                MessageBox.Show("Transporte agregado correctamente.");

                // Llama al evento ClienteTransporte para realizar acciones adicionales antes de cerrar la ventana
                OnTransporteAgregado();
            }
            catch (Exception ex)
            {
                // Muestra un mensaje de error en caso de que ocurra una excepción durante el proceso
                MessageBox.Show("Error al agregar el transporte: " + ex.Message);
            }

            // Cierra la ventana después de procesar el transporte
            this.Close();
        }

        // Define un método privado que se llama OnTransporteAgregado.
        private void OnTransporteAgregado()
        {
            // Verifica si hay algún suscriptor al evento TransporteAgregado.
            TransporteAgregado?.Invoke(this, EventArgs.Empty);
        }


        private void txtIDP_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Verifica si la tecla presionada es un espacio y muestra un mensaje de error si es el caso.
            if (!Validaciones.ValidarTeclaEspacioIDP(e, out string mensajeError))
            {
                // Si la validación indica que la tecla es un espacio (o no válida),
                e.Handled = true;

                // Muestra un cuadro de mensaje con el mensaje de error proporcionado.
                MessageBox.Show(mensajeError, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // Llama a la función 'Validaciones.BloquearControles' para realizar cualquier acción adicional.
            Validaciones.BloquearControles(e);
        }

        private void txtAño_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Esta línea invoca el método `Validaciones.ValidarTeclaEspacioIDP` para validar si la tecla presionada
            // es un espacio, usando el evento de teclado proporcionado (e) y generando un mensaje de error si no es válido.
            if (!Validaciones.ValidarTeclaEspacioIDP(e, out string mensajeError))
            {
                // Si el método `ValidarTeclaEspacioIDP` devuelve false, se maneja el evento de la tecla
                e.Handled = true;

                // Muestra un mensaje de error utilizando `MessageBox.Show`. El mensaje de error es proporcionado por `mensajeError`
                MessageBox.Show(mensajeError, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // Esta línea llama al método `Validaciones.BloquearControles` para gestionar el estado de los controles de la interfaz
            Validaciones.BloquearControles(e);
        }


        private void txtIDP_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            // Combinar el texto actual con el texto que se está ingresando
            string nuevoTexto = textBox.Text + e.Text;

            // Validar el texto combinado
            if (!Validaciones.ValidarIDPC(nuevoTexto, out string mensajeError))
            {
                e.Handled = true; // Detener la entrada de texto no válida
                MessageBox.Show(mensajeError, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtMarca_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            // Combinar el texto actual con el texto que se está ingresando
            string nuevoTexto = textBox.Text + e.Text;

            // Validar el texto combinado
            if (!Validaciones.ValidarMarca(nuevoTexto, out string mensajeError))
            {
                e.Handled = true; // Detener la entrada de texto no válida
                MessageBox.Show(mensajeError, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtModelo_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            // Combinar el texto actual con el texto que se está ingresando
            string nuevoTexto = textBox.Text + e.Text;

            // Validar el texto combinado
            if (!Validaciones.ValidarModelo(nuevoTexto, out string mensajeError))
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

