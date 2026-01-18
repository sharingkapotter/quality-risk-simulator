using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.models;
using Polly.CircuitBreaker;


[ApiController]
[Route("api/orders")]
[Authorize]
public class OrdersController : ControllerBase
{
    private static readonly List<Order> Orders = new();
    private readonly IHttpClientFactory _httpClientFactory;

    public OrdersController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] string product)
    {
        var username = User.Identity?.Name;

        var order = new Order
        {
            User = username!,
            Product = product
        };

        Orders.Add(order);

        var client = _httpClientFactory.CreateClient("PaymentService");

        try
        {
            var response = await client.PostAsync("/api/payments", null);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode(502, "Payment failed, order created but unpaid");
            }
        }
        catch (BrokenCircuitException)
        {
            return StatusCode(503, "Payment service temporarily unavailable (circuit open)");
        }
        catch (TaskCanceledException)
        {
            return StatusCode(504, "Payment timed out");
        }
        catch (Exception)
        {
            return StatusCode(502, "Payment service unavailable");
        }

        return Ok(order);
    }

}
