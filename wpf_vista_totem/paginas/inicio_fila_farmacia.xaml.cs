using MaterialDesignThemes.Wpf;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using wpf_vista_totem.controlador;
using wpf_vista_totem.modelo;

using ESC_POS_USB_NET.Printer;
using ESC_POS_USB_NET.Enums;
using System.Management;
using ToastNotifications;
using TOTEM_FARMACIA;
using TOTEM_FARMACIA.paginas;
using MaterialDesignColors;
using static TOTEM_FARMACIA.paginas.UserControl_ini;

namespace wpf_vista_totem.paginas {
    /// <summary>
    /// Lógica de interacción para inicio_fila_farmacia.xaml
    /// </summary>
    public partial class inicio_fila_farmacia : Page {

        //private DialogHost _dialogo_preferencial;
        //private DialogHost _dialogo_imprime_ticket;
        //private DialogHost _info_sin_papel2;
        private DialogHost _dialogo_transicion_page;
        //private Card _transicion_print;
        private DialogHost _dialogHost;
        private DispatcherTimer _timer;
        public inicio_fila_farmacia() {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e){
            //ini dialogos
            _dialogHost                 =   (DialogHost)FindName("load_carga_inicio");
            _dialogHost.IsOpen          =   true;
            _timer                      =   new DispatcherTimer();
            _timer.Interval             =   TimeSpan.FromSeconds(3);
            _timer.Tick                 +=  load_CierraDialogo;
            _timer.Start();
            _dialogo_transicion_page    =   (DialogHost)FindName("load_transicion_page");
            //_dialogo_imprime_ticket     =   (DialogHost)FindName("load_imprimiendo_ticket");
            //_info_sin_papel2          =   (DialogHost)FindName("info_sin_papel2");
            Control_User_inicio(0);
        }
        private void load_CierraDialogo(object sender, object e){
            _dialogHost.IsOpen = false;
        }

        public UserControl_ini nuevoControl;

        private void Control_User_inicio(int _opt){
            if (_opt == 1) {
                this.sp_control_ususario.Visibility = Visibility.Visible;
                
                //load_imprimiendo_ticket.IsOpen = false;
                //Thread.Sleep(3000);
            }

            nuevoControl = new UserControl_ini();
            this.gridContainer.Children.Add(nuevoControl);
            nuevoControl.Name = "Pagina_inicio";
            nuevoControl.ControlUsuarioFinalizado += UserControl_ControlTerminado;
        }

        private void UserControl_ControlTerminado(object sender, EventArgs e){
            //Star_Pagina_Transicion();
            UserControl_ini control = sender as UserControl_ini;
            int informacionAdicional = control.InformacionAdicional;
            this.gridContainer.Children.Remove(nuevoControl);
            if (informacionAdicional == 1){
                Control_User_General();
            } else {
                Control_User_Preferencial();
            }
        }


        public UserControl_General UserControl_General;
        private void Control_User_General()  {
            UserControl_General = new UserControl_General();
            this.gridContainer.Children.Add(UserControl_General);
            UserControl_General.Name = "ControlUser_geneal";
            UserControl_General.ControlU_Finalizado_General += ControlU_Finalizado_General;
        }

        public UserControl_Preferencial UserControl_Preferencial;
        private void Control_User_Preferencial() {
            UserControl_Preferencial = new UserControl_Preferencial();
            this.gridContainer.Children.Add(UserControl_Preferencial);
            UserControl_Preferencial.Name = "ControlUser_preferncial";
            UserControl_Preferencial.ControlU_Finalizado_Preferncial += ControlU_Finalizado_Preferncial;
        }

        private void ControlU_Finalizado_General(object sender, EventArgs e){
            UserControl_General control = sender as UserControl_General;
            int num_Ticket_finalizado = control.num_Ticket_finalizado;
            this.gridContainer.Children.Remove(UserControl_General);
            if (num_Ticket_finalizado == 0)  {
                Control_User_inicio(1);
            } else {
                fn_Opcion_Elegida_Tiket(num_Ticket_finalizado);
            }
        }


        private void ControlU_Finalizado_Preferncial(object sender, EventArgs e){
            UserControl_Preferencial control = sender as UserControl_Preferencial;
            int num_Ticket_finalizado = control.num_Ticket_finalizado;
            this.gridContainer.Children.Remove(UserControl_Preferencial);
            if (num_Ticket_finalizado == 0){
                Control_User_inicio(1);
            } else {
                fn_Opcion_Elegida_Tiket(num_Ticket_finalizado);
            }
        }

        private void load_CierraTransicion(object sender, object e){
            this.sp_control_ususario.Visibility = Visibility.Visible;
            _dialogo_transicion_page.IsOpen = false;
        }

