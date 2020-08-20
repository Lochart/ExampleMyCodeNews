using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace News
{
    /// <summary>
    /// Модель представление Dashboard
    /// </summary>
    public class ViewModelDashboard : ViewModel
    {
        /// <summary>
        /// Список новостей
        /// </summary>
        public ObservableCollection<ModelObjectNews> SourceNews { get; set; }

        /// <summary>
        /// Список избранных
        /// </summary>
        public ObservableCollection<ModelObjectNews> SourceFavorites { get; set; }

        /// <summary>
        /// Обновление списка
        /// </summary>
        public bool IsRefreshingNews { get; set; }

        /// <summary>
        /// Заголовок верхней шапки bar
        /// </summary>
        public string TextBarTitle { get; set; }

        /// <summary>
        /// Комманда обновление действия контекста
        /// </summary>
        public ICommand CommandUpdareContext => new Command(UpdareContext);

        /// <summary>
        /// Комманда скрывание новость
        /// </summary>
        public ICommand CommandHideNews => new Command(HideNews);

        /// <summary>
        /// Комманда добовляем в закладки
        /// </summary>
        public ICommand CommandAddFavorites => new Command(AddFavorites);

        /// <summary>
        /// Комманда обновление списка новостей
        /// </summary>
        public ICommand CommandRefreshListNews => new Command(RefreshListNews);

        public ViewModelDashboard()
        {
            TextBarTitle = "Новости";
            SourceNews = new ObservableCollection<ModelObjectNews>();
            SourceFavorites = new ObservableCollection<ModelObjectNews>();
            RequestGetNews();
        }

        /// <summary>
        /// Функция обновления контекста, показать больше или скрыть
        /// </summary>
        /// <param name="sender">StackLayout</param>
        private void UpdareContext(object sender)
        {
            Debug.WriteLine("UpdareContext : " + sender);
            var viewCellUI = sender as StackLayout;

            if (viewCellUI.Children.Count == 0)
                return;

            var viewContent = viewCellUI.FindByName<Label>("ViewContent");
            var viewMoreAndHide = viewCellUI.FindByName<Button>("ViewMoreAndHide");

            viewContent.IsVisible = !viewContent.IsVisible;

            viewMoreAndHide.Text = viewContent.IsVisible ? "Скрыть" : "Показать еще...";
        }

        /// <summary>
        /// Метод удаляем из списка новостей
        /// </summary>
        /// <param name="sender"></param>
        private void HideNews(object sender)
        {
            Debug.WriteLine("HideNews : " + sender);
            var model = sender as ModelObjectNews;
            DeleteNewsOrAddFavorites(model);
        }

        /// <summary>
        /// Метод добавляем в избранное
        /// </summary>
        /// <param name="sender"></param>
        private void AddFavorites(object sender)
        {
            Debug.WriteLine("AddFavorites : " + sender);
            var model = sender as ModelObjectNews;
            DeleteNewsOrAddFavorites(model, true);
        }

        /// <summary>
        /// Метод добавление в избранное и удаление из новостей
        /// </summary>
        /// <param name="modelObjectNews"></param>
        /// <param name="statusAddFavorites"></param>
        private void DeleteNewsOrAddFavorites(ModelObjectNews modelObjectNews, bool statusAddFavorites = false)
        {
            // Если true добавлем в избранное
            if(statusAddFavorites)
                SourceFavorites.Add(modelObjectNews);

            SourceNews.Remove(modelObjectNews);
            OnPropertyChanged("");
        }

        /// <summary>
        /// Метод обновление списка новостей
        /// </summary>
        private void RefreshListNews()
        {
            Debug.WriteLine("IsRefreshingNews : " + IsRefreshingNews);
            
            SourceFavorites.Clear();
            RequestGetNews();
        }

        /// <summary>
        /// Запрос получение новостей
        /// </summary>
        public async void RequestGetNews()
        {
            try
            {
                SourceNews.Clear();
                var response = await RequestResponse("news/get?count=20&page=1", null, OperationRequest.Get);

                Debug.WriteLine("RequestGetNews Success : " + response.Success);
                Debug.WriteLine("RequestGetNews Data : " + response.Data);

                if (!response.Success)
                    throw new Exception(response.Data);
                else
                {
                    var data = JsonConvert.DeserializeObject<List<ModelObjectNews>>(response.Data);

                    if (data.Count != 0)
                    {
                        SourceNews = new ObservableCollection<ModelObjectNews>(data);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            IsRefreshingNews = false;
            OnPropertyChanged("");
        }

        /// <summary>
        /// Метод обновление загаловка Bar Title
        /// </summary>
        /// <param name="currentPage"></param>
        public void UpdateTitleBar(int currentPage)
        {
            switch (currentPage)
            {
                case 0:
                    TextBarTitle = "Новости";
                    break;
                case 1:
                    TextBarTitle = "Избранное";
                    break;
            }
            OnPropertyChanged("TextBarTitle");
        }
    }
}
