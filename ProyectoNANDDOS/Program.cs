namespace ProyectoNANDDOS;

// Punto de entrada principal de la aplicacion.
static class Program
{
    [STAThread]
    static void Main()
    {
        // Inicializa la configuracion visual requerida por Windows Forms.
        ApplicationConfiguration.Initialize();

        try
        {
            // Antes de abrir el Login, confirma que la base existente responde.
            ConexionDB.ProbarConexion();
            Application.Run(new LoginForm());
        }
        catch (Exception ex)
        {
            // Si MySQL o la base no estan listos, se muestra un mensaje claro.
            MessageBox.Show(
                "No se pudo conectar con la base de datos existente.\n\n" +
                "La aplicación no crea la base de datos automáticamente. Debes crear manualmente la base " +
                "proyecto_nanddos_db en MySQL Workbench y ejecutar el script SQL indicado en INSTRUCCIONES_BASE_DATOS.txt.\n\n" +
                "También verifica que MySQL Server esté activo y que la contraseña en ConexionDB.cs sea correcta.\n\n" +
                $"Detalle: {ex.Message}",
                "Proyecto NANDDOS",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }    
}
