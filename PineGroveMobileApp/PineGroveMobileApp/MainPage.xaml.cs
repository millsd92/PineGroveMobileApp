﻿using System;
using System.ComponentModel;
using Xamarin.Forms;
using PineGroveMobileApp.Services;

namespace PineGroveMobileApp
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private double width = 0;
        private double height = 0;
        private RestClient client;

        public MainPage(ref RestClient client)
        {
            InitializeComponent();
            this.client = client;
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            if (this.width != width || this.height != height)
            {
                this.width = width;
                this.height = height;
                if (width > height)
                {
                    MainStack.Orientation = StackOrientation.Horizontal;
                    RightBorder.IsVisible = true;
                }
                else
                {
                    MainStack.Orientation = StackOrientation.Vertical;
                    RightBorder.IsVisible = false;
                }
            }
            base.OnSizeAllocated(width, height);
        }

        private async void BtnAnnouncement_Clicked(object sender, EventArgs e)
        { await Navigation.PushModalAsync(new AnnouncementPage(ref client)); }

        private void BtnEvent_Clicked(object sender, EventArgs e)
        {

        }

        private void BtnRegister_Clicked(object sender, EventArgs e)
        {
            Application.Current.Properties.Remove("Username");
            Application.Current.MainPage = new LoginPage(ref client);
        }

        private void BtnRequest_Clicked(object sender, EventArgs e)
        {

        }

        private void BtnAnnouncement_Pressed(object sender, EventArgs e)
        {
            Device.StartTimer(TimeSpan.FromMilliseconds(1), () => 
            {
                return (BtnEffect(imgAnnouncement, true) && ((Button)sender).IsPressed);
            });
        }

        private void BtnAnnouncement_Released(object sender, EventArgs e)
        {
            Device.StartTimer(TimeSpan.FromMilliseconds(.5), () =>
            {
                return BtnEffect(imgAnnouncement, false);
            });
        }

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
                if (img.Opacity > .25)
                {
                    img.Opacity -= .05f;
                    return true;
                }
                else
                    return false;
            }
        }

        private void BtnRegister_Pressed(object sender, EventArgs e)
        {
            Device.StartTimer(TimeSpan.FromMilliseconds(1), () =>
            {
                return (BtnEffect(imgRegister, true) && ((Button)sender).IsPressed);
            });
        }

        private void BtnRegister_Released(object sender, EventArgs e)
        {
            Device.StartTimer(TimeSpan.FromMilliseconds(.5), () =>
            {
                return BtnEffect(imgRegister, false);
            });
        }

        private void BtnRequest_Pressed(object sender, EventArgs e)
        {
            Device.StartTimer(TimeSpan.FromMilliseconds(1), () =>
            {
                return (BtnEffect(imgRequest, true) && ((Button)sender).IsPressed);
            });
        }

        private void BtnRequest_Released(object sender, EventArgs e)
        {
            Device.StartTimer(TimeSpan.FromMilliseconds(.5), () =>
            {
                return BtnEffect(imgRequest, false);
            });
        }

        private void BtnEvent_Pressed(object sender, EventArgs e)
        {
            Device.StartTimer(TimeSpan.FromMilliseconds(1), () =>
            {
                return (BtnEffect(imgEvent, true) && ((Button)sender).IsPressed);
            });
        }

        private void BtnEvent_Released(object sender, EventArgs e)
        {
            Device.StartTimer(TimeSpan.FromMilliseconds(.5), () =>
            {
                return BtnEffect(imgEvent, false);
            });
        }
    }
}
