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
    /// <summary>
    /// Получает заказ по ID
    /// </summary>
    /// <param name="id">ID заказа</param>
    /// <returns>DTO заказа</returns>
    /// <exception cref="KeyNotFoundException">Если заказ не найден</exception>
    public async Task<OrderDto> GetOrderByIdAsync(int id)
    {
        var order = await _orderRepository.GetOrderByIdAsync(id);
        if (order == null)
            throw new KeyNotFoundException($"Order with ID {id} not found.");

        return MapToDto(order);
    }
    /// <summary>
    /// Получает все заказы
    /// </summary>
    /// <returns>Список DTO заказов</returns>
    public async Task<IEnumerable<OrderDto>> GetOrdersAsync()
    {
        var orders = await _orderRepository.GetOrdersAsync();
        return orders.Select(MapToDto);
    }
    /// <summary>
    /// Получает заказы пользователя
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <returns>Список DTO заказов пользователя</returns>
    public async Task<IEnumerable<OrderDto>> GetUserOrdersAsync(string userId)
    {
        var orders = await _orderRepository.GetUserOrdersAsync(userId);
        return orders.Select(MapToDto);
    }
    /// <summary>
    /// Создает новый заказ
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <param name="orderDto">DTO с данными для создания заказа</param>
    /// <returns>DTO созданного заказа</returns>
    /// <exception cref="Exception">Если статус "Pending" не найден</exception>
    /// <exception cref="KeyNotFoundException">Если товар не найден</exception>
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
    /// <summary>
    /// Обновляет статус заказа
    /// </summary>
    /// <param name="orderId">ID заказа</param>
    /// <param name="statusId">ID нового статуса</param>
    /// <exception cref="KeyNotFoundException">Если заказ или статус не найдены</exception>
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
    /// <summary>
    /// Получает все возможные статусы заказов
    /// </summary>
    /// <returns>Список DTO статусов заказов</returns>
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
    /// <summary>
    /// Конвертирует объект заказа из серверной ef сущности в DTO сущность
    /// </summary>
    /// <param name="order">Серверная ef сущность заказа</param>
    /// <returns>DTO сущность</returns>
    /// <exception cref="ArgumentNullException">Если объект null</exception>
    /// <exception cref="InvalidOperationException">Если нет статуса</exception>
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