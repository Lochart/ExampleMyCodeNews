using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace News
{
    public partial class ComponentsListNews : ContentView
    {
        /// <summary>
        /// Ресурсы списка
        /// </summary>
        public static readonly BindableProperty SourceListProperty =
            BindableProperty.Create(
                "SourceList", typeof(ObservableCollection<ModelObjectNews>),
                typeof(ComponentsListNews), new ObservableCollection<ModelObjectNews>());

        public ObservableCollection<ModelObjectNews> SourceList
        {
            get { return (ObservableCollection<ModelObjectNews>)GetValue(SourceListProperty); }
            set { SetValue(SourceListProperty, value); }
        }

        /// <summary>
        /// Swaping влево и вправо
        /// </summary>
        public static readonly BindableProperty IsSwapingProperty =
            BindableProperty.Create(
                "IsSwaping", typeof(bool),
                typeof(ComponentsListNews), false);

        public bool IsSwaping
        {
            get { return (bool)GetValue(IsSwapingProperty); }
            set { SetValue(IsSwapingProperty, value); }
        }

        ///// <summary>
        ///// Swaping влево и вправо
        ///// </summary>
        //public static readonly BindableProperty IsRefreshingListViewProperty =
        //    BindableProperty.Create(
        //        "IsRefreshingListView", typeof(bool),
        //        typeof(ComponentsListNews), false);

        //public bool IsRefreshingListView
        //{
        //    get { return (bool)GetValue(IsRefreshingListViewProperty); }
        //    set { SetValue(IsRefreshingListViewProperty, value); }
        //}

        public ComponentsListNews()
        {
            InitializeComponent();
        }
    }
}
