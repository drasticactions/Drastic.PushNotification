// <copyright file="PushNotificationTokenEventArgs.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;

namespace Drastic.PushNotification
{
    public class PushNotificationTokenEventArgs : EventArgs
    {
        public PushNotificationTokenEventArgs(string token)
        {
            this.Token = token;
        }

        public string Token { get; }
    }
}