using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Acr.UserDialogs;
using PineGroveMobileApp.Services;
using System.Timers;

namespace PineGroveMobileApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        private RestClient client;
        public LoginPage(ref RestClient client)
        {
            InitializeComponent();
            this.client = client;
        }

        private async void BtnLogin_Clicked(object sender, EventArgs e)
        {
            if (txtUsername.Text != null && txtUsername.Text.Length > 0)
            {
                Timer timer = new Timer(10000);
                ToastConfig config = new ToastConfig("Attempting to log in...")
                {
                    BackgroundColor = System.Drawing.Color.FromArgb(51, 51, 51),
                    Duration = TimeSpan.FromSeconds(10)
                };
                UserDialogs.Instance.Toast(config);
                try
                {
                    timer.Elapsed += Timer_Elapsed;
                    timer.AutoReset = false;
                    timer.Enabled = true;
                    timer.Start();
                    var user = await client.GetUser(txtUsername.Text);
                    config.Message = "Successfully logged in as " + user.FirstName + " " + user.LastName + "!";
                    UserDialogs.Instance.Toast(config);
                    Application.Current.Properties["Username"] = user.UserName;
                    await Application.Current.SavePropertiesAsync();
                    timer.Stop();
                    timer.Dispose();
                    Application.Current.MainPage = new MainPage(ref client);
                }
                catch (Refit.ValidationApiException)
                {
                    timer.Stop();
                    timer.Dispose();
                    UserDialogs.Instance.Toast("Username not found!");
                }
                catch (System.Net.Http.HttpRequestException)
                {
                    timer.Stop();
                    timer.Dispose();
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        if (await DisplayAlert("Error!", "Access denied! Connection blocked.\nWould you like to browse offline?", "Yes", "Try Again"))
                            Application.Current.MainPage = new MainPage(ref client);
                    });
                }
            }
            else
                UserDialogs.Instance.Toast(new ToastConfig("Error! No text was entered for the username!")
                {
                    BackgroundColor = System.Drawing.Color.FromArgb(51, 51, 51)
                });
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (await DisplayAlert("Error!", "Request timeout...\nWould you like to browse offline?", "Yes", "Try Again"))
                    Application.Current.MainPage = new MainPage(ref client);
            });
        }

        private void BtnLookup_Clicked(object sender, EventArgs e)
        {

        }
    }
}