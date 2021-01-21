using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Vocabulary.Model;
using Xamarin.Forms;

namespace VocabularyPages.ViewModels
{
    public class NewCategoryViewModel : BaseViewModel
    {
        public string Name { get; set; }

        public ObservableCollection<string> Words { get; set; }

        public NewCategoryViewModel()
        {
            Title = "New category";
            Words = new ObservableCollection<string>();
            Words.Add("aaaa");

            SaveCommand = new Command(OnSave, ValidateSave);
            CancelCommand = new Command(OnCancel);
            AddWordCommand = new Command(OnAddWord);

            //AddWordCommand = new Command(() =>
            //{
            //    Words.Add(NewWord);
            //    NewWord = string.Empty;
            //});

            this.PropertyChanged +=
                (_, __) => SaveCommand.ChangeCanExecute();
        }

        private bool ValidateSave()
        {
            return !String.IsNullOrWhiteSpace(Name)
                && Words.Count > 0;
        }

        public Command SaveCommand { get; }

        public Command CancelCommand { get; }

        public Command AddWordCommand { get; }

        private async void OnCancel()
        {
            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }

        private async void OnSave()
        {
            Category newCategory = new Category()
            {
                Name = Name,
                LastUse = DateTime.MinValue,
                Words = new List<Word>()
            };

            foreach (string s in Words)
            {
                newCategory.Words.Add(new Word() { Text = s });
            }

            await WordData.AddItemAsync(newCategory);

            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }

        string newWord;
        public string NewWord
        {
            get => newWord;
            set => SetProperty<string>(ref newWord, value);
        }

        private void OnAddWord()
        {
            Words.Add(NewWord);
            NewWord = string.Empty;
        }

    }
}
