using System;
using System.Linq;
using Lab3_.Models;

namespace Lab3_.Data
{
    public static class DbInitializer
    {
        public static void Initialize(LogisticContext db)
        {
            db.Database.EnsureCreated();

            if (db.Organizations.Any()) return;

            const int organizationsNumber = 35;
            const int driverNumber = 35;
            const int carsNumber = 300;

            var randObj = new Random(1);

            var organizations = Enumerable.Range(1, organizationsNumber)
                .Select(orgId => new Organization
                {
                    Name = "TestOrganization_" + orgId
                })
                .ToList();
            db.Organizations.AddRange(organizations);
            db.SaveChanges();

            var drivers = Enumerable.Range(1, driverNumber)
                .Select(driverId => new Driver
                {
                    FirstName = "TestDriverFirstName" + driverId,
                    LastName = "TestDriverLastName" + driverId,
                    Passport = "TestDriverPassport" + driverId
                })
                .ToList();
            db.Drivers.AddRange(drivers);
            db.SaveChanges();

            var carMarks = new[] { "Mercedes", "Ford", "MAN" };
            var cars = Enumerable.Range(1, carsNumber)
                .Select(_ => new Car
                {
                    CarryingVolume = randObj.Next(1, 100),
                    CarryingWeight = randObj.Next(1, 100),
                    Mark = carMarks[randObj.Next(carMarks.Length)],
                    OrganizationId = randObj.Next(1, organizationsNumber + 1)
                })
                .ToList();

            db.Cars.AddRange(cars);
            db.SaveChanges();
        }
    }
}