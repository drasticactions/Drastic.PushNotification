﻿// <copyright file="PushNotificationManager.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Text;
using UserNotifications;

namespace Drastic.PushNotification
{
    public static class HelperExtensions
    {
        public static string ToJson(this NSDictionary dictionary)
        {
            var json = NSJsonSerialization.Serialize(
                dictionary,
            NSJsonWritingOptions.SortedKeys, out NSError error);
            return json.ToString(NSStringEncoding.UTF8);
        }
    }

    /// <summary>
    /// Implementation for PushNotification.
    /// </summary>
    public class PushNotificationManager : NSObject, IPushNotification, IUNUserNotificationCenterDelegate
    {
        private const string TokenKey = "Token";
        private static NotificationResponse delayedNotificationResponse = null;

        private static PushNotificationTokenEventHandler onTokenRefresh;

        private static PushNotificationErrorEventHandler onNotificationError;

        private static PushNotificationResponseEventHandler onNotificationOpened;

        private static PushNotificationResponseEventHandler onNotificationAction;

        private static PushNotificationDataEventHandler onNotificationReceived;

        private static PushNotificationDataEventHandler onNotificationDeleted;

        private NSString notificationIdKey = new NSString("id");
        private NSString apsNotificationIdKey = new NSString("aps.id");

        private NSString notificationTagKey = new NSString("tag");
        private NSString apsNotificationTagKey = new NSString("aps.tag");

        public Func<string> RetrieveSavedToken { get; set; } = InternalRetrieveSavedToken;

        public Action<string> SaveToken { get; set; } = InternalSaveToken;

        public string Token
        {
            get
            {
                return this.RetrieveSavedToken?.Invoke() ?? string.Empty;
            }

            internal set
            {
                this.SaveToken?.Invoke(value);
            }
        }

        public IPushNotificationHandler NotificationHandler { get; set; }

        internal static string InternalRetrieveSavedToken()
        {
            return NSUserDefaults.StandardUserDefaults.StringForKey(TokenKey);
        }

        internal static void InternalSaveToken(string token)
        {
            NSUserDefaults.StandardUserDefaults.SetString(token, TokenKey);
        }

        public static UNNotificationPresentationOptions CurrentNotificationPresentationOption { get; set; } = UNNotificationPresentationOptions.None;

        private static IList<NotificationUserCategory> UsernNotificationCategories { get; } = new List<NotificationUserCategory>();

        public event PushNotificationTokenEventHandler OnTokenRefresh
        {
            add
            {
                onTokenRefresh += value;
            }

            remove
            {
                onTokenRefresh -= value;
            }
        }

        public event PushNotificationErrorEventHandler OnNotificationError
        {
            add
            {
                onNotificationError += value;
            }

            remove
            {
                onNotificationError -= value;
            }
        }

        public event PushNotificationResponseEventHandler OnNotificationOpened
        {
            add
            {
                var previousVal = onNotificationOpened;
                onNotificationOpened += value;
                if (delayedNotificationResponse != null && previousVal == null)
                {
                    var tmpParams = delayedNotificationResponse;
                    onNotificationOpened?.Invoke(CrossPushNotification.Current, new PushNotificationResponseEventArgs(tmpParams.Data, tmpParams.Identifier, tmpParams.Type));
                    delayedNotificationResponse = null;
                }
            }

            remove
            {
                onNotificationOpened -= value;
            }
        }

        public event PushNotificationResponseEventHandler OnNotificationAction
        {
            add
            {
                onNotificationAction += value;
            }

            remove
            {
                onNotificationAction -= value;
            }
        }

        public NotificationUserCategory[] GetUserNotificationCategories()
        {
            return UsernNotificationCategories?.ToArray();
        }

        public event PushNotificationDataEventHandler OnNotificationReceived
        {
            add
            {
                onNotificationReceived += value;
            }

            remove
            {
                onNotificationReceived -= value;
            }
        }

        public event PushNotificationDataEventHandler OnNotificationDeleted
        {
            add
            {
                onNotificationDeleted += value;
            }

            remove
            {
                onNotificationDeleted -= value;
            }
        }

