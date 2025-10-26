using System.Net;
using FastEndpoints;
using Weather.Infrastructure.Services;
using Weather.Domain.Objects;

namespace Weather.Application.Endpoints.CreateLocation;

public class CreateLocationEndpoint(ILocationService service) : Endpoint<CreateLocationRequest, CreateLocationResponse>
{
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

            var data = await service.CreateAsync(location, ct);
            
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