using System.Collections.Generic;
using Lab3_.Models;

namespace Lab3_.ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<Driver> Drivers { get; set; }
        public IEnumerable<Organization> Organizations { get; set; }
        public IEnumerable<CarViewModel> Cars { get; set; }
    }
}