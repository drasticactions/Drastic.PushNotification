// <copyright file="PushNotificationErrorEventArgs.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;

namespace Drastic.PushNotification
{
    public class PushNotificationErrorEventArgs : EventArgs
    {
        public PushNotificationErrorType Type;

        public PushNotificationErrorEventArgs(PushNotificationErrorType type, string message)
        {
            this.Type = type;
            this.Message = message;
        }

        public string Message { get; }
    }
}