        private async void fn_Opcion_Elegida_Tiket(int num_Tiket){
            //this.sp_control_ususario.Visibility = Visibility.Hidden;
            this.load_imprimiendo_ticket.IsOpen = true;
            Control_User_inicio(1);
            await Inst_insertbd(num_Tiket.ToString());
        }

        /*
        private DispatcherTimer _timer_cticket;
        private void load_ImprimierndoTicket(object sender, object e){
            this.sp_control_ususario.Visibility = Visibility.Visible;
            load_imprimiendo_ticket.IsOpen = false;
            _timer_cticket.Stop();
        }
        */

        public delegate Task new_return_paciente(string _ind_prioridad, string sub_numero);
        public event new_return_paciente delegado_return_paciente;
        configuracion_bd configuracion_bd;
        private async Task<bool> Inst_insertbd(string _priopedad){
            configuracion_bd = new configuracion_bd();
            DataTable table = configuracion_bd.genera_nuevo_ticket(_priopedad);
            if (table.Rows.Count > 0)  {
                string txt_subnumero = "";
                foreach (DataRow row in table.Rows){
                    txt_subnumero = row["V_SUB_NUMERO"].ToString();
                    Console.WriteLine("IND_PRIORIDAD.       {0}", row["IND_PRIORIDAD"]);
                    Console.WriteLine("V_SUB_NUMERO.        {0}", row["V_SUB_NUMERO"]);
                    Console.WriteLine("V_UNICO_FARMACIA.    {0}", row["V_UNICO_FARMACIA"]);
                    Console.WriteLine("FECHA_TICKET.        {0}", row["FECHA_TICKET"]);
                }  try  {
                    this.load_imprimiendo_ticket.IsOpen = false;
                    await Task.Run(() => imprime_ticket(txt_subnumero));
                    await delegado_return_paciente(_priopedad,txt_subnumero);
                }  catch (Exception ex) {
                    Console.WriteLine(ex.ToString());
                }
            }  else  {
                Console.WriteLine("No hay datos en el DataTable");
            }
            return true;
        }

        private Task<bool> imprime_ticket(string txt_subnumero)  {
            string[] words = txt_subnumero.Split('-');
            string txt_tipo_paciente = "";
            switch (words[0]){
                case "A":
                    txt_tipo_paciente   =   "PUBLICO GENERAL";
                    break;
                case "B":
                    txt_tipo_paciente   =   "ADULTO MAYOR";
                    break;
                case "C":
                    txt_tipo_paciente   =   "EMBARAZADA";
                    break;
                case "D":
                    txt_tipo_paciente   =   "CUIDADORES";
                    break;
                case "E":
                    txt_tipo_paciente   =   "CARNET DISCAPACITADO";
                    break;
                default:
                    txt_tipo_paciente   =   "TEST";
                    break;
            }

            string fechaactual = DateTime.Now.ToShortDateString();
            string horaActual = DateTime.Now.ToString("HH:mm:ss");
            Printer printer = new Printer("BK-T680");
            printer.ExpandedMode(PrinterModeState.On);
            
            printer.Append("HOSPITAL");
            printer.Append("MAURICIO HEYERMANN");
            printer.Append("FARMACIA");
            printer.Append("ANTENCION ABIERTA");

            printer.ExpandedMode(PrinterModeState.Off);
            printer.Append("SU NUMERO DE TICKET ES : ");
            printer.Separator();
            printer.DoubleWidth3();
            printer.AlignCenter();
            printer.Append(" \x1d\x21\x35" + txt_subnumero);
            printer.ExpandedMode(PrinterModeState.Off);
            printer.Separator();
            printer.ExpandedMode(PrinterModeState.Off);

            printer.Append(" TIPO DE PACIENTE: " + txt_tipo_paciente);
            printer.Append(" FECHA : " + fechaactual);
            printer.Append(" HORA : " + horaActual);

            printer.Append(" HORARIO DE ATENCION");
            printer.Append(" 08:30  A 17:00 ");
            printer.Append(" SABADO, DOMINGO Y FESTIVOS ");
            printer.Append(" CERRADO ");

            printer.FullPaperCut();
            printer.PrintDocument();
            return Task.FromResult(true);
        }

        private void Star_Pagina_Transicion(){
            _dialogo_transicion_page.IsOpen = true;
            this.sp_control_ususario.Visibility = Visibility.Hidden;
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += load_CierraTransicion;
            _timer.Start();
            //return Task.FromResult(true);
        }


