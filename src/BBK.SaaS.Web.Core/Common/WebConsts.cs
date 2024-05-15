using System.Collections.Generic;

namespace BBK.SaaS.Web.Common
{
	public static class WebConsts
	{
		public const string SwaggerUiEndPoint = "/swagger";
		public const string HangfireDashboardEndPoint = "/hangfire";

		public static bool SwaggerUiEnabled = true;
		public static bool HangfireDashboardEnabled = false;

		public const string UnknowChar = "?";

		public static List<string> ReCaptchaIgnoreWhiteList = new List<string>
		{
			SaaSConsts.AbpApiClientUserAgent
		};

		public static class GraphQL
		{
			public const string PlaygroundEndPoint = "/ui/playground";
			public const string EndPoint = "/graphql";

			public static bool PlaygroundEnabled = false;
			public static bool Enabled = false;
		}

		/// <summary>
		/// Gets a subset of the given array as a new array.
		/// </summary>
		/// <typeparam name="T">The array type</typeparam>
		/// <param name="arr">The array</param>
		/// <param name="startpos">The startpos</param>
		/// <param name="length">The length</param>
		/// <returns>The new array</returns>
		public static T[] Subset<T>(this T[] arr, int startpos = 0, int length = 0)
		{
			List<T> tmp = new List<T>();

			length = length > 0 ? length : arr.Length - startpos;

			for (var i = 0; i < arr.Length; i++)
			{
				if (i >= startpos && i < (startpos + length))
				{
					tmp.Add(arr[i]);
				}
			}
			return tmp.ToArray();
		}
	}
}
