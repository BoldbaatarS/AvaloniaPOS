#if IOS
using Avalonia;
using Avalonia.iOS;
using Foundation;
using UIKit;

namespace RestaurantPOS;

[Register("AppDelegate")]
public class AppDelegate : AvaloniaAppDelegate<App>
{
}
#endif
