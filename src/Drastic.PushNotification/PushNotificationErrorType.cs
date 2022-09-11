// <copyright file="PushNotificationErrorType.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;

namespace Drastic.PushNotification
{
    /// <summary>
    /// Push Notification Error Type.
    /// </summary>
    public enum PushNotificationErrorType
    {
        /// <summary>
        /// Unknown.
        /// </summary>
        Unknown,

        /// <summary>
        /// Permission Denied.
        /// </summary>
        PermissionDenied,

        /// <summary>
        /// Registration Failed.
        /// </summary>
        RegistrationFailed,

        /// <summary>
        /// Unregistration Failed.
        /// </summary>
        UnregistrationFailed,
    }
}