using MaterialAPI.Data;
using MaterialAPI.Model;
using MaterialAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NuGet.Protocol.Plugins;
using System.Configuration;
using System.Net.Http.Json;
using System.Net.NetworkInformation;
using static System.Net.WebRequestMethods;

const string BATCH_URL = "https://localhost:7219/api/batch";

using HttpClient client = new();
client.DefaultRequestHeaders.Accept.Clear();

await ProcessRepositoriesAsync(client);

Console.ReadLine();

//HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
//builder.Services.AddDbContext<MaterialAPIContext>(options =>
//    options.UseSqlite(builder.Configuration.GetConnectionString("MaterialAPIContext") ?? throw new InvalidOperationException("Connection string 'MaterialAPIContext' not found.")));

//builder.Services.AddScoped<IService, Service>();

//using IHost host = builder.Build();

////await host.RunAsync();

//RunLocal(host.Services);


static void RunLocal(IServiceProvider hostProvider)
{
    using IServiceScope serviceScope = hostProvider.CreateScope();
    IServiceProvider provider = serviceScope.ServiceProvider;
    IService service = provider.GetRequiredService<IService>();

    service.CheckDatabase();

    Category ctg = service.BuildBatchFromCategory(1);
    Console.WriteLine($"Category {ctg.Name}");
    Console.WriteLine($"{ctg.Items.Count} items in category.");
    Console.WriteLine("---------------------------------------------------");
}



static Category GetBatchFromCategory(int id, HttpClient client)
{
    return client.GetFromJsonAsync<Category>($"{BATCH_URL}/{id}").Result;
    //var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, $"{BATCH_URL}/{id}"));
    //HttpResponseMessage msg = response..EnsureSuccessStatusCode();
}

static async void UpdateCategoryAsync(Category ctg, HttpClient client) 
{
    var response = await client.PutAsJsonAsync($"{BATCH_URL}/", ctg);
    HttpResponseMessage msg = response.EnsureSuccessStatusCode();
}

static async void ResetCategoryAsync(int id, HttpClient client)
{
    var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Put, $"{BATCH_URL}/reset/{id}"));
    HttpResponseMessage msg = response.EnsureSuccessStatusCode();
}

static async Task ProcessRepositoriesAsync(HttpClient client)
{
    //var json = await client.GetStringAsync("https://localhost:7219/api/Category");

    //json = await client.GetStringAsync("https://localhost:7219/api/Category/1");
    //Console.Write(json);



    var ctg = await client.GetFromJsonAsync<Category>("https://localhost:7219/api/Category/1");
    Console.WriteLine($"Category {ctg.Name}");
    Console.WriteLine($"{ctg.Items.Count} items in category.");
    Console.WriteLine("---------------------------------------------------");
    int iteration = 1;
    while(iteration < 10)
    {
        Console.WriteLine($"Iteration: {iteration}");
        var batch = GetBatchFromCategory(1, client);
        Console.WriteLine(batch.Name);
        foreach (Item w in batch.Items)
            Console.WriteLine($"{w.Text}\t{w.Views}");

        UpdateCategoryAsync(batch, client);
        iteration += 1;
        Console.WriteLine("---------------------------------------------------");
    }
    /*
            Category ctg = service.GetCategory(1).Result;
            Console.WriteLine($"Category {ctg.Name}.");
            Console.WriteLine($"{ctg.Items.Count} words in category.");
            foreach (Item w in ctg.Items)
            {
                Console.WriteLine(w.Text);
            }
            int iteration = 0;
            while (iteration < 10)
            {
                Category batch = service.BuildBatchFromCategory(ctg);
                if (batch.Items == null)
                    break;
                var words = batch.Items.Select(w => w.Text);
                Console.WriteLine($"Iteration { ++iteration }");
                Console.WriteLine(string.Join(",", words));
                await service.UpdateBatch(batch);
            }
            Console.WriteLine($"Completed");
            ctg = service.GetCategory(1).Result;
            var views = ctg.Items.Select(w => $"{w.Text}: {w.Views}");
            foreach (string view in views)
                Console.WriteLine(view);
            await service.Close();
     */
}