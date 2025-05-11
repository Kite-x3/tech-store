using System;
using TechStore.Application.DTOs;
using TechStore.Domain.Entities;
using TechStore.Domain.Interfaces;

public class OrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IOrderStatusRepository _orderStatusRepository;

    public OrderService(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        IOrderStatusRepository orderStatusRepository)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _orderStatusRepository = orderStatusRepository;
    }

    public async Task<OrderDto> GetOrderByIdAsync(int id)
    {
        var order = await _orderRepository.GetOrderByIdAsync(id);
        if (order == null)
            throw new KeyNotFoundException($"Order with ID {id} not found.");

        return MapToDto(order);
    }

    public async Task<IEnumerable<OrderDto>> GetOrdersAsync()
    {
        var orders = await _orderRepository.GetOrdersAsync();
        return orders.Select(MapToDto);
    }

    public async Task<IEnumerable<OrderDto>> GetUserOrdersAsync(string userId)
    {
        var orders = await _orderRepository.GetUserOrdersAsync(userId);
        return orders.Select(MapToDto);
    }

    public async Task<OrderDto> CreateOrderAsync(string userId, CreateOrderDto orderDto)
    {
        var pendingStatus = await _orderStatusRepository.GetStatusByNameAsync("Pending");

        if (pendingStatus == null)
            throw new Exception("Order status 'Pending' not found in database");

        var order = new Order
        {
            UserId = userId,
            OrderDate = DateTime.UtcNow,
            OrderStatusId = pendingStatus.OrderStatusId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        decimal totalAmount = 0;

        foreach (var item in orderDto.Items)
        {
            var product = await _productRepository.GetProductByIdAsync(item.ProductId);
            if (product == null)
                throw new KeyNotFoundException($"Product with ID {item.ProductId} not found.");

            var orderItem = new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = product.Price
            };

            totalAmount += orderItem.UnitPrice * orderItem.Quantity;
            order.OrderItems.Add(orderItem);
        }

        order.TotalAmount = totalAmount;
        await _orderRepository.AddOrderAsync(order);

        return MapToDto(order);
    }

    public async Task UpdateOrderStatusAsync(int orderId, int statusId)
    {
        var order = await _orderRepository.GetOrderByIdAsync(orderId);
        if (order == null)
            throw new KeyNotFoundException($"Order with ID {orderId} not found.");

        var statusExists = await _orderStatusRepository.StatusExistsAsync(statusId);
        if (!statusExists)
            throw new KeyNotFoundException($"Status with ID {statusId} not found.");

        order.OrderStatusId = statusId;
        order.UpdatedAt = DateTime.UtcNow;

        await _orderRepository.UpdateOrderAsync(order);
    }

    public async Task<IEnumerable<OrderStatusDto>> GetOrderStatusesAsync()
    {
        var statuses = await _orderStatusRepository.GetAllStatusesAsync();
        return statuses.Select(s => new OrderStatusDto
        {
            OrderStatusId = s.OrderStatusId,
            Name = s.Name,
            Description = s.Description
        });
    }

    private static OrderDto MapToDto(Order order)
    {
        if (order == null)
            throw new ArgumentNullException(nameof(order));

        if (order.Status == null)
            throw new InvalidOperationException("Order status is required");

        return new OrderDto
        {
            OrderId = order.OrderId,
            UserId = order.UserId,
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount,
            Status = new OrderStatusDto
            {
                OrderStatusId = order.Status.OrderStatusId,
                Name = order.Status.Name,
                Description = order.Status.Description
            },
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            OrderItems = order.OrderItems?.Select(oi => new OrderItemDto
            {
                ProductId = oi.ProductId,
                ProductName = oi.Product?.Name ?? "Unknown Product",
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice
            }).ToList() ?? new List<OrderItemDto>()
        };
    }
}