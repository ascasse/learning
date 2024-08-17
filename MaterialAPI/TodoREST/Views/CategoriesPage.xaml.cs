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

	async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
	{

	}
}