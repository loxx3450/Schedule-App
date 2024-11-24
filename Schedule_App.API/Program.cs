using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Schedule_App.API;
using Schedule_App.API.Filters;
using Schedule_App.API.Services;
using Schedule_App.Core.Interfaces;
using Schedule_App.Storage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<DataContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Local")));

builder.Services.AddAutoMapper(typeof(Program));

// Repository
builder.Services.AddScoped<IRepository, Repository>();

// Services
builder.Services.AddScoped<ILessonStatusService, LessonStatusService>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<ITeacherService, TeacherService>();
builder.Services.AddScoped<ITeacherSubjectService, TeacherSubjectService>();
builder.Services.AddScoped<IGroupTeacherService, GroupTeacherService>();
builder.Services.AddScoped<ISubjectService, SubjectService>();

builder.Services.AddControllers(opt =>
{
    // IExceptionFilter
    opt.Filters.Add(new ExceptionFilter());

    // IActionFilter's
    opt.Filters.Add(new ValidationFilter());
    opt.Filters.Add(new StandardResponseFilter());
});

// Turns off automatic Validation Check
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
