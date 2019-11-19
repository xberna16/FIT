using APIClient;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using TeamCommunication.App.Commands;
using TeamCommunication.BL;
using TeamCommunication.BL.Messages;
using TeamDetailModel = APIClient.TeamDetailModel;
using TopicDetailModel = APIClient.TopicDetailModel;
using UserListModel = APIClient.UserListModel;
using TopicListModel = APIClient.TopicListModel;
using UserDetailModel = APIClient.UserDetailModel;
using UserTeamDetailModel = APIClient.UserTeamDetailModel;
using System.Windows;

namespace TeamCommunication.App.ViewModels
{
    public class TeamDetailViewModel :ViewModelBase
    {

        private readonly IMediator mediator;
        private Client apiclient;

        public TeamDetailViewModel(IMediator mediator, Client apiclient)
        {
            this.mediator = mediator;
            this.apiclient = apiclient;
            
            AddTopicCommand = new RelayCommand(AddTopic);
            AddUserCommand = new RelayCommand(AddUser);
            RemoveUserCommand = new RelayCommand(RemoveUser);

            TopicSelectedCommand = new RelayCommand<TopicListModel>(SelectTopic);
            UserAddSelectedCommand = new RelayCommand<UserDetailModel>(SelectAddUser);
            UserRemoveSelectedCommand = new RelayCommand<UserDetailModel>(SelectRemoveUser);


            mediator.Register<TeamSelectedMessage>(TeamSelected);

        }
        
        public ObservableCollection<UserDetailModel> UsersAdd { get; } = new ObservableCollection<UserDetailModel>();
        public ObservableCollection<UserDetailModel> UsersRemove { get; } = new ObservableCollection<UserDetailModel>();
        public ObservableCollection<TopicListModel> TopicList { get; } = new ObservableCollection<TopicListModel>();

        public UserDetailModel useraddmodel = new UserDetailModel();
        public UserDetailModel UserAddModel
        {
            get => useraddmodel;
            set
            {
                useraddmodel = value;
                OnPropertyChanged();
            }
        }
        public UserDetailModel userremovemodel = new UserDetailModel();
        public UserDetailModel UserRemoveModel
        {
            get => userremovemodel;
            set
            {
                userremovemodel = value;
                OnPropertyChanged();
            }
        }
        public TeamDetailModel teammodel;
        public TeamDetailModel TeamModel
        {
            get => teammodel;
            set
            {
                teammodel = value;
                OnPropertyChanged();
            }
        }
        public TopicDetailModel topicmodel = new TopicDetailModel();
        public TopicDetailModel TopicModel
        {
            get => topicmodel;
            set
            {
                topicmodel = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddTopicCommand { get; set; }
        public ICommand AddUserCommand { get; set; }
        public ICommand RemoveUserCommand { get; set; }
        public ICommand UserAddSelectedCommand { get; }
        public ICommand UserRemoveSelectedCommand { get; }
        public ICommand TopicSelectedCommand { get; }


        private void SelectTopic(TopicListModel topic) => mediator.Send(new TopicSelectedMessage { Id = topic.Id });
        private async void TeamSelected(TeamSelectedMessage teamSelectedMessage)
        {
            Debug.WriteLine("TeamSelected");
            TeamModel = await apiclient.GetTeamAsync((System.Guid)teamSelectedMessage.Id);
            Debug.WriteLine(teammodel.Description);
            
            UsersAdd.Clear();
            var users = await apiclient.GetUsersNotInTeamAsync((System.Guid)TeamModel.Id);

            foreach (var item in users)
            {
                //if user neni v tymu tak add
                UsersAdd.Add(item);
            }
            
            UsersRemove.Clear();
            var users2 = await apiclient.GetUsersByTeamIdAsync((System.Guid)TeamModel.Id);

            foreach (var item in users2)
            {
                //if user je v tymu tak add
                  
                UsersRemove.Add(item);
            }
            
            TopicList.Clear();
            var topics = await apiclient.GetTopicsByTeamIdAsync((System.Guid)TeamModel.Id);

            foreach (var item in topics)
            {
                //if user je v tymu tak add
                TopicList.Add(item);
            }
        }

        public async void AddTopic()
        {
            Debug.WriteLine("AddTopic Button");
            Debug.WriteLine(topicmodel.Heading);
            Debug.WriteLine(topicmodel.Text);
            topicmodel.TeamId = TeamModel.Id;
            topicmodel.UserId = Properties.Settings.Default.IdLogged;
            TimeSpan time = DateTime.Now - DateTime.UtcNow;
            topicmodel.Date = new DateTimeOffset(2017, 5, 6, 10, 10, 10, time);
            topicmodel.Id = null;
            try
            {
                await apiclient.PostTopicAsync(topicmodel);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + " " + e.InnerException);
            }

            TopicList.Clear();
            var topics = await apiclient.GetTopicsByTeamIdAsync((System.Guid)TeamModel.Id);
            foreach (var item in topics)
            {
                //if user je v tymu tak add
                TopicList.Add(item);
            }

        }

        public async void AddUser()
        {
            Debug.WriteLine("AddUser Button");

            UserTeamDetailModel model = new UserTeamDetailModel();
            model.IdTeam = TeamModel.Id;
            model.IdUser = UserAddModel.Id;
            await apiclient.PostUserTeamAsync(model);

            var users = await apiclient.GetUsersNotInTeamAsync((System.Guid)model.IdTeam);
            UsersAdd.Clear();
            foreach (var item in users)
            {
                UsersAdd.Add(item);
            }

            var usersIn = await apiclient.GetUsersByTeamIdAsync((System.Guid)model.IdTeam);
            UsersRemove.Clear();
            foreach (var item in usersIn)
            {
                UsersRemove.Add(item);
            }
        }

        public async void RemoveUser()
        {
            Debug.WriteLine("RemoveUser Button");
            Debug.WriteLine(UserRemoveModel.Id);
            Debug.WriteLine(TeamModel.Id);

            UserTeamDetailModel model = new UserTeamDetailModel();
            model.IdTeam = TeamModel.Id;
            model.IdUser = UserRemoveModel.Id;
            await apiclient.DeleteUserTeamAsync(model);
            var users = await apiclient.GetUsersByTeamIdAsync((System.Guid)model.IdTeam);
            UsersRemove.Clear();
            foreach (var item in users)
            {
                UsersRemove.Add(item);
            }
            var usersNotIn = await apiclient.GetUsersNotInTeamAsync((System.Guid)model.IdTeam);
            UsersAdd.Clear();
            foreach (var item in usersNotIn)
            {
                UsersAdd.Add(item);
            }
        }

        private async void SelectAddUser(UserDetailModel user)
        {
            Debug.WriteLine("Selected AddUser");
            useraddmodel = await apiclient.GetUserAsync((System.Guid)user.Id);
        }

        private async void SelectRemoveUser(UserDetailModel user)
        {
            Debug.WriteLine("Selected RemoveUser");
            userremovemodel = await apiclient.GetUserAsync((System.Guid)user.Id);
        }

    }
}
