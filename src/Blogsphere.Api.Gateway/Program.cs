using Blogsphere.Api.Gateway;

var builder = WebApplication.CreateBuilder(args);

var logger = Logging.CreateLogger(builder.Configuration);

builder.Services.AddSingleton(logger);
builder.Host.UseSerilog(logger);

builder.Services.AddApplicationServices(builder.Configuration, builder.Environment.IsDevelopment());
builder.Services.AddDatabaseServices(builder.Configuration);
builder.Services.AddBusinessLogicServices();

#if DEBUG
builder.WebHost.UseUrls("http://localhost:8000");
#endif

var app = builder.Build();

await app.UseApplicationPipelineAsync();

try{
    await app.RunAsync();
}
finally{
    Log.CloseAndFlush();
}

