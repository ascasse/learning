// See https://aka.ms/new-console-template for more information
using System.Configuration;
using System.Drawing.Printing;
using Learning;
using Learning.Model;

string database = ConfigurationManager.AppSettings["Database"]??string.Empty;

if (string.IsNullOrEmpty(database))
{
    Console.WriteLine("Could not read database info.");
    Environment.Exit(1);
}

if (!File.Exists(database))
{
    Console.WriteLine("Could not read database info.");
    Environment.Exit(2);
}

Service service = new Service(database);
List<ItemCategory> categories = await service.GetCategoriesComplete();

IEnumerable<int> ids = categories.Select(x => x.Id).Distinct();
Console.WriteLine($"Found {ids.Count()} categories.");
foreach (int id in ids)
{
    string categoryName = categories.FirstOrDefault(c => c.Id == id)?.Name ?? string.Empty;
    if (categoryName != string.Empty)
    {
        Console.WriteLine(categoryName);
        IEnumerable<Item> items = categories.Where(x => x.Id == id).Select(item => new Item { Text = item.Text, Image = item.Image });
        foreach (var item in items)
            Console.WriteLine($"   {item.Text}\t\t\t{item.Image}");
    }
}
