using Ecommerce_Final.Controllers;
using Ecommerce_Final.Data;
using Ecommerce_Final.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Ecommerce_Application_VerveBridge_Internship_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrderController(OrderService orderService)
        {
            _orderService = orderService;   
        }
        // GET: api/<OrderController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<OrderController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(Guid id)
        {
            var order = await _orderService.GetOrderById(id);
            if (order == null)
            {
                return NotFound($"Order with ID {id} not found.");
            }
            return Ok(order);
        }

        // POST api/<OrderController>
        [HttpPost("Checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequest request)
        {
            if (request == null)
                return BadRequest("Invalid request.");

            try
            {
                var order = await _orderService.CreateOrder(request.UserId, request.ShippingAddress, request.BillingAddress, request.PaymentMethod);
                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error during checkout: {ex.Message}");
            }
        }

        // PUT api/<OrderController>/5
        [HttpPut("{id}")]
        public void UpdateOrder(Guid id, Order updatedOrder)
        {
        }

        // DELETE api/<OrderController>/5
        [HttpDelete("{id}")]
        public void DeleteOrder(Guid id)
        {
        }
        public class CheckoutRequest
        {
            public Guid UserId { get; set; }
            public string ShippingAddress { get; set; }
            public string BillingAddress { get; set; }
            public string PaymentMethod { get; set; }
        }
    }
}
