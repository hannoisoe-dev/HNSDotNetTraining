using HNSDotNetTraining.ConsoleApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace HNSDotNetTraining.ConsoleApp
{
    public class SpaServiceManagementEFCore : ISpaServiceManagement
    {
        public void Create(string serviceName, string? description, int durationMinutes, decimal price)
        {
            using AppDBContext db = new AppDBContext();
            var service = new SpaServiceDataModel
            {
                ServiceName = serviceName,
                Description = description,
                DurationMinutes = durationMinutes,
                Price = price,
                DeleteFlag = false
            };

            db.SpaServices.Add(service);
            int result = db.SaveChanges();

            string message = result > 0 
                ? $"Spa Service '{serviceName}' created successfully with ID: {service.ServiceId} (${price:F2}, {durationMinutes} mins)." 
                : "Failed to create spa service.";
            Console.WriteLine(message);
        }

        public void Read()
        {
            using AppDBContext db = new AppDBContext();
            var services = db.SpaServices.Where(s => !s.DeleteFlag).OrderBy(s => s.ServiceId).ToList();

            if (!services.Any())
            {
                Console.WriteLine("No spa services found.");
                return;
            }

            Console.WriteLine("==================== Spa Services List ====================");
            foreach (var s in services)
            {
                Console.WriteLine($"ID: {s.ServiceId} | {s.ServiceName} | {s.DurationMinutes} mins | ${s.Price:F2} | {s.Description ?? "No description"}");
            }
            Console.WriteLine("===========================================================");
        }

        public void Edit(int serviceId)
        {
            using AppDBContext db = new AppDBContext();
            var service = db.SpaServices.FirstOrDefault(s => s.ServiceId == serviceId && !s.DeleteFlag);

            if (service == null)
            {
                Console.WriteLine($"Service with ID {serviceId} not found!");
                return;
            }

            Console.WriteLine($"Service ID: {service.ServiceId}");
            Console.WriteLine($"Service Name: {service.ServiceName}");
            Console.WriteLine($"Description: {service.Description ?? "N/A"}");
            Console.WriteLine($"Duration: {service.DurationMinutes} minutes");
            Console.WriteLine($"Price: ${service.Price:F2}");
        }

        public void Update(int serviceId, string serviceName, string? description, int durationMinutes, decimal price)
        {
            using AppDBContext db = new AppDBContext();
            var service = db.SpaServices.FirstOrDefault(s => s.ServiceId == serviceId && !s.DeleteFlag);

            if (service == null)
            {
                Console.WriteLine("Service not found!");
                return;
            }

            service.ServiceName = serviceName;
            service.Description = description;
            service.DurationMinutes = durationMinutes;
            service.Price = price;

            db.Entry(service).State = EntityState.Modified;
            int result = db.SaveChanges();

            string message = result > 0 ? "Service updated successfully." : "Failed to update service.";
            Console.WriteLine(message);
        }

        public void Delete(int serviceId)
        {
            using AppDBContext db = new AppDBContext();
            var service = db.SpaServices.FirstOrDefault(s => s.ServiceId == serviceId && !s.DeleteFlag);

            if (service == null)
            {
                Console.WriteLine("Service not found!");
                return;
            }

            service.DeleteFlag = true;
            int result = db.SaveChanges();

            string message = result > 0 ? "Service deleted successfully." : "Failed to delete service.";
            Console.WriteLine(message);
        }
    }
}
