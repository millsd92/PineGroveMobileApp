using System;
using System.Threading;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Acr.UserDialogs;
using PineGroveMobileApp.Services;

namespace PineGroveMobileApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        private double width = 0, height = 0;
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
                ToastConfig config = new ToastConfig("Attempting to log in...")
                {
                    BackgroundColor = App.toastColor,
                    Duration = TimeSpan.FromMilliseconds(App.timeoutTime)
                };
                UserDialogs.Instance.Toast(config);
                try
                {
                    CancellationTokenSource tokenSource = new CancellationTokenSource();
                    tokenSource.CancelAfter((int)App.timeoutTime);
                    var user = await client.GetUser(txtUsername.Text, tokenSource.Token);
                    config.Message = "Successfully logged in as " + user.FirstName + " " + user.LastName + "!";
                    config.Duration = TimeSpan.FromSeconds(2);
                    UserDialogs.Instance.Toast(config);
                    Application.Current.Properties["Username"] = user.UserName;
                    await Application.Current.SavePropertiesAsync();
                    Application.Current.MainPage = new MainPage(ref client);
                }
                catch (Refit.ValidationApiException)
                {
                    UserDialogs.Instance.Toast("Username not found!");
                }
                catch (Refit.ApiException)
                {
                    UserDialogs.Instance.Toast("Username not found!");
                }
                catch (System.Net.Http.HttpRequestException)
                {
                    Timer_Elapsed("Access denied! Connection blocked.\nWould you like to browse offline?");
                }
                catch (System.Threading.Tasks.TaskCanceledException)
                {
                    Timer_Elapsed("Request timeout...\nWould you like to browse offline?");
                }
            }
            else
                UserDialogs.Instance.Toast(new ToastConfig("Error! No text was entered for the username!")
                {
                    BackgroundColor = App.toastColor
                });
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            if (this.width != width || this.height != height)
            {
                this.width = width;
                this.height = height;
                if (width > height)
                {
                    LandscapeOrientation();
                }
                else
                {
                    PortraitOrientation();
                }
            }
            base.OnSizeAllocated(width, height);
        }

        private void Timer_Elapsed(string message)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (await DisplayAlert("Error!", message, "Yes", "Try Again"))
                    Application.Current.MainPage = new MainPage(ref client);
            });
        }

        private async void BtnLookup_Clicked(object sender, EventArgs e) { await Navigation.PushModalAsync(new LookupPage(ref client)); }

        private void LandscapeOrientation()
        {
            imgLogo.HeightRequest = 50;
            lblLookupPrompt.Text = "Press 'Find Me' for account lookup or 'Register' to create an account.";
            grdAll.Padding = 5;
            grdAll.Children.Clear();
            grdAll.Children.Add(lblLoginTitle, 0, 0);
            grdAll.Children.Add(lblLoginPrompt, 0, 1);
            grdAll.Children.Add(lblLookupTitle, 1, 0);
            grdAll.Children.Add(lblLookupPrompt, 1, 1);
            grdAll.Children.Add(txtUsername, 0, 2);
            grdAll.Children.Add(btnLogin, 0, 3);
            grdAll.Children.Add(btnLookup, 1, 2);
            grdAll.Children.Add(btnRegister, 1, 3);
            grdAll.Children.Add(btnSkip, 0, 4);
            Grid.SetColumnSpan(btnSkip, 2);
        }

        private void PortraitOrientation()
        {
            imgLogo.HeightRequest = 75;
            lblLookupPrompt.Text = "Forgot your username? Press the button below.";
            grdAll.Padding = 25;
            grdAll.Children.Clear();
            grdAll.Children.Add(lblLoginTitle, 0, 0);
            grdAll.Children.Add(lblLoginPrompt, 0, 1);
            grdAll.Children.Add(txtUsername, 0, 2);
            grdAll.Children.Add(btnLogin, 0, 3);
            grdAll.Children.Add(lblLookupTitle, 0, 4);
            grdAll.Children.Add(lblLookupPrompt, 0, 5);
            grdAll.Children.Add(btnLookup, 0, 6);
            grdAll.Children.Add(lblRegister, 0, 7);
            grdAll.Children.Add(btnRegister, 0, 8);
            grdAll.Children.Add(btnSkip, 0, 9);
        }

        private void BtnSkip_Clicked(object sender, EventArgs e) { Application.Current.MainPage = new MainPage(ref client); }

        private void BtnRegister_Clicked(object sender, EventArgs e)
        {

        }
    }
}