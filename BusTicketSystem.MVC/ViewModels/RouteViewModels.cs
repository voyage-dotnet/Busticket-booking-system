namespace BusTicketSystem.MVC.ViewModels
{
    public class RouteViewModel
    {
        public int RouteId { get; set; }
        public string FromCity { get; set; } = string.Empty;
        public string ToCity { get; set; } = string.Empty;
        public double Distance { get; set; }
        public double EstimatedDuration { get; set; }
    }

    public class CreateRouteViewModel
    {
        public string FromCity { get; set; } = string.Empty;
        public string ToCity { get; set; } = string.Empty;
        public double Distance { get; set; }
        public double EstimatedDuration { get; set; }
    }

    public class EditRouteViewModel
    {
        public int RouteId { get; set; }
        public string FromCity { get; set; } = string.Empty;
        public string ToCity { get; set; } = string.Empty;
        public double Distance { get; set; }
        public double EstimatedDuration { get; set; }
    }
}
