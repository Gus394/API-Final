using AutoMapper;

namespace Univali.Api.Profiles;

public class AddressProfile : Profile
{
    public AddressProfile()
    {
        CreateMap<Entities.Address, Models.AddressDto>().ReverseMap(); // reverse map: ordem inversa tambem eh valida
        CreateMap<Models.AddressForUpdateDto, Entities.Address>();
    }
}