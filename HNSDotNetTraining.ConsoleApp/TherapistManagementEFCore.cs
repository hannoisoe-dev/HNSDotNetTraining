using HNSDotNetTraining.ConsoleApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace HNSDotNetTraining.ConsoleApp
{
    public class TherapistManagementEFCore : ITherapistManagement
    {
        public void Create(string therapistName, string specialty, string workingHours = "09:00 - 18:00")
        {
            using AppDBContext db = new AppDBContext();
            var therapist = new TherapistDataModel
            {
                TherapistName = therapistName,
                Specialty = specialty,
                WorkingHours = workingHours,
                IsAvailable = true,
                DeleteFlag = false
            };

            db.Therapists.Add(therapist);
            int result = db.SaveChanges();

            string message = result > 0 
                ? $"Therapist '{therapistName}' registered successfully with ID: {therapist.TherapistId} (Specialty: {specialty})." 
                : "Failed to register therapist.";
            Console.WriteLine(message);
        }

        public void Read()
        {
            using AppDBContext db = new AppDBContext();
            var therapists = db.Therapists.Where(t => !t.DeleteFlag).OrderBy(t => t.TherapistId).ToList();

            if (!therapists.Any())
            {
                Console.WriteLine("No therapists found.");
                return;
            }

            Console.WriteLine("==================== Therapist List ====================");
            foreach (var t in therapists)
            {
                string availStr = t.IsAvailable ? "Available" : "Not Available";
                Console.WriteLine($"ID: {t.TherapistId} | Name: {t.TherapistName} | Specialty: {t.Specialty} | Hours: {t.WorkingHours} | Status: {availStr}");
            }
            Console.WriteLine("========================================================");
        }

        public void Edit(int therapistId)
        {
            using AppDBContext db = new AppDBContext();
            var therapist = db.Therapists.FirstOrDefault(t => t.TherapistId == therapistId && !t.DeleteFlag);

            if (therapist == null)
            {
                Console.WriteLine($"Therapist with ID {therapistId} not found!");
                return;
            }

            Console.WriteLine($"Therapist ID: {therapist.TherapistId}");
            Console.WriteLine($"Name: {therapist.TherapistName}");
            Console.WriteLine($"Specialty: {therapist.Specialty}");
            Console.WriteLine($"Working Hours: {therapist.WorkingHours}");
            Console.WriteLine($"Availability: {(therapist.IsAvailable ? "Available" : "Unavailable")}");
        }

        public void Update(int therapistId, string therapistName, string specialty, string workingHours, bool isAvailable)
        {
            using AppDBContext db = new AppDBContext();
            var therapist = db.Therapists.FirstOrDefault(t => t.TherapistId == therapistId && !t.DeleteFlag);

            if (therapist == null)
            {
                Console.WriteLine("Therapist not found!");
                return;
            }

            therapist.TherapistName = therapistName;
            therapist.Specialty = specialty;
            therapist.WorkingHours = workingHours;
            therapist.IsAvailable = isAvailable;

            db.Entry(therapist).State = EntityState.Modified;
            int result = db.SaveChanges();

            string message = result > 0 ? "Therapist updated successfully." : "Failed to update therapist.";
            Console.WriteLine(message);
        }

        public void Delete(int therapistId)
        {
            using AppDBContext db = new AppDBContext();
            var therapist = db.Therapists.FirstOrDefault(t => t.TherapistId == therapistId && !t.DeleteFlag);

            if (therapist == null)
            {
                Console.WriteLine("Therapist not found!");
                return;
            }

            therapist.DeleteFlag = true;
            int result = db.SaveChanges();

            string message = result > 0 ? "Therapist deleted successfully." : "Failed to delete therapist.";
            Console.WriteLine(message);
        }
    }
}
