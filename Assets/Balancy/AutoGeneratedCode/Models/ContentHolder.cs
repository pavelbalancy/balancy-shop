using Newtonsoft.Json;
using System;

namespace Balancy.Models
{
#pragma warning disable 649

	public class ContentHolder : BaseModel
	{



		[JsonProperty("content")]
		public readonly Models.BalancyShop.UIItem[] Content;

	}
#pragma warning restore 649
}