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
    // Ventana para agregar clientes en el sistema de telecomunicaciones
    public partial class Agregar_Cliente : Window
    {
        // Evento que se activa cuando un cliente ha sido agregado
        public event EventHandler ClienteAgregado;

        // Lista para almacenar instancias de clientes
        private List<Clientes> clientes;

        // Propiedad para acceder al nuevo cliente desde fuera de la clase
        public Clientes NuevoCliente { get; private set; }

        // Constructor de la ventana de agregar o modificar cliente
        public Agregar_Cliente()
        {
            InitializeComponent();
            MostrarNuevoID();

            // Inicializa el objeto NuevoCliente
            NuevoCliente = new Clientes();
        }

        // Método para mostrar el nuevo ID generado para el cliente
        private void MostrarNuevoID()
        {
            int nuevoID = GenerarNuevoID(); // Obtiene el nuevo ID
            txtIDC.Text = nuevoID.ToString(); // Asigna el nuevo ID al campo de texto
        }

        // Maneja el clic en el botón Aceptar
        private void BtnAceptar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validaciones para asegurar que los campos del cliente no contengan espacios en blanco
                if (!Validaciones.NoContieneEspaciosEnBlanco(txtNombreC.Text) ||
                    !Validaciones.NoContieneEspaciosEnBlanco(txtApellidoC.Text) ||
                    !Validaciones.NoContieneEspaciosEnBlanco(txtTelefonoC.Text) ||
                    !Validaciones.NoContieneEspaciosEnBlanco(txtCorreoC.Text))
                {
                    MessageBox.Show("Todos los campos del cliente deben llenarse.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string idCliente = txtIDC.Text; // Obtiene el ID del cliente
                string numeroDireccion = "";
                string correo = txtCorreoC.Text; // Obtiene el correo del cliente
                ComboBoxItem itemSeleccionado = (ComboBoxItem)cmbDire.SelectedItem; // Obtiene la dirección seleccionada
                string direccion = itemSeleccionado?.Content?.ToString();
                string telefono = txtTelefonoC.Text; // Obtiene el teléfono del cliente

                // Validaciones específicas para el teléfono
                if (telefono.Length > 8)
                {
                    MessageBox.Show("El número de teléfono no puede tener más de 8 dígitos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Verifica si se ha seleccionado una dirección
                if (string.IsNullOrEmpty(direccion))
                {
                    MessageBox.Show("Por favor, seleccione una dirección.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Validaciones del nombre
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

                // Validaciones del apellido
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

                // Validaciones adicionales del teléfono
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
                    MessageBox.Show("El número de teléfono no puede tener más de cinco números repetidos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Validaciones del correo electrónico
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

                // Extrae solo el número de la dirección
                string[] partesDireccion = direccion.Split('-');
                numeroDireccion = partesDireccion[0].Trim();

                // Verifica si algún campo del cliente está vacío
                if (Validaciones.CamposClienteVacios(txtNombreC.Text, txtApellidoC.Text, txtTelefonoC.Text, txtCorreoC.Text, numeroDireccion, cmbDire))
                {
                    MessageBox.Show("Todos los campos del cliente deben llenarse.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                int resultados = ClienteDAL.ClienteExisteConDatosMod(idCliente, correo, telefono);

                // Manejar el resultado según el tipo de duplicado encontrado
                switch (resultados)
                {
                    case 1:
                        MessageBox.Show("El cliente con el mismo correo ya existe en la base de datos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    case 2:
                        MessageBox.Show("El cliente con el mismo teléfono ya existe en la base de datos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    case 3:
                        MessageBox.Show("El ID del cliente ya está registrado en la base de datos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                }

                // Crea una instancia del cliente con los datos ingresados
                NuevoCliente = new Clientes
                {
                    ID_Cliente = idCliente,
                    Nombre = txtNombreC.Text,
                    Apellido = txtApellidoC.Text,
                    Correo = txtCorreoC.Text,
                    ID_Dirección = numeroDireccion
                };

                // Verifica si el texto del campo de teléfono es un número válido
                if (!decimal.TryParse(txtTelefonoC.Text, out decimal telefonoDecimal))
                {
                    MessageBox.Show("El número de teléfono debe ser un número válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Asigna el valor convertido a decimal al Teléfono del NuevoCliente
                NuevoCliente.Teléfono = telefonoDecimal;

                // Agrega el nuevo cliente a la base de datos
                ClienteDAL.AgregarCliente(NuevoCliente);
                MessageBox.Show("Cliente agregado correctamente.");

                // Activa el evento ClienteAgregado antes de cerrar la ventana
                OnClienteAgregado();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar el cliente: " + ex.Message);
            }

            // Cierra la ventana después de procesar el cliente
            this.Close();
        }

        // Valida la entrada del texto en el campo de teléfono para evitar caracteres no permitidos
        private void txtTelefonoC_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (!Validaciones.ValidarTelefonoLongCar(textBox.Text, e.Text, out string mensajeError))
            {
                e.Handled = true;
                MessageBox.Show(mensajeError, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Valida la entrada de teclas en el campo de teléfono para evitar espacios
        private void txtTelefonoE_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!Validaciones.ValidarTeclaEspacioTel(e, out string mensajeError))
            {
                e.Handled = true;
                MessageBox.Show(mensajeError, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Validaciones.BloquearControles(e);
        }

        // Valida la entrada del texto en el campo de nombre para evitar caracteres no permitidos
        private void txtNombreC_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (!Validaciones.ValidarNombreLongNumCar(textBox.Text, e.Text, out string mensajeError))
            {
                e.Handled = true;
                MessageBox.Show(mensajeError, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Valida la entrada del texto en el campo de apellido para evitar caracteres no permitidos
        private void txtApellidoC_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (!Validaciones.ValidarApellidoLongNumCar(textBox.Text, e.Text, out string mensajeError))
            {
                e.Handled = true;
                MessageBox.Show(mensajeError, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Valida la entrada del texto en el campo de correo electrónico para evitar caracteres no permitidos
        private void txtCorreoC_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (!Validaciones.ValidarCorreoLongitud(textBox.Text, e.Text, out string mensajeError))
            {
                e.Handled = true;
                MessageBox.Show(mensajeError, "Advertencia", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // Valida la entrada de teclas en el campo de correo electrónico para evitar espacios
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

        // Maneja el clic en el botón Regresar para ocultar la ventana actual y abrir el registro de clientes
        private void BtnRegresar_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            Registro_Cliente frmPr = new Registro_Cliente();
        }

        // Genera un nuevo ID para el cliente basado en el último ID registrado
        private int GenerarNuevoID()
        {
            int nuevoID = 0;
            try
            {
                int ultimoIDRegistrado = ClienteDAL.ObtenerUltimoIDRegistrado(); // Obtiene el último ID registrado
                nuevoID = ultimoIDRegistrado + 1; // Calcula el nuevo ID
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar el nuevo ID: " + ex.Message);
            }
            return nuevoID;
        }

        // Formatea el texto del campo de nombre cuando pierde el foco
        private void TxtNombreC_LostFocus(object sender, RoutedEventArgs e)
        {
            txtNombreC.Text = Validaciones.FormatearTexto(txtNombreC.Text); // Aplica el formato al texto
        }

        // Formatea el texto del campo de apellido cuando pierde el foco
        private void TxtApellidoC_LostFocus(object sender, RoutedEventArgs e)
        {
            txtApellidoC.Text = Validaciones.FormatearTexto(txtApellidoC.Text); // Aplica el formato al texto
        }

        // Maneja la entrada de teclas en los controles para bloquear copiar, pegar y cortar
        private void InputControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Validaciones.BloquearControles(e); // Bloquea operaciones no permitidas
        }
    }
}
