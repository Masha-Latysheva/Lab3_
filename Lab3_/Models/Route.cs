using System.Collections.Generic;

namespace Lab3_.Models
{
    public class Route
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int RouteLength { get; set; }


        public List<Transportation> Transportations { get; set; }
    }
}