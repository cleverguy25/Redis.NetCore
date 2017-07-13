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