using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using VocabularyPages.Models;
using VocabularyPages.Views;
using Xamarin.Forms;

namespace VocabularyPages.ViewModels
{
    public class RecentViewModel : BaseViewModel
    {
        public Category SelectedItem { get; set; }

        /// <summary>Recently used categories</summary>
        public ObservableCollection<Category> RecentCategories { get; }

        public Command LoadRecentCommand { get; }

        public RecentViewModel()
        {
            Title = "Recent";
            RecentCategories = new ObservableCollection<Category>();

            LoadRecentCommand = new Command(async () => await ExecuteLoadRecentCommand());
        }

        async Task ExecuteLoadRecentCommand()
        {
            IsBusy = true;

            try
            {
                RecentCategories.Clear();
                var recent = await WordData.GetRecentAsync(true);
                foreach (Vocabulary.Model.Category item in recent)
                {
                    RecentCategories.Add(MapCategory(item));
                }
                NoRecentFound = RecentCategories.Count == 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async void OnCategoriesClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(CategoriesPage));
        }

        public bool NoRecentFound { get; set; }

        public void OnAppearing()
        {
            IsBusy = true;
            SelectedItem = null;
        }

        private Category MapCategory(Vocabulary.Model.Category category)
        {
            return new Category()
            {
                Id = category.Id,
                Name = category.Name,
            };
        }
    }
}
