using Abp.Dependency;
using GraphQL.Types;
using GraphQL.Utilities;
using BBK.SaaS.Queries.Container;
using System;

namespace BBK.SaaS.Schemas
{
    public class MainSchema : Schema, ITransientDependency
    {
        public MainSchema(IServiceProvider provider) :
            base(provider)
        {
            Query = provider.GetRequiredService<QueryContainer>();
        }
    }
}