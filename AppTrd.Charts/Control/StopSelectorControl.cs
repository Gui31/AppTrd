using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AppTrd.Charts.Control
{
    public class StopSelectorControl : Button
    {
        public static readonly DependencyProperty IsPopupOpenProperty = DependencyProperty.Register(
            "IsPopupOpen", typeof(bool), typeof(StopSelectorControl), new PropertyMetadata(default(bool)));

        public bool IsPopupOpen
        {
            get { return (bool)GetValue(IsPopupOpenProperty); }
            set { SetValue(IsPopupOpenProperty, value); }
        }

        public static readonly DependencyProperty StopDisplayProperty = DependencyProperty.Register(
            "StopDisplay", typeof(string), typeof(StopSelectorControl), new PropertyMetadata(default(string)));

        public string StopDisplay
        {
            get { return (string)GetValue(StopDisplayProperty); }
            set { SetValue(StopDisplayProperty, value); }
        }

        public static readonly DependencyProperty HasStopProperty = DependencyProperty.Register(
            "HasStop", typeof(bool), typeof(StopSelectorControl), new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (o, args) => ((StopSelectorControl)o).UpdateDisplay()));

        public bool HasStop
        {
            get { return (bool)GetValue(HasStopProperty); }
            set { SetValue(HasStopProperty, value); }
        }

        public static readonly DependencyProperty GarantedStopProperty = DependencyProperty.Register(
            "GarantedStop", typeof(bool), typeof(StopSelectorControl), new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (o, args) => ((StopSelectorControl)o).UpdateDisplay()));

        public bool GarantedStop
        {
            get { return (bool)GetValue(GarantedStopProperty); }
            set { SetValue(GarantedStopProperty, value); }
        }

        public static readonly DependencyProperty StopDistanceProperty = DependencyProperty.Register(
            "StopDistance", typeof(int), typeof(StopSelectorControl), new FrameworkPropertyMetadata(default(int), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (o, args) => ((StopSelectorControl)o).UpdateDisplay()));

        public int StopDistance
        {
            get { return (int)GetValue(StopDistanceProperty); }
            set { SetValue(StopDistanceProperty, value); }
        }

        static StopSelectorControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(StopSelectorControl), new FrameworkPropertyMetadata(typeof(StopSelectorControl)));
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

            if (HasStop)
            {
                if (GarantedStop)
                    display += "G";

                display += StopDistance;
            }

            StopDisplay = display;
        }
    }
}
