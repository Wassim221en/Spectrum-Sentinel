using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Template.Dashboard.Core.Response;
using Template.Domain.Exceptions.Http;

namespace Template.Dashboard.Neighborhood.Queries.GetById;

public class GetNeighborhoodByIdHandler : IRequestHandler<GetNeighborhoodByIdQuery.Request, OperationResponse<GetNeighborhoodByIdQuery.Response>>
{
    private readonly IRepository _repository;

    public GetNeighborhoodByIdHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<OperationResponse<GetNeighborhoodByIdQuery.Response>> Handle(GetNeighborhoodByIdQuery.Request request, CancellationToken cancellationToken)
    {
        var neighborhood = await _repository.Query<Domain.Entities.Settings.Neighborhood>()
            .Include(n => n.City)
            .Where(n => n.Id == request.Id)
            .Select(GetNeighborhoodByIdQuery.Response.Selector())
            .FirstOrDefaultAsync(cancellationToken);

        if (neighborhood is null)
            return new HttpMessage("Neighborhood not found", HttpStatusCode.NotFound);

        return neighborhood;
    }
}

