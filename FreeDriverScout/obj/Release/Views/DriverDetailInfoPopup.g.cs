﻿#pragma checksum "..\..\..\Views\DriverDetailInfoPopup.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "1EB1EDB40068E873A12B854DC00EE859"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.468
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

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
using WPFLocalizeExtension.Engine;
using WPFLocalizeExtension.Extensions;


namespace FreemiumUtilites {
    
    
    /// <summary>
    /// DriverDetailInfoPopup
    /// </summary>
    public partial class DriverDetailInfoPopup : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 8 "..\..\..\Views\DriverDetailInfoPopup.xaml"
        internal FreemiumUtilites.DriverDetailInfoPopup driverDetailInfo;
        
        #line default
        #line hidden
        
        
        #line 58 "..\..\..\Views\DriverDetailInfoPopup.xaml"
        internal System.Windows.Controls.Border Inner;
        
        #line default
        #line hidden
        
        
        #line 85 "..\..\..\Views\DriverDetailInfoPopup.xaml"
        internal System.Windows.Controls.TextBlock tbName;
        
        #line default
        #line hidden
        
        
        #line 88 "..\..\..\Views\DriverDetailInfoPopup.xaml"
        internal System.Windows.Controls.TextBlock DriverName;
        
        #line default
        #line hidden
        
        
        #line 91 "..\..\..\Views\DriverDetailInfoPopup.xaml"
        internal System.Windows.Controls.TextBlock tbCurrentDriverDate;
        
        #line default
        #line hidden
        
        
        #line 94 "..\..\..\Views\DriverDetailInfoPopup.xaml"
        internal System.Windows.Controls.TextBlock DetailCurrentDriverDate;
        
        #line default
        #line hidden
        
        
        #line 97 "..\..\..\Views\DriverDetailInfoPopup.xaml"
        internal System.Windows.Controls.TextBlock tbNewDriverDate;
        
        #line default
        #line hidden
        
        
        #line 100 "..\..\..\Views\DriverDetailInfoPopup.xaml"
        internal System.Windows.Controls.TextBlock DetailNewDriverDate;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/FreeDriverScout;component/views/driverdetailinfopopup.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Views\DriverDetailInfoPopup.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.driverDetailInfo = ((FreemiumUtilites.DriverDetailInfoPopup)(target));
            return;
            case 2:
            
            #line 55 "..\..\..\Views\DriverDetailInfoPopup.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Close);
            
            #line default
            #line hidden
            return;
            case 3:
            this.Inner = ((System.Windows.Controls.Border)(target));
            return;
            case 4:
            this.tbName = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 5:
            this.DriverName = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 6:
            this.tbCurrentDriverDate = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 7:
            this.DetailCurrentDriverDate = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 8:
            this.tbNewDriverDate = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 9:
            this.DetailNewDriverDate = ((System.Windows.Controls.TextBlock)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