        public static void Initialize(NSNotification notification, bool autoRegistration = true, bool enableDelayedResponse = true)
        {
            CrossPushNotification.Current.NotificationHandler = CrossPushNotification.Current.NotificationHandler ?? new DefaultPushNotificationHandler();

            if (notification != null && notification.UserInfo != null)
            {
                var parameters = GetParameters(notification.UserInfo);

                var notificationResponse = new NotificationResponse(parameters, string.Empty, NotificationCategoryType.Default);

                if (onNotificationOpened == null && enableDelayedResponse)
                {
                    delayedNotificationResponse = notificationResponse;
                }
                else
                {
                    onNotificationOpened?.Invoke(CrossPushNotification.Current, new PushNotificationResponseEventArgs(notificationResponse.Data, notificationResponse.Identifier, notificationResponse.Type));
                }

                CrossPushNotification.Current.NotificationHandler?.OnOpened(notificationResponse);
            }

            if (autoRegistration)
            {
                CrossPushNotification.Current.RegisterForPushNotifications();
            }
        }

        public static void Initialize(NSNotification notification, IPushNotificationHandler pushNotificationHandler, bool autoRegistration = true, bool enableDelayedResponse = true)
        {
            CrossPushNotification.Current.NotificationHandler = pushNotificationHandler;
            Initialize(notification, autoRegistration, enableDelayedResponse);
        }

        public static void Initialize(NSNotification notification, NotificationUserCategory[] notificationUserCategories, bool autoRegistration = true, bool enableDelayedResponse = true)
        {
            Initialize(notification, autoRegistration, enableDelayedResponse);
            RegisterUserNotificationCategories(notificationUserCategories);
        }

        public static void DidRegisterRemoteNotifications(NSData deviceToken)
        {
            var length = (int)deviceToken.Length;
            if (length == 0)
            {
                return;
            }

            var hex = new StringBuilder(length * 2);
            foreach (var b in deviceToken)
            {
                hex.AppendFormat("{0:x2}", b);
            }

            var cleanedDeviceToken = hex.ToString();
            InternalSaveToken(cleanedDeviceToken);
            onTokenRefresh?.Invoke(CrossPushNotification.Current, new PushNotificationTokenEventArgs(cleanedDeviceToken));
        }

        public void RegisterForPushNotifications()
        {
            // Register your app for remote notifications.
            var authOptions = UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound;

            UNUserNotificationCenter.Current.Delegate = CrossPushNotification.Current as IUNUserNotificationCenterDelegate;
            UNUserNotificationCenter.Current.RequestAuthorization(authOptions, (granted, error) =>
            {
                if (error != null)
                {
                    onNotificationError?.Invoke(CrossPushNotification.Current, new PushNotificationErrorEventArgs(PushNotificationErrorType.PermissionDenied, error.Description));
                }
                else if (!granted)
                {
                    onNotificationError?.Invoke(CrossPushNotification.Current, new PushNotificationErrorEventArgs(PushNotificationErrorType.PermissionDenied, "Push notification permission not granted"));
                }
                else
                {
                    this.InvokeOnMainThread(() => NSApplication.SharedApplication.RegisterForRemoteNotifications());
                }
            });
        }

        private static void RegisterUserNotificationCategories(NotificationUserCategory[] userCategories)
        {
            if (userCategories != null && userCategories.Length > 0)
            {
                UsernNotificationCategories.Clear();
                IList<UNNotificationCategory> categories = new List<UNNotificationCategory>();
                foreach (var userCat in userCategories)
                {
                    IList<UNNotificationAction> actions = new List<UNNotificationAction>();

                    foreach (var action in userCat.Actions)
                    {
                        // Create action
                        switch (action.Type)
                        {
                            case NotificationActionType.AuthenticationRequired:
                                actions.Add(UNNotificationAction.FromIdentifier(action.Id, action.Title, UNNotificationActionOptions.AuthenticationRequired));
                                break;
                            case NotificationActionType.Destructive:
                                actions.Add(UNNotificationAction.FromIdentifier(action.Id, action.Title, UNNotificationActionOptions.Destructive));
                                break;
                            case NotificationActionType.Foreground:
                                actions.Add(UNNotificationAction.FromIdentifier(action.Id, action.Title, UNNotificationActionOptions.Foreground));
                                break;
                            case NotificationActionType.Reply:
                                actions.Add(UNTextInputNotificationAction.FromIdentifier(action.Id, action.Title, UNNotificationActionOptions.None, action.Title, string.Empty));
                                break;
                        }
                    }

                    // Create category
                    var categoryID = userCat.Category;
                    var notificationActions = actions.ToArray() ?? new UNNotificationAction[] { };
                    var intentIDs = new string[] { };
                    var categoryOptions = new UNNotificationCategoryOptions[] { };

                    var category = UNNotificationCategory.FromIdentifier(categoryID, notificationActions, intentIDs, userCat.Type == NotificationCategoryType.Dismiss ? UNNotificationCategoryOptions.CustomDismissAction : UNNotificationCategoryOptions.None);
                    categories.Add(category);

                    UsernNotificationCategories.Add(userCat);
                }

                // Register categories
                UNUserNotificationCenter.Current.SetNotificationCategories(new NSSet<UNNotificationCategory>(categories.ToArray()));
            }
        }

