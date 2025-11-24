using Microsoft.EntityFrameworkCore;
using Ticketing.Infrastructure;
using Ticketing.Application;
using Ticketing.Api;

var builder = WebApplication.CreateBuilder(args);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
builder.Services.AddScoped<ReportService>();

var app = builder.Build();

// 啟用 Swagger（只在 Development）
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Minimal API Endpoints（集中在 Api/*Endpoints.cs）
app.MapActivityEndpoints();
app.MapTimeslotEndpoints();
app.MapOrderEndpoints();
app.MapSeatEndpoints();
app.MapRefundEndpoints();
app.MapNotificationEndpoints();
app.MapReportEndpoints();

app.MapGet("/", () => "Ticketing API Running (.NET 9 Minimal API)");

app.Run();
