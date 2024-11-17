using AutoMapper;
using Schedule_App.API.DTOs.LessonStatus;
using Schedule_App.Core.DTOs.Group;
using Schedule_App.Core.DTOs.Teacher;
using Schedule_App.Core.Models;

namespace Schedule_App.API
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<LessonStatus, LessonStatusReadDTO>().ReverseMap();

            CreateMap<Group, GroupReadDTO>().ReverseMap();
            CreateMap<Group, GroupCreateDTO>().ReverseMap();

            CreateMap<Teacher, TeacherReadDTO>().ReverseMap();
            CreateMap<Teacher, TeacherCreateDTO>().ReverseMap();
            CreateMap<Teacher, TeacherUpdateDTO>().ReverseMap();
        }
    }
}
