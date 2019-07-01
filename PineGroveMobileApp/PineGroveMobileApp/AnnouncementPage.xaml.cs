using System;
using System.Threading;
using PineGroveMobileApp.Services;
using PineGroveMobileApp.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Acr.UserDialogs;
using Refit;

namespace PineGroveMobileApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AnnouncementPage : ContentPage
    {
        private readonly RestClient client;
        public AnnouncementPage(ref RestClient client)
        {
            InitializeComponent();
            this.client = client;
            grdAll.Children.Add(txtTitle, 0, 0);
            Grid.SetColumnSpan(txtTitle, 2);
            grdAll.Children.Add(txtDescription, 0, 1);
            Grid.SetColumnSpan(txtDescription, 2);
            grdAll.Children.Add(btnClear, 0, 2);
            grdAll.Children.Add(btnSubmit, 1, 2);
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            if (width > height)
            {
                imgIcon.HeightRequest = 50;
                grdAll.Margin = 5;
            }
            else
            {
                imgIcon.HeightRequest = 75;
                grdAll.Margin = 25;
            }
            base.OnSizeAllocated(width, height);
        }

        private async void BtnSubmit_Clicked(object sender, EventArgs e)
        {
            if (!Application.Current.Properties.ContainsKey("Username"))
                UserDialogs.Instance.Toast(new ToastConfig("Error! You are not logged in! Please log in and try again.") { BackgroundColor = App.toastColor });
            else if (txtTitle.Text == null || txtTitle.Text.Length < 1)
                UserDialogs.Instance.Toast(new ToastConfig("Error! No title!") { BackgroundColor = App.toastColor });
            else if (txtDescription.Text == null || txtDescription.Text.Length < 1)
                UserDialogs.Instance.Toast(new ToastConfig("Error! No description!") { BackgroundColor = App.toastColor });
            else
            {
                try
                {
                    UserDialogs.Instance.Toast(new ToastConfig("Attempting to post announcement request...") { BackgroundColor = App.toastColor });
                    CancellationTokenSource source = new CancellationTokenSource();
                    source.CancelAfter((int)App.timeoutTime);
                    User user = await client.GetUser(Application.Current.Properties["Username"].ToString(), source.Token);
                    AnnouncementRequest announcement = new AnnouncementRequest()
                    {
                        AnnouncementDate = DateTime.Now,
                        UserId = user.UserId,
                        Announcement = txtTitle.Text + " - " + txtDescription.Text,
                    };
                    await client.CreateAnnouncement(announcement, source.Token);
                    txtDescription.Text = "";
                    txtTitle.Text = "";
                    UserDialogs.Instance.Toast(new ToastConfig("Succeeded in posting the announcement request!") { BackgroundColor = App.toastColor });
                }
                catch (ValidationApiException)
                {
                    UserDialogs.Instance.Toast(new ToastConfig("Access denied! Connection blocked. Please try again later."));
                }
                catch (ApiException)
                {
                    UserDialogs.Instance.Toast(new ToastConfig("Error! There was an error posting your announcement.") { BackgroundColor = App.toastColor });
                }
                catch (System.Threading.Tasks.TaskCanceledException)
                {
                    UserDialogs.Instance.Toast(new ToastConfig("Error! Request timed out! Please try again later.") { BackgroundColor = App.toastColor });
                }
            }
        }

        private void BtnClear_Clicked(object sender, EventArgs e)
        {
            txtDescription.Text = "";
            txtTitle.Text = "";
            UserDialogs.Instance.Toast(new ToastConfig("Entries Cleared!") { BackgroundColor = App.toastColor });
        }
    }
}