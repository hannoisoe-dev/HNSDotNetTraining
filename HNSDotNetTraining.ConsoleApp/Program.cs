using System;
using System.Linq;

namespace HNSDotNetTraining.ConsoleApp
{
    internal class Program
    {
        public enum DataAccessMode
        {
            AdoDotNet = 1,
            Dapper = 2,
            EFCore = 3
        }

        private static DataAccessMode _currentMode = DataAccessMode.EFCore;

        private static ISpaServiceManagement _spaService = new SpaServiceManagementEFCore();
        private static IAppointmentManagement _appointment = new AppointmentManagementEFCore();
        private static ICustomerManagement _customer = new CustomerManagementEFCore();
        private static ITherapistManagement _therapist = new TherapistManagementEFCore();
        private static IPaymentManagement _payment = new PaymentManagementEFCore();

        static void Main(string[] args)
        {
            SeedInitialDataIfEmpty();
            SwitchMode(DataAccessMode.EFCore);

            bool exit = false;

            while (!exit)
            {
                Console.WriteLine("\n==================================================================");
                Console.WriteLine($"      SPA MANAGEMENT SYSTEM (Current Mode: {_currentMode.ToString().ToUpper()})");
                Console.WriteLine("==================================================================");
                Console.WriteLine("1. Spa Service Management (Add, Edit, View, Delete)");
                Console.WriteLine("2. Appointment Management (Booking, Overlap Prevention, Status)");
                Console.WriteLine("3. Customer Management (Registration, History)");
                Console.WriteLine("4. Therapist Management (Specialties, Availability)");
                Console.WriteLine("5. Payment Management (Checkout, Receipts, Revenue)");
                Console.WriteLine("6. Switch Data Access Mode (ADO.NET / Dapper / EF Core)");
                Console.WriteLine("0. Exit");
                Console.WriteLine("==================================================================");
                Console.Write("Choose an option (0-6): ");

                string? choice = Console.ReadLine();

                switch (choice?.Trim())
                {
                    case "1":
                        SpaServiceMenu();
                        break;
                    case "2":
                        AppointmentMenu();
                        break;
                    case "3":
                        CustomerMenu();
                        break;
                    case "4":
                        TherapistMenu();
                        break;
                    case "5":
                        PaymentMenu();
                        break;
                    case "6":
                        SwitchModeMenu();
                        break;
                    case "0":
                        exit = true;
                        Console.WriteLine("Thank you for using Spa Management System. Goodbye!");
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please enter a number between 0 and 6.");
                        break;
                }
            }
        }

        private static void SwitchModeMenu()
        {
            Console.WriteLine("\n--- SWITCH DATA ACCESS ARCHITECTURE ---");
            Console.WriteLine("1. ADO.NET (Raw SQL & SqlDataReader/SqlDataAdapter/SqlTransaction)");
            Console.WriteLine("2. Dapper (High-performance Micro-ORM & SQL Queries)");
            Console.WriteLine("3. Entity Framework Core (Full ORM & LINQ)");
            Console.Write("Select Mode (1-3): ");

            string? choice = Console.ReadLine();
            switch (choice?.Trim())
            {
                case "1":
                    SwitchMode(DataAccessMode.AdoDotNet);
                    break;
                case "2":
                    SwitchMode(DataAccessMode.Dapper);
                    break;
                case "3":
                    SwitchMode(DataAccessMode.EFCore);
                    break;
                default:
                    Console.WriteLine("Invalid selection. Current mode retained.");
                    break;
            }
        }

        private static void SwitchMode(DataAccessMode mode)
        {
            _currentMode = mode;
            switch (mode)
            {
                case DataAccessMode.AdoDotNet:
                    _spaService = new SpaServiceManagementAdoDotNet();
                    _appointment = new AppointmentManagementAdoDotNet();
                    _customer = new CustomerManagementAdoDotNet();
                    _therapist = new TherapistManagementAdoDotNet();
                    _payment = new PaymentManagementAdoDotNet();
                    break;

                case DataAccessMode.Dapper:
                    _spaService = new SpaServiceManagementDapper();
                    _appointment = new AppointmentManagementDapper();
                    _customer = new CustomerManagementDapper();
                    _therapist = new TherapistManagementDapper();
                    _payment = new PaymentManagementDapper();
                    break;

                case DataAccessMode.EFCore:
                default:
                    _spaService = new SpaServiceManagementEFCore();
                    _appointment = new AppointmentManagementEFCore();
                    _customer = new CustomerManagementEFCore();
                    _therapist = new TherapistManagementEFCore();
                    _payment = new PaymentManagementEFCore();
                    break;
            }

            Console.WriteLine($"\n[Switched] Data Access Engine is now: {_currentMode} across all modules!");
        }

