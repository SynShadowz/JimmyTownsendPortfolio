using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Server.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder =>
        builder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
});

builder.Services.AddDbContext<AppDBContext>(options => options.UseSqlite(
    builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Server", Version = "v1" });
});

builder.Services.AddControllers();

builder.Services.AddRouting(options => options.LowercaseUrls = true);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Server v1"));
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseCors("CorsPolicy");

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

//app.MapControllers();

app.Run();
