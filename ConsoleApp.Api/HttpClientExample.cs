using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ConsoleApp.Api
{
    public class HttpClientExample
    {
        private readonly HttpClient _client;

        private readonly string _postEndPoint = "https://jsonplaceholder.typicode.com/posts";

        public HttpClientExample()
        {
            _client = new HttpClient();
        }

        public async Task Read()
        {
            var response = await _client.GetAsync(_postEndPoint);
            if (response.IsSuccessStatusCode)
            {
                string jStr = await response.Content.ReadAsStringAsync();
                Console.WriteLine(jStr);
            }
        }

        public async Task Edit(int id)
        {
            var response = await _client.GetAsync($"{_postEndPoint}/{id}");
            if(response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                Console.WriteLine("Not Found!");
                return;
            }
            if (response.IsSuccessStatusCode)
            {
                string jStr = await response.Content.ReadAsStringAsync();
                Console.WriteLine(jStr);
            }
        }

        public async Task Create(int userId,string title,string body)
        {
            Post requestModel = new Post()
            {
                userId = userId,
                title = title,
                body= body
            };
            string jStr=JsonConvert.SerializeObject(requestModel);
            var content= new StringContent(jStr, Encoding.UTF8, Application.Json);
            var response= await _client.PostAsync(_postEndPoint,content);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine(await response.Content.ReadAsStringAsync());
            }
        }

        public async Task Update(int id,string title,string body,int userId)
        {
            Post requestModel = new Post()
            {
                id = id,
                title = title,
                body = body,
                userId = userId
            };

            string jStr=JsonConvert.SerializeObject(requestModel);
            var content = new StringContent(jStr,Encoding.UTF8,Application.Json);
            var  response=await _client.PutAsync($"{_postEndPoint}/{id}", content);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine(await response.Content.ReadAsStringAsync());
            }
        }

        public async Task Delete(int id)
        {
            var response=await _client.DeleteAsync($"{_postEndPoint}/{id}");
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Deleted!");
            }
        }


    }


    public class Post
    {
        public int userId { get; set; }
        public int id { get; set; }
        public string title { get; set; }
        public string body { get; set; }
    }

}
