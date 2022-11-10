namespace Lab3_.Models
{
    public class RoutePoint
    {
        public int Id { get; set; }

        public int RouteId { get; set; }

        public int PointId { get; set; }


        public Route Route { get; set; }

        public Point Point { get; set; }
    }
}