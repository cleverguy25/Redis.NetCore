// <copyright file="GeoPosition.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace Redis.NetCore
{
    public class GeoPosition
    {
        public GeoPosition(double longitude, double latitude)
        {
            Longitude = longitude;
            Latitude = latitude;
        }

        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public override bool Equals(object other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return other.GetType() == GetType() && Equals((GeoPosition)other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Longitude.GetHashCode() * 397) ^ Latitude.GetHashCode();
            }
        }

        private bool Equals(GeoPosition other)
        {
            const double tolerance = .000005;
            return Math.Abs(Longitude - other.Longitude) < tolerance && Math.Abs(Latitude - other.Latitude) < tolerance;
        }
    }
}