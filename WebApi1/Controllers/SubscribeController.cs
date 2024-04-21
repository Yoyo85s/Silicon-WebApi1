using Infrastructure.Context;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SubscribeController(ApiContext context) : ControllerBase
{
    private readonly ApiContext _context = context;
    [HttpPost]
    public async Task<IActionResult> Subscribe([FromBody] SubscribersEntity dto)
    {

        if (!ModelState.IsValid)
        {
            return StatusCode(StatusCodes.Status400BadRequest, ModelState);
        }

        try
        {
            if (await _context.Subscribers.AnyAsync(x => x.Email == dto.Email))
            {
                return StatusCode(StatusCodes.Status409Conflict, $"Email: \"{dto.Email}\" already subscribed!");
            }

            var subscriber = new SubscribersEntity
            {
                Email = dto.Email,
                DailyNewsletter = dto.DailyNewsletter,
                AdvertisingUpdates = dto.AdvertisingUpdates,
                WeekinReview = dto.WeekinReview,
                EventUpdates = dto.EventUpdates,
                StartupsWeekly = dto.StartupsWeekly,
                Podcasts = dto.Podcasts
            };

            _context.Subscribers.Add(subscriber);
            await _context.SaveChangesAsync();
            return StatusCode(StatusCodes.Status201Created, $"Subscriber \"{dto.Email}\" created successfully");
        }
        catch { return Problem("Faild to create Subscribtion"); }
    }
    //[HttpPost]
    //public async Task<IActionResult> Subscribe(string email)
    //{
    //    if (ModelState.IsValid)
    //    {
    //        if (await _context.Subscribers.AnyAsync(x => x.Email == email))
    //            return StatusCode(StatusCodes.Status409Conflict, $"Email: \"{email}\" already subscribed!");



    //        _context.Add(new SubscribersEntity { Email = email });
    //        await _context.SaveChangesAsync();
    //        return Ok();
    //    }

    //    return BadRequest();
    //}

    [HttpDelete]
    public async Task<IActionResult> Unsubscribe(string email)
    {
        if (ModelState.IsValid)
        {
            var SubscriberEntity = await _context.Subscribers.FirstOrDefaultAsync(x => x.Email == email);
            if (SubscriberEntity == null)
                return NotFound();

            _context.Remove(SubscriberEntity);
            await _context.SaveChangesAsync();
            return Ok();


        }

        return BadRequest();
    }
}

