namespace ProyectoNANDDOS;

// Maneja la busqueda y presentacion del logo oficial de NANDDOS.
public static class ImagenEmpresa
{
    // Extensiones de imagen permitidas dentro de la carpeta IMGNANDDOS.
    private static readonly string[] ExtensionesPermitidas = [".png", ".jpg", ".jpeg", ".bmp", ".gif"];

    // Crea un contenedor con el logo centrado y proporcional.
    public static Control CrearLogoCentrado(int ancho, int alto)
    {
        var contenedor = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent
        };

        // Si no se encuentra imagen, devuelve un contenedor vacio para evitar errores.
        var ruta = BuscarRutaImagen();
        if (ruta is null)
        {
            return contenedor;
        }

        var pictureBox = new PictureBox
        {
            Width = ancho,
            Height = alto,
            SizeMode = PictureBoxSizeMode.Zoom,
            Image = new Bitmap(ruta),
            BackColor = Color.Transparent
        };

        // Mantiene el logo centrado aunque cambie el tamano del contenedor.
        void Centrar()
        {
            pictureBox.Left = Math.Max(0, (contenedor.Width - pictureBox.Width) / 2);
            pictureBox.Top = Math.Max(0, (contenedor.Height - pictureBox.Height) / 2);
        }

        contenedor.Resize += (_, _) => Centrar();
        contenedor.Controls.Add(pictureBox);
        Centrar();
        return contenedor;
    }

    // Devuelve la ruta del logo para usos externos, como el comprobante PDF.
    public static string? ObtenerRutaImagen()
    {
        return BuscarRutaImagen();
    }

    // Busca la primera imagen valida dentro de las carpetas candidatas.
    private static string? BuscarRutaImagen()
    {
        foreach (var carpeta in ObtenerCarpetasCandidatas())
        {
            if (!Directory.Exists(carpeta))
            {
                continue;
            }

            var archivo = Directory
                .EnumerateFiles(carpeta)
                .FirstOrDefault(ruta => ExtensionesPermitidas.Contains(Path.GetExtension(ruta).ToLowerInvariant()));

            if (archivo is not null)
            {
                return archivo;
            }
        }

        return null;
    }

    // Revisa la carpeta de salida y tambien carpetas superiores del proyecto.
    private static IEnumerable<string> ObtenerCarpetasCandidatas()
    {
        var bases = new[]
        {
            AppContext.BaseDirectory,
            Directory.GetCurrentDirectory()
        };

        var visitadas = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var rutaBase in bases)
        {
            var directorio = new DirectoryInfo(rutaBase);
            while (directorio is not null)
            {
                var carpeta = Path.Combine(directorio.FullName, "IMGNANDDOS");
                if (visitadas.Add(carpeta))
                {
                    yield return carpeta;
                }

                directorio = directorio.Parent;
            }
        }
    }
}
