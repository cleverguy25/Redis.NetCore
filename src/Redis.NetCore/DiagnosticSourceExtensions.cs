// <copyright file="DiagnosticSourceExtensions.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Redis.NetCore
{
    public static class DiagnosticSourceExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogEvent(this DiagnosticSource source, string eventName, object payload = null)
        {
            if (source.IsEnabled(eventName))
            {
                source.Write(eventName, payload);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogEvent(this DiagnosticSource source, string eventName, Func<object> getPayload)
        {
            if (source.IsEnabled(eventName))
            {
                source.Write(eventName, getPayload());
            }
        }
    }
}