        /*
        private DispatcherTimer _transicion_preferencial;
        private DialogHost _dialogo_preferencial;
        //inicio publico preferencial
        private void Button_Click_1(object sender, RoutedEventArgs e) {
            Console.WriteLine("1.- INICIO -> publico preferencial <-     " + this.progressBar.Value.ToString());
            oculta_all();
            _dialogo_preferencial.IsOpen = true;
            _transicion_preferencial = new DispatcherTimer();
            _transicion_preferencial.Interval = TimeSpan.FromSeconds(3);
            _transicion_preferencial.Tick += load_OpenListaPreferencial;
            _transicion_preferencial.Start();
        }

        //inicio publico general
        private void Irapagina_Publigeneral(object sender, RoutedEventArgs e) {
            Console.WriteLine("2.- INICIO -> publico general <-     " + this.progressBar.Value.ToString());
            oculta_all();
            _dialogo_preferencial.IsOpen = true;
            _transicion_preferencial = new DispatcherTimer();
            _transicion_preferencial.Interval = TimeSpan.FromSeconds(3);
            _transicion_preferencial.Tick += load_openLitaGeneral;
            _transicion_preferencial.Start();
        }

        private void load_OpenListaPreferencial(object sender, object e) {
            _dialogo_preferencial.IsOpen = false;
            this.ind_publico_preferencial.Visibility = Visibility.Visible;
            _transicion_preferencial.Stop();
            temporizadorGoback();
        }

        private void load_openLitaGeneral(object sender, object e) {
            _dialogo_preferencial.IsOpen = false;
            this.ind_publico_general.Visibility = Visibility.Visible;
            _transicion_preferencial.Stop();
            temporizadorGoback();
        }

        //timer para volver a tras
        public DispatcherTimer timer;
        private void temporizadorGoback() {
            timer = new DispatcherTimer();
            timer.Stop();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += OnTimerTick;
            this.ind_load.Visibility = Visibility.Visible;
            timer.Start();
        }

        private async void OnTimerTick(object sender, EventArgs e) {
            progressBar.Value += 1;
            if (progressBar.Value == progressBar.Maximum) {
                Console.WriteLine(" progressBar.Value : " + progressBar.Value.ToString());
                this.ind_load.Visibility = Visibility.Hidden;
                await reseteaTemporizador(0);
                await oculta_all();
                load_Inicio();
            }
        }

        void load_Inicio() {
            this.ind_inicio_pagina.Visibility = Visibility.Visible;
        }

       

      




        //-----------------------------------------
       


        
        private Task<bool> return_dialog_ok(){
            this.ind_load.Visibility                    =   Visibility.Hidden;
            this.ind_publico_general.Visibility         =   Visibility.Hidden;
            this.ind_publico_preferencial.Visibility    =   Visibility.Hidden;
            this.ind_inicio_pagina.Visibility           =   Visibility.Hidden;
            _transicion_preferencial                    =   new DispatcherTimer();
            _transicion_preferencial.Interval           =   TimeSpan.FromSeconds(5);
            _transicion_preferencial.Tick               +=  load_vuelve_inicio;
            _transicion_preferencial.Start();
            return Task.FromResult(true);
        }

        private Task<bool> reseteaTemporizador(int _prioridad){
            if (_prioridad !=0) {
                Comprueba_impresoraAsync2();
            }
            timer.Stop();
            this.progressBar.Value                      =   0;
            this.progressBar.Minimum                    =   0;
            this.progressBar.Maximum                    =   60;
            return Task.FromResult(true);
        }

        private Task Comprueba_impresoraAsync2(){
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Printer WHERE Name = 'BK-T680'"))  {
                ManagementObjectCollection printers = searcher.Get();
                if (printers.Count > 0)  {
                    foreach (ManagementObject printer in printers)
                    {
                        string status = printer["PrinterStatus"].ToString();
                        Console.WriteLine(" ----------------------- ");
                        Console.WriteLine(" status   :   " + status);
                        // 3 indica "Listo"
                        if (status == "3")  {
                            try  {
                                //Console.WriteLine("La impresora está encendida.");
                            }
                            catch (Exception ex)   {
                                Console.WriteLine(ex.ToString());
                                this._info_sin_papel2.IsOpen = true;
                            }
                        } else {
                            Console.WriteLine("error    :   " + status);
                            this._info_sin_papel2.IsOpen = true;
                        }
                    }
                }  else  {

                }
            }
            return Task.CompletedTask;
        }

        private void load_vuelve_inicio(object sender, object e){
            _dialogo_imprime_ticket.IsOpen              =   false;
            _transicion_preferencial.Stop();
            this.ind_inicio_pagina.Visibility           =   Visibility.Visible;
        }

        
        */
    }
}
