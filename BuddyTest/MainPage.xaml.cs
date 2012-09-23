using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Windows;
using Buddy;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Maps;
using System.Linq;

namespace BuddyTest
{
    public partial class MainPage : PhoneApplicationPage
    {
        private LocationTracker updateLocation;

        public MainPage()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(this.MainPage_Loaded);
        }

        private bool setMeOnce;

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.setMeOnce = true;

            this.InitializeUpdateLocation();
        }

        private void InitializeUpdateLocation()
        {
            this.updateLocation = LocationTracker.GetInstance(positionChangedArgs =>
            {
                var currentLocation = positionChangedArgs.Position.Location;

                this.UpdatePage(currentLocation);

                this.CheckIn(currentLocation);
            });
        }

        private void UpdatePage(GeoCoordinate coordinate)
        {
            const int NumberOfPlaces = 20;
            
            const int SearchDistanceInMeters = 100; // about one football field

            ((App)App.Current).User.Places.FindAsync((places, callbackParams) =>
            {
                Utilities.HandleAsyncResults(places, callbackParams, () =>
                {
                    this.UpdatePage(places);
                });

            }, SearchDistanceInMeters, coordinate.Latitude, coordinate.Longitude, NumberOfPlaces);
        }

        private void UpdatePage(List<Place> places)
        {
            this.DataContext = places;

            this.SetMeOnce();
        }

        private void SetMeOnce()
        {
            if (this.setMeOnce)
            {
                if (this.SetMe())
                {
                    this.setMeOnce = false;

                    // TODO: For some reason, we have to manually invalidate; otherwise, the map won't render correctly before user interaction
                    this.Map.InvalidateMeasure();
                }
            }
        }

        private void CheckIn(GeoCoordinate coordinate)
        {
            ((App)App.Current).User.CheckInAsync((success, callbackParams) =>
            {
            }, coordinate.Latitude, coordinate.Longitude);
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            this.SetMe();
        }

        private bool SetMe()
        {
            var places = this.DataContext as List<Place>;

            if (places != null && places.Count > 0)
            {
                // using Linq here is concise, but, other algorithms can be faster I bet
                var boundingRectangle = new LocationRect(
                    places.Max((p) => p.Latitude),
                    places.Min((p) => p.Longitude),
                    places.Min((p) => p.Latitude),
                    places.Max((p) => p.Longitude));

                this.Map.SetView(boundingRectangle);

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}