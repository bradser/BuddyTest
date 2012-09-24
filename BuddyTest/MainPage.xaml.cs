using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Diagnostics;
using System.Windows;
using Buddy;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Maps;
using System.Linq;
using Microsoft.Phone.Shell;

namespace BuddyTest
{
    public partial class MainPage : PhoneApplicationPage
    {
        private bool setMeOnce;

        private LocationTracker updateLocation;

        public MainPage()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(this.MainPage_Loaded);
        }

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
#if DEBUG
            this.DumpPlaces(places);
#endif
            
            this.DataContext = places;

            this.SetMeOnce();

            this.UpdateLiveTile();
        }

#if DEBUG       
        private void DumpPlaces(List<Place> places)
        {
            foreach (var place in places)
            {
                Debug.WriteLine(place.Name);
            }

            Debug.WriteLine("");
        }
#endif

        private void SetMeOnce()
        {
            if (this.setMeOnce)
            {
                if (this.SetMe())
                {
                    this.setMeOnce = false;
                }
            }
        }

        private void UpdateLiveTile()
        {
            var tileToFind = ShellTile.ActiveTiles.First();

            if (tileToFind != null)
            {
                var newTileData = this.GetNewTileData();

                tileToFind.Update(newTileData);
            }
        }

        private StandardTileData GetNewTileData()
        {
            return new StandardTileData
            {
                BackgroundImage = new Uri("/Background.png", UriKind.Relative),

                BackContent = ((App)App.Current).User.Name,

                Count = this.SafePlaceCount()
            };
        }

        private int SafePlaceCount()
        {
            var places = this.DataContext as List<Place>;

            return places == null ? 0 : places.Count;
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
            if (this.SafePlaceCount() > 0)
            {
                Debug.WriteLine("SetMe");

                var places = (List<Place>) this.DataContext;

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