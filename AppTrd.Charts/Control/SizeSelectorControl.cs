using System;
using System.Collections.Generic;
using System.Linq;
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

namespace AppTrd.Charts.Control
{
    public class SizeSelectorControl : Button
    {
        public static readonly DependencyProperty IsPopupOpenProperty = DependencyProperty.Register(
            "IsPopupOpen", typeof(bool), typeof(SizeSelectorControl), new PropertyMetadata(default(bool)));

        public bool IsPopupOpen
        {
            get { return (bool) GetValue(IsPopupOpenProperty); }
            set { SetValue(IsPopupOpenProperty, value); }
        }

        public static readonly DependencyProperty SizeDisplayProperty = DependencyProperty.Register(
            "SizeDisplay", typeof(string), typeof(SizeSelectorControl), new PropertyMetadata(default(string)));

        public string SizeDisplay
        {
            get { return (string) GetValue(SizeDisplayProperty); }
            set { SetValue(SizeDisplayProperty, value); }
        }

        public static readonly DependencyProperty SizeProperty = DependencyProperty.Register(
            "Size", typeof(double), typeof(SizeSelectorControl), new FrameworkPropertyMetadata(default(double), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (o, args) => ((SizeSelectorControl)o).UpdateDisplay()));

        public double Size
        {
            get { return (double) GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }

        static SizeSelectorControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SizeSelectorControl), new FrameworkPropertyMetadata(typeof(SizeSelectorControl)));
        }

        protected override void OnClick()
        {
            IsPopupOpen = true;

            if (GetTemplateChild("MainInput") is TextBox input)
            {
                input.Focus();
                input.SelectAll();
            }
        }

        private void UpdateDisplay()
        {
            SizeDisplay = Size.ToString();
        }
    }
}
