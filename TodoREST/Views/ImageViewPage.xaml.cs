using TodoREST.Models;

namespace TodoREST.Views;

[QueryProperty(nameof(Category), "Category")]
public partial class ImageViewPage : ContentPage
{

	Category _category;


	public Category Category { get => _category; set { _category = value; OnPropertyChanged(); } }
	public IEnumerable<Item> Items { get; private set; }

    public ImageViewPage()
	{
		InitializeComponent();
        BindingContext = this;
    }

	protected override void OnAppearing()
	{
		base.OnAppearing();
        //bits.SetBinding(ItemsView.ItemsSourceProperty, "Items");
    }
}