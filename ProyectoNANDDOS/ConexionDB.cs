using MySql.Data.MySqlClient;

namespace ProyectoNANDDOS;

// Centraliza la conexion a MySQL para que todo el sistema use la misma configuracion.
public static class ConexionDB
{
    // Nombre fijo de la base de datos existente en MySQL Workbench.
    public const string NombreBaseDatos = "proyecto_nanddos_db";

    // Si cambia la contraseña de root, modifica este valor.
    // SECCION: datos de conexion.
    private const string Servidor = "localhost";
    private const uint Puerto = 3306;
    private const string Usuario = "root";
    private const string Password = "W@qh1";

    // Construye la cadena de conexion usada por MySql.Data.MySqlClient.
    public static string CadenaConexion
    {
        get
        {
            var builder = new MySqlConnectionStringBuilder
            {
                Server = Servidor,
                Port = Puerto,
                Database = NombreBaseDatos,
                UserID = Usuario,
                Password = Password,
                SslMode = MySqlSslMode.Disabled,
                AllowPublicKeyRetrieval = true,
                CharacterSet = "utf8mb4"
            };

            return builder.ConnectionString;
        }
    }

    // Abre y devuelve una conexion lista para ejecutar comandos SQL.
    public static MySqlConnection ObtenerConexion()
    {
        var conexion = new MySqlConnection(CadenaConexion);
        conexion.Open();
        return conexion;
    }

    // Prueba rapida usada al iniciar la aplicacion.
    public static void ProbarConexion()
    {
        using var conexion = ObtenerConexion();
        using var comando = new MySqlCommand("SELECT 1;", conexion);
        comando.ExecuteScalar();
    }
}
