using Newtonsoft.Json;
using System.Net.Http.Json;
namespace KooliProjekt.WindowsForms.Api

{

    public class ApiClient : IApiClient

    {

        private readonly string _baseUrl;

        private readonly HttpClient _client;

        public ApiClient()

        {

            _baseUrl = "http://localhost:5086/api/Employees/";

            _client = new HttpClient();

        }

        public async Task<OperationResult<PagedResult<Employees>>> List(int page, int pageSize)

        {

            var url = _baseUrl + "List?page=" + page + "&pageSize=" + pageSize;

            using var request = new HttpRequestMessage(HttpMethod.Get, url);

            using var response = await _client.SendAsync(request);

            var body = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<OperationResult<PagedResult<Employees>>>(body);

            return result;

        }

        public async Task<OperationResult> Save(Employees list)

        {

            var url = _baseUrl + "Save";

            using var request = new HttpRequestMessage(HttpMethod.Post, url)

            {

                Content = JsonContent.Create(list)

            };

            using var response = await _client.SendAsync(request);

            var body = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<OperationResult>(body);

            return result;

        }

        public async Task<OperationResult> Delete(int id)

        {

            var url = _baseUrl + "Delete/?id=" + id;

            using var request = new HttpRequestMessage(HttpMethod.Delete, url);

            using var response = await _client.SendAsync(request);

            var body = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<OperationResult>(body);

            return result;

        }

    }

}
