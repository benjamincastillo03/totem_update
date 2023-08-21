using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
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

using System.IO;
using LibVLCSharp.Shared;


namespace wpf_vista_totem.paginas
{
    /// <summary>
    /// Lógica de interacción para Pagina_vlcmedia.xaml
    /// </summary>
    public partial class Pagina_vlcmedia : Page  {

        LibVLC _libVLC;
        LibVLCSharp.Shared.MediaPlayer _mediaPlayer;

        private string ind_url;
        private bool ind_play = false;

        public Pagina_vlcmedia(){
            InitializeComponent();
            this.Unloaded += OnPageUnloaded;
            this.ind_url = "http://xdgo.live:8080/benjamincastillo2021/MNZugDrqEq/23842";
            this.ind_play = true;
            videoView.Loaded += VideoView_Loaded;
        }

        private void OnPageUnloaded(object sender, RoutedEventArgs e){
            _mediaPlayer.Dispose();
        }
        private void Window_Closed(object sender, EventArgs e){
            _mediaPlayer.Dispose();
        }

        private void Stop_class(object sender, EventArgs e){
            this.ind_play = false;
            _mediaPlayer.Dispose();
        }

        void VideoView_Loaded(object sender, RoutedEventArgs e){
            Core.Initialize();
            strar_video();
        }

        void strar_video(){
            using (_libVLC = new LibVLC()){
                _mediaPlayer = new LibVLCSharp.Shared.MediaPlayer(_libVLC);
                videoView.MediaPlayer = _mediaPlayer;
                _mediaPlayer.Play(new Media(_libVLC, new Uri(this.ind_url)));
            }
        }
        private void Button_Click_1(object sender, RoutedEventArgs e){
            //novelas
            if (this.ind_play){
                _mediaPlayer.Stop();
            }
            this.ind_url                = "http://xdgo.live:8080/benjamincastillo2021/MNZugDrqEq/28909";
            strar_video();
        }

        private void Dragon_Ball(object sender, RoutedEventArgs e)
        {
            //Dragon ball z
            if (this.ind_play)
            {
                _mediaPlayer.Stop();
            }
            this.ind_url = "http://xdgo.live:8080/benjamincastillo2021/MNZugDrqEq/23822";
            strar_video();
        }

        private void Malcom_TV(object sender, RoutedEventArgs e){
            //Dragon ball z
            if (this.ind_play){
                _mediaPlayer.Stop();
            }
            this.ind_url = "http://xdgo.live:8080/benjamincastillo2021/MNZugDrqEq/23900";
            strar_video();
        }

        private void teoriaDelbigbang(object sender, RoutedEventArgs e)
        {
            //Dragon ball z
            if (this.ind_play)
            {
                _mediaPlayer.Stop();
            }
            this.ind_url = "http://xdgo.live:8080/benjamincastillo2021/MNZugDrqEq/23897";
            strar_video();
        }
        private void tvn(object sender, RoutedEventArgs e)
        {
            //Dragon ball z
            if (this.ind_play)
            {
                _mediaPlayer.Stop();
            }
            this.ind_url = "http://xdgo.live:8080/benjamincastillo2021/MNZugDrqEq/23827";
            strar_video();
        }

        
        private void Button_Click_2(object sender, RoutedEventArgs e){
            //tnt spotny 2
            if (this.ind_play){
                _mediaPlayer.Stop();
            }
            this.ind_url                =   "http://xdgo.live:8080/benjamincastillo2021/MNZugDrqEq/41171";
            strar_video();
        }
        private void Button_Click(object sender, RoutedEventArgs e){
            //El chavo
            if (this.ind_play){
                _mediaPlayer.Stop();
            }
            this.ind_url                =   "http://xdgo.live:8080/benjamincastillo2021/MNZugDrqEq/23850";
            strar_video();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e){
            //dragon ball
            if (this.ind_play) {
                _mediaPlayer.Stop();
            }
            this.ind_url                =   "http://xdgo.live:8080/benjamincastillo2021/MNZugDrqEq/23842";
            strar_video();
        }
    }
}
