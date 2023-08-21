using System;
using System.Collections.Generic;
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

namespace TOTEM_FARMACIA.paginas {
    /// <summary>
    /// Lógica de interacción para UserControl_ini.xaml
    /// </summary>
    public partial class UserControl_ini : UserControl {

        public int InformacionAdicional { get; set; }
        public event EventHandler ControlUsuarioFinalizado;

        public UserControl_ini()  {
            InitializeComponent();
        }

        private void Opt_Publico_General(object sender, RoutedEventArgs e){
            Console.WriteLine("1.- PUBLICO GENERAL <-   ");
            Finalizado_Inicio(1);
        }

        private void Opt_Publico_Preferencial(object sender, RoutedEventArgs e)  {
            Console.WriteLine("2.- PUBLICO PREFERENCIAL <-     ");
            Finalizado_Inicio(2);
        }

        private void Finalizado_Inicio(int ind_opcion){
            InformacionAdicional = ind_opcion;
            ControlUsuarioFinalizado?.Invoke(this, EventArgs.Empty);
        }

    }
}
