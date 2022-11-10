using System.Collections.Generic;
using Lab3_.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lab3_.ViewModels
{
    public class CarsViewModel
    {
        public IEnumerable<Car> Cars { get; set; }
        public CarViewModel CarViewModel { get; set; }
        public PageViewModel PageViewModel { get; set; }
        public SelectList ListYears { get; set; }
    }
}