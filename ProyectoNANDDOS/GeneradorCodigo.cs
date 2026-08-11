using MySql.Data.MySqlClient;

namespace ProyectoNANDDOS;

// Genera identificadores visibles como CLI-0001, LP-0001 o ENT-0001.
public static class GeneradorCodigo
{
    // Genera el siguiente codigo para un cliente.
    public static string GenerarCodigoCliente(MySqlConnection? conexionExistente = null, MySqlTransaction? transaccion = null)
    {
        return Generar("clientes", "codigo", "CLI", conexionExistente, transaccion);
    }

    // Genera el siguiente codigo de equipo usando el prefijo de nomenclatura.
    public static string GenerarCodigoEquipo(string prefijo, MySqlConnection? conexionExistente = null, MySqlTransaction? transaccion = null)
    {
        return Generar("equipos", "codigo", prefijo, conexionExistente, transaccion);
    }

    // Genera el siguiente codigo para una entrega.
    public static string GenerarCodigoEntrega(MySqlConnection? conexionExistente = null, MySqlTransaction? transaccion = null)
    {
        return Generar("entregas", "codigo", "ENT", conexionExistente, transaccion);
    }

    // Busca el mayor consecutivo existente y devuelve el siguiente valor.
    private static string Generar(string tabla, string columna, string prefijo, MySqlConnection? conexionExistente, MySqlTransaction? transaccion)
    {
        if (!EsPrefijoValido(prefijo))
        {
            throw new ArgumentException("El prefijo del codigo no es valido.", nameof(prefijo));
        }

        // Reutiliza la conexion/transaccion cuando el guardado ya esta dentro de una operacion.
        var abrirConexion = conexionExistente is null;
        using var conexionNueva = abrirConexion ? ConexionDB.ObtenerConexion() : null;
        var conexion = conexionExistente ?? conexionNueva!;

        var sql = $"""
            SELECT IFNULL(MAX(CAST(SUBSTRING({columna}, @inicio) AS UNSIGNED)), 0)
            FROM {tabla}
            WHERE {columna} LIKE @patron;
            """;

        using var comando = new MySqlCommand(sql, conexion);
        comando.Transaction = transaccion;
        comando.Parameters.AddWithValue("@inicio", prefijo.Length + 2);
        comando.Parameters.AddWithValue("@patron", $"{prefijo}-%");

        var ultimo = Convert.ToInt32(comando.ExecuteScalar());
        return $"{prefijo}-{ultimo + 1:0000}";
    }

    // Evita prefijos inesperados antes de construir la consulta SQL.
    private static bool EsPrefijoValido(string prefijo)
    {
        return prefijo.Length is >= 2 and <= 5 && prefijo.All(char.IsLetterOrDigit);
    }
}
