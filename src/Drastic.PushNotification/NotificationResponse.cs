// <copyright file="NotificationResponse.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;

namespace Drastic.PushNotification
{
    public class NotificationResponse
    {
        public NotificationResponse(IDictionary<string, object> data, string identifier = "", NotificationCategoryType type = NotificationCategoryType.Default, string? result = null)
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