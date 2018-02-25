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
    /// <summary>
    /// Interaction logic for KeySelector.xaml
    /// </summary>
    public partial class KeySelector : UserControl
    {
        public static readonly DependencyProperty SelectedKeyProperty = DependencyProperty.Register(
            "SelectedKey", typeof(Key?), typeof(KeySelector), new FrameworkPropertyMetadata(default(Key?), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (o, args) => ((KeySelector)o).SelectedKeyChanged()));

        private void SelectedKeyChanged()
        {
            Update();
        }

        public Key? SelectedKey
        {
            get { return (Key?)GetValue(SelectedKeyProperty); }
            set { SetValue(SelectedKeyProperty, value); }
        }

        public KeySelector()
        {
            InitializeComponent();

            SelectButton.Click += SelectButtonClick;
            ClearButton.Click += ClearButtonClick;
            Update();
        }

        private void SelectButtonClick(object sender, RoutedEventArgs e)
        {
            if (SelectButton.IsChecked.Value)
            {
                SelectButton.Content = "Press any key";
                Focus();
            }
            else
            {
                Update();
            }
        }

        private void ClearButtonClick(object sender, RoutedEventArgs e)
        {
            if (SelectButton.IsChecked.Value)
            {
                SelectButton.IsChecked = false;
                Update();
                return;
            }

            SelectedKey = null;
            Update();
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            if (SelectButton.IsChecked.Value)
            {
                SelectButton.IsChecked = false;
                Update();
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (SelectButton.IsChecked == false)
                return;

            if (IsKeyValid(e.Key) == false)
                return;

            SelectButton.IsChecked = false;
            SelectedKey = e.Key;
            Update();
        }

        private void Update()
        {
            if (SelectedKey.HasValue)
            {
                SelectButton.Content = SelectedKey.ToString();
                ClearButton.Visibility = Visibility.Visible;
            }
            else
            {
                SelectButton.Content = "Unset";
                ClearButton.Visibility = Visibility.Collapsed;
            }
        }

        private bool IsKeyValid(Key key)
        {
            if (key == Key.Escape)
                return true;

            if (key >= Key.Left && key <= Key.Down)
                return true;

            if (key >= Key.D0 && key <= Key.Z)
                return true;

            if (key >= Key.NumPad0 && key <= Key.F24)
                return true;

            return false;
        }
    }
}
