using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Acr.UserDialogs;
using System.Threading;

namespace PineGroveMobileApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PrayerRequestPage : ContentPage
    {
        private readonly Services.RestClient client;
        public PrayerRequestPage(ref Services.RestClient client)
        {
            this.client = client;
            InitializeComponent();
            SetUpGrid();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            if (width > height)
            {
                imgLogo.HeightRequest = 50;
                grdAll.Padding = 5;
            }
            else
            {
                imgLogo.HeightRequest = 75;
                grdAll.Padding = 25;
            }
            base.OnSizeAllocated(width, height);
        }

        private void SetUpGrid()
        {
            grdAll.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
            grdAll.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            grdAll.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });

            grdAll.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
            grdAll.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(8, GridUnitType.Star) });
            grdAll.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) });
            grdAll.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) });

            grdAll.Children.Add(lblTitle, 0, 0);
            Grid.SetColumnSpan(lblTitle, 4);
            grdAll.Children.Add(txtDescription, 0, 1);
            Grid.SetColumnSpan(txtDescription, 4);
            grdAll.Children.Add(lblAnonymous, 1, 2);
            grdAll.Children.Add(chkAnonymous, 0, 2);
            grdAll.Children.Add(btnSubmit, 2, 2);
            Grid.SetColumnSpan(btnSubmit, 2);
        }

        private async void Submit_Clicked(object sender, EventArgs e)
        {
            if (txtDescription.Text == null || txtDescription.Text.Equals(string.Empty))
                UserDialogs.Instance.Toast(new ToastConfig("Prayer request is empty!") { BackgroundColor = App.toastColor });
            else if (!chkAnonymous.IsChecked && !Application.Current.Properties.ContainsKey("Username"))
                UserDialogs.Instance.Toast(new ToastConfig("You are not logged in! Please log in or check anonymous request and try again.") { BackgroundColor = App.toastColor });
            else
            {
                try
                {
                    CancellationTokenSource source = new CancellationTokenSource();
                    source.CancelAfter((int)App.timeoutTime);
                    UserDialogs.Instance.Toast(new ToastConfig("Creating prayer request...") { BackgroundColor = App.toastColor, Duration = TimeSpan.FromMilliseconds(App.timeoutTime) });
                    Models.PrayerRequest prayerRequest = new Models.PrayerRequest()
                    {
                        PrayerDate = DateTime.Now,
                        PrayerDescription = txtDescription.Text,
                        UserId = (chkAnonymous.IsChecked ? -1 : (await client.GetUser(Application.Current.Properties["Username"].ToString(), source.Token)).UserId)
                    };
                    await client.CreatePrayerRequest(prayerRequest, source.Token);
                    UserDialogs.Instance.Toast(new ToastConfig("Prayer request posted!") { BackgroundColor = App.toastColor });
                    txtDescription.Text = "";
                    chkAnonymous.IsChecked = false;
                }
                catch (Refit.ValidationApiException)
                {
                    UserDialogs.Instance.Toast(new ToastConfig("Database rejected the post! Try again later.") { BackgroundColor = App.toastColor });
                }
                catch (Refit.ApiException)
                {
                    UserDialogs.Instance.Toast(new ToastConfig("Database rejected the post! Try again later.") { BackgroundColor = App.toastColor });
                }
                catch (System.Net.Http.HttpRequestException)
                {
                    UserDialogs.Instance.Toast(new ToastConfig("Database rejected the post! Try again later.") { BackgroundColor = App.toastColor });
                }
                catch (TaskCanceledException)
                {
                    UserDialogs.Instance.Toast(new ToastConfig("Request timed out... Try again later.") { BackgroundColor = App.toastColor });
                }
            }
        }
    }
}