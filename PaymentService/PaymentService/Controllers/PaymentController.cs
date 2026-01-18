using Microsoft.AspNetCore.Mvc;

namespace PaymentService.Controllers
{
    [ApiController]
    [Route("api/payments")]
    public class PaymentController : ControllerBase
    {
        private static readonly Random _random = new();

        [HttpPost]
        public async Task<IActionResult> ProcessPayment()
        {
            var chance = _random.Next(1, 101);

            // 30% chance of hard failure
            if (chance <= 30)
            {
                return StatusCode(500, "Payment service failed");
            }

            // 30% chance of slow response
            if (chance <= 60)
            {
                await Task.Delay(TimeSpan.FromSeconds(5));
            }

            // 40% success
            return Ok(new
            {
                Status = "Payment successful",
                Timestamp = DateTime.UtcNow
            });
        }
    }
}
