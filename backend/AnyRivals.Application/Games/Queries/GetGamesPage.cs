using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using AnyRivals.Application.Common.Extensions;
using AnyRivals.Application.Common.Models;
using AnyRivals.Application.Common.Models.DTOs;
using AnyRivals.Domain.Entities;
using AnyRivals.Application.Common.Interfaces.Data;
using AnyRivals.Application.Common.Interfaces.Handlers;

namespace AnyRivals.Application.Games.Queries;
public record GetGamesPageQuery: IQuery<PaginatedList<GameDto>>
{
    public string? NameFilter { get; init; }

    public bool OnlyWithoutPassword { get; init; }

    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = 10;
}

public class GetGamesPageQueryHandler : IQueryHandler<GetGamesPageQuery, PaginatedList<GameDto>>
{
    private readonly IDataAccessContext _context;
    private readonly IMapper _mapper;

    public GetGamesPageQueryHandler(IDataAccessContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<GameDto>> Handle(GetGamesPageQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Games
            .Where(x => request.NameFilter == null
                || x.Name.Contains(request.NameFilter, StringComparison.InvariantCultureIgnoreCase)
            && !request.OnlyWithoutPassword
                || string.IsNullOrWhiteSpace(x.GamePassword))
            .ProjectTo<GameDto>(_mapper.ConfigurationProvider);

        return await query.PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}
