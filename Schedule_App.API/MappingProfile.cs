using AutoMapper;
using Schedule_App.API.DTOs.LessonStatus;
using Schedule_App.Core.DTOs.Group;
using Schedule_App.Core.Models;

namespace Schedule_App.API
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<LessonStatus, LessonStatusReadDTO>();

            CreateMap<Group, GroupReadDTO>();
            CreateMap<Group, GroupCreateDTO>();
        }
    }
}
