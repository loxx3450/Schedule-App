using AutoMapper;
using Schedule_App.API.DTOs.LessonStatus;
using Schedule_App.Core.Models;

namespace Schedule_App.API
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<LessonStatus, LessonStatusReadDTO>();
        }
    }
}
