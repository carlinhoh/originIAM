using OriginIAM.Application.Interfaces;
using OriginIAM.Application.Services;
using OriginIAM.Domain.Interfaces;
using OriginIAM.Domain.Services;
using OriginIAM.Infrastructure.Repositories;
using OriginIAM.Infrastructure.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
});

#region Scopped Services
builder.Services.AddScoped<IEligibilityFileService, EligibilityFileService>();
builder.Services.AddScoped<IEligibilityService, EligibilityService>();
builder.Services.AddScoped<IEligibilityRepository, EligibilityRepository>();
builder.Services.AddScoped<IUserEligibilityProcessor, UserEligibilityProcessorService>();
builder.Services.AddScoped<IEligibilityReportService, EligibilityReportService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEmployerService, EmployerService>();
builder.Services.AddScoped<ISignupService, SignupService>();
builder.Services.AddScoped(typeof(ICsvParser<>), typeof(CsvParserService<>));
builder.Services.AddScoped<IFileDownloader, FileDownloaderService>();
builder.Services.AddScoped<IEmployerRepository, InMemoryEmployerRepository>();
builder.Services.AddScoped<IUserRepository, InMemoryUserRepository>();

#endregion

#region redis configuration
var redisConnectionString = configuration["Redis:ConnectionString"];

var redisConnection = ConnectionMultiplexer.Connect(redisConnectionString);

var redisDatabase = redisConnection.GetDatabase();
#endregion

#region Singleton Services
builder.Services.AddSingleton(redisDatabase);
builder.Services.AddSingleton<IEligibilityReportRepository>(provider => new EligibilityReportRepository(configuration.GetConnectionString("DefaultConnection")));
#endregion

var app = builder.Build();

#region Swagger
app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Origin API V1");
});

#endregion 

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();