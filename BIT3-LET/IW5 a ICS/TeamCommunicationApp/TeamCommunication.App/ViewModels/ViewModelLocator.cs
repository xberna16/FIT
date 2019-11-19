using APIClient;
using CookBook.BL;
using TeamCommunication.BL;

namespace TeamCommunication.App.ViewModels
{
    class ViewModelLocator
    {
        private readonly IMediator mediator;
        private Client apiclient;

        public ViewModelLocator()
        {
            mediator = new Mediator();
            apiclient = new Client("http://localhost:5000/", new System.Net.Http.HttpClient());
        }

        public TeamListViewModel TeamListViewModel => new TeamListViewModel(mediator, apiclient);
        public TeamDetailViewModel TeamDetailViewModel => new TeamDetailViewModel(mediator, apiclient);
        public CommentDetailViewModel CommentDetailViewModel => new CommentDetailViewModel(mediator, apiclient);
        public SigninViewModel SigninViewModel => new SigninViewModel(mediator, apiclient);

    }
}
