using SaleManagerApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaleManagerApp.Navigation
{
    public static class NavigationService
    {
        public static Action<BaseViewModel> NavigateAction;

        public static void Navigate(BaseViewModel viewModel)
        {
            NavigateAction?.Invoke(viewModel);
        }
    }

}
