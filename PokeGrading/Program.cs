using PokeGrading.Utilities;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);


// Controllers
builder.Services.AddControllers();


// Database
builder.Services.AddScoped<SQL_connection>();
builder.Services.AddScoped<DatabaseService>();


// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowReactFrontend",
        policy =>
        {
            policy.WithOrigins(
                    "http://localhost:3000",
                    "https://localhost:3000"
                )
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});


// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();


// Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// HTTPS
app.UseHttpsRedirection();


// CORS
app.UseCors("AllowReactFrontend");


// Images Folder
var imagesPath =
    Path.Combine(
        builder.Environment.ContentRootPath,
        "Images"
    );

if (!Directory.Exists(imagesPath))
{
    Directory.CreateDirectory(imagesPath);
}


// Static Files
app.UseStaticFiles();

app.UseStaticFiles(
    new StaticFileOptions
    {
        FileProvider =
            new PhysicalFileProvider(
                imagesPath
            ),

        RequestPath = "/images"
    });


// Auth
app.UseAuthorization();


// Controllers
app.MapControllers();

app.Run();