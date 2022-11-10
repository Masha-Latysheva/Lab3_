using System.Collections.Generic;
using Lab3_.ViewModels;

namespace Lab3_.Services
{
    public interface ICarsService
    {
        HomeViewModel GetHomeViewModel(string cacheKey);
        List<string> GetMarks();

        List<CarViewModel> SearchCars(string organization, string mark);
    }
}