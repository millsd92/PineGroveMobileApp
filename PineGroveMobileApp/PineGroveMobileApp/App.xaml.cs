using System;
using Xamarin.Forms;
using PineGroveMobileApp.Services;

namespace PineGroveMobileApp
{
    public partial class App : Application
    {
        private readonly RestClient client;
        public App()
        {
            InitializeComponent();
            client = new RestClient();
            MainPage = new MainPage(ref client);
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
