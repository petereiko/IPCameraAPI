using IPCameraAPI.Business;
using IPCameraAPI.Business.Implementations;
using IPCameraAPI.Business.Interfaces;
using IPCameraAPI.Business.Modules.Authentication;
using IPCameraAPI.Business.Modules.Execute;
using IPCameraAPI.Business.Modules.Record;
using IPCameraAPI.Business.Modules.Streaming;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var appSettingsSection = builder.Configuration.GetSection("AppSetting");
builder.Services.Configure<AppSetting>(appSettingsSection);
var appSettings = appSettingsSection.Get<AppSetting>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<IAuthenticationService, AuthenticationService>();
builder.Services.AddTransient<IStreamingService, StreamingService>();
builder.Services.AddTransient<IRecordService, RecordService>();
builder.Services.AddTransient<ICookieStore, CookieStore>();
builder.Services.AddTransient<IExecuteService, ExecuteService>();
builder.Services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddTransient<INotificationService, NotificationService>();

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
