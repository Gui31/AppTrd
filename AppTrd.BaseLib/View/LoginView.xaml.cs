using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AppTrd.BaseLib.ViewModel;

namespace AppTrd.BaseLib.View
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : BaseView
    {
        public LoginView()
        {
            InitializeComponent();

            UsernameTextBox.KeyDown += UsernameTextBoxKeyDown;
            ApiKeyTextBox.KeyDown += ApiKeyTextBoxKeyDown;
            LoginPasswordBox.KeyDown += LoginPasswordBoxKeyDown;
            AccountComboBox.IsEnabledChanged += AccountComboBoxIsEnabledChanged;
            AccountComboBox.KeyDown += AccountComboBoxKeyDown;
        }

        private void UsernameTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                ApiKeyTextBox.Focus();
        }

        private void ApiKeyTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                LoginPasswordBox.Focus();
        }

        private void LoginPasswordBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                LoginButtonClick(null, null);
        }

        private void AccountComboBoxIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (AccountComboBox.IsEnabled)
                AccountComboBox.Focus();
        }

        private void AccountComboBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var vm = DataContext as LoginViewModel;

                if (vm != null)
                    vm.ConnectCommand.Execute(null);
            }
        }

        protected override void OnInit()
        {
            if (string.IsNullOrEmpty(UsernameTextBox.Text))
            {
                UsernameTextBox.Focus();
                return;
            }

            if (string.IsNullOrEmpty(ApiKeyTextBox.Text))
            {
                ApiKeyTextBox.Focus();
                return;
            }

            LoginPasswordBox.Focus();
        }

        private void LoginButtonClick(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as LoginViewModel;

            if (vm != null)
                vm.LoginCommand.Execute(LoginPasswordBox.Password);

            LoginPasswordBox.Password = null;
        }
    }
}
