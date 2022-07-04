using Microsoft.EntityFrameworkCore;
using PlatformService.AsyncDataService;
using PlatformService.Data;
using PlatformService.Mapping;
using PlatformService.SyncDataService;

var builder = WebApplication.CreateBuilder(args);

//logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
// services
builder.Services.AddGrpc();
builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();
builder.Services.AddHttpClient<ICommandHttpDataClient, CommandHttpDataClient>();
builder.Services.AddDbContext<AppDbContext>(opt =>
{
  opt.UseInMemoryDatabase("PlatformInMem");
});
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<PlatformMappingProfile>());
builder.Services.AddScoped<IPlatformRepository, PlatformRrepository>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseAppDbContext();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseRouting();

app.UseEndpoints(endpoints =>
{
  endpoints.MapControllers();
  endpoints.MapGrpcService<GrpcPlatformService>();

  endpoints.MapGet("protos/platform.proto", async context =>
  {
    await context.Response.WriteAsync(File.ReadAllText("Protos/platform.proto"));
  });
});

app.Run();
