using AutoMapper;
using AutoMapper.QueryableExtensions;
using Demo.Application.Roles.Queries.GetRoleById.Dtos;
using Demo.Domain.Shared.Interfaces;
using Demo.Domain.Role;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.Application.Roles.Queries.GetRoleById
{
    public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, GetRoleByIdQueryResult>
    {
        private readonly IDbQuery<Role> _query;
        private readonly IMapper _mapper;

        public GetRoleByIdQueryHandler(
            IDbQuery<Role> query,
            IMapper mapper
        )
        {
            _query = query;
            _mapper = mapper;
        }

        public async Task<GetRoleByIdQueryResult> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
        {
            var role = await _query.AsQueryable()
                .ProjectTo<RoleDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            return new GetRoleByIdQueryResult { Role = role };
        }
    }
}