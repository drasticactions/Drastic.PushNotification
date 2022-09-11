// <copyright file="NotificationUserAction.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

namespace Drastic.PushNotification
{
    public class NotificationUserAction
    {
        public NotificationUserAction(string id, string title, NotificationActionType type = NotificationActionType.Default, string icon = "")
        {
            this.Id = id;
            this.Title = title;
            this.Type = type;
            this.Icon = icon;
        }

        public string Id { get; }

        public string Title { get; }

        public NotificationActionType Type { get; }

        public string Icon { get; }
    }
}