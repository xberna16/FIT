using System.Windows.Input;
using System.Collections.ObjectModel;
using APIClient;
using TeamCommunication.App.Commands;
using TeamCommunication.BL;
using TeamCommunication.BL.Messages;
using TeamDetailModel = APIClient.TeamDetailModel;
using System.Diagnostics;

namespace TeamCommunication.App.ViewModels
{
    public class TeamListViewModel : ViewModelBase
    {

        private readonly IMediator mediator;
        private Client apiclient;

        public TeamListViewModel(IMediator mediator, Client apiclient)
        {
            this.mediator = mediator;
            this.apiclient = apiclient;

            TeamSelectedCommand = new RelayCommand<TeamDetailModel>(TeamSelected);

            Load();
            
        }

        public ObservableCollection<TeamDetailModel> Teams { get; } = new ObservableCollection<TeamDetailModel>();

        public ICommand TeamSelectedCommand { get; }

        private void TeamSelected(TeamDetailModel team) => mediator.Send(new TeamSelectedMessage { Id = team.Id });

        public async void Load()
        {

            Debug.WriteLine("Load");
            Teams.Clear();
            //getTeamById
            var teams = await apiclient.GetTeamsByUserIdAsync(Properties.Settings.Default.IdLogged);

            foreach (var item in teams)
            {
                Teams.Add(item);
            }
        }

    }
}
