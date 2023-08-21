using Newtonsoft.Json;
using SocketIOClient;
using SocketIOClient.Transport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Windows;
using System.Windows.Input;
using System.Net.Sockets;
using Newtonsoft.Json.Linq;

namespace wpf_vista_totem.controlador {

    public class ConstructorWebsocket {

        //const string v_rura_ws_proxy = "http://10.68.159.13:3000/instancia_totem_wpf";
        //https://github.com/doghappy/socket.io-client-csharp

        public SocketIO client_cx;

        public bool ind_conectado;
        public bool ind_estado;
        
        public string miip { get; set; }
        public string ip_totem { get; set; }
        public string txt_estado { get; set; }
        public int ind_opcion { get; set; }
        public delegate void d_estadoconexion(string txt_estado);

        public ConstructorWebsocket(string txt_mi_ip_local){
            this.miip = mi_ip_local();
            string ip_socket = Environment.GetEnvironmentVariable("ip_socket");
            string txt_new_conexion = "http://" + ip_socket;
            client_cx = new SocketIO(txt_new_conexion, new SocketIOOptions {
                Transport = TransportProtocol.WebSocket,
                Reconnection = true,
                ReconnectionAttempts = 1,
                AutoUpgrade = true,
                Query = new List<KeyValuePair<string, string>>{
                    new KeyValuePair<string,string>("totem","true"),
                    new KeyValuePair<string,string>("ip_totem",ObtenerDireccionIP()),
                    new KeyValuePair<string,string>("mac_totem",GetMacAddress()),
                },
            });
        }

        //public delegate void new_estadosoketio(bool _return);
        public delegate Task new_estadosoketio(bool _return);
        public event new_estadosoketio delegado_estadosoketio;
        //load conexion + manejos de errores de socket.io
        public async Task Conectado(){

            client_cx.OnConnected += async (sender, e) => {
                Console.WriteLine("+++++++++++++++ client_cx conectado ++++++++++++++++");
                this.ind_estado = true;
                this.txt_estado = "Conectado";
                await delegado_estadosoketio(true);
            };

            client_cx.OnDisconnected += async (sender, e) => {
                this.ind_estado = false;
                this.txt_estado = "OnDisconnected";
                await delegado_estadosoketio(true);
                Console.WriteLine("--------------------------------------------------------");
                Console.WriteLine("+++++++++++++++client_cx OnDisconnected ++++++++++++++++");
                await Task.Delay(5000); // Esperar un segundo
                await client_cx.DisconnectAsync();
                await Task.Delay(1000);
                try  {
                    await client_cx.ConnectAsync();
                }  catch (Exception ex){
                    Console.WriteLine("*****************************************************");
                    Console.WriteLine($"Se ha producido una excepción al conectarse: {ex.Message}");
                    // Aquí puedes implementar la lógica para manejar la excepción, como intentar reconectar
                }
            };

            client_cx.OnReconnected += async (sender, e) => {
                Console.WriteLine("+++++++++++++++client_cx OnReconnected ++++++++++++++++");
                this.ind_estado = true;
                this.txt_estado = "OnReconnected";
                await delegado_estadosoketio(true);
            };

            client_cx.OnReconnectAttempt += async (sender, e) => {
                Console.WriteLine("+++++++++++++++client_cx OnReconnectAttempt ++++++++++++++++");
                this.ind_estado = true;
                this.txt_estado = "OnReconnectAttempt";
                await delegado_estadosoketio(true);
            };

            client_cx.OnReconnectError += async (sender, e) => {
                Console.WriteLine("+++++++++++++++client_cx OnReconnectError ++++++++++++++++");
                this.ind_estado = true;
                this.txt_estado = "OnReconnectError";
                await delegado_estadosoketio(true);
            };

            client_cx.OnError += async (sender, e) => {
                this.ind_estado = false;
                this.txt_estado = "OnError";
                await delegado_estadosoketio(true);
                Console.WriteLine("+++++++++++++++client_cx OnError ++++++++++++++++");
            };

            client_cx.OnReconnectFailed += async (sender, e) => {
                this.ind_estado = false;
                this.txt_estado = "OnReconnectFailed";
                await delegado_estadosoketio(true);
                Console.WriteLine("+++++++++++++++client_cx OnReconnectError ++++++++++++++++");
                await Task.Delay(5000); // Esperar un segundo
                await Task.Delay(1000);
                try {
                    await client_cx.ConnectAsync();
                } catch (Exception ex) {
                    Console.WriteLine("     *********************************************************   ");
                    Console.WriteLine($"Se ha producido una excepción al conectarse: {ex.Message}");
                }
            };
            /*
            client_cx.OnPing += async (sender, e) => {
                this.ind_estado = true;
                this.txt_estado = "OnPing";
                DateTime timeStamp = DateTime.Now;
                Console.WriteLine("-----------------------------------------------");
                string timeStampStr = timeStamp.ToString("yyyy-MM-dd HH:mm:ss.fff");
                Console.WriteLine("client_cx OnPing ->: " + timeStampStr);
            };

            client_cx.OnPong += async (sender, e) => {
                this.ind_estado = true;
                this.txt_estado = "OnPong";
                DateTime timeStamp = DateTime.Now;
                string timeStampStr = timeStamp.ToString("yyyy-MM-dd HH:mm:ss.fff");
                Console.WriteLine("-----------------------------------------------");
                Console.WriteLine("client_cx OnPong     ->  " + timeStampStr);
            };
            */
            try {
                await client_cx.ConnectAsync();
            }  catch (Exception ex) {
                Console.WriteLine("Exception      ->  " + ex);
            }
        }

