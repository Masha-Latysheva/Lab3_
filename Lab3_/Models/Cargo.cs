using System.Collections.Generic;

namespace Lab3_.Models
{
    public class Cargo
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Weight { get; set; }

        public int Volume { get; set; }


        public List<Transportation> Transportations { get; set; }
    }
}