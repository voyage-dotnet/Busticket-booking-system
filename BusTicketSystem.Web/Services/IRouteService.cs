using BusTicketSystem.Web.DTOs;

namespace BusTicketSystem.Web.Services;

public interface IRouteService
{
    Task<IEnumerable<RouteResponseDTO>> GetAllRoutesAsync();
    Task<RouteResponseDTO?> GetRouteByIdAsync(int id);
    Task<IEnumerable<RouteResponseDTO>> SearchRoutesAsync(string fromCity, string toCity);
    Task<RouteResponseDTO> CreateRouteAsync(CreateRouteRequestDTO request);
    Task<RouteResponseDTO> UpdateRouteAsync(int id, UpdateRouteRequestDTO request);
}
