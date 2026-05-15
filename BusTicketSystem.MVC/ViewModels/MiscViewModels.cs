namespace BusTicketSystem.MVC.ViewModels
{
    public class PublicAgencyViewModel
    {
        public int AgencyId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string ContactPersonName { get; set; } = string.Empty;
        public int TotalBuses { get; set; }
        public double Rating { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
