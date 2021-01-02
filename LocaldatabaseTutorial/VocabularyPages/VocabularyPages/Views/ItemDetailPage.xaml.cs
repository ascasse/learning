using System.ComponentModel;
using VocabularyPages.ViewModels;
using Xamarin.Forms;

namespace VocabularyPages.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}