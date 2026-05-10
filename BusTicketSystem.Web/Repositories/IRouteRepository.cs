
namespace BusTicketSystem.Web.Repositories;

public interface IRouteRepository
{
    Task<IEnumerable<Models.Route>> GetAllAsync();
    Task<Models.Route?> GetByIdAsync(int id);
    Task<IEnumerable<Models.Route>> SearchRoutesAsync(string fromCity, string toCity);
    Task<Models.Route> AddRouteAsync(Models.Route route);
    Task<Models.Route> UpdateRouteAsync(Models.Route route);
    Task<bool> ExistsAsync(int id);
}