        public bool status_estado(){
            return this.ind_estado;
        }

        //***********************************************************************************
        //api de voz
        public delegate Task api_voz_delegado(string PartName, string run, string txtfirmasimple, string r_id_uid);
        public event api_voz_delegado fun_api_voz;
        public Task escuchando_api_voz(){
            client_cx.On("app_totem_defaul_api_voz", async response =>{
                string text = response.GetValue<string>();
                await fun_api_voz(text, "", "", "");
            });
            return Task.CompletedTask;
        }
        //***********************************************************************************

        public string estado_socket_io(){
            string txt_estado;
            if (this.txt_estado == "") {
                txt_estado          =   "No informado";
            } else {
                txt_estado          =   this.txt_estado;
            } 
            return txt_estado;
        }
        
        public delegate Task notifica_ping(string ind_pagina);
        public event notifica_ping delegado_ping_esissan;
        public Task esissan_ping_pong(){
            client_cx.On("totem_escuchando_ping", async response => {
                string text         =   response.GetValue<string>();
                await delegado_ping_esissan(text);
                await esissan_respues_pong(text);
            });
            return Task.CompletedTask;
        }

        public async Task esissan_respues_pong(string text){
            await client_cx.EmitAsync("respuesta_pong_esissan",response => { }, 
                JsonConvert.SerializeObject(new{ _id_cliente = text}
            ));
        }

        public delegate Task new_ticket_impresora(string ind_pagina);
        public event new_ticket_impresora delegado_texto_imprime;
        public Task escucha_Ticket_totem(){
            client_cx.On("imprimir_ticket_totem", async response => {
                string text = response.GetValue<string>();
                await delegado_texto_imprime(text);
            });
            return Task.CompletedTask;
        }

        public async Task emite_nuevo_ticket(int _prioridad){
            try  {
                await client_cx.EmitAsync("emite_nuevo_ticket", response => { }, JsonConvert.SerializeObject(new   {
                    _id_priopiedad = _prioridad,
                }));
            }  catch (Exception ex) {
                // Manejo de la excepción
                MessageBox.Show("Error de conexion ", ex.Message.ToString());
            }
        }

        public async Task anunciNuevoTicket(string _prioridad,string sub_numero)   {
            await client_cx.EmitAsync("anunciNuevoTicket", response => { }, JsonConvert.SerializeObject(new{
                txt_subnumero = sub_numero,
                ind_propidad = _prioridad,
            }));
        }
        

        public delegate Task new_llamada_ticket(string ind_sonido,string txt_nombre_ticker,string txt_ventanilla);
        public event new_llamada_ticket delegado_llamada_ticket;
        public Task escucha_TicketmasVentanilla(){
            client_cx.On("escucha_llamado_ticket", async response => {
                string text = response.GetValue<string>();
                Call_ticket m = JsonConvert.DeserializeObject<Call_ticket>(text);
                string ind_sonido = m.ind_sonido;
                string txt_nombre_ticker = m.txt_nombre_ticker;
                string txt_ventanilla = m.txt_ventanilla;
                await delegado_llamada_ticket(ind_sonido, txt_nombre_ticker, txt_ventanilla);
            });
            return Task.CompletedTask;
        }

        class Call_ticket { 
            public string ind_sonido { get; set; }
            public string txt_nombre_ticker { get; set; }
            public string txt_ventanilla { get; set; }
        }

        public string ObtenerDireccionIP() {
            string direccionIP = string.Empty;
            IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in hostEntry.AddressList){
                if (ip.AddressFamily == AddressFamily.InterNetwork){
                    direccionIP = ip.ToString();
                    break;
                }
            }
            return direccionIP;
        }

        public delegate Task pasar(string PartName, string run, string txtfirmasimple, string r_id_uid);
        public event pasar pasado;
        //escuha_full_scream
        public Task escuha_full_scream(){
            client_cx.On("app_totem_full_scream_get", async response => {
                await pasado("0", "", "", "");
            });
            return Task.CompletedTask;
        }

        public Task escuha_win_Minimize(){
            client_cx.On("app_totem_win_Minimize_get", async response => {
                await pasado("1", "", "", "");
            });
            return Task.CompletedTask;
        }

        public Task escuha_win_ApagaWindows(){
            client_cx.On("app_totem_win_Apagado", async response => {
                await pasado("2", "", "", "");
            });
            return Task.CompletedTask;
        }

        public delegate Task ind_tipo_pagina(string ind_pagina);
        public event ind_tipo_pagina deleg_tipo_pagina;
        public Task app_totem_cambio_pagina_get()  {
            client_cx.On("app_totem_cambio_pagina_get", async response => {
                string text = response.GetValue<string>();
                await deleg_tipo_pagina(text);
            });
            return Task.CompletedTask;
        }

        public string mi_ip_local() {
            string strHostName = Dns.GetHostName();
            IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
            IPAddress[] addr = ipEntry.AddressList;
            return addr[addr.Length - 1].ToString();
        }

        public static string GetMacAddress() {
            var macAddr = (from nic in NetworkInterface.GetAllNetworkInterfaces()
                           where nic.OperationalStatus == OperationalStatus.Up
                           select nic.GetPhysicalAddress().ToString()  ).FirstOrDefault();
            return macAddr.ToString();
        }
    }
}
