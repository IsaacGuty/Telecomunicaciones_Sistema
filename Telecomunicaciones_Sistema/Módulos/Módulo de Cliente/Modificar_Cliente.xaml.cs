using System.Collections.Generic;
using System.Data.SqlClient;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace Telecomunicaciones_Sistema
{
    //Ventana para modificar la información de un cliente
    public partial class Modificar_Cliente : Window
    {
        // Evento que se activa cuando un cliente es modificado
        public event EventHandler ClienteModificado;

        // Variable para indicar si estamos en modo modificación
        private bool esModificacion;

        // Cliente seleccionado que se está modificando
        private Registro_Cliente.Clientes clienteSeleccionado;

        // Constructor de la ventana de modificación de cliente
        public Modificar_Cliente(Registro_Cliente.Clientes clienteSeleccionado)
        {
            InitializeComponent(); // Inicializa los componentes de la ventana
            this.clienteSeleccionado = clienteSeleccionado; // Asigna el cliente seleccionado a la variable de instancia
            this.esModificacion = true; // Establece que estamos en modo de modificación
            MostrarDetallesCliente(); // Llama al método para mostrar los detalles del cliente en los campos de entrada
        }

        // Propiedad que almacena el nuevo cliente con los datos modificados
        public Clientes NuevoCliente { get; private set; }

        // Maneja el clic en el botón Aceptar para guardar los cambios
        private void BtnAceptar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Valida que ninguno de los campos de texto esté vacío
                if (!Validaciones.NoContieneEspaciosEnBlanco(txtNombreC.Text) ||
                    !Validaciones.NoContieneEspaciosEnBlanco(txtApellidoC.Text) ||
                    !Validaciones.NoContieneEspaciosEnBlanco(txtTelefonoC.Text) ||
                    !Validaciones.NoContieneEspaciosEnBlanco(txtCorreoC.Text))
                {
                    MessageBox.Show("Todos los campos del cliente deben llenarse.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return; // Sale del método si la validación falla
                }

                // Obtiene los valores de los campos de texto y comboBox
                string idCliente = txtIDC.Text;
                string numeroDireccion = "";
                string correo = txtCorreoC.Text;
                ComboBoxItem itemSeleccionado = (ComboBoxItem)cmbDire.SelectedItem;
                string direccion = itemSeleccionado?.Content?.ToString();
                string telefono = txtTelefonoC.Text;

                // Verifica que el número de teléfono no tenga más de 8 dígitos
                if (telefono.Length > 8)
                {
                    MessageBox.Show("El número de teléfono no puede tener más de 8 dígitos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Verifica que se haya seleccionado una dirección
                if (string.IsNullOrEmpty(direccion))
                {
                    MessageBox.Show("Por favor, seleccione una dirección.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Valida el nombre del cliente con varias reglas
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

                // Valida el apellido del cliente con las mismas reglas que el nombre
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

                // Valida el teléfono del cliente
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

                // Valida el correo electrónico del cliente
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

                // Extrae el número de dirección de la dirección completa seleccionada
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

                // Crea un nuevo objeto Cliente con los datos modificados
                NuevoCliente = new Clientes
                {
                    ID_Cliente = idCliente,
                    Nombre = txtNombreC.Text,
                    Apellido = txtApellidoC.Text,
                    Correo = txtCorreoC.Text,
                    ID_Dirección = numeroDireccion,
                };

                // Verifica si el texto del campo de teléfono es un número válido
                if (!decimal.TryParse(txtTelefonoC.Text, out decimal telefonoDecimal))
                {
                    MessageBox.Show("El número de teléfono debe ser un número válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Asigna el número de teléfono convertido a decimal al nuevo cliente
                NuevoCliente.Teléfono = telefonoDecimal;

                // Actualiza el cliente existente en la base de datos con los nuevos datos
                ClienteDAL.ActualizarCliente(NuevoCliente);
                MessageBox.Show("Cliente modificado correctamente.");

                // Llama al evento ClienteModificado para notificar a otros componentes sobre la modificación
                OnClienteModificado();
            }
            catch (Exception ex)
            {
                // Muestra un mensaje de error si ocurre una excepción durante el proceso
                MessageBox.Show("Error al modificar el cliente: " + ex.Message);
            }

            // Cierra la ventana después de procesar la modificación
            this.Close();
        }

        // Método para activar el evento ClienteModificado
        private void OnClienteModificado()
        {
            ClienteModificado?.Invoke(this, EventArgs.Empty);
        }

        // Valida la entrada de texto en el campo de teléfono para evitar caracteres no permitidos
        private void txtTelefonoC_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (!Validaciones.ValidarTelefonoLongCar(textBox.Text, e.Text, out string mensajeError))
            {
                e.Handled = true; // Evita que el texto sea ingresado
                MessageBox.Show(mensajeError, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Maneja el evento de la tecla presionada en el campo de teléfono para evitar caracteres no permitidos
        private void txtTelefonoE_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!Validaciones.ValidarTeclaEspacioTel(e, out string mensajeError))
            {
                e.Handled = true; // Evita que se pueda ingresar espacios en blanco
                MessageBox.Show(mensajeError, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Validaciones.BloquearControles(e); // Bloquea acciones como copiar, pegar, cortar
        }

        // Valida la entrada de texto en el campo de nombre para evitar caracteres no permitidos
        private void txtNombreC_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (!Validaciones.ValidarNombreLongNumCar(textBox.Text, e.Text, out string mensajeError))
            {
                e.Handled = true; // Evita que el texto sea ingresado
                MessageBox.Show(mensajeError, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Valida la entrada de texto en el campo de apellido para evitar caracteres no permitidos
        private void txtApellidoC_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (!Validaciones.ValidarApellidoLongNumCar(textBox.Text, e.Text, out string mensajeError))
            {
                e.Handled = true; // Evita que el texto sea ingresado
                MessageBox.Show(mensajeError, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Valida la entrada de texto en el campo de correo electrónico para evitar caracteres no permitidos
        private void txtCorreoC_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (!Validaciones.ValidarCorreoLongitud(textBox.Text, e.Text, out string mensajeError))
            {
                e.Handled = true; // Evita que el texto sea ingresado
                MessageBox.Show(mensajeError, "Advertencia", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // Maneja el evento de la tecla presionada en el campo de correo electrónico para evitar caracteres no permitidos
        private void txtCorreoC_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (!Validaciones.ValidarTeclaEspacioCorr(e, out string mensajeError))
            {
                e.Handled = true; // Evita que se pueda ingresar espacios en blanco
                MessageBox.Show(mensajeError, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Validaciones.BloquearControles(e); // Bloquea acciones como copiar, pegar, cortar
        }

        // Muestra los detalles del cliente seleccionado en los campos de entrada
        private void MostrarDetallesCliente()
        {
            txtIDC.Text = clienteSeleccionado.ID_Cliente;
            txtNombreC.Text = clienteSeleccionado.Nombre;
            txtApellidoC.Text = clienteSeleccionado.Apellido;
            txtCorreoC.Text = clienteSeleccionado.Correo;
            txtTelefonoC.Text = clienteSeleccionado.Teléfono;

            // Obtiene el valor actual de la dirección del cliente seleccionado
            string direccionSeleccionada = clienteSeleccionado.ID_Dirección;

            // Itera sobre los elementos del ComboBox para seleccionar el que coincide con la dirección del cliente
            foreach (ComboBoxItem item in cmbDire.Items)
            {
                string direccion = item.Content?.ToString().Split('-')[0].Trim(); // Obtiene solo el número de dirección
                if (direccion == direccionSeleccionada)
                {
                    // Establece este elemento como seleccionado en el ComboBox
                    cmbDire.SelectedItem = item;
                    break; // Sale del bucle una vez que se haya encontrado la dirección correcta
                }
            }
        }

        // Maneja el clic en el botón Regresar para ocultar la ventana y abrir el formulario de registro de cliente
        private void BtnRegresar_Click(object sender, RoutedEventArgs e)
        {
            this.Hide(); // Oculta la ventana actual
            Registro_Cliente frmPr = new Registro_Cliente(); // Crea una nueva instancia del formulario de registro de cliente
        }

        // Método invocado cuando el campo de nombre pierde el foco para formatear el texto
        private void TxtNombreC_LostFocus(object sender, RoutedEventArgs e)
        {
            txtNombreC.Text = Validaciones.FormatearTexto(txtNombreC.Text); // Formatea el texto del campo de nombre
        }

        // Método invocado cuando el campo de apellido pierde el foco para formatear el texto
        private void TxtApellidoC_LostFocus(object sender, RoutedEventArgs e)
        {
            txtApellidoC.Text = Validaciones.FormatearTexto(txtApellidoC.Text); // Formatea el texto del campo de apellido
        }

        // Bloquea acciones como copiar, pegar, cortar en los campos de entrada
        private void InputControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Validaciones.BloquearControles(e); // Bloquea las acciones especificadas
        }
    }
}

