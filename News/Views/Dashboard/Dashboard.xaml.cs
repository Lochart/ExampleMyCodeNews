namespace News
{
    public partial class Dashboard : Xamarin.Forms.TabbedPage
    {
        private ViewModelDashboard viewModel;

        public Dashboard()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            viewModel = BindingContext as ViewModelDashboard;

            viewModel?.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            viewModel = BindingContext as ViewModelDashboard;

            viewModel?.OnDisappearing();
        }

        protected override void OnCurrentPageChanged()
        {
            base.OnCurrentPageChanged();

            var indexPage = Children.IndexOf(CurrentPage);

            if (viewModel != null)
                viewModel.UpdateTitleBar(indexPage);

            SelectedItem = Children[indexPage];
        }
    }
}
