using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using static Microsoft.Maui.ApplicationModel.Platform;
using Intent = Android.Content.Intent;

namespace GateCaller;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    public static void CallPhone(string number)
    {
        var status = CheckAndRequestCallPermission();
        if (status != true) return;
        Intent callIntent = new(Intent.ActionCall);
        callIntent.SetData(Android.Net.Uri.Parse("tel:" + number));
        callIntent.AddFlags(ActivityFlags.NewTask);
        CurrentActivity.StartActivity(callIntent);
    }

    private static bool CheckAndRequestCallPermission()
    {
        if (ContextCompat.CheckSelfPermission(CurrentActivity.ApplicationContext, Manifest.Permission.CallPhone) != Permission.Granted)
        {
            ActivityCompat.RequestPermissions(CurrentActivity, new string[] { Manifest.Permission.CallPhone }, 1);
            return ContextCompat.CheckSelfPermission(CurrentActivity.ApplicationContext, Manifest.Permission.CallPhone) == Permission.Granted;
        }
        else
        {
            return true;
        }
    }

    public static bool CheckAndRequestForLocPermission()
    {
        if (ContextCompat.CheckSelfPermission(CurrentActivity.ApplicationContext,
                Manifest.Permission.AccessFineLocation) != Permission.Granted)
        {
            ActivityCompat.RequestPermissions(CurrentActivity, new string[] { Manifest.Permission.AccessFineLocation }, 1);
            return ContextCompat.CheckSelfPermission(CurrentActivity.ApplicationContext,
                Manifest.Permission.AccessFineLocation) != Permission.Granted;
        }
        else
        {
            return true;
        }
    }
}
