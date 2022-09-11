// <copyright file="PushNotificationResponseEventArgs.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;

namespace Drastic.PushNotification
{
    public class PushNotificationResponseEventArgs : EventArgs
    {
        public PushNotificationResponseEventArgs(IDictionary<string, object> data, string identifier = "", NotificationCategoryType type = NotificationCategoryType.Default, string? result = null)
        {
            this.Identifier = identifier;
            this.Data = data;
            this.Type = type;
            this.Result = result;
        }

        public string Identifier { get; }

        public IDictionary<string, object> Data { get; }

        public NotificationCategoryType Type { get; }

        public string? Result { get; }
    }
}