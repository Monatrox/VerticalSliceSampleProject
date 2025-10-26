using System.Net;
using FastEndpoints;
using Weather.Infrastructure.Services;
using Weather.Domain.Objects;

namespace Weather.Application.Endpoints.CreateLocation;

public class CreateLocationEndpoint : Endpoint<CreateLocationRequest, CreateLocationResponse>
{
    private readonly ILocationService _service;

    public CreateLocationEndpoint(ILocationService service)
    {
        _service = service;
    }

    public override void Configure()
    {
        Post("/locations");
        AllowAnonymous();
    }

    public override async Task<CreateLocationResponse> ExecuteAsync(CreateLocationRequest request, CancellationToken ct)
    {
        try
        {
            var location = new Location
            {
                City = request.City,
                State = request.State
            };

            var data = await _service.CreateAsync(location, ct);
            
            return new CreateLocationResponse(data);
        }
        catch (InvalidOperationException ex)
        {
            return new CreateLocationResponse(HttpStatusCode.BadRequest, ex.Message);
        }
        catch (Exception ex)
        {
            return new CreateLocationResponse(HttpStatusCode.InternalServerError, ex.Message);
        }
    }
}