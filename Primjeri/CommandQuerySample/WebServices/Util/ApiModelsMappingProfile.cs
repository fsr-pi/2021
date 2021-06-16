using AutoMapper;
using Contract.Commands;
using Contract.DTOs;

namespace WebServices.Util
{
  public class ApiModelsMappingProfile : Profile
  {
    public ApiModelsMappingProfile()
    {     
      CreateMap<Mjesto, AddMjesto>(); //model sa klijenta -> klasa za naredbu dodavanje države
      CreateMap<Mjesto, UpdateMjesto>();
    }
  }
}
