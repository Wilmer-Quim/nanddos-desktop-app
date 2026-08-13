using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ProyectoNANDDOS;

public class MensajeNanddosForm : Form
{
    private Label lblTitulo;
    private Label lblMensaje;
    private PictureBox picIcono;
    private Button btnAceptar;

    public MensajeNanddosForm()
    {
        InitializeComponent();
        CargarIconoCorporativo();
    }

    private void InitializeComponent()
    {
        this.lblTitulo = new Label();
        this.lblMensaje = new Label();
        this.picIcono = new PictureBox();
        this.btnAceptar = new Button();
        ((System.ComponentModel.ISupportInitialize)(this.picIcono)).BeginInit();
        this.SuspendLayout();
        
        // 
        // lblTitulo
        // 
        this.lblTitulo.AutoSize = true;
        this.lblTitulo.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        this.lblTitulo.ForeColor = Color.White;
        this.lblTitulo.Location = new Point(90, 20);
        this.lblTitulo.Name = "lblTitulo";
        this.lblTitulo.Size = new Size(54, 19);
        this.lblTitulo.TabIndex = 0;
        this.lblTitulo.Text = "Título";
        
        // 
        // lblMensaje
        // 
        this.lblMensaje.Font = new Font("Segoe UI", 9F);
        this.lblMensaje.ForeColor = Color.White;
        this.lblMensaje.Location = new Point(90, 50);
        this.lblMensaje.Name = "lblMensaje";
        this.lblMensaje.Size = new Size(250, 70);
        this.lblMensaje.TabIndex = 1;
        this.lblMensaje.Text = "Mensaje";
        
        // 
        // picIcono
        // 
        this.picIcono.BackColor = Color.Transparent;
        this.picIcono.Location = new Point(20, 20);
        this.picIcono.Name = "picIcono";
        this.picIcono.Size = new Size(60, 60);
        this.picIcono.SizeMode = PictureBoxSizeMode.Zoom;
        this.picIcono.TabIndex = 2;
        this.picIcono.TabStop = false;
        
        // 
        // btnAceptar
        // 
        this.btnAceptar.BackColor = Color.FromArgb(37, 99, 235); // #2563EB
        this.btnAceptar.FlatAppearance.BorderSize = 0;
        this.btnAceptar.FlatStyle = FlatStyle.Flat;
        this.btnAceptar.ForeColor = Color.White;
        this.btnAceptar.Location = new Point(140, 130);
        this.btnAceptar.Name = "btnAceptar";
        this.btnAceptar.Size = new Size(80, 30);
        this.btnAceptar.TabIndex = 3;
        this.btnAceptar.Text = "Aceptar";
        this.btnAceptar.UseVisualStyleBackColor = false;
        this.btnAceptar.Click += new EventHandler(this.btnAceptar_Click);
        
        // 
        // MensajeNanddosForm
        // 
        this.BackColor = Color.FromArgb(15, 23, 42); // Azul corporativo oscuro
        this.ClientSize = new Size(360, 180);
        this.Controls.Add(this.btnAceptar);
        this.Controls.Add(this.picIcono);
        this.Controls.Add(this.lblMensaje);
        this.Controls.Add(this.lblTitulo);
        this.FormBorderStyle = FormBorderStyle.None;
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Name = "MensajeNanddosForm";
        this.Text = "MensajeNanddosForm";
        ((System.ComponentModel.ISupportInitialize)(this.picIcono)).EndInit();
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    private void CargarIconoCorporativo()
    {
        try
        {
            var carpetaIconos = Path.GetFullPath(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\iconos"));

            string[] extensiones = [".png", ".jpg", ".jpeg", ".bmp"];
            
            foreach (var ext in extensiones)
            {
                var ruta = Path.Combine(carpetaIconos, "icono_nanddos" + ext);
                if (File.Exists(ruta))
                {
                    picIcono.Image = Image.FromFile(ruta);
                    break;
                }
            }
        }
        catch
        {
            // Silenciar error si no se encuentra la imagen.
        }
    }

    private void btnAceptar_Click(object? sender, EventArgs e)
    {
        this.DialogResult = DialogResult.OK;
        this.Close();
    }

    public static DialogResult Mostrar(string mensaje, string titulo)
    {
        using (MensajeNanddosForm form = new MensajeNanddosForm())
        {
            form.lblMensaje.Text = mensaje;
            form.lblTitulo.Text = titulo;
            return form.ShowDialog();
        }
    }
}
