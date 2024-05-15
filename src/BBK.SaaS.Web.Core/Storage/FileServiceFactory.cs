//using Abp.Configuration;
//using Abp.Dependency;
//using BBK.SaaS.Storage;
//using System.Threading.Tasks;

//namespace BBK.SaaS.Web.Storage
//{
//	public class FileServiceFactory : ITransientDependency
//	{
//		private readonly ISettingManager _settingManager;
//		private readonly IIocResolver _iocResolver;

//		public FileServiceFactory(
//			ISettingManager settingManager,
//			IIocResolver iocResolver)
//		{
//			_settingManager = settingManager;
//			_iocResolver = iocResolver;
//		}

//		//public async Task<IDisposableDependencyObjectWrapper<IFileManager>> Get()
//		public IDisposableDependencyObjectWrapper<IFileManager> Get()
//		{
//			//if (await _settingManager.GetSettingValueForUserAsync<bool>(AppSettings.UserManagement.UseGravatarProfilePicture, userIdentifier))
//			//{
//			//	return _iocResolver.ResolveAsDisposable<GravatarProfileImageService>();
//			//}

//			return _iocResolver.ResolveAsDisposable<FileManager>();
//		}

//		//public async Task<IDisposableDependencyObjectWrapper<IProfileImageService>> CreateFile()
//		//{
//		//	if (await _settingManager.GetSettingValueForUserAsync<bool>(AppSettings.UserManagement.UseGravatarProfilePicture, userIdentifier))
//		//	{
//		//		return _iocResolver.ResolveAsDisposable<GravatarProfileImageService>();
//		//	}

//		//	return _iocResolver.ResolveAsDisposable<LocalProfileImageService>();
//		//}
//	}
//}
