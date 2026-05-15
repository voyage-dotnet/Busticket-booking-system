using AutoMapper;
using BusTicketSystem.Web.DTOs;
using BusTicketSystem.Web.Exceptions;
using BusTicketSystem.Web.Models;
using BusTicketSystem.Web.Repositories;

namespace BusTicketSystem.Web.Services
{
    public class AgencyService : IAgencyService
    {
        private readonly IAgencyRepository _agencyRepository;
        private readonly IMapper _mapper;

        public AgencyService(IAgencyRepository agencyRepository, IMapper mapper)
        {
            _agencyRepository = agencyRepository;
            _mapper = mapper;
        }

        public async Task<List<AgencyResponseDTO>> GetAllAgenciesAsync()
        {
            var agencies = await _agencyRepository.GetAllAgenciesAsync();
            return _mapper.Map<List<AgencyResponseDTO>>(agencies);
        }

        public async Task<AgencyResponseDTO> GetAgencyByIdAsync(int agencyId)
        {
            var agency = await _agencyRepository.GetAgencyByIdAsync(agencyId);
            if (agency == null) throw new NotFoundException("Agency not found.");
            return _mapper.Map<AgencyResponseDTO>(agency);
        }

        public async Task<AgencyResponseDTO> GetMyAgencyAsync(int agencyId)
        {
            var agency = await _agencyRepository.GetAgencyByIdAsync(agencyId);
            if (agency == null) throw new NotFoundException("Agency profile not found.");
            return _mapper.Map<AgencyResponseDTO>(agency);
        }

        public async Task<AgencyResponseDTO> UpdateMyAgencyAsync(int agencyId, UpdateAgencyRequestDTO dto)
        {
            var agency = await _agencyRepository.GetAgencyByIdAsync(agencyId);
            if (agency == null) throw new NotFoundException("Agency profile not found.");

            if (dto.Email != null)
            {
                var emailExists = await _agencyRepository.AgencyEmailExistsAsync(dto.Email, agencyId);
                if (emailExists) throw new BadRequestException("Email is already used by another agency.");
                agency.Email = dto.Email.Trim();
            }
            if (dto.Name != null) agency.Name = dto.Name.Trim();
            if (dto.ContactPersonName != null) agency.ContactPersonName = dto.ContactPersonName.Trim();
            if (dto.Phone != null) agency.Phone = dto.Phone.Trim();

            await _agencyRepository.UpdateAgencyAsync(agency);
            return _mapper.Map<AgencyResponseDTO>(agency);
        }

        public async Task<List<OfficeResponseDTO>> GetMyOfficesAsync(int agencyId)
        {
            var offices = await _agencyRepository.GetOfficesByAgencyIdAsync(agencyId);
            return _mapper.Map<List<OfficeResponseDTO>>(offices);
        }

        public async Task<OfficeResponseDTO> GetOfficeByIdAsync(int officeId, int agencyId)
        {
            var office = await _agencyRepository.GetOfficeByIdAsync(officeId);
            if (office == null) throw new NotFoundException("Office not found.");
            if (office.AgencyId != agencyId) throw new ForbiddenException("You cannot access another agency's office.");
            return _mapper.Map<OfficeResponseDTO>(office);
        }

        public async Task<OfficeResponseDTO> CreateOfficeAsync(int agencyId, OfficeCreateDTO dto)
        {
            var addressExists = await _agencyRepository.AddressExistsAsync(dto.OfficeAddressId);
            if (!addressExists) throw new NotFoundException("Office address not found.");

            var office = new AgencyOffice
            {
                AgencyId = agencyId,
                OfficeMail = dto.OfficeMail.Trim(),
                OfficeContactPersonName = dto.OfficeContactPersonName.Trim(),
                OfficeContactNumber = dto.OfficeContactNumber.Trim(),
                OfficeAddressId = dto.OfficeAddressId
            };

            var createdOffice = await _agencyRepository.AddOfficeAsync(office);
            var officeWithAddress = await _agencyRepository.GetOfficeByIdAsync(createdOffice.OfficeId);
            return _mapper.Map<OfficeResponseDTO>(officeWithAddress);
        }

