using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnyRivals.Application.Common.Interfaces.Data;
using AnyRivals.Application.Common.Interfaces.Handlers;

namespace AnyRivals.Application.Games.Queries;

public record CheckGameExistanceQuery: IQuery<bool>
{
    public string Id { get; set; }
}

public class CheckGameExistanceQueryHandler : IQueryHandler<CheckGameExistanceQuery, bool>
{
    private readonly IDataAccessContext _context;

    public CheckGameExistanceQueryHandler(IDataAccessContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(CheckGameExistanceQuery request, CancellationToken cancellationToken)
    {
        return await _context.Games.AnyAsync(x => x.ExternalId == request.Id, cancellationToken);
    }
}
