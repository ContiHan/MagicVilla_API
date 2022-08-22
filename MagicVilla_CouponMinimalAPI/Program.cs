using MagicVilla_CouponMinimalAPI.Data;
using MagicVilla_CouponMinimalAPI.Models;
using Microsoft.AspNetCore.Mvc;

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
})
    .WithName("GetCoupon");

app.MapPost("/api/coupon", ([FromBody] Coupon coupon) =>
{
    if (coupon.Id != 0 || string.IsNullOrEmpty(coupon.Name))
    {
        return Results.BadRequest("Invalid Id or Coupon Name");
    }

    if (CouponStore.couponList is null)
    {
        return Results.NotFound("Coupon store not found");
    }

    if (CouponStore.couponList.FirstOrDefault(c => c.Name.ToLower() == coupon.Name.ToLower()) is not null)
    {
        return Results.BadRequest("Coupon Name already exists");
    }

    if (CouponStore.couponList.Any())
    {
        coupon.Id = CouponStore.couponList.OrderByDescending(c => c.Id).FirstOrDefault().Id + 1;
    }
    else
    {
        coupon.Id = 1;
    }
    coupon.Created = coupon.LastUpdate = DateTime.Now;

    CouponStore.couponList.Add(coupon);

    return Results.CreatedAtRoute("GetCoupon", new { coupon.Id }, coupon);
});

app.MapPut("/api/coupon/{id:int}", (int id, [FromBody] Coupon coupon) =>
{
    if (id != coupon.Id)
    {
        return Results.BadRequest("Ids do not match");
    }

    var couponFromStore = CouponStore.couponList.FirstOrDefault(c => c.Id == id);
    if (couponFromStore is null)
    {
        return Results.NotFound("Coupon not exists");
    }

    couponFromStore.Name = coupon.Name;
    couponFromStore.Percent = coupon.Percent;
    couponFromStore.IsActive = coupon.IsActive;
    couponFromStore.LastUpdate = DateTime.Now;

    return Results.NoContent();
});

app.MapDelete("/api/coupon/{id:int}", (int id) =>
{
    var coupon = CouponStore.couponList.FirstOrDefault(c => c.Id == id);
    if (coupon is null)
    {
        return Results.NotFound("Coupon not exists");
    }

    CouponStore.couponList.Remove(coupon);
    return Results.NoContent();
});

app.UseHttpsRedirection();
app.Run();