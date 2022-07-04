using CommandService.AsyncDataService;
using CommandService.Data;
using CommandService.EventProcessing;
using CommandService.Mapping;
using CommandService.SyncDataService;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
// logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
// Add services to the container.
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<CommandMappingProfile>());
builder.Services.AddControllers();
builder.Services.AddHostedService<MessageBusSubscriber>();
builder.Services.AddSingleton<IEventProcessor, EventProcessor>();
builder.Services.AddScoped<IPlatformDataClient, PlatformDataClient>();
builder.Services.AddDbContext<AppDbContext>(opt =>
{
  opt.UseInMemoryDatabase("CommandsInMem");
});
builder.Services.AddTransient<ICommandRepository, CommandRepository>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
await app.PopulatePlatformsAsync();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
