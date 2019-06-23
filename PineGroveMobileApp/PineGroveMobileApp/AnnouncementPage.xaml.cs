using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PineGroveMobileApp.Services;
using PineGroveMobileApp.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PineGroveMobileApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AnnouncementPage : ContentPage
    {
        private readonly RestClient client;
        public AnnouncementPage(ref RestClient client)
        {
            InitializeComponent();
            this.client = client;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            User[] rawUsers = await client.GetUsers();
            listviewMain.ItemsSource = rawUsers;
        }
    }
}