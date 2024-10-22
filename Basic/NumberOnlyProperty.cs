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
    public static class NumberOnlyProperty
    {
        public static readonly DependencyProperty IsNumberOnlyProperty =
            DependencyProperty.RegisterAttached(
            "IsNumberOnly",
            typeof(bool),
            typeof(NumberOnlyProperty),
            new PropertyMetadata(false, OnIsNumberOnlyPropertyChanged));

        public static void SetIsNumberOnly(DependencyObject obj, bool value)
        {
            obj.SetValue(IsNumberOnlyProperty, value);
        }

        public static bool GetIsNumberOnly(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsNumberOnlyProperty);
        }

        private static void OnIsNumberOnlyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
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
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
