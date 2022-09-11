using Drastic.PushNotification;
using Foundation;
using UIKit;

namespace MauiSample;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
    {
        PushNotificationManager.Initialize(launchOptions, true);
        return base.FinishedLaunching(application, launchOptions);
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    [Export("application:didRegisterForRemoteNotificationsWithDeviceToken:")]
    public void RegisteredForRemoteNotifications(UIKit.UIApplication application, NSData deviceToken)
    {
        PushNotificationManager.DidRegisterRemoteNotifications(deviceToken);
    }

    [Export("application:didFailToRegisterForRemoteNotificationsWithError:")]
    public void FailedToRegisterForRemoteNotifications(UIKit.UIApplication application, NSError error)
    {
        PushNotificationManager.RemoteNotificationRegistrationFailed(error);
    }

    [Export("application:didReceiveRemoteNotification:fetchCompletionHandler:")]
    public void DidReceiveRemoteNotification(UIKit.UIApplication application, NSDictionary userInfo, Action<UIKit.UIBackgroundFetchResult> completionHandler)
    {
        PushNotificationManager.DidReceiveMessage(userInfo);
    }
}
