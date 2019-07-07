using System;
using System.Text.RegularExpressions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Refit;
using Acr.UserDialogs;
using System.Threading;
using System.Threading.Tasks;

namespace PineGroveMobileApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegistrationPage : ContentPage
    {
        private Services.RestClient client;
        private readonly bool editing = false;
        private Models.User newUser;
        public RegistrationPage(ref Services.RestClient client)
        {
            this.client = client;
            InitializeComponent();
            SetUpPage();
            SetUpGrid();
        }

        public RegistrationPage(ref Services.RestClient client, string firstName, string lastName)
        {
            this.client = client;
            InitializeComponent();
            SetUpPage();
            SetUpGrid();
            txtFirstName.Text = firstName;
            txtLastName.Text = lastName;
        }

        public RegistrationPage(ref Services.RestClient client, Models.User user)
        {
            this.client = client;
            editing = true;
            newUser = user;
            InitializeComponent();
            SetUpPage();
            SetUpGrid();
            lblTitle.Text = "Edit User Form";
            txtFirstName.Text = user.FirstName;
            txtLastName.Text = user.LastName;
            txtEmail.Text = user.EmailAddress;
            txtPhoneNumber.Text = user.PhoneNumber.ToString();
            if (txtPhoneNumber.Text != null && !txtPhoneNumber.Text.Equals(string.Empty))
                TxtPhoneNumber_Unfocused(null, null);
            txtAddressLineOne.Text = user.AddressLineOne;
            txtAddressLineTwo.Text = user.AddressLineTwo;
            txtZip.Text = user.ZipCode;
            txtCity.Text = user.City;
            if (user.State != null && !user.State.Equals(string.Empty))
                lstState.SelectedIndex = lstState.Items.IndexOf(user.State);
        }

        private void SetUpPage()
        {
            string[] states =
            {
                "NC", "AK", "AL", "AR", "AS", "AZ", "CA", "CO", "CT", "DC", "DE", "FL", "GA",
                "GU", "HI", "IA", "ID", "IL", "IN", "KS", "KY", "LA", "MA", "MD", "ME", "MI",
                "MN", "MO", "MP", "MS", "MT", "ND", "NE", "NH", "NJ", "NM", "NV", "NY", "OH",
                "OK", "OR", "PA", "PR", "RI", "SC", "SD", "TN", "TX", "UM", "UT", "VA", "VI",
                "VT", "WA", "WI", "WV", "WY"
            };
            foreach (string state in states)
                lstState.Items.Add(state);
            txtAddressLineOne.Keyboard = Keyboard.Create(KeyboardFlags.CapitalizeWord);
            txtAddressLineTwo.Keyboard = Keyboard.Create(KeyboardFlags.CapitalizeWord);
        }

        private void BtnClear_Clicked(object sender, EventArgs e)
        {
            foreach (View view in grdAll.Children)
                if (view is Entry)
                    (view as Entry).Text = null;
            foreach (View view in grdAddress.Children)
                if (view is Entry)
                    (view as Entry).Text = null;
            lstState.SelectedIndex = -1;
        }

        private async void BtnSubmit_Clicked(object sender, EventArgs e)
        {
            if (!RequiredValidation())
            {
                UserDialogs.Instance.Toast(new ToastConfig("First and last name are required!") { BackgroundColor = App.toastColor });
                return;
            }
            if (!editing)
                newUser = new Models.User() { FirstName = txtFirstName.Text, LastName = txtLastName.Text };
            if (txtEmail.Text != null && !txtEmail.Text.Equals(string.Empty))
            {
                if (!Regex.IsMatch(txtEmail.Text, @"(?<=[\w]{1})[\w-\._\+%]*(?=[\w]{2}@)"))
                {
                    UserDialogs.Instance.Toast(new ToastConfig("Email address is invalid!") { BackgroundColor = App.toastColor });
                    return;
                }
                else
                    newUser.EmailAddress = txtEmail.Text;
            }
            else
                newUser.EmailAddress = null;
            if (txtPhoneNumber.Text != null && !txtPhoneNumber.Text.Equals(string.Empty))
            {
                if (txtPhoneNumber.Text.Length != 14)
                {
                    UserDialogs.Instance.Toast(new ToastConfig("Phone number is invalid!") { BackgroundColor = App.toastColor });
                    return;
                }
                if (!long.TryParse(txtPhoneNumber.Text.Substring(1, 3) + txtPhoneNumber.Text.Substring(6, 3) + txtPhoneNumber.Text.Substring(10, 4), out long phoneNumber))
                {
                    UserDialogs.Instance.Toast(new ToastConfig("Phone number is invalid!") { BackgroundColor = App.toastColor });
                    return;
                }
                else
                    newUser.PhoneNumber = phoneNumber;
            }
            else
                newUser.PhoneNumber = null;
            if (AddressStarted())
            {
                if (txtAddressLineOne.Text == null || txtAddressLineOne.Text.Equals(string.Empty))
                {
                    UserDialogs.Instance.Toast(new ToastConfig("Address line one is empty or invalid!") { BackgroundColor = App.toastColor });
                    return;
                }
                newUser.AddressLineOne = txtAddressLineOne.Text;
                if (txtAddressLineTwo.Text == null || txtAddressLineTwo.Text.Equals(string.Empty))
                    newUser.AddressLineTwo = null;
                else
                    newUser.AddressLineTwo = txtAddressLineTwo.Text;
                if (txtCity.Text == null || txtCity.Text.Equals(string.Empty))
                {
                    UserDialogs.Instance.Toast(new ToastConfig("City is empty or invalid!") { BackgroundColor = App.toastColor });
                    return;
                }
                else
                    newUser.City = txtCity.Text;
                if (txtZip.Text == null || txtZip.Text.Equals(string.Empty))
                {
                    UserDialogs.Instance.Toast(new ToastConfig("Zip code empty!") { BackgroundColor = App.toastColor });
                    return;
                }
                if (txtZip.Text.Length != 5)
                {
                    UserDialogs.Instance.Toast(new ToastConfig("Zip code invalid!") { BackgroundColor = App.toastColor });
                    return;
                }
                if (!int.TryParse(txtZip.Text, out int zipCode))
                {
                    UserDialogs.Instance.Toast(new ToastConfig("Zip code invalid!") { BackgroundColor = App.toastColor });
                    return;
                }
                if (zipCode - 9999 < 1)
                {
                    UserDialogs.Instance.Toast(new ToastConfig("Zip code invalid!") { BackgroundColor = App.toastColor });
                    return;
                }
                else
                    newUser.ZipCode = txtZip.Text;
                if (lstState.SelectedIndex == -1)
                {
                    UserDialogs.Instance.Toast(new ToastConfig("State is not selected!") { BackgroundColor = App.toastColor });
                    return;
                }
                else
                    newUser.State = lstState.Items[lstState.SelectedIndex];
            }
            else
            {
                newUser.AddressLineOne = newUser.AddressLineTwo = newUser.City = null;
                newUser.ZipCode = newUser.State = null;
            }
            try
            {
                CancellationTokenSource source = new CancellationTokenSource();
                source.CancelAfter((int)App.timeoutTime);
                if (!editing)
                {
                    UserDialogs.Instance.Toast(new ToastConfig("Generating user name...") { BackgroundColor = App.toastColor, Duration = TimeSpan.FromMilliseconds(App.timeoutTime) });
                    newUser.UserName = await GenerateUsername(newUser.FirstName, newUser.LastName, 1, source);
                    if (newUser.UserName == null)
                        return;
                    UserDialogs.Instance.Toast(new ToastConfig("Posting new user to database...") { BackgroundColor = App.toastColor, Duration = TimeSpan.FromMilliseconds(App.timeoutTime) });
                    await client.CreateUser(newUser, source.Token);
                    Application.Current.Properties["Username"] = newUser.UserName;
                    await Application.Current.SavePropertiesAsync();
                    UserDialogs.Instance.Toast(new ToastConfig("Succeeded in creating user! Your username is " + newUser.UserName + "!") { BackgroundColor = App.toastColor });
                }
                else
                {
                    UserDialogs.Instance.Toast(new ToastConfig("Updating database...") { BackgroundColor = App.toastColor, Duration = TimeSpan.FromMilliseconds(App.timeoutTime) });
                    await client.UpdateUser(newUser.UserId, newUser, source.Token);
                    UserDialogs.Instance.Toast(new ToastConfig("User edit successful!") { BackgroundColor = App.toastColor });
                }
                Application.Current.MainPage = new MainPage(ref client);
            }
            catch (ValidationApiException)
            {
                UserDialogs.Instance.Toast(new ToastConfig("Access denied! Connection blocked. Please try again later.") { BackgroundColor = App.toastColor });
            }
            catch (ApiException)
            {
                UserDialogs.Instance.Toast(new ToastConfig("Error! There was an error creating the user in the database. Try again later.") { BackgroundColor = App.toastColor });
            }
            catch (TaskCanceledException)
            {
                UserDialogs.Instance.Toast(new ToastConfig("Error! Request timed out! Please try again later.") { BackgroundColor = App.toastColor });
            }
        }

        private bool RequiredValidation()
        {
            if (txtFirstName.Text == null || txtLastName.Text == null)
                return false;
            if (txtFirstName.Text.Equals(string.Empty) || txtLastName.Text.Equals(string.Empty))
                return false;
            return true;
        }

        private bool AddressStarted()
        {
            if (txtAddressLineOne.Text != null && !txtAddressLineOne.Text.Equals(string.Empty))
                return true;
            if (txtAddressLineTwo.Text != null && !txtAddressLineTwo.Text.Equals(string.Empty))
                return true;
            if (txtCity.Text != null && !txtCity.Text.Equals(string.Empty))
                return true;
            if (txtZip.Text != null && !txtZip.Text.Equals(string.Empty))
                return true;
            if (lstState.SelectedIndex != -1)
                return true;
            return false;
        }

        private async Task<string> GenerateUsername(string firstName, string lastName, int userNumber, CancellationTokenSource source)
        {
            string userName;
            if (lastName.Length < 3)
                userName = lastName.ToUpper();
            else
                userName = lastName.Substring(0, 3).ToUpper();
            if (firstName.Length < 3)
                userName += firstName.ToUpper();
            else
                userName += firstName.Substring(0, 3).ToUpper();
            userName += "_" + userNumber.ToString("00");
            try
            {
                await client.GetUser(userName, source.Token);
                return await GenerateUsername(firstName, lastName, userNumber + 1, source);
            }
            catch (ValidationApiException)
            {
                return userName;
            }
            catch (ApiException)
            {
                return userName;
            }
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
            grdAddress.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            grdAddress.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
            grdAddress.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
            grdAddress.Children.Add(txtCity, 0, 0);
            grdAddress.Children.Add(lstState, 1, 0);
            grdAddress.Children.Add(txtZip, 2, 0);

            grdAll.Children.Add(lblTitle, 0, 0);
            Grid.SetColumnSpan(lblTitle, 2);
            grdAll.Children.Add(lblPrompt, 0, 1);
            Grid.SetColumnSpan(lblPrompt, 2);
            grdAll.Children.Add(txtFirstName, 0, 2);
            grdAll.Children.Add(txtLastName, 1, 2);
            grdAll.Children.Add(txtEmail, 0, 3);
            grdAll.Children.Add(txtPhoneNumber, 1, 3);
            grdAll.Children.Add(txtAddressLineOne, 0, 4);
            Grid.SetColumnSpan(txtAddressLineOne, 2);
            grdAll.Children.Add(txtAddressLineTwo, 0, 5);
            Grid.SetColumnSpan(txtAddressLineTwo, 2);
            grdAll.Children.Add(grdAddress, 0, 6);
            Grid.SetColumnSpan(grdAddress, 2);
            grdAll.Children.Add(btnClear, 0, 7);
            grdAll.Children.Add(btnSubmit, 1, 7);
        }

        private void TxtFirstName_Completed(object sender, EventArgs e)
        { txtLastName.Focus(); }

        private void TxtLastName_Completed(object sender, EventArgs e)
        { txtEmail.Focus(); }

        private void TxtEmail_Completed(object sender, EventArgs e)
        { txtPhoneNumber.Focus(); }

        private void TxtPhoneNumber_Completed(object sender, EventArgs e)
        {
            if (long.TryParse(txtPhoneNumber.Text, out long _) && txtPhoneNumber.Text.Length == 10)
                txtPhoneNumber.Text = "(" + txtPhoneNumber.Text.Substring(0, 3) + ") " + txtPhoneNumber.Text.Substring(3, 3) + "-" + txtPhoneNumber.Text.Substring(6);
            txtAddressLineOne.Focus();
        }

        private void TxtAddressLineOne_Completed(object sender, EventArgs e)
        { txtAddressLineTwo.Focus(); }

        private void TxtAddressLineTwo_Completed(object sender, EventArgs e)
        { txtCity.Focus(); }

        private void TxtCity_Completed(object sender, EventArgs e)
        { lstState.Focus(); }

        private void TxtZip_Completed(object sender, EventArgs e)
        { BtnSubmit_Clicked(sender, e); }

        private void LstState_SelectedIndexChanged(object sender, EventArgs e)
        { txtZip.Focus(); }

        private void TxtPhoneNumber_Focused(object sender, FocusEventArgs e)
        {
            if (txtPhoneNumber.Text != null && Regex.IsMatch(txtPhoneNumber.Text, @"\(\d{3}\) \d{3}-\d{4}"))
                txtPhoneNumber.Text = txtPhoneNumber.Text.Substring(1, 3) + txtPhoneNumber.Text.Substring(6, 3) + txtPhoneNumber.Text.Substring(10, 4);
        }

        private void TxtPhoneNumber_Unfocused(object sender, FocusEventArgs e)
        {
            if (long.TryParse(txtPhoneNumber.Text, out long _) && txtPhoneNumber.Text.Length == 10)
                txtPhoneNumber.Text = "(" + txtPhoneNumber.Text.Substring(0, 3) + ") " + txtPhoneNumber.Text.Substring(3, 3) + "-" + txtPhoneNumber.Text.Substring(6);
        }
    }
}