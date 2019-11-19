using System;
using TeamCommunication.App.Commands;
using TeamCommunication.BL;
using System.Windows.Input;
using APIClient;
using System.Diagnostics;
using UserDetailModel = APIClient.UserDetailModel;
using System.Windows;

namespace TeamCommunication.App.ViewModels
{
    class SigninViewModel : ViewModelBase
    {

        private readonly IPasswordControl _authentication = new PasswordControl(new DbContextFactory());
        
        private readonly IMediator mediator;
        private Client apiclient;
        public SigninViewModel(IMediator mediator, Client apiclient)
        {
            this.mediator = mediator;
            this.apiclient = apiclient;

            SigninCommand = new RelayCommand(Login);
            RegisterCommand = new RelayCommand(Register);
        }

        public UserDetailModel userlogin = new UserDetailModel();
        public UserDetailModel UserLogin
        {
            get => userlogin;
            set
            {
                userlogin = value;
                OnPropertyChanged();
            }
        }

        public UserDetailModel userregistration = new UserDetailModel();
        public UserDetailModel UserRegistration
        {
            get => userregistration;
            set
            {
                userregistration = value;
                OnPropertyChanged();
            }
        }

        public ICommand SigninCommand { get; set; }
        public ICommand RegisterCommand { get; set; }

        private void OpenWindow()
        {
            var window = new MainWindow();
            window.Show();
        }
        public async void Login()
        {
            Debug.WriteLine("Signin Button");
            Debug.WriteLine(userlogin.Email);
            Debug.WriteLine(userlogin.Password);
            try
            {
                if (!canLogin())
                    throw new Exception("Nejsou vyplnena vsechna pole");
                var login = await apiclient.ULoginAsync((APIClient.UserDetailModel)userlogin);
                Debug.WriteLine(login);
                Properties.Settings.Default.IdLogged = login;
                Debug.WriteLine(Properties.Settings.Default.IdLogged);
                OpenWindow();
                
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + " " + e.InnerException);
            }
            
        }
        private bool canLogin()
        {
            return (!string.IsNullOrWhiteSpace(userlogin.Email)) && (!string.IsNullOrWhiteSpace(userlogin.Password));
        }
        private bool canRegister()
        {
            return (!string.IsNullOrWhiteSpace(userregistration.Nickname)) && (!string.IsNullOrWhiteSpace(userregistration.Password)) && (!string.IsNullOrWhiteSpace(userregistration.Email)) && (!string.IsNullOrWhiteSpace(userregistration.Nickname)) && (!string.IsNullOrWhiteSpace(userregistration.FirstName)) && (!string.IsNullOrWhiteSpace(userregistration.LastName));
        }
        public async void Register()
        {
            Debug.WriteLine("Register Button");
            Debug.WriteLine(userregistration.Email);
            Debug.WriteLine(userregistration.Nickname);
            Debug.WriteLine(userregistration.FirstName);
            Debug.WriteLine(userregistration.LastName);
            Debug.WriteLine(userregistration.Password);
            try
            {
                if (!canRegister())
                    throw new Exception("Nejsou vyplnena vsechna pole");
                var registration = await apiclient.URegisterAsync((APIClient.UserDetailModel)userregistration);

                userlogin.Email = userregistration.Email;
                userlogin.Password = userregistration.Password;
                Login();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + " " + e.InnerException);
            }
        }

    }
}
