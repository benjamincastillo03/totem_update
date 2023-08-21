using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
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
    /// Lógica de interacción para pagina_inicio.xaml
    /// </summary>
    public partial class pagina_inicio : Page {

        public pagina_inicio()  {
            InitializeComponent();
            //this.txt_ip.Text = mi_ip_local();
            //this.txt_ip.Text = ObtenerDireccionIP();
            //this.txt_mimac.Text = GetMacAddress();
            //this.Height = 1600;
        }

        public string mi_ip_local()  {
            string strHostName      =   Dns.GetHostName();
            IPHostEntry ipEntry     =   Dns.GetHostEntry(strHostName);
            IPAddress[] addr        =   ipEntry.AddressList;
            return addr[addr.Length-1].ToString();
        }

        public static string GetMacAddress()  {
            var macAddr = (from nic in NetworkInterface.GetAllNetworkInterfaces()
                where nic.OperationalStatus == OperationalStatus.Up
                select nic.GetPhysicalAddress().ToString()
            ).FirstOrDefault();
            return macAddr.ToString();
        }

        public string ObtenerDireccionIP()  {
            string direccionIP = string.Empty;
            IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in hostEntry.AddressList)  {
                if (ip.AddressFamily == AddressFamily.InterNetwork) {
                    direccionIP = ip.ToString();
                    break;
                }
            }
            return direccionIP;
        }
    }
}
