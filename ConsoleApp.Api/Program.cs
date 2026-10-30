// See https://aka.ms/new-console-template for more information
using ConsoleApp.Api;

Console.WriteLine("Hello, World!");

HttpClientExample httpClient = new HttpClientExample();
//await httpClient.Read();
//await httpClient.Edit(1);
//await httpClient.Create(1, "string title", "string body");
//await httpClient.Update(1, "string title", "string body",1);
//await httpClient.Delete(1);

RestClientExample restClientExample = new RestClientExample();
//await restClientExample.Read();
//await restClientExample.Edit(2);
await restClientExample.Update(1, 1, "update testing title", "update testing body");
await restClientExample.Delete(1);