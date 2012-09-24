using System;
using System.Diagnostics;
using System.Device.Location;

namespace BuddyTest
{
    public class LocationTracker
    {
        private Action<GeoPositionChangedEventArgs<GeoCoordinate>> callback;

        private GeoCoordinateWatcher watcher;

        private bool ready;

        private GeoPositionChangedEventArgs<GeoCoordinate> latestEventArgs;
        
        private LocationTracker(Action<GeoPositionChangedEventArgs<GeoCoordinate>> callback)
        {
            this.callback = callback;

            this.InitializeWatcher();
        }

        private void InitializeWatcher()
        {
            this.ready = false;
            
            this.watcher = new GeoCoordinateWatcher();

            const int movementInMeters = 20;

            this.watcher.MovementThreshold = movementInMeters;

            this.watcher.StatusChanged += new EventHandler<GeoPositionStatusChangedEventArgs>(this.watcher_StatusChanged);
                                
            this.watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(this.watcher_PositionChanged);

            this.watcher.Start(); // TODO: find best way to stop
        }

        void watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case GeoPositionStatus.Disabled:
                    // The Location Service is disabled or unsupported.
                    // Check to see whether the user has disabled the Location Service.
                    if (this.watcher.Permission == GeoPositionPermission.Denied)
                    {
                        // The user has disabled the Location Service on their device.
                        Utilities.CrossThreadMessageBox("You have disabled location services on this device.");
                    }
                    else
                    {
                        Utilities.CrossThreadMessageBox("Location is not functioning on this device.");
                    }
                    break;

                case GeoPositionStatus.NoData:
                    // The Location Service is working, but it cannot get location data.
                    Utilities.CrossThreadMessageBox("Location data is not available.");
                    break;

                case GeoPositionStatus.Ready:
                    Debug.WriteLine("GeoPositionStatus.Ready");

                    this.ready = true;

                    this.watcher_PositionChanged(this, this.latestEventArgs);

                    break;
            }
        }

        // watcher_PositionChanged sometimes fires twice upon GeoCoordinateWatcher.Start().
        // To work around this, before GeoCoordinateWatcher is ready, I remember only the last GeoPositionChangedEventArgs.
        // Once ready, I act upon that latest GeoPositionChangedEventArgs.
        
        void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            Debug.WriteLine("watcher_PositionChanged");

            if (this.ready)
            {
                this.callback(e);
            }
            else
            {
                this.latestEventArgs = e;
            }
        }

        public static LocationTracker GetInstance(Action<GeoPositionChangedEventArgs<GeoCoordinate>> callback)
        {
            return new LocationTracker(callback);
        }        
    }
}
