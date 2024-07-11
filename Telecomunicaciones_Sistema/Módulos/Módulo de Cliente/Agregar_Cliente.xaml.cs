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

namespace Telecomunicaciones_Sistema
{
    public partial class Agregar_Cliente : Window
    {
        // Eventos para notificar cuando se agrega o modifica un cliente
        public event EventHandler ClienteAgregado;

        private List<Clientes> clientes; // Lista para almacenar clientes 

        // Propiedad para acceder al nuevo cliente desde fuera de la clase
        public Clientes NuevoCliente { get; private set; }

        // Constructor para la ventana de agregar o modificar cliente
        public Agregar_Cliente()
        {
            InitializeComponent();
            MostrarNuevoID();

            // Inicializar NuevoCliente
            NuevoCliente = new Clientes();
        }

        private void MostrarNuevoID()
        {
            // Obtener el nuevo ID generado
            int nuevoID = GenerarNuevoID();
            // Asignar el nuevo ID al campo txtIDC
            txtIDC.Text = nuevoID.ToString();
        }

        private void BtnAceptar_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                // Validar campos del cliente
                if (!Validaciones.NoContieneEspaciosEnBlanco(txtNombreC.Text) || !Validaciones.NoContieneEspaciosEnBlanco(txtApellidoC.Text) || !Validaciones.NoContieneEspaciosEnBlanco(txtTelefonoC.Text) || !Validaciones.NoContieneEspaciosEnBlanco(txtCorreoC.Text))
                {
                    MessageBox.Show("Todos los campos del cliente deben llenarse.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string idCliente = txtIDC.Text;
                string numeroDireccion = "";
                string correo = txtCorreoC.Text;
                ComboBoxItem itemSeleccionado = (ComboBoxItem)cmbDire.SelectedItem;
                string direccion = itemSeleccionado?.Content?.ToString();
                string telefono = txtTelefonoC.Text;

                if (telefono.Length > 8)
                {
                    MessageBox.Show("El número de teléfono no puede tener más de 8 dígitos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Verificar si se seleccionó una dirección
                if (string.IsNullOrEmpty(direccion))
                {
                    MessageBox.Show("Por favor, seleccione una dirección.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!Validaciones.NombreValido(txtNombreC.Text))
                {
                    MessageBox.Show("El nombre no es válido. No se permiten espacios en blanco al inicio ni entre caracteres.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!Validaciones.TresVecesSeguidas(txtNombreC.Text))
                {
                    MessageBox.Show("El nombre no es válido. No se permiten más de 3 veces seguidas la misma letra.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!Validaciones.NombreV(txtNombreC.Text))
                {
                    MessageBox.Show("El nombre no es válido. Debe tener al menos 3 letras.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!Validaciones.ApellidoValido(txtApellidoC.Text))
                {
                    MessageBox.Show("El apellido no es válido. No se permiten espacios en blanco al inicio ni entre caracteres.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!Validaciones.TresVecesSeguidas(txtApellidoC.Text))
                {
                    MessageBox.Show("El apellido no es válido. No se permiten más de 3 veces seguidas la misma letra.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!Validaciones.ApellidoV(txtApellidoC.Text))
                {
                    MessageBox.Show("El apellido no es válido. Debe tener al menos 3 letras.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!Validaciones.NoContieneEspaciosEnBlancoEnNumero(txtTelefonoC.Text))
                {
                    MessageBox.Show("El teléfono no debe contener espacios en blanco.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!Validaciones.EsTelefonoValido(txtTelefonoC.Text))
                {
                    MessageBox.Show("El número de teléfono debe empezar con 3, 8 o 9.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!Validaciones.TelefonoValido(txtTelefonoC.Text))
                {
                    MessageBox.Show("El número de teléfono no puede más de cinco números repetidos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!Validaciones.CorreoSinEspacios(txtCorreoC.Text))
                {
                    MessageBox.Show("El correo electrónico no es válido. No se permiten espacios en blanco.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!Validaciones.CorreoArrobas(txtCorreoC.Text))
                {
                    MessageBox.Show("El correo electrónico no puede contener más de un símbolo de arroba (@).", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!Validaciones.CorreoValidoEstructura(txtCorreoC.Text))
                {
                    MessageBox.Show("El correo electrónico no es válido. Debe tener un formato válido (nombre@dominio).", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!Validaciones.CorreoTresLetras(correo))
                {
                    MessageBox.Show("El correo electrónico debe tener al menos 3 letras antes del símbolo de arroba (@).", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!Validaciones.CorreoValidoDominio(txtCorreoC.Text))
                {
                    MessageBox.Show("El correo electrónico debe tener un dominio válido (gmail.com, yahoo.com, hotmail.com).", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Extraer solo el número de la dirección
                string[] partesDireccion = direccion.Split('-');
                numeroDireccion = partesDireccion[0].Trim();

                // Verificar si algún campo del cliente está vacío
                if (Validaciones.CamposClienteVacios(txtNombreC.Text, txtApellidoC.Text, txtTelefonoC.Text, txtCorreoC.Text, numeroDireccion, cmbDire))
                {
                    MessageBox.Show("Todos los campos del cliente deben llenarse.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Verificar si el ID del cliente ya existe en la base de datos
                bool clienteExistente = ClienteDAL.ClienteExiste(idCliente);

                // Si el cliente ya existe, mostrar un mensaje de error
                if (clienteExistente)
                {
                    MessageBox.Show("El cliente con este ID ya existe en la base de datos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Crear el objeto NuevoCliente con los datos del nuevo cliente
                NuevoCliente = new Clientes
                {
                    ID_Cliente = idCliente,
                    Nombre = txtNombreC.Text,
                    Apellido = txtApellidoC.Text,
                    Correo = txtCorreoC.Text,
                    ID_Dirección = numeroDireccion
                };

                // Verificar si el texto del campo de teléfono es un número válido
                if (!decimal.TryParse(txtTelefonoC.Text, out decimal telefonoDecimal))
                {
                    MessageBox.Show("El número de teléfono debe ser un número válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Asignar el valor convertido a decimal al Teléfono del NuevoCliente
                NuevoCliente.Teléfono = telefonoDecimal;

                // Agregar el nuevo cliente a la base de datos
                ClienteDAL.AgregarCliente(NuevoCliente);
                MessageBox.Show("Cliente agregado correctamente.");

                // Llama al evento ClienteAgregado antes de cerrar la ventana
                OnClienteAgregado();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar el cliente: " + ex.Message);
            }

            // Cierra la ventana después de procesar el cliente
            this.Close();
        }

        private void txtTelefonoC_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (!Validaciones.ValidarTelefonoLongCar(textBox.Text, e.Text, out string mensajeError))
            {
                e.Handled = true;
                MessageBox.Show(mensajeError, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtTelefonoE_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!Validaciones.ValidarTeclaEspacioTel(e, out string mensajeError))
            {
                e.Handled = true;
                MessageBox.Show(mensajeError, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }   
            Validaciones.BloquearControles(e);
        }

        private void txtNombreC_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (!Validaciones.ValidarNombreLongNumCar(textBox.Text, e.Text, out string mensajeError))
            {
                e.Handled = true;
                MessageBox.Show(mensajeError, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtApellidoC_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (!Validaciones.ValidarApellidoLongNumCar(textBox.Text, e.Text, out string mensajeError))
            {
                e.Handled = true;
                MessageBox.Show(mensajeError, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtCorreoC_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (!Validaciones.ValidarCorreoLongitud(textBox.Text, e.Text, out string mensajeError))
            {
                e.Handled = true;
                MessageBox.Show(mensajeError, "Advertencia", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void txtCorreoC_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (!Validaciones.ValidarTeclaEspacioCorr(e, out string mensajeError))
            {
                e.Handled = true;
                MessageBox.Show(mensajeError, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Validaciones.BloquearControles(e);
        }

        // Método invocado cuando se agrega un cliente, activa el evento ClienteAgregado
        private void OnClienteAgregado()
        {
            ClienteAgregado?.Invoke(this, EventArgs.Empty);
        }
        private void BtnRegresar_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            Registro_Cliente frmPr = new Registro_Cliente();
        }

        private int GenerarNuevoID()
        {
            int nuevoID = 0;
            try
            {
                // Obtener el último ID registrado en la base de datos
                int ultimoIDRegistrado = ClienteDAL.ObtenerUltimoIDRegistrado();
                // Sumar 1 al último ID para obtener el nuevo ID
                nuevoID = ultimoIDRegistrado + 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar el nuevo ID: " + ex.Message);
            }
            return nuevoID;
        }

        // Método invocado cuando el campo de nombre pierde el foco
        private void TxtNombreC_LostFocus(object sender, RoutedEventArgs e)
        {
            // Formatea el texto del control de texto (txtNombreC)
            txtNombreC.Text = Validaciones.FormatearTexto(txtNombreC.Text);       
        }

        // Método invocado cuando el campo de apellido pierde el foco
        private void TxtApellidoC_LostFocus(object sender, RoutedEventArgs e)
        {
            // Formatea el texto del control de texto (txtApellidoC)
            txtApellidoC.Text = Validaciones.FormatearTexto(txtApellidoC.Text);
        }

        private void InputControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Llama al método de Validaciones para bloquear copiar, pegar y cortar
            Validaciones.BloquearControles(e);
        }
    }
}

