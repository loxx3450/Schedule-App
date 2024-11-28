using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Schedule_App.API.Filters;
using Schedule_App.API.Services;
using Schedule_App.Core.Interfaces;
using Schedule_App.Storage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Schedule_App.API.Extensions;
using Schedule_App.API.Services.Infrastructure;
using Schedule_App.Core.Interfaces.Services;

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
builder.Services.AddScoped<IClassroomService, ClassroomService>();
builder.Services.AddScoped<ILessonService, LessonService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Helpers
builder.Services.AddSingleton<TokenProvider>();
builder.Services.AddScoped<IDataHelper, DataHelper>();

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

// Cofiguring JWT Bearer Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:Token"]!)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGenWithAuth();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
