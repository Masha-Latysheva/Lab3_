using System.Collections.Generic;

namespace Lab3_.Models
{
    public class Organization
    {
        public int Id { get; set; }

        public string Name { get; set; }


        public List<Car> Cars { get; set; }
    }
}