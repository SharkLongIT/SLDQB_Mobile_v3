using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Runtime.Caching;
using Abp.Runtime.Caching.Configuration;

namespace BBK.SaaS.Mdls.Cms.Caching
{
    /// <summary>
    /// An upper level container for <see cref="ICache"/> objects.
    /// <para>
    /// ICache objects requested from <see cref="ICmsRedisCacheManager"/> stores redis caches per http context. And it does not try to pull it again from redis during the http context.
    /// It should not be used if the changes on this data in one instance of the project can causes an error in the other instance of the project.
    /// It is only recommended to use it in cases that you know cache will never be changed until current http context is done or it is not an important value. (for example some ui view settings etc.)
    /// Otherwise use <see cref="ICacheManager"/>
    /// </para>
    /// A cache manager should work as Singleton and track and manage <see cref="ICache"/> objects.
    /// </summary>
    public interface ICmsRedisCacheManager : ICacheManager
    {
    }

	/// <summary>
    /// Used to create <see cref="CmsRedisCache"/> instances.
    /// </summary>
    public class CmsRedisCacheManager : CacheManagerBase<ICache>, ICmsRedisCacheManager
    {
        private readonly IIocManager _iocManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="CmsRedisCacheManager"/> class.
        /// </summary>
        public CmsRedisCacheManager(IIocManager iocManager, ICachingConfiguration configuration)
            : base(configuration)
        {
            _iocManager = iocManager;
            _iocManager.RegisterIfNot<CmsRedisCache>(DependencyLifeStyle.Transient);
        }

        protected override ICache CreateCacheImplementation(string name)
        {
            return _iocManager.Resolve<CmsRedisCache>(new {name});
        }

        protected override void DisposeCaches()
        {
            foreach (var cache in Caches)
            {
                _iocManager.Release(cache.Value);
            }
        }
    }
}
