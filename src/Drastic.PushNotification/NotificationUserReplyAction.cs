// <copyright file="NotificationUserReplyAction.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;

namespace Drastic.PushNotification
{
    public class NotificationUserReplyAction : NotificationUserAction
    {
        public NotificationUserReplyAction(string id, string title, NotificationActionType type = NotificationActionType.Default, string icon = "", string placeholder = "")
            : base(id, title, type, icon)
        {
            this.Placeholder = placeholder;
        }

        public string Placeholder { get; }
    }
}