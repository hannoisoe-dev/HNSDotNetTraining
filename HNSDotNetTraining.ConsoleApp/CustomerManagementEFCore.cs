using HNSDotNetTraining.ConsoleApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace HNSDotNetTraining.ConsoleApp
{
    public class CustomerManagementEFCore : ICustomerManagement
    {
        public void Create(string customerName, string phoneNumber, string? email)
        {
            using AppDBContext db = new AppDBContext();
            var customer = new CustomerDataModel
            {
                CustomerName = customerName,
                PhoneNumber = phoneNumber,
                Email = email,
                DeleteFlag = false
            };

            db.Customers.Add(customer);
            int result = db.SaveChanges();

            string message = result > 0 
                ? $"Customer '{customerName}' registered successfully with ID: {customer.CustomerId}" 
                : "Failed to register customer.";
            Console.WriteLine(message);
        }

        public void Read()
        {
            using AppDBContext db = new AppDBContext();
            var customers = db.Customers.Where(c => !c.DeleteFlag).OrderBy(c => c.CustomerId).ToList();

            if (!customers.Any())
            {
                Console.WriteLine("No customers found.");
                return;
            }

            Console.WriteLine("==================== Customer List ====================");
            foreach (var c in customers)
            {
                Console.WriteLine($"ID: {c.CustomerId} | Name: {c.CustomerName} | Phone: {c.PhoneNumber} | Email: {c.Email ?? "N/A"}");
            }
            Console.WriteLine("=======================================================");
        }

        public void Edit(int customerId)
        {
            using AppDBContext db = new AppDBContext();
            var customer = db.Customers.FirstOrDefault(c => c.CustomerId == customerId && !c.DeleteFlag);

            if (customer == null)
            {
                Console.WriteLine($"Customer with ID {customerId} not found!");
                return;
            }

            Console.WriteLine($"Customer ID: {customer.CustomerId}");
            Console.WriteLine($"Name: {customer.CustomerName}");
            Console.WriteLine($"Phone: {customer.PhoneNumber}");
            Console.WriteLine($"Email: {customer.Email ?? "N/A"}");
        }

        public void Update(int customerId, string customerName, string phoneNumber, string? email)
        {
            using AppDBContext db = new AppDBContext();
            var customer = db.Customers.FirstOrDefault(c => c.CustomerId == customerId && !c.DeleteFlag);

            if (customer == null)
            {
                Console.WriteLine("Customer not found!");
                return;
            }

            customer.CustomerName = customerName;
            customer.PhoneNumber = phoneNumber;
            customer.Email = email;

            db.Entry(customer).State = EntityState.Modified;
            int result = db.SaveChanges();

            string message = result > 0 ? "Customer updated successfully." : "Failed to update customer.";
            Console.WriteLine(message);
        }

        public void ViewHistory(int customerId)
        {
            using AppDBContext db = new AppDBContext();
            var customer = db.Customers
                             .Include(c => c.Appointments.Where(a => !a.DeleteFlag))
                                .ThenInclude(a => a.Service)
                             .Include(c => c.Appointments.Where(a => !a.DeleteFlag))
                                .ThenInclude(a => a.Therapist)
                             .Include(c => c.Appointments.Where(a => !a.DeleteFlag))
                                .ThenInclude(a => a.Payments)
                             .FirstOrDefault(c => c.CustomerId == customerId && !c.DeleteFlag);

            if (customer == null)
            {
                Console.WriteLine($"Customer with ID {customerId} not found!");
                return;
            }

            Console.WriteLine($"\n================ Appointment History for {customer.CustomerName} (Phone: {customer.PhoneNumber}) ================");
            if (!customer.Appointments.Any())
            {
                Console.WriteLine("No appointment history for this customer.");
                return;
            }

            foreach (var a in customer.Appointments.OrderByDescending(a => a.AppointmentDate).ThenByDescending(a => a.StartTime))
            {
                string therapist = a.Therapist != null ? a.Therapist.TherapistName : "Unassigned";
                string service = a.Service != null ? a.Service.ServiceName : "Unknown";
                decimal price = a.Service != null ? a.Service.Price : 0;

                Console.WriteLine($"Appt #{a.AppointmentId} | Date: {a.AppointmentDate:yyyy-MM-dd} {a.StartTime:hh\\:mm}-{a.EndTime:hh\\:mm} | Service: {service} (${price:F2}) | Therapist: {therapist} | Status: {a.Status} | Payment: {a.PaymentStatus}");
            }
            Console.WriteLine("===================================================================================================");
        }
    }
}
