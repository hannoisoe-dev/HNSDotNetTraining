using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Runtime.InteropServices.Marshalling;

namespace HNSDotNetTraining.MvcApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }


        //PL
        [ActionName("Index")]
        public async Task<IActionResult> ProductIndex()
        {
            var lst=await _productService.List();
            
            return View("ProductIndex",lst);
        }

        [ActionName("Create")]

        public async Task<IActionResult> ProductCreate()
        {
            return View("ProductCreate");
        }

        [ActionName("Update")]

        public async Task<IActionResult> ProductUpdate()
        {
            return View("ProductUpdate");
        }




        
        //BL + DAL
        [HttpPost]
        [ActionName("Save")]

        public async Task<IActionResult> ProductSave(ProductRequestModel requestModel)
        {
            var result=await _productService.CreateProduct(requestModel);
            string message = result ? "Created!" : "Failed!";
            return Json(new {
                IsSuccess = result,
                message = message
            });

        }


        [HttpPut]
        [ActionName("Update")]
        public async Task<IActionResult> ProductUpdate(int id,ProductRequestModel requestModel)
        {
           var result=await _productService.UpdateProduct(id, requestModel);
           string message = result ? "Updated!" : "Update Fail";
            return Json(new
            {
                IsSuccess = result,
                message = message
            });
        }

    }

    public class ProductService : IProductService
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public ProductService(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<List<ProductModel>> List()
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            var lst=await db.QueryAsync<ProductModel>("select * from Tbl_Product ");
            return lst.ToList();
        }

        public async Task<bool> CreateProduct(ProductRequestModel requestModel)
        {
            using IDbConnection db=new SqlConnection(_connectionString);
            string query = @"INSERT INTO [dbo].[Tbl_Product]
                               ([ProductName]
                               ,[Price]
                               ,[Quantity])
                         VALUES
                               (@Name
                               ,@Price
                               ,@Quantity)";
            var result =await db.ExecuteAsync(query, new
            {
                Name = requestModel.name,
                Price = requestModel.price,
                Quantity = requestModel.quantity
            });

            return result > 0 ? true: false;
        }


        public async Task<bool> UpdateProduct(int id,ProductRequestModel requestModel)
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            string query = @"UPDATE [dbo].[Tbl_Product]
                           SET [ProductName] = @Name
                              ,[Price] = @Price
                              ,[Quantity] = @Quantity
                         WHERE ProductId=@ProductId";
           var result=await  db.ExecuteAsync(query, new
            {
                Name = requestModel.name,
                Price = requestModel.price,
                Quantity = requestModel.quantity,
                ProductId = id
            });

            return result > 0;
        }

       
    }
    public interface IProductService
    {
        Task<List<ProductModel>> List();

        Task<bool> CreateProduct(ProductRequestModel requestModel);

        Task<bool> UpdateProduct(int id, ProductRequestModel requestModel);
    }


    public class ProductModel
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public string ProductCode { get; set; }
    }

    public class ProductRequestModel
    {
        public string name { get; set; }

        public decimal price { get; set; }

        public int quantity { get; set; }
    }
}
