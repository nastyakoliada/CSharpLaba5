using MusicCatalog.Data;
using MusicCatalog.WebService.Repositories;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Add context to SQlite MusicCatalog context
builder.Services.AddMusicCatalogContext();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Add musiccatalog repository for controller
builder.Services.AddScoped<IMusicCatalogRepository, MusicCatalogRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// We prefer https
app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

app.Run();
