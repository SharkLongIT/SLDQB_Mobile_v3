using Abp.Runtime.Security;
using BBK.SaaS.Dto;
using BBK.SaaS.Security;
using BBK.SaaS.Storage;
using System.IO;
using System.Web;

namespace BBK.SaaS.Web.Helpers
{
	public static class FileHelpers
	{
		public static FileMgr GetFileInfo(string filePath)
		{
			if (string.IsNullOrWhiteSpace(filePath))
			{
				throw new System.ArgumentException($"'{nameof(filePath)}' cannot be null or whitespace.", nameof(filePath));
			}

			FileMgr fileMgr = new()
			{
				FileName = Path.GetFileName(filePath),
				FileUrl = HttpUtility.UrlEncode(StringCipher.Instance.Encrypt(filePath)),
				FilePath = StringCipher.Instance.Encrypt(filePath)
			};
			return fileMgr;
		}
	}
}
