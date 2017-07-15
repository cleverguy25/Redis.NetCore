// <copyright file="GeoUnit.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Redis.NetCore
{
    public class GeoUnit
    {
        public static readonly GeoUnit Meters = new GeoUnit("m");
        public static readonly GeoUnit Kilometers = new GeoUnit("km");
        public static readonly GeoUnit Miles = new GeoUnit("mi");
        public static readonly GeoUnit Feet = new GeoUnit("ft");

        public GeoUnit(string unit)
        {
            Unit = unit;
        }

        public string Unit { get; }
    }
}