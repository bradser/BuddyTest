using System.Windows;
using Buddy;
using Microsoft.Phone.Controls;

namespace BuddyTest
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.Gender.ItemsSource = EnumHelper<UserGender>.GetInstance().AlphabeticalNames;

            this.Status.ItemsSource = EnumHelper<UserStatus>.GetInstance().AlphabeticalNames;
        }

        private void Submit_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.CreateUser();
        }

        private void CreateUser()
        {
            BuddyClient client = new BuddyClient("brad_serbus@msn.com - Sample App", "81108202-A922-4D81-999A-2EFA7B554893");

            /*client.LoginAsync((user, state) =>
            {
                // enable add user in first pivot
            }, "username", "password");*/

            var genderEnum = EnumHelper<UserGender>.GetInstance().GetValue((string)Gender.SelectedItem);

            var statusEnum = EnumHelper<UserStatus>.GetInstance().GetValue((string)Status.SelectedItem);

            var asyncResult = client.CreateUserAsync((user, parameters) =>
            {
                Deployment.Current.Dispatcher.BeginInvoke(() => { MessageBox.Show("User created"); });

            }, Name.Text, Password.Password, genderEnum, int.Parse(Age.Text), Email.Text, statusEnum);       
        }
    }
}