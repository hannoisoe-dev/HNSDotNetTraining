using Newtonsoft.Json;

namespace HNSDotNetTraining.RegionMinimalAPI.EndPoint
{
    public static class Region
    {
        public static void UseRegionEndPoint(this IEndpointRouteBuilder app)
        {
           
                app.MapGet("/Region", () =>
                {
                    string folder = "Data/region.json";
                    var jsonStr = File.ReadAllText(folder);
                    var result = JsonConvert.DeserializeObject<RegionResponseModel>(jsonStr);
                    return Results.Ok(result.Property1);

                })
                .WithName("GetRegion")
                .WithOpenApi();

                app.MapGet("/Region/{id}", (int id) =>
                {
                    string folder = "Data/region.json";
                    var jsonStr = File.ReadAllText(folder);
                    RegionResponseModel result = JsonConvert.DeserializeObject<RegionResponseModel>(jsonStr)!;
                    var lst = result.Property1.Where(x => x.id == id);
                    return lst is not null ? Results.Ok(lst) : Results.BadRequest("Not found!");
                })
                .WithName("GetRegionById")
                .WithOpenApi();

            }


        }
    }

