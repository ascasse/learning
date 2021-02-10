using System;
using System.Diagnostics;
using System.Threading.Tasks;
using VocabularyPages.Models;
using Xamarin.Forms;

namespace VocabularyPages.ViewModels
{
    public class ItemDetailViewModel : BaseViewModel
    {
        public ItemDetailViewModel() { }

        public ItemDetailViewModel (int id)
        {
            ItemId = id;
        }

        public Category Category { get; set; }

        private int itemId;
        private string name;
        private string description;
        public int Id { get; set; }


        //public string Name
        //{
        //    get => name;
        //    set => SetProperty(ref name, value);
        //}

        //public string Description
        //{
        //    get => description;
        //    set => SetProperty(ref description, value);
        //}

        public int ItemId
        {
            get => itemId;
            set
            {
                itemId = value;
                LoadItemId(value);
            }
        }

        public async void LoadItemId(int itemId)
        {
            try
            {
                var item = await WordData.GetItemAsync(itemId);
                Category = new Category(item);
                //Id = ctg.Id;
                //Name = ctg.Name;
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }
    }
}
