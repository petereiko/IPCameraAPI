using FluentValidation;
using FluentValidation.AspNetCore;
using IPCameraAPI.Business;
using IPCameraAPI.Business.DTOs;
using IPCameraAPI.Business.Implementations;
using IPCameraAPI.Business.Interfaces;
using IPCameraAPI.Business.Modules.Alarm;
using IPCameraAPI.Business.Modules.Authentication;
using IPCameraAPI.Business.Modules.Record;
using IPCameraAPI.Business.Modules.Streaming;
using IPCameraAPI.Business.Validations;
using IPCameraAPI.Data;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var appSettingsSection = builder.Configuration.GetSection("AppSetting");
builder.Services.Configure<AppSetting>(appSettingsSection);
var appSettings = appSettingsSection.Get<AppSetting>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IAuthenticationService, AuthenticationService>();
builder.Services.AddTransient<IAlarmService, AlarmService>();
builder.Services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddTransient<ICookieStore, CookieStore>();
builder.Services.AddTransient<IStreamingService, StreamingService>();
builder.Services.AddTransient<IRecordService, RecordService>();


builder.Services.AddScoped<IValidator<ApplicationUserDto>, ApplicationUserDtoValidation>();
builder.Services.AddScoped<IValidator<EmailRequest>, EmailRequestValidation>();
builder.Services.AddTransient<INotificationService, NotificationService>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddFluentValidation();
builder.Services.AddControllers()
                .AddFluentValidation(v =>
                {
                    v.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
                });




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
