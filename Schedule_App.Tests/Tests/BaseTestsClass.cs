using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Schedule_App.API;
using Schedule_App.API.Services.Infrastructure;
using Schedule_App.Core.Interfaces;
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

        protected IDataHelper GetDataHelper(IRepository repository)
        {
            return new DataHelper(repository);
        }
    }
}