        #region 1. Spa Service Management Menu
        private static void SpaServiceMenu()
        {
            bool back = false;
            while (!back)
            {
                Console.WriteLine($"\n--- 1. SPA SERVICE MANAGEMENT ({_currentMode}) ---");
                Console.WriteLine("1. View All Services");
                Console.WriteLine("2. Add New Service");
                Console.WriteLine("3. View Service Details");
                Console.WriteLine("4. Update Service");
                Console.WriteLine("5. Delete Service");
                Console.WriteLine("0. Back to Main Menu");
                Console.Write("Enter choice: ");

                string? opt = Console.ReadLine();
                switch (opt?.Trim())
                {
                    case "1":
                        _spaService.Read();
                        break;

                    case "2":
                        Console.Write("Enter Service Name (e.g. Thai Massage, Facial Treatment): ");
                        string? name = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(name))
                        {
                            Console.WriteLine("Service name cannot be empty.");
                            break;
                        }

                        Console.Write("Enter Description (optional): ");
                        string? desc = Console.ReadLine();

                        Console.Write("Enter Duration in minutes (e.g. 60, 90): ");
                        if (!int.TryParse(Console.ReadLine(), out int duration) || duration <= 0)
                        {
                            Console.WriteLine("Invalid duration.");
                            break;
                        }

                        Console.Write("Enter Price ($): ");
                        if (!decimal.TryParse(Console.ReadLine(), out decimal price) || price < 0)
                        {
                            Console.WriteLine("Invalid price.");
                            break;
                        }

                        _spaService.Create(name.Trim(), string.IsNullOrWhiteSpace(desc) ? null : desc.Trim(), duration, price);
                        break;

                    case "3":
                        Console.Write("Enter Service ID: ");
                        if (int.TryParse(Console.ReadLine(), out int sId))
                        {
                            _spaService.Edit(sId);
                        }
                        else
                        {
                            Console.WriteLine("Invalid ID.");
                        }
                        break;

                    case "4":
                        Console.Write("Enter Service ID to Update: ");
                        if (!int.TryParse(Console.ReadLine(), out int updateId))
                        {
                            Console.WriteLine("Invalid Service ID.");
                            break;
                        }

                        Console.Write("Enter New Service Name: ");
                        string? newName = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(newName))
                        {
                            Console.WriteLine("Service name cannot be empty.");
                            break;
                        }

                        Console.Write("Enter New Description (optional): ");
                        string? newDesc = Console.ReadLine();

                        Console.Write("Enter New Duration in minutes: ");
                        if (!int.TryParse(Console.ReadLine(), out int newDuration) || newDuration <= 0)
                        {
                            Console.WriteLine("Invalid duration.");
                            break;
                        }

                        Console.Write("Enter New Price ($): ");
                        if (!decimal.TryParse(Console.ReadLine(), out decimal newPrice) || newPrice < 0)
                        {
                            Console.WriteLine("Invalid price.");
                            break;
                        }

                        _spaService.Update(updateId, newName.Trim(), string.IsNullOrWhiteSpace(newDesc) ? null : newDesc.Trim(), newDuration, newPrice);
                        break;

                    case "5":
                        Console.Write("Enter Service ID to Delete: ");
                        if (!int.TryParse(Console.ReadLine(), out int delId))
                        {
                            Console.WriteLine("Invalid Service ID.");
                            break;
                        }
                        Console.Write($"Are you sure you want to delete Service ID {delId} (y/n)? ");
                        if (Console.ReadLine()?.Trim().ToUpper() == "Y")
                        {
                            _spaService.Delete(delId);
                        }
                        else
                        {
                            Console.WriteLine("Delete cancelled.");
                        }
                        break;

                    case "0":
                        back = true;
                        break;

                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
        }
        #endregion

