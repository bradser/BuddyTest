using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Buddy;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;

namespace BuddyTest
{
    public partial class LoginOrCreateUser : PhoneApplicationPage
    {
        private Settings settings;

        private byte[] chosenPhotoBytes;

        public LoginOrCreateUser()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(this.LoginOrCreateUser_Loaded);

            this.settings = Settings.GetInstance();
        }

        private void LoginOrCreateUser_Loaded(object sender, RoutedEventArgs e)
        {          
            this.SetupAdditionalFields();
        }

        private void SetupAdditionalFields()
        {
            if (this.HasName)
            {
                this.AdditionalFields.Visibility = Visibility.Collapsed;

                this.Name.Text = this.settings.Name;
            }
            else
            {
                this.Gender.ItemsSource = EnumHelper<UserGender>.GetInstance().AlphabeticalNames;

                this.Status.ItemsSource = EnumHelper<UserStatus>.GetInstance().AlphabeticalNames;
            }
        }

        private bool HasName
        {
            get
            {
                return !string.IsNullOrEmpty(this.settings.Name);
            }
        }

        private void EmptyPhoto_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.SelectPhoto();
        }

        private void Photo_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.SelectPhoto();
        }

        private void SelectPhoto()
        {
            var photoChooserTask = new PhotoChooserTask();

            photoChooserTask.Completed += new EventHandler<PhotoResult>(this.photoChooserTask_Completed);

            photoChooserTask.Show();
        }

        void photoChooserTask_Completed(object sender, PhotoResult e)
        {
 	        if (e.TaskResult == TaskResult.OK)
            {
                this.SetPhoto(e.ChosenPhoto);

                this.GetChosenPhotoBytes(e.ChosenPhoto);
            }
        }

        private void SetPhoto(Stream stream)
        {
            this.SetPhotoSource(stream);

            this.EmptyPhoto.Visibility = Visibility.Collapsed;

            this.Photo.Visibility = Visibility.Visible;
        }

        private void SetPhotoSource(Stream stream)
        {
            var bitmapImage = new BitmapImage();

            bitmapImage.SetSource(stream);

            this.Photo.Source = bitmapImage;
        }

        private void GetChosenPhotoBytes(Stream stream)
        {
            this.chosenPhotoBytes = new byte[stream.Length];

            stream.Seek(0, SeekOrigin.Begin); // reset to beginning, as it was already loaded into this.Photo

            stream.Read(this.chosenPhotoBytes, 0, this.chosenPhotoBytes.Length);
        }
        
        private void Submit_Tap(object sender, RoutedEventArgs e)
        {
            if (this.HasName)
            {
                this.Login();
            }
            else
            {
                this.CreateUser();
            }
        }

        private void Login()
        {
            this.SaveSettings();

            this.LoginUser();
        }

        private void LoginUser()
        {
            ((App)App.Current).Client.LoginAsync((user, callbackParams) =>
            {
                Utilities.HandleAsyncResults(user, callbackParams, () => { this.NavigateToMain(user); }, "Login failed. Please try again."); // TODO: move all strings to resource

            }, this.Name.Text, this.Password.Password);
        }

        private void CreateUser()
        {
            if (this.ValidateFields())
            {
                var genderEnum = EnumHelper<UserGender>.GetInstance().GetValue((string)this.Gender.SelectedItem);

                var statusEnum = EnumHelper<UserStatus>.GetInstance().GetValue((string)this.Status.SelectedItem);

                this.CreateUser(genderEnum, statusEnum);
            }
        }

        private bool ValidateFields()
        {
            if (string.IsNullOrEmpty(this.Name.Text))
            {
                Utilities.CrossThreadMessageBox("Please enter a name.");

                return false;
            }
            else
                if (string.IsNullOrEmpty(this.Password.Password))
                {
                    Utilities.CrossThreadMessageBox("Please enter a password.");

                    return false;
                }

            return true;
        }

        private void CreateUser(UserGender gender, UserStatus status)
        {
            ((App)App.Current).Client.CreateUserAsync((user, callbackParams) =>
            {
                Utilities.HandleAsyncResults(user, callbackParams, () => { this.UserCreated(user); }, "Create user failed. Please try again.");

            }, this.Name.Text, this.Password.Password, gender: gender, age: this.GetAge(),
            email: this.Email.Text, status: status);
        }

        private int GetAge()
        {
            var age = 0;

            int.TryParse(this.Age.Text, out age);

            return age;
        }

        private void UserCreated(AuthenticatedUser user)
        {
            this.SaveSettings();

            this.SavePhoto(user);

            this.NavigateToMain(user);
        }

        private void SaveSettings()
        {
            Utilities.CrossThread(() =>
            { 
                this.settings.Name = this.Name.Text;

                this.settings.Password = this.Password.Password;
            });
        }

        private void SavePhoto(AuthenticatedUser user)
        {
            if (this.chosenPhotoBytes != null)
            {
                user.AddProfilePhotoAsync((success, parameters) =>
                {
                    // TODO: handle errors

                }, this.chosenPhotoBytes);
            }
        }

        private void NavigateToMain(AuthenticatedUser user)
        {
            ((App)App.Current).User = user;

            Utilities.CrossThread(() =>
            {
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            });
        }
    }
}