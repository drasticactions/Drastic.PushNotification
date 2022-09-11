// <copyright file="NotificationUserCategory.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;

namespace Drastic.PushNotification
{
    public class NotificationUserCategory
    {
        public NotificationUserCategory(string category, List<NotificationUserAction> actions, NotificationCategoryType type = NotificationCategoryType.Default)
        {
            this.Category = category;
            this.Actions = actions;
            this.Type = type;
        }

        public string Category { get; }

        public List<NotificationUserAction> Actions { get; }

        public NotificationCategoryType Type { get; }
    }
}