using TodoREST.Models;
using TodoREST.Services;

namespace TodoREST.Views;

public partial class CategoriesPage : ContentPage
{
	ILearningService _learningService;

	public CategoriesPage(ILearningService service)
	{
		InitializeComponent();
		_learningService = service;
	}

	protected async override void OnAppearing()
	{
		base.OnAppearing();
		categoriesView.ItemsSource = await _learningService.GetCategoriesAsync();
	}

	async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		var navigatorParameter = new Dictionary<string, object>
		{
			{ nameof(Category), e.CurrentSelection.FirstOrDefault() as Category }
		};
		await Shell.Current.GoToAsync(nameof(ImageViewPage), navigatorParameter);
	}
}