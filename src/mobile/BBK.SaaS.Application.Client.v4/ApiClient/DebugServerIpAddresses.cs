namespace BBK.SaaS.ApiClient
{
    public static class DebugServerIpAddresses
    {
        /*
         * This field is being used for setting IP address for debugging the clients (eg: Xamarin application)
         * It's being set in
         *  - SplashActivity.cs (StartApplication method) in *.Mobile.Droid project,
         *  - AppDelegate.cs (FinishedLaunching method) in *.Mobile.iOS project.
         */
        public static string Current = "192.168.0.248";
        //public static string Current = "10.0.2.2";
    }
}