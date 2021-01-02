using System;
using System.Collections.Generic;
using System.ComponentModel;
using VocabularyPages.Models;
using VocabularyPages.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VocabularyPages.Views
{
    public partial class NewItemPage : ContentPage
    {
        public Item Item { get; set; }

        public NewItemPage()
        {
            InitializeComponent();
            BindingContext = new NewItemViewModel();
        }
    }
}