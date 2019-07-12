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
    // This page actually is repurposed depending on circumstances.
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegistrationPage : ContentPage
    {
        // Class variables.
        private Services.RestClient client;
        private readonly bool editing = false;
        private Models.User newUser;

        /// <summary>
        /// This is a blank, brand-new registration page to create a user in the database.
        /// </summary>
        /// <param name="client">The REST Client.</param>
        public RegistrationPage(ref Services.RestClient client)
        {
            this.client = client;
            InitializeComponent();
            SetUpPage();    // Sets up the page as a whole.
            SetUpGrid();    // Sets up the grid on the page.
        }

        /// <summary>
        /// This is a registration page with pre-existing first and last names from the lookup page.
        /// </summary>
        /// <param name="client">The REST Client.</param>
        /// <param name="firstName">The user's first name.</param>
        /// <param name="lastName">The user's last name.</param>
        public RegistrationPage(ref Services.RestClient client, string firstName, string lastName)
        {
            // This constructor is used when the lookup page does not find a user and the user then wishes to create a new user.
            // It simply fills in the first and last names with the information they provided in the lookup page to make it
            // slightly easier on the user.
            this.client = client;
            InitializeComponent();
            SetUpPage();    // Sets up the page as a whole.
            SetUpGrid();    // Sets up the grid on the page.
            txtFirstName.Text = firstName;  // Fill in the first name.
            txtLastName.Text = lastName;    // Fill in the last name.
        }

        /// <summary>
        /// This is a user-editing page that uses the same controls as the registration page.
        /// </summary>
        /// <param name="client">The REST Client.</param>
        /// <param name="user">The user that is to be edited.</param>
        public RegistrationPage(ref Services.RestClient client, Models.User user)
        {
            this.client = client;
            editing = true;
            newUser = user;
            InitializeComponent();
            SetUpPage();    // Sets up the page as a whole.
            SetUpGrid();    // Sets up the grid on the page.
            lblTitle.Text = "Edit User Form";   // Change the title to reflect editing.
            // Fill in the fields with the user's current information.
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

        /// <summary>
        /// Sets up very specific parts of the page as necessary.
        /// </summary>
        private void SetUpPage()
        {
            // This really just fills in the state drop-down list and creates two custom keyboards for the address fields.
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

        /// <summary>
        /// This is called when the clear button is pressed.
        /// </summary>
        /// <param name="sender">The object that causes the event.</param>
        /// <param name="e">Event arguments.</param>
        private void BtnClear_Clicked(object sender, EventArgs e)
        {
            // This simply goes through and clears all the controls from each grid.
            foreach (View view in grdAll.Children)
                if (view is Entry)
                    (view as Entry).Text = null;
            foreach (View view in grdAddress.Children)
                if (view is Entry)
                    (view as Entry).Text = null;
            lstState.SelectedIndex = -1;
        }

        /// <summary>
        /// This is called when the submit button is pressed.
        /// </summary>
        /// <param name="sender">The object that causes the event.</param>
        /// <param name="e">Event arguments.</param>
        private async void BtnSubmit_Clicked(object sender, EventArgs e)
        {
            // If the required information is not filled out, end processing and let the user know what the mistake is.
            if (!RequiredValidation())
            {
                UserDialogs.Instance.Toast(new ToastConfig("First and last name are required!") { BackgroundColor = App.toastColor });
                return;
            }
            // If we are not in edit mode we need to create a new User model. If we are editing, this has already been created.
            if (!editing)
                newUser = new Models.User() { FirstName = txtFirstName.Text, LastName = txtLastName.Text };
            // If the email address is not empty (it isn't required, after all), make sure the email is valid using a regular expression.
            if (txtEmail.Text != null && !txtEmail.Text.Equals(string.Empty))
            {
                // If it isn't valid, stop processing and let the user know what the mistake is.
                if (!Regex.IsMatch(txtEmail.Text, @"(?<=[\w]{1})[\w-\._\+%]*(?=[\w]{2}@)")) // This is the same regex pattern that is used for displaying the email on the lookup page.
                {
                    UserDialogs.Instance.Toast(new ToastConfig("Email address is invalid!") { BackgroundColor = App.toastColor });
                    return;
                }
                else
                    newUser.EmailAddress = txtEmail.Text;
            }
            // If the email address is empty, set it to null. There are circumstances where the user can click on the field and then leave it to where the text will be string.Empty instead of null, so this avoids that.
            else
                newUser.EmailAddress = null;
            // If the phone number has been started, fall here.
            if (txtPhoneNumber.Text != null && !txtPhoneNumber.Text.Equals(string.Empty))
            {
                // I made it to where the phone number is stored, after processing, like (xxx) xxx-xxxx, which is 14 characters. If it isn't 14 characters, it is invalid.
                if (txtPhoneNumber.Text.Length != 14)
                {
                    UserDialogs.Instance.Toast(new ToastConfig("Phone number is invalid!") { BackgroundColor = App.toastColor });
                    return;
                }
                // If I can't parse the re-non-processed phone number to a long integer, it is invalid.
                if (!long.TryParse(txtPhoneNumber.Text.Substring(1, 3) + txtPhoneNumber.Text.Substring(6, 3) + txtPhoneNumber.Text.Substring(10, 4), out long phoneNumber))
                {
                    UserDialogs.Instance.Toast(new ToastConfig("Phone number is invalid!") { BackgroundColor = App.toastColor });
                    return;
                }
                // Otherwise, I got it to be in its original format and can then store it.
                else
                    newUser.PhoneNumber = phoneNumber;
            }
            // If there was no phone number, make it null.
            else
                newUser.PhoneNumber = null;
            // This is validation for the address if it is necessary.
            if (AddressStarted())
            {
                // Address line one is required for any address. This verifies that it is filled out.
                if (txtAddressLineOne.Text == null || txtAddressLineOne.Text.Equals(string.Empty))
                {
                    UserDialogs.Instance.Toast(new ToastConfig("Address line one is empty or invalid!") { BackgroundColor = App.toastColor });
                    return;
                }
                // And then stores it.
                newUser.AddressLineOne = txtAddressLineOne.Text;
                // If address line two has nothing in it, it isn't required so leave it to be null.
                if (txtAddressLineTwo.Text == null || txtAddressLineTwo.Text.Equals(string.Empty))
                    newUser.AddressLineTwo = null;
                // If it does have something in it, use it.
                else
                    newUser.AddressLineTwo = txtAddressLineTwo.Text;
                // The city is required if the address has been started, so verify it has something in it. Looking back, I should have made this a method that takes a textbox and a error message string as a parameter... Oh well.
                if (txtCity.Text == null || txtCity.Text.Equals(string.Empty))
                {
                    UserDialogs.Instance.Toast(new ToastConfig("City is empty or invalid!") { BackgroundColor = App.toastColor });
                    return;
                }
                else
                    newUser.City = txtCity.Text;
                // Same with the zip code.
                if (txtZip.Text == null || txtZip.Text.Equals(string.Empty))
                {
                    UserDialogs.Instance.Toast(new ToastConfig("Zip code empty!") { BackgroundColor = App.toastColor });
                    return;
                }
                // The zip code should only be five characters in this circumstance.
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
                // If the five characters start with a 0, that's invalid...
                if (zipCode - 9999 < 1)
                {
                    UserDialogs.Instance.Toast(new ToastConfig("Zip code invalid!") { BackgroundColor = App.toastColor });
                    return;
                }
                // Store the validated zip code.
                else
                    newUser.ZipCode = txtZip.Text;
                // The user has to pick a state.
                if (lstState.SelectedIndex == -1)
                {
                    UserDialogs.Instance.Toast(new ToastConfig("State is not selected!") { BackgroundColor = App.toastColor });
                    return;
                }
                // Store the selected state.
                else
                    newUser.State = lstState.Items[lstState.SelectedIndex];
            }
            // The address wasn't started? Make everything null.
            else
            {
                newUser.AddressLineOne = newUser.AddressLineTwo = newUser.City = null;
                newUser.ZipCode = newUser.State = null;
            }
            // Now we try to create (or edit) the user.
            try
            {
                CancellationTokenSource source = new CancellationTokenSource(); // I should have made a static CancellationTokenSource in the App.xaml.cs file to avoid having to do this every time... Or just built it into the RestClient class.
                source.CancelAfter((int)App.timeoutTime);
                // If we are not editing an already-existing user, we have to generate a user name before posting.
                if (!editing)
                {
                    // First thing, we gotta tell the user what we are doing.
                    UserDialogs.Instance.Toast(new ToastConfig("Generating user name...") { BackgroundColor = App.toastColor, Duration = TimeSpan.FromMilliseconds(App.timeoutTime) });
                    // Make the first call to the recursive function to generate a username. This function works well for this environment where multiple users having the same first three of their first name and first three of their last name
                    // is relatively uncommon, but in different environment I would find a better, non-recursive way of doing this. There are clearly limitations on this kind of username as well. However, at the end of the day, this is for a
                    // church of only 300 or so people. So, it works for now.
                    newUser.UserName = await GenerateUsername(newUser.FirstName, newUser.LastName, 1, source);
                    // If something happened and it broke, stop the process. The generate username method already shows an error message.
                    if (newUser.UserName == null)
                        return;
                    // Let them know we are done generating a username.
                    UserDialogs.Instance.Toast(new ToastConfig("Posting new user to database...") { BackgroundColor = App.toastColor, Duration = TimeSpan.FromMilliseconds(App.timeoutTime) });
                    await client.CreateUser(newUser, source.Token);
                    // Save the new username to the local storage.
                    Application.Current.Properties["Username"] = newUser.UserName;
                    await Application.Current.SavePropertiesAsync();
                    // Tell them we were successful!
                    UserDialogs.Instance.Toast(new ToastConfig("Succeeded in creating user! Your username is " + newUser.UserName + "!") { BackgroundColor = App.toastColor });
                }
                else
                {
                    // If we were editing, we can bypass the generation of the username and just go with whatever information was changed.
                    UserDialogs.Instance.Toast(new ToastConfig("Updating database...") { BackgroundColor = App.toastColor, Duration = TimeSpan.FromMilliseconds(App.timeoutTime) });
                    await client.UpdateUser(newUser.UserId, newUser, source.Token); // Perform a PUT instead of a POST on the API.
                    UserDialogs.Instance.Toast(new ToastConfig("User edit successful!") { BackgroundColor = App.toastColor });  // Let them know we succeeded.
                }
                Application.Current.MainPage = new MainPage(ref client);
            }
            catch (ValidationApiException)  // Bad information...
            {
                UserDialogs.Instance.Toast(new ToastConfig("Error! Database may be offline. Please try again later.") { BackgroundColor = App.toastColor });
            }
            catch (ApiException)            // User was deleted admist operation by admin...
            {
                UserDialogs.Instance.Toast(new ToastConfig("Error! There was an error creating the user in the database. Try again later.") { BackgroundColor = App.toastColor });
            }
            catch (System.Net.Http.HttpRequestException)    // No internet...
            {
                UserDialogs.Instance.Toast(new ToastConfig("Error! Connection blocked! Please try again later.") { BackgroundColor = App.toastColor });
            }
            catch (TaskCanceledException)   // Request timed out...
            {
                UserDialogs.Instance.Toast(new ToastConfig("Error! Request timed out! Please try again later.") { BackgroundColor = App.toastColor });
            }
        }

        /// <summary>
        /// This checks to see if the required fields are filled out.
        /// </summary>
        /// <returns>A boolean indicating if the first and last name fields contain information.</returns>
        private bool RequiredValidation()
        {
            if (txtFirstName.Text == null || txtLastName.Text == null)
                return false;
            if (txtFirstName.Text.Equals(string.Empty) || txtLastName.Text.Equals(string.Empty))
                return false;
            return true;
        }

        /// <summary>
        /// This checks to see if any of the address fields contain information.
        /// </summary>
        /// <returns>A boolean indicating whether or not the any of the address fields contain information.</returns>
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

        /// <summary>
        /// A recursive method to generate a new username based on the first and last names.
        /// </summary>
        /// <param name="firstName">The user's first name.</param>
        /// <param name="lastName">The user's last name.</param>
        /// <param name="userNumber">The number appended to the end of the user's username.</param>
        /// <param name="source">A cancellation token source to generate CancellationToken objects as needed.</param>
        /// <returns>An awaitable task who's result is the user's username as a string.</returns>
        private async Task<string> GenerateUsername(string firstName, string lastName, int userNumber, CancellationTokenSource source)
        {
            // First we get their first three of the first and last names (or how many of the letters we can fit if there are less than three).
            string userName;
            if (lastName.Length < 3)
                userName = lastName.ToUpper();
            else
                userName = lastName.Substring(0, 3).ToUpper();
            if (firstName.Length < 3)
                userName += firstName.ToUpper();
            else
                userName += firstName.Substring(0, 3).ToUpper();
            userName += "_" + userNumber.ToString("00");    // The format will always be 01, 02, 03, etc.
            // Try to get the user from the database via an API GET call.
            try
            {
                await client.GetUser(userName, source.Token);   // Call the API.
                return await GenerateUsername(firstName, lastName, userNumber + 1, source); // If we got someone back, call this function again but with a different user number.
            }
            catch (ValidationApiException)
            {
                return userName;    // We got an error? That's actually a good thing. Show them the money!
            }
            catch (ApiException)
            {
                return userName;    // Same here. This is the one that will more than likely be the reason we return.
            }
        }

        /// <summary>
        /// This is called when the size of the screen is changed.
        /// </summary>
        /// <param name="width">The new width of the screen.</param>
        /// <param name="height">The new height of the screen.</param>
        protected override void OnSizeAllocated(double width, double height)
        {
            // This is the same as many of the other pages.
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
        /// Sets up the grids for this page.
        /// </summary>
        private void SetUpGrid()
        {
            // I've gotten used to doing this programmatically... I know some people highly prefer doing it with XAML, and I do know how to do it...
            // It just seems easier to me to do it the way I have been doing it.
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

        // The following events help move the user from one part of the form to the next upon pressing the 'next' button.

        /// <summary>
        /// This event is called when the user completes the first name textbox.
        /// </summary>
        /// <param name="sender">The object that causes the event.</param>
        /// <param name="e">Event arguments.</param>
        private void TxtFirstName_Completed(object sender, EventArgs e)
        { txtLastName.Focus(); }

        /// <summary>
        /// This event is called when the user completes the last name textbox.
        /// </summary>
        /// <param name="sender">The object that causes the event.</param>
        /// <param name="e">Event arguments.</param>
        private void TxtLastName_Completed(object sender, EventArgs e)
        { txtEmail.Focus(); }

        /// <summary>
        /// This event is called when the user completes the email textbox.
        /// </summary>
        /// <param name="sender">The object that causes the event.</param>
        /// <param name="e">Event arguments.</param>
        private void TxtEmail_Completed(object sender, EventArgs e)
        { txtPhoneNumber.Focus(); }

        /// <summary>
        /// This event is called when the user completes the phone number textbox.
        /// </summary>
        /// <param name="sender">The object that causes the event.</param>
        /// <param name="e">Event arguments.</param>
        private void TxtPhoneNumber_Completed(object sender, EventArgs e)
        {
            // This formats the phone number to look better.
            if (long.TryParse(txtPhoneNumber.Text, out long _) && txtPhoneNumber.Text.Length == 10)
                txtPhoneNumber.Text = "(" + txtPhoneNumber.Text.Substring(0, 3) + ") " + txtPhoneNumber.Text.Substring(3, 3) + "-" + txtPhoneNumber.Text.Substring(6);
            txtAddressLineOne.Focus();
        }

        /// <summary>
        /// This event is called when the user completes the address line one textbox.
        /// </summary>
        /// <param name="sender">The object that causes the event.</param>
        /// <param name="e">Event arguments.</param>
        private void TxtAddressLineOne_Completed(object sender, EventArgs e)
        { txtAddressLineTwo.Focus(); }

        /// <summary>
        /// This event is called when the user completes the address line two textbox.
        /// </summary>
        /// <param name="sender">The object that causes the event.</param>
        /// <param name="e">Event arguments.</param>
        private void TxtAddressLineTwo_Completed(object sender, EventArgs e)
        { txtCity.Focus(); }

        /// <summary>
        /// This event is called when the user completes the city textbox.
        /// </summary>
        /// <param name="sender">The object that causes the event.</param>
        /// <param name="e">Event arguments.</param>
        private void TxtCity_Completed(object sender, EventArgs e)
        { lstState.Focus(); }

        /// <summary>
        /// This event is called when the user completes the zip code textbox.
        /// </summary>
        /// <param name="sender">The object that causes the event.</param>
        /// <param name="e">Event arguments.</param>
        private void TxtZip_Completed(object sender, EventArgs e)
        { BtnSubmit_Clicked(sender, e); }

        /// <summary>
        /// This event is called when the user changes the state list's index.
        /// </summary>
        /// <param name="sender">The object that causes the event.</param>
        /// <param name="e">Event arguments.</param>
        private void LstState_SelectedIndexChanged(object sender, EventArgs e)
        { txtZip.Focus(); }

        /// <summary>
        /// This event is called when the user focuses the phone number textbox.
        /// </summary>
        /// <param name="sender">The object that causes the event.</param>
        /// <param name="e">Event arguments.</param>
        private void TxtPhoneNumber_Focused(object sender, FocusEventArgs e)
        {
            // This makes the phone number turn back into digits.
            if (txtPhoneNumber.Text != null && Regex.IsMatch(txtPhoneNumber.Text, @"\(\d{3}\) \d{3}-\d{4}"))
                txtPhoneNumber.Text = txtPhoneNumber.Text.Substring(1, 3) + txtPhoneNumber.Text.Substring(6, 3) + txtPhoneNumber.Text.Substring(10, 4);
        }

        /// <summary>
        /// This event is called when the user unfocuses the phone number textbox.
        /// </summary>
        /// <param name="sender">The object that causes the event.</param>
        /// <param name="e">Event arguments.</param>
        private void TxtPhoneNumber_Unfocused(object sender, FocusEventArgs e)
        {
            // This just makes the phone number look fancier.
            if (long.TryParse(txtPhoneNumber.Text, out long _) && txtPhoneNumber.Text.Length == 10)
                txtPhoneNumber.Text = "(" + txtPhoneNumber.Text.Substring(0, 3) + ") " + txtPhoneNumber.Text.Substring(3, 3) + "-" + txtPhoneNumber.Text.Substring(6);
        }
    }
}