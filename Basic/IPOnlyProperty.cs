using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace DESCADA
{
    public static class IPOnlyProperty
    {
        public static readonly DependencyProperty IsIPOnlyProperty =
            DependencyProperty.RegisterAttached(
            "IsIPOnly",
            typeof(bool),
            typeof(IPOnlyProperty),
            new PropertyMetadata(false, OnIsIPOnlyPropertyChanged));

        public static void SetIsIPOnly(DependencyObject obj, bool value)
        {
            obj.SetValue(IsIPOnlyProperty, value);
        }

        public static bool GetIsIPOnly(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsIPOnlyProperty);
        }

        private static void OnIsIPOnlyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool newValue = (bool)e.NewValue;
            if (d is System.Windows.Controls.TextBox textBox)
            {
                if (newValue)
                {
                    textBox.PreviewTextInput += TextBox_PreviewTextInput;
                }
                else
                {
                    textBox.PreviewTextInput -= TextBox_PreviewTextInput;
                }
            }
        }

        private static void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //  Regex regex = new Regex("([1-9]|[1-9]\\d|1\\d{2}|2[0-4]\\d|25[0-5])(.(\\d|[1-9]\\d|1\\d{2}|2[0-4]\\d|25[0-5])){3}");
            //Regex regex = new Regex(@"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$");
            Regex regex = new Regex("^/d+(/./d+)?$");
           // Regex regex = new Regex("[^0-9]+");

            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
