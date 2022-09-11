// <copyright file="PushNotificationDataEventArgs.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;

namespace Drastic.PushNotification
{
    public class PushNotificationDataEventArgs : EventArgs
    {
        public PushNotificationDataEventArgs(IDictionary<string, object> data)
        {
            this.Data = data;
        }

        public IDictionary<string, object> Data { get; }
    }
}