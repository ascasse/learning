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
    [QueryProperty("CategoryId", "id")]
    public partial class CategoryDetailPage : ContentPage
    {
        public string CategoryId
        {
            set
            {
                BindingContext = new CategoryDetailViewModel(int.Parse(Uri.UnescapeDataString(value)));
            }
        }

        public CategoryDetailPage()
        {
            InitializeComponent();
        }
    }
}