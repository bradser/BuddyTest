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

        private byte[] chosenPhotoBytes;
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
                Debug.WriteLine(user == null ? "LoginAsync null" : "LoginAsync");
                
                if (user != null)
                {
                    this.NavigateToMain(user);
                }

            }, this.Name.Text, this.Password.Password);
        }

        private void CreateUser()
        {
            var genderEnum = EnumHelper<UserGender>.GetInstance().GetValue((string)this.Gender.SelectedItem);

            var statusEnum = EnumHelper<UserStatus>.GetInstance().GetValue((string)this.Status.SelectedItem);

            this.CreateUser(genderEnum, statusEnum);
        }

        private void CreateUser(UserGender gender, UserStatus status)
        {
            ((App)App.Current).Client.CreateUserAsync((user, parameters) =>
            {
                Debug.WriteLine(user == null ? "CreateUserAsync null" : "CreateUserAsync");

                if (user != null)
                {
                    this.UserCreated(user);
                }

            }, this.Name.Text, this.Password.Password, gender, int.Parse(this.Age.Text), this.Email.Text, status);
        }

        private void UserCreated(AuthenticatedUser user)
        {
            this.SaveSettings();

            this.SavePicture(user);

            this.NavigateToMain(user);
        }

        private void SaveSettings()
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => // Dispatcher for cross-thread access to UI
            { 
                this.settings.Name = this.Name.Text;

                this.settings.Password = this.Password.Password;
            });
        }

        private void SavePicture(AuthenticatedUser user)
        {
            Debug.Assert(this.chosenPhotoBytes != null);

            user.AddProfilePhotoAsync((success, parameters) =>
            {
            }, this.chosenPhotoBytes);
        }

        private void NavigateToMain(AuthenticatedUser user)
        {
            ((App)App.Current).User = user;

            Deployment.Current.Dispatcher.BeginInvoke(() => // Dispatcher for cross-thread access to UI
            {
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            });
        }
    }
}