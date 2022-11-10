using System.Collections.Generic;

namespace Lab3_.Models
{
    public class Rate
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int VolumeRate { get; set; }

        public int CarryingRate { get; set; }


        public List<Transportation> Transportations { get; set; }
    }
}