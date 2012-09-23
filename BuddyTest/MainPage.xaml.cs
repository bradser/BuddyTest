using System.Windows;
using Buddy;
using Microsoft.Phone.Controls;
using System.Collections.Generic;
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

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void InitializeUpdateLocation()
        {
            this.updateLocation = LocationTracker.GetInstance(positionChangedArgs =>
            {
                ((App)App.Current).User.CheckInAsync((success, callbackParams) =>
                {
                }, positionChangedArgs.Position.Location.Latitude, positionChangedArgs.Position.Location.Longitude);
            });
        }
    }
}