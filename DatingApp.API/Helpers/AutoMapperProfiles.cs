using System.Linq;
using AutoMapper;
using DatingApp.API.DTO;
using DatingApp.API.Models;

namespace DatingApp.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
           CreateMap<User, UserForListDTO>()
           .ForMember(dest => dest.PhotoUrl,
            opt => opt.MapFrom(src => 
            src.Photos.FirstOrDefault(p => p.IsMain).Url)
            )
            .ForMember(dest => dest.Age,
             option => option.ResolveUsing(d => 
             d.DateOfBirth.CalculateAge()));

           CreateMap<User, UserForDetailedDTO>()
           .ForMember(dest => dest.PhotoUrl,
            opt => opt.MapFrom(src => 
            src.Photos.FirstOrDefault(p =>p.IsMain).Url)
            )
            .ForMember(dest => dest.Age,
             option => option.ResolveUsing(d => 
             d.DateOfBirth.CalculateAge()));
           
           CreateMap<Photo, PhotosForDetailedDTO>();
           CreateMap<UserForUpdateDto, User>();
           CreateMap<Photo, PhotosForReturnDto>();
           CreateMap<PhotoForCreationDto, Photo>();
           CreateMap<UserRegisterDTO, User>();
           CreateMap<MessageForCreationDto, Message>().ReverseMap(); 
           CreateMap<Message, MessageToReturnDto>()
                .ForMember(M => M.SenderPhotoUrl, opt =>
                           opt.MapFrom(U => U.Sender.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(M => M.RecipientPhotoUrl, opt =>
                           opt.MapFrom(U => U.Recipient.Photos.FirstOrDefault(p => p.IsMain).Url));
           
        }
    }
}