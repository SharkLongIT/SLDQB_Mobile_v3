using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using BBK.SaaS.Storage;

namespace BBK.SaaS.Mdls.Cms.Medias.Dto
{
	public class MediaDto
	{
		public long Id { get; set; }

		public long? FolderId { get; set; }

		/// <summary>
		/// Gets/sets the media type.
		/// </summary>
		public MediaType Type { get; set; }

		/// <summary>
		/// Gets/sets the filename.
		/// </summary>
		public string Filename { get; set; }

		/// <summary>
		/// Gets/sets the content type.
		/// </summary>
		public string ContentType { get; set; }

		/// <summary>
		/// Gets/sets the optional title.
		/// </summary>
		public string Title { get; set; }

		//public string RelativePath { get; set; }

		/// <summary>
		/// Gets/sets the public url used to access the uploaded media.
		/// </summary>
		public string PublicUrl { get; set; }

		/// <summary>
		/// Gets/sets the public url used to access the uploaded media.
		/// </summary>
		public string ThumbUrl { get; set; }

		/// <summary>
		/// Gets/sets the file size.
		/// </summary>
		public string Size { get; set; }

		public DateTime? Modified { get; set; }
	}
}
