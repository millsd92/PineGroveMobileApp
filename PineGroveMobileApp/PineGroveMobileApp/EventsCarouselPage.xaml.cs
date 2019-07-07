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
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EventsCarouselPage : CarouselPage
    {
        private readonly RestClient client;
        public event EventHandler OnLoad;
        private List<Models.Event> events;

        public EventsCarouselPage(ref RestClient client)
        {
            this.client = client;
            InitializeComponent();
            MakePages();
        }

        private async void MakePages()
        {
            events = new List<Models.Event>(await client.GetEvents());
            events.RemoveAll(e => e.EndTime < DateTime.Now);
            ItemsSource = events;
            foreach (ContentPage page in Children)
            {
                SetupPage(page, Children.IndexOf(page));
                page.SizeChanged += Page_SizeChanged;
            }
            OnLoad(this, new EventArgs());
        }

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

        private void SetupPage(ContentPage page, int index)
        {
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
                if (lblEndTime.Text == null || lblEnd.Text.Length < 1)
                    lblEndTime.Text = "Whenever it ends!";
                else
                    lblEndTime.Text = string.Format("{0:g}", Convert.ToDateTime(lblEndTime.Text));
            Label lblAddress = page.FindByName("lblAddress") as Label;
            Label lblAddressInfo = page.FindByName("lblAddressInfo") as Label;
                lblAddressInfo.Text = lblAddressInfo.Text.Replace("\n", "\n\n");
            Button btnRegister = page.FindByName("btnRegister") as Button;
                btnRegister.HorizontalOptions = LayoutOptions.FillAndExpand;
                if (events[index].CurrentAttendees >= events[index].MaxAttendees)
                {
                    btnRegister.IsEnabled = false;
                    btnRegister.Text = "Event full!";
                }
            Label lblGuests = page.FindByName("lblGuests") as Label;
            Stepper stpGuests = page.FindByName("stpGuests") as Stepper;
            if ((events[index].MaxAttendees ?? int.MaxValue) - events[index].CurrentAttendees < 10)
            {
                if ((events[index].MaxAttendees ?? int.MaxValue) - events[index].CurrentAttendees > 1)
                    stpGuests.Maximum = (double)(events[index].MaxAttendees - events[index].CurrentAttendees - 1);
                else
                    stpGuests.Minimum = -1;
            }
            Label lblMaximum = page.FindByName("lblMaximum") as Label;
                if (stpGuests.Minimum == -1)
                {
                    lblMaximum.Text = "Only one spot left!";
                    lblGuests.Text = "";
                }
                else
                    lblMaximum.Text = "Maximum is " + ((int)stpGuests.Maximum).ToString();
            Label txtStepper = page.FindByName("txtStepper") as Label;
                txtStepper.Text = "Current value: " + ((int)stpGuests.Minimum).ToString();
            CheckBox chkVolunteer = page.FindByName("chkVolunteer") as CheckBox;
            Label lblVolunteer = page.FindByName("lblVolunteer") as Label;

            (page.FindByName("imgLogo") as Image).HeightRequest = 75;
            imgEvent.HeightRequest = 75;

            volunteer.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
            volunteer.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            volunteer.Children.Add(chkVolunteer, 0, 0);
            volunteer.Children.Add(lblVolunteer, 1, 0);

            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });

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

        protected override bool OnBackButtonPressed()
        {
            Navigation.PopModalAsync();
            return base.OnBackButtonPressed();
        }

        private async void BtnRegister_Clicked(object sender, EventArgs e)
        {
            if ((sender as Button).Text.Equals("RSVP"))
            {
                int currentValue = (int)(((sender as Button).Parent as Grid).FindByName("stpGuests") as Stepper).Minimum;
                if (currentValue != -1)
                {
                    (((sender as Button).Parent as Grid).FindByName("lblGuests") as Label).IsVisible = true;
                    (((sender as Button).Parent as Grid).FindByName("stpGuests") as Stepper).IsVisible = true;
                    (((sender as Button).Parent as Grid).FindByName("txtStepper") as Label).IsVisible = true;
                    (((sender as Button).Parent as Grid).FindByName("lblMaximum") as Label).IsVisible = true;
                }
                else
                {
                    (((sender as Button).Parent as Grid).FindByName("lblMaximum") as Label).IsVisible = true;
                    Grid.SetColumnSpan(((sender as Button).Parent as Grid).FindByName("lblMaximum") as Label, 2);
                }
                (((sender as Button).Parent as Grid).FindByName("chkVolunteer") as CheckBox).IsVisible = true;
                (((sender as Button).Parent as Grid).FindByName("lblVolunteer") as Label).IsVisible = true;
                (sender as Button).Text = "Confirm";
            }
            else
            {
                try
                {
                    (sender as Button).IsEnabled = false;
                    UserDialogs.Instance.Toast(new ToastConfig("Attempting to register...") { BackgroundColor = App.toastColor, Duration = TimeSpan.FromMilliseconds(App.timeoutTime) });
                    CancellationTokenSource source = new CancellationTokenSource();
                    source.CancelAfter((int)App.timeoutTime);
                    int index = Children.IndexOf((((((sender as Button).Parent as Grid).Parent as StackLayout).Parent as ScrollView).Parent as StackLayout).Parent as ContentPage);
                    if (!(((sender as Button).Parent as Grid).FindByName("chkVolunteer") as CheckBox).IsChecked)
                    {
                        await client.CreateRegistration(new Models.EventRegistration()
                        {
                            EventId = events[index].EventId,
                            UserId = (await client.GetUser(Application.Current.Properties["Username"].ToString(), source.Token)).UserId,
                            Guests = (int)(((sender as Button).Parent as Grid).FindByName("stpGuests") as Stepper).Value
                        }, source.Token);
                        events[index].CurrentAttendees += (int)(((sender as Button).Parent as Grid).FindByName("stpGuests") as Stepper).Value + 1;
                        UserDialogs.Instance.Toast(new ToastConfig("Updating database...") { BackgroundColor = App.toastColor });
                        await client.UpdateEvent(events[index].EventId, events[index], source.Token);
                    }
                    else
                    {
                        await client.CreateRegistration(new Models.EventRegistration()
                        {
                            EventId = events[index].EventId,
                            UserId = (await client.GetUser(Application.Current.Properties["Username"].ToString(), source.Token)).UserId,
                            Guests = 0
                        }, source.Token);
                    }
                    UserDialogs.Instance.Toast(new ToastConfig("Registration successful!") { BackgroundColor = App.toastColor });
                    await Navigation.PopModalAsync();
                    MakePages();
                }
                catch (Refit.ValidationApiException)
                {
                    UserDialogs.Instance.Toast(new ToastConfig("API Validation error! Server may be down... Try again later.") { BackgroundColor = App.toastColor });
                }
                catch (Refit.ApiException)
                {
                    UserDialogs.Instance.Toast(new ToastConfig("API error! Server may be down... Try again later.") { BackgroundColor = App.toastColor });
                }
                catch (System.Net.Http.HttpRequestException)
                {
                    UserDialogs.Instance.Toast(new ToastConfig("Connection blocked! (Internet may be down) Try again later.") { BackgroundColor = App.toastColor });
                }
                catch (TaskCanceledException)
                {
                    UserDialogs.Instance.Toast(new ToastConfig("Request timed out. Try again later.") { BackgroundColor = App.toastColor });
                }
            }
        }

        private void StpGuests_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            (((sender as Stepper).Parent as Grid).FindByName("txtStepper") as Label).Text = "Current value: " + ((int)e.NewValue).ToString();
        }

        private void ChkVolunteer_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (e.Value)
            {
                ((((sender as CheckBox).Parent as Grid).Parent as Grid).FindByName("lblGuests") as Label).IsVisible = false;
                ((((sender as CheckBox).Parent as Grid).Parent as Grid).FindByName("stpGuests") as Stepper).IsVisible = false;
                ((((sender as CheckBox).Parent as Grid).Parent as Grid).FindByName("lblMaximum") as Label).IsVisible = false;
                ((((sender as CheckBox).Parent as Grid).Parent as Grid).FindByName("txtStepper") as Label).IsVisible = false;
            }
            else if (((((sender as CheckBox).Parent as Grid).Parent as Grid).FindByName("stpGuests") as Stepper).Minimum != -1)
            {
                ((((sender as CheckBox).Parent as Grid).Parent as Grid).FindByName("lblGuests") as Label).IsVisible = true;
                ((((sender as CheckBox).Parent as Grid).Parent as Grid).FindByName("stpGuests") as Stepper).IsVisible = true;
                ((((sender as CheckBox).Parent as Grid).Parent as Grid).FindByName("lblMaximum") as Label).IsVisible = true;
                ((((sender as CheckBox).Parent as Grid).Parent as Grid).FindByName("txtStepper") as Label).IsVisible = true;
            }
            else
            {
                (((sender as Button).Parent as Grid).FindByName("lblMaximum") as Label).IsVisible = true;
                Grid.SetColumnSpan(((sender as Button).Parent as Grid).FindByName("lblMaximum") as Label, 2);
            }
        }
    }

    public class ByteToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ImageSource retSource = ImageSource.FromFile("PineGroveLogo.png");
            if (value != null)
            {
                byte[] imageAsBytes = (byte[])value;
                retSource = ImageSource.FromStream(() => new MemoryStream(imageAsBytes));
            }
            return retSource;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public partial class LoadingPage : ContentPage
    {
        private EventsCarouselPage events;
        private event EventHandler AfterAnimation;
        private readonly Image logo;
        public LoadingPage(ref RestClient client)
        {
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
            AfterAnimation += LoadingPage_AfterAnimation;
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                LoadingPage_AfterAnimation(this, new EventArgs());
                return false;
            });
            MakeEvents(client);
        }

        private void LoadingPage_AfterAnimation(object sender, EventArgs e)
        {
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

        private void MakeEvents(RestClient client)
        {
            events = new EventsCarouselPage(ref client);
            events.OnLoad += Events_OnLoad;
        }

        private async void Events_OnLoad(object sender, EventArgs e) { await Navigation.PushModalAsync(events); }
    }
}