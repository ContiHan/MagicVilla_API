using MagicVilla_CouponMinimalAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/api/coupons", () =>
{
    return CouponStore.couponList.Any() ? Results.Ok(CouponStore.couponList) : Results.NotFound("Empty coupon store");
});

app.MapGet("/api/coupon/{id:int}", (int id) =>
{
    if (CouponStore.couponList is null || !CouponStore.couponList.Any())
    {
        return Results.NotFound("Empty coupon store");
    }

    if (id == 0)
    {
        return Results.BadRequest("Id cannot be zero");
    }

    var coupon = CouponStore.couponList.FirstOrDefault(c => c.Id == id);
    if (coupon is null)
    {
        return Results.NotFound("Id not exists");
    }

    return Results.Ok(coupon);
});

app.UseHttpsRedirection();
app.Run();