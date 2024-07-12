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
    /// Lógica de interacción para Agregar_Empleado.xaml
    /// </summary>
    public partial class Agregar_Empleado : Window
    {
        // Declaración de eventos para notificar cuando se agrega o modifica un empleado
        public event EventHandler EmpleadoAgregado;

        // Lista para almacenar empleados
        private List<Empleados> empleados;

        // Propiedad para obtener el nuevo empleado creado
        public Empleados NuevoEmpleado { get; private set; }

        // Variable para indicar si se está modificando un empleado
        private bool esModificacion;

        // Constructor para ventana de modificación/agregación de empleado
        public Agregar_Empleado()
        {
            InitializeComponent();
            cmbEstado.Items.Clear(); // Limpiar cualquier elemento existente
            cmbEstado.Items.Add("Activo"); // Agregar solo la opción "Activo"
            cmbEstado.SelectedIndex = 0; // Establecer "Activo" como seleccionado
        }

        // Variables de clase
        private SqlConnection Conn;

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
                    MessageBox.Show("El número de teléfono no puede más de cinco números repetidos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!Validaciones.CorreoSinEspacios(txtCorreoE.Text))
                {
                    MessageBox.Show("El correo electrónico no es válido. No se permiten espacios en blanco.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!Validaciones.CorreoArrobas(txtCorreoE.Text))
                {
                    MessageBox.Show("El correo electrónico no puede contener más de un símbolo de arroba (@).", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!Validaciones.CorreoValidoEstructura(txtCorreoE.Text))
                {
                    MessageBox.Show("El correo electrónico no es válido. Debe tener un formato válido (nombre@dominio).", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!Validaciones.CorreoTresLetras(txtCorreoE.Text))
                {
                    MessageBox.Show("El correo electrónico debe tener al menos 3 letras antes del símbolo de arroba (@).", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                if (passContraseña.Password.Length < 8)
                {
                    MessageBox.Show("La contraseña debe tener al menos 8 caracteres.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string contrasena = passContraseña.Password;

                // Extraer solo el número de la dirección
                string[] partesDireccion = direccion.Split('-');
                string numeroDireccion = partesDireccion[0].Trim();

                if (Validaciones.CamposEmpleadosVacios(txtIDE.Text, txtNombreE.Text, txtApellidoE.Text, txtTelefonoE.Text, txtCorreoE.Text, numeroDireccion))
                {
                    MessageBox.Show("Todos los campos del empleado deben llenarse.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

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

                // Asignar el valor convertido a decimal al Teléfono del NuevoEmpleado
                NuevoEmpleado.Teléfono_E = telefonoDecimal;

                // Agregar el nuevo empleado a la base de datos
                EmpleadoDAL.AgregarEmpleado(NuevoEmpleado, contrasena);
                MessageBox.Show("Empleado agregado correctamente.");

                // Llama al evento EmpleadoAgregado antes de cerrar la ventana
                OnEmpleadoAgregado();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar el empleado: " + ex.Message);
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

                if (char.IsLetter(e.Text, 0))
                {
                    // Muestra un mensaje informativo al usuario sobre letras
                    MessageBox.Show("No se permiten letras en el ID del empleado.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    // Muestra un mensaje informativo al usuario sobre caracteres especiales
                    MessageBox.Show("No se permiten caracteres especiales en el ID del empleado.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void txtIDE_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!Validaciones.ValidarTeclaEspacioIDE(e, out string mensajeError))
            {
                e.Handled = true;
                MessageBox.Show(mensajeError, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Validaciones.BloquearControles(e);
        }

        private void txtTelefonoE_PreviewTextInput(object sender, TextCompositionEventArgs e)
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

        private void txtNombreE_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (!Validaciones.ValidarNombreLongNumCar(textBox.Text, e.Text, out string mensajeError))
            {
                e.Handled = true;
                MessageBox.Show(mensajeError, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtApellidoE_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (!Validaciones.ValidarApellidoLongNumCar(textBox.Text, e.Text, out string mensajeError))
            {
                e.Handled = true;
                MessageBox.Show(mensajeError, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtCorreoE_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (!Validaciones.ValidarCorreoLongitud(textBox.Text, e.Text, out string mensajeError))
            {
                e.Handled = true;
                MessageBox.Show(mensajeError, "Advertencia", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void txtCorreoE_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (!Validaciones.ValidarTeclaEspacioCorr(e, out string mensajeError))
            {
                e.Handled = true;
                MessageBox.Show(mensajeError, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Validaciones.BloquearControles(e);
        }

        private void txtContraseña_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;

            if (!Validaciones.ValidarContraseñaEspLong(passwordBox.Password, e, out string mensajeError))
            {
                e.Handled = true;
                MessageBox.Show(mensajeError, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Validaciones.BloquearControles(e);
        }

        // Método para llamar al evento EmpleadoAgregado
        private void OnEmpleadoAgregado()
        {
            EmpleadoAgregado?.Invoke(this, EventArgs.Empty);
        }

        private void BtnRegresar_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();

            // Crea una nueva instancia de Registro_Empleado (ventana principal de empleados) y la muestra
            Registro_Empleado frmPr = new Registro_Empleado();
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

        public static implicit operator Agregar_Empleado(Modificar_Empleado v)
        {
            throw new NotImplementedException();
        }
    }
}

