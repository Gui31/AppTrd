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
    public class LimitSelectorControl : Button
    {
        public static readonly DependencyProperty IsPopupOpenProperty = DependencyProperty.Register(
            "IsPopupOpen", typeof(bool), typeof(LimitSelectorControl), new PropertyMetadata(default(bool)));

        public bool IsPopupOpen
        {
            get { return (bool)GetValue(IsPopupOpenProperty); }
            set { SetValue(IsPopupOpenProperty, value); }
        }

        public static readonly DependencyProperty LimitDisplayProperty = DependencyProperty.Register(
            "LimitDisplay", typeof(string), typeof(LimitSelectorControl), new PropertyMetadata(default(string)));

        public string LimitDisplay
        {
            get { return (string)GetValue(LimitDisplayProperty); }
            set { SetValue(LimitDisplayProperty, value); }
        }

        public static readonly DependencyProperty HasLimitProperty = DependencyProperty.Register(
            "HasLimit", typeof(bool), typeof(LimitSelectorControl), new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (o, args) => ((LimitSelectorControl)o).UpdateDisplay()));

        public bool HasLimit
        {
            get { return (bool)GetValue(HasLimitProperty); }
            set { SetValue(HasLimitProperty, value); }
        }

        public static readonly DependencyProperty LimitDistanceProperty = DependencyProperty.Register(
            "LimitDistance", typeof(int), typeof(LimitSelectorControl), new FrameworkPropertyMetadata(default(int), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (o, args) => ((LimitSelectorControl)o).UpdateDisplay()));

        public int LimitDistance
        {
            get { return (int)GetValue(LimitDistanceProperty); }
            set { SetValue(LimitDistanceProperty, value); }
        }

        static LimitSelectorControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LimitSelectorControl), new FrameworkPropertyMetadata(typeof(LimitSelectorControl)));
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
            var display = string.Empty;

            if (HasLimit)
            {
                display += LimitDistance;
            }

            LimitDisplay = display;
        }
    }
}