        #region 2. Appointment Management Menu
        private static void AppointmentMenu()
        {
            bool back = false;
            while (!back)
            {
                Console.WriteLine($"\n--- 2. APPOINTMENT MANAGEMENT ({_currentMode}) ---");
                Console.WriteLine("1. View All Appointments");
                Console.WriteLine("2. Book New Appointment (with Double-Booking Prevention)");
                Console.WriteLine("3. Update Appointment Status (Pending, Confirmed, Completed, Cancelled)");
                Console.WriteLine("4. Filter Appointments by Status");
                Console.WriteLine("0. Back to Main Menu");
                Console.Write("Enter choice: ");

                string? opt = Console.ReadLine();
                switch (opt?.Trim())
                {
                    case "1":
                        _appointment.ViewAppointments();
                        break;

                    case "2":
                        Console.Write("Enter Customer ID: ");
                        if (!int.TryParse(Console.ReadLine(), out int custId))
                        {
                            Console.WriteLine("Invalid Customer ID.");
                            break;
                        }

                        Console.Write("Enter Service ID: ");
                        if (!int.TryParse(Console.ReadLine(), out int srvId))
                        {
                            Console.WriteLine("Invalid Service ID.");
                            break;
                        }

                        Console.Write("Enter Therapist ID: ");
                        if (!int.TryParse(Console.ReadLine(), out int thId))
                        {
                            Console.WriteLine("Invalid Therapist ID.");
                            break;
                        }

                        Console.Write("Enter Appointment Date (yyyy-MM-dd, e.g. 2026-10-20 or press Enter for today): ");
                        string? dateInput = Console.ReadLine();
                        DateTime date = string.IsNullOrWhiteSpace(dateInput) ? DateTime.Today : (DateTime.TryParse(dateInput, out DateTime d) ? d : DateTime.Today);

                        

                        Console.Write("Enter Start Time (HH:mm, e.g. 10:00, 14:30): ");
                        string? timeInput = Console.ReadLine();
                        if (!TimeSpan.TryParse(timeInput, out TimeSpan startTime))
                        {
                            Console.WriteLine("Invalid time format. Please use HH:mm (e.g. 14:00).");
                            break;
                        }

                        _appointment.BookAppointment(custId, thId, srvId, date, startTime);
                        break;

                    case "3":
                        Console.Write("Enter Appointment ID: ");
                        if (!int.TryParse(Console.ReadLine(), out int apptId))
                        {
                            Console.WriteLine("Invalid Appointment ID.");
                            break;
                        }

                        Console.Write("Enter New Status (Pending / Confirmed / Completed / Cancelled): ");
                        string? status = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(status))
                        {
                            Console.WriteLine("Status cannot be empty.");
                            break;
                        }

                        _appointment.UpdateStatus(apptId, status);
                        break;

                    case "4":
                        Console.Write("Enter Status to filter by (Pending / Confirmed / Completed / Cancelled): ");
                        string? filter = Console.ReadLine();
                        _appointment.ViewAppointments(filter);
                        break;

                    case "0":
                        back = true;
                        break;

                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
        }
        #endregion

        #region 3. Customer Management Menu
        private static void CustomerMenu()
        {
            bool back = false;
            while (!back)
            {
                Console.WriteLine($"\n--- 3. CUSTOMER MANAGEMENT ({_currentMode}) ---");
                Console.WriteLine("1. View All Customers");
                Console.WriteLine("2. Register New Customer");
                Console.WriteLine("3. View Customer Details");
                Console.WriteLine("4. Edit Customer Information");
                Console.WriteLine("5. View Customer Appointment History");
                Console.WriteLine("0. Back to Main Menu");
                Console.Write("Enter choice: ");

                string? opt = Console.ReadLine();
                switch (opt?.Trim())
                {
                    case "1":
                        _customer.Read();
                        break;

                    case "2":
                        Console.Write("Enter Customer Name: ");
                        string? name = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(name))
                        {
                            Console.WriteLine("Name cannot be empty.");
                            break;
                        }

                        Console.Write("Enter Phone Number: ");
                        string? phone = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(phone))
                        {
                            Console.WriteLine("Phone number cannot be empty.");
                            break;
                        }

                        Console.Write("Enter Email (optional): ");
                        string? email = Console.ReadLine();

                        _customer.Create(name.Trim(), phone.Trim(), string.IsNullOrWhiteSpace(email) ? null : email.Trim());
                        break;

                    case "3":
                        Console.Write("Enter Customer ID: ");
                        if (int.TryParse(Console.ReadLine(), out int cId))
                        {
                            _customer.Edit(cId);
                        }
                        else
                        {
                            Console.WriteLine("Invalid ID.");
                        }
                        break;

                    case "4":
                        Console.Write("Enter Customer ID to Edit: ");
                        if (!int.TryParse(Console.ReadLine(), out int editId))
                        {
                            Console.WriteLine("Invalid Customer ID.");
                            break;
                        }

                        Console.Write("Enter Updated Customer Name: ");
                        string? newName = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(newName))
                        {
                            Console.WriteLine("Name cannot be empty.");
                            break;
                        }

                        Console.Write("Enter Updated Phone Number: ");
                        string? newPhone = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(newPhone))
                        {
                            Console.WriteLine("Phone number cannot be empty.");
                            break;
                        }

