using System;
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
        // This is the REST Client.
        private readonly Services.RestClient client;

        /// <summary>
        /// This is the page where the user can submit prayer requests.
        /// </summary>
        /// <param name="client">The REST Client.</param>
        public PrayerRequestPage(ref Services.RestClient client)
        {
            this.client = client;   // As per usual, this is the original REST Client object.
            InitializeComponent();
            SetUpGrid();            // This sets up the grid programmatically.
        }

        /// <summary>
        /// This is called when the size of the screen is changed.
        /// </summary>
        /// <param name="width">The new width of the screen.</param>
        /// <param name="height">The new height of the screen.</param>
        protected override void OnSizeAllocated(double width, double height)
        {
            // Same small changes to the logo as every other page.
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

        /// <summary>
        /// This sets up the grid for this page.
        /// </summary>
        private void SetUpGrid()
        {
            // I only did it programmatically on this page because it was being odd the very first time doing it in the XAML page...
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

        /// <summary>
        /// This is called when the submit button is pressed.
        /// </summary>
        /// <param name="sender">The object that causes the event.</param>
        /// <param name="e">Event arguments.</param>
        private async void Submit_Clicked(object sender, EventArgs e)
        {
            // First, ensure that the user either logged in or they are submitting the request anonymously.
            if (!chkAnonymous.IsChecked && !Application.Current.Properties.ContainsKey("Username"))
                UserDialogs.Instance.Toast(new ToastConfig("You are not logged in! Please log in or check anonymous request and try again.") { BackgroundColor = App.toastColor });
            // Now, make sure that there is text in the prayer request text area.
            else if (txtDescription.Text == null || txtDescription.Text.Equals(string.Empty))
                UserDialogs.Instance.Toast(new ToastConfig("Prayer request is empty!") { BackgroundColor = App.toastColor });
            // If they passed those simple validations, call here.
            else
            {
                try
                {
                    CancellationTokenSource source = new CancellationTokenSource();
                    source.CancelAfter((int)App.timeoutTime);
                    // Let the user know we are creating the prayer request.
                    UserDialogs.Instance.Toast(new ToastConfig("Creating prayer request...") { BackgroundColor = App.toastColor, Duration = TimeSpan.FromMilliseconds(App.timeoutTime) });
                    // Build the actual prayer request model using the information provided.
                    Models.PrayerRequest prayerRequest = new Models.PrayerRequest()
                    {
                        PrayerDate = DateTime.Now,
                        PrayerDescription = txtDescription.Text,
                        UserId = (chkAnonymous.IsChecked ? -1 : (await client.GetUser(Application.Current.Properties["Username"].ToString(), source.Token)).UserId) // If anonymous is selected, use the -1 user ID (the special anonymous user).
                    };
                    await client.CreatePrayerRequest(prayerRequest, source.Token);  // Actually post the new prayer request.
                    // Let the user know the request was created.
                    UserDialogs.Instance.Toast(new ToastConfig("Prayer request posted!") { BackgroundColor = App.toastColor });
                    // Reset the form.
                    txtDescription.Text = "";
                    chkAnonymous.IsChecked = false;
                }
                catch (Refit.ValidationApiException)    // Bad input.
                {
                    UserDialogs.Instance.Toast(new ToastConfig("Database rejected the post! Try again later.") { BackgroundColor = App.toastColor });
                }
                catch (Refit.ApiException)              // User was deleted.
                {
                    UserDialogs.Instance.Toast(new ToastConfig("Database rejected the post! Try again later.") { BackgroundColor = App.toastColor });
                }
                catch (System.Net.Http.HttpRequestException)    // Lost connection.
                {
                    UserDialogs.Instance.Toast(new ToastConfig("Connection blocked! Try again later.") { BackgroundColor = App.toastColor });
                }
                catch (TaskCanceledException)           // Request timed out.
                {
                    UserDialogs.Instance.Toast(new ToastConfig("Request timed out... Try again later.") { BackgroundColor = App.toastColor });
                }
            }
        }
    }
}