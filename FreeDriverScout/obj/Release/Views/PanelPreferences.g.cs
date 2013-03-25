﻿#pragma checksum "..\..\..\Views\PanelPreferences.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "D89D2D6A7047394040161357A87CC2B6"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.468
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using FreeDriverScout.Routine;
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
using WPFLocalizeExtension.Extensions;


namespace FreeDriverScout.Views {
    
    
    /// <summary>
    /// PanelPreferences
    /// </summary>
    public partial class PanelPreferences : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 38 "..\..\..\Views\PanelPreferences.xaml"
        internal System.Windows.Controls.ComboBox LanguagesList;
        
        #line default
        #line hidden
        
        
        #line 51 "..\..\..\Views\PanelPreferences.xaml"
        internal System.Windows.Controls.CheckBox StartUpAction;
        
        #line default
        #line hidden
        
        
        #line 66 "..\..\..\Views\PanelPreferences.xaml"
        internal System.Windows.Controls.CheckBox MinToTray;
        
        #line default
        #line hidden
        
        
        #line 83 "..\..\..\Views\PanelPreferences.xaml"
        internal System.Windows.Controls.TextBox driverDownloadsFolder;
        
        #line default
        #line hidden
        
        
        #line 86 "..\..\..\Views\PanelPreferences.xaml"
        internal System.Windows.Controls.Button selectDriverDownloadsFolder;
        
        #line default
        #line hidden
        
        
        #line 104 "..\..\..\Views\PanelPreferences.xaml"
        internal System.Windows.Controls.TextBox backupsFolder;
        
        #line default
        #line hidden
        
        
        #line 107 "..\..\..\Views\PanelPreferences.xaml"
        internal System.Windows.Controls.Button selectBackupsFolder;
        
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
            System.Uri resourceLocater = new System.Uri("/FreeDriverScout;component/views/panelpreferences.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Views\PanelPreferences.xaml"
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
            this.LanguagesList = ((System.Windows.Controls.ComboBox)(target));
            
            #line 37 "..\..\..\Views\PanelPreferences.xaml"
            this.LanguagesList.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.LanguageChanged);
            
            #line default
            #line hidden
            return;
            case 2:
            this.StartUpAction = ((System.Windows.Controls.CheckBox)(target));
            
            #line 52 "..\..\..\Views\PanelPreferences.xaml"
            this.StartUpAction.Checked += new System.Windows.RoutedEventHandler(this.LaunchAtStartup);
            
            #line default
            #line hidden
            
            #line 53 "..\..\..\Views\PanelPreferences.xaml"
            this.StartUpAction.Unchecked += new System.Windows.RoutedEventHandler(this.DoNotLaunchAtStartup);
            
            #line default
            #line hidden
            return;
            case 3:
            this.MinToTray = ((System.Windows.Controls.CheckBox)(target));
            
            #line 64 "..\..\..\Views\PanelPreferences.xaml"
            this.MinToTray.Checked += new System.Windows.RoutedEventHandler(this.MinToTrayOnClose);
            
            #line default
            #line hidden
            
            #line 65 "..\..\..\Views\PanelPreferences.xaml"
            this.MinToTray.Unchecked += new System.Windows.RoutedEventHandler(this.ShutdownOnClose);
            
            #line default
            #line hidden
            return;
            case 4:
            this.driverDownloadsFolder = ((System.Windows.Controls.TextBox)(target));
            
            #line 84 "..\..\..\Views\PanelPreferences.xaml"
            this.driverDownloadsFolder.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.driverDownloadsFolder_TextChanged);
            
            #line default
            #line hidden
            return;
            case 5:
            this.selectDriverDownloadsFolder = ((System.Windows.Controls.Button)(target));
            
            #line 87 "..\..\..\Views\PanelPreferences.xaml"
            this.selectDriverDownloadsFolder.Click += new System.Windows.RoutedEventHandler(this.selectDriverDownloadsFolder_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.backupsFolder = ((System.Windows.Controls.TextBox)(target));
            
            #line 105 "..\..\..\Views\PanelPreferences.xaml"
            this.backupsFolder.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.backupsFolder_TextChanged);
            
            #line default
            #line hidden
            return;
            case 7:
            this.selectBackupsFolder = ((System.Windows.Controls.Button)(target));
            
            #line 108 "..\..\..\Views\PanelPreferences.xaml"
            this.selectBackupsFolder.Click += new System.Windows.RoutedEventHandler(this.selectBackupsFolder_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            
            #line 133 "..\..\..\Views\PanelPreferences.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.ShowScanPanel);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

