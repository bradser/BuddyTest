using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Windows;
using Buddy;
using Microsoft.Phone.Controls;

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

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.InitializeUpdateLocation();
        }

        private GeoCoordinate currentLocation;

        private void InitializeUpdateLocation()
        {
            this.updateLocation = LocationTracker.GetInstance(positionChangedArgs =>
            {
                this.currentLocation = positionChangedArgs.Position.Location;
                
                this.UpdatePage(this.currentLocation);

                this.CheckIn(this.currentLocation);
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
        }

        private void CheckIn(GeoCoordinate coordinate)
        {
            ((App)App.Current).User.CheckInAsync((success, callbackParams) =>
            {
            }, coordinate.Latitude, coordinate.Longitude);
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            if (this.currentLocation != null)
            {
                const int cityZoomLevel = 11;

                this.Map.SetView(this.currentLocation, cityZoomLevel);
            }
        }
    }
}