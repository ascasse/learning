using System.Net.Http.Headers;
using System.Text.Json.Serialization;

using HttpClient client = new();
client.DefaultRequestHeaders.Accept.Clear();
//client.DefaultRequestHeaders.Accept.Add(
//    new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
//client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

await ProcessRepositoriesAsync(client);

static async Task ProcessRepositoriesAsync(HttpClient client)
{
    var json = await client.GetStringAsync("https://localhost:7219/api/Category");

    json = await client.GetStringAsync("https://localhost:7219/api/Category/1");    
    Console.Write(json);
}