        public async Task<OfficeResponseDTO> UpdateOfficeAsync(int officeId, int agencyId, OfficeUpdateDTO dto)
        {
            var office = await _agencyRepository.GetOfficeByIdAsync(officeId);
            if (office == null) throw new NotFoundException("Office not found.");
            if (office.AgencyId != agencyId) throw new ForbiddenException("You cannot update another agency's office.");

            if (dto.OfficeAddressId != null)
            {
                var addressExists = await _agencyRepository.AddressExistsAsync(dto.OfficeAddressId.Value);
                if (!addressExists) throw new NotFoundException("Office address not found.");
                office.OfficeAddressId = dto.OfficeAddressId.Value;
            }
            if (dto.OfficeMail != null) office.OfficeMail = dto.OfficeMail.Trim();
            if (dto.OfficeContactPersonName != null) office.OfficeContactPersonName = dto.OfficeContactPersonName.Trim();
            if (dto.OfficeContactNumber != null) office.OfficeContactNumber = dto.OfficeContactNumber.Trim();

            await _agencyRepository.UpdateOfficeAsync(office);
            var updatedOffice = await _agencyRepository.GetOfficeByIdAsync(officeId);
            return _mapper.Map<OfficeResponseDTO>(updatedOffice);
        }

        public async Task<List<BusResponseDTO>> GetBusesByOfficeIdAsync(int officeId, int agencyId)
        {
            var belongsToAgency = await _agencyRepository.OfficeBelongsToAgencyAsync(officeId, agencyId);
            if (!belongsToAgency) throw new ForbiddenException("You cannot access buses of another agency's office.");
            var buses = await _agencyRepository.GetBusesByOfficeIdAsync(officeId);
            return _mapper.Map<List<BusResponseDTO>>(buses);
        }

        public async Task<BusResponseDTO> CreateBusAsync(int agencyId, CreateBusRequestDTO dto)
        {
            var belongsToAgency = await _agencyRepository.OfficeBelongsToAgencyAsync(dto.OfficeId, agencyId);
            if (!belongsToAgency) throw new ForbiddenException("You cannot add bus to another agency's office.");

            var registrationExists = await _agencyRepository.BusRegistrationExistsAsync(dto.RegistrationNumber);
            if (registrationExists) throw new BadRequestException("Bus registration number already exists.");

            var bus = new Bus
            {
                OfficeId = dto.OfficeId,
                RegistrationNumber = dto.RegistrationNumber.Trim(),
                Capacity = dto.Capacity,
                Type = dto.Type.Trim()
            };

            var createdBus = await _agencyRepository.AddBusAsync(bus);
            return _mapper.Map<BusResponseDTO>(createdBus);
        }

        public async Task<BusResponseDTO> UpdateBusAsync(int busId, int agencyId, UpdateBusRequestDTO dto)
        {
            var bus = await _agencyRepository.GetBusByIdAsync(busId);
            if (bus == null) throw new NotFoundException("Bus not found.");

            var belongsToAgency = await _agencyRepository.BusBelongsToAgencyAsync(busId, agencyId);
            if (!belongsToAgency) throw new ForbiddenException("You cannot update another agency's bus.");

            if (dto.RegistrationNumber != null)
            {
                var registrationExists = await _agencyRepository.BusRegistrationExistsAsync(dto.RegistrationNumber, busId);
                if (registrationExists) throw new BadRequestException("Bus registration number already exists.");
                bus.RegistrationNumber = dto.RegistrationNumber.Trim();
            }
            if (dto.Capacity != null) bus.Capacity = dto.Capacity.Value;
            if (dto.Type != null) bus.Type = dto.Type.Trim();

            await _agencyRepository.UpdateBusAsync(bus);
            return _mapper.Map<BusResponseDTO>(bus);
        }

