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
    public partial class Modificar_Empleado : Window
    {
        // Declaración de eventos para notificar cuando se modifica un empleado
        public event EventHandler EmpleadoModificado;

        // Variables de clase conexión
        private SqlConnection Conn;

        // Se declara una variable privada para almacenar la información del empleado seleccionado. 
        private Registro_Empleado.Empleados empleadoSeleccionado;

        // Se declara una variable privada de tipo bool (booleano) para indicar si se está realizando una modificación.
        private bool esModificacion;

        // Propiedad para obtener el nuevo empleado creado
        public Empleados NuevoEmpleado { get; private set; }

        // Constructor para ventana de modificación/agregación de empleado
        public Modificar_Empleado(Registro_Empleado.Empleados empleadoSeleccionado, bool esModificacion)
        {
            InitializeComponent();
            this.esModificacion = esModificacion;
            this.empleadoSeleccionado = empleadoSeleccionado;
            MostrarDetallesEmpleado();
        }

        //Maneja el evento Click del botón Aceptar.Valida la entrada del usuario y modifica un empleado a la base de datos.
        private void BtnAceptar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validaciones de campos
                if (Validaciones.CamposEmpleadosVacios(txtIDE.Text, txtNombreE.Text, txtApellidoE.Text, txtTelefonoE.Text, txtCorreoE.Text, cmbDireccion.Text))
                {
                    MessageBox.Show("Todos los campos del empleado deben llenarse.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Validación de longitud de ID de empleado
                if (txtIDE.Text.Length > 7)
                {
                    MessageBox.Show("El ID del empleado no puede tener más de 7 dígitos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Validación de longitud de teléfono
                if (txtTelefonoE.Text.Length > 8)
                {
                    MessageBox.Show("El número de teléfono no puede tener más de 8 dígitos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Obtención de valores de los campos
                string idEmpleado = txtIDE.Text;
                string nombre = txtNombreE.Text;
                string apellido = txtApellidoE.Text;
                string correo = txtCorreoE.Text;
                string telefono = txtTelefonoE.Text;

                // Obtención de la dirección seleccionada
                ComboBoxItem itemSeleccionado = (ComboBoxItem)cmbDireccion.SelectedItem;
                string direccion = itemSeleccionado?.Content?.ToString();

                // Validación de selección de dirección
                if (string.IsNullOrEmpty(direccion))
                {
                    MessageBox.Show("Por favor, seleccione una dirección.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Validación de selección de puesto
                if (cmbPuesto.SelectedItem == null)
                {
                    MessageBox.Show("Por favor, seleccione un puesto para el empleado.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Validación de formato de ID de empleado
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

                // Validación de formato de nombre y apellido
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

                // Validación de formato de teléfono
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
                    MessageBox.Show("El número de teléfono no puede tener más de cinco números repetidos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Validación de formato de correo electrónico
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

                    // Verificar si el ID del cliente ya existe en la base de datos
                int resultados = EmpleadoDAL.EmpleadoExisteConDatosMod(idEmpleado, correo, telefono);

                // Manejar el resultado según el tipo de duplicado encontrado
                switch (resultados)
                {
                    case 1:
                        MessageBox.Show("El empleado con el mismo correo ya existe en la base de datos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    case 2:
                        MessageBox.Show("El empleado con el mismo teléfono ya existe en la base de datos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    case 3:
                        MessageBox.Show("El ID del empleado ya está registrado en la base de datos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                }

                // Extraer solo el número de la dirección
                string[] partesDireccion = direccion.Split('-');
                string numeroDireccion = partesDireccion[0].Trim();

                // Extraer solo el número del estado
                string estadoSeleccionado = ((ComboBoxItem)cmbEstado.SelectedItem)?.Content.ToString();
                string[] partesEstado = estadoSeleccionado?.Split('-');
                string numeroEstado = partesEstado[0].Trim();
                int idEstado;

                if (!int.TryParse(numeroEstado, out idEstado))
                {
                    MessageBox.Show("El estado seleccionado no es válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Crear el objeto EmpleadoModificado con los datos modificados
                NuevoEmpleado = new Empleados
                {
                    ID_Empleado = idEmpleado,
                    Nombre_E = txtNombreE.Text,
                    Apellido_E = txtApellidoE.Text,
                    Correo_E = txtCorreoE.Text,
                    ID_Dirección = numeroDireccion,
                    Puesto = (cmbPuesto.SelectedItem as ComboBoxItem)?.Content.ToString(),
                    ID_Estado = numeroEstado,
                };

                // Verificar si el texto del campo de teléfono es un número válido
                if (!decimal.TryParse(txtTelefonoE.Text, out decimal telefonoDecimal))
                {
                    MessageBox.Show("El número de teléfono debe ser un número válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Validación adicional del teléfono
                if (!Validaciones.EsTelefonoValido(txtTelefonoE.Text))
                {
                    MessageBox.Show("El teléfono debe tener 8 dígitos y comenzar con 3, 8 o 9.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Asignar el valor de teléfono convertido al objeto empleadoModificado
                NuevoEmpleado.Teléfono_E = telefonoDecimal;

                // Actualizar el empleado modificado en la base de datos
                EmpleadoDAL.ActualizarEmpleado(NuevoEmpleado);

                // Mostrar mensaje de éxito
                MessageBox.Show("Empleado modificado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                // Notificar que se modificó un empleado
                OnEmpleadoModificado();

                // Cerrar la ventana
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al modificar el empleado: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // Cierra la ventana después de procesar el empleado
            this.Close();
        }

        // Método para llamar al evento EmpleadoAgregado
        private void OnEmpleadoModificado()
        {
            EmpleadoModificado?.Invoke(this, EventArgs.Empty);
        }

        private void txtIDE_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Convertir el sender al tipo TextBox para poder acceder a sus propiedades y métodos
            TextBox textBox = sender as TextBox;

            // Declarar una variable para almacenar mensajes de error que puedan surgir durante la validación
            string mensajeError;

            // Validar la longitud del texto ingresado en el TextBox.
            if (!Validaciones.ValidarLongitudTextoID(textBox, e.Text, 7, out mensajeError))
            {
                e.Handled = true;
                MessageBox.Show(mensajeError, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Validar los caracteres del texto ingresado en el TextBox.
            // La función ValidarCaracteresID recibe el texto ingresado y una variable de salida para el mensaje de error.
            if (!Validaciones.ValidarCaracteresID(e.Text, out mensajeError))
            {
                e.Handled = true;
                MessageBox.Show(mensajeError, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtIDE_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!Validaciones.ValidarTeclaEspacioIDE(e, out string mensajeError))
            {
                // Se llama al método ValidarTeclaEspacioIDE de la clase Validaciones,
                // que valida si la tecla presionada es un espacio en blanco y devuelve un mensaje de error.


                e.Handled = true;
                // Establece la propiedad Handled del evento a true para indicar que el evento ha sido manejado

                MessageBox.Show(mensajeError, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                // Muestra un cuadro de mensaje con el mensaje de error obtenido de la validación.
            }

            Validaciones.BloquearControles(e);
            // Llama al método BloquearControles de la clase Validaciones, que puede realizar acciones adicionales
        }

        private void txtTelefonoE_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            // Convierte el parámetro 'sender' a un objeto TextBox.

            if (!Validaciones.ValidarTelefonoLongCar(textBox.Text, e.Text, out string mensajeError))
            {
                // Llama al método ValidarTelefonoLongCar de la clase Validaciones para verificar si el texto ingresado es válido.

                e.Handled = true;
                // Establece la propiedad Handled del evento a true para indicar que el texto no debe ser insertado en el TextBox.

                MessageBox.Show(mensajeError, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                // Muestra un mensaje de error al usuario utilizando un cuadro de diálogo MessageBox.
            }
        }

        private void txtTelefonoE_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Llama al método ValidarTeclaEspacioTel de la clase Validaciones para validar la tecla pulsada.
            // Si la tecla no es válida, muestra un mensaje de error y marca el evento como manejado para prevenir la entrada.
            if (!Validaciones.ValidarTeclaEspacioTel(e, out string mensajeError))
            {
                // Marca el evento como manejado, evitando que el carácter sea ingresado en el control de texto.
                e.Handled = true;

                // Muestra un cuadro de mensaje con el error correspondiente.
                MessageBox.Show(mensajeError, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // Llama al método BloquearControles de la clase Validaciones para gestionar el control del estado de los controles de la interfaz.
            Validaciones.BloquearControles(e);
        }

        private void txtNombreE_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Convierte el objeto sender al tipo TextBox. Esto permite acceder a las propiedades del TextBox.
            TextBox textBox = sender as TextBox;

            // Llama a un método de validación estática para verificar si el texto ingresado cumple con ciertos requisitos.
            if (!Validaciones.ValidarNombreLongNumCar(textBox.Text, e.Text, out string mensajeError))
            {
                // Si la validación falla (es decir, el método ValidarNombreLongNumCar devuelve false),
                // se establece e.Handled a true para evitar que el texto no válido sea ingresado en el TextBox.
                e.Handled = true;

                // Muestra un mensaje de error al usuario utilizando un cuadro de mensaje de Windows.
                // El mensaje se toma del string mensajeError proporcionado por el método de validación.
                MessageBox.Show(mensajeError, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtApellidoE_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Convierte el sender a un objeto TextBox
            TextBox textBox = sender as TextBox;

            // Llama a la función de validación, pasándole el texto actual del TextBox, el texto que se está ingresando,
            // y una variable de salida para recibir cualquier mensaje de error. 
            // La función devuelve un booleano indicando si el texto es válido o no.
            if (!Validaciones.ValidarApellidoLongNumCar(textBox.Text, e.Text, out string mensajeError))
            {
                // Si la validación falla, se establece e.Handled en true para evitar que el texto se ingrese en el TextBox.
                e.Handled = true;

                // Muestra un mensaje de error al usuario con el mensaje proporcionado por la validación.
                MessageBox.Show(mensajeError, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtCorreoE_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //Se realiza un casting del objeto sender a TextBox para poder trabajar con él.
            TextBox textBox = sender as TextBox;

            //Se verifica si el correo electrónico ingresado cumple con las reglas de longitud permitidas.
            if (!Validaciones.ValidarCorreoLongitud(textBox.Text, e.Text, out string mensajeError))
            {
                //Si la validación falla (es decir, si el correo electrónico no cumple con los criterios de longitud),
                // se marca el evento como manejado para evitar que el texto sea ingresado en el TextBox.
                e.Handled = true;

                //Se muestra un mensaje de advertencia al usuario indicando que el correo electrónico no cumple con las reglas de longitud.
                MessageBox.Show(mensajeError, "Advertencia", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void txtCorreoE_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Convierte el objeto sender a un TextBox. Este es el control que disparó el evento.
            TextBox textBox = sender as TextBox;

            // Llama al método ValidarTeclaEspacioCorr de la clase Validaciones para verificar si la tecla presionada es un espacio.
            if (!Validaciones.ValidarTeclaEspacioCorr(e, out string mensajeError))
            {
                // Si la tecla presionada no es válida, se marca el evento como manejado para evitar la acción predeterminada.
                e.Handled = true;

                // Muestra un mensaje de error al usuario con el mensaje de error obtenido.
                MessageBox.Show(mensajeError, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // Llama al método BloquearControles de la clase Validaciones para bloquear otros controles, si es necesario.
            Validaciones.BloquearControles(e);
        }

        private void BtnRegresar_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();

            // Crea una nueva instancia de Registro_Empleado (ventana principal de empleados) y la muestra
            Registro_Empleado frmPr = new Registro_Empleado();
        }

        // Método para mostrar los detalles del empleado seleccionado en los campos de texto
        private void MostrarDetallesEmpleado()
        {
            txtIDE.Text = empleadoSeleccionado.ID_Empleado;
            txtNombreE.Text = empleadoSeleccionado.Nombre_E;
            txtApellidoE.Text = empleadoSeleccionado.Apellido_E;
            txtCorreoE.Text = empleadoSeleccionado.Correo_E;
            txtTelefonoE.Text = empleadoSeleccionado.Teléfono_E;

            // Obtener el valor actual del campo de dirección del empleado seleccionado
            string direccionSeleccionada = empleadoSeleccionado.ID_Dirección;

            // Iterar sobre los elementos del ComboBox para seleccionar el que coincida con la dirección del empleado seleccionado
            foreach (ComboBoxItem item in cmbDireccion.Items)
            {
                string direccion = item.Content?.ToString().Split('-')[0].Trim(); // Obtener solo el número de dirección
                if (direccion == direccionSeleccionada)
                {
                    // Establecer este elemento como seleccionado en el ComboBox
                    cmbDireccion.SelectedItem = item;
                    break; // Salir del bucle una vez que se haya encontrado la dirección correcta
                }
            }

            // Obtener el puesto del empleado seleccionado
            string puestoEmpleado = empleadoSeleccionado.Puesto;

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

            // Obtener el estado del empleado seleccionado
            string estadoEmpleado = empleadoSeleccionado.ID_Estado;

            // Buscar el estado en los elementos del ComboBox
            foreach (ComboBoxItem item in cmbEstado.Items)
            {
                string estado = item.Content?.ToString().Split('-')[0].Trim(); // Obtener solo el número de estado
                if (estado == estadoEmpleado)
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
    }
}

