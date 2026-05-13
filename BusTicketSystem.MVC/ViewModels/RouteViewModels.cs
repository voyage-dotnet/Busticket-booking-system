using BusTicketSystem.Web.DTOs;

namespace BusTicketSystem.MVC.ViewModels
{

    public class RouteIndexViewModel
    {
        public string FromSearch { get; set; }
        public string ToSearch { get; set; }
        public IEnumerable<RouteResponseDTO> Routes { get; set; } = new List<RouteResponseDTO>();
    }

    public class RouteDetailsViewModel
    {
        public RouteResponseDTO Route { get; set; }
    }

    public class CreateRouteViewModel
    {
        public CreateRouteRequestDTO Request { get; set; } = new();
    }

    public class EditRouteViewModel
    {
        public int RouteId { get; set; }
        public string FromCity { get; set; }
        public string ToCity { get; set; }
        public UpdateRouteRequestDTO Request { get; set; } = new();
    }
}
