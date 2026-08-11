using System;
using System.IO;
using System.Windows.Forms;
using ZXing;
using ZXing.Windows.Compatibility;
using PdfDocument = iTextSharp.text.Document;
using PdfParagraph = iTextSharp.text.Paragraph;
using PdfImage = iTextSharp.text.Image;
using PdfWriter = iTextSharp.text.pdf.PdfWriter;
using PdfFont = iTextSharp.text.Font;
using PdfElement = iTextSharp.text.Element;
using PdfBaseColor = iTextSharp.text.BaseColor;
using PdfPageSize = iTextSharp.text.PageSize;

namespace ProyectoNANDDOS;

// Genera comprobantes PDF con dos paginas separadas: una para el cliente y otra para el tecnico.
public class GeneradorTickets
{
    // Resolucion de ruta dinamica hacia la carpeta TIKETS en la raiz de la solucion.
    private static readonly string CarpetaTickets = Path.GetFullPath(
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\TIKETS"));

    // Helper para crear fuentes PDF sin colision con System.Drawing.Font.
    private static PdfFont Fuente(float tamano, int estilo, PdfBaseColor? color = null)
    {
        return iTextSharp.text.FontFactory.GetFont(
            iTextSharp.text.FontFactory.HELVETICA, tamano, estilo,
            color ?? new PdfBaseColor(0, 0, 0));
    }

    // Genera un PDF de 2 paginas (cliente y tecnico) y lo guarda en la carpeta TIKETS.
    // Devuelve la ruta completa del archivo generado, o null si hubo un error.
    public string? GenerarTicketPDF(string codigoEquipo, string cliente, string telefono, string problema, string equipoInfo)
    {
        try
        {
            // Asegura que la carpeta TIKETS exista en la raiz de la solucion.
            if (!Directory.Exists(CarpetaTickets))
            {
                Directory.CreateDirectory(CarpetaTickets);
            }

            // Nomenclatura del archivo: solo el codigo del equipo.
            string nombreArchivo = $"{codigoEquipo}.pdf";
            string rutaPDF = Path.Combine(CarpetaTickets, nombreArchivo);

            // Custom Page Size: Ancho 58mm (~164 points), Alto 400 points.
            var tamañoTermico = new iTextSharp.text.Rectangle(164f, 400f);
            
            using var documento = new PdfDocument(tamañoTermico);
            using var escritor = PdfWriter.GetInstance(documento, new FileStream(rutaPDF, FileMode.Create));
            
            // Establecer margenes muy pequeños para aprovechar el papel termico
            documento.SetMargins(10f, 10f, 10f, 10f);
            
            documento.Open();

            // ===== PAGINA 1: Copia Cliente =====
            CrearPaginaCliente(documento, codigoEquipo, cliente, telefono);

            // ===== PAGINA 2: Copia Tecnico =====
            documento.NewPage();
            CrearPaginaTecnico(documento, codigoEquipo, cliente, telefono, equipoInfo, problema);

            documento.Close();
            
            // Previsualizacion Automatica
            try 
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo 
                { 
                    FileName = rutaPDF, 
                    UseShellExecute = true 
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NANDDOS] No se pudo abrir el PDF para previsualizar: {ex.Message}");
            }
            
            return rutaPDF;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"No se pudo generar el PDF del ticket.\nEs probable que el archivo ya esté abierto. Por favor, ciérrelo e intente nuevamente.\n\nDetalle: {ex.Message}",
                "Error al generar ticket",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return null;
        }
    }

