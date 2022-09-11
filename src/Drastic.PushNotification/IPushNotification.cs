// <copyright file="IPushNotification.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;

namespace Drastic.PushNotification
{
    public delegate void PushNotificationTokenEventHandler(object source, PushNotificationTokenEventArgs e);

    public delegate void PushNotificationErrorEventHandler(object source, PushNotificationErrorEventArgs e);

    public delegate void PushNotificationDataEventHandler(object source, PushNotificationDataEventArgs e);

    public delegate void PushNotificationResponseEventHandler(object source, PushNotificationResponseEventArgs e);

    /// <summary>
    /// Interface for PushNotification.
    /// </summary>
    public interface IPushNotification
    {
        /// <summary>
        /// Event triggered when token is refreshed
        /// </summary>
        event PushNotificationTokenEventHandler OnTokenRefresh;

        /// <summary>
        /// Event triggered when a notification is opened
        /// </summary>
        event PushNotificationResponseEventHandler OnNotificationOpened;

        /// <summary>
        /// Gets or sets notification handler to receive, customize notification feedback and provide user actions.
        /// </summary>
        IPushNotificationHandler NotificationHandler { get; set; }

        /// <summary>
        /// Event triggered when a notification is opened by tapping an action
        /// </summary>
        event PushNotificationResponseEventHandler OnNotificationAction;

        /// <summary>
        /// Event triggered when a notification is received
        /// </summary>
        event PushNotificationDataEventHandler OnNotificationReceived;

        /// <summary>
        /// Event triggered when a notification is deleted (Android Only)
        /// </summary>
        event PushNotificationDataEventHandler OnNotificationDeleted;

        /// <summary>
        /// Event triggered when there's an error
        /// </summary>
        event PushNotificationErrorEventHandler OnNotificationError;

        /// <summary>
        /// Gets push notification token.
        /// </summary>
        string Token { get; }

        /// <summary>
        /// Gets or sets delegate to feed token back to the plugin.
        /// </summary>
        Func<string> RetrieveSavedToken { get; set; }

        /// <summary>
        /// Register push notifications on demand.
        /// </summary>
        void RegisterForPushNotifications();

        /// <summary>
        /// Unregister push notifications on demand.
        /// </summary>
        void UnregisterForPushNotifications();

        /// <summary>
        /// Gets or sets delegate to save the token.
        /// </summary>
        Action<string> SaveToken { get; set; }

        /// <summary>
        /// Get all user notification categories.
        /// </summary>
        /// <returns></returns>
        NotificationUserCategory[] GetUserNotificationCategories();

        /// <summary>
        /// Clear all notifications.
        /// </summary>
        void ClearAllNotifications();

        /// <summary>
        /// Remove specific id notification.
        /// </summary>
        void RemoveNotification(int id);

        /// <summary>
        /// Remove specific id and tag notification.
        /// </summary>
        void RemoveNotification(string tag, int id);
    }
}