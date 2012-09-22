using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Buddy;

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
            var x = typeof(UserGender);

            this.Gender.ItemsSource = this.GetNamesFromEnum(typeof(UserGender));

            this.Status.ItemsSource = this.GetNamesFromEnum(typeof(UserStatus));
        }

        private IOrderedEnumerable<string> GetNamesFromEnum(Type enumType)
        {
            return enumType.GetFields().Where(fi => fi.IsLiteral).Select(fi => fi.Name).OrderBy(name => name);
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

            var genderEnum = this.GetEnumValueFromListPickerItem<UserGender>(Gender);

            var statusEnum = this.GetEnumValueFromListPickerItem<UserStatus>(Gender);

            //client.CreateUserAsync((user, parameters) =>
            //{
            //}, Name.Text, Password.Text, genderEnum, int.Parse(Age.Text), Email.Text, statusEnum);       
        }

        private EnumType GetEnumValueFromListPickerItem<EnumType>(ListPicker listPicker)
        {
            return (EnumType)Enum.Parse(typeof(EnumType), (string)((ListPickerItem)Gender.SelectedItem).Content, false);
        }
    }
}