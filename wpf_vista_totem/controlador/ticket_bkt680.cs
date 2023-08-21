using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using ToastNotifications.Messages.Core;
using System.Windows;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Documents;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Web.UI.WebControls;
using Image = System.Drawing.Image;
using System.Linq.Expressions;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;  // Importar este espacio de nombres para utilizar la clase BarcodeWriter
using System.Management;
using System.Net.Mail;
using System.Net;
using System.ComponentModel;

namespace wpf_vista_totem.controlador {

    internal class ticket_bkt680 {
        private PrintDocument printDocument;
        private string txt_imprime;
        private string txt_tipo_paciente;
        public ticket_bkt680(string txt_imprime){
            string[] words                  =   txt_imprime.Split('-');
            this.txt_imprime                =   txt_imprime;
            switch (words[0]){
                case "A":
                    this.txt_tipo_paciente  =   "PÚBLICO GENERAL";
                    break;
                case "B":
                    this.txt_tipo_paciente  =   "ADULTO MAYOR";
                    break;
                case "C":
                    this.txt_tipo_paciente  =   "EMBARAZADA";
                    break;
                case "D":
                    this.txt_tipo_paciente  =   "CUIDADORES";
                    break;
                case "E":
                    this.txt_tipo_paciente  =   "CARNET DISCAPACITADO";
                    break;
                default:
                    this.txt_tipo_paciente  =   "TEST";
                    break;
            }
            //Ver si impresora tiene papel y si esta conectada.
            printDocument                                       =   new PrintDocument();
            printDocument.PrinterSettings.PrinterName           =   "BK-T680";
            printDocument.PrintController                       =   new StandardPrintController(); // Desactivar diálogo "Imprimiendo"
            printDocument.PrintPage                             +=  PrintDocument_PrintPage;
        }

        public void Print_2() {
            printDocument.Print();
        }

        public void Print(){
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Printer WHERE Name = 'BK-T680'"))
            {
                ManagementObjectCollection printers = searcher.Get();
                if (printers.Count > 0)
                {
                    foreach (ManagementObject printer in printers)
                    {
                        string status = printer["PrinterStatus"].ToString();
                        // 3 indica "Listo"
                        if (status == "3")
                        {
                            Console.WriteLine("La impresora está encendida.");
                            try
                            {
                                printDocument.Print();
                                //worker.RunWorkerAsync(textToPrint);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($" Error en la impresión : '{ex}' ");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Lo siento, parece que la impresora está apagada o se ha quedado sin papel. Por favor, diríjase a la ventanilla correspondiente para obtener su número de forma manual. Gracias por su comprensión.");
                            //EnviarCorreo();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("No se encontró la impresora.");
                }
            }
        }

        //Posición de inicio para dibujar el texto
        //Generar el código QR
        //BarcodeWriter barcodeWriter   =   new BarcodeWriter();
        //barcodeWriter.Format          =   BarcodeFormat.QR_CODE;
        //Bitmap qrCodeBitmap           =   new Bitmap(barcodeWriter.Write("WIFI:T:WPA;S:SSID_RED;P:CLAVE_RED"), new System.Drawing.Size(80,80));
        //Dibujar el código QR en el objeto Graphics
        //graphics.DrawImage(qrCodeBitmap, new System.Drawing.Point(10,205));
        //y += font_xs.Height;

        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e){
            string txt_numero       =   this.txt_imprime;
            string tipo_paciente    =   this.txt_tipo_paciente;
            // Fuente y tamaño de texto
            Font font_xs            =   new Font("Arial",9);
            Font font               =   new Font("Arial",12);
            Font font_lg            =   new Font("Arial",35);
            float x                 =   10;
            float y                 =   60;
            string fechaactual      =   DateTime.Now.ToShortDateString();
            string horaActual       =   DateTime.Now.ToString("HH:mm:ss");
            string base64Image      =   get_base64_100();
            byte[] imageBytes       =   Convert.FromBase64String(base64Image);
            using (Graphics graphics      =   e.Graphics){
                using (MemoryStream ms    =   new MemoryStream(imageBytes)){
                    Image image           =   Image.FromStream(ms);
                    graphics.DrawImage(image, 0, 0, image.Width - 40, image.Height);
                    //Dibuja el texto para la boleta
                    graphics.DrawString("Su número de ticket es:", font, System.Drawing.Brushes.Black, x, y);
                    y += font.Height;
                    graphics.DrawString("N° " + txt_numero, font_lg, System.Drawing.Brushes.Black, x, y);
                    y += font_lg.Height;
                    graphics.DrawString("Tipo de paciente : " + tipo_paciente, font, System.Drawing.Brushes.Black, x, y);
                    y += font.Height;
                    graphics.DrawString("Fecha : " + fechaactual + " | " + horaActual, font, System.Drawing.Brushes.Black, x, y);
                    y += font.Height;
                    graphics.DrawString("Fila virtual - Farmacia atención abierta", font, System.Drawing.Brushes.Black, x, y);
                    y += font.Height;
                    graphics.DrawString("Horario, lunes a vienes. 08:00 a 17:00 hrs.", font, System.Drawing.Brushes.Black, x, y);
                    y += font.Height;
                    graphics.DrawString("Sabado, domingo y festivos. Cerrado", font, System.Drawing.Brushes.Black, x, y);
                    //y += font.Height;
                    graphics.DrawString("Departamento De Informática", font_xs, System.Drawing.Brushes.Black, x, y);
                }
            }
        }

        private string get_base64_100() {
            string txt_return;
            txt_return = "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";
            return txt_return;
        }

        public void EnviarCorreo(){
            /*
            // Configurar el cliente SMTP de Gmail
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential("totem.angol@gmail.com", "totem.2010");
            */
            /*
            SmtpClient smtpClient = new SmtpClient("smtp.araucanianorte.cl", 587);
            smtpClient.EnableSsl = false;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential("benjamin.castillo@araucanianorte.cl", "16869726");
            MailMessage mensaje = new MailMessage();
                        mensaje.From = new MailAddress("luis.robles@araucanianorte.cl");
                        mensaje.To.Add(new MailAddress("benjamin.castillo@araucanianorte.cl"));
                        mensaje.To.Add(new MailAddress("eduardo.mendoza@araucanianorte.cl"));
                        mensaje.Subject = "TOTEM SIN PAPEL";
                        mensaje.Body = "TOTEM SIN PAPEL";
            */
            //Adjuntar un archivo (opcional)
            //mensaje.Attachments.Add(new Attachment("ruta_del_archivo"));
            // Enviar el mensaje
            /*
            try  {
                smtpClient.Send(mensaje);
                Console.WriteLine("El correo electrónico se ha enviado correctamente.");
            }
            catch (SmtpException ex)
            {
                Console.WriteLine("Error al enviar el correo electrónico: " + ex.Message);
            }
            */
        }
    }
}
