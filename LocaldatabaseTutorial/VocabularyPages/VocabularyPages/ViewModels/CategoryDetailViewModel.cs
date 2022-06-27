using System;
using System.Collections.Generic;
using System.Text;
using VocabularyPages.Models;
using Xamarin.Forms;

namespace VocabularyPages.ViewModels
{
    public class CategoryDetailViewModel : BaseViewModel
    {
        public CategoryDetailViewModel() { }

        public CategoryDetailViewModel(int id) 
        {
            Id = id;
        }

        public int Id 
        { 
            set 
            {
                var item = WordData.GetItemAsync(value).Result;
                Category = new Category(item);
            }
        }

        public Category Category { get; set; }
            
    }
}