        public async Task<List<DriverResponseDTO>> GetDriversByOfficeIdAsync(int officeId, int agencyId)
        {
            var belongsToAgency = await _agencyRepository.OfficeBelongsToAgencyAsync(officeId, agencyId);
            if (!belongsToAgency) throw new ForbiddenException("You cannot access drivers of another agency's office.");
            var drivers = await _agencyRepository.GetDriversByOfficeIdAsync(officeId);
            return _mapper.Map<List<DriverResponseDTO>>(drivers);
        }

        public async Task<DriverResponseDTO> CreateDriverAsync(int agencyId, DriverCreateDTO dto)
        {
            var belongsToAgency = await _agencyRepository.OfficeBelongsToAgencyAsync(dto.OfficeId, agencyId);
            if (!belongsToAgency) throw new ForbiddenException("You cannot add driver to another agency's office.");

            var addressExists = await _agencyRepository.AddressExistsAsync(dto.AddressId);
            if (!addressExists) throw new NotFoundException("Driver address not found.");

            var licenseExists = await _agencyRepository.DriverLicenseExistsAsync(dto.LicenseNumber);
            if (licenseExists) throw new BadRequestException("Driver license number already exists.");

            var driver = new Driver
            {
                OfficeId = dto.OfficeId,
                AddressId = dto.AddressId,
                LicenseNumber = dto.LicenseNumber.Trim(),
                Name = dto.Name.Trim(),
                Phone = dto.Phone.Trim()
            };

            var createdDriver = await _agencyRepository.AddDriverAsync(driver);
            return _mapper.Map<DriverResponseDTO>(createdDriver);
        }

        public async Task<DriverResponseDTO> UpdateDriverAsync(int driverId, int agencyId, DriverUpdateDTO dto)
        {
            var driver = await _agencyRepository.GetDriverByIdAsync(driverId);
            if (driver == null) throw new NotFoundException("Driver not found.");

            var belongsToAgency = await _agencyRepository.DriverBelongsToAgencyAsync(driverId, agencyId);
            if (!belongsToAgency) throw new ForbiddenException("You cannot update another agency's driver.");

            if (dto.OfficeId != null)
            {
                var officeBelongs = await _agencyRepository.OfficeBelongsToAgencyAsync(dto.OfficeId.Value, agencyId);
                if (!officeBelongs) throw new ForbiddenException("You cannot move driver to another agency's office.");
                driver.OfficeId = dto.OfficeId.Value;
            }
            if (dto.AddressId != null)
            {
                var addressExists = await _agencyRepository.AddressExistsAsync(dto.AddressId.Value);
                if (!addressExists) throw new NotFoundException("Driver address not found.");
                driver.AddressId = dto.AddressId.Value;
            }
            if (dto.LicenseNumber != null)
            {
                var licenseExists = await _agencyRepository.DriverLicenseExistsAsync(dto.LicenseNumber, driverId);
                if (licenseExists) throw new BadRequestException("Driver license number already exists.");
                driver.LicenseNumber = dto.LicenseNumber.Trim();
            }
            if (dto.Name != null) driver.Name = dto.Name.Trim();
            if (dto.Phone != null) driver.Phone = dto.Phone.Trim();

            await _agencyRepository.UpdateDriverAsync(driver);
            return _mapper.Map<DriverResponseDTO>(driver);
        }

        public async Task<List<BusResponseDTO>> GetMyBusesAsync(int agencyId)
        {
            var buses = await _agencyRepository.GetBusesByAgencyIdAsync(agencyId);
            return _mapper.Map<List<BusResponseDTO>>(buses);
        }

        public async Task<List<DriverResponseDTO>> GetMyDriversAsync(int agencyId)
        {
            var drivers = await _agencyRepository.GetDriversByAgencyIdAsync(agencyId);
            return _mapper.Map<List<DriverResponseDTO>>(drivers);
        }
    }
}
