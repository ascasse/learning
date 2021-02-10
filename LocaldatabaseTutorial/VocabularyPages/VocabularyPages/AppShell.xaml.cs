using System;
using System.Collections.Generic;
using VocabularyPages.ViewModels;
using VocabularyPages.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace VocabularyPages
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
            Routing.RegisterRoute(nameof(CategoryDetailPage), typeof(CategoryDetailPage));
            Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
            Routing.RegisterRoute(nameof(NewCategoryPage), typeof(NewCategoryPage));
        }

        private async void OnMenuItemClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
