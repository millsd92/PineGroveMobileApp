using Xamarin.Forms;
using Xamarin.Essentials;
using PineGroveMobileApp.Services;

namespace PineGroveMobileApp
{
    // This is the entry-point of the application.
    public partial class App : Application
    {
        // Defining some constants and whatnot.
        private readonly RestClient client;
        public static readonly System.Drawing.Color toastColor = System.Drawing.Color.FromArgb(51, 51, 51);
        public static readonly double timeoutTime = 15000;

        /// <summary>
        /// The entry-point of the application and where local storage access lies.
        /// </summary>
        public App()
        {
            InitializeComponent();
            // If we have internet, we can use the app.
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                client = new RestClient();
                // Warm up the API to help prevent harsh "cold start" time. The API is stored using a AWS Lambda function that is called by AWS API Gateway,
                // but because I am accessing a database being stored by AWS Relational Database Storage, I have to use a virtual private cloud to access
                // the database... When using a VPC this way, there is a long start time for the first time the API is called as it has to set up the elastic
                // network, create an instance of the Lambda function, and then finally connect to the database the first time.

                // I combat this by using a CloudWatch function that is called every five minutes to ping the AWS Lambda function, but this only goes so far.
                // If multiple people were to call the function at the same time, there would still be a harsh cold start time for the second user. To also aid
                // in combating this issue, I make a call to the API here (a throwaway call) just to help minimize the lengthy startup time for the Lambda call.

                // There is still a chance that the user will time out the first request, however. In true production environments, I would A) not use AWS but
                // rather use Azure, B) if I had to use AWS I would ping the Lambda more frequently and simultaneously at peak usage times to help with this,
                // and C) not use C#... C# Lambda functions take drastically longer than Python, JavaScript, Java, Go, or Ruby Lambda functions do.
                _ = client.GetUser("none", new System.Threading.CancellationToken());
                // If the user has already logged in, keep them logged in on start.
                if (Properties.ContainsKey("Username"))
                    MainPage = new MainPage(ref client);
                // Otherwise, they need to login in order to do pretty much anything...
                else
                    MainPage = new LoginPage(ref client);
            }
            // If we have no internet, we can't really do anything...
            else
                MainPage = new NoInternetPage();
        }
    }
}
