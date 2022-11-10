using System.Collections.Generic;

namespace Lab3_.Models
{
    public class Point
    {
        public int Id { get; set; }

        public string Name { get; set; }


        public List<Route> StartRoutes { get; set; }

        public List<Route> EndRoutes { get; set; }
    }
}