                        Console.Write("Enter Updated Email (optional): ");
                        string? newEmail = Console.ReadLine();

                        _customer.Update(editId, newName.Trim(), newPhone.Trim(), string.IsNullOrWhiteSpace(newEmail) ? null : newEmail.Trim());
                        break;

                    case "5":
                        Console.Write("Enter Customer ID to View History: ");
                        if (int.TryParse(Console.ReadLine(), out int histId))
                        {
                            _customer.ViewHistory(histId);
                        }
                        else
                        {
                            Console.WriteLine("Invalid Customer ID.");
                        }
                        break;

                    case "0":
                        back = true;
                        break;

                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
        }
        #endregion

        #region 4. Therapist Management Menu
        private static void TherapistMenu()
        {
            bool back = false;
            while (!back)
            {
                Console.WriteLine($"\n--- 4. THERAPIST MANAGEMENT ({_currentMode}) ---");
                Console.WriteLine("1. View All Therapists");
                Console.WriteLine("2. Register New Therapist");
                Console.WriteLine("3. View Therapist Details");
                Console.WriteLine("4. Edit Therapist Information & Working Hours");
                Console.WriteLine("5. Delete Therapist");
                Console.WriteLine("0. Back to Main Menu");
                Console.Write("Enter choice: ");

                string? opt = Console.ReadLine();
                switch (opt?.Trim())
                {
                    case "1":
                        _therapist.Read();
                        break;

                    case "2":
                        Console.Write("Enter Therapist Name: ");
                        string? name = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(name))
                        {
                            Console.WriteLine("Name cannot be empty.");
                            break;
                        }

                        Console.Write("Enter Specialty (e.g. Traditional Thai Massage, Aromatherapy, Reflexology): ");
                        string? specialty = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(specialty))
                        {
                            Console.WriteLine("Specialty cannot be empty.");
                            break;
                        }

                        Console.Write("Enter Working Hours (e.g. 09:00 - 18:00, or press Enter for default): ");
                        string? hours = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(hours)) hours = "09:00 - 18:00";

                        _therapist.Create(name.Trim(), specialty.Trim(), hours.Trim());
                        break;

                    case "3":
                        Console.Write("Enter Therapist ID: ");
                        if (int.TryParse(Console.ReadLine(), out int thId))
                        {
                            _therapist.Edit(thId);
                        }
                        else
                        {
                            Console.WriteLine("Invalid ID.");
                        }
                        break;

                    case "4":
                        Console.Write("Enter Therapist ID to Edit: ");
                        if (!int.TryParse(Console.ReadLine(), out int editThId))
                        {
                            Console.WriteLine("Invalid Therapist ID.");
                            break;
                        }

