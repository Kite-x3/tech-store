using Microsoft.AspNetCore.Mvc;
using TechStore.Application.DTOs;

namespace TechStore.Api.Controllers
{
    [ApiController]
    [Route("api/order-statuses")]
    public class OrderStatusesController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrderStatusesController(OrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderStatusDto>>> GetAll()
        {
            var statuses = await _orderService.GetOrderStatusesAsync();
            return Ok(statuses);
        }
    }
}
