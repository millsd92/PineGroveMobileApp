﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using PineGroveMobileApp.Services;
using Acr.UserDialogs;
using System.Threading;
using System.Text.RegularExpressions;

namespace PineGroveMobileApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LookupPage : ContentPage
    {
        private double width = 0, height = 0;
        private RestClient client;
        private Models.User user;
        public LookupPage(ref RestClient client)
        {
            this.client = client;
            InitializeComponent();
            txtFirstName.TextChanged += TxtName_Changed;
            txtLastName.TextChanged += TxtName_Changed;
            txtFirstName.Keyboard = Keyboard.Create(KeyboardFlags.CapitalizeWord);
            txtLastName.Keyboard = Keyboard.Create(KeyboardFlags.CapitalizeWord);
            txtFirstName.ReturnCommand = new Command(() => txtLastName.Focus());
            txtLastName.ReturnCommand = new Command(() => BtnSearch_Clicked(null, null));
        }

        private void TxtName_Changed(object sender, TextChangedEventArgs e)
        {
            btnSearch.Text = "Search";
            lblEmail.Text = "N/A";
            lblPhone.Text = "N/A";
            lblUsername.Text = "N/A";
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            if (this.width != width || this.height != height)
            {
                this.width = width;
                this.height = height;
                if (width > height)
                    LandscapeOrientation();
                else
                    PortraitOrientation();
            }
            base.OnSizeAllocated(width, height);
        }

        private void LandscapeOrientation()
        {
            grdAll.Children.Clear();
            imgLogo.HeightRequest = 50;
            grdAll.Padding = 5;
            grdAll.Children.Add(lblLookupTitle, 0, 0);
            Grid.SetColumnSpan(lblLookupTitle, 2);
            grdAll.Children.Add(lblLookupPrompt, 0, 1);
            Grid.SetColumnSpan(lblLookupPrompt, 2);
            grdAll.Children.Add(txtFirstName, 0, 2);
            Grid.SetRowSpan(txtFirstName, 2);
            grdAll.Children.Add(txtLastName, 1, 2);
            Grid.SetRowSpan(txtLastName, 2);
            grdAll.Children.Add(btnSearch, 0, 4);
            Grid.SetColumnSpan(btnSearch, 2);
            grdAll.Children.Add(lblInfoTitle, 2, 0);
            Grid.SetColumnSpan(lblInfoTitle, 2);
            grdAll.Children.Add(lblUsernamePrompt, 2, 1);
            grdAll.Children.Add(lblUsername, 3, 1);
            grdAll.Children.Add(lblPhonePrompt, 2, 2);
            grdAll.Children.Add(lblPhone, 3, 2);
            grdAll.Children.Add(lblEmailPrompt, 2, 3);
            grdAll.Children.Add(lblEmail, 3, 3);
            grdAll.Children.Add(btnLogin, 2, 4);
            Grid.SetColumnSpan(btnLogin, 2);
        }

        private void PortraitOrientation()
        {
            grdAll.Children.Clear();
            imgLogo.HeightRequest = 75;
            grdAll.Padding = 25;
            grdAll.Children.Add(lblLookupTitle, 0, 0);
            Grid.SetColumnSpan(lblLookupTitle, 2);
            grdAll.Children.Add(lblLookupPrompt, 0, 1);
            Grid.SetColumnSpan(lblLookupPrompt, 2);
            grdAll.Children.Add(txtFirstName, 0, 2);
            grdAll.Children.Add(txtLastName, 1, 2);
            grdAll.Children.Add(btnSearch, 0, 3);
            Grid.SetColumnSpan(btnSearch, 2);
            grdAll.Children.Add(lblInfoTitle, 0, 4);
            Grid.SetColumnSpan(lblInfoTitle, 2);
            grdAll.Children.Add(lblUsernamePrompt, 0, 5);
            grdAll.Children.Add(lblUsername, 1, 5);
            grdAll.Children.Add(lblPhonePrompt, 0, 6);
            grdAll.Children.Add(lblPhone, 1, 6);
            grdAll.Children.Add(lblEmailPrompt, 0, 7);
            grdAll.Children.Add(lblEmail, 1, 7);
            grdAll.Children.Add(btnLogin, 0, 8);
            Grid.SetColumnSpan(btnLogin, 2);
        }


        private async void BtnSearch_Clicked(object sender, EventArgs e)
        {
            ToastConfig config = new ToastConfig("") { BackgroundColor = App.toastColor };
            string lastName = "", firstName = "";
            if (txtFirstName.Text == null && txtLastName.Text == null)
            {
                config.Message = "Error! No text was entered for either the first or last name!";
                UserDialogs.Instance.Toast(config);
            }
            else
            {
                config.Message = "Searching...";
                config.Duration = TimeSpan.FromMilliseconds(App.timeoutTime);
                UserDialogs.Instance.Toast(config);
                if (txtLastName.Text != null)
                    lastName = txtLastName.Text.TrimEnd();
                if (txtFirstName.Text != null)
                    firstName = txtFirstName.Text.TrimEnd();
                if (lastName.Length == 0 && firstName.Length == 0)
                    UserDialogs.Instance.Toast(new ToastConfig("Error! No text was entered for either first or last name!") { BackgroundColor = App.toastColor });
                else
                {
                    try
                    {
                        CancellationTokenSource tokenSource = new CancellationTokenSource();
                        tokenSource.CancelAfter((int)App.timeoutTime);
                        Models.User[] users = await client.GetUsersByName(firstName, lastName, tokenSource.Token);
                        if (users.Length == 1)
                        {
                            config.Message = "Only one user found!";
                            config.Duration = TimeSpan.FromSeconds(2);
                            UserDialogs.Instance.Toast(config);
                            user = users[0];
                            if (user.EmailAddress == null)
                                lblEmail.Text = "Not provided";
                            else
                            {
                                string pattern = @"(?<=[\w]{1})[\w-\._\+%]*(?=[\w]{2}@)";
                                lblEmail.Text = Regex.Replace(user.EmailAddress, pattern, m => new string('*', m.Length));
                            }
                            if (user.PhoneNumber == null)
                                lblPhone.Text = "Not provided";
                            else
                            {
                                lblPhone.Text = "(XXX) XXX-" + (user.PhoneNumber % 10000).ToString();
                            }
                            lblUsername.Text = user.UserName;
                        }
                        else
                        {
                            //Handle if more than one user
                        }
                        btnSearch.Text = "That's Not Me!";
                    }
                    catch (Refit.ValidationApiException)
                    {
                        config.Message = "Error! No users found!";
                        config.Duration = TimeSpan.FromSeconds(2);
                        UserDialogs.Instance.Toast(config);
                    }
                    catch (Refit.ApiException)
                    {
                        config.Message = "Error! No users found!";
                        config.Duration = TimeSpan.FromSeconds(2);
                        UserDialogs.Instance.Toast(config);
                    }
                    catch (System.Net.Http.HttpRequestException)
                    {
                        Timer_Elapsed("Access denied! Connection blocked.\nWould you like to browse offline?");
                    }
                    catch (TaskCanceledException)
                    {
                        Timer_Elapsed("Request timeout...\nWould you like to browse offline?");
                    }
                }
            }
        }

        private async void BtnLogin_Clicked(object sender, EventArgs e)
        {
            if (lblUsername.Text.Equals("N/A"))
                UserDialogs.Instance.Toast(new ToastConfig("Error! No user selected!") { BackgroundColor = App.toastColor });
            else
            {
                ToastConfig config = new ToastConfig("Successfully logged in as " + user.FirstName + " " + user.LastName + "!")
                {
                    Duration = TimeSpan.FromSeconds(2)
                };
                UserDialogs.Instance.Toast(config);
                Application.Current.Properties["Username"] = user.UserName;
                await Application.Current.SavePropertiesAsync();
                Application.Current.MainPage = new MainPage(ref client);
            }
        }

        private void Timer_Elapsed(string message)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (await DisplayAlert("Error!", message, "Yes", "Try Again"))
                    Application.Current.MainPage = new MainPage(ref client);
            });
        }
    }
}