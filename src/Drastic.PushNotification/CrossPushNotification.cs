// <copyright file="CrossPushNotification.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;

namespace Drastic.PushNotification
{
    /// <summary>
    /// Cross platform PushNotification implemenations.
    /// </summary>
    public class CrossPushNotification
    {
        private static readonly Lazy<IPushNotification?> Implementation = new Lazy<IPushNotification?>(() => CreatePushNotification(), System.Threading.LazyThreadSafetyMode.PublicationOnly);

        /// <summary>
        /// Gets current settings to use.
        /// </summary>
        public static IPushNotification Current
        {
            get
            {
                var ret = Implementation.Value;
                if (ret == null)
                {
                    throw NotImplementedInReferenceAssembly();
                }

                return ret;
            }
        }

        internal static Exception NotImplementedInReferenceAssembly()
        {
            return new NotImplementedException("This functionality is not implemented in the portable version of this assembly.  You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
        }

        private static IPushNotification? CreatePushNotification()
        {
#if NETSTANDARD2_0
            return null;
#else
            return new PushNotificationManager();
#endif
        }
    }
}