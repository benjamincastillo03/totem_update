using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Speech.Synthesis;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static wpf_vista_totem.MainWindow;
using Squirrel;
using WebDav;
using MaterialDesignThemes.Wpf;

using IWshRuntimeLibrary;
using System.Reflection;
using System.Diagnostics;
using System.ComponentModel;

//using System.Windows.Forms;


namespace wpf_vista_totem.paginas {
    /// <summary>
    /// Lógica de interacción para Pagina_herramientas.xaml
    /// </summary>
    public partial class Pagina_herramientas : Page {
        public string txt_ip_server = "";
        public static IWebDavClient _client;
        private DialogHost _dialogo_imprime_ticket;
        private DialogHost _dialogo_load;
        
        public Pagina_herramientas(){

            InitializeComponent();
        
            int ind_servidor = int.Parse(Environment.GetEnvironmentVariable("ind_servidor"));
            Servidor_conexion(ind_servidor);
            string voice_defaul = Environment.GetEnvironmentVariable("voice_defaul");
            Seleccion_Voces(voice_defaul);

            this.txt_mimac.Text = ObtenerDireccionIP() + " | " + GetMacAddress();
            this._dialogo_imprime_ticket = (DialogHost)FindName("load_busqueda_nube");
            this._dialogo_load = (DialogHost)FindName("dialogo_load");
        }

        //iniciando descarga
        private async void Button_Click2(object sender, RoutedEventArgs e){
            this._dialogo_load.IsOpen = true;
            var cadena_socket = Environment.GetEnvironmentVariable("ip_socket");
            string[] arr_cadena_sk = cadena_socket.Split(':');
            this.txt_ip_server = arr_cadena_sk[0];
            await InitializeWebDavClient(arr_cadena_sk[0]);
        }

        private async Task InitializeWebDavClient(string baseAddress) {
            string _Uri = "http://" + baseAddress + ":7777/remote.php/dav/files/administrador";
            Console.WriteLine(" ->  "+ _Uri);
            this.txt_ip_server = _Uri;
            _client = new WebDavClient(new WebDavClientParams {
                BaseAddress = new Uri(_Uri),
                Credentials = new NetworkCredential("administrador","totem.2010")
            });
            int aux = 0;
            List<KeyValuePair<string,int>> remote_archivos = new List<KeyValuePair<string, int>>();
            try {
                var propfindResponse = await _client.Propfind(_Uri + "/Documents/");
                if (propfindResponse.IsSuccessful) {
                    Console.WriteLine("Conexión exitosa!");
                    foreach (var resource in propfindResponse.Resources) {
                        if (aux != 0) {
                            remote_archivos.Add(new KeyValuePair<string, int>(resource.Uri, aux));
                        }
                        aux++;
                    }
                    versiones_wpf.ItemsSource = remote_archivos;
                    versiones_wpf.DisplayMemberPath = "Key";
                } else  {
                    Console.WriteLine("Error en la conexión: " + propfindResponse.StatusCode);
                }
            }  catch (Exception ex)  {
                Console.WriteLine("Error: " + ex.Message);
            }
            _client.Dispose();
            this._dialogo_load.IsOpen = false;
            this._dialogo_imprime_ticket.IsOpen = true;
        }

        private async void gestion_Descarga(object sender, RoutedEventArgs e){
            var selectedItem = (KeyValuePair<string, int>?)versiones_wpf.SelectedItem;
            if (selectedItem == null) {
                MessageBox.Show("El elemento seleccionado es null.");
                return;
            } else {
                string Key = selectedItem.Value.Key;
                string[] arr_ruta = Key.Split('/');
                string ultimoElemento = arr_ruta[arr_ruta.Length - 1];
                int selectedValue = selectedItem.Value.Value;
                await Descarga_local(Key,ultimoElemento);
            }
        }

