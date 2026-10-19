using System;

namespace HNSDotNetTraining.ConsoleApp
{
    public interface ISpaServiceManagement
    {
        void Create(string serviceName, string? description, int durationMinutes, decimal price);
        void Read();
        void Edit(int serviceId);
        void Update(int serviceId, string serviceName, string? description, int durationMinutes, decimal price);
        void Delete(int serviceId);
    }

    public interface ICustomerManagement
    {
        void Create(string customerName, string phoneNumber, string? email);
        void Read();
        void Edit(int customerId);
        void Update(int customerId, string customerName, string phoneNumber, string? email);
        void ViewHistory(int customerId);
    }

    public interface ITherapistManagement
    {
        void Create(string therapistName, string specialty, string workingHours = "09:00 - 18:00");
        void Read();
        void Edit(int therapistId);
        void Update(int therapistId, string therapistName, string specialty, string workingHours, bool isAvailable);
        void Delete(int therapistId);
    }

    public interface IAppointmentManagement
    {
        void BookAppointment(int customerId, int therapistId, int serviceId, DateTime date, TimeSpan startTime);
        void ViewAppointments(string? statusFilter = null);
        void UpdateStatus(int appointmentId, string newStatus);
    }

    public interface IPaymentManagement
    {
        void ProcessPayment(int appointmentId, string paymentMethod);
        void ViewPaymentReport();
    }
}
