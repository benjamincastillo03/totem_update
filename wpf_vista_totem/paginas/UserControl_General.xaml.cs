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

namespace TOTEM_FARMACIA.paginas
{
    /// <summary>
    /// Lógica de interacción para UserControl_General.xaml
    /// </summary>
    public partial class UserControl_General : UserControl{
        public UserControl_General(){
            InitializeComponent();
        }
        public event EventHandler ControlU_Finalizado_General;
        public int num_Ticket_finalizado { get; set; }
        public int Imprimio { get; set; }
        private void Print_ticket_PublicoGeneral(object sender, RoutedEventArgs e){
            num_Ticket_finalizado = 1;
            ControlU_Finalizado_General?.Invoke(this, EventArgs.Empty);
        }
        private void btn_vuelve_inicio(object sender, RoutedEventArgs e) {
            num_Ticket_finalizado = 0;
            ControlU_Finalizado_General?.Invoke(this, EventArgs.Empty);
        }
    }
}
