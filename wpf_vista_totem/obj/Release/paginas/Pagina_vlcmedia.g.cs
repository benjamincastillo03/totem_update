﻿#pragma checksum "..\..\..\paginas\Pagina_vlcmedia.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "EF252EEF90DF3DFB1738AE1D6A8E48E70A9D672D3CA72DBB7AD2B2A0BF30C500"
//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

using LibVLCSharp.WPF;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using wpf_vista_totem.paginas;


namespace wpf_vista_totem.paginas {
    
    
    /// <summary>
    /// Pagina_vlcmedia
    /// </summary>
    public partial class Pagina_vlcmedia : System.Windows.Controls.Page, System.Windows.Markup.IComponentConnector {
        
        
        #line 30 "..\..\..\paginas\Pagina_vlcmedia.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock txt_ventanilla;
        
        #line default
        #line hidden
        
        
        #line 45 "..\..\..\paginas\Pagina_vlcmedia.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal LibVLCSharp.WPF.VideoView videoView;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/TOTEM_FARMACIA;component/paginas/pagina_vlcmedia.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\paginas\Pagina_vlcmedia.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.txt_ventanilla = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 2:
            this.videoView = ((LibVLCSharp.WPF.VideoView)(target));
            return;
            case 3:
            
            #line 71 "..\..\..\paginas\Pagina_vlcmedia.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Click_1);
            
            #line default
            #line hidden
            return;
            case 4:
            
            #line 72 "..\..\..\paginas\Pagina_vlcmedia.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Click_2);
            
            #line default
            #line hidden
            return;
            case 5:
            
            #line 73 "..\..\..\paginas\Pagina_vlcmedia.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            
            #line 74 "..\..\..\paginas\Pagina_vlcmedia.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Click_3);
            
            #line default
            #line hidden
            return;
            case 7:
            
            #line 75 "..\..\..\paginas\Pagina_vlcmedia.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Dragon_Ball);
            
            #line default
            #line hidden
            return;
            case 8:
            
            #line 76 "..\..\..\paginas\Pagina_vlcmedia.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Malcom_TV);
            
            #line default
            #line hidden
            return;
            case 9:
            
            #line 77 "..\..\..\paginas\Pagina_vlcmedia.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.teoriaDelbigbang);
            
            #line default
            #line hidden
            return;
            case 10:
            
            #line 78 "..\..\..\paginas\Pagina_vlcmedia.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.tvn);
            
            #line default
            #line hidden
            return;
            case 11:
            
            #line 79 "..\..\..\paginas\Pagina_vlcmedia.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Stop_class);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
