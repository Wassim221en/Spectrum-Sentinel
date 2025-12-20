using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Template.Dashboard.Core.Response;
using Template.Domain.Exceptions.Http;

namespace Template.Dashboard.City.Queries.GetById;

public class GetCityByIdHandler : IRequestHandler<GetCityByIdQuery.Request, OperationResponse<GetCityByIdQuery.Response>>
{
    private readonly IRepository _repository;

    public GetCityByIdHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<OperationResponse<GetCityByIdQuery.Response>> Handle(GetCityByIdQuery.Request request, CancellationToken cancellationToken)
    {
        var city = await _repository.Query<Domain.Entities.Settings.City>()
            .Include(c => c.Country)
            .Where(c => c.Id == request.Id)
            .Select(GetCityByIdQuery.Response.Selector())
            .FirstOrDefaultAsync(cancellationToken);

        if (city is null)
            return new HttpMessage("City not found", HttpStatusCode.NotFound);

        return city;
    }
}

