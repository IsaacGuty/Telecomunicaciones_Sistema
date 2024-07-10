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
    public partial class AM_Cliente : Window
    {
        // Eventos para notificar cuando se agrega o modifica un cliente
        public event EventHandler ClienteAgregado;

        public event EventHandler ClienteModificado;

        private List<Clientes> clientes; // Lista para almacenar clientes 

        // Propiedad para acceder al nuevo cliente desde fuera de la clase
        public Clientes NuevoCliente { get; private set; }

        // Variable para indicar si se está modificando un cliente existente
        private bool esModificacion;

        // Constructor para la ventana de agregar o modificar cliente
        public AM_Cliente(bool esModificacion)
        {
            InitializeComponent();
            MostrarNuevoID();
            this.esModificacion = esModificacion;
            if (esModificacion)
            {
                lblNom.Content = "Modificar cliente"; // Cambia el título de la ventana si se está modificando
            }
            else
            {
                lblNom.Content = "Agregar un nuevo cliente"; // Cambia el título de la ventana si se está agregando
            }

            // Inicializar NuevoCliente
            NuevoCliente = new Clientes();
        }

        // Constructor sobrecargado para la ventana de agregar o modificar cliente, recibiendo el cliente a modificar
        public AM_Cliente(Registro_Cliente.Clientes clienteSeleccionado, bool esModificacion)
        {
            InitializeComponent();
            this.esModificacion = esModificacion;
            Conn = BD.ObtenerConexion();
            clientes = new List<Clientes>();
            this.clienteSeleccionado = clienteSeleccionado;
            MostrarDetallesCliente(); // Muestra los detalles del cliente a modificar en los campos correspondientes
            ActualizarLabel();
        }

        // Método para actualizar el contenido de la etiqueta según si se está modificando o agregando un cliente
        private void ActualizarLabel()
        {
            if (esModificacion)
            {
                lblNom.Content = "Modificar cliente";
            }
            else
            {
                lblNom.Content = "Agregar un nuevo cliente";
            }
        }

        private void MostrarNuevoID()
        {
            if (!esModificacion)
            {
                // Obtener el nuevo ID generado
                int nuevoID = GenerarNuevoID();
                // Asignar el nuevo ID al campo txtIDC
                txtIDC.Text = nuevoID.ToString();
            }
        }

        // Constructor 
        public AM_Cliente(bool esModificacion, bool esOtraModificacion)
        {
            InitializeComponent();
            if (esOtraModificacion)
            {
                
            }
            Conn = BD.ObtenerConexion();
            clientes = new List<Clientes>();
        }

        // Constructor por defecto 
        public AM_Cliente()
        {
        }

        // Conexión a la base de datos SQL Server 
        private SqlConnection Conn;

        // Cliente seleccionado para modificar 
        private Registro_Cliente.Clientes clienteSeleccionado;

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

                // Si estamos en modo modificación y el cliente no existe, mostrar un mensaje de error
                if (esModificacion && !clienteExistente)
                {
                    MessageBox.Show("El cliente con este ID no existe en la base de datos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Si estamos en modo modificación y el cliente existe, verificar si la información es igual a la de otro cliente
                if (esModificacion && clienteExistente)
                {
                    int resultado = ClienteDAL.ClienteDI(idCliente, txtCorreoC.Text, txtTelefonoC.Text);

                    if (resultado == 1)
                    {
                        MessageBox.Show("El cliente con el mismo correo ya existe en la base de datos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    else if (resultado == 2)
                    {
                        MessageBox.Show("El cliente con el mismo teléfono ya existe en la base de datos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Crear el objeto NuevoCliente con los datos modificados
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

                    // Verificar si el teléfono cumple con los criterios de validación
                    if (!Validaciones.EsTelefonoValido(txtTelefonoC.Text))
                    {
                        MessageBox.Show("El número de teléfono debe empezar con 3, 8 o 9.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Verificar si el texto del campo de correo electrónico es válido
                    if (!Validaciones.CorreoValido(txtCorreoC.Text))
                    {
                        MessageBox.Show("El formato del correo electrónico no es válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Actualizar el cliente existente en la base de datos
                    ClienteDAL.ActualizarCliente(NuevoCliente);
                    MessageBox.Show("Cliente modificado correctamente.");
                }
                // Si estamos en modo agregado y el cliente no existe, agregar el nuevo cliente
                else if (!esModificacion && !clienteExistente)
                {
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

                    // Verificar si el teléfono cumple con los criterios de validación
                    if (!Validaciones.EsTelefonoValido(txtTelefonoC.Text))
                    {
                        MessageBox.Show("El número de teléfono debe empezar con 3, 8 o 9.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Verificar si el texto del campo de correo electrónico es válido
                    if (!Validaciones.CorreoValido(txtCorreoC.Text))
                    {
                        MessageBox.Show("El formato del correo electrónico no es válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Asignar el valor convertido a decimal al Teléfono del NuevoCliente
                    NuevoCliente.Teléfono = telefonoDecimal;

                    // Verificar si ya existe un cliente con los mismos datos en la base de datos
                    int resultado = ClienteDAL.ClienteDI(idCliente, txtCorreoC.Text, txtTelefonoC.Text);

                    if (resultado == 1)
                    {
                        MessageBox.Show("El cliente con el mismo correo ya existe en la base de datos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    else if (resultado == 2)
                    {
                        MessageBox.Show("El cliente con el mismo teléfono ya existe en la base de datos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Agregar el nuevo cliente a la base de datos
                    ClienteDAL.AgregarCliente(NuevoCliente);
                    MessageBox.Show("Cliente agregado correctamente.");

                    // Llama al evento ClienteAgregado antes de cerrar la ventana
                    OnClienteAgregado();
                }
                // Si estamos en modo agregado y el cliente ya existe, mostrar un mensaje de error
                else if (!esModificacion && clienteExistente)
                {
                    MessageBox.Show("El cliente con este ID ya existe en la base de datos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al modificar/agregar el cliente: " + ex.Message);
            }

            // Cierra la ventana después de procesar el cliente
            this.Close();
        }

        private void txtTelefonoC_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            // Verifica si el texto resultante después de agregar el nuevo carácter excederá la longitud máxima permitida
            if (textBox.Text.Length + e.Text.Length > 8)
            {
                // Si excede la longitud máxima permitida, marca el evento como manejado para evitar que se agregue el nuevo carácter
                e.Handled = true;

                // Muestra un mensaje informativo al usuario
                MessageBox.Show("El número de teléfono no puede tener más de 8 dígitos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Verifica si el carácter ingresado es un dígito
            if (!char.IsDigit(e.Text, 0))
            {
                // Si el carácter no es un dígito, marca el evento como manejado para evitar que se agregue
                e.Handled = true;

                if (char.IsLetter(e.Text, 0))
                {
                    // Muestra un mensaje informativo al usuario sobre letras
                    MessageBox.Show("No se permiten letras en el número de teléfono.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    // Muestra un mensaje informativo al usuario sobre caracteres especiales
                    MessageBox.Show("No se permiten caracteres especiales en el número de teléfono.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void txtTelefonoE_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            // Verifica si la tecla presionada es la barra espaciadora
            if (e.Key == Key.Space)
            {
                // Marca el evento como manejado para evitar que se agregue el espacio
                e.Handled = true;

                // Muestra un mensaje informativo al usuario
                MessageBox.Show("No se permiten espacios en blanco en el número de teléfono.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Validaciones.BloquearControles(e);
        }

        private void txtNombreC_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Verificar si se ha alcanzado el límite de 50 caracteres
            if (txtNombreC.Text.Length + e.Text.Length > 50)
            {
                e.Handled = true;
                MessageBox.Show("Se ha alcanzado el límite máximo de 50 caracteres en el campo de nombre.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            // Verificar si se ingresaron números
            else if (System.Text.RegularExpressions.Regex.IsMatch(e.Text, "[0-9]"))
            {
                e.Handled = true;
                MessageBox.Show("No se permiten números en el campo de nombre.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            // Verificar si se ingresaron caracteres especiales
            else if (System.Text.RegularExpressions.Regex.IsMatch(e.Text, "[^a-zA-ZáéíóúÁÉÍÓÚñÑ]"))
            {
                e.Handled = true;
                MessageBox.Show("No se permiten caracteres especiales en el campo de nombre.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TxtNombreC_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
                MessageBox.Show("No se permiten espacios en el campo de nombre.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void txtApellidoC_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Verificar si se ha alcanzado el límite de 50 caracteres
            if (txtApellidoC.Text.Length + e.Text.Length > 50)
            {
                e.Handled = true;
                MessageBox.Show("Se ha alcanzado el límite máximo de 50 caracteres en el campo de apellido.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            // Verificar si se ingresaron números
            else if (System.Text.RegularExpressions.Regex.IsMatch(e.Text, "[0-9]"))
            {
                e.Handled = true;
                MessageBox.Show("No se permiten números en el campo de apellido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            // Verificar si se ingresaron caracteres especiales
            else if (System.Text.RegularExpressions.Regex.IsMatch(e.Text, "[^a-zA-ZáéíóúÁÉÍÓÚñÑ]"))
            {
                e.Handled = true;
                MessageBox.Show("No se permiten caracteres especiales en el campo de apellido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TxtApellidoC_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
                MessageBox.Show("No se permiten espacios en el campo de apellido.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void txtCorreoC_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Verificar si se ha alcanzado el límite de 50 caracteres
            if (txtCorreoC.Text.Length + e.Text.Length > 40)
            {
                e.Handled = true;
                MessageBox.Show("Se ha alcanzado el límite máximo de 50 caracteres en el campo de correo.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void txtCorreoC_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            // Verifica si la tecla presionada es la barra espaciadora
            if (e.Key == Key.Space)
            {
                // Marca el evento como manejado para evitar que se agregue el espacio
                e.Handled = true;

                // Muestra un mensaje informativo al usuario
                MessageBox.Show("No se permiten espacios en blanco en el campo de correo.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Validaciones.BloquearControles(e);
        }

        // Método invocado cuando se agrega un cliente, activa el evento ClienteAgregado
        private void OnClienteAgregado()
        {
            ClienteAgregado?.Invoke(this, EventArgs.Empty);
        }

        // Método para mostrar los detalles del cliente seleccionado para modificar
        private void MostrarDetallesCliente()
        {
            txtIDC.Text = clienteSeleccionado.ID_Cliente;
            txtNombreC.Text = clienteSeleccionado.Nombre;
            txtApellidoC.Text = clienteSeleccionado.Apellido;
            txtCorreoC.Text = clienteSeleccionado.Correo;
            txtTelefonoC.Text = clienteSeleccionado.Teléfono;

            // Obtener el valor actual del campo de dirección del cliente seleccionado
            string direccionSeleccionada = clienteSeleccionado.ID_Dirección;

            // Iterar sobre los elementos del ComboBox para seleccionar el que coincida con la dirección del cliente seleccionado
            foreach (ComboBoxItem item in cmbDire.Items)
            {
                string direccion = item.Content?.ToString().Split('-')[0].Trim(); // Obtener solo el número de dirección
                if (direccion == direccionSeleccionada)
                {
                    // Establecer este elemento como seleccionado en el ComboBox
                    cmbDire.SelectedItem = item;
                    break; // Salir del bucle una vez que se haya encontrado la dirección correcta
                }
            }
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