        public void UnregisterForPushNotifications()
        {
            NSApplication.SharedApplication.UnregisterForRemoteNotifications();
            this.Token = string.Empty;
        }

        [Export("userNotificationCenter:willPresentNotification:withCompletionHandler:")]
        public void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
        {
            // Do your magic to handle the notification data
            System.Console.WriteLine(notification.Request.Content.UserInfo);
            System.Diagnostics.Debug.WriteLine("WillPresentNotification");
            var parameters = GetParameters(notification.Request.Content.UserInfo);
            onNotificationReceived?.Invoke(CrossPushNotification.Current, new PushNotificationDataEventArgs(parameters));
            CrossPushNotification.Current.NotificationHandler?.OnReceived(parameters);

            string[] priorityKeys = new string[] { "priority", "aps.priority" };

            foreach (var pKey in priorityKeys)
            {
                if (parameters.TryGetValue(pKey, out object priority))
                {
                    var priorityValue = $"{priority}".ToLower();
                    switch (priorityValue)
                    {
                        case "max":
                        case "high":
                            if (!CurrentNotificationPresentationOption.HasFlag(UNNotificationPresentationOptions.Alert))
                            {
                                CurrentNotificationPresentationOption |= UNNotificationPresentationOptions.Alert;
                            }

                            if (!CurrentNotificationPresentationOption.HasFlag(UNNotificationPresentationOptions.Sound))
                            {
                                CurrentNotificationPresentationOption |= UNNotificationPresentationOptions.Sound;
                            }

                            break;
                        case "low":
                        case "min":
                        case "default":
                        default:
                            if (CurrentNotificationPresentationOption.HasFlag(UNNotificationPresentationOptions.Alert))
                            {
                                CurrentNotificationPresentationOption &= ~UNNotificationPresentationOptions.Alert;
                            }

                            break;
                    }

                    break;
                }
            }

            completionHandler(CurrentNotificationPresentationOption);
        }

        [Export("userNotificationCenter:didReceiveNotificationResponse:withCompletionHandler:")]
        public void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler)
        {
            var parameters = GetParameters(response.Notification.Request.Content.UserInfo);
            string? result = null;
            NotificationCategoryType catType = NotificationCategoryType.Default;
            if (response.IsCustomAction)
            {
                catType = NotificationCategoryType.Custom;
            }
            else if (response.IsDismissAction)
            {
                catType = NotificationCategoryType.Dismiss;
            }

            if (response is UNTextInputNotificationResponse textResponse)
            {
                result = textResponse.UserText;
            }

            var notificationResponse = new NotificationResponse(parameters, $"{response.ActionIdentifier}".Equals("com.apple.UNNotificationDefaultActionIdentifier", StringComparison.CurrentCultureIgnoreCase) ? string.Empty : $"{response.ActionIdentifier}", catType, result);

            onNotificationAction?.Invoke(this, new PushNotificationResponseEventArgs(notificationResponse.Data, notificationResponse.Identifier, notificationResponse.Type, result));

            CrossPushNotification.Current.NotificationHandler?.OnAction(notificationResponse);

            // Inform caller it has been handled
            completionHandler();
        }

        public static void DidReceiveMessage(NSDictionary data)
        {
            var parameters = GetParameters(data);

            onNotificationReceived?.Invoke(CrossPushNotification.Current, new PushNotificationDataEventArgs(parameters));

            CrossPushNotification.Current.NotificationHandler?.OnReceived(parameters);
            System.Diagnostics.Debug.WriteLine("DidReceivedMessage");
        }

