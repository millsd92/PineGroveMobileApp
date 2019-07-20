using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Acr.UserDialogs;

namespace PineGroveMobileApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VisitRequestPage : ContentPage
    {
        // Class variables.
        private readonly Services.RestClient client;
        private Models.User currentUser = null;

        /// <summary>
        /// This is the page where the user will submit a request to be visited by the pastor or other church personnel.
        /// </summary>
        /// <param name="client">The REST Client.</param>
        public VisitRequestPage(ref Services.RestClient client)
        {
            this.client = client;   // The same original RestClient object.
            InitializeComponent();
            SetUpPage();            // Sets up the page programmatically.
        }

        /// <summary>
        /// This sets up the page by adding the item source for the drop-down list and setting up the grid for the page.
        /// </summary>
        private void SetUpPage()
        {
            // All the states! Including some outside the US!
            string[] states =
            {
                "NC", "AK", "AL", "AR", "AS", "AZ", "CA", "CO", "CT", "DC", "DE", "FL", "GA",
                "GU", "HI", "IA", "ID", "IL", "IN", "KS", "KY", "LA", "MA", "MD", "ME", "MI",
                "MN", "MO", "MP", "MS", "MT", "ND", "NE", "NH", "NJ", "NM", "NV", "NY", "OH",
                "OK", "OR", "PA", "PR", "RI", "SC", "SD", "TN", "TX", "UM", "UT", "VA", "VI",
                "VT", "WA", "WI", "WV", "WY"
            };
            foreach (string state in states)
                lstState.Items.Add(state);  // Add the states to the list.
            // Custom keyboards that capitalize every word in a sentence.
            txtAddressLineOne.Keyboard = Keyboard.Create(KeyboardFlags.CapitalizeWord);
            txtAddressLineTwo.Keyboard = Keyboard.Create(KeyboardFlags.CapitalizeWord);

            // This makes the columns fit to specific widths. I did this to make the checkboxes only take up as much room as they require instead of evenly spacing the columns.
            grdAddressPrompt.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });    // As much space as needed.
            grdAddressPrompt.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });    // Whatever room is left.
            grdAddressPrompt.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });    // As much space as needed.
            grdAddressPrompt.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });    // Whatever room is left.
            grdAddressPrompt.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });    // As much space as needed.
            grdAddressPrompt.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });    // Whatever room is left.

            // Add children to the inner grid.
            grdAddressPrompt.Children.Add(chkHome, 0, 0);
            grdAddressPrompt.Children.Add(lblHome, 1, 0);
            grdAddressPrompt.Children.Add(chkChurch, 2, 0);
            grdAddressPrompt.Children.Add(lblChurch, 3, 0);
            grdAddressPrompt.Children.Add(chkOther, 4, 0);
            grdAddressPrompt.Children.Add(lblOther, 5, 0);

            // The outer-inner grid.
            grdAddress.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            grdAddress.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
            grdAddress.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });

            // Adding children to the outer-inner grid.
            grdAddress.Children.Add(txtCity, 0, 0);
            grdAddress.Children.Add(lstState, 1, 0);
            grdAddress.Children.Add(txtZip, 2, 0);

            // Adding the grids together!
            grdAll.Children.Add(lblTitle, 0, 0);
            Grid.SetColumnSpan(lblTitle, 2);
            grdAll.Children.Add(txtDescription, 0, 1);
            Grid.SetColumnSpan(txtDescription, 2);
            Grid.SetRowSpan(txtDescription, 4);
            grdAll.Children.Add(lblAddressPrompt, 0, 5);
            Grid.SetColumnSpan(lblAddressPrompt, 2);
            grdAll.Children.Add(grdAddressPrompt, 0, 6);
            Grid.SetColumnSpan(grdAddressPrompt, 2);
            grdAll.Children.Add(txtAddressLineOne, 0, 7);
            Grid.SetColumnSpan(txtAddressLineOne, 2);
            grdAll.Children.Add(txtAddressLineTwo, 0, 8);
            Grid.SetColumnSpan(txtAddressLineTwo, 2);
            grdAll.Children.Add(grdAddress, 0, 9);
            Grid.SetColumnSpan(grdAddress, 2);
            grdAll.Children.Add(btnClear, 0, 10);
            grdAll.Children.Add(btnSubmit, 1, 10);
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

        // These simply move the user from one control to the next upon pressing the 'next' keyboard button.
        private void TxtAddressLineOne_Completed(object sender, EventArgs e)
        { txtAddressLineTwo.Focus(); }

        private void TxtAddressLineTwo_Completed(object sender, EventArgs e)
        { txtCity.Focus(); }

        private void TxtCity_Completed(object sender, EventArgs e)
        { lstState.Focus(); }

        private void TxtZip_Completed(object sender, EventArgs e)
        { BtnSubmit_Clicked(sender, e); }

        private void LstState_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is CheckBox)
                txtZip.Focus();
            chkOther.IsChecked = true;
        }

        /// <summary>
        /// This is called upon the submit button being pressed.
        /// </summary>
        /// <param name="sender">The object that causes the event.</param>
        /// <param name="e">Event arguments.</param>
        private async void BtnSubmit_Clicked(object sender, EventArgs e)
        {
            // If the user is not logged in, we can't do anything.
            if (!Application.Current.Properties.ContainsKey("Username"))
                UserDialogs.Instance.Toast(new ToastConfig("User not logged in! Please log in and try again!") { BackgroundColor = App.toastColor });
            // We also need a description of some sort as to why this visit is necessary.
            else if (txtDescription.Text is null || txtDescription.Text.Equals(string.Empty))
                UserDialogs.Instance.Toast(new ToastConfig("Error! Description is empty!") { BackgroundColor = App.toastColor });
            // Assuming we have those two things, now we have to create the model to post to our database!
            else
            {
                Models.VisitRequest visitRequest = new Models.VisitRequest();
                // Address line one is necessary, so ensure it is filled out.
                if (txtAddressLineOne.Text == null || txtAddressLineOne.Text.Equals(string.Empty))
                {
                    UserDialogs.Instance.Toast(new ToastConfig("Address line one is empty or invalid!") { BackgroundColor = App.toastColor });
                    return;
                }
                visitRequest.AddressLineOne = txtAddressLineOne.Text;
                // Address line two isn't necessary, but it should be null if blank.
                if (txtAddressLineTwo.Text == null || txtAddressLineTwo.Text.Equals(string.Empty))
                    visitRequest.AddressLineTwo = null;
                else
                    visitRequest.AddressLineTwo = txtAddressLineTwo.Text;
                // The city is required so ensure that there is text in the city spot.
                if (txtCity.Text == null || txtCity.Text.Equals(string.Empty))
                {
                    UserDialogs.Instance.Toast(new ToastConfig("City is empty or invalid!") { BackgroundColor = App.toastColor });
                    return;
                }
                else
                    visitRequest.City = txtCity.Text;
                // The zip code is required so ensure that there is something in that spot.
                if (txtZip.Text == null || txtZip.Text.Equals(string.Empty))
                {
                    UserDialogs.Instance.Toast(new ToastConfig("Zip code empty!") { BackgroundColor = App.toastColor });
                    return;
                }
                // It also needs to be 5 characters in length.
                if (txtZip.Text.Length != 5)
                {
                    UserDialogs.Instance.Toast(new ToastConfig("Zip code invalid!") { BackgroundColor = App.toastColor });
                    return;
                }
                // It also needs to be an integer...
                if (!int.TryParse(txtZip.Text, out int zipCode))
                {
                    UserDialogs.Instance.Toast(new ToastConfig("Zip code invalid!") { BackgroundColor = App.toastColor });
                    return;
                }
                // And it also needs to be an integer that is greater than 9999 (this prevents users from putting 00001 or something similar).
                if (zipCode - 9999 < 1)
                {
                    UserDialogs.Instance.Toast(new ToastConfig("Zip code invalid!") { BackgroundColor = App.toastColor });
                    return;
                }
                else
                    visitRequest.ZipCode = txtZip.Text;
                // The list needs to have something selected.
                if (lstState.SelectedIndex == -1)
                {
                    UserDialogs.Instance.Toast(new ToastConfig("State is not selected!") { BackgroundColor = App.toastColor });
                    return;
                }
                else
                    visitRequest.State = lstState.Items[lstState.SelectedIndex];
                // Now, let the user know that we are greating a request for them.
                UserDialogs.Instance.Toast(new ToastConfig("Creating visit request...") { BackgroundColor = App.toastColor });
                visitRequest.Reason = txtDescription.Text;
                visitRequest.RequestDate = DateTime.Now;
                // If we do not have the current user in memory, we need to get them.
                if (currentUser is null)
                {
                    try
                    {
                        UserDialogs.Instance.Toast(new ToastConfig("Fetching user details...") { BackgroundColor = App.toastColor, Duration = TimeSpan.FromMilliseconds(App.timeoutTime) });
                        currentUser = await client.GetUser(Application.Current.Properties["Username"].ToString());
                        UserDialogs.Instance.Toast(new ToastConfig("Fetched user details!") { BackgroundColor = App.toastColor });
                    }
                    catch (Refit.ValidationApiException)    // Bad information...
                    {
                        UserDialogs.Instance.Toast(new ToastConfig("Database error! Database may be offline. Try again later.") { BackgroundColor = App.toastColor });
                        return;
                    }
                    catch (Refit.ApiException)              // User was deleted by admin...
                    {
                        UserDialogs.Instance.Toast(new ToastConfig("User not found in database! Database may be offline. Try again later.") { BackgroundColor = App.toastColor });
                        return;
                    }
                    catch (System.Net.Http.HttpRequestException)    // Internet connection was lost (or firewall settings may have prevented access)
                    {
                        UserDialogs.Instance.Toast(new ToastConfig("Connection blocked! You may have restricted or no internet access! Try again later.") { BackgroundColor = App.toastColor });
                        return;
                    }
                    catch (TaskCanceledException)           // The request timed out.
                    {
                        UserDialogs.Instance.Toast(new ToastConfig("Request timed out. Try again later.") { BackgroundColor = App.toastColor });
                        return;
                    }
                }
                visitRequest.UserId = currentUser.UserId;   // Set the user's id part of the visit request...
                visitRequest.Visited = false;               // ... the user has not been visited yet...
                visitRequest.VisitDate = null;              // ... and the date of the visit hasn't happened yet, either.
                try
                {
                    // Inform our user...
                    UserDialogs.Instance.Toast(new ToastConfig("Posting visit request...") { BackgroundColor = App.toastColor, Duration = TimeSpan.FromMilliseconds(App.timeoutTime) });
                    await client.CreateVisitRequest(visitRequest);
                    // ... of the success!
                    UserDialogs.Instance.Toast(new ToastConfig("Visit request successfully posted!") { BackgroundColor = App.toastColor });
                    BtnClear_Clicked(sender, e);    // Clear the form.
                }
                catch (Refit.ValidationApiException)    // Bad information...
                {
                    UserDialogs.Instance.Toast(new ToastConfig("Database error! Database may be offline. Try again later.") { BackgroundColor = App.toastColor });
                    return;
                }
                catch (Refit.ApiException)              // User was deleted by admin...
                {
                    UserDialogs.Instance.Toast(new ToastConfig("User not found in database! Database may be offline. Try again later.") { BackgroundColor = App.toastColor });
                    return;
                }
                catch (System.Net.Http.HttpRequestException)    // Internet lost...
                {
                    UserDialogs.Instance.Toast(new ToastConfig("Connection blocked! You may have restricted or no internet access! Try again later.") { BackgroundColor = App.toastColor });
                    return;
                }
                catch (TaskCanceledException)           // Request timed out...
                {
                    UserDialogs.Instance.Toast(new ToastConfig("Request timed out. Try again later.") { BackgroundColor = App.toastColor });
                    return;
                }
            }
        }

        /// <summary>
        /// This is called upon the clear button being pressed.
        /// </summary>
        /// <param name="sender">The object that causes the event.</param>
        /// <param name="e">Event arguments.</param>
        private void BtnClear_Clicked(object sender, EventArgs e)
        {
            // This just resets all textboxes, checkboxes, and the listbox to their original values.
            foreach (View view in grdAll.Children)
                if (view is Entry)
                    (view as Entry).Text = null;
                else if (view is Grid)
                    foreach (View secondTierView in (view as Grid).Children)
                        if (secondTierView is Entry)
                            (secondTierView as Entry).Text = null;
            txtDescription.Text = null;
            lstState.SelectedIndex = -1;
            chkOther.IsChecked = true;
        }

        // I chose to use checkboxes instead of radio buttons for only one reason - there is no native radio button control in Xamarin.Forms. That is the only reason.

        /// <summary>
        /// This is called upon the user checking the "home" checkbox option.
        /// </summary>
        /// <param name="sender">The object that causes the event.</param>
        /// <param name="e">Event arguments.</param>
        private async void ChkHome_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            // I placed this option for the convenience of the user to allow them to schedule a visit at the church itself.

            // If it was checked, we are going to change the address fields to show the user's own address (if applicable).
            if (e.Value)
            {
                // First, we need to take off the event handlers for the textbox's content being changed for the time being.
                foreach (View view in grdAll.Children)
                    if (view is Grid)
                    {
                        foreach (View subview in (view as Grid).Children)
                            if (subview is Entry)
                                (subview as Entry).TextChanged -= AddressChanged;
                    }
                    else if (view is Entry)
                        (view as Entry).TextChanged -= AddressChanged;
                lstState.SelectedIndexChanged -= LstState_SelectedIndexChanged;
                // Now we need to ensure the other checkboxes are not checked.
                chkChurch.IsChecked = false;
                chkOther.IsChecked = false;
                // If we don't currently have the user in memory and there is a user logged in, fall here.
                if (currentUser is null && Application.Current.Properties.ContainsKey("Username"))
                {
                    try
                    {
                        // Let the user know what is happening.
                        UserDialogs.Instance.Toast(new ToastConfig("Fetching user details...") { BackgroundColor = App.toastColor, Duration = TimeSpan.FromMilliseconds(App.timeoutTime) });
                        currentUser = await client.GetUser(Application.Current.Properties["Username"].ToString());
                        // Get the user's address from the database. If there is no address, there isn't an address line one for this model.
                        if (currentUser.AddressLineOne is null)
                            UserDialogs.Instance.Toast(new ToastConfig("No address on file for user!") { BackgroundColor = App.toastColor });   // Communicate with our user.
                        // Otherwise, we found an address!
                        else
                        {
                            // Set all the address fields to the ones we found.
                            txtAddressLineOne.Text = currentUser.AddressLineOne;
                            txtAddressLineTwo.Text = currentUser.AddressLineTwo;
                            txtCity.Text = currentUser.City;
                            txtZip.Text = currentUser.ZipCode;
                            lstState.SelectedIndex = lstState.Items.IndexOf(currentUser.State);
                            // Let them know we found it!
                            UserDialogs.Instance.Toast(new ToastConfig("Fetched user address!") { BackgroundColor = App.toastColor });
                        }
                    }
                    catch (Refit.ValidationApiException)    // Bad information...
                    {
                        UserDialogs.Instance.Toast(new ToastConfig("Database error! Database may be offline. Try again later.") { BackgroundColor = App.toastColor });
                    }
                    catch (Refit.ApiException)              // User was deleted from the database from the admin...
                    {
                        UserDialogs.Instance.Toast(new ToastConfig("User not found in database! Database may be offline. Try again later.") { BackgroundColor = App.toastColor });
                    }
                    catch (System.Net.Http.HttpRequestException)    // The connection to the internet was lost...
                    {
                        UserDialogs.Instance.Toast(new ToastConfig("Connection blocked! You may have restricted or no internet access! Try again later.") { BackgroundColor = App.toastColor });
                    }
                    catch (TaskCanceledException)           // Request timed out...
                    {
                        UserDialogs.Instance.Toast(new ToastConfig("Request timed out. Try again later.") { BackgroundColor = App.toastColor });
                    }
                }
                // If we already have the user in memory, just re-put their information back in the textboxes.
                else if (!(currentUser is null))
                {
                    txtAddressLineOne.Text = currentUser.AddressLineOne;
                    txtAddressLineTwo.Text = currentUser.AddressLineTwo;
                    txtCity.Text = currentUser.City;
                    txtZip.Text = currentUser.ZipCode;
                    lstState.SelectedIndex = lstState.Items.IndexOf(currentUser.State);
                }
                // If there is no user logged in, we need to tell them now before they even attempt to submit the request. Looking back, there were much better ways of handling this...
                else
                {
                    UserDialogs.Instance.Toast(new ToastConfig("Warning! User not logged in. Please log in before submitting request.") { BackgroundColor = App.toastColor });
                }
                // Put back all of the event handlers where they were.
                foreach (View view in grdAll.Children)
                    if (view is Grid)
                    {
                        foreach (View subview in (view as Grid).Children)
                            if (subview is Entry)
                                (subview as Entry).TextChanged -= AddressChanged;
                    }
                    else if (view is Entry)
                        (view as Entry).TextChanged += AddressChanged;
                lstState.SelectedIndexChanged += LstState_SelectedIndexChanged;
                txtDescription.TextChanged -= AddressChanged;
            }
            // If this checkbox is being unchecked and the other two are already unchecked, re-check this one.
            else if (!chkChurch.IsChecked && !chkOther.IsChecked)
                (sender as CheckBox).IsChecked = true;
        }

        /// <summary>
        /// This is called upon the user checking the "church" checkbox option.
        /// </summary>
        /// <param name="sender">The object that causes the event.</param>
        /// <param name="e">Event arguments.</param>
        private void ChkChurch_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            // I placed this option for the convenience of the user to allow them to schedule a visit at the church itself.

            // If it was checked, we are going to change the address fields to show the church's own address.
            if (e.Value)
            {
                // First, we need to take off the event that will automatically check the 'Other Address' option for the time being.
                foreach (View view in grdAll.Children)
                    if (view is Grid)
                    {
                        foreach (View subview in (view as Grid).Children)
                            if (subview is Entry)
                                (subview as Entry).TextChanged -= AddressChanged;
                    }
                    else if (view is Entry)
                        (view as Entry).TextChanged -= AddressChanged;
                lstState.SelectedIndexChanged -= LstState_SelectedIndexChanged;
                // Now we make sure that the other two are no longer checked.
                chkHome.IsChecked = false;
                chkOther.IsChecked = false;
                // Now we set the address fields to the church's own address.
                txtAddressLineOne.Text = "1018 Piney Grove Rd.";
                txtAddressLineTwo.Text = null;
                txtCity.Text = "Kernersville";
                txtZip.Text = "27284";
                lstState.SelectedIndex = lstState.Items.IndexOf("NC");
                // And finally we add the AddressChanged event handler back to all the textboxes...
                foreach (View view in grdAll.Children)
                    if (view is Grid)
                    {
                        foreach (View subview in (view as Grid).Children)
                            if (subview is Entry)
                                (subview as Entry).TextChanged -= AddressChanged;
                    }
                    else if (view is Entry)
                        (view as Entry).TextChanged += AddressChanged;
                // ... and the listbox...
                lstState.SelectedIndexChanged += LstState_SelectedIndexChanged;
                // ... except the description textbox, as it is unnecessary.
                txtDescription.TextChanged -= AddressChanged;
            }
            // If this was unchecked and the other two are also not checked, keep this one checked.
            else if (!chkHome.IsChecked && !chkOther.IsChecked)
                (sender as CheckBox).IsChecked = true;
        }

        /// <summary>
        /// This is called upon the user checking the "other" checkbox option.
        /// </summary>
        /// <param name="sender">The object that causes the event.</param>
        /// <param name="e">Event arguments.</param>
        private void ChkOther_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            // If this was checked, ensure the others are no longer checked.
            if (e.Value)
            {
                chkHome.IsChecked = false;
                chkChurch.IsChecked = false;
            }
            // If this was unchecked and the other two are also not checked, keep this one checked.
            else if (!chkChurch.IsChecked && !chkHome.IsChecked)
                (sender as CheckBox).IsChecked = true;
        }

        /// <summary>
        /// If any part of the address is changed, they are no longer using their default home or the default church address. This will uncheck those two options.
        /// </summary>
        /// <param name="sender">The object that causes the event.</param>
        /// <param name="e">Event arguments.</param>
        private void AddressChanged(object sender, TextChangedEventArgs e)
        {
            if (!(sender is CheckBox))
                chkOther.IsChecked = true;
        }
    }
}