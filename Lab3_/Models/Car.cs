using System.Collections.Generic;

namespace Lab3_.Models
{
    public class Car
    {
        public int Id { get; set; }

        public string Mark { get; set; }

        public int OrganizationId { get; set; }

        public int CarryingWeight { get; set; }

        public int CarryingVolume { get; set; }


        public List<Transportation> Transportations { get; set; }

        public Organization Organization { get; set; }
    }
}