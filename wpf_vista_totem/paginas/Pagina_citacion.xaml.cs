using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace wpf_vista_totem.paginas {
    /// <summary>
    /// Lógica de interacción para Pagina_citacion.xaml
    /// </summary>
    public partial class Pagina_citacion : Page {
        
        public Pagina_citacion(){
            InitializeComponent();
            this.txt_run_principal.Focus();
        }

        private void busca_citacion(object sender, RoutedEventArgs e){
            //show_pdf_citacionxaml show_Pdf_Citacionxaml = new show_pdf_citacionxaml();
            //show_Pdf_Citacionxaml.ShowDialog();
            //Process.Start("chrome.exe",@"https://www.esissan.cl/pdf/Ssan_ae_pdfCita?idBloque=453046&sobrecupo=0");
        }

        private void busca_limiaform(object sender, RoutedEventArgs e){


        }

        private void txt_run_principal_TextChanged(object sender, TextChangedEventArgs e){


        }

        private void btn_persona_especial_Click(object sender, RoutedEventArgs e){

        }
    }
}
