using System;
using System.ComponentModel;
using Xamarin.Forms;
using Acr.UserDialogs;
using PineGroveMobileApp.Services;

namespace PineGroveMobileApp
{
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        // This was the first page I made so it may be a little different than some of the later ones. I appologize in advance.
        // Class variables.
        private double width = 0, height = 0;
        private RestClient client;

        /// <summary>
        /// This is the main page that provides naviagation to the other forms and pages.
        /// </summary>
        /// <param name="client">The REST Client.</param>
        public MainPage(ref RestClient client)
        {
            InitializeComponent();
            // If the user is not logged in, change the login page's text to reflect that.
            if (!Application.Current.Properties.ContainsKey("Username"))
                lblLogout.Text = "Login / Register";
            this.client = client;
        }

        /// <summary>
        /// This is called when the size of the screen is changed.
        /// </summary>
        /// <param name="width">The new width of the screen.</param>
        /// <param name="height">The new height of the screen.</param>
        protected override void OnSizeAllocated(double width, double height)
        {
            if (this.width != width || this.height != height)
            {
                this.width = width;
                this.height = height;
                if (width > height)
                {
                    // If the orientation is landscape, change the main stack to being 2x2.
                    MainStack.Orientation = StackOrientation.Horizontal;
                    RightBorder.IsVisible = true;
                }
                else
                {
                    // Else, make it 1x4.
                    MainStack.Orientation = StackOrientation.Vertical;
                    RightBorder.IsVisible = false;
                }
            }
            // You have to call this otherwise problems will occur.
            base.OnSizeAllocated(width, height);
        }

        /// <summary>
        /// This is called when the announcement button is pressed.
        /// </summary>
        /// <param name="sender">The object that causes the event.</param>
        /// <param name="e">Event arguments.</param>
        private async void BtnAnnouncement_Clicked(object sender, EventArgs e)
        { await Navigation.PushModalAsync(new AnnouncementPage(ref client)); }

        /// <summary>
        /// This is called when the event button is pressed.
        /// </summary>
        /// <param name="sender">The object that causes the event.</param>
        /// <param name="e">Event arguments.</param>
        private async void BtnEvent_Clicked(object sender, EventArgs e)
        { await Navigation.PushModalAsync(new LoadingPage(ref client)); }

        /// <summary>
        /// This is called when the login/logout button is pressed.
        /// </summary>
        /// <param name="sender">The object that causes the event.</param>
        /// <param name="e">Event arguments.</param>
        private async void BtnRegister_Clicked(object sender, EventArgs e)
        {
            if (lblLogout.Text.Equals("Edit User / Logout"))
            {
                string results = await UserDialogs.Instance.ActionSheetAsync("Choose an option below:", "Cancel", null, null, new string[] { "Edit Current User", "Logout" });
                if (results.Equals("Logout"))
                {
                    if (Application.Current.Properties.Remove("Username"))
                        UserDialogs.Instance.Toast(new ToastConfig("Successfully logged out!") { BackgroundColor = App.toastColor });
                    Application.Current.MainPage = new LoginPage(ref client);
                }
                else if (results.Equals("Edit Current User"))
                {
                    System.Threading.CancellationTokenSource source = new System.Threading.CancellationTokenSource();
                    source.CancelAfter((int)App.timeoutTime);
                    UserDialogs.Instance.Toast(new ToastConfig("Loading user details... Please wait.") { BackgroundColor = App.toastColor, Duration = TimeSpan.FromMilliseconds(App.timeoutTime) });
                    Models.User currentUser = await client.GetUser(Application.Current.Properties["Username"].ToString(), source.Token);
                    UserDialogs.Instance.Toast(new ToastConfig("User details successfully loaded!") { BackgroundColor = App.toastColor });
                    await Navigation.PushModalAsync(new RegistrationPage(ref client, currentUser));
                }
            }
            else
                Application.Current.MainPage = new LoginPage(ref client);
        }

        /// <summary>
        /// This is called when the prayer request/request a visit button is pressed.
        /// </summary>
        /// <param name="sender">The object that causes the event.</param>
        /// <param name="e">Event arguments.</param>
        private async void BtnRequest_Clicked(object sender, EventArgs e)
        {
            string results = await UserDialogs.Instance.ActionSheetAsync("Choose an option below:", "Cancel", null, null, new string[] { "Prayer Request", "Request a Visit" });
            if (results.Equals("Prayer Request"))
                await Navigation.PushModalAsync(new PrayerRequestPage(ref client));
            else if (results.Equals("Request a Visit"))
                await Navigation.PushModalAsync(new VisitRequestPage(ref client));
        }

        /// <summary>
        /// This is called when the announcement button is initially pressed.
        /// </summary>
        /// <param name="sender">The object that causes the event.</param>
        /// <param name="e">Event arguments.</param>
        private void BtnAnnouncement_Pressed(object sender, EventArgs e)
        {
            Device.StartTimer(TimeSpan.FromMilliseconds(1), () => 
            {
                return (BtnEffect(imgAnnouncement, true) && ((Button)sender).IsPressed);
            });
        }

        /// <summary>
        /// This is called when the announcement button is released.
        /// </summary>
        /// <param name="sender">The object that causes the event.</param>
        /// <param name="e">Event arguments.</param>
        private void BtnAnnouncement_Released(object sender, EventArgs e)
        {
            Device.StartTimer(TimeSpan.FromMilliseconds(.5), () =>
            {
                return BtnEffect(imgAnnouncement, false);
            });
        }
        /// <summary>
        /// This is an effect that changes the opacity of an image up until a point.
        /// </summary>
        /// <param name="img">The image who's opacity is to be changed.</param>
        /// <param name="moreOpaque">Determines if the image is to be more opaque or more visible.</param>
        /// <returns>A boolean to either keep the timer going or stop it once the image reaches a certain opacity.</returns>
        private bool BtnEffect(Image img, bool moreOpaque)
        {
            if (moreOpaque)
            {
                if (img.Opacity < .75f)
                {
                    img.Opacity += .05f;
                    return true;
                }
                else
                    return false;
            }
            else
            {
                if (img.Opacity > .25f)
                {
                    img.Opacity -= .05f;
                    return true;
                }
                else
                    return false;
            }
        }

        /// <summary>
        /// This is called when the login/logout button is initially pressed.
        /// </summary>
        /// <param name="sender">The object that causes the event.</param>
        /// <param name="e">Event arguments.</param>
        private void BtnRegister_Pressed(object sender, EventArgs e)
        {
            Device.StartTimer(TimeSpan.FromMilliseconds(1), () =>
            {
                return (BtnEffect(imgRegister, true) && ((Button)sender).IsPressed);
            });
        }

        /// <summary>
        /// This is called when the login/logout button is released.
        /// </summary>
        /// <param name="sender">The object that causes the event.</param>
        /// <param name="e">Event arguments.</param>
        private void BtnRegister_Released(object sender, EventArgs e)
        {
            Device.StartTimer(TimeSpan.FromMilliseconds(.5), () =>
            {
                return BtnEffect(imgRegister, false);
            });
        }

        /// <summary>
        /// This is called when the prayer request/request a visit button is initially pressed.
        /// </summary>
        /// <param name="sender">The object that causes the event.</param>
        /// <param name="e">Event arguments.</param>
        private void BtnRequest_Pressed(object sender, EventArgs e)
        {
            Device.StartTimer(TimeSpan.FromMilliseconds(1), () =>
            {
                return (BtnEffect(imgRequest, true) && ((Button)sender).IsPressed);
            });
        }

        /// <summary>
        /// This is called when the prayer request/request a visit button is released.
        /// </summary>
        /// <param name="sender">The object that causes the event.</param>
        /// <param name="e">Event arguments.</param>
        private void BtnRequest_Released(object sender, EventArgs e)
        {
            Device.StartTimer(TimeSpan.FromMilliseconds(.5), () =>
            {
                return BtnEffect(imgRequest, false);
            });
        }

        /// <summary>
        /// This is called when the event button is initially pressed.
        /// </summary>
        /// <param name="sender">The object that causes the event.</param>
        /// <param name="e">Event arguments.</param>
        private void BtnEvent_Pressed(object sender, EventArgs e)
        {
            Device.StartTimer(TimeSpan.FromMilliseconds(1), () =>
            {
                return (BtnEffect(imgEvent, true) && ((Button)sender).IsPressed);
            });
        }

        /// <summary>
        /// This is called when the event button is released.
        /// </summary>
        /// <param name="sender">The object that causes the event.</param>
        /// <param name="e">Event arguments.</param>
        private void BtnEvent_Released(object sender, EventArgs e)
        {
            Device.StartTimer(TimeSpan.FromMilliseconds(.5), () =>
            {
                return BtnEffect(imgEvent, false);
            });
        }
    }
}
