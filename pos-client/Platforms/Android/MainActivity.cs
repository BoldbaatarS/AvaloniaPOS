#if ANDROID
using Avalonia;
using Avalonia.Android;
using Android.App;
using Android.Content.PM;

namespace RestaurantPOS;

[Activity(Label = "RestaurantPOS",
    Theme = "@style/AvaloniaMainTheme",
    Icon = "@drawable/icon",
    LaunchMode = LaunchMode.SingleTop,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
public class MainActivity : AvaloniaMainActivity<App>
{
}
#endif
