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
    /// <summary>
    /// Lógica de interacción para Window8.xaml
    /// </summary>
    public partial class Window8 : Window
    {
        // Declaración de eventos para notificar cuando se agrega o modifica un empleado
        public event EventHandler EmpleadoAgregado;

        public event EventHandler EmpleadoModificado;

        // Lista para almacenar empleados
        private List<Empleados> empleados;

        // Propiedad para obtener el nuevo empleado creado
        public Empleados NuevoEmpleado { get; private set; }

        // Variable para indicar si se está modificando un empleado
        private bool esModificacion;

        // Constructor para ventana de modificación/agregación de empleado
        public Window8(bool esModificacion)
        {
            InitializeComponent();
            this.esModificacion = esModificacion;
            // Actualiza la etiqueta según si se está modificando o agregando un empleado
            ActualizarLabel();

            if (!esModificacion)
            {
                cmbEstado.Items.Clear(); // Limpiar cualquier elemento existente
                cmbEstado.Items.Add("Activo"); // Agregar solo la opción "Activo"
                cmbEstado.SelectedIndex = 0; // Establecer "Activo" como seleccionado
            }
        }

        // Constructor para ventana de modificación de empleado con empleado seleccionado
        public Window8(Window6.Empleados empleadoSeleccionado, bool esModificacion)
        {
            InitializeComponent();
            this.esModificacion = esModificacion;
            // Conexión a la base de datos y carga de detalles del empleado seleccionado
            Conn = BD.ObtenerConexion();
            empleados = new List<Empleados>();
            this.empleadoSeleccionado = empleadoSeleccionado;
            MostrarDetallesEmpleado();
            // Actualiza la etiqueta según si se está modificando o agregando un empleado
            ActualizarLabel();

            if (esModificacion)
            {
                lblContra.Visibility = Visibility.Collapsed;
                passContraseña.Visibility = Visibility.Collapsed;
            }
        }

        // Método para actualizar la etiqueta según si se está modificando o agregando un empleado
        private void ActualizarLabel()
        {
            if (esModificacion)
            {
                lblNom.Content = "Modificar empleado";
            }
            else
            {
                lblNom.Content = "Agregar un nuevo empleado";
            }
        }

        // Constructor sin parámetros 
        public Window8()
        {
        }

        // Variables de clase
        private SqlConnection Conn;
        private Window6.Empleados empleadoSeleccionado;

        private void BtnAceptar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Validaciones.CamposEmpleadosVacios(txtIDE.Text, txtNombreE.Text, txtApellidoE.Text, txtTelefonoE.Text, txtCorreoE.Text, cmbDireccion.Text))
                {
                    MessageBox.Show("Todos los campos del empleado deben llenarse.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string idEmpleado = txtIDE.Text;

                if (idEmpleado.Length > 7)
                {
                    MessageBox.Show("El ID del empleado no puede tener más de 7 dígitos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string nombre = txtNombreE.Text;
                string apellido = txtApellidoE.Text;
                string correo = txtCorreoE.Text;
                string telefono = txtTelefonoE.Text;

                if (telefono.Length > 8)
                {
                    MessageBox.Show("El número de teléfono no puede tener más de 8 dígitos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                ComboBoxItem itemSeleccionado = (ComboBoxItem)cmbDireccion.SelectedItem;
                string direccion = itemSeleccionado?.Content?.ToString();

                int resultado = EmpleadoDAL.EmpleadoExisteConDatos(idEmpleado, correo, telefono);

                if (resultado == 1)
                {
                    MessageBox.Show("El empleado con el mismo correo ya existe en la base de datos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else if (resultado == 2)
                {
                    MessageBox.Show("El empleado con el mismo teléfono ya existe en la base de datos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Verificar si se seleccionó una dirección
                if (string.IsNullOrEmpty(direccion))
                {
                    MessageBox.Show("Por favor, seleccione una dirección.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Verificar si se ha seleccionado un puesto en el ComboBox
                if (cmbPuesto.SelectedItem == null)
                {
                    MessageBox.Show("Por favor, seleccione un puesto para el empleado.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!Validaciones.NoContieneEspaciosEnBlancoEnNumero(txtIDE.Text))
                {
                    MessageBox.Show("El ID no debe contener espacios en blanco.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (Validaciones.ValidarLongitudIDEmpleado(idEmpleado))
                {
                    MessageBox.Show("El ID del empleado no puede tener más de 7 caracteres.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return; 
                }

                if (!Validaciones.EsIDEmpleadoValido(idEmpleado))
                {
                    MessageBox.Show("El ID del empleado solo puede contener números.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return; 
                }

                if (!Validaciones.NombreValido(txtNombreE.Text))
                {
                    MessageBox.Show("El nombre no es válido. No se permiten espacios en blanco al inicio ni entre caracteres.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!Validaciones.TresVecesSeguidas(txtNombreE.Text))
                {
                    MessageBox.Show("El nombre no es válido. No se permiten más de 3 veces seguidas la misma letra.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!Validaciones.NombreV(txtNombreE.Text))
                {
                    MessageBox.Show("El nombre no es válido. Debe tener al menos 3 letras.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!Validaciones.ApellidoValido(txtApellidoE.Text))
                {
                    MessageBox.Show("El apellido no es válido. No se permiten espacios en blanco al inicio ni entre caracteres.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!Validaciones.TresVecesSeguidas(txtApellidoE.Text))
                {
                    MessageBox.Show("El apellido no es válido. No se permiten más de 3 veces seguidas la misma letra.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!Validaciones.ApellidoV(txtApellidoE.Text))
                {
                    MessageBox.Show("El apellido no es válido. Debe tener al menos 3 letras.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!Validaciones.NoContieneEspaciosEnBlancoEnNumero(txtTelefonoE.Text))
                {
                    MessageBox.Show("El teléfono no debe contener espacios en blanco.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!Validaciones.EsTelefonoValido(txtTelefonoE.Text))
                {
                    MessageBox.Show("El número de teléfono debe empezar con 3, 8 o 9.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!Validaciones.TelefonoValido(txtTelefonoE.Text))
                {
                    MessageBox.Show("El número de teléfono no puede contener demasiados ceros repetidos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Validación de espacios en blanco
                if (!Validaciones.CorreoSinEspacios(txtCorreoE.Text))
                {
                    MessageBox.Show("El correo electrónico no es válido. No se permiten espacios en blanco.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Validación de la estructura básica del correo
                if (!Validaciones.CorreoTresLetras(correo))
                {
                    MessageBox.Show("El correo electrónico debe tener al menos 3 letras antes del símbolo de arroba (@).", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!Validaciones.CorreoArrobas(txtCorreoE.Text))
                {
                    MessageBox.Show("El correo electrónico no puede contener más de un símbolo de arroba (@).", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!Validaciones.CorreoValidoEstructura(txtCorreoE.Text))
                {
                    MessageBox.Show("El correo electrónico no es válido. Debe contener un símbolo de arroba (@).", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!Validaciones.CorreoValidoDominio(txtCorreoE.Text))
                {
                    MessageBox.Show("El correo electrónico debe tener un dominio válido (gmail.com, yahoo.com, hotmail.com).", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (string.IsNullOrEmpty(passContraseña.Password))
                {
                    MessageBox.Show("Por favor, ingrese una contraseña.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string contrasena = passContraseña.Password;

                // Extraer solo el número de la dirección
                string[] partesDireccion = direccion.Split('-');
                string numeroDireccion = partesDireccion[0].Trim();

                // Verificar si algún campo del empleado está vacío
                if (Validaciones.CamposEmpleadosVacios(txtIDE.Text, txtNombreE.Text, txtApellidoE.Text, txtTelefonoE.Text, txtCorreoE.Text, numeroDireccion))
                {
                    MessageBox.Show("Todos los campos del empleado deben llenarse.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Verificar si el ID del empleado ya existe en la base de datos
                bool empleadoExistente = EmpleadoDAL.EmpleadoExiste(idEmpleado);

                // Si estamos en modo modificación y el empleado existe, actualizar los datos del empleado
                if (esModificacion && empleadoExistente)
                {
                    if (EmpleadoDAL.EmpleadoDI(txtCorreoE.Text, txtTelefonoE.Text, numeroDireccion, idEmpleado))
                    {
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
                    }

                    // Crear el objeto NuevoEmpleado con los datos modificados
                    NuevoEmpleado = new Empleados
                    {
                        ID_Empleado = idEmpleado,
                        Nombre_E = txtNombreE.Text,
                        Apellido_E = txtApellidoE.Text,
                        Correo_E = txtCorreoE.Text,
                        ID_Dirección = numeroDireccion,
                        Puesto = (cmbPuesto.SelectedItem as ComboBoxItem)?.Content.ToString(),
                        Estado = (cmbEstado.SelectedItem as ComboBoxItem)?.Content.ToString(),
                    };

                    // Verificar si el texto del campo de teléfono es un número válido
                    if (!decimal.TryParse(txtTelefonoE.Text, out decimal telefonoDecimal))
                    {
                        MessageBox.Show("El número de teléfono debe ser un número válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (!Validaciones.EsTelefonoValido(txtTelefonoE.Text))
                    {
                        MessageBox.Show("El teléfono debe tener 8 dígitos y comenzar con 3, 8 o 9.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Validar el formato del correo electrónico
                    if (!Validaciones.CorreoValido(txtCorreoE.Text))
                    {
                        MessageBox.Show("El formato del correo electrónico no es válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Asignar el valor convertido a decimal al Teléfono del NuevoEmpleado
                    NuevoEmpleado.Teléfono_E = telefonoDecimal;

                    // Actualizar el empleado existente en la base de datos
                    EmpleadoDAL.ActualizarEmpleado(NuevoEmpleado);
                    MessageBox.Show("Empleado modificado correctamente.");
                }
                // Si estamos en modo agregado y el empleado no existe, agregar el nuevo empleado
                else if (!esModificacion && !empleadoExistente)
                {


                    // Crear el objeto NuevoEmpleado con los datos del nuevo empleado
                    NuevoEmpleado = new Empleados
                    {
                        ID_Empleado = idEmpleado,
                        Nombre_E = txtNombreE.Text,
                        Apellido_E = txtApellidoE.Text,
                        Correo_E = txtCorreoE.Text,
                        ID_Dirección = numeroDireccion,
                        Puesto = (cmbPuesto.SelectedItem as ComboBoxItem)?.Content.ToString(),
                        Estado = "Activo", // Establecer el estado como "Activo" por defecto
                    };

                    // Verificar si el texto del campo de teléfono es un número válido
                    if (!decimal.TryParse(txtTelefonoE.Text, out decimal telefonoDecimal))
                    {
                        MessageBox.Show("El número de teléfono debe ser un número válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (!Validaciones.EsTelefonoValido(txtTelefonoE.Text))
                    {
                        MessageBox.Show("El número de teléfono debe empezar con 3, 8 o 9.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Validar el formato del correo electrónico
                    if (!Validaciones.CorreoValido(txtCorreoE.Text))
                    {
                        MessageBox.Show("El formato del correo electrónico no es válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Asignar el valor convertido a decimal al Teléfono del NuevoEmpleado
                    NuevoEmpleado.Teléfono_E = telefonoDecimal;

                    // Agregar el nuevo empleado a la base de datos
                    EmpleadoDAL.AgregarEmpleado(NuevoEmpleado, contrasena);
                    MessageBox.Show("Empleado agregado correctamente.");

                    // Llama al evento EmpleadoAgregado antes de cerrar la ventana
                    OnEmpleadoAgregado();
                }
                // Si estamos en modo agregado y el cliente ya existe, mostrar un mensaje de error
                else if (!esModificacion && empleadoExistente)
                {
                    MessageBox.Show("El empleado con este ID ya existe en la base de datos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al modificar/agregar el empleado: " + ex.Message);
            }

            // Cierra la ventana después de procesar el empleado
            this.Close();
        }

        private void txtIDE_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            // Verifica si el texto resultante después de agregar el nuevo carácter excederá la longitud máxima permitida
            if (textBox.Text.Length + e.Text.Length > 7)
            {
                // Si excede la longitud máxima permitida, marca el evento como manejado para evitar que se agregue el nuevo carácter
                e.Handled = true;

                // Muestra un mensaje informativo al usuario
                MessageBox.Show("El ID del empleado no puede tener más de 7 dígitos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Verifica si el carácter ingresado es un dígito
            if (!char.IsDigit(e.Text, 0))
            {
                // Si el carácter no es un dígito, marca el evento como manejado para evitar que se agregue
                e.Handled = true;

                // Muestra un mensaje informativo al usuario
                MessageBox.Show("Solo se permiten números en el ID del empleado.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtIDE_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            // Verifica si la tecla presionada es la barra espaciadora
            if (e.Key == Key.Space)
            {
                // Marca el evento como manejado para evitar que se agregue el espacio
                e.Handled = true;

                // Muestra un mensaje informativo al usuario
                MessageBox.Show("No se permiten espacios en blanco en el campo de usuario.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Validaciones.BloquearControles(e);
        }

        private void txtTelefonoE_PreviewTextInput(object sender, TextCompositionEventArgs e)
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

                // Muestra un mensaje informativo al usuario
                MessageBox.Show("Solo se permiten números en el número de teléfono.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show("No se permiten espacios en blanco en el campo de usuario.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Validaciones.BloquearControles(e);
        }

        private void txtNombreE_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Verificar si se ha alcanzado el límite de 50 caracteres
            if (txtNombreE.Text.Length + e.Text.Length > 50)
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
            else if (System.Text.RegularExpressions.Regex.IsMatch(e.Text, "[^a-zA-Z]"))
            {
                e.Handled = true;
                MessageBox.Show("No se permiten caracteres especiales en el campo de nombre.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtApellidoE_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Verificar si se ha alcanzado el límite de 50 caracteres
            if (txtApellidoE.Text.Length + e.Text.Length > 50)
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
            else if (System.Text.RegularExpressions.Regex.IsMatch(e.Text, "[^a-zA-Z]"))
            {
                e.Handled = true;
                MessageBox.Show("No se permiten caracteres especiales en el campo de apellido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtCorreoE_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Verificar si se ha alcanzado el límite de 50 caracteres
            if (txtCorreoE.Text.Length + e.Text.Length > 40)
            {
                e.Handled = true;
                MessageBox.Show("Se ha alcanzado el límite máximo de 50 caracteres en el campo de nombre.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void txtCorreoE_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            // Verifica si la tecla presionada es la barra espaciadora
            if (e.Key == Key.Space)
            {
                // Marca el evento como manejado para evitar que se agregue el espacio
                e.Handled = true;

                // Muestra un mensaje informativo al usuario
                MessageBox.Show("No se permiten espacios en blanco en el campo de usuario.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Validaciones.BloquearControles(e);
        }

        private void txtContraseña_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;

            // Verifica si la tecla presionada es la barra espaciadora
            if (e.Key == Key.Space)
            {
                // Marca el evento como manejado para evitar que se agregue el espacio
                e.Handled = true;

                // Muestra un mensaje informativo al usuario
                MessageBox.Show("No se permiten espacios en blanco en la contraseña.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            // Verifica si la longitud de la contraseña supera los 12 caracteres
            else if (passwordBox.Password.Length >= 12 && !char.IsControl((char)KeyInterop.VirtualKeyFromKey(e.Key)))
            {
                // Marca el evento como manejado para evitar que se agregue más caracteres
                e.Handled = true;

                // Muestra un mensaje informativo al usuario
                MessageBox.Show("La contraseña no puede tener más de 12 caracteres.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Método para llamar al evento EmpleadoAgregado
        private void OnEmpleadoAgregado()
        {
            EmpleadoAgregado?.Invoke(this, EventArgs.Empty);
        }

        private void BtnRegresar_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();

            // Crea una nueva instancia de Window6 (ventana principal de empleados) y la muestra
            Window6 frmPr = new Window6();
        }

        // Método para mostrar los detalles del empleado seleccionado en los campos de texto
        private void MostrarDetallesEmpleado()
        {
            txtIDE.Text = empleadoSeleccionado.ID_Empleado;
            txtNombreE.Text = empleadoSeleccionado.Nombre_E;
            txtApellidoE.Text = empleadoSeleccionado.Apellido_E;
            txtCorreoE.Text = empleadoSeleccionado.Correo_E;
            txtTelefonoE.Text = empleadoSeleccionado.Teléfono_E;
            cmbDireccion.Text = empleadoSeleccionado.ID_Dirección;

            // Obtener el puesto del empleado seleccionado
            string puestoEmpleado = empleadoSeleccionado.Puesto;

            string estadoEmpleado = empleadoSeleccionado.Estado;

            // Buscar el puesto en los elementos del ComboBox
            foreach (ComboBoxItem item in cmbPuesto.Items)
            {
                if (item.Content.ToString() == puestoEmpleado)
                {
                    // Establecer el elemento correspondiente como seleccionado en el ComboBox
                    cmbPuesto.SelectedItem = item;
                    break;
                }
            }

            foreach (ComboBoxItem item in cmbEstado.Items)
            {
                if (item.Content.ToString() == estadoEmpleado)
                {
                    // Establecer el elemento correspondiente como seleccionado en el ComboBox cmbEstado
                    cmbEstado.SelectedItem = item;
                    break;
                }
            }
        }

        private void TxtNombreE_LostFocus(object sender, RoutedEventArgs e)
        {
            // Formatea el texto del control de texto (txtNombreE) 
            txtNombreE.Text = Validaciones.FormatearTexto(txtNombreE.Text);
        }

        private void TxtApellidoE_LostFocus(object sender, RoutedEventArgs e)
        {
            // Formatea el texto del control de texto (txtApellidoE) 
            txtApellidoE.Text = Validaciones.FormatearTexto(txtApellidoE.Text);
        }

        private void InputControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Llama al método de Validaciones para bloquear copiar, pegar y cortar
            Validaciones.BloquearControles(e);
        }

        private void txtContraseña_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}

