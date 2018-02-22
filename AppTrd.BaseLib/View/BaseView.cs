using System.Windows.Controls;
using AppTrd.BaseLib.Common;
using AppTrd.BaseLib.ViewModel;

namespace AppTrd.BaseLib.View
{
    public class BaseView : UserControl
    {
        public BaseView()
        {
            Loaded += BaseViewLoaded;
        }

        private void BaseViewLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var vm = DataContext as BaseViewModel;

            if (vm != null)
                vm.Init();
        }
    }
}
