using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PineGroveMobileApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NoInternetPage : ContentPage
    {
        // This is a simple page to let the user know that there is no internet and, therefore,
        // they can't really use the application for anything as everything about the application
        // is designed to query a remote database through an API (AKA you need internet to do anything).
        /// <summary>
        /// This is the page that is shown if the user does not have an internet connection available upon launch.
        /// </summary>
        public NoInternetPage()
        { InitializeComponent(); }

        /// <summary>
        /// This is called when the size of the screen is changed.
        /// </summary>
        /// <param name="width">The new width of the screen.</param>
        /// <param name="height">The new height of the screen.</param>
        protected override void OnSizeAllocated(double width, double height)
        {
            if (width > height)
                imgLogo.HeightRequest = 50;
            else
                imgLogo.HeightRequest = 75;
            base.OnSizeAllocated(width, height);
        }
    }
}