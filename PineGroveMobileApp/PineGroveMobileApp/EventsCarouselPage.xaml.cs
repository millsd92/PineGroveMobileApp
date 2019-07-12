using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using PineGroveMobileApp.Services;
using System.Globalization;
using System.IO;
using System.Threading;
using Acr.UserDialogs;
using System.Collections.Generic;

namespace PineGroveMobileApp
{
    // This is where the hard work went.
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EventsCarouselPage : CarouselPage
    {
        // Class variables.
        private readonly RestClient client;     // The REST Client (accesses the API).
        public event EventHandler OnLoad;       // An event that is triggered upon fully loading all the events and populating the pages.
        private List<Models.Event> events;      // A list of all of the events.

        /// <summary>
        /// This is the page that shows all of the current events and allows users to register for said events.
        /// </summary>
        /// <param name="client">The REST Client.</param>
        public EventsCarouselPage(ref RestClient client)
        {
            this.client = client;   // Set the REST Client to the original instance of the REST Client.
            InitializeComponent();  // Necessary statement.
            MakePages();            // This makes all of the pages by calling the API and populating the item template with data.
        }

        /// <summary>
        /// This method calls the API to get all of the events and only shows the events that are current.
        /// </summary>
        private async void MakePages()
        {
            events = new List<Models.Event>(await client.GetEvents());  // Calls the API and populates the events list with the results.
            events.RemoveAll(e => e.EndTime < DateTime.Now);            // Removes all of the events that are not current from the results.
            ItemsSource = events;                                       // Sets the source of data for the page to the events list.
            foreach (ContentPage page in Children)                      // Sets up each page programmatically and adds the size changed event handler to each page.
            {
                SetupPage(page, Children.IndexOf(page));                // Sets up the page to make it presentable.
                page.SizeChanged += Page_SizeChanged;                   // Adds the event handler to the page.
            }
            OnLoad(this, new EventArgs());                              // Tells the world this page is ready to be shown!
        }

        /// <summary>
        /// This is called if the orientation of the page changes or the page is resized. It changes some small aspects of the page upon orientation change to make it more presentable.
        /// </summary>
        /// <param name="sender">The object that causes the event.</param>
        /// <param name="e">Event arguments.</param>
        private void Page_SizeChanged(object sender, EventArgs e)
        {
            if ((sender as ContentPage).Width > (sender as ContentPage).Height)
            {
                ((sender as ContentPage).FindByName("imgLogo") as Image).HeightRequest = 50;
                ((sender as ContentPage).FindByName("imgEvent") as Image).HeightRequest = 50;
                ((sender as ContentPage).FindByName("grdAll") as Grid).Margin = 10;
            }
            else
            {
                ((sender as ContentPage).FindByName("imgLogo") as Image).HeightRequest = 75;
                ((sender as ContentPage).FindByName("imgEvent") as Image).HeightRequest = 75;
                ((sender as ContentPage).FindByName("grdAll") as Grid).Margin = 25;
            }
        }

        /// <summary>
        /// This programmatically sets up the page's content.
        /// </summary>
        /// <param name="page">The content page that is to be set up.</param>
        /// <param name="index">The index of the page in the carousel page's page list.</param>
        private void SetupPage(ContentPage page, int index)
        {
            // Okay, so let me explain this mess... If I was better at Xamarin.Forms, I probably could have avoided doing the
            // page setup in this manner. However, I could not access each page's views and controls without doing all of this...
            // Most of this is grid setup that could have occurred in the XAML part of this page, but there were some changes
            // that needed to happen in certain circumstances so I elected to just do it all here instead.

            // I know it looks messy, but it works and I would probably avoid doing it this way now that I know how to use
            // XAML and Xamarin.Forms a bit better then when I initially did this page.

            Grid grid = page.FindByName("grdAll") as Grid;
            Grid volunteer = page.FindByName("grdVolunteer") as Grid;
            Image imgEvent = page.FindByName("imgEvent") as Image;
            Label lblSwipe = page.FindByName("lblSwipe") as Label;
            Label lblTitle = page.FindByName("lblTitle") as Label;
            Label lblDescription = page.FindByName("lblDescription") as Label;
            Label lblDate = page.FindByName("lblDate") as Label;
            Label lblStart = page.FindByName("lblStart") as Label;
            Label lblStartTime = page.FindByName("lblStartTime") as Label;
            lblStartTime.Text = string.Format("{0:g}", Convert.ToDateTime(lblStartTime.Text));
            Label lblEnd = page.FindByName("lblEnd") as Label;
            Label lblEndTime = page.FindByName("lblEndTime") as Label;
                if (lblEndTime.Text == null || lblEnd.Text.Length < 1)      // Some events do not have an 'End Time' so I
                    lblEndTime.Text = "Whenever it ends!";                  // put a check here to avoid errors.
                else
                    lblEndTime.Text = string.Format("{0:g}", Convert.ToDateTime(lblEndTime.Text));
            Label lblAddress = page.FindByName("lblAddress") as Label;
            Label lblAddressInfo = page.FindByName("lblAddressInfo") as Label;
                lblAddressInfo.Text = lblAddressInfo.Text.Replace("\n", "\n\n");
            Button btnRegister = page.FindByName("btnRegister") as Button;
                btnRegister.HorizontalOptions = LayoutOptions.FillAndExpand;
                if (events[index].CurrentAttendees >= events[index].MaxAttendees)   // If the event is full, don't allow people
                {                                                                   // to register for it.
                    btnRegister.IsEnabled = false;
                    btnRegister.Text = "Event full!";
                }
            Label lblGuests = page.FindByName("lblGuests") as Label;
            Stepper stpGuests = page.FindByName("stpGuests") as Stepper;
            if ((events[index].MaxAttendees ?? int.MaxValue) - events[index].CurrentAttendees < 11)     // If there is a maximum amount
            {                                                                                           // of attendees and that number
                if ((events[index].MaxAttendees ?? int.MaxValue) - events[index].CurrentAttendees > 1)  // is lower than 11, they can't
                    stpGuests.Maximum = (double)(events[index].MaxAttendees - events[index].CurrentAttendees - 1);  // bring 10 guests,
                else                                                                                    // which is the default max one
                    stpGuests.Minimum = -1;                                                             // can normally bring. If the
            }                                                                                           // event only has one spot, set
            Label lblMaximum = page.FindByName("lblMaximum") as Label;                                  // the minimum to -1.
                if (stpGuests.Minimum == -1)                                                            // If that minimum is -1, don't
                {                                                                                       // allow the user to bring guests,
                    lblMaximum.Text = "Only one spot left!";                                            // as that would exceed the max
                    lblGuests.Text = "";                                                                // attendees for the event.
                }
                else
                    lblMaximum.Text = "Maximum is " + ((int)stpGuests.Maximum).ToString();              // Otherwise, set the maximum
            Label txtStepper = page.FindByName("txtStepper") as Label;                                  // to default (10), or the max
                txtStepper.Text = "Current value: " + ((int)stpGuests.Minimum).ToString();              // minus current plus one (to
            CheckBox chkVolunteer = page.FindByName("chkVolunteer") as CheckBox;                        // include themself).
            Label lblVolunteer = page.FindByName("lblVolunteer") as Label;

            (page.FindByName("imgLogo") as Image).HeightRequest = 75;
            imgEvent.HeightRequest = 75;

            // Inner grid definitions to make the checkbox look pretty.
            volunteer.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
            volunteer.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            // Add the checkbox and the label to the inner grid.
            volunteer.Children.Add(chkVolunteer, 0, 0);
            volunteer.Children.Add(lblVolunteer, 1, 0);

            // This part would have been unnecessary if it weren't for one row...
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) }); // This one...
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });

            // Set up the grid programmatically. This was a load of fun to do.
            grid.Children.Clear();
            grid.Children.Add(lblSwipe, 0, 0);
            Grid.SetColumnSpan(lblSwipe, 2);
            grid.Children.Add(imgEvent, 0, 1);
            Grid.SetColumnSpan(imgEvent, 2);
            grid.Children.Add(lblTitle, 0, 2);
            Grid.SetColumnSpan(lblTitle, 2);
            grid.Children.Add(lblDescription, 0, 3);
            Grid.SetColumnSpan(lblDescription, 2);
            grid.Children.Add(lblDate, 0, 4);
            grid.Children.Add(lblAddress, 1, 4);
            grid.Children.Add(lblStart, 0, 5);
            grid.Children.Add(lblStartTime, 0, 6);
            grid.Children.Add(lblEnd, 0, 7);
            grid.Children.Add(lblEndTime, 0, 8);
            grid.Children.Add(lblAddressInfo, 1, 5);
            Grid.SetRowSpan(lblAddressInfo, 4);
            grid.Children.Add(lblGuests, 0, 10);
            grid.Children.Add(txtStepper, 1, 10);
            grid.Children.Add(lblMaximum, 0, 11);
            grid.Children.Add(stpGuests, 1, 11);
            grid.Children.Add(volunteer, 0, 12);
            Grid.SetColumnSpan(volunteer, 2);
            grid.Children.Add(btnRegister, 0, 13);
            Grid.SetColumnSpan(btnRegister, 2);
            
        }

        /// <summary>
        /// This is called when the user presses the back button on their device.
        /// </summary>
        /// <returns></returns>
        protected override bool OnBackButtonPressed()
        {
            // Because of the way this page is tied with a load page, when the user
            // presses back the navigation must pop both this page and the loading page.
            Navigation.PopModalAsync();         // Pops the load page.
            return base.OnBackButtonPressed();  // Pops this page.
        }

        /// <summary>
        /// This is called when the register button is pressed.
        /// </summary>
        /// <param name="sender">The object that causes the event.</param>
        /// <param name="e">Event arguments.</param>
        private async void BtnRegister_Clicked(object sender, EventArgs e)
        {
            // The way this button works the user will press it twice. If it is the first time the user pressed the button, fall here.
            if ((sender as Button).Text.Equals("RSVP"))
            {
                // If there is only one spot left, the stepper's minimum value was set to -1.
                int currentValue = (int)(((sender as Button).Parent as Grid).FindByName("stpGuests") as Stepper).Minimum;
                // If it isn't -1, there are available spots. Remember, this button cannot be pressed if the event is already full.
                if (currentValue != -1)
                {
                    (((sender as Button).Parent as Grid).FindByName("lblGuests") as Label).IsVisible = true;
                    (((sender as Button).Parent as Grid).FindByName("stpGuests") as Stepper).IsVisible = true;
                    (((sender as Button).Parent as Grid).FindByName("txtStepper") as Label).IsVisible = true;
                    (((sender as Button).Parent as Grid).FindByName("lblMaximum") as Label).IsVisible = true;
                }
                // If it is -1, there is only one spot left. In this case, don't show the stepper or any guest-bringing-related controls.
                else
                {
                    (((sender as Button).Parent as Grid).FindByName("lblMaximum") as Label).IsVisible = true;
                    Grid.SetColumnSpan(((sender as Button).Parent as Grid).FindByName("lblMaximum") as Label, 2);
                }
                // No matter what, allow them to volunteer.
                (((sender as Button).Parent as Grid).FindByName("chkVolunteer") as CheckBox).IsVisible = true;
                (((sender as Button).Parent as Grid).FindByName("lblVolunteer") as Label).IsVisible = true;
                (sender as Button).Text = "Confirm";    // Change the button's text to confirm now that the form is showing completely.
            }
            // If they are trying to confirm their RSVP, fall here.
            else
            {
                // First, check to make sure they are logged in.
                if (Application.Current.Properties.ContainsKey("Username"))
                {
                    try
                    {
                        (sender as Button).IsEnabled = false;   // First, lets ensure that the user can't spam press the button and make a whole bunch of requests at the same time...
                                                                // Then lets show them a message saying we are working on their registration.
                        UserDialogs.Instance.Toast(new ToastConfig("Attempting to register...") { BackgroundColor = App.toastColor, Duration = TimeSpan.FromMilliseconds(App.timeoutTime) });
                        CancellationTokenSource source = new CancellationTokenSource();
                        source.CancelAfter((int)App.timeoutTime);
                        int index = Children.IndexOf((((((sender as Button).Parent as Grid).Parent as StackLayout).Parent as ScrollView).Parent as StackLayout).Parent as ContentPage); // Fun line just to get the index of this page... Six nested parentheses...
                        // If they are not volunteering, we need to add the stepper's value to the event itself as well.
                        if (!(((sender as Button).Parent as Grid).FindByName("chkVolunteer") as CheckBox).IsChecked)
                        {
                            // Let's make an event registration for the database.
                            await client.CreateRegistration(new Models.EventRegistration()
                            {
                                EventId = events[index].EventId,
                                UserId = (await client.GetUser(Application.Current.Properties["Username"].ToString(), source.Token)).UserId,
                                Guests = (int)(((sender as Button).Parent as Grid).FindByName("stpGuests") as Stepper).Value
                            }, source.Token);
                            // Now let's add the guests to the event...
                            events[index].CurrentAttendees += (int)(((sender as Button).Parent as Grid).FindByName("stpGuests") as Stepper).Value + 1;
                            // ... let the user know we are updating the database for the event now...
                            UserDialogs.Instance.Toast(new ToastConfig("Updating database...") { BackgroundColor = App.toastColor });
                            // ... and update the database for real.
                            await client.UpdateEvent(events[index].EventId, events[index], source.Token);
                        }
                        // Otherwise they are volunteering and it becomes a bit easier.
                        else // We just need to create an event registration for the database with no guests.
                            await client.CreateRegistration(new Models.EventRegistration()
                            {
                                EventId = events[index].EventId,
                                UserId = (await client.GetUser(Application.Current.Properties["Username"].ToString(), source.Token)).UserId,
                                Guests = 0
                            }, source.Token);
                        // Volunteers do not count as attending the event so we do not need to update the event.
                        UserDialogs.Instance.Toast(new ToastConfig("Registration successful!") { BackgroundColor = App.toastColor });
                        await Navigation.PopModalAsync();   // Let's re-bind the events to the page to update
                        MakePages();                        // the controls to reflect the changes made to the database.
                    }
                    catch (Refit.ValidationApiException)        // Invalid data was being given to the database... Probably due to the server going down during the operation.
                    {
                        UserDialogs.Instance.Toast(new ToastConfig("API Validation error! Server may be down... Try again later.") { BackgroundColor = App.toastColor });
                        (sender as Button).IsEnabled = true;
                    }
                    catch (Refit.ApiException)                  // I'm not sure how there would be a different API error unless the admin deleted the user from the database...
                    {
                        UserDialogs.Instance.Toast(new ToastConfig("API error! Server may be down... Try again later.") { BackgroundColor = App.toastColor });
                        (sender as Button).IsEnabled = true;
                    }
                    catch (System.Net.Http.HttpRequestException)// Connection was lost.
                    {
                        UserDialogs.Instance.Toast(new ToastConfig("Connection blocked! (Internet may be down) Try again later.") { BackgroundColor = App.toastColor });
                        (sender as Button).IsEnabled = true;
                    }
                    catch (TaskCanceledException)               // The request timed out.
                    {
                        UserDialogs.Instance.Toast(new ToastConfig("Request timed out. Try again later.") { BackgroundColor = App.toastColor });
                        (sender as Button).IsEnabled = true;
                    }
                }
                // If the user is not logged in, display a message and ignore their registration attempt.
                else
                    UserDialogs.Instance.Toast(new ToastConfig("Error! You must be logged in to register for an event! Log in and try again.") { BackgroundColor = App.toastColor });
            }
        }

        /// <summary>
        /// This is called when the plus or minus button is pressed on the guests stepper.
        /// </summary>
        /// <param name="sender">The object that causes the event.</param>
        /// <param name="e">Event arguments.</param>
        private void StpGuests_ValueChanged(object sender, ValueChangedEventArgs e)
        { (((sender as Stepper).Parent as Grid).FindByName("txtStepper") as Label).Text = "Current value: " + ((int)e.NewValue).ToString(); }

        /// <summary>
        /// This is called when the checkbox denoting volunteering is pressed.
        /// </summary>
        /// <param name="sender">The object that causes the event.</param>
        /// <param name="e">Event arguments.</param>
        private void ChkVolunteer_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            // If the volunteering checkbox is checked, hide the ability to bring guests. This is a personal choice, honestly.
            if (e.Value)
            {
                ((((sender as CheckBox).Parent as Grid).Parent as Grid).FindByName("lblGuests") as Label).IsVisible = false;
                ((((sender as CheckBox).Parent as Grid).Parent as Grid).FindByName("stpGuests") as Stepper).IsVisible = false;
                ((((sender as CheckBox).Parent as Grid).Parent as Grid).FindByName("lblMaximum") as Label).IsVisible = false;
                ((((sender as CheckBox).Parent as Grid).Parent as Grid).FindByName("txtStepper") as Label).IsVisible = false;
            }
            // If the checkbox is now not checked and the event has more than one empty slot, show the controls to allow the user to bring a guest.
            else if (((((sender as CheckBox).Parent as Grid).Parent as Grid).FindByName("stpGuests") as Stepper).Minimum != -1)
            {
                ((((sender as CheckBox).Parent as Grid).Parent as Grid).FindByName("lblGuests") as Label).IsVisible = true;
                ((((sender as CheckBox).Parent as Grid).Parent as Grid).FindByName("stpGuests") as Stepper).IsVisible = true;
                ((((sender as CheckBox).Parent as Grid).Parent as Grid).FindByName("lblMaximum") as Label).IsVisible = true;
                ((((sender as CheckBox).Parent as Grid).Parent as Grid).FindByName("txtStepper") as Label).IsVisible = true;
            }
            // If the checkbox is now not checked and there is only one empty slot, show the label that says there is only one slot left and span it across two columns (to fill the empty space).
            else
            {
                (((sender as Button).Parent as Grid).FindByName("lblMaximum") as Label).IsVisible = true;
                Grid.SetColumnSpan(((sender as Button).Parent as Grid).FindByName("lblMaximum") as Label, 2);
            }
        }
    }

    /// <summary>
    /// This class is used by the XAML form to convert the VARBINARY(MAX) image field in the database to an actual image.
    /// </summary>
    public class ByteToImageConverter : IValueConverter
    {
        /// <summary>
        /// Converts the incoming object into an image.
        /// </summary>
        /// <param name="value">The value to be converted.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">Any additional parameters.</param>
        /// <param name="culture">Additional information to aid in conversion.</param>
        /// <returns>The converted object.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ImageSource retSource = ImageSource.FromFile("PineGroveLogo.png");  // If there is no picture (as it could be null), use this placeholder image.
            if (value != null)                                                  // If there is a value here, fall here.
            {
                byte[] imageAsBytes = (byte[])value;                            // If there is a value, it is VARBINARY(MAX), which is, in C#, a byte array.
                retSource = ImageSource.FromStream(() => new MemoryStream(imageAsBytes));   // Use the built-in ImageSource.FromStream method to get a stream from the byte array (via a MemoryStream object).
            }
            return retSource;
        }

        /// <summary>
        /// In order to implement the IValueConverter interface (which is necessary to be used by the XAML convert parameter), this has to be implemented. However, in this context, it is unneccessary and, therefore, not implemented.
        /// </summary>
        /// <param name="value">The value to be converted.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">Any additional parameters.</param>
        /// <param name="culture">Additional information to aid in conversion.</param>
        /// <returns>The converted object.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        { throw new NotImplementedException(); }
    }

    /// <summary>
    /// This is the loading page that fires before the events carousel page (as that particular page has slightly longer load times than any other page).
    /// </summary>
    public partial class LoadingPage : ContentPage
    {
        // Class variables.
        private EventsCarouselPage events;
        private event EventHandler AfterAnimation;
        private readonly Image logo;

        /// <summary>
        /// This is the constructor for the loading page that fires before the events carousel page.
        /// This page is entirely programmatically created as has no XAML page associated with it.
        /// </summary>
        /// <param name="client">The REST Client.</param>
        public LoadingPage(ref RestClient client)
        {
            // As stated earlier, there is no XAML page associated with this page so everything here is simply
            // written in order to make the page actually appear.
            logo = new Image()
            {
                Source = ImageSource.FromFile("PineGroveLogo.png"),
                Aspect = Aspect.AspectFill,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Opacity = 1
            };
            Grid innerGrid = new Grid()
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.FromHex("999999"),
                Padding = 25
            };
            innerGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            innerGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
            innerGrid.Children.Add(logo, 0, 0);
            innerGrid.Children.Add(new Label()
            {
                Text = "Loading...",
                VerticalOptions = LayoutOptions.EndAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                HorizontalTextAlignment = TextAlignment.Center,
                FontSize = 30,
                FontFamily = (OnPlatform<string>)Application.Current.Resources["Font"],
                TextColor = Color.FromHex("000000")
            }, 0, 1);
            StackLayout mainStack = new StackLayout()
            {
                Spacing = 0,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.FromHex("333333"),
                Children =
                {
                    new Image()
                    {
                        Source = ImageSource.FromFile("pinegrovebanner.png"),
                        Margin = new Thickness(15, 5),
                        HeightRequest = 75,
                        IsOpaque = true
                    },
                    new BoxView()
                    {
                        Color = Color.FromHex("000000"),
                        HeightRequest = 1
                    },
                    innerGrid
                }
            };
            Content = mainStack;
            AfterAnimation += LoadingPage_AfterAnimation;   // This event is used to give the logo the effect of fading in and out.
                                                            // I wrote this before I knew that there is actually a built-in method
                                                            // to do this in Xamarin.Forms...
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>    // This waits one second before starting the opacity change (to prevent frame dropping due to too much loading at once).
            {
                LoadingPage_AfterAnimation(this, new EventArgs());  // Start the effect.
                return false;                                       // End the timer.
            });
            MakeEvents(client); // Go ahead and start making the events now that this page is loaded.
        }

        /// <summary>
        /// After the animation of the modal page appearing, this changes the opacity of the logo image to indicate to the user that loading is happening.
        /// </summary>
        /// <param name="sender">The object that causes the event.</param>
        /// <param name="e">Event arguments.</param>
        private void LoadingPage_AfterAnimation(object sender, EventArgs e)
        {
            // This essentially just changes to opacity to 100%, down to 0%, and back over and over.
            // The timer never truely stops until the page is no longer being displayed.
            bool on = true;
            Device.StartTimer(TimeSpan.FromMilliseconds(.5), () =>
            {
                if (on)
                {
                    if (logo.Opacity < 1)
                        logo.Opacity += .025f;
                    else
                        on = false;
                    return true;
                }
                else
                {
                    if (logo.Opacity > 0)
                        logo.Opacity -= .025f;
                    else
                        on = true;
                    return true;
                }
            });
        }

        /// <summary>
        /// This creates a new carousel page that shows all of the events. Once the page is populated and loaded, the page is then shown.
        /// </summary>
        /// <param name="client">The REST Client.</param>
        private void MakeEvents(RestClient client)
        {
            events = new EventsCarouselPage(ref client);
            events.OnLoad += Events_OnLoad;
        }

        /// <summary>
        /// Once the events are loaded and the carousel page is populated, this shows the page.
        /// </summary>
        /// <param name="sender">The object that causes the event.</param>
        /// <param name="e">Event arguments.</param>
        private async void Events_OnLoad(object sender, EventArgs e) { await Navigation.PushModalAsync(events); }
    }
}