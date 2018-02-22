using System.Windows;
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
