using Microsoft.EntityFrameworkCore;
using Ticketing.Infrastructure;
using Ticketing.Application;
using Ticketing.Api;

var builder = WebApplication.CreateBuilder(args);

// DbContext（PostgreSQL）
builder.Services.AddDbContext<TicketingDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

// Application Services
builder.Services.AddScoped<ActivityService>();
builder.Services.AddScoped<TimeslotService>();
builder.Services.AddScoped<SeatService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<RefundService>();
builder.Services.AddScoped<NotificationService>();

var app = builder.Build();

// 之後所有 Minimal API 都集中在 Api/*Endpoints.cs
app.MapActivityEndpoints();
app.MapTimeslotEndpoints();
app.MapOrderEndpoints();


app.MapGet("/", () => "Ticketing API Running (.NET 9 Minimal API)");

app.Run();
