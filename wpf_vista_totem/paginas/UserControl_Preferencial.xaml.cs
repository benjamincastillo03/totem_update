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
    /// Lógica de interacción para UserControl_Preferencial.xaml
    /// </summary>
    public partial class UserControl_Preferencial : UserControl {
        public UserControl_Preferencial(){
            InitializeComponent();
        }

        public event EventHandler ControlU_Finalizado_Preferncial;
        public int num_Ticket_finalizado { get; set; }
        
        private void Print_ticket_AdultoMayor(object sender, RoutedEventArgs e) {
            num_Ticket_finalizado = 2;
            Control_user_fin();
        }

        private void Print_ticket_Embarazadas(object sender, RoutedEventArgs e){
            num_Ticket_finalizado = 3;
            Control_user_fin();
        }

        private void Print_ticket_Cuidadores(object sender, RoutedEventArgs e){
            num_Ticket_finalizado = 4;
            Control_user_fin();
        }

        private void imprime_ticket_carnet_discapacidad(object sender, RoutedEventArgs e){
            num_Ticket_finalizado = 5;
            Control_user_fin();
        }

        private void btn_vuelve_inicio(object sender, RoutedEventArgs e){
            num_Ticket_finalizado = 0;
            Control_user_fin();
        }

        private void Control_user_fin() {
            ControlU_Finalizado_Preferncial?.Invoke(this, EventArgs.Empty);
        }
    }
}
