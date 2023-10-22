using Newtonsoft.Json;
using System;

namespace Balancy.Models.BalancyShop
{
#pragma warning disable 649

	public class UIStoreItem : BaseModel
	{



		[JsonProperty("content")]
		public readonly Models.BalancyShop.UIItem[] Content;

		[JsonProperty("background")]
		public readonly UnnyAsset Background;

		[JsonProperty("name")]
		public readonly string Name;

		[JsonProperty("asset")]
		public readonly UnnyAsset Asset;

		[JsonProperty("button")]
		public readonly UnnyAsset Button;

	}
#pragma warning restore 649
}