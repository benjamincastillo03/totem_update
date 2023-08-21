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

namespace wpf_vista_totem.paginas {
    /// <summary>
    /// Lógica de interacción para Pagina_TableroxVentanilla.xaml
    /// </summary>
    public partial class Pagina_TableroxVentanilla : Page  {
        public Pagina_TableroxVentanilla(string _n_ticket, string _txt_ventanilla) {
            InitializeComponent();
            if (_txt_ventanilla != "")  {   txt_ventanilla.Text     =   _txt_ventanilla;  }
            if (_n_ticket       != "")  {   txt_ticket.Text         =   "N°"+_n_ticket;   }
        }
    }
}
