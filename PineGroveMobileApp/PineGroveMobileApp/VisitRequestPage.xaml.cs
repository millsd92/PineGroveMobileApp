using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Acr.UserDialogs;

namespace PineGroveMobileApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VisitRequestPage : ContentPage
    {
        private readonly Services.RestClient client;
        private Models.User currentUser = null;
        public VisitRequestPage(ref Services.RestClient client)
        {
            this.client = client;
            InitializeComponent();
            SetUpPage();
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

            grdAddressPrompt.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
            grdAddressPrompt.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            grdAddressPrompt.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
            grdAddressPrompt.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            grdAddressPrompt.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
            grdAddressPrompt.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            grdAddressPrompt.Children.Add(chkHome, 0, 0);
            grdAddressPrompt.Children.Add(lblHome, 1, 0);
            grdAddressPrompt.Children.Add(chkChurch, 2, 0);
            grdAddressPrompt.Children.Add(lblChurch, 3, 0);
            grdAddressPrompt.Children.Add(chkOther, 4, 0);
            grdAddressPrompt.Children.Add(lblOther, 5, 0);

            grdAddress.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            grdAddress.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
            grdAddress.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });

            grdAddress.Children.Add(txtCity, 0, 0);
            grdAddress.Children.Add(lstState, 1, 0);
            grdAddress.Children.Add(txtZip, 2, 0);

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

        private async void BtnSubmit_Clicked(object sender, EventArgs e)
        {
            if (!Application.Current.Properties.ContainsKey("Username"))
                UserDialogs.Instance.Toast(new ToastConfig("User not logged in! Please log in and try again!") { BackgroundColor = App.toastColor });
            else if (txtDescription.Text is null || txtDescription.Text.Equals(string.Empty))
                UserDialogs.Instance.Toast(new ToastConfig("Error! Description is empty!") { BackgroundColor = App.toastColor });
            else
            {
                UserDialogs.Instance.Toast(new ToastConfig("Creating visit request...") { BackgroundColor = App.toastColor });
                Models.VisitRequest visitRequest = new Models.VisitRequest();
                if (txtAddressLineOne.Text == null || txtAddressLineOne.Text.Equals(string.Empty))
                {
                    UserDialogs.Instance.Toast(new ToastConfig("Address line one is empty or invalid!") { BackgroundColor = App.toastColor });
                    return;
                }
                visitRequest.AddressLineOne = txtAddressLineOne.Text;
                if (txtAddressLineTwo.Text == null || txtAddressLineTwo.Text.Equals(string.Empty))
                    visitRequest.AddressLineTwo = null;
                else
                    visitRequest.AddressLineTwo = txtAddressLineTwo.Text;
                if (txtCity.Text == null || txtCity.Text.Equals(string.Empty))
                {
                    UserDialogs.Instance.Toast(new ToastConfig("City is empty or invalid!") { BackgroundColor = App.toastColor });
                    return;
                }
                else
                    visitRequest.City = txtCity.Text;
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
                    visitRequest.ZipCode = txtZip.Text;
                if (lstState.SelectedIndex == -1)
                {
                    UserDialogs.Instance.Toast(new ToastConfig("State is not selected!") { BackgroundColor = App.toastColor });
                    return;
                }
                else
                    visitRequest.State = lstState.Items[lstState.SelectedIndex];
                System.Threading.CancellationTokenSource source = new System.Threading.CancellationTokenSource();
                source.CancelAfter((int)App.timeoutTime);
                visitRequest.Reason = txtDescription.Text;
                visitRequest.RequestDate = DateTime.Now;
                if (currentUser is null)
                {
                    try
                    {
                        UserDialogs.Instance.Toast(new ToastConfig("Fetching user details...") { BackgroundColor = App.toastColor, Duration = TimeSpan.FromMilliseconds(App.timeoutTime) });
                        currentUser = await client.GetUser(Application.Current.Properties["Username"].ToString(), source.Token);
                        UserDialogs.Instance.Toast(new ToastConfig("Fetched user details!") { BackgroundColor = App.toastColor });
                    }
                    catch (Refit.ValidationApiException)
                    {
                        UserDialogs.Instance.Toast(new ToastConfig("Database error! Database may be offline. Try again later.") { BackgroundColor = App.toastColor });
                        return;
                    }
                    catch (Refit.ApiException)
                    {
                        UserDialogs.Instance.Toast(new ToastConfig("User not found in database! Database may be offline. Try again later.") { BackgroundColor = App.toastColor });
                        return;
                    }
                    catch (System.Net.Http.HttpRequestException)
                    {
                        UserDialogs.Instance.Toast(new ToastConfig("Connection blocked! You may have restricted or no internet access! Try again later.") { BackgroundColor = App.toastColor });
                        return;
                    }
                    catch (TaskCanceledException)
                    {
                        UserDialogs.Instance.Toast(new ToastConfig("Request timed out. Try again later.") { BackgroundColor = App.toastColor });
                        return;
                    }
                }
                visitRequest.UserId = currentUser.UserId;
                visitRequest.Visited = false;
                visitRequest.VisitDate = null;
                try
                {
                    UserDialogs.Instance.Toast(new ToastConfig("Posting visit request...") { BackgroundColor = App.toastColor, Duration = TimeSpan.FromMilliseconds(App.timeoutTime) });
                    await client.CreateVisitRequest(visitRequest, source.Token);
                    UserDialogs.Instance.Toast(new ToastConfig("Visit request successfully posted!") { BackgroundColor = App.toastColor });
                    BtnClear_Clicked(sender, e);
                }
                catch (Refit.ValidationApiException)
                {
                    UserDialogs.Instance.Toast(new ToastConfig("Database error! Database may be offline. Try again later.") { BackgroundColor = App.toastColor });
                    return;
                }
                catch (Refit.ApiException)
                {
                    UserDialogs.Instance.Toast(new ToastConfig("User not found in database! Database may be offline. Try again later.") { BackgroundColor = App.toastColor });
                    return;
                }
                catch (System.Net.Http.HttpRequestException)
                {
                    UserDialogs.Instance.Toast(new ToastConfig("Connection blocked! You may have restricted or no internet access! Try again later.") { BackgroundColor = App.toastColor });
                    return;
                }
                catch (TaskCanceledException)
                {
                    UserDialogs.Instance.Toast(new ToastConfig("Request timed out. Try again later.") { BackgroundColor = App.toastColor });
                    return;
                }
            }
        }

        private void BtnClear_Clicked(object sender, EventArgs e)
        {
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

        private async void ChkHome_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (e.Value)
            {
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
                chkChurch.IsChecked = false;
                chkOther.IsChecked = false;
                System.Threading.CancellationTokenSource source = new System.Threading.CancellationTokenSource();
                source.CancelAfter((int)App.timeoutTime);
                if (currentUser is null && Application.Current.Properties.ContainsKey("Username"))
                {
                    try
                    {
                        UserDialogs.Instance.Toast(new ToastConfig("Fetching user details...") { BackgroundColor = App.toastColor, Duration = TimeSpan.FromMilliseconds(App.timeoutTime) });
                        currentUser = await client.GetUser(Application.Current.Properties["Username"].ToString(), source.Token);
                        if (currentUser.AddressLineOne is null)
                            UserDialogs.Instance.Toast(new ToastConfig("No address on file for user!") { BackgroundColor = App.toastColor });
                        else
                        {
                            txtAddressLineOne.Text = currentUser.AddressLineOne;
                            txtAddressLineTwo.Text = currentUser.AddressLineTwo;
                            txtCity.Text = currentUser.City;
                            txtZip.Text = currentUser.ZipCode;
                            lstState.SelectedIndex = lstState.Items.IndexOf(currentUser.State);
                            UserDialogs.Instance.Toast(new ToastConfig("Fetched user address!") { BackgroundColor = App.toastColor });
                        }
                    }
                    catch (Refit.ValidationApiException)
                    {
                        UserDialogs.Instance.Toast(new ToastConfig("Database error! Database may be offline. Try again later.") { BackgroundColor = App.toastColor });
                    }
                    catch (Refit.ApiException)
                    {
                        UserDialogs.Instance.Toast(new ToastConfig("User not found in database! Database may be offline. Try again later.") { BackgroundColor = App.toastColor });
                    }
                    catch (System.Net.Http.HttpRequestException)
                    {
                        UserDialogs.Instance.Toast(new ToastConfig("Connection blocked! You may have restricted or no internet access! Try again later.") { BackgroundColor = App.toastColor });
                    }
                    catch (TaskCanceledException)
                    {
                        UserDialogs.Instance.Toast(new ToastConfig("Request timed out. Try again later.") { BackgroundColor = App.toastColor });
                    }
                }
                else if (!(currentUser is null))
                {
                    txtAddressLineOne.Text = currentUser.AddressLineOne;
                    txtAddressLineTwo.Text = currentUser.AddressLineTwo;
                    txtCity.Text = currentUser.City;
                    txtZip.Text = currentUser.ZipCode;
                    lstState.SelectedIndex = lstState.Items.IndexOf(currentUser.State);
                }
                else
                {
                    UserDialogs.Instance.Toast(new ToastConfig("Warning! User not logged in. Please log in before submitting request.") { BackgroundColor = App.toastColor });
                }
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
            else if (!chkChurch.IsChecked && !chkOther.IsChecked)
                (sender as CheckBox).IsChecked = true;
        }

        private void ChkChurch_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (e.Value)
            {
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
                chkHome.IsChecked = false;
                chkOther.IsChecked = false;
                txtAddressLineOne.Text = "1018 Piney Grove Rd.";
                txtAddressLineTwo.Text = null;
                txtCity.Text = "Kernersville";
                txtZip.Text = "27284";
                lstState.SelectedIndex = lstState.Items.IndexOf("NC");
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
            else if (!chkHome.IsChecked && !chkOther.IsChecked)
                (sender as CheckBox).IsChecked = true;
        }

        private void ChkOther_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (e.Value)
            {
                chkHome.IsChecked = false;
                chkChurch.IsChecked = false;
            }
            else if (!chkChurch.IsChecked && !chkHome.IsChecked)
                (sender as CheckBox).IsChecked = true;
        }

        private void AddressChanged(object sender, TextChangedEventArgs e)
        {
            if (!(sender is CheckBox))
                chkOther.IsChecked = true;
            System.Diagnostics.Debug.WriteLine(sender.ToString());
        }
    }
}