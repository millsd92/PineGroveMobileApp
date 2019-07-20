using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Acr.UserDialogs;
using PineGroveMobileApp.Services;

namespace PineGroveMobileApp
{
    // This is, generally speaking, the first page the user will see - the login page.
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        private double width = 0, height = 0;   // Used to determine the orientation.
        private RestClient client;  // API Access

        /// <summary>
        /// This is the page where the user will login. It also provides access to the lookup page and registration page.
        /// </summary>
        /// <param name="client">The REST Client.</param>
        public LoginPage(ref RestClient client)
        {
            InitializeComponent();
            this.client = client;   // Uses the preexisting RestClient object.
        }

        /// <summary>
        /// This is the event handler for the login button being pressed.
        /// </summary>
        /// <param name="sender">The object that causes the event.</param>
        /// <param name="e">Event arguments.</param>
        private async void BtnLogin_Clicked(object sender, EventArgs e)
        {
            // First, lets make sure that there is some text in the username textbox.
            if (txtUsername.Text != null && txtUsername.Text.Length > 0)
            {
                // The first thing we want to do is let the user know we are working on logging them in.
                ToastConfig config = new ToastConfig("Attempting to log in...")
                {
                    BackgroundColor = App.toastColor,
                    Duration = TimeSpan.FromMilliseconds(App.timeoutTime)
                };
                UserDialogs.Instance.Toast(config);
                // Now we attempt to log them in.
                try
                {
                    var user = await client.GetUser(txtUsername.Text);       // Call the API and try to get the user.
                    config.Message = "Successfully logged in as " + user.FirstName + " " + user.LastName + "!"; // Success!
                    config.Duration = TimeSpan.FromSeconds(2);                                  // Reset the length of time the toast message is displayed.
                    UserDialogs.Instance.Toast(config);                                         // Show the toast message.
                    Application.Current.Properties["Username"] = user.UserName;                 // Update the apps local storage.
                    await Application.Current.SavePropertiesAsync();                            // Save changes.
                    Application.Current.MainPage = new MainPage(ref client);                    // Show the main page.
                }
                // There are four main errors that could occur when calling the API.
                catch (Refit.ValidationApiException)            // The username was in an unreadable format.
                {
                    UserDialogs.Instance.Toast("Username not found!");
                }
                catch (Refit.ApiException)                      // There was no user by the username given.
                {
                    UserDialogs.Instance.Toast("Username not found!");
                }
                catch (System.Net.Http.HttpRequestException)    // The connection was lost.
                {
                    Timer_Elapsed("Access denied! Connection blocked.\nWould you like to browse offline?");
                }
                catch (System.Threading.Tasks.TaskCanceledException)    // The task timed out.
                {
                    Timer_Elapsed("Request timeout...\nWould you like to browse offline?");
                }
            }
            // If there isn't, display a small message on the bottom of the screen.
            else
                UserDialogs.Instance.Toast(new ToastConfig("Error! No text was entered for the username!")
                {
                    BackgroundColor = App.toastColor
                });
        }

        /// <summary>
        /// This is called when the size of the screen is changed.
        /// </summary>
        /// <param name="width">The new width of the screen.</param>
        /// <param name="height">The new height of the screen.</param>
        protected override void OnSizeAllocated(double width, double height)
        {
            // If the size changed at all, come here.
            if (this.width != width || this.height != height)
            {
                // Set the width and the height to the new values.
                this.width = width;
                this.height = height;
                // Depending on whether or not the width is greater than the height, lets move some stuff around.
                if (width > height)
                    LandscapeOrientation();
                else
                    PortraitOrientation();
            }
            // You have to call this in order for the program to work correctly.
            base.OnSizeAllocated(width, height);
        }

        /// <summary>
        /// This event is called upon a request timeout.
        /// </summary>
        /// <param name="message">The message to be displayed upon timeout.</param>
        private void Timer_Elapsed(string message)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                // If the user wants to browse offline (AKA there is a lapse in internet coverage), skip to the main page.
                if (await DisplayAlert("Error!", message, "Yes", "Try Again"))
                    Application.Current.MainPage = new MainPage(ref client);
            });
        }

        /// <summary>
        /// This is called upon the pressing of the lookup button.
        /// </summary>
        /// <param name="sender">The object that causes the event.</param>
        /// <param name="e">Event arguments.</param>
        private async void BtnLookup_Clicked(object sender, EventArgs e) { await Navigation.PushModalAsync(new LookupPage(ref client)); }

        /// <summary>
        /// Changes the grid to suit a landscape screen orientation.
        /// </summary>
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

        /// <summary>
        /// Changes the grid to suit a portrait screen orientation.
        /// </summary>
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

        /// <summary>
        /// This is called upon the pressing of the skip login button.
        /// </summary>
        /// <param name="sender">The object that causes the event.</param>
        /// <param name="e">Event arguments.</param>
        private void BtnSkip_Clicked(object sender, EventArgs e) { Application.Current.MainPage = new MainPage(ref client); }

        /// <summary>
        /// This is called upon the pressing of the register button.
        /// </summary>
        /// <param name="sender">The object that causes the event.</param>
        /// <param name="e">Event arguments.</param>
        private async void BtnRegister_Clicked(object sender, EventArgs e)
        { await Navigation.PushModalAsync(new RegistrationPage(ref client)); }
    }
}