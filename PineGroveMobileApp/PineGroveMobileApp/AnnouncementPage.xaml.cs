using System;
using PineGroveMobileApp.Services;
using PineGroveMobileApp.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Acr.UserDialogs;
using Refit;

namespace PineGroveMobileApp
{
    // One of the simpler pages for the application.
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AnnouncementPage : ContentPage
    {
        private readonly RestClient client;

        /// <summary>
        /// This is the page the user will use to submit announcement requests.
        /// </summary>
        /// <param name="client">The REST Client.</param>
        public AnnouncementPage(ref RestClient client)
        {
            InitializeComponent();
            this.client = client;   // Set this to the original client object.
            // I set up the grid here because the grid is so basic.
            grdAll.Children.Add(txtTitle, 0, 0);
            Grid.SetColumnSpan(txtTitle, 2);
            grdAll.Children.Add(txtDescription, 0, 1);
            Grid.SetColumnSpan(txtDescription, 2);
            grdAll.Children.Add(btnClear, 0, 2);
            grdAll.Children.Add(btnSubmit, 1, 2);
        }

        /// <summary>
        /// This is called when the size of the screen is changed.
        /// </summary>
        /// <param name="width">The new width of the screen.</param>
        /// <param name="height">The new height of the screen.</param>
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

        /// <summary>
        /// This is called when the submit button is pressed.
        /// </summary>
        /// <param name="sender">The object that causes the event.</param>
        /// <param name="e">Event arguments.</param>
        private async void BtnSubmit_Clicked(object sender, EventArgs e)
        {
            // First, ensure that the user is logged in.
            if (!Application.Current.Properties.ContainsKey("Username"))
                UserDialogs.Instance.Toast(new ToastConfig("Error! You are not logged in! Please log in and try again.") { BackgroundColor = App.toastColor });
            // Also, ensure that there is information in the title textbox.
            else if (txtTitle.Text == null || txtTitle.Text.Length < 1)
                UserDialogs.Instance.Toast(new ToastConfig("Error! No title!") { BackgroundColor = App.toastColor });
            // Finally, ensure that there is information in the description of the announcement.
            else if (txtDescription.Text == null || txtDescription.Text.Length < 1)
                UserDialogs.Instance.Toast(new ToastConfig("Error! No description!") { BackgroundColor = App.toastColor });
            else
            {
                try
                {
                    // Let the user know that there is something happening.
                    UserDialogs.Instance.Toast(new ToastConfig("Attempting to post announcement request...") { BackgroundColor = App.toastColor, Duration = TimeSpan.FromMilliseconds(App.timeoutTime) });
                    // Get the user so we can get the user ID.
                    User user = await client.GetUser(Application.Current.Properties["Username"].ToString());
                    // Create the announcement model.
                    AnnouncementRequest announcement = new AnnouncementRequest()
                    {
                        AnnouncementDate = DateTime.Now,
                        UserId = user.UserId,
                        Announcement = txtTitle.Text + " - " + txtDescription.Text,
                    };
                    // Post the announcement to the database.
                    await client.CreateAnnouncement(announcement);
                    txtDescription.Text = "";   // Clear the
                    txtTitle.Text = "";         // entry form.
                    // Let the user know it has been submitted!
                    UserDialogs.Instance.Toast(new ToastConfig("Succeeded in posting the announcement request!") { BackgroundColor = App.toastColor });
                }
                catch (ValidationApiException)  // The information is bad.
                {
                    UserDialogs.Instance.Toast(new ToastConfig("Error! The server may be temporarily down. Please try again later.") { BackgroundColor = App.toastColor });
                }
                catch (ApiException)            // The user doesn't exist.
                {
                    UserDialogs.Instance.Toast(new ToastConfig("Error! There was an error posting your announcement.") { BackgroundColor = App.toastColor });
                }
                catch (System.Net.Http.HttpRequestException)    // Connection was lost.
                {
                    UserDialogs.Instance.Toast(new ToastConfig("Access denied! Connection blocked. Please try again later.") { BackgroundColor = App.toastColor });
                }
                catch (System.Threading.Tasks.TaskCanceledException)    // Request timed out.
                {
                    UserDialogs.Instance.Toast(new ToastConfig("Error! Request timed out! Please try again later.") { BackgroundColor = App.toastColor });
                }
            }
        }

        /// <summary>
        /// This is called when the clear button is pressed.
        /// </summary>
        /// <param name="sender">The object that causes the event.</param>
        /// <param name="e">Event arguments.</param>
        private void BtnClear_Clicked(object sender, EventArgs e)
        {
            // It just resets the form.
            txtDescription.Text = "";
            txtTitle.Text = "";
            UserDialogs.Instance.Toast(new ToastConfig("Entries Cleared!") { BackgroundColor = App.toastColor });
        }
    }
}