using System;
using System.Collections.Generic;
using System.Linq;
using Lab3_.Data;
using Lab3_.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Lab3_.Services
{
    // Класс выборки 10 записей из таблиц 
    public class CarsService : ICarsService
    {
        private readonly LogisticContext _context;
        private readonly IMemoryCache _cache;

        private const int NumberRows = 20;

        public CarsService(LogisticContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public List<string> GetMarks()
        {
            var marks = _context.Cars
                .Select(car => car.Mark)
                .Distinct()
                .ToList();

            return marks;
        }

        public List<CarViewModel> SearchCars(string organization, string mark)
        {
            organization = organization.ToLower();
            mark = mark.ToLower();

            var result = _context.Cars
                .Include(x => x.Organization)
                .Where(x => x.Mark.ToLower().StartsWith(mark) && x.Organization.Name.StartsWith(organization))
                .Select(x => new CarViewModel
                {
                    Id = x.Id,
                    CarryingVolume = x.CarryingVolume,
                    CarryingWeight = x.CarryingWeight,
                    Mark = x.Mark,
                    Organization = x.Organization.Name
                })
                .ToList();

            return result;
        }

        public HomeViewModel GetHomeViewModel(string cacheKey)
        {
            if (_cache.TryGetValue(cacheKey, out HomeViewModel result))
            {
                return result;
            }

            var organizations = _context.Organizations.Take(NumberRows).ToList();
            var drivers = _context.Drivers.Take(NumberRows).ToList();

            var operations = _context.Cars
                .Include(t => t.Organization)
                .Select(t => new CarViewModel
                {
                    CarryingVolume = t.CarryingVolume,
                    CarryingWeight = t.CarryingWeight,
                    Id = t.Id,
                    Mark = t.Mark,
                    Organization = t.Organization.Name
                })
                .Take(NumberRows)
                .ToList();

            result = new HomeViewModel
            {
                Drivers = drivers,
                Organizations = organizations,
                Cars = operations
            };
            _cache.Set(cacheKey, result,
                new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(2 * 14 + 240)));

            return result;
        }
    }
}