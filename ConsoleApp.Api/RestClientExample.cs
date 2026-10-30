using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.Api
{
    public class RestClientExample
    {
        private readonly RestClient _client;
        private readonly string _postEndPoint = "https://jsonplaceholder.typicode.com/posts";

        public RestClientExample()
        {
            _client = new RestClient();
        }

        public async Task Read()
        {
            RestRequest request = new RestRequest(_postEndPoint, Method.Get);
            var response = await _client.GetAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.Content);
            }
        }

        public async Task Edit(int id)
        {
            RestRequest request = new RestRequest($"{_postEndPoint}/{id}", Method.Get);
            var response = await _client.GetAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.Content);
            }
        }

        public async Task Update(int id,int userid,string title,string body)
        {
            PostModel post = new PostModel()
            {
                id = id,
                userId = userid,
                title = title,
                body = body
            };
            RestRequest request = new RestRequest($"{_postEndPoint}/{id}", Method.Put);
            request.AddJsonBody(post);
            var response=await _client.ExecuteAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.Content);
            }
        }

        public async Task Delete(int id)
        {
            RestRequest request = new RestRequest($"{_postEndPoint}/{id}", Method.Delete);
            var response=await _client.ExecuteAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Deleted!");
            }
        }
    }


    public class PostModel

    {
        public int userId { get; set; }
        public int id { get; set; }
        public string title { get; set; }
        public string body { get; set; }
    }

}
