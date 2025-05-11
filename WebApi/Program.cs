using Application;
using Infrastructure;
using Microsoft.IdentityModel.Logging;
using WebApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddServices();
builder.Services.AddApplication();
builder.Services.AddIdentitySettings();
builder.Services.AddOpenApi();
builder.Services.RegisterSwagger();
builder.Services.AddJWTAuthentication(builder.Services.GetAppConfiguration(builder.Configuration));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SwaggerDemo API V1");
    });
}
IdentityModelEventSource.ShowPII = true;
app.SeedDatabase();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
