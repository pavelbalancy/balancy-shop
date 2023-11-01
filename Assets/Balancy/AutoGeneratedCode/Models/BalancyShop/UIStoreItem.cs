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
		public readonly UnnyObject Background;

		[JsonProperty("name")]
		public readonly string Name;

		[JsonProperty("asset")]
		public readonly UnnyAsset Asset;

		[JsonProperty("button2")]
		public readonly UnnyAsset Button2;

		[JsonProperty("button")]
		public readonly UnnyObject Button;

	}
#pragma warning restore 649
}