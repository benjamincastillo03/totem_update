using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using wpf_vista_totem.controlador;
using wpf_vista_totem.paginas;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using ToastNotifications.Messages;
using System.Net.NetworkInformation;
using Newtonsoft.Json;
using System.Speech.Synthesis;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.IO;
using System.Data;
using System.Diagnostics;
using System.Collections.Generic;
using MaterialDesignThemes.Wpf;
using ESC_POS_USB_NET.Printer;
using ESC_POS_USB_NET.Enums;
using System.Drawing.Printing;
using System.Management;
using TOTEM_FARMACIA.controlador;
using System.Windows.Interop;
//using System.Windows.Forms;
//using System.Windows.Interop;
using WpfScreenHelper.Enum;
using TOTEM_FARMACIA.paginas;
using System.Text;
using System.Printing;
using System.Runtime.InteropServices;
using System.Net.Http;

namespace wpf_vista_totem {
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public static class GlobalVariables {
            public static string GetInstalledVoices { get; set; }
            public static int GetNumerosVoces { get; set; }
        }
        
        //********************************************
        //https://github.com/raflop/ToastNotifications
        //********************************************
        
        public Notifier notifier = new Notifier(cfg => {
            cfg.PositionProvider = new WindowPositionProvider(
                parentWindow: System.Windows.Application.Current.MainWindow,
                corner: Corner.TopRight,
                offsetX: 10,
                offsetY: 10);
            cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                notificationLifetime: TimeSpan.FromSeconds(2),
                maximumNotificationCount: MaximumNotificationCount.FromCount(5));
            cfg.Dispatcher = System.Windows.Application.Current.Dispatcher;
        });

        private DialogHost _dialogo_load_pide_pass;
        private DialogHost _info_sin_papel;
        public MainWindow() {
            InitializeComponent();
            Config_variables_entorno();
            fecha_hora_inicio();
            Base64_logo100();//logo 
            Base64_logoEsissan();
            Listdo_desbloquedado(1);
            //Show_tablero_segundapantalla();
            Show_tablero_segundapantalla_wpf(1);
            _dialogo_load_pide_pass =   (DialogHost)FindName("load_pide_pass");
            _info_sin_papel         =   (DialogHost)FindName("info_sin_papel");
        }

        public ConstructorWebsocket constructorWebsocket;
        private async Task InitializeConstructorWebsocketAsync(){
            constructorWebsocket = new ConstructorWebsocket(mi_ip_local());
            await SubscribeToEstadoSocketioAsync();
            await constructorWebsocket.Conectado();
            await constructorWebsocket.escuha_full_scream();
            await constructorWebsocket.escuha_win_Minimize();
            await constructorWebsocket.escuha_win_ApagaWindows();
            await constructorWebsocket.escuchando_api_voz();
            await constructorWebsocket.app_totem_cambio_pagina_get();
            await constructorWebsocket.esissan_ping_pong();
            await constructorWebsocket.escucha_Ticket_totem();
            await constructorWebsocket.escucha_TicketmasVentanilla();
        }

        public Task SubscribeToEstadoSocketioAsync(){
            constructorWebsocket.delegado_estadosoketio += new ConstructorWebsocket.new_estadosoketio(fun_estado_socketio);
            constructorWebsocket.pasado += new ConstructorWebsocket.pasar(ejecutar);
            constructorWebsocket.fun_api_voz += new ConstructorWebsocket.api_voz_delegado(func_api_voz_windows);
            constructorWebsocket.deleg_tipo_pagina += new ConstructorWebsocket.ind_tipo_pagina(func_cambia_pagina);
            constructorWebsocket.delegado_ping_esissan += new ConstructorWebsocket.notifica_ping(func_notifica_ping_ping);
            constructorWebsocket.delegado_texto_imprime += new ConstructorWebsocket.new_ticket_impresora(fun_nueva_impresion);
            constructorWebsocket.delegado_llamada_ticket += new ConstructorWebsocket.new_llamada_ticket(fun_Ejecuta_callticket);
            return Task.CompletedTask;
        }

        private void Listdo_desbloquedado(int opcion){
            if (opcion == 0) {
                AddListBoxItem<ListBoxItem>("Página inicio farmacia central", get_PaginaInicio);
                AddListBoxItem<ListBoxItem>("Fila Virtual – Farmacia", get_Gestorfilasvirtual);
                AddListBoxItem<ListBoxItem>("Tablero Ticket Farmacia", Get_tableroxVentanilla);
                //AddListBoxItem<ListBoxItem>("Cable Pirata", get_mediavlc);
                AddListBoxItem<ListBoxItem>("Heramientas", Get_Opciones_windows);
                AddListBoxItem<ListBoxItem>("Maximizar", get_fullscreem);
                AddListBoxItem<ListBoxItem>("Minimizar", get_Minimizar);
                //AddListBoxItem<ListBoxItem>("Test impresora - BK-T680", get_test_impresora);
                //AddListBoxItem<ListBoxItem>("Test impresora (Desuso) | BK-T680", get_test_impresora);
                AddListBoxItem<ListBoxItem>("Test impresión | BK-T680 ", get_RawPrinterHelper);
                AddListBoxItem<ListBoxItem>("Mensajes Impresora | BK-T680 ", get_Gestion_Mesajes);
                //AddListBoxItem<ListBoxItem>("Test mesajes ", EnviaWasap);
                AddListBoxItem<ListBoxItem>("Bloquear Menu", get_Bloquear_menu);
                //AddListBoxItem<ListBoxItem>("Suspender Equipo", get_SuspenderPc);
                AddListBoxItem<ListBoxItem>("Apagar PC", get_ApagarPc);
                AddListBoxItem<ListBoxItem>("Reinicio Aplicativo", Get_reinicio_aplicacion);
                AddListBoxItem<ListBoxItem>("Cerrar Aplicativo", get_cerrar_aplicacion);
            } else {
                AddListBoxItem<ListBoxItem>("Información", get_Desbloqueo);
                //AddListBoxItem<ListBoxItem>("Test impresión | BK-T680 ", get_RawPrinterHelper);
            }
        }
        
        private void get_ApagarPc(object sender, MouseButtonEventArgs e){
            Apagar_pc();
        }

        // Importar la función SetSuspendState de la API de Windows
        [DllImport("powrprof.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern bool SetSuspendState(bool hibernate, bool forceCritical, bool disableWakeEvent);
        private void get_SuspenderPc(object sender, MouseButtonEventArgs e){
            //SetSuspendState(false, true, true);
        }

        public SegundaVentana window;
        public bool bSegunda_pantalla { get; set; }
        private void Show_tablero_segundapantalla_wpf(int opcion) {
            var screens = System.Windows.Forms.Screen.AllScreens;
            var extendedScreen = screens.FirstOrDefault(s => !s.Primary);
            if (extendedScreen == null){
                //Console.WriteLine("--------------- NO se detecta pantalla extendida ---------------");
                bSegunda_pantalla = false;
            } else {
                bSegunda_pantalla = true;
                window = new SegundaVentana();
                window.WindowStartupLocation = WindowStartupLocation.Manual; // establecer la ubicación de inicio manualmente
                window.Left = extendedScreen.Bounds.Left;
                window.Top = extendedScreen.Bounds.Top;
                //extendedScreen.Bounds.Height
                //window.WindowState = WindowState.Maximized; // establecer el estado de la ventana a maximizado
                window.WindowStyle = WindowStyle.None; // establecer el estilo de la ventana a sin bordes
                //window.Topmost = true; // establecer la ventana en la parte superior
                window.Width = 1920; // establecer el ancho de la ventana en 1920
                window.Height = 1080; // establecer la altura de la ventana en 1080
                window.Show();
            }
        }

        private Task fun_Ejecuta_callticket(string ind_sonido, string txt_nombre_ticker, string txt_ventanilla){
            Application.Current.Dispatcher.Invoke(new Action(() => {
                Page pagina_actual = frame_pagina_inicio.Content as Page;
                if (pagina_actual != null) {
                    if (pagina_actual.ToString() == "wpf_vista_totem.paginas.Pagina_TableroxVentanilla") {
                        var Pagina_TableroxVentanilla = new Pagina_TableroxVentanilla(txt_nombre_ticker, txt_ventanilla);
                        this.frame_pagina_inicio.Content = null;
                        this.frame_pagina_inicio.Navigate(Pagina_TableroxVentanilla);
                    }
                }
                AgregarMensaje("Ticket :" + txt_nombre_ticker + ", " + txt_ventanilla); // agregar mensaje a la cola FIFO
                ProcesarMensajes(); // procesar mensajes en orden FIFO
                if (bSegunda_pantalla){
                    Console.WriteLine("Gestion_llamada -> bSegunda_pantalla");
                    window.Gestion_llamada(txt_nombre_ticker,txt_ventanilla);
                }
            }));
            return Task.CompletedTask;
        }

        private async void get_Gestion_Mesajes(object sender, MouseButtonEventArgs e){
            await Comprueba_impresoraAsync();
        }

        private async Task Comprueba_impresoraAsync(){
            using (PrintQueue printQueue = new PrintQueue(new PrintServer(),"BK-T680")){
                bool Creaticket_automatico = false;
                if (printQueue.IsOffline){
                    // La impresora está desconectada
                    this.info_sin_papel.IsOpen = true;
                    Creaticket_automatico = true;
                }  else if (printQueue.IsOutOfPaper)  {
                    // La impresora está sin papel
                    this.info_sin_papel.IsOpen = true;
                    Creaticket_automatico = true;
                }  else if (printQueue.IsPaperJammed)  {
                    // La impresora tiene un atasco de papel
                    this.info_sin_papel.IsOpen = true;
                }  else if (printQueue.IsBusy)  {
                    // La impresora está ocupada
                    this.info_sin_papel.IsOpen = true;
                    Creaticket_automatico = true;
                } else  {
                    // La impresora está lista para imprimir
                }
                string txt_ind_servidor                             =   Environment.GetEnvironmentVariable("ind_servidor");
                //Console.WriteLine(" ----------------------------------------------------------");
                //Console.WriteLine(" Falla control de impresora  ->  "   +   Creaticket_automatico);
                //Console.WriteLine(" txt_ind_servidor            ->  "   +   txt_ind_servidor);
                if(Creaticket_automatico){
                    if (txt_ind_servidor!="0") {
                        await Creacion_autocaticaticker();
                    }
                }
            }
            //return Task.CompletedTask;
        }

        private async Task Creacion_autocaticaticker() {
            var url = "http://10.69.76.49/portal/Apis/ApiTotem/create";
            var client = new HttpClient();
            // Crear el objeto JSON que quieres enviar
            var ip_totem = mi_ip_local();
            //var json = "{ \"ip\": \"10.68.159.13\" }";
            var json = "{\"ip\":\"" + ip_totem + "\"}";
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            // Añadir la clave de API al encabezado
            client.DefaultRequestHeaders.Add("X-Api-Key","u5c2LqXsGn82");
            // Hacer la solicitud POST
            // Imprimir el código de estado HTTP y el cuerpo de la respuesta
            try {
                var response = await client.PostAsync(url, data);
                var result = await response.Content.ReadAsStringAsync();
                //Console.WriteLine(response.ToString());
                //Console.WriteLine(result.ToString());
            } catch (Exception e){
                Console.WriteLine(e.ToString());
                //MessageBox.Show(e.ToString());  
            }
        }

        private void get_Bloquear_menu(object sender, MouseButtonEventArgs e){
            this.myListBox.Items.Clear();
            Listdo_desbloquedado(1);
        }

        private void get_Desbloqueo(object sender, MouseButtonEventArgs e){
            this.txt_pass.Password = "";
            this._dialogo_load_pide_pass.IsOpen = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e){
            Button button = (Button)sender;
            this.txt_pass.Password += button.Content.ToString();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e){
            if (txt_pass.Password.Length > 0){
                txt_pass.Password = txt_pass.Password.Substring(0, txt_pass.Password.Length - 1);
            }
        }

        private void Confirma_Desbloqueo(object sender, RoutedEventArgs e){
            if (txt_pass.Password.Length > 0) {
                if (txt_pass.Password == "1405"){
                    notifier.ShowSuccess("Desbloqueado");
                    this._dialogo_load_pide_pass.IsOpen = false;
                    this.myListBox.Items.Clear();
                    Listdo_desbloquedado(0);
                } else {
                    notifier.ShowError("Error");
                }
            } else {
                notifier.ShowError("Pass Vacio");
            }
        }
        
        private async void Window_Loaded(object sender, RoutedEventArgs e) {
            await InitializeConstructorWebsocketAsync();
        }

        public Task ejecutar(string PartName, string run, string txtfirmasimple, string r_id_uid){
            try  {
                Application.Current.Dispatcher.Invoke(new Action(() => {
                    if (PartName == "0"){
                        WindowStyle = WindowStyle.None;
                        WindowState = WindowState.Maximized;
                    }
                    if (PartName == "1"){
                        WindowStyle = WindowStyle.ThreeDBorderWindow;
                        WindowState = WindowState.Normal;
                    }
                    
                    if (PartName == "2"){
                        try  {
                            Apagar_pc();
                        }   catch (Exception ex)  {
                            Console.WriteLine("Error al apagar la PC: " + ex.Message);
                        }
                    }

                }));
            }  catch (Exception e) {
                MessageBox.Show(e.Message);
            }
            return Task.CompletedTask;
        }

        public void Apagar_pc() {
            try  {
                Process.Start("shutdown", "/s /f /t 0");
            } catch (Exception ex)  {
                Console.WriteLine("Error al apagar la PC: " + ex.Message);
            }
        }

        public Task<bool> fun_estado_socketio(bool _return)  {
            try  {
                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() => {
                    if (_return)  {
                        notifier.ShowSuccess("Conectado a instancia totems");
                    }  else   {
                        notifier.ShowError("Desconectado de instancia");
                    }
                    Icono_Socket(_return);
                }));
            }  catch (Exception e)  {
                notifier.ShowError("Desconocido - " + e.Message);
            }
            return Task.FromResult(_return);
        }

        //Obtener la lista de pantallas disponibles
        private void Show_tablero_segundapantalla(){
            string txt_ind_servidor     =   Environment.GetEnvironmentVariable("ind_servidor");
            var startInfo               =   new ProcessStartInfo("chrome.exe");
            startInfo.WindowStyle       =   ProcessWindowStyle.Maximized;
            //Obtener la lista de pantallas disponibles
            System.Windows.Forms.Screen[] screens = System.Windows.Forms.Screen.AllScreens;
            //Seleccionar la segunda pantalla si está disponible, de lo contrario, utilizar la pantalla principal
            System.Windows.Forms.Screen secondaryScreen = (screens.Length > 1) ? screens[1] : System.Windows.Forms.Screen.PrimaryScreen;
            //Configurar la posición y el tamaño de la ventana en la segunda pantalla
            int windowWidth             =   (int)(secondaryScreen.Bounds.Width * 0.75); //Establecer el ancho de la ventana al 75% del ancho de la pantalla
            int windowHeight            =   (int)(secondaryScreen.Bounds.Height * 0.75); //Establecer la altura de la ventana al 75% de la altura de la pantalla
            int windowLeft              =   secondaryScreen.Bounds.Left + ((secondaryScreen.Bounds.Width - windowWidth) / 2); //Centrar la ventana horizontalmente en la pantalla
            int windowTop               =   secondaryScreen.Bounds.Top + ((secondaryScreen.Bounds.Height - windowHeight) / 2); //Centrar la ventana verticalmente en la pantalla
            if (txt_ind_servidor        ==  "2") {
                startInfo.Arguments     =   $"--kiosk --window-size={windowWidth},{windowHeight} --window-position={windowLeft},{windowTop} https://10.69.76.39:3000/tablero?ind="+ txt_ind_servidor;
                //startInfo.Arguments   =   $"--kiosk --window-size={windowWidth},{windowHeight} --window-position={windowLeft},{windowTop} https://www.esissan.cl/ssan_tot_filafarmacia?externo=true&ind=" + txt_ind_servidor;
            } else {
                startInfo.Arguments     =   $"--kiosk --window-size={windowWidth},{windowHeight} --window-position={windowLeft},{windowTop} https://10.68.159.13:3000/tablero?ind=" + txt_ind_servidor;
                //startInfo.Arguments   =   $"--kiosk --window-size={windowWidth},{windowHeight} --window-position={windowLeft},{windowTop} https://10.5.183.210/ssan_tot_filafarmacia?externo=true&ind=" + txt_ind_servidor;
            }
            Process.Start(startInfo);
        }

        public void Config_variables_entorno(){
            string rutaArchivo      =   AppDomain.CurrentDomain.BaseDirectory + "archivo.json";
            //Console.WriteLine("rutaArchivo  ->  "+ rutaArchivo);
            Environment.SetEnvironmentVariable("ruta_confi", rutaArchivo);
            string ind_servidor;
            string ip_socket;
            string ip_oracle;
            string page_star;
            string voice_defaul;
            if (!File.Exists(rutaArchivo)){
                Console.WriteLine("NO existe el archivo");
                ind_servidor        =   "0";
                ip_socket           =   "localhost:3000/instancia_totem_wpf";
                ip_oracle           =   "localhost:1521";
                page_star           =   "1";
                voice_defaul        =   txt_voz_default();
                //configuracion default
                var _configuracion  =   new {
                    ind_servidor    =   ind_servidor,
                    ip_socket       =   ip_socket,
                    ip_oracle       =   ip_oracle,
                    voice_defaul    =   voice_defaul,
                    page_star       =   page_star,
                };
                string json         =   JsonConvert.SerializeObject(_configuracion);
                File.WriteAllText(rutaArchivo,json);
                cambio_pagina_principal(page_star);
            }  else  {
                Console.WriteLine("Si existe el archivo");
                using (StreamReader archivoJson = File.OpenText(rutaArchivo)){
                    Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
                    _Configuracion      configuracion = (_Configuracion)serializer.Deserialize(archivoJson, typeof(_Configuracion));
                    ind_servidor    =   configuracion.ind_servidor;
                    ip_socket       =   configuracion.ip_socket;
                    ip_oracle       =   configuracion.ip_oracle;
                    voice_defaul    =   configuracion.voice_defaul;
                    page_star       =   configuracion.page_star;  
                }
            }
            Environment.SetEnvironmentVariable("ind_servidor", ind_servidor);
            Environment.SetEnvironmentVariable("ip_socket", ip_socket);
            Environment.SetEnvironmentVariable("ip_oracle", ip_oracle);
            Environment.SetEnvironmentVariable("voice_defaul", voice_defaul);
            Thread.Sleep(1000);
            cambio_pagina_principal(page_star);
        }
       
        static Queue<string> colaMensajes = new Queue<string>(); // creación de la cola FIFO
        static SemaphoreSlim semaforo = new SemaphoreSlim(1); // creación de un semáforo con un recuento inicial de 1
        public object WPFScreenHelper { get; private set; }
        static void AgregarMensaje(string mensaje){
            colaMensajes.Enqueue(mensaje); // agregar mensaje a la cola FIFO
        }
        static void ProcesarMensajes(){
            while (colaMensajes.Count > 0){
                string mensaje = colaMensajes.Dequeue(); // eliminar y obtener el primer mensaje de la cola
                semaforo.Wait(); // esperar hasta que haya un "permiso" disponible en el semáforo
                Thread speechThread = new Thread(() => HablarMensaje(mensaje));
                speechThread.Start();
            }
        }
        static void HablarMensaje(string mensaje){
            try  {
                SpeakText(mensaje, "1");
            }  finally  {
                semaforo.Release(); // liberar un "permiso" en el semáforo para permitir que otro hilo pueda ejecutarse
            }
        }

        static void SpeakText(string text, string ind_sonido)  {
            Console.WriteLine(" texto a hablar  = " + text);
            try {
                string rutaArchivo = AppDomain.CurrentDomain.BaseDirectory + "export.wav";
                Console.WriteLine(" rutaArchivo = "+ rutaArchivo);
                string voice_defaul = Environment.GetEnvironmentVariable("voice_defaul");
                Console.WriteLine(" voice_defaul   |   " + voice_defaul);
                MediaPlayer mediaPlayer = new MediaPlayer();
                //mediaPlayer.Open(new Uri(@"C:\Default.wav"));
                //mediaPlayer.Open(new Uri(rutaArchivo));
                mediaPlayer.Open(new Uri(@"C:\Windows\Media\Windows Foreground.wav"));
                mediaPlayer.Play();
                //Console.WriteLine("SpeakText");
                //System.Media.SystemSounds.Exclamation.Play();
                string new_texto = text.Replace("N°"," numero ");
                System.Threading.Thread.Sleep(1000);
                SpeechSynthesizer synthesizer = new SpeechSynthesizer();
                synthesizer.Rate  = -0; //velociadad
                synthesizer.Volume = 100;
                synthesizer.SelectVoice(voice_defaul); //espanol
                synthesizer.Speak(new_texto);
                synthesizer.Dispose(); //livera recursos
            }  catch (Exception e){
                System.Windows.MessageBox.Show(e.Message);
            }
        }

        public Task func_api_voz_windows(string PartName, string run, string txtfirmasimple, string r_id_uid){
            try {
                Application.Current.Dispatcher.Invoke(new Action(() => {
                    Thread speechThread = new Thread(() => SpeakText(PartName, "2"));
                    speechThread.Start();
                }));
            } catch (Exception e) {
                MessageBox.Show(e.Message);
            }
            return Task.CompletedTask;
        }

        public Task fun_nueva_impresion(string value){
            try {
                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() => {
                    //notifier.ShowInformation("Imprimiendo ticket : " + value);
                    //imprimime(value);
                }));
            }  catch (Exception e)   {
                System.Windows.MessageBox.Show(e.Message);
            }
            return Task.CompletedTask;
        }

        public Task func_notifica_ping_ping(string value){
            try {
                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() => {
                    notifier.ShowInformation("Configuracion de Totem por :" + value);
                }));
            }  catch (Exception e) {
                System.Windows.MessageBox.Show(e.Message);
            }
            return Task.CompletedTask;
        }

        public Task func_cambia_pagina(string value){
            try {
                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() => {
                    if(cambio_pagina_principal(value)){
                        notifier.ShowInformation("Cambio de pagina remoto");
                    } else {
                        notifier.ShowError("No se identifico pagina.");
                    }
                }));
            }  catch (Exception e)  {
                System.Windows.MessageBox.Show(e.Message);
            }
            return Task.CompletedTask;
        }

        public void windows_api_voz(string PartName)  {
            Thread hilo_api_voz = new Thread(new ParameterizedThreadStart(api_voz_call));
            string cadena_completa = PartName;
            hilo_api_voz.Start(cadena_completa);
        }

        private void api_voz_call(Object texto){
            try  {
                SpeechSynthesizer synthesizer = new SpeechSynthesizer();
                synthesizer.Rate = -0; //velociadad
                synthesizer.Volume = 100;
                synthesizer.SelectVoice(GlobalVariables.GetInstalledVoices); //espanol
                synthesizer.Speak(texto.ToString());
                synthesizer.Dispose(); //livera recursos
            }  catch (Exception e){
                System.Windows.MessageBox.Show(e.Message);
            }
        }
        public void VozPordefaul(){
            SpeechSynthesizer synth = new SpeechSynthesizer();
            int aux = 0;
            try {
                string txt_voce = "";
                foreach (InstalledVoice voice in synth.GetInstalledVoices()){
                    VoiceInfo info = voice.VoiceInfo;
                    if (aux == 0)  {
                        txt_voce                            =       info.Name;
                        GlobalVariables.GetInstalledVoices  =       info.Name;
                        notifier.ShowSuccess("Voz Asignada  :   "+  info.Name);
                    }
                    aux++;
                }
                //Properties.Settings.Default.Parametro   =   txt_voce;
                //Properties.Settings.Default.Save();
                GlobalVariables.GetNumerosVoces         =   aux;
            }  catch (Exception e)  {
                Console.WriteLine(e.ToString());
                GlobalVariables.GetNumerosVoces         =   0;
            }
        }
        public string txt_voz_default() {
            string txt_voce = "";
            SpeechSynthesizer synth = new SpeechSynthesizer();
            int aux = 0;
            try {
                foreach (InstalledVoice voice in synth.GetInstalledVoices()){
                    VoiceInfo info = voice.VoiceInfo;
                    if (aux == 0) {
                        txt_voce                            =   info.Name;
                        GlobalVariables.GetInstalledVoices  =   info.Name;
                        notifier.ShowSuccess("Voz Asignada  :   " + info.Name);
                    }
                    aux++;
                }
                return txt_voce;
            } catch (Exception e)  {
                Console.WriteLine(e.ToString());
                return "";
            }
        }
        public void Icono_Socket(bool _estado) {
            if (_estado) {
                this.btn_EstadoConexion.Foreground = Brushes.Green;
            } else {
                this.btn_EstadoConexion.Foreground = Brushes.Red;
            }
        }
        public void inicio_defaul(){
            var pagina_inicio = new pagina_inicio();
            this.frame_pagina_inicio.Navigate(pagina_inicio);
        }

        private void get_test_impresora(object sender, MouseButtonEventArgs e){
            notifier.ShowInformation("IMPRIMIERNTO TEST");
            //await imprimime("A-0000");
            Print_test2();
        }
        private bool cambio_pagina_principal(string value){
            bool _return = true;
            switch (value){
                case "1":
                    var pagina_inicio = new pagina_inicio();
                    this.frame_pagina_inicio.Content = null;
                    this.frame_pagina_inicio.Navigate(pagina_inicio);
                    break;
                case "2":
                    var inicio_fila_farmacia = new inicio_fila_farmacia();
                    inicio_fila_farmacia.delegado_return_paciente += new inicio_fila_farmacia.new_return_paciente(fun_new_ricket);
                    this.frame_pagina_inicio.Content = null;
                    this.frame_pagina_inicio.Navigate(inicio_fila_farmacia);
                    break;
                case "3":
                    var Pagina_vlcmedia = new Pagina_vlcmedia();
                    this.frame_pagina_inicio.Content = null;
                    this.frame_pagina_inicio.Navigated += myFrame_Navigated;
                    this.frame_pagina_inicio.Navigate(Pagina_vlcmedia);
                    break;
                case "4":
                    var Pagina_herramientas = new Pagina_herramientas();
                    this.frame_pagina_inicio.Content = null;
                    this.frame_pagina_inicio.Navigate(Pagina_herramientas);
                    break;
                case "5":
                    var Pagina_TableroxVentanilla = new Pagina_TableroxVentanilla("","");
                    this.frame_pagina_inicio.Content = null;
                    this.frame_pagina_inicio.Navigate(Pagina_TableroxVentanilla);
                    break;
                //NUEVAS PAGINAS
               


                default:
                    _return = false;
                    break;
            }
            //GUARDA ALMACENAMIENTO DE ARCHIVO 
            string rutaArchivo  =   Environment.GetEnvironmentVariable("ruta_confi");
            string jsonString   =   File.ReadAllText(rutaArchivo);
            dynamic datos       =   JsonConvert.DeserializeObject(jsonString);
            datos.page_star     =   value;
            string output       =   JsonConvert.SerializeObject(datos, Formatting.Indented);
            File.WriteAllText(rutaArchivo,output);
            return _return;
        }

        public async Task fun_new_ricket(string ind_prioridad,string txt_subnumero){
            await constructorWebsocket.anunciNuevoTicket(ind_prioridad,txt_subnumero);
            await Comprueba_impresoraAsync();
        }

        private void myFrame_Navigated(object sender, NavigationEventArgs e)  {
            Page currentPage = e.Content as Page;
            //MessageBox.Show(currentPage.Title);
        }

        public class class_tipo_pagina  {
            public string _text  { get; set; }
            public object _value { get; set; }
        }

        public void fecha_hora_inicio() {
            DispatcherTimer timer   =   new DispatcherTimer();
            timer.Interval          =   TimeSpan.FromSeconds(1);
            timer.Tick              +=  tickevent;
            timer.Start();
        }
        private void tickevent(object sender,EventArgs e) {
            string fechaactual      =   DateTime.Now.ToShortDateString();
            string horaActual       =   DateTime.Now.ToString("HH:mm:ss");
            this.mi_tiempo.Text     =   horaActual;
        }

        public class class_paciente_call {
            public string _id_mac { get; set; }
            public object _txt_nombre { get; set; }
        }

        private void get_fullscreem(object sender, MouseButtonEventArgs e){
            this.WindowStyle    =   WindowStyle.None;
            this.WindowState    =   WindowState.Maximized;
        }

        private void get_Minimizar(object sender, MouseButtonEventArgs e){
            this.WindowStyle    =   WindowStyle.ThreeDBorderWindow;
            this.WindowState    =   WindowState.Normal;
        }

        public void full(string txt_estado)  {
            notifier.ShowSuccess("FULL SCREEM");
        }
        public void d_estadoconexion(string txt_estado){
           
        }
        public string mi_ip_local()  {
            string strHostName = Dns.GetHostName();
            IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
            IPAddress[] addr = ipEntry.AddressList;
            return addr[addr.Length - 1].ToString();
        }

        public static string GetMacAddress(){
            var macAddr = (from nic in NetworkInterface.GetAllNetworkInterfaces()
                           where nic.OperationalStatus == OperationalStatus.Up
                           select nic.GetPhysicalAddress().ToString()).FirstOrDefault();
            return macAddr.ToString();
        }

        //load inicio default
        private void get_PaginaInicio(object sender, MouseButtonEventArgs e){
            var item = ItemsControl.ContainerFromElement(sender as System.Windows.Controls.ListBox, e.OriginalSource as DependencyObject) as ListBoxItem;
            if (item != null)  {
                cambio_pagina_principal("1");
            }
        }
        private void get_PaginaBusquedaCita(object sender, MouseButtonEventArgs e){
            var item = ItemsControl.ContainerFromElement(sender as System.Windows.Controls.ListBox, e.OriginalSource as DependencyObject) as ListBoxItem;
            if (item != null)  {
                var Pagina_citacion                     =   new Pagina_citacion();
                //this.frame_pagina_inicio.NavigationUIVisibility = "Hidden";
                this.frame_pagina_inicio.Content        =   null;
                this.frame_pagina_inicio.Navigate(Pagina_citacion);
            }
        }
        private void get_Gestorfilasvirtual(object sender, MouseButtonEventArgs e){
            var item = ItemsControl.ContainerFromElement(sender as System.Windows.Controls.ListBox, e.OriginalSource as DependencyObject) as ListBoxItem;
            if (item != null){
                cambio_pagina_principal("2");
            }
        }
        private void get_mediavlc(object sender, MouseButtonEventArgs e){
            var item = ItemsControl.ContainerFromElement(sender as System.Windows.Controls.ListBox, e.OriginalSource as DependencyObject) as ListBoxItem;
            if (item != null){
                cambio_pagina_principal("3");
            }
        }

        private void Get_tableroxVentanilla(object sender, MouseButtonEventArgs e){
            var item = ItemsControl.ContainerFromElement(sender as System.Windows.Controls.ListBox, e.OriginalSource as DependencyObject) as ListBoxItem;
            if (item != null){
                cambio_pagina_principal("5");
            }
        }
        private void ws_ventana_principal_totem_Unloaded(object sender, RoutedEventArgs e){
            //notifier.ShowInformation("Welcome in the jungla");
        }
        private void Base64_logo100() {
            string base64String = "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";
            var imageSource = new BitmapImage();
            using(var stream = new MemoryStream(Convert.FromBase64String(base64String))){
                stream.Seek(0,SeekOrigin.Begin);
                imageSource.BeginInit();
                imageSource.CacheOption = BitmapCacheOption.OnLoad;
                imageSource.StreamSource = stream;
                imageSource.EndInit();
            }
            this.imageControl.Source = imageSource;
        }

        private void Base64_logoEsissan(){
            string base64String         =   "iVBORw0KGgoAAAANSUhEUgAAAJYAAAAiCAYAAAHKXRAXAAAACXBIWXMAAC4jAAAuIwF4pT92AAABNmlDQ1BQaG90b3Nob3AgSUNDIHByb2ZpbGUAAHjarY6xSsNQFEDPi6LiUCsEcXB4kygotupgxqQtRRCs1SHJ1qShSmkSXl7VfoSjWwcXd7 / AyVFwUPwC / 0Bx6uAQIYODCJ7p3MPlcsGo2HWnYZRhEGvVbjrS9Xw5 + 8QMUwDQCbPUbrUOAOIkjvjB5ysC4HnTrjsN / sZ8mCoNTIDtbpSFICpA / 0KnGsQYMIN + qkHcAaY6addAPAClXu4vQCnI / Q0oKdfzQXwAZs / 1fDDmADPIfQUwdXSpAWpJOlJnvVMtq5ZlSbubBJE8HmU6GmRyPw4TlSaqo6MukP8HwGK + 2G46cq1qWXvr / DOu58vc3o8QgFh6LFpBOFTn3yqMnd / n4sZ4GQ5vYXpStN0ruNmAheuirVahvAX34y / Axk / 96FpPYgAAACBjSFJNAAB6JQAAgIMAAPn / AACA6AAAUggAARVYAAA6lwAAF2 / XWh + QAAAcfUlEQVR42qyLwUoCURSGv3M02iUGUz3ALHyAwMa9rWYzL9CmoJ1b8Tl8gtZml5wLQqBTbsaVi6CeoU0oYghq3BZNcHHdDx / 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";            
            var imageSource             =   new BitmapImage();
            using (var stream           =   new MemoryStream(Convert.FromBase64String(base64String))){
                stream.Seek(0, SeekOrigin.Begin);
                imageSource.BeginInit();
                imageSource.CacheOption     =   BitmapCacheOption.OnLoad;
                imageSource.StreamSource    =   stream;
                imageSource.EndInit();
            }
            this.imageControl2.Source = imageSource;
        }
        private void Get_Opciones_windows(object sender, MouseButtonEventArgs e){
            cambio_pagina_principal("4");
        }
        private void Get_reinicio_aplicacion(object sender, MouseButtonEventArgs e){
            Application.Current.Shutdown();
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
        }
        private void get_cerrar_aplicacion(object sender, MouseButtonEventArgs e){
            Application.Current.Shutdown();
        }

        private void get_RawPrinterHelper(object sender, MouseButtonEventArgs e){
            Print_test2();
        }

        private void Print_test2(){
            //ESC-POS-USB-NET

            string fechaactual  =   DateTime.Now.ToShortDateString();
            string horaActual   =   DateTime.Now.ToString("HH:mm:ss");
            Printer printer     =   new Printer("BK-T680");

            printer.ExpandedMode(PrinterModeState.On);
            printer.Append(" HOSPITAL ");
            printer.Append(" MAURICIO HEYERMANN ");
            printer.Append(" FARMACIA");
            printer.Append(" ANTENCION ABIERTA ");
            printer.ExpandedMode(PrinterModeState.Off);
            printer.Append(" SU NUMERO DE TICKET ES: ");
         
            printer.Separator();
            printer.AlignCenter();
            printer.Append(" \x1d\x21\x35 A-0000 ");
            printer.Separator();
            printer.ExpandedMode(PrinterModeState.Off);
            printer.Append(" TIPO DE PACIENTE: TEST ");
            printer.Append(" FECHA : " + fechaactual);
            printer.Append(" HORA : " + horaActual);
            
            printer.FullPaperCut();
            printer.PrintDocument();
        }

        private async void EnviaWasap(object sender, MouseButtonEventArgs e){
            Console.WriteLine(" ->EnviaWasap<-");
            Whatsapp_NET wasap = new Whatsapp_NET();
            try  {
                await wasap.tele_Me(); ;
            }  catch (Exception ex) {
                Console.WriteLine($"Al enviar mensaje : '{ex}' ");
                //MessageBox.Show($" Error en la impresión : '{ex}' ");
            }
            try   {
                await wasap.Envio_Mensaje(); ;
            }
            catch (Exception ex) {
                notifier.ShowError($"Al enviar mensaje : '{ex}' ");
                Console.WriteLine($"Al enviar mensaje : '{ex}' ");
                //MessageBox.Show($" Error en la impresión : '{ex}' ");
            }
            
        }

        //impresion de ticke v1 (desuso)
        private Task imprimime(string txt_imp){
            ticket_bkt680 printer = new ticket_bkt680(txt_imp);
            printer.Print();
            //printer.Print_2();
            return Task.CompletedTask;
        }

        private void AddListBoxItem<T>(string content, MouseButtonEventHandler handler) where T : ListBoxItem, new(){
            T item = new T();
            item.Content = content;
            item.PreviewMouseDown += handler;
            this.myListBox.Items.Add(item);
        }

        public class _Configuracion {
            public string ind_servidor { get; set; }
            public string ip_socket { get; set; }
            public string ip_oracle { get; set; }
            public string voice_defaul { get; set; }
            public string page_star { get; set; }
        }
    }
}
