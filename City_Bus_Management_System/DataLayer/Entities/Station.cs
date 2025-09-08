namespace City_Bus_Management_System.DataLayer.Entities
{
    public class Station
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Area { get; set; }
        public required decimal Latitude { get; set; }
        public required decimal Longitude { get; set; }
    }
}
