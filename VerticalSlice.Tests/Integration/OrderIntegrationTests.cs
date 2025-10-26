using System.Net;
using System.Net.Http.Json;
using Order.Application.Endpoints.AddOrderItem;
using Order.Application.Endpoints.ConfirmOrder;
using Order.Application.Endpoints.CreateOrder;
using Order.Application.Endpoints.GetOrder;
using Order.Application.Endpoints.ShipOrder;
using Order.Domain.Enums;
using Shouldly;
using VerticalSlice.Tests.Fixtures;

namespace VerticalSlice.Tests.Integration;

// These are not even REMOTELY ideal, but here we are
// AI generated tests at least prove that things exist

[Collection("AppHost")]
public class OrderIntegrationTests(AppHostFixture fixture)
{
    [Fact]
    public async Task CreateOrder_ShouldReturnOrderId()
    {
        // Arrange
        var request = new CreateOrderRequest
        {
            CustomerId = "customer-123"
        };

        // Act
        var response = await fixture.Client!.PostAsJsonAsync("/orders", request);
        var result = await response.Content.ReadFromJsonAsync<CreateOrderResponse>();

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.ShouldNotBeNull();
        result.Success.ShouldBeTrue();
        result.Data.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public async Task GetOrder_ShouldReturnOrder_WhenOrderExists()
    {
        // Arrange - Create an order first
        var createRequest = new CreateOrderRequest { CustomerId = "customer-456" };
        var createResponse = await fixture.Client!.PostAsJsonAsync("/orders", createRequest);
        var createResult = await createResponse.Content.ReadFromJsonAsync<CreateOrderResponse>();
        var orderId = createResult!.Data;

        // Act
        var response = await fixture.Client.GetAsync($"/orders/{orderId}");
        var result = await response.Content.ReadFromJsonAsync<GetOrderResponse>();

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.ShouldNotBeNull();
        result.Success.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Data.Id.ShouldBe(orderId);
        result.Data.CustomerId.ShouldBe("customer-456");
        result.Data.Status.ShouldBe(OrderStatus.Draft);
        result.Data.Items.ShouldBeEmpty();
    }

    // [Fact]
    // public async Task GetOrder_ShouldReturnNotFound_WhenOrderDoesNotExist()
    // {
    //     // Arrange
    //     var nonExistentOrderId = Guid.NewGuid();
    //
    //     // Act
    //     var response = await fixture.Client!.GetAsync($"/orders/{nonExistentOrderId}");
    //     var result = await response.Content.ReadFromJsonAsync<GetOrderResponse>();
    //
    //     // Assert
    //     response.StatusCode.ShouldBe(HttpStatusCode.OK);
    //     result.ShouldNotBeNull();
    //     result.Success.ShouldBeFalse();
    //     result.Status.ShouldBe((int)HttpStatusCode.NotFound);
    // }

    [Fact]
    public async Task AddOrderItem_ShouldAddItemToOrder()
    {
        // Arrange - Create an order first
        var createRequest = new CreateOrderRequest { CustomerId = "customer-789" };
        var createResponse = await fixture.Client!.PostAsJsonAsync("/orders", createRequest);
        var createResult = await createResponse.Content.ReadFromJsonAsync<CreateOrderResponse>();
        var orderId = createResult!.Data;

        var addItemRequest = new AddOrderItemRequest
        {
            OrderId = orderId,
            ProductId = "product-001",
            ProductName = "Test Product",
            Quantity = 2,
            UnitPrice = 29.99m
        };

        // Act
        var response = await fixture.Client.PostAsJsonAsync($"/orders/{orderId}/items", addItemRequest);
        var result = await response.Content.ReadFromJsonAsync<AddOrderItemResponse>();

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.ShouldNotBeNull();
        result.Success.ShouldBeTrue();

        // Verify the item was added
        var getResponse = await fixture.Client.GetAsync($"/orders/{orderId}");
        var getResult = await getResponse.Content.ReadFromJsonAsync<GetOrderResponse>();
        getResult!.Data.Items.Count.ShouldBe(1);
        getResult.Data.Items[0].ProductId.ShouldBe("product-001");
        getResult.Data.Items[0].ProductName.ShouldBe("Test Product");
        getResult.Data.Items[0].Quantity.ShouldBe(2);
        getResult.Data.Items[0].UnitPrice.ShouldBe(29.99m);
        getResult.Data.Items[0].TotalPrice.ShouldBe(59.98m);
        getResult.Data.TotalAmount.ShouldBe(59.98m);
    }

    [Fact]
    public async Task AddOrderItem_ShouldAddMultipleItems()
    {
        // Arrange - Create an order first
        var createRequest = new CreateOrderRequest { CustomerId = "customer-multi" };
        var createResponse = await fixture.Client!.PostAsJsonAsync("/orders", createRequest);
        var createResult = await createResponse.Content.ReadFromJsonAsync<CreateOrderResponse>();
        var orderId = createResult!.Data;

        var item1 = new AddOrderItemRequest
        {
            OrderId = orderId,
            ProductId = "product-001",
            ProductName = "Product 1",
            Quantity = 2,
            UnitPrice = 10.00m
        };

        var item2 = new AddOrderItemRequest
        {
            OrderId = orderId,
            ProductId = "product-002",
            ProductName = "Product 2",
            Quantity = 1,
            UnitPrice = 25.00m
        };

        // Act
        await fixture.Client.PostAsJsonAsync($"/orders/{orderId}/items", item1);
        await fixture.Client.PostAsJsonAsync($"/orders/{orderId}/items", item2);

        // Assert
        var getResponse = await fixture.Client.GetAsync($"/orders/{orderId}");
        var getResult = await getResponse.Content.ReadFromJsonAsync<GetOrderResponse>();
        getResult!.Data.Items.Count.ShouldBe(2);
        getResult.Data.TotalAmount.ShouldBe(45.00m); // 20 + 25
    }

    [Fact]
    public async Task ConfirmOrder_ShouldChangeStatusToConfirmed()
    {
        // Arrange - Create an order and add an item
        var createRequest = new CreateOrderRequest { CustomerId = "customer-confirm" };
        var createResponse = await fixture.Client!.PostAsJsonAsync("/orders", createRequest);
        var createResult = await createResponse.Content.ReadFromJsonAsync<CreateOrderResponse>();
        var orderId = createResult!.Data;

        var addItemRequest = new AddOrderItemRequest
        {
            OrderId = orderId,
            ProductId = "product-001",
            ProductName = "Test Product",
            Quantity = 1,
            UnitPrice = 10.00m
        };
        await fixture.Client.PostAsJsonAsync($"/orders/{orderId}/items", addItemRequest);

        var confirmRequest = new ConfirmOrderRequest { OrderId = orderId };

        // Act
        var response = await fixture.Client.PostAsJsonAsync($"/orders/{orderId}/confirm", confirmRequest);
        var result = await response.Content.ReadFromJsonAsync<ConfirmOrderResponse>();

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.ShouldNotBeNull();
        result.Success.ShouldBeTrue();

        // Verify the status changed
        var getResponse = await fixture.Client.GetAsync($"/orders/{orderId}");
        var getResult = await getResponse.Content.ReadFromJsonAsync<GetOrderResponse>();
        getResult!.Data.Status.ShouldBe(OrderStatus.Confirmed);
        getResult.Data.ConfirmedAt.ShouldNotBeNull();
    }

    [Fact]
    public async Task ShipOrder_ShouldChangeStatusToShipped()
    {
        // Arrange - Create, add item, and confirm an order
        var createRequest = new CreateOrderRequest { CustomerId = "customer-ship" };
        var createResponse = await fixture.Client!.PostAsJsonAsync("/orders", createRequest);
        var createResult = await createResponse.Content.ReadFromJsonAsync<CreateOrderResponse>();
        var orderId = createResult!.Data;

        var addItemRequest = new AddOrderItemRequest
        {
            OrderId = orderId,
            ProductId = "product-001",
            ProductName = "Test Product",
            Quantity = 1,
            UnitPrice = 10.00m
        };
        await fixture.Client.PostAsJsonAsync($"/orders/{orderId}/items", addItemRequest);

        var confirmRequest = new ConfirmOrderRequest { OrderId = orderId };
        await fixture.Client.PostAsJsonAsync($"/orders/{orderId}/confirm", confirmRequest);

        var shipRequest = new ShipOrderRequest { OrderId = orderId };

        // Act
        var response = await fixture.Client.PostAsJsonAsync($"/orders/{orderId}/ship", shipRequest);
        var result = await response.Content.ReadFromJsonAsync<ShipOrderResponse>();

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.ShouldNotBeNull();
        result.Success.ShouldBeTrue();

        // Verify the status changed
        var getResponse = await fixture.Client.GetAsync($"/orders/{orderId}");
        var getResult = await getResponse.Content.ReadFromJsonAsync<GetOrderResponse>();
        getResult!.Data.Status.ShouldBe(OrderStatus.Shipped);
        getResult.Data.ShippedAt.ShouldNotBeNull();
    }

    // [Fact]
    // public async Task CompleteOrderWorkflow_ShouldSucceed()
    // {
    //     // This test validates the complete order lifecycle
    //     
    //     // 1. Create Order
    //     var createRequest = new CreateOrderRequest { CustomerId = "customer-workflow" };
    //     var createResponse = await fixture.Client!.PostAsJsonAsync("/orders", createRequest);
    //     var createResult = await createResponse.Content.ReadFromJsonAsync<CreateOrderResponse>();
    //     var orderId = createResult!.Data;
    //     
    //     // Verify Draft status
    //     var getAfterCreate = await fixture.Client.GetAsync($"/orders/{orderId}");
    //     var orderAfterCreate = await getAfterCreate.Content.ReadFromJsonAsync<GetOrderResponse>();
    //     orderAfterCreate!.Data.Status.ShouldBe(OrderStatus.Draft);
    //
    //     // 2. Add Items
    //     await fixture.Client.PostAsJsonAsync($"/orders/{orderId}/items", new AddOrderItemRequest
    //     {
    //         OrderId = orderId,
    //         ProductId = "product-001",
    //         ProductName = "Widget",
    //         Quantity = 3,
    //         UnitPrice = 15.00m
    //     });
    //     
    //     await fixture.Client.PostAsJsonAsync($"/orders/{orderId}/items", new AddOrderItemRequest
    //     {
    //         OrderId = orderId,
    //         ProductId = "product-002",
    //         ProductName = "Gadget",
    //         Quantity = 1,
    //         UnitPrice = 50.00m
    //     });
    //
    //     // Verify items were added
    //     var getAfterItems = await fixture.Client.GetAsync($"/orders/{orderId}");
    //     var orderAfterItems = await getAfterItems.Content.ReadFromJsonAsync<GetOrderResponse>();
    //     orderAfterItems!.Data.Items.Count.ShouldBe(2);
    //     orderAfterItems.Data.TotalAmount.ShouldBe(95.00m); // (3 * 15) + (1 * 50)
    //
    //     // 3. Confirm Order
    //     await fixture.Client.PostAsJsonAsync($"/orders/{orderId}/confirm", new ConfirmOrderRequest { OrderId = orderId });
    //     
    //     var getAfterConfirm = await fixture.Client.GetAsync($"/orders/{orderId}");
    //     var orderAfterConfirm = await getAfterConfirm.Content.ReadFromJsonAsync<GetOrderResponse>();
    //     orderAfterConfirm!.Data.Status.ShouldBe(OrderStatus.Confirmed);
    //     orderAfterConfirm.Data.ConfirmedAt.ShouldNotBeNull();
    //
    //     // 4. Ship Order
    //     await fixture.Client.PostAsJsonAsync($"/orders/{orderId}/ship", new ShipOrderRequest { OrderId = orderId });
    //     
    //     var getAfterShip = await fixture.Client.GetAsync($"/orders/{orderId}");
    //     var orderAfterShip = await getAfterShip.Content.ReadFromJsonAsync<GetOrderResponse>();
    //     orderAfterShip!.Data.Status.ShouldBe(OrderStatus.Shipped);
    //     orderAfterShip.Data.ShippedAt.ShouldNotBeNull();
    //     
    //     // Verify all timestamps
    //     orderAfterShip.Data.CreatedAt.ShouldNotBe(default);
    //     orderAfterShip.Data.ConfirmedAt.ShouldNotBe(null);
    //     orderAfterShip.Data.ShippedAt.ShouldNotBe(null);
    //     orderAfterShip.Data.CreatedAt.ShouldBeLessThan(orderAfterShip.Data.ConfirmedAt!.Value);
    //     orderAfterShip.Data.ConfirmedAt!.Value.ShouldBeLessThanOrEqualTo(orderAfterShip.Data.ShippedAt!.Value);
    // }
}