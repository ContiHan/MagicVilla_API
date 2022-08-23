using AutoMapper;
using FluentValidation;
using MagicVilla_CouponMinimalAPI;
using MagicVilla_CouponMinimalAPI.Data;
using MagicVilla_CouponMinimalAPI.Models;
using MagicVilla_CouponMinimalAPI.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/api/coupons", (ILogger<Program> _logger) =>
{
    _logger.Log(LogLevel.Information, "Getting all coupons");
    return CouponStore.couponList.Any() ? Results.Ok(CouponStore.couponList) : Results.NotFound("Empty coupon store");
})
    .WithName("GetCoupons")
    .Produces<IEnumerable<Coupon>>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound);

app.MapGet("/api/coupon/{id:int}", (ILogger<Program> _logger, int id) =>
{
    if (CouponStore.couponList is null || !CouponStore.couponList.Any())
    {
        return Results.NotFound("Empty coupon store");
    }

    if (id == 0)
    {
        _logger.Log(LogLevel.Error, $"Id is zero");
        return Results.BadRequest("Id cannot be zero");
    }

    var coupon = CouponStore.couponList.FirstOrDefault(c => c.Id == id);
    if (coupon is null)
    {
        return Results.NotFound("Id not exists");
    }

    _logger.Log(LogLevel.Information, $"Getting coupon with id: {coupon.Id} and name: {coupon.Name}");
    return Results.Ok(coupon);
})
    .WithName("GetCoupon")
    .Produces<Coupon>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status404NotFound);

app.MapPost("/api/coupon", async (IMapper _mapper, IValidator<CouponCreateDTO> _validator, [FromBody] CouponCreateDTO couponCreateDTO) =>
{
    var validationResult = await _validator.ValidateAsync(couponCreateDTO);
    if (!validationResult.IsValid)
    {
        return Results.BadRequest(validationResult.Errors.FirstOrDefault().ToString());
    }

    if (CouponStore.couponList is null)
    {
        return Results.NotFound("Coupon store not found");
    }

    if (CouponStore.couponList.FirstOrDefault(c => c.Name.ToLower() == couponCreateDTO.Name.ToLower()) is not null)
    {
        return Results.BadRequest("Coupon Name already exists");
    }

    var coupon = _mapper.Map<Coupon>(couponCreateDTO);
    coupon.Created = coupon.LastUpdate = DateTime.Now;
    if (CouponStore.couponList.Any())
    {
        coupon.Id = CouponStore.couponList.OrderByDescending(c => c.Id).FirstOrDefault().Id + 1;
    }
    else
    {
        coupon.Id = 1;
    }
    CouponStore.couponList.Add(coupon);

    var couponDTO = _mapper.Map<CouponDTO>(coupon);
    return Results.CreatedAtRoute("GetCoupon", new { coupon.Id }, couponDTO);
})
    .WithName("CreateCoupon")
    .Accepts<CouponCreateDTO>("application/json")
    .Produces<CouponDTO>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status404NotFound);

app.MapPut("/api/coupon", async (IMapper _mapper, IValidator<CouponUpdateDTO> _validator, [FromBody] CouponUpdateDTO couponUpdateDTO) =>
{
    var validationResult = await _validator.ValidateAsync(couponUpdateDTO);
    if (!validationResult.IsValid)
    {
        return Results.BadRequest(validationResult.Errors.FirstOrDefault().ToString());
    }

    var couponFromStore = CouponStore.couponList.FirstOrDefault(c => c.Id == couponUpdateDTO.Id);
    if (couponFromStore is null)
    {
        return Results.NotFound("Coupon not exists");
    }

    _mapper.Map(couponUpdateDTO, couponFromStore);
    couponFromStore.LastUpdate = DateTime.Now;

    var couponDTO = _mapper.Map<CouponDTO>(couponFromStore);
    return Results.Ok(couponDTO);
})
    .WithName("UpdateCoupon")
    .Accepts<CouponUpdateDTO>("application/json")
    .Produces<CouponDTO>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status404NotFound);

app.MapDelete("/api/coupon/{id:int}", (int id) =>
{
    var coupon = CouponStore.couponList.FirstOrDefault(c => c.Id == id);
    if (coupon is null)
    {
        return Results.NotFound("Coupon not exists");
    }

    CouponStore.couponList.Remove(coupon);
    return Results.NoContent();
})
    .WithName("DeleteCoupon")
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status404NotFound);

app.UseHttpsRedirection();
app.Run();