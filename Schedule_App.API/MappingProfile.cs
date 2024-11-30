using AutoMapper;
using Schedule_App.API.DTOs.LessonStatus;
using Schedule_App.Core.DTOs.Classroom;
using Schedule_App.Core.DTOs.Group;
using Schedule_App.Core.DTOs.GroupTeacher;
using Schedule_App.Core.DTOs.Lesson;
using Schedule_App.Core.DTOs.Subject;
using Schedule_App.Core.DTOs.Teacher;
using Schedule_App.Core.Models;

namespace Schedule_App.API
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<LessonStatus, LessonStatusReadDTO>().ReverseMap();

            CreateMap<Group, GroupReadSummaryDTO>().ReverseMap();
            CreateMap<Group, GroupReadFullDTO>().ReverseMap();
            CreateMap<Group, GroupCreateDTO>().ReverseMap();

            CreateMap<Teacher, TeacherReadSummaryDTO>().ReverseMap();
            CreateMap<Teacher, TeacherReadFullDTO>().ReverseMap();
            CreateMap<Teacher, TeacherCreateDTO>().ReverseMap();

            CreateMap<GroupTeacher, GroupTeacherReadDTO>().ReverseMap();
            CreateMap<GroupTeacher, GroupTeacherCreateDTO>().ReverseMap();

            CreateMap<Subject, SubjectReadSummaryDTO>().ReverseMap();
            CreateMap<Subject, SubjectReadFullDTO>().ReverseMap();
            CreateMap<Subject, SubjectCreateDTO>().ReverseMap();

            CreateMap<Classroom, ClassroomReadSummaryDTO>().ReverseMap();
            CreateMap<Classroom, ClassroomReadFullDTO>().ReverseMap();
            CreateMap<Classroom, ClassroomCreateDTO>().ReverseMap();

            CreateMap<Lesson, LessonReadSummaryDTO>().ReverseMap();
            CreateMap<Lesson, LessonReadFullDTO>().ReverseMap();
            CreateMap<Lesson, LessonCreateDTO>().ReverseMap();
        }
    }
}
