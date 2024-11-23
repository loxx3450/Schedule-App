using AutoMapper;
using Schedule_App.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_App.Tests.Tests
{
    public abstract class BaseTestsClass
    {
        protected readonly IMapper _mapper;

        public BaseTestsClass()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            _mapper = config.CreateMapper();
        }
    }
}