                        Console.Write("Enter Updated Therapist Name: ");
                        string? newName = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(newName))
                        {
                            Console.WriteLine("Name cannot be empty.");
                            break;
                        }

                        Console.Write("Enter Updated Specialty: ");
                        string? newSpec = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(newSpec))
                        {
                            Console.WriteLine("Specialty cannot be empty.");
                            break;
                        }

                        Console.Write("Enter Updated Working Hours: ");
                        string? newHours = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(newHours)) newHours = "09:00 - 18:00";

                        Console.Write("Is Therapist Available (y/n)? ");
                        bool isAvail = Console.ReadLine()?.Trim().ToUpper() == "Y";

                        _therapist.Update(editThId, newName.Trim(), newSpec.Trim(), newHours.Trim(), isAvail);
                        break;

                    case "5":
                        Console.Write("Enter Therapist ID to Delete: ");
                        if (!int.TryParse(Console.ReadLine(), out int delThId))
                        {
                            Console.WriteLine("Invalid Therapist ID.");
                            break;
                        }
                        Console.Write($"Are you sure you want to delete Therapist ID {delThId} (y/n)? ");
                        if (Console.ReadLine()?.Trim().ToUpper() == "Y")
                        {
                            _therapist.Delete(delThId);
                        }
                        else
                        {
                            Console.WriteLine("Delete cancelled.");
                        }
                        break;

                    case "0":
                        back = true;
                        break;

                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
        }
        #endregion

        #region 5. Payment Management Menu
        private static void PaymentMenu()
        {
            bool back = false;
            while (!back)
            {
                Console.WriteLine($"\n--- 5. PAYMENT MANAGEMENT ({_currentMode}) ---");
                Console.WriteLine("1. Process Payment & Generate Receipt");
                Console.WriteLine("2. View Payment Transactions & Total Revenue");
                Console.WriteLine("3. View All Appointments (Track Paid vs Unpaid)");
                Console.WriteLine("0. Back to Main Menu");
                Console.Write("Enter choice: ");

                string? opt = Console.ReadLine();
                switch (opt?.Trim())
                {
                    case "1":
                        Console.Write("Enter Appointment ID to Checkout: ");
                        if (!int.TryParse(Console.ReadLine(), out int apptId))
                        {
                            Console.WriteLine("Invalid Appointment ID.");
                            break;
                        }

                        Console.Write("Enter Payment Method (Cash / Card / KBZPay / WavePay / CBPay): ");
                        string? method = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(method))
                        {
                            Console.WriteLine("Payment method cannot be empty.");
                            break;
                        }

                        _payment.ProcessPayment(apptId, method.Trim());
                        break;

                    case "2":
                        _payment.ViewPaymentReport();
                        break;

                    case "3":
                        _appointment.ViewAppointments();
                        break;

                    case "0":
                        back = true;
                        break;

                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
        }
        #endregion

        #region Sample Seed Helper
        private static void SeedInitialDataIfEmpty()
        {
            try
            {
                using AppDBContext db = new AppDBContext();
                if (!db.SpaServices.Any())
                {
                    db.SpaServices.AddRange(
                        new Models.SpaServiceDataModel { ServiceName = "Thai Massage", Description = "Traditional full-body acupressure and stretching", DurationMinutes = 60, Price = 45.00m },
                        new Models.SpaServiceDataModel { ServiceName = "Facial Treatment", Description = "Deep cleansing, exfoliating, and hydrating mask", DurationMinutes = 45, Price = 38.00m },
                        new Models.SpaServiceDataModel { ServiceName = "Body Scrub", Description = "Exfoliating organic herbal scrub and moisturization", DurationMinutes = 60, Price = 50.00m },
                        new Models.SpaServiceDataModel { ServiceName = "Aromatherapy Massage", Description = "Gentle essential oil relaxation massage", DurationMinutes = 90, Price = 65.00m }
                    );
                }

                if (!db.Therapists.Any())
                {
                    db.Therapists.AddRange(
                        new Models.TherapistDataModel { TherapistName = "May Thu", Specialty = "Thai Traditional Massage & Reflexology", WorkingHours = "09:00 - 18:00", IsAvailable = true },
                        new Models.TherapistDataModel { TherapistName = "Su Su", Specialty = "Facial Treatment & Skin Care", WorkingHours = "10:00 - 19:00", IsAvailable = true },
                        new Models.TherapistDataModel { TherapistName = "Aung Ko", Specialty = "Deep Tissue & Body Scrub", WorkingHours = "09:00 - 18:00", IsAvailable = true }
                    );
                }

                if (!db.Customers.Any())
                {
                    db.Customers.AddRange(
                        new Models.CustomerDataModel { CustomerName = "Daw Hla", PhoneNumber = "09450011223", Email = "dawhla@gmail.com" },
                        new Models.CustomerDataModel { CustomerName = "U Kyaw Min", PhoneNumber = "09250088991", Email = "kyawmin@gmail.com" }
                    );
                }

                db.SaveChanges();
            }
            catch
            {
                // Ignore seed error if already populated or db offline
            }
        }
        #endregion
    }
}