    // Pagina 1: Comprobante para el cliente. No incluye diagnostico ni problema.
    private static void CrearPaginaCliente(PdfDocument documento, string codigoEquipo, string cliente, string telefono)
    {
        AgregarEncabezado(documento);

        var seccion = new PdfParagraph("Copia: Cliente", Fuente(13f, PdfFont.BOLD))
        {
            Alignment = PdfElement.ALIGN_LEFT,
            SpacingBefore = 20f
        };
        documento.Add(seccion);
        documento.Add(new PdfParagraph(" "));

        documento.Add(new PdfParagraph($"Código de equipo:  {codigoEquipo}", Fuente(11f, PdfFont.NORMAL)));
        documento.Add(new PdfParagraph($"Fecha:  {DateTime.Now:dd/MM/yyyy  HH:mm}", Fuente(11f, PdfFont.NORMAL)));
        documento.Add(new PdfParagraph($"Cliente:  {cliente}", Fuente(11f, PdfFont.NORMAL)));
        documento.Add(new PdfParagraph($"Teléfono:  {telefono}", Fuente(11f, PdfFont.NORMAL)));

        var nota = new PdfParagraph(
            "\nConserve este comprobante para retirar su equipo.",
            Fuente(8f, PdfFont.ITALIC, new PdfBaseColor(80, 80, 80)))
        {
            Alignment = PdfElement.ALIGN_CENTER,
            SpacingBefore = 15f
        };
        documento.Add(nota);
        
        var notaLegal = new PdfParagraph(
            "Nota: NANDDOS no se hace responsable por equipos abandonados después de 30 días.",
            Fuente(7.5f, PdfFont.NORMAL, new PdfBaseColor(50, 50, 50)))
        {
            Alignment = PdfElement.ALIGN_CENTER,
            SpacingBefore = 10f
        };
        documento.Add(notaLegal);
    }

    // Pagina 2: Comprobante para el tecnico. Incluye codigo QR con el diagnostico.
    private static void CrearPaginaTecnico(PdfDocument documento, string codigoEquipo, string cliente, string telefono, string equipoInfo, string problema)
    {
        AgregarEncabezado(documento);

        var seccion = new PdfParagraph("Copia: Técnico", Fuente(13f, PdfFont.BOLD))
        {
            Alignment = PdfElement.ALIGN_LEFT,
            SpacingBefore = 20f
        };
        documento.Add(seccion);
        documento.Add(new PdfParagraph(" "));

        documento.Add(new PdfParagraph($"Código de equipo:  {codigoEquipo}", Fuente(11f, PdfFont.NORMAL)));
        documento.Add(new PdfParagraph(" "));

        // Genera el codigo QR con ZXing y lo inserta como imagen en el PDF.
        string contenidoQR = 
            $"--- NANDDOS ---\n" +
            $"ID Equipo: {codigoEquipo}\n" +
            $"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}\n" +
            $"Cliente: {cliente}\n" +
            $"Teléfono: {telefono}\n" +
            $"Equipo: {equipoInfo}\n" +
            $"Problema/Diagnóstico: {problema}";
        var qrWriter = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new ZXing.Common.EncodingOptions
            {
                Height = 200,
                Width = 200,
                Margin = 1
            }
        };

        using var qrBitmap = qrWriter.Write(contenidoQR);
        using var stream = new MemoryStream();
        qrBitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);

        var imagenQR = PdfImage.GetInstance(stream.ToArray());
        imagenQR.Alignment = PdfElement.ALIGN_CENTER;
        imagenQR.ScaleAbsolute(120f, 120f);
        documento.Add(imagenQR);
    }

    // Encabezado comun para ambas paginas del ticket.
    private static void AgregarEncabezado(PdfDocument documento)
    {
        var titulo = new PdfParagraph("NANDDOS", Fuente(16f, PdfFont.BOLD))
        {
            Alignment = PdfElement.ALIGN_CENTER
        };
        documento.Add(titulo);

        var subtitulo = new PdfParagraph("Soporte Técnico",
            Fuente(10f, PdfFont.NORMAL, new PdfBaseColor(100, 100, 100)))
        {
            Alignment = PdfElement.ALIGN_CENTER,
            SpacingAfter = 10f
        };
        documento.Add(subtitulo);
    }
}
