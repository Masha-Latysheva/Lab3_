using System.Collections.Generic;

namespace Lab3_.Models
{
    public class Driver
    {
        public int Id { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string Passport { get; set; }


        public List<Transportation> Transportations { get; set; }
    }
}