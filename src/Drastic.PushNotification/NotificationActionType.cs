// <copyright file="NotificationActionType.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;

namespace Drastic.PushNotification
{
    /// <summary>
    /// Notification Action Type.
    /// </summary>
    public enum NotificationActionType
    {
        /// <summary>
        /// Default.
        /// </summary>
        Default,

        /// <summary>
        /// Authentication Required.
        /// Only applies for iOS.
        /// </summary>
        AuthenticationRequired,

        /// <summary>
        /// Foreground.
        /// </summary>
        Foreground,

        /// <summary>
        /// Reply.
        /// </summary>
        Reply,

        /// <summary>
        /// Destructive.
        /// Only applies for iOS.
        /// </summary>
        Destructive,
    }
}