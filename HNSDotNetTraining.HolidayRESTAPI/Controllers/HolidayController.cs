using HNSDotNetTraining.HolidayRESTAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace HNSDotNetTraining.HolidayRESTAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HolidayController : ControllerBase
    {
        [HttpPost]

        public IActionResult CreateHoliday(Holiday model)
        {
            //string folderPath = "Data/holidays-us.json";
            //string jsonStr = System.IO.File.ReadAllText(folderPath);
            //var obj = JsonConvert.DeserializeObject<HolidayResponseModel>(jsonStr);

            //AppDbContext db = new AppDbContext();

            //foreach (var item in obj.holidays)
            //{
            //    db.TblHolidays.Add(new TblHoliday
            //    {
            //        Name = item.name,
            //        Date = item.date,
            //        Type = item.type,
            //        Observed = item.observed,
            //        Paid = item.paid
            //    });
            //    db.SaveChanges();
            //}

            //return Ok("Created!");


            AppDbContext db = new AppDbContext();
            db.TblHolidays.Add(new TblHoliday
            {
                Name = model.name,
                Date = model.date,
                Type = model.type,
                Observed = model.observed,
                Paid = model.paid
            });
            db.SaveChanges();

            return Ok("Created!");

        }

        [HttpPut("{id}")]

        public IActionResult UpdateHoliday(int id,Holiday model)
        {
            AppDbContext db = new AppDbContext();
            {
                var lst=db.TblHolidays.FirstOrDefault(x => x.Id == id);
                if(lst is null)
                {
                    return BadRequest("Not found!");
                }

                lst.Name = model.name;
                lst.Date = model.date;
                lst.Type = model.type;
                lst.Observed = model.observed;
                lst.Paid = model.paid;

                db.SaveChanges();

                return Ok("Updated!");
            }

        }

        [HttpPatch("{id}")]

        public IActionResult PatchUpdate(int id,Holiday holiday)
        {
            AppDbContext db = new AppDbContext();
            var lst=db.TblHolidays.FirstOrDefault(x => x.Id == id);
            if(lst is null)
            {
                return BadRequest("Not Found!");
            }

            if (!holiday.name.IsNullOrEmpty())
            {
               lst.Name = holiday.name;
            }
            if (!holiday.date.IsNullOrEmpty())
            {
               lst.Date = holiday.date;
            }
            if (!holiday.type.IsNullOrEmpty())
            {
               lst.Type = holiday.type;
            }

           int result= db.SaveChanges();

            return Ok("Updated!");
            
        }

        [HttpDelete("{id}")]
        
        public IActionResult DeleteHolidays(int id)
        {
            AppDbContext db = new AppDbContext();
            var lst =db.TblHolidays.FirstOrDefault(x => x.Id == id);
            if(lst is null)
            {
                return BadRequest("Not Found!");
            }

            db.TblHolidays.Remove(lst);
            db.SaveChanges();
            return Ok("Deleted!");
        }
        


        [HttpGet]

        public IActionResult GetHolidays()
        {
            AppDbContext db = new AppDbContext();
            var lst = db.TblHolidays.ToList();
            return Ok(lst);
        }

        [HttpGet("{id}")]

        public IActionResult GetHolidaysById(int id)
        {
            AppDbContext db = new AppDbContext();
            var result = db.TblHolidays.Where(x => x.Id == id).ToList();
            return Ok(result);
        }

        //[HttpPost]

        //public IActionResult Create(Holiday model)
        //{
        //        AppDbContext db = new AppDbContext();
        //        db.TblHolidays.Add(new TblHoliday
        //        {
        //            Name = model.name,
        //            Date = model.date,
        //            Type = model.type,
        //            Observed = model.observed,
        //            Paid = model.paid
        //        });
        //        db.SaveChanges();

        //        return Ok("Created!");
        //    }
        //}


       
        public class HolidayResponseModel
        {
            public Holiday[] holidays { get; set; }
        }

        public class Holiday
        {
            public string? name { get; set; }
            public string? date { get; set; }
            public string? type { get; set; }
            public bool observed { get; set; }
            public bool paid { get; set; }
        }

    }

}

