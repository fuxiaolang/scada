﻿#pragma checksum "../../../../../View/Inquiry/UserEdit.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "7512DA2A60A8A506502432D1DD326FFE7D6C5CE9"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using DESCADA;
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
    /// UserEdit
    /// </summary>
    public partial class UserEdit : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 28 "../../../../../View/Inquiry/UserEdit.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock txtTitle;
        
        #line default
        #line hidden
        
        
        #line 35 "../../../../../View/Inquiry/UserEdit.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtAccount;
        
        #line default
        #line hidden
        
        
        #line 46 "../../../../../View/Inquiry/UserEdit.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtName;
        
        #line default
        #line hidden
        
        
        #line 56 "../../../../../View/Inquiry/UserEdit.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox cmbRole;
        
        #line default
        #line hidden
        
        
        #line 65 "../../../../../View/Inquiry/UserEdit.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnPass;
        
        #line default
        #line hidden
        
        
        #line 70 "../../../../../View/Inquiry/UserEdit.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnOK;
        
        #line default
        #line hidden
        
        
        #line 76 "../../../../../View/Inquiry/UserEdit.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button bnt1;
        
        #line default
        #line hidden
        
        
        #line 81 "../../../../../View/Inquiry/UserEdit.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnEnable;
        
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
            System.Uri resourceLocater = new System.Uri("/DESCADA;component/view/inquiry/useredit.xaml", System.UriKind.Relative);
            
            #line 1 "../../../../../View/Inquiry/UserEdit.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
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
            
            #line 9 "../../../../../View/Inquiry/UserEdit.xaml"
            ((DESCADA.UserEdit)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.txtTitle = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 3:
            
            #line 29 "../../../../../View/Inquiry/UserEdit.xaml"
            ((System.Windows.Controls.Image)(target)).MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.Close_MouseDown);
            
            #line default
            #line hidden
            return;
            case 4:
            this.txtAccount = ((System.Windows.Controls.TextBox)(target));
            return;
            case 5:
            this.txtName = ((System.Windows.Controls.TextBox)(target));
            return;
            case 6:
            this.cmbRole = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 7:
            this.btnPass = ((System.Windows.Controls.Button)(target));
            
            #line 65 "../../../../../View/Inquiry/UserEdit.xaml"
            this.btnPass.Click += new System.Windows.RoutedEventHandler(this.btnPass_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.btnOK = ((System.Windows.Controls.Button)(target));
            
            #line 70 "../../../../../View/Inquiry/UserEdit.xaml"
            this.btnOK.Click += new System.Windows.RoutedEventHandler(this.btnOK_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            this.bnt1 = ((System.Windows.Controls.Button)(target));
            
            #line 76 "../../../../../View/Inquiry/UserEdit.xaml"
            this.bnt1.Click += new System.Windows.RoutedEventHandler(this.btnCancel_Click);
            
            #line default
            #line hidden
            return;
            case 10:
            this.btnEnable = ((System.Windows.Controls.Button)(target));
            
            #line 81 "../../../../../View/Inquiry/UserEdit.xaml"
            this.btnEnable.Click += new System.Windows.RoutedEventHandler(this.btnEnable_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

