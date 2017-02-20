using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace MyNote
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().BackRequested += MainPage_BackRequested;
            DataContext = AppDeferral.MainPageViewModel;
            base.OnNavigatedTo(e);
        }

        private void MainPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (AppDeferral.MainPageViewModel.SelectedNote != null)
            {
                AppDeferral.MainPageViewModel.SelectedNote = null;
                e.Handled = true;
            }
        }

        private void StackPanel_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if (e.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse)
            {
                ShowMenuFlyout(sender as FrameworkElement);
            }
        }

        private void StackPanel_Holding(object sender, HoldingRoutedEventArgs e)
        {
            if (e.PointerDeviceType != Windows.Devices.Input.PointerDeviceType.Mouse && e.HoldingState == Windows.UI.Input.HoldingState.Started)
            {
                ShowMenuFlyout(sender as FrameworkElement);
            }
        }
        private void ShowMenuFlyout(FrameworkElement element)
        {
            if (element != null)
            {
                AppDeferral.MainPageViewModel.MenuTartgetItem = element.DataContext as Note;
                AppDeferral.MainPageViewModel.PinCommand.CanExecuteChange();
                var menu = FlyoutBase.GetAttachedFlyout(element);
                if(menu != null)
                    menu.ShowAt(element);
            }
        }
    }
}