        public static void RemoteNotificationRegistrationFailed(NSError error)
        {
            onNotificationError?.Invoke(CrossPushNotification.Current, new PushNotificationErrorEventArgs(PushNotificationErrorType.RegistrationFailed, error.Description));
        }

        public void ClearAllNotifications()
        {
            UNUserNotificationCenter.Current.RemoveAllDeliveredNotifications();
        }

        private static IDictionary<string, object> GetParameters(NSDictionary data)
        {
            var parameters = new Dictionary<string, object>();

            var keyAps = new NSString("aps");
            var keyAlert = new NSString("alert");

            foreach (var val in data)
            {
                if (val.Key.Equals(keyAps))
                {
                    if (data.ValueForKey(keyAps) is NSDictionary aps)
                    {
                        foreach (var apsVal in aps)
                        {
                            if (apsVal.Value is NSDictionary apsValDict)
                            {
                                if (apsVal.Key.Equals(keyAlert))
                                {
                                    foreach (var alertVal in apsValDict)
                                    {
                                        if (alertVal.Value is NSDictionary valDict)
                                        {
                                            var value = valDict.ToJson();
                                            parameters.Add($"aps.alert.{alertVal.Key}", value);
                                        }
                                        else
                                        {
                                            parameters.Add($"aps.alert.{alertVal.Key}", $"{alertVal.Value}");
                                        }
                                    }
                                }
                                else
                                {
                                    var value = apsValDict.ToJson();
                                    parameters.Add($"aps.{apsVal.Key}", value);
                                }
                            }
                            else
                            {
                                parameters.Add($"aps.{apsVal.Key}", $"{apsVal.Value}");
                            }
                        }
                    }
                }
                else if (val.Value is NSDictionary valDict)
                {
                    var value = valDict.ToJson();
                    parameters.Add($"{val.Key}", value);
                }
                else
                {
                    parameters.Add($"{val.Key}", $"{val.Value}");
                }
            }

            return parameters;
        }

        public async void RemoveNotification(int id)
        {
            var deliveredNotifications = await UNUserNotificationCenter.Current.GetDeliveredNotificationsAsync();
            var deliveredNotificationsMatches = deliveredNotifications.Where(u => (u.Request.Content.UserInfo.ContainsKey(this.notificationIdKey) && $"{u.Request.Content.UserInfo[this.notificationIdKey]}".Equals($"{id}")) || (u.Request.Content.UserInfo.ContainsKey(this.apsNotificationIdKey) && u.Request.Content.UserInfo[this.apsNotificationIdKey].Equals($"{id}"))).Select(s => s.Request.Identifier).ToArray();
            if (deliveredNotificationsMatches.Length > 0)
            {
                UNUserNotificationCenter.Current.RemoveDeliveredNotifications(deliveredNotificationsMatches);
            }
        }

        public async void RemoveNotification(string tag, int id)
        {
            if (string.IsNullOrEmpty(tag))
            {
                this.RemoveNotification(id);
            }
            else
            {
                var deliveredNotifications = await UNUserNotificationCenter.Current.GetDeliveredNotificationsAsync();
                var deliveredNotificationsMatches = deliveredNotifications.Where(u => (u.Request.Content.UserInfo.ContainsKey(this.notificationIdKey) && $"{u.Request.Content.UserInfo[this.notificationIdKey]}".Equals($"{id}") && u.Request.Content.UserInfo.ContainsKey(this.notificationTagKey) && u.Request.Content.UserInfo[this.notificationTagKey].Equals(tag)) || (u.Request.Content.UserInfo.ContainsKey(this.apsNotificationIdKey) && u.Request.Content.UserInfo[this.apsNotificationIdKey].Equals($"{id}") && u.Request.Content.UserInfo.ContainsKey(this.apsNotificationTagKey) && u.Request.Content.UserInfo[this.apsNotificationTagKey].Equals(tag))).Select(s => s.Request.Identifier).ToArray();
                if (deliveredNotificationsMatches.Length > 0)
                {
                    UNUserNotificationCenter.Current.RemoveDeliveredNotifications(deliveredNotificationsMatches);
                }
            }
        }
    }
}