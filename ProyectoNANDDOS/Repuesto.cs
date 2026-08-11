namespace ProyectoNANDDOS;

// Modelo que representa un registro de la tabla 'repuestos'.
public class Repuesto
{
    public int IdRepuesto { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public int Stock { get; set; }
    public decimal PrecioCosto { get; set; }
    public decimal PrecioVenta { get; set; }
    public DateTime FechaIngreso { get; set; } = DateTime.Now;
}
