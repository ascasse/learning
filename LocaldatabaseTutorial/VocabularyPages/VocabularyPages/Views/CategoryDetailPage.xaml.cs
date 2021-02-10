using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VocabularyPages.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VocabularyPages.Views
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public partial class CategoryDetailPage : ContentPage
    {
        private string itemId;

        public CategoryDetailPage()
        {
            InitializeComponent();
            //BindingContext = new ItemDetailViewModel(int.Parse(ItemId));
            //BindingContext = new ItemDetailViewModel();
        }

        public string ItemId 
        { 
            get => itemId;
            set 
            {
                itemId = Uri.UnescapeDataString(value ?? string.Empty);
                if (!String.IsNullOrEmpty(itemId))
                {
                    BindingContext = new ItemDetailViewModel(int.Parse(itemId));
                }
            } 
        }
    }
}