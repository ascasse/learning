// See https://aka.ms/new-console-template for more information
using Learning;
using Learning.Model;
using System.Configuration;

// Find the database file
string database = ConfigurationManager.AppSettings["Database"]??string.Empty;
if (string.IsNullOrEmpty(database))
{
    Console.WriteLine("Could not read configuration.");
    Environment.Exit(1);
}

Service service = new Service(database)
{
    BatchSize = 5,
    MaxViews = 3,
    RefreshRate = 3
};

List<Category> categories = await service.GetCategories();

Console.WriteLine($"Read {categories.Count} categories.");

Category ctg = categories[0];
Console.WriteLine($"Using category {ctg.Name}");

int iteration = 0;
while (iteration < 10)
{
    Category batch = service.BuildBatchFromCategory(ctg);
    if (batch.Items == null)
        break;
    var words = batch.Items.Select(w => w.Text);
    Console.WriteLine($"Iteration {++iteration}");
    Console.WriteLine(string.Join(",", words));
    await service.UpdateBatch(batch);
}
Console.WriteLine($"Completed");
ctg = service.GetCategory(ctg.Id).Result;
var views = ctg.Items.Select(w => w.Views);
Console.WriteLine(string.Join(",", views));
await service.Close();