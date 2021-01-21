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
    public class CategoriesViewModel : BaseViewModel
    {
        private Category _selectedItem;

        public ObservableCollection<Category> Categories { get; }
        public Command LoadItemsCommand { get; }
        public Command AddItemCommand { get; }
        public Command<Category> ItemTapped { get; }

        public CategoriesViewModel()
        {
            Title = "Categories";
            Categories = new ObservableCollection<Category>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            ItemTapped = new Command<Category>(OnItemSelected);

            AddItemCommand = new Command(OnAddItem);
        }

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                Categories.Clear();
                var recent = await WordData.GetItemsAsync();
                foreach (Vocabulary.Model.Category item in recent)
                {
                    Categories.Add(MapCategory(item));
                }
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

        public void OnAppearing()
        {
            IsBusy = true;
            SelectedItem = null;
        }

        public Category SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
                OnItemSelected(value);
            }
        }

        private async void OnAddItem(object obj)
        {
            await Shell.Current.GoToAsync(nameof(NewCategoryPage));
        }

        async void OnItemSelected(Category item)
        {
            if (item == null)
                return;

            // This will push the ItemDetailPage onto the navigation stack
            await Shell.Current.GoToAsync($"{nameof(CategoryDetailPage)}?{nameof(ItemDetailViewModel.ItemId)}={item.Id}");
        }

        Category MapCategory(Vocabulary.Model.Category category)
        {
            Category ctg = new Category()
            {
                Id = category.Id,
                Name = category.Name,
                LastUse = category.LastUse,
                Words = new List<Word>()
            };
            if (category.Words != null)
            {
                foreach (Vocabulary.Model.Word w in category.Words)
                {
                    ctg.Words.Add(new Word()
                    {
                        Id = w.Id,
                        Text = w.Text,
                        LastUse = w.LastUse,
                        Views = w.Views
                    });
                }
            }
            return ctg;
        }
    }
}
