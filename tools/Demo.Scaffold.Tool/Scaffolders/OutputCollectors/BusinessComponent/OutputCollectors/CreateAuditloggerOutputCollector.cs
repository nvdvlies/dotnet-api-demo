﻿using Demo.Scaffold.Tool.Changes;
using Demo.Scaffold.Tool.Helpers;
using Demo.Scaffold.Tool.Interfaces;
using System.Collections.Generic;
using System.IO;

namespace Demo.Scaffold.Tool.Scaffolders.OutputCollectors.BusinessComponent.OutputCollectors
{
    internal class CreateAuditloggerOutputCollector : IOutputCollector
    {
        public IEnumerable<IChange> CollectChanges(ScaffolderContext context)
        {
            var changes = new List<IChange>();

            context.Variables.TryGet<bool>(Constants.EnableAuditlogging, out var enableAuditlogging);
            if (!enableAuditlogging)
            {
                return changes;
            }

            var entityName = context.Variables.Get<string>(Constants.EntityName);

            changes.Add(new CreateNewClass(
                directory: context.GetAuditloggerDirectory(),
                fileName: $"{entityName}Auditlogger.cs",
                content: GetTemplate(entityName)
            ));

            return changes;
        }

        private static string GetTemplate(string entityName)
        {
            var code = @"
using Demo.Common.Interfaces;
using Demo.Domain.Auditlog;
using Demo.Domain.Auditlog.BusinessComponent.Interfaces;
using Demo.Domain.%ENTITY%;
using Demo.Domain.Shared.Interfaces;
using Demo.Infrastructure.Auditlogging.Shared;
using System.Collections.Generic;

namespace Demo.Infrastructure.Auditlogging.Configuration
{
    internal class %ENTITY%Auditlogger : AuditlogBase<%ENTITY%>, IAuditlog<%ENTITY%>
    {
        public %ENTITY%Auditlogger(            
            ICurrentUser currentUser, 
            IDateTime dateTime,
            IAuditlogBusinessComponent auditlogBusinessComponent
        ) : base(currentUser, dateTime, auditlogBusinessComponent)
        {
        }

        protected override List<AuditlogItem> AuditlogItems(%ENTITY% current, %ENTITY% previous) => 
            new AuditlogBuilder<%ENTITY%>()
                .Build(current, previous);
    }
}
";
            code = code.Replace("%ENTITY%", entityName);
            return code;
        }
    }
}
