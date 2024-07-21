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
        // Declara un evento llamado TransporteModificado que será utilizado para notificar cuando se realice una modificación en el transporte.
        public event EventHandler TransporteModificado;

        // Declara una variable de instancia para manejar la conexión a la base de datos.
        private SqlConnection Conn;

        // Declara una variable de instancia para almacenar el transporte seleccionado que será modificado.
        private Registro_Transporte.Transportes TransporteSeleccionado;

        // Declara una variable de instancia para indicar si la operación es una modificación o una adición de transporte.
        private bool esModificacion;

        // Propiedad pública para acceder al nuevo transporte creado o modificado. Solo puede ser establecido desde dentro de la clase.
        public Transporte NuevoTransporte { get; private set; }

        // Constructor de la clase Modificar_Transporte. Se llama al crear una nueva instancia de la clase.
        public Modificar_Transporte(Registro_Transporte.Transportes TransporteSeleccionado, bool esModificacion)
        {
            // Inicializa los componentes de la interfaz de usuario definidos en el archivo XAML asociado.
            InitializeComponent();

            // Establece si la operación es una modificación (true) o una adición (false).
            this.esModificacion = esModificacion;

            // Asigna el transporte seleccionado a la variable de instancia.
            this.TransporteSeleccionado = TransporteSeleccionado;

            // Llama a un método para mostrar los detalles del transporte en la interfaz de usuario.
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
                // Verifica si alguno de los campos 'txtColor' o 'Fecha_Pago' contiene espacios en blanco usando la clase Validaciones.
                if (!Validaciones.NoContieneEspaciosEnBlanco(txtColor.Text) || !Validaciones.NoContieneEspaciosEnBlanco(Fecha_Pago.Text))
                {
                    // Muestra un mensaje de error si los campos contienen espacios en blanco.
                    MessageBox.Show("Todos los campos del transporte deben llenarse.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return; // Sale del método si hay un error en los campos.
                }

                // Asigna el valor del campo 'txtColor' a la variable 'color'.
                string color = txtColor.Text;

                // Verifica si el valor del campo 'txtColor' es válido según las reglas definidas en la clase Validaciones.
                if (!Validaciones.MMValido(txtColor.Text))
                {
                    // Muestra un mensaje de error si el color no es válido (espacios en blanco al inicio o entre caracteres).
                    MessageBox.Show("El color no es válido. No se permiten espacios en blanco al inicio ni entre caracteres.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return; // Sale del método si hay un error en el color.
                }

                // Verifica si el valor del campo 'txtColor' contiene más de 3 letras seguidas utilizando la clase Validaciones.
                if (!Validaciones.TresVecesSeguidas(txtColor.Text))
                {
                    // Muestra un mensaje de error si hay más de 3 letras seguidas.
                    MessageBox.Show("El color no es válido. No se permiten más de 3 veces seguidas la misma letra.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return; // Sale del método si hay un error en el color.
                }

                // Obtiene la fecha seleccionada del control 'Fecha_Pago', si está disponible; de lo contrario, asigna la fecha mínima.
                DateTime fechaPago = Fecha_Pago.SelectedDate.HasValue ? Fecha_Pago.SelectedDate.Value : DateTime.MinValue;

                // Obtiene el estado seleccionado del ComboBox 'cmbEstado', lo divide en partes y extrae el número del estado.
                string estadoSeleccionado = ((ComboBoxItem)cmbEstado.SelectedItem)?.Content.ToString();
                string[] partesEstado = estadoSeleccionado?.Split('-');
                string numeroEstado = partesEstado[0].Trim();

                // Intenta convertir el número del estado a un entero. Si no es válido, muestra un mensaje de error.
                if (!int.TryParse(numeroEstado, out int idEstado))
                {
                    MessageBox.Show("El estado seleccionado no es válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return; // Sale del método si el estado no es válido.
                }

                // Crea una nueva instancia de la clase 'Transporte' con los valores proporcionados y el estado del transporte seleccionado.
                NuevoTransporte = new Transporte
                {
                    ID_Placa = TransporteSeleccionado.ID_Placa,
                    Color = txtColor.Text,
                    Fecha_Pago = fechaPago,
                    ID_Estado = numeroEstado
                };

                // Llama al método 'ActualizarTransporte' de 'TransporteDAL' para actualizar el transporte en la base de datos.
                TransporteDAL.ActualizarTransporte(NuevoTransporte);
                // Muestra un mensaje de éxito indicando que el transporte se modificó correctamente.
                MessageBox.Show("Transporte modificado correctamente.");
                // Llama al evento 'OnTransporteModificado' para notificar que el transporte ha sido modificado.
                OnTransporteModificado();
            }
            catch (Exception ex)
            {
                // Muestra un mensaje de error si ocurre una excepción durante el proceso de modificación.
                MessageBox.Show("Error al modificar el transporte: " + ex.Message);
            }

            // Cierra la ventana actual después de realizar la actualización o si ocurre un error.
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

