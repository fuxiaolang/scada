﻿#pragma checksum "../../../../../View/Inquiry/Veh.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "554FED521708B87B68A9C471F9DE18C4E5AC5AB2"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using DESCADA;
using DESCADA.Controls;
using MahApps.Metro.IconPacks;
using MahApps.Metro.IconPacks.Converter;
using MetroDemo.Models;
using Microsoft.Xaml.Behaviors;
using Microsoft.Xaml.Behaviors.Core;
using Microsoft.Xaml.Behaviors.Input;
using Microsoft.Xaml.Behaviors.Layout;
using Microsoft.Xaml.Behaviors.Media;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
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


namespace DESCADA {
    
    
    /// <summary>
    /// Veh
    /// </summary>
    public partial class Veh : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector {
        
        
        #line 36 "../../../../../View/Inquiry/Veh.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox InqType;
        
        #line default
        #line hidden
        
        
        #line 40 "../../../../../View/Inquiry/Veh.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtInq;
        
        #line default
        #line hidden
        
        
        #line 52 "../../../../../View/Inquiry/Veh.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnInquiry;
        
        #line default
        #line hidden
        
        
        #line 77 "../../../../../View/Inquiry/Veh.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnAddVeh;
        
        #line default
        #line hidden
        
        
        #line 93 "../../../../../View/Inquiry/Veh.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid MyDataGrid;
        
        #line default
        #line hidden
        
        
        #line 154 "../../../../../View/Inquiry/Veh.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal DESCADA.Controls.Pagination ctlPage;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "7.0.20.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/DESCADA;component/view/inquiry/veh.xaml", System.UriKind.Relative);
            
            #line 1 "../../../../../View/Inquiry/Veh.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "7.0.20.0")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "7.0.20.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.InqType = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 2:
            this.txtInq = ((System.Windows.Controls.TextBox)(target));
            return;
            case 3:
            this.BtnInquiry = ((System.Windows.Controls.Button)(target));
            
            #line 52 "../../../../../View/Inquiry/Veh.xaml"
            this.BtnInquiry.Click += new System.Windows.RoutedEventHandler(this.BtnInquiry_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.btnAddVeh = ((System.Windows.Controls.Button)(target));
            
            #line 77 "../../../../../View/Inquiry/Veh.xaml"
            this.btnAddVeh.Click += new System.Windows.RoutedEventHandler(this.btnAddVeh_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            
            #line 89 "../../../../../View/Inquiry/Veh.xaml"
            ((System.Windows.Controls.Image)(target)).MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.export_MouseDown);
            
            #line default
            #line hidden
            return;
            case 6:
            this.MyDataGrid = ((System.Windows.Controls.DataGrid)(target));
            
            #line 96 "../../../../../View/Inquiry/Veh.xaml"
            this.MyDataGrid.LoadingRow += new System.EventHandler<System.Windows.Controls.DataGridRowEventArgs>(this.MyDataGrid_LoadingRow);
            
            #line default
            #line hidden
            return;
            case 9:
            this.ctlPage = ((DESCADA.Controls.Pagination)(target));
            return;
            }
            this._contentLoaded = true;
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "7.0.20.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        void System.Windows.Markup.IStyleConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 7:
            
            #line 145 "../../../../../View/Inquiry/Veh.xaml"
            ((System.Windows.Controls.TextBlock)(target)).MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.btnVehEidt_MouseDown);
            
            #line default
            #line hidden
            break;
            case 8:
            
            #line 146 "../../../../../View/Inquiry/Veh.xaml"
            ((System.Windows.Controls.TextBlock)(target)).MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.btnVehDel_MouseDown);
            
            #line default
            #line hidden
            break;
            }
        }
    }
}

