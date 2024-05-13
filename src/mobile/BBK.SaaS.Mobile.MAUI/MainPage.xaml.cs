using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Plugin.LocalNotification;

namespace BBK.SaaS.Mobile.MAUI
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
           

            LocalNotificationCenter.Current.NotificationActionTapped += Current_NotificationActionTapped;
        }

        private void Current_NotificationActionTapped(Plugin.LocalNotification.EventArgs.NotificationActionEventArgs e)
        {
            //throw new NotImplementedException();
            if (e.IsDismissed) { }
            else if (e.IsTapped)
            {

            }
        }

    }
}