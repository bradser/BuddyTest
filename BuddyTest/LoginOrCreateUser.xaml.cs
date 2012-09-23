using System;
using System.Diagnostics;
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
            if (!this.HasName)
            {
                this.Gender.ItemsSource = EnumHelper<UserGender>.GetInstance().AlphabeticalNames;

                this.Status.ItemsSource = EnumHelper<UserStatus>.GetInstance().AlphabeticalNames;

                this.AdditionalFields.Visibility = Visibility.Visible;
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

        private bool HasName
        {
            get
            {
                return !string.IsNullOrEmpty(this.settings.Name);
            }
        }

        private void Login()
        {
            this.SaveSettings();

            this.LoginUser();
        }

        private void LoginUser()
        {
            ((App)App.Current).Client.LoginAsync((user, state) =>
            {
                this.UserCreatedOrLoggedIn(user, () => { this.NavigateToMain(user); }, "Login failed. Please try again."); // TODO: move string to resource

            }, this.Name.Text, this.Password.Password);
        }

        private void UserCreatedOrLoggedIn(AuthenticatedUser user, Action action, string errorMessage)
        {
            if (user == null)
            {
                this.CrossThreadMessageBox(errorMessage);
            }
            else
            {
                action();
            }
        }

        private void CrossThreadMessageBox(string message)
        {
            this.CrossThread(() => { MessageBox.Show(message); });
        }

        private void CrossThread(Action action)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => // Dispatcher for cross-thread access to UI
            {
                action();
            });
        }

        private void CreateUser()
        {
            if (string.IsNullOrEmpty(this.Name.Text) || string.IsNullOrEmpty(this.Password.Password))
            {
                this.CrossThreadMessageBox("Enter a name and a password.");
            }
            else
            {
                var genderEnum = EnumHelper<UserGender>.GetInstance().GetValue((string)this.Gender.SelectedItem);

                var statusEnum = EnumHelper<UserStatus>.GetInstance().GetValue((string)this.Status.SelectedItem);

                this.CreateUser(genderEnum, statusEnum);
            }
        }

        private void CreateUser(UserGender gender, UserStatus status)
        {
            ((App)App.Current).Client.CreateUserAsync((user, parameters) =>
            {
                this.UserCreatedOrLoggedIn(user, () => { this.UserCreated(user); }, "Create user failed. Please try again."); // TODO: move string to resource

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
            this.CrossThread(() =>
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

            this.CrossThread(() =>
            {
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            });
        }
    }
}