using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace TOTEM_FARMACIA.paginas
{
    /// <summary>
    /// Lógica de interacción para SegundaVentana.xaml
    /// </summary>
    public partial class SegundaVentana : Window {

        double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
        double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
        private DispatcherTimer timer_venanilla1 = new DispatcherTimer();
        private int counter = 0;
        private TextBlock currentTextBlock;
        //private TextBlock[] textBlocks;
        public SegundaVentana() {
            InitializeComponent();
            SizeChanged += MainWindow_SizeChanged; // Suscribirse al evento SizeChanged
            timer_venanilla1.Interval = TimeSpan.FromSeconds(0.5); // Intervalo de parpadeo
            timer_venanilla1.Tick += Timer_Tick;
            this.txt_ventanilla_1.Text = "---";
            this.txt_ventanilla_2.Text = "---";
            this.txt_ventanilla_3.Text = "---";
            this.txt_ventanilla_4.Text = "---";
        }

        private void Timer_Tick(object sender, EventArgs e){
            if (counter < 10) // Duración del parpadeo (10 veces * 0.5 segundos = 5 segundos)
            {
                currentTextBlock.Opacity = (currentTextBlock.Opacity == 1 ? 0 : 1);
                counter++;
            }  else   {
                timer_venanilla1.Stop();
                currentTextBlock.Opacity = 1;
            }
        }

        private void StartBlinking(TextBlock textBlock){
            counter = 0;
            currentTextBlock = textBlock;
            timer_venanilla1.Start();
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e) {

            Console.WriteLine("--------------------------------------------------------------------------");
            Console.WriteLine("--------------- MainWindow_SizeChanged -----------------------------------");
            Console.WriteLine(" screenWidthxscreenHeight    :   " + screenWidth + "x" + screenHeight);

            double height = e.NewSize.Height;
            double fontSize = height / 4;
            this.txt_ventanilla_1.FontSize = fontSize;
            this.txt_nombreventanilla_1.FontSize = fontSize / 3;
            this.txt_ventanilla_2.FontSize = fontSize;
            this.txt_nombreventanilla_2.FontSize = fontSize / 3;
            this.txt_ventanilla_3.FontSize = fontSize;
            this.txt_nombreventanilla_3.FontSize = fontSize / 3;
            this.txt_ventanilla_4.FontSize = fontSize;
            this.txt_nombreventanilla_4.FontSize = fontSize / 3;
        }

        public void Gestion_llamada(string txt_nombre_ticker, string txt_ventanilla){
            string[] words      =   txt_ventanilla.Split('°');
            string num_Ventana  =   words[1];
            //Console.WriteLine("-------Gestion_llamada-----------");
            //Console.WriteLine(words[0]);
            string N_Vennilla   =   Regex.Replace(words[1], @"\s", "");
            //Console.WriteLine(N_Vennilla);
            if (N_Vennilla == "1"){
                this.txt_ventanilla_1.Text = txt_nombre_ticker;
                StartBlinking(this.txt_ventanilla_1);
            }
            if (N_Vennilla == "2"){
                this.txt_ventanilla_2.Text = txt_nombre_ticker;
                StartBlinking(this.txt_ventanilla_2);
            }
            if (N_Vennilla == "3"){
                this.txt_ventanilla_3.Text = txt_nombre_ticker;
                StartBlinking(this.txt_ventanilla_3);
            }
            if (N_Vennilla == "4"){
                this.txt_ventanilla_4.Text = txt_nombre_ticker;
                StartBlinking(this.txt_ventanilla_4);
            }
        }
    }
}
