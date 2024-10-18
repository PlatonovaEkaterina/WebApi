using Mapping;
using WebApi.Settings;
using WebApi.Services;
using WebApi.Workers;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

builder.Services.AddOutputCache();

builder.Services.AddLogging(builder => builder.AddConsole());

builder.Services.AddSingleton<Settings>();
builder.Services.AddScoped<IKeycloakUsersService,KeycloakUsersService>();
builder.Services.AddScoped<IKeycloakRolesService, KeycloakRolesService>();

builder.Services.AddHostedService<ScopedBackgroundService>();
builder.Services.AddScoped<IScopedProcessingService, UsersSynchronizationService>();

builder.Services.AddAutoMapper(typeof(Users));




builder.Services.Configure<Settings>(Settings.KeycloakSettings,
    builder.Configuration.GetSection("Settings:KeycloakSettings"));
builder.Services.Configure<Settings>(Settings.ShopSettings,
    builder.Configuration.GetSection("Settings:ShopSettings"));


var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}
app.UseHttpsRedirection();
app.UseOutputCache();

app.UseAuthorization();

app.MapControllers();

app.Run();




