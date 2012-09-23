using System;
using System.Device.Location;

namespace BuddyTest
{
    public class LocationTracker
    {
        private Action<GeoPositionChangedEventArgs<GeoCoordinate>> callback;
        private GeoCoordinateWatcher watcher;

        private LocationTracker(Action<GeoPositionChangedEventArgs<GeoCoordinate>> callback)
        {
            this.callback = callback;

            this.InitializeWatcher();
        }

        private void InitializeWatcher()
        {
            this.watcher = new GeoCoordinateWatcher();

            const int movementInMeters = 20;

            this.watcher.MovementThreshold = movementInMeters;

            this.watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(this.watcher_PositionChanged);
        }

        void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            this.callback(e);
        }

        public static LocationTracker GetInstance(Action<GeoPositionChangedEventArgs<GeoCoordinate>> callback)
        {
            return new LocationTracker(callback);
        }        
    }
}
