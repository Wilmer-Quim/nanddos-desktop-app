using System.Runtime.InteropServices;

namespace ProyectoNANDDOS;

// Genera e inyecta una barra de titulo personalizada estilo Windows 11 en cualquier formulario.
// Incluye icono de la app, titulo del sistema, botones de control de ventana y arrastre.
public static class BarraTitulo
{
    // Win32 API para permitir arrastrar la ventana sin bordes nativos.
    [DllImport("user32.dll")]
    private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
    [DllImport("user32.dll")]
    private static extern bool ReleaseCapture();
    private const int WM_NCLBUTTONDOWN = 0xA1;
    private const int HT_CAPTION = 0x2;

    // Ruta resuelta a la carpeta de iconos en la raiz de la solucion.
    private static readonly string CarpetaIconos = Path.GetFullPath(
        Path.Combine(Application.StartupPath, @"..\..\..\..\iconos"));

    // Inyecta la barra de titulo en el formulario indicado.
    // Debe llamarse dentro del constructor o del metodo de inicializacion del formulario,
    // despues de configurar FormBorderStyle = None.
    public static void Inyectar(Form formulario)
    {
        Inyectar(formulario, formulario.Text);
    }

    // Inyecta la barra de titulo con un texto personalizado.
    public static void Inyectar(Form formulario, string titulo)
    {
        var barra = new Panel
        {
            Dock = DockStyle.Top,
            Height = 36,
            BackColor = Color.White,
            Padding = Padding.Empty
        };

        // --- Lado izquierdo: icono de la app + titulo ---
        var picIcono = new PictureBox
        {
            Image = CargarIcono("icono_nanddos.png"),
            SizeMode = PictureBoxSizeMode.Zoom,
            BackColor = Color.Transparent,
            Size = new Size(20, 20),
            Location = new Point(10, 8)
        };

        var lblTitulo = new Label
        {
            Text = titulo,
            AutoSize = true,
            ForeColor = Color.FromArgb(51, 65, 85), // #334155
            Font = new Font("Segoe UI", 9F),
            BackColor = Color.Transparent,
            Location = new Point(36, 9)
        };

        // --- Lado derecho: botones de control de ventana ---
        // Posicionamiento absoluto y anclaje a la derecha para garantizar el orden visual: [ — ] [ ◻ ] [ ✕ ]
        var btnCerrar = CrearBotonVentana("\u2715", esBotonCerrar: true);
        var btnMaximizar = CrearBotonVentana("\uE922", esBotonCerrar: false);
        var btnMinimizar = CrearBotonVentana("\uE921", esBotonCerrar: false);

        btnCerrar.Location = new Point(barra.Width - 46, 0);
        btnMaximizar.Location = new Point(barra.Width - 92, 0);
        btnMinimizar.Location = new Point(barra.Width - 138, 0);

        btnMinimizar.Click += (_, _) => formulario.WindowState = FormWindowState.Minimized;

        btnMaximizar.Click += (_, _) =>
        {
            formulario.WindowState = formulario.WindowState == FormWindowState.Maximized
                ? FormWindowState.Normal
                : FormWindowState.Maximized;
        };

        btnCerrar.Click += (_, _) => formulario.Close();

        // Habilitar arrastre en la barra, el titulo y el icono.
        HabilitarArrastre(formulario, barra);
        HabilitarArrastre(formulario, lblTitulo);
        HabilitarArrastre(formulario, picIcono);

        // Doble clic en la barra alterna maximizar/restaurar.
        barra.DoubleClick += (_, _) =>
        {
            formulario.WindowState = formulario.WindowState == FormWindowState.Maximized
                ? FormWindowState.Normal
                : FormWindowState.Maximized;
        };

        barra.Controls.Add(picIcono);
        barra.Controls.Add(lblTitulo);
        barra.Controls.Add(btnMinimizar);
        barra.Controls.Add(btnMaximizar);
        barra.Controls.Add(btnCerrar);

        formulario.Controls.Add(barra);
        barra.BringToFront();
    }

    // Crea un boton de control de ventana (minimizar, maximizar o cerrar).
    private static Label CrearBotonVentana(string simbolo, bool esBotonCerrar)
    {
        var btn = new Label
        {
            Text = simbolo,
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Size = new Size(46, 36),
            TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = Color.FromArgb(51, 65, 85), // #334155
            Font = new Font("Segoe MDL2 Assets", 10F),
            BackColor = Color.Transparent,
            Cursor = Cursors.Default
        };

        // Hover: rojo para cerrar, gris claro para los demas.
        btn.MouseEnter += (_, _) =>
        {
            if (esBotonCerrar)
            {
                btn.BackColor = Color.FromArgb(232, 17, 35); // #E81123
                btn.ForeColor = Color.White;
            }
            else
            {
                btn.BackColor = Color.FromArgb(226, 232, 240); // #E2E8F0
            }
        };

        btn.MouseLeave += (_, _) =>
        {
            btn.BackColor = Color.Transparent;
            btn.ForeColor = Color.FromArgb(51, 65, 85);
        };

        return btn;
    }

    // Permite arrastrar la ventana haciendo clic sobre un control.
    private static void HabilitarArrastre(Form formulario, Control control)
    {
        control.MouseDown += (_, e) =>
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(formulario.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        };
    }

    // Carga un icono desde la carpeta local de recursos del proyecto.
    // Si el archivo no existe, devuelve null y el PictureBox queda vacio.
    private static Image? CargarIcono(string archivo)
    {
        try
        {
            var ruta = Path.Combine(CarpetaIconos, archivo);
            return Image.FromFile(ruta);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"[NANDDOS] No se pudo cargar el icono '{archivo}': {ex.Message}");
            return null;
        }
    }
}
