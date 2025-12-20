using MediatR;
using Template.Dashboard.Core.Response;
using Template.Dashboard.Country.Queries.GetAll;

namespace Template.Dashboard.Country.Commands.Add;

public class AddCountryHandler : IRequestHandler<AddCountryCommand.Request, OperationResponse<GetAllCountriesQuery.Response.CountryRes>>
{
    private readonly IRepository _repository;

    public AddCountryHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<OperationResponse<GetAllCountriesQuery.Response.CountryRes>> Handle(AddCountryCommand.Request request, CancellationToken cancellationToken)
    {
        var country = new Domain.Entities.Settings.Country
        {
            Name = request.Name
        };

        await _repository.AddAsync(country);
        await _repository.SaveChangesAsync(cancellationToken);

        return new GetAllCountriesQuery.Response.CountryRes
        {
            CountryId = country.Id,
            Name = country.Name
        };
    }
}

