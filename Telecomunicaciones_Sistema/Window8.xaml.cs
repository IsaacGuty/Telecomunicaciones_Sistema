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
                string idEmpleado = txtIDE.Text;

                // Verificar si el ID del empleado ya existe en la base de datos
                bool EmpleadoExistente = EmpleadoDAL.EmpleadoExiste(idEmpleado);

                // Si estamos en modo modificación y el empleado no existe, mostrar un mensaje de error
                if (esModificacion && !EmpleadoExistente)
                {
                    MessageBox.Show("El cliente con este ID no existe en la base de datos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!esModificacion && EmpleadoDAL.EmpleadoDI(txtNombreE.Text, txtApellidoE.Text, txtCorreoE.Text, txtTelefonoE.Text, txtDireccionE.Text))
                {
                    MessageBox.Show("Ya existe un cliente con la misma información en la base de datos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Si estamos en modo modificación y el empleado existe, actualizar los datos del empleado
                if (esModificacion && EmpleadoExistente)
                {
                    string puestoSeleccionado = cmbPuesto.SelectedItem?.ToString();
                    // Crear el objeto NuevoCliente con los datos modificados
                    NuevoEmpleado = new Empleados
                    {
                        ID_Empleado = idEmpleado,
                        Nombre_E = txtNombreE.Text,
                        Apellido_E = txtApellidoE.Text,
                        Correo_E = txtCorreoE.Text,
                        ID_Dirección = txtDireccionE.Text,
                        Puesto = (cmbPuesto.SelectedItem as ComboBoxItem)?.Content.ToString(),
                        Estado = (cmbEstado.SelectedItem as ComboBoxItem)?.Content.ToString(),
                    };

                    // Verificar si algún campo del empleado está vacío
                    if (Validaciones.CamposEmpleadosVacios(NuevoEmpleado))
                    {
                        MessageBox.Show("Todos los campos del cliente deben llenarse.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Verificar si el texto del campo de teléfono es un número válido
                    if (!decimal.TryParse(txtTelefonoE.Text, out decimal telefonoDecimal))
                    {
                        MessageBox.Show("El teléfono debe ser un número válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                else if (!esModificacion && !EmpleadoExistente)
                {
                    // Crear el objeto NuevoEmpleado con los datos del nuevo empleado
                    NuevoEmpleado = new Empleados
                    {
                        ID_Empleado = idEmpleado,
                        Nombre_E = txtNombreE.Text,
                        Apellido_E = txtApellidoE.Text,
                        Correo_E = txtCorreoE.Text,
                        ID_Dirección = txtDireccionE.Text,
                        Puesto = (cmbPuesto.SelectedItem as ComboBoxItem)?.Content.ToString(),
                        Estado = (cmbEstado.SelectedItem as ComboBoxItem)?.Content.ToString(),
                    };

                    // Verificar si algún campo del empleado está vacío
                    if (Validaciones.CamposEmpleadosVacios(NuevoEmpleado))
                    {
                        MessageBox.Show("Todos los campos del empleado deben llenarse.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Verificar si el texto del campo de teléfono es un número válido
                    if (!decimal.TryParse(txtTelefonoE.Text, out decimal telefonoDecimal))
                    {
                        MessageBox.Show("El teléfono debe ser un número válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

                    // Agregar el nuevo empleado a la base de datos
                    EmpleadoDAL.AgregarEmpleado(NuevoEmpleado);
                    MessageBox.Show("Empleado agregado correctamente.");

                    // Llama al evento EmpleadoAgregado antes de cerrar la ventana
                    OnEmpleadoAgregado();
                }
                // Si estamos en modo agregado y el cliente ya existe, mostrar un mensaje de error
                else if (!esModificacion && EmpleadoExistente)
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
            txtDireccionE.Text = empleadoSeleccionado.ID_Dirección;

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
    }
}

