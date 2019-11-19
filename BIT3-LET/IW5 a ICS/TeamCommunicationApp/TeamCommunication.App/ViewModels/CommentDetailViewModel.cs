using APIClient;
using System;
using System.Diagnostics;
using System.Windows.Input;
using TeamCommunication.App.Commands;
using TeamCommunication.BL;
using TeamCommunication.BL.Messages;
using CommentDetailModel = APIClient.CommentDetailModel;
using TopicDetailModel = APIClient.TopicDetailModel;
using CommentListModel = APIClient.CommentListModel;
using System.Collections.ObjectModel;
using System.Windows;

namespace TeamCommunication.App.ViewModels
{
    class CommentDetailViewModel : ViewModelBase
    {

        private readonly IMediator mediator;
        private Client apiclient;

        public CommentDetailViewModel(IMediator mediator, Client apiclient)
        {
            this.mediator = mediator;
            this.apiclient = apiclient;

            AddCommentCommand = new RelayCommand(AddComment);
            CommentSelectedCommand = new RelayCommand<CommentDetailModel>(CommentSelected);

            mediator.Register<TopicSelectedMessage>(TopicSelected);
        }

        public ICommand AddCommentCommand { get; set; }
        public ICommand CommentSelectedCommand { get; }

        public ObservableCollection<CommentListModel> Comments { get; } = new ObservableCollection<CommentListModel>();
        public CommentDetailModel commentdetail = new CommentDetailModel();
        public CommentDetailModel CommentDetail
        {
            get => commentdetail;
            set
            {
                commentdetail = value;
                OnPropertyChanged();
            }
        }
        public TopicDetailModel topicdetail = new TopicDetailModel();
        public TopicDetailModel TopicDetail
        {
            get => topicdetail;
            set
            {
                topicdetail = value;
                OnPropertyChanged();
            }
        }


        private void CommentSelected(CommentDetailModel comment) => mediator.Send(new CommentSelectedMessage { Id = comment.Id });
        private async void TopicSelected(TopicSelectedMessage topicSelectedMessage)
        {
            Debug.WriteLine("TopicSelected");
            TopicDetail = await apiclient.GetTopicAsync((System.Guid)topicSelectedMessage.Id);
            Debug.WriteLine(topicdetail.Heading);
            Debug.WriteLine(topicdetail.Text);

            var comments = await apiclient.GetCommentsByTopicIdAsync((System.Guid)topicSelectedMessage.Id);
            Comments.Clear();
            foreach (var item in comments)
            {
                Comments.Add(item);
            }
        }
        
        private async void AddComment()
        {
            Debug.WriteLine("AddComment Button");
            commentdetail.UserId = Properties.Settings.Default.IdLogged;
            commentdetail.TopicId = TopicDetail.Id;
            try
            {
                var comment = await apiclient.PostCommentAsync(commentdetail);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + " " + e.InnerException);
            }
            var comments = await apiclient.GetCommentsByTopicIdAsync((System.Guid)TopicDetail.Id);
            Comments.Clear();
            foreach (var item in comments)
            {
                Comments.Add(item);
            }
        }
        
    }
}
