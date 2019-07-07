using Xamarin.Forms;
using Xamarin.Essentials;
using PineGroveMobileApp.Services;

namespace PineGroveMobileApp
{
    public partial class App : Application
    {
        private readonly RestClient client;
        public static readonly System.Drawing.Color toastColor = System.Drawing.Color.FromArgb(51, 51, 51);
        public static readonly double timeoutTime = 15000;
        public App()
        {
            InitializeComponent();
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                client = new RestClient();
                _ = client.GetUser("none", new System.Threading.CancellationToken());   //Warm up the API to help prevent harsh "cold start" time
                if (Properties.ContainsKey("Username"))
                    MainPage = new MainPage(ref client);
                else
                    MainPage = new LoginPage(ref client);
            }
            else
            {
                //TODO: Handle when the user has no internet
            }
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