        public async Task Descarga_local(string txt_ruta_local, string txt_archivo){
            string localFilePath = "C:/a_descargas/" + txt_archivo;
            string path = @"C:\a_descargas/";
            if (!Directory.Exists(path)){
                Directory.CreateDirectory(path);
            }
            string nextcloudUrl = this.txt_ip_server;
            string username = "administrador";
            string password = "totem.2010";
            string remoteFilePath = txt_ruta_local;
            using (HttpClient client = new HttpClient()){
                client.BaseAddress = new Uri(nextcloudUrl);
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{username}:{password}")));
                using (var response = await client.GetAsync(remoteFilePath)){
                    if (response.IsSuccessStatusCode){
                        using(var fileStream = new FileStream(localFilePath, FileMode.Create, FileAccess.Write, FileShare.None)) {
                            await response.Content.CopyToAsync(fileStream);
                        }
                        //Console.WriteLine("Archivo descargado con éxito");
                        MessageBox.Show("Archivo descargado con éxito en :"+ path);
                    } else  {
                        Console.WriteLine($"Error al descargar el archivo: {response.StatusCode}");
                    }
                }
            }
        }


        public void get_anclar_inicio(object sender, RoutedEventArgs e){
            AddToStartup();
            /*
            MessageBox.Show("Para mejorar la experiencia del usuario, nos gustaría que nuestra aplicación se inicie automáticamente al arrancar Windows. ¿Desea permitir esto?",
                    "Iniciar con Windows",
                    MessageBoxButton.YesNo);
            */
        }

        private void AddToStartup(){
            string startupPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            Console.WriteLine("--------------------------------------");
            Console.WriteLine($" startupPath  -> " + startupPath);
            string shortcutPath = System.IO.Path.Combine(startupPath, "totem_angol.lnk");
            if (!System.IO.File.Exists(shortcutPath)){
                WshShell shell = new WshShell();
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
                shortcut.Description = "App de escritorio del hospital";
                shortcut.TargetPath = Assembly.GetExecutingAssembly().Location;  // Ruta del .exe de tu aplicación
                shortcut.Save();
            }
        }


        public static string GetMacAddress(){
            var macAddr = (from nic in NetworkInterface.GetAllNetworkInterfaces()
                           where nic.OperationalStatus == OperationalStatus.Up
                           select nic.GetPhysicalAddress().ToString()
            ).FirstOrDefault();
            return macAddr.ToString();
        }

        private string ObtenerDireccionIP(){
            string direccionIP = string.Empty;
            IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in hostEntry.AddressList){
                if (ip.AddressFamily == AddressFamily.InterNetwork) {
                    direccionIP = ip.ToString();
                    break;
                }
            }
            return direccionIP;
        }

        private void Servidor_conexion(int ind_servidor)  {
            List<KeyValuePair<string,int>> items = new List<KeyValuePair<string,int>>();
            items.Add(new KeyValuePair<string,int>("Entorno de desarrollo - localhost",0));
            items.Add(new KeyValuePair<string,int>("QA", 1));
            items.Add(new KeyValuePair<string,int>("En Producción - 10.69.76.39", 2));
            this.Usodeltotem.ItemsSource = items;
            this.Usodeltotem.SelectedIndex = ind_servidor;
            this.Usodeltotem.DisplayMemberPath = "Key";
            this.Usodeltotem.SelectionChanged += Usodeltotem_SelectionChanged;
            //Console.WriteLine(" Servidor_conexion ----------------------- ->  " + ind_servidor.ToString());
        }

        private void Usodeltotem_SelectionChanged(object sender, SelectionChangedEventArgs e){
            var selectedItem        =   (KeyValuePair<string,int>)Usodeltotem.SelectedItem;
            int selectedValue       =   selectedItem.Value;
            string ip_socket;
            string ip_oracle;
            switch(selectedValue){
                case 0: //localhost
                    //ip_socket     =   "localhost:5000/instancia_totem_wpf";
                    //ip_oracle     =   "localhost:1521";
                    ip_socket       =   "10.68.159.13:5000/instancia_totem_wpf";
                    ip_oracle       =   "10.68.159.13:1521";
                break;
                case 1: //QA
                    ip_socket       =   "10.69.76.39:5000/instancia_totem_wpf";
                    ip_oracle       =   "10.69.76.39:1522";
                break;
                case 2: //Produccion
                    ip_socket       =   "10.69.76.39:5000/instancia_totem_wpf";
                    ip_oracle       =   "10.69.76.39:1521";
                break;
                default:
                    ip_socket       =   "10.68.159.13:3000/instancia_totem_wpf";
                    ip_oracle       =   "10.68.159.13:1521";
                break;
            }
            string rutaArchivo      =   Environment.GetEnvironmentVariable("ruta_confi");
            string jsonString       =   System.IO.File.ReadAllText(rutaArchivo);
            dynamic datos           =   JsonConvert.DeserializeObject(jsonString);
            datos.ind_servidor      =   selectedValue.ToString();
            datos.ip_socket         =   ip_socket;
            datos.ip_oracle         =   ip_oracle;
            string output           =   JsonConvert.SerializeObject(datos,Formatting.Indented);
            System.IO.File.WriteAllText(rutaArchivo,output);
            MessageBox.Show("Aplicación se reiniciara para completar cambios");
            Application.Current.Shutdown();
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
        }

        private void Seleccion_Voces(string voice_defaul){
            SpeechSynthesizer synth = new SpeechSynthesizer();
            foreach(InstalledVoice voice in synth.GetInstalledVoices()){
                VoiceInfo info = voice.VoiceInfo;
                string name = info.Name;
                this.comboBox_Voces.Items.Add(name);
            }
            this.comboBox_Voces.SelectedItem = voice_defaul;
        }

        private void CambioVoces(object sender, SelectionChangedEventArgs e){
            string itemSeleccionado = comboBox_Voces.SelectedItem as string;
            GlobalVariables.GetInstalledVoices = itemSeleccionado;
            string rutaArchivo = Environment.GetEnvironmentVariable("ruta_confi");
            string jsonString = System.IO.File.ReadAllText(rutaArchivo);
            dynamic datos = JsonConvert.DeserializeObject(jsonString);
            datos.voice_defaul = itemSeleccionado;
            string output = JsonConvert.SerializeObject(datos, Formatting.Indented);
            System.IO.File.WriteAllText(rutaArchivo,output);
        }

        private async void Button_Click(object sender, RoutedEventArgs e){
            Console.WriteLine("     inician     ");
            await HandleUpdates();
        }

        private void get_pabel_control(object sender, RoutedEventArgs e){
            Process.Start("ms-settings:");
        }

        private void Administrador_tareas(object sender, RoutedEventArgs e){
            Process.Start("taskmgr.exe");
        }

        private void Teclado_Pantalla(object sender, RoutedEventArgs e) {
            try  {
                //Process.Start(@"C:\Windows\System32\osk.exe");
                string ruta = @"C:\a_descargas";  // Reemplaza esto con la ruta que desees.
                Process.Start("explorer.exe", ruta);
            } catch (Win32Exception ex)   {
                MessageBox.Show($"Error al ejecutar osk.exe: {ex.Message}");
            }   catch (Exception ex)  {
                MessageBox.Show($"Error inesperado: {ex.Message}");
            }
        }

        //https://10.68.159.13:3000/update_nuget
        //C:\Users\LENOVO\source\repos\wpf_vista_totem\wpf_vista_totem\Releases
        private async Task HandleUpdates(){
                try  {
                    Console.WriteLine("_________________________");
                    Console.WriteLine("     empieza el show     ");
                    using (var manager = new UpdateManager("https://10.68.159.13:3000/update_nuget")){
                        await manager.UpdateApp();
                    }
                } catch (Exception ex){
                    Console.WriteLine("     error     ");
                    Console.WriteLine(ex);
                    MessageBox.Show(ex.ToString());
                }
            }
        }
    }

