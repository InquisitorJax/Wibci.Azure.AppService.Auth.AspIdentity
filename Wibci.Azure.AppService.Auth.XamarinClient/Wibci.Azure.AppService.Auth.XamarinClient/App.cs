using Wibci.Azure.AppService.Auth.XamarinClient.Pages;
using Xamarin.Forms;

namespace Wibci.Azure.AppService.Auth.XamarinClient
{
    public class App : Application
    {
        public App()
        {
            // The root page of your application
            MainPage = new AuthPage();
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }
    }
}