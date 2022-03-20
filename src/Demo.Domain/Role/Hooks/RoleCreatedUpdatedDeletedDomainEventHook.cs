using Demo.Common.Interfaces;
using Demo.Domain.Shared.DomainEntity;
using Demo.Domain.Shared.Interfaces;
using Demo.Events.Role;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.Domain.Role.Hooks
{
    internal class RoleCreatedUpdatedDeletedEventHook : IAfterCreate<Role>, IAfterUpdate<Role>, IAfterDelete<Role>
    {
        private readonly ICurrentUser _currentUser;
        private readonly ICorrelationIdProvider _correlationIdProvider;

        public RoleCreatedUpdatedDeletedEventHook(
            ICurrentUser currentUser,
            ICorrelationIdProvider correlationIdProvider
        )
        {
            _currentUser = currentUser;
            _correlationIdProvider = correlationIdProvider;
        }

        public Task ExecuteAsync(HookType type, IDomainEntityContext<Role> context, CancellationToken cancellationToken)
        {
            switch (context.EditMode)
            {
                case EditMode.Create:
                    context.AddEventAsync(RoleCreatedEvent.Create(_correlationIdProvider.Id, context.Entity.Id, _currentUser.Id), cancellationToken);
                    break;
                case EditMode.Update:
                    context.AddEventAsync(RoleUpdatedEvent.Create(_correlationIdProvider.Id, context.Entity.Id, _currentUser.Id), cancellationToken);
                    break;
                case EditMode.Delete:
                    context.AddEventAsync(RoleDeletedEvent.Create(_correlationIdProvider.Id, context.Entity.Id, _currentUser.Id), cancellationToken);
                    break;
            }
            return Task.CompletedTask;
        }
    }
}