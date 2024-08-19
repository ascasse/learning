using System.Diagnostics;
using System.Text;
using System.Text.Json;
using TodoREST.Models;

namespace TodoREST.Services
{
    class LearningRestService : ILearningRestService
    {
        HttpClient _client;
        JsonSerializerOptions _serializerOptions;
        IHttpsClientHandlerService _httpsClientHandlerService;

        public List<Category> Categories { get; set; }

        public LearningRestService(IHttpsClientHandlerService service)
        {
#if DEBUG
            _httpsClientHandlerService = service;
            HttpMessageHandler handler = _httpsClientHandlerService.GetPlatformMessageHandler();
            if (handler != null)
                _client = new HttpClient(handler);
            else
                _client = new HttpClient();
#else
            _client = new HttpClient();
#endif            
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            Categories = [];

            Uri uri = new Uri(string.Format(Constants.LearningRestUrl, string.Empty));
            try
            {
                HttpResponseMessage response = await _client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    Categories = JsonSerializer.Deserialize<List<Category>>(content, _serializerOptions);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($@"\tERROR {ex.Message}");
            }
            return Categories;
        }

        public async Task SaveCategoryAsync(Category category, bool isNewItem)
        {
            Uri uri = new Uri(string.Format(Constants.LearningRestUrl, string.Empty));

            try
            {
                string json = JsonSerializer.Serialize<Category>(category, _serializerOptions);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = null;
                if (isNewItem)
                    response = await _client.PostAsync(uri, content);
                else
                    response = await _client.PutAsync(uri, content);

                if (response.IsSuccessStatusCode)
                    Debug.WriteLine(@"\tTodoItem successfully saved.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }
        }

        public async Task DeleteCategoryAsync(int id)
        {
            Uri uri = new Uri(string.Format(Constants.LearningRestUrl, id));

            try
            {
                HttpResponseMessage response = await _client.DeleteAsync(uri);
                if (response.IsSuccessStatusCode)
                    Debug.WriteLine(@"\tCategory successfully deleted.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }
        }
    }
}
