using HotelAPI.Data;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BookingContext>();
    DbInitializer.Initialize(context);
}