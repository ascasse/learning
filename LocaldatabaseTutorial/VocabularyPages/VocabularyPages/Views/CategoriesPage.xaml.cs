using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VocabularyPages.Models;
using VocabularyPages.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VocabularyPages.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CategoriesPage : ContentPage
    {
        CategoriesViewModel _viewModel;

        public CategoriesPage()
        {
            InitializeComponent();
            BindingContext = _viewModel = new CategoriesViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }

        async void CategoriesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Category selected = (Category)e.CurrentSelection.FirstOrDefault();
            if (selected == null)
                return;
            await Shell.Current.GoToAsync($"{nameof(CategoryDetailPage)}?id={selected.Id}");
        }
    }
}