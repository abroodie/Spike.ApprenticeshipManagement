using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Plugin.Firebase.CloudMessaging;

namespace DASApprenticeManagementPOC
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            FirebaseCloudMessagingImplementation.OnNewIntent(Intent);
            CreateNotificationChannel();
        }

        protected override void OnNewIntent(Intent? intent)
        {
            base.OnNewIntent(intent);
            FirebaseCloudMessagingImplementation.OnNewIntent(Intent);
        }

        private void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt <= BuildVersionCodes.Base)
                return;

            var channelId = $"{PackageName}.general";
            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            var channel = new NotificationChannel(channelId, "General", NotificationImportance.Default);
            notificationManager.CreateNotificationChannel(channel);
            FirebaseCloudMessagingImplementation.ChannelId = channelId;
            //FirebaseCloudMessagingImplementation.SmallIconRef = Resource.Drawable.ic_push_small;

        }

    }
}
