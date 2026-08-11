using System.Data;
using MySql.Data.MySqlClient;

namespace ProyectoNANDDOS;

// Modulo de inventario de repuestos con estilo Fluent Design corporativo.
public class InventarioForm : Form
{
    // Controles del panel de registro.
    private readonly TextBox txtCodigo = new();
    private readonly TextBox txtNombre = new();
    private readonly TextBox txtCategoria = new();
    private readonly NumericUpDown nudStock = new();
    private readonly NumericUpDown nudPrecioCosto = new();
    private readonly NumericUpDown nudPrecioVenta = new();

    // Barra de busqueda.
    private readonly TextBox txtBusqueda = new();

    // Tabla de datos.
    private readonly DataGridView dgvInventario = new();

    // Botones de accion.
    private readonly Button btnGuardar = new();
    private readonly Button btnEditar = new();
    private readonly Button btnEliminar = new();
    private readonly Button btnBuscar = new();
    private readonly Button btnLimpiar = new();

    public InventarioForm()
    {
        InicializarComponentes();
        ConfigurarTablaInventario();
        ConfigurarBotonesInventario();
        ConfigurarPanelRegistro();
        CargarInventario();
    }

    // SECCION: construccion visual principal.
    private void InicializarComponentes()
    {
        Text = "Inventario de Repuestos";
        BackColor = Color.FromArgb(246, 248, 251);
        Font = new Font("Segoe UI", 10F);

        // Layout general: titulo, panel de registro, barra de busqueda y tabla.
        var principal = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4,
            Padding = new Padding(8)
        };
        principal.RowStyles.Add(new RowStyle(SizeType.Absolute, 46));   // Titulo
        principal.RowStyles.Add(new RowStyle(SizeType.Absolute, 200));  // Panel de registro
        principal.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));   // Barra de busqueda
        principal.RowStyles.Add(new RowStyle(SizeType.Percent, 100));   // Tabla

        // Titulo principal.
        principal.Controls.Add(new Label
        {
            Text = "Inventario de Repuestos",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 18F, FontStyle.Bold),
            ForeColor = Color.FromArgb(15, 23, 42) // #0F172A
        }, 0, 0);

        // Panel de registro (GroupBox).
        principal.Controls.Add(CrearPanelRegistro(), 0, 1);

        // Barra de busqueda con botones.
        principal.Controls.Add(CrearBarraBusqueda(), 0, 2);

        // Tabla de inventario.
        dgvInventario.Dock = DockStyle.Fill;
        principal.Controls.Add(dgvInventario, 0, 3);

        Controls.Add(principal);
    }

    // Crea el GroupBox con los campos de entrada para registrar/editar repuestos.
    private GroupBox CrearPanelRegistro()
    {
        var grupo = new GroupBox
        {
            Text = "Datos del Repuesto",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            ForeColor = Color.FromArgb(15, 23, 42),
            Padding = new Padding(12, 8, 12, 8)
        };

        // Layout interno: 3 filas x 6 columnas (label + control por par).
        var tabla = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 6,
            RowCount = 3,
            Padding = new Padding(0)
        };
        // Columnas: Label | Control | Label | Control | Label | Control
        tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
        tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
        tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
        tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
        tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
        tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34));
        tabla.RowStyles.Add(new RowStyle(SizeType.Percent, 33));
        tabla.RowStyles.Add(new RowStyle(SizeType.Percent, 33));
        tabla.RowStyles.Add(new RowStyle(SizeType.Percent, 34));

        var fuenteLabel = new Font("Segoe UI", 9F, FontStyle.Regular);
        var fuenteControl = new Font("Segoe UI", 9F, FontStyle.Regular);
        var colorLabel = Color.FromArgb(51, 65, 85); // #334155

        // Helper para crear labels alineados.
        Label CrearLabel(string texto)
        {
            return new Label
            {
                Text = texto,
                Font = fuenteLabel,
                ForeColor = colorLabel,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleRight,
                Padding = new Padding(0, 0, 6, 0)
            };
        }

        // Fila 1: Codigo | Nombre | Categoria
        tabla.Controls.Add(CrearLabel("Código:"), 0, 0);
        txtCodigo.Dock = DockStyle.Fill;
        txtCodigo.Font = fuenteControl;
        txtCodigo.BorderStyle = BorderStyle.FixedSingle;
        tabla.Controls.Add(txtCodigo, 1, 0);

        tabla.Controls.Add(CrearLabel("Nombre:"), 2, 0);
        txtNombre.Dock = DockStyle.Fill;
        txtNombre.Font = fuenteControl;
        txtNombre.BorderStyle = BorderStyle.FixedSingle;
        tabla.Controls.Add(txtNombre, 3, 0);

        tabla.Controls.Add(CrearLabel("Categoría:"), 4, 0);
        txtCategoria.Dock = DockStyle.Fill;
        txtCategoria.Font = fuenteControl;
        txtCategoria.BorderStyle = BorderStyle.FixedSingle;
        tabla.Controls.Add(txtCategoria, 5, 0);

        // Fila 2: Stock | Precio Costo | Precio Venta
        tabla.Controls.Add(CrearLabel("Stock:"), 0, 1);
        nudStock.Dock = DockStyle.Fill;
        nudStock.Font = fuenteControl;
        nudStock.Minimum = 0;
        nudStock.Maximum = 99999;
        nudStock.DecimalPlaces = 0;
        tabla.Controls.Add(nudStock, 1, 1);

        tabla.Controls.Add(CrearLabel("Precio Costo:"), 2, 1);
        nudPrecioCosto.Dock = DockStyle.Fill;
        nudPrecioCosto.Font = fuenteControl;
        nudPrecioCosto.Minimum = 0;
        nudPrecioCosto.Maximum = 999999.99M;
        nudPrecioCosto.DecimalPlaces = 2;
        nudPrecioCosto.ThousandsSeparator = true;
        tabla.Controls.Add(nudPrecioCosto, 3, 1);

        tabla.Controls.Add(CrearLabel("Precio Venta:"), 4, 1);
        nudPrecioVenta.Dock = DockStyle.Fill;
        nudPrecioVenta.Font = fuenteControl;
        nudPrecioVenta.Minimum = 0;
        nudPrecioVenta.Maximum = 999999.99M;
        nudPrecioVenta.DecimalPlaces = 2;
        nudPrecioVenta.ThousandsSeparator = true;
        tabla.Controls.Add(nudPrecioVenta, 5, 1);

        // Fila 3: Botones de accion (Guardar, Editar, Eliminar, Limpiar).
        var panelBotones = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 4,
            RowCount = 1
        };
        panelBotones.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        panelBotones.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        panelBotones.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        panelBotones.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));

        btnGuardar.Text = "Guardar";
        btnGuardar.Dock = DockStyle.Fill;
        btnGuardar.Margin = new Padding(4);
        btnGuardar.Click += (_, _) => GuardarRepuesto();

        btnEditar.Text = "Editar";
        btnEditar.Dock = DockStyle.Fill;
        btnEditar.Margin = new Padding(4);
        btnEditar.Click += (_, _) => EditarRepuesto();

        btnEliminar.Text = "Eliminar";
        btnEliminar.Dock = DockStyle.Fill;
        btnEliminar.Margin = new Padding(4);
        btnEliminar.Click += (_, _) => EliminarRepuesto();

        btnLimpiar.Text = "Limpiar";
        btnLimpiar.Dock = DockStyle.Fill;
        btnLimpiar.Margin = new Padding(4);
        btnLimpiar.Click += (_, _) => LimpiarCampos();

        panelBotones.Controls.Add(btnGuardar, 0, 0);
        panelBotones.Controls.Add(btnEditar, 1, 0);
        panelBotones.Controls.Add(btnEliminar, 2, 0);
        panelBotones.Controls.Add(btnLimpiar, 3, 0);

        // Ocupa las 6 columnas de la fila 3.
        tabla.SetColumnSpan(panelBotones, 6);
        tabla.Controls.Add(panelBotones, 0, 2);

        grupo.Controls.Add(tabla);
        return grupo;
    }

    // Crea la barra de busqueda rapida.
    private TableLayoutPanel CrearBarraBusqueda()
    {
        var barra = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2
        };
        barra.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        barra.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));

        txtBusqueda.Dock = DockStyle.Fill;
        txtBusqueda.Font = new Font("Segoe UI", 9F);
        txtBusqueda.BorderStyle = BorderStyle.FixedSingle;
        txtBusqueda.PlaceholderText = "Buscar por código o nombre...";
        txtBusqueda.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Enter)
                CargarInventario();
        };

        btnBuscar.Text = "Buscar";
        btnBuscar.Dock = DockStyle.Fill;
        btnBuscar.Click += (_, _) => CargarInventario();

        barra.Controls.Add(txtBusqueda, 0, 0);
        barra.Controls.Add(btnBuscar, 1, 0);

        return barra;
    }

    // Aplica el estilo visual de los campos del panel de registro.
    private void ConfigurarPanelRegistro()
    {
        // Los TextBox y NumericUpDown ya se estilizan en CrearPanelRegistro.
        // Aqui aplicamos ajustes adicionales si es necesario.
    }

    // Aplica el estilo Fluent Design y los iconos locales a los botones.
    private void ConfigurarBotonesInventario()
    {
        var carpetaIconos = Path.GetFullPath(Path.Combine(Application.StartupPath, @"..\..\..\..\iconos"));

        // Helper local para aplicar estilo a cada boton.
        void AplicarEstilo(Button btn, string archivoIcono, Color fondo, Color texto, Color hover)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Cursor = Cursors.Hand;
            btn.Font = new Font("Segoe UI", 9F);
            btn.BackColor = fondo;
            btn.ForeColor = texto;
            btn.FlatAppearance.MouseOverBackColor = hover;
            btn.ImageAlign = ContentAlignment.MiddleLeft;
            btn.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn.Padding = new Padding(8, 0, 0, 0);

            try
            {
                var ruta = Path.Combine(carpetaIconos, archivoIcono);
                if (File.Exists(ruta))
                    btn.Image = Image.FromFile(ruta);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NANDDOS] No se pudo cargar el icono '{archivoIcono}': {ex.Message}");
            }
        }

        var azulAcento = Color.FromArgb(37, 99, 235);       // #2563EB
        var azulAcentoHover = Color.FromArgb(29, 78, 216);
        var grisPizarra = Color.FromArgb(71, 85, 105);       // #475569
        var grisPizarraHover = Color.FromArgb(51, 65, 85);
        var rojoSuave = Color.FromArgb(239, 68, 68);         // #EF4444
        var rojoSuaveHover = Color.FromArgb(220, 38, 38);
        var grisClaro = Color.FromArgb(100, 116, 139);       // #64748B
        var grisClaroHover = Color.FromArgb(71, 85, 105);

        AplicarEstilo(btnGuardar, "btn_guardar.png", azulAcento, Color.White, azulAcentoHover);
        AplicarEstilo(btnEditar, "btn_editar.png", grisPizarra, Color.White, grisPizarraHover);
        AplicarEstilo(btnEliminar, "btn_eliminar.png", rojoSuave, Color.White, rojoSuaveHover);
        AplicarEstilo(btnBuscar, "btn_buscar.png", azulAcento, Color.White, azulAcentoHover);
        AplicarEstilo(btnLimpiar, "btn_limpiar.png", grisClaro, Color.White, grisClaroHover);
    }

    // Configura el DataGridView con estilo Fluent Design corporativo.
    private void ConfigurarTablaInventario()
    {
        dgvInventario.AllowUserToAddRows = false;
        dgvInventario.AllowUserToDeleteRows = false;
        dgvInventario.AllowUserToOrderColumns = false;
        dgvInventario.AllowUserToResizeColumns = false;
        dgvInventario.AllowUserToResizeRows = false;
        dgvInventario.ReadOnly = true;
        dgvInventario.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvInventario.MultiSelect = false;
        dgvInventario.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvInventario.RowHeadersVisible = false;

        // Estilo visual general.
        dgvInventario.BackgroundColor = Color.White;
        dgvInventario.BorderStyle = BorderStyle.None;
        dgvInventario.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        dgvInventario.GridColor = Color.FromArgb(226, 232, 240); // #E2E8F0

        // Estilo de encabezados.
        dgvInventario.EnableHeadersVisualStyles = false;
        dgvInventario.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
        dgvInventario.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        dgvInventario.ColumnHeadersHeight = 40;
        dgvInventario.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(15, 23, 42);   // #0F172A
        dgvInventario.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        dgvInventario.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        dgvInventario.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(15, 23, 42);

        // Estilo de filas y colores alternos.
        dgvInventario.RowTemplate.Height = 35;
        dgvInventario.DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
        dgvInventario.DefaultCellStyle.BackColor = Color.White;
        dgvInventario.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252); // #F8FAFC

        // Color de seleccion (azul suave con texto oscuro).
        dgvInventario.DefaultCellStyle.SelectionBackColor = Color.FromArgb(224, 242, 254);   // #E0F2FE
        dgvInventario.DefaultCellStyle.SelectionForeColor = Color.FromArgb(15, 23, 42);       // #0F172A

        // Al hacer clic en una fila, llena los campos de edicion.
        dgvInventario.CellClick += (_, e) =>
        {
            if (e.RowIndex < 0) return;
            CargarFilaEnCampos(e.RowIndex);
        };
    }

    // SECCION: logica de datos.

    // Carga todos los repuestos o filtra por texto de busqueda.
    private void CargarInventario()
    {
        try
        {
            var texto = txtBusqueda.Text.Trim();
            var tabla = string.IsNullOrEmpty(texto)
                ? RepuestoDAO.ObtenerTodos()
                : RepuestoDAO.Buscar(texto);

            dgvInventario.DataSource = tabla;

            // Renombrar encabezados a español.
            if (dgvInventario.Columns.Count > 0)
            {
                RenombrarColumnas();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al cargar el inventario.\n\n{ex.Message}",
                "Inventario", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // Renombra las columnas del DataGridView a textos legibles en español.
    private void RenombrarColumnas()
    {
        var nombres = new Dictionary<string, string>
        {
            { "id_repuesto", "ID" },
            { "codigo", "Código" },
            { "nombre", "Nombre" },
            { "categoria", "Categoría" },
            { "stock", "Stock" },
            { "precio_costo", "Precio Costo" },
            { "precio_venta", "Precio Venta" },
            { "fecha_ingreso", "Fecha de Ingreso" }
        };

        foreach (DataGridViewColumn col in dgvInventario.Columns)
        {
            if (nombres.TryGetValue(col.Name, out var nombre))
                col.HeaderText = nombre;
        }

        // Ocultar la columna ID al usuario.
        if (dgvInventario.Columns.Contains("id_repuesto"))
            dgvInventario.Columns["id_repuesto"].Visible = false;
    }

    // Llena los campos de edicion con los datos de la fila seleccionada.
    private void CargarFilaEnCampos(int indice)
    {
        var fila = dgvInventario.Rows[indice];

        txtCodigo.Text = fila.Cells["codigo"]?.Value?.ToString() ?? "";
        txtNombre.Text = fila.Cells["nombre"]?.Value?.ToString() ?? "";
        txtCategoria.Text = fila.Cells["categoria"]?.Value?.ToString() ?? "";

        if (int.TryParse(fila.Cells["stock"]?.Value?.ToString(), out int stock))
            nudStock.Value = stock;

        if (decimal.TryParse(fila.Cells["precio_costo"]?.Value?.ToString(), out decimal costo))
            nudPrecioCosto.Value = costo;

        if (decimal.TryParse(fila.Cells["precio_venta"]?.Value?.ToString(), out decimal venta))
            nudPrecioVenta.Value = venta;

        // Guardar el ID interno como Tag del formulario para uso en Editar/Eliminar.
        Tag = fila.Cells["id_repuesto"]?.Value;
    }

    // Guarda un nuevo repuesto en la base de datos.
    private void GuardarRepuesto()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(txtCodigo.Text) || string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El código y el nombre del repuesto son obligatorios.",
                    "Inventario", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var repuesto = new Repuesto
            {
                Codigo = txtCodigo.Text.Trim(),
                Nombre = txtNombre.Text.Trim(),
                Categoria = txtCategoria.Text.Trim(),
                Stock = (int)nudStock.Value,
                PrecioCosto = nudPrecioCosto.Value,
                PrecioVenta = nudPrecioVenta.Value,
                FechaIngreso = DateTime.Now
            };

            RepuestoDAO.Insertar(repuesto);
            MessageBox.Show("Repuesto registrado correctamente.",
                "Inventario", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LimpiarCampos();
            CargarInventario();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al guardar el repuesto.\n\n{ex.Message}",
                "Inventario", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // Actualiza el repuesto seleccionado con los datos de los campos.
    private void EditarRepuesto()
    {
        try
        {
            if (Tag is not int idRepuesto)
            {
                MessageBox.Show("Seleccione un repuesto de la tabla para editarlo.",
                    "Inventario", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var repuesto = new Repuesto
            {
                IdRepuesto = idRepuesto,
                Codigo = txtCodigo.Text.Trim(),
                Nombre = txtNombre.Text.Trim(),
                Categoria = txtCategoria.Text.Trim(),
                Stock = (int)nudStock.Value,
                PrecioCosto = nudPrecioCosto.Value,
                PrecioVenta = nudPrecioVenta.Value,
                FechaIngreso = DateTime.Now
            };

            RepuestoDAO.Actualizar(repuesto);
            MessageBox.Show("Repuesto actualizado correctamente.",
                "Inventario", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LimpiarCampos();
            CargarInventario();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al actualizar el repuesto.\n\n{ex.Message}",
                "Inventario", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // Elimina el repuesto seleccionado previa confirmacion.
    private void EliminarRepuesto()
    {
        try
        {
            if (Tag is not int idRepuesto)
            {
                MessageBox.Show("Seleccione un repuesto de la tabla para eliminarlo.",
                    "Inventario", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirmacion = MessageBox.Show(
                "¿Está seguro de que desea eliminar este repuesto?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmacion != DialogResult.Yes) return;

            RepuestoDAO.Eliminar(idRepuesto);
            MessageBox.Show("Repuesto eliminado correctamente.",
                "Inventario", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LimpiarCampos();
            CargarInventario();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al eliminar el repuesto.\n\n{ex.Message}",
                "Inventario", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // Reinicia todos los campos del formulario.
    private void LimpiarCampos()
    {
        txtCodigo.Clear();
        txtNombre.Clear();
        txtCategoria.Clear();
        nudStock.Value = 0;
        nudPrecioCosto.Value = 0;
        nudPrecioVenta.Value = 0;
        Tag = null;
        dgvInventario.ClearSelection();
    }
}
