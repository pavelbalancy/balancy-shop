using Newtonsoft.Json;
using System;

namespace Balancy.Models.BalancyShop
{
#pragma warning disable 649

	public class UIStoreItem : BaseModel
	{

		[JsonProperty]
		private string unnyIdBadge;


		[JsonProperty("name")]
		public readonly string Name;

		[JsonProperty("asset")]
		public readonly UnnyAsset Asset;

		[JsonProperty("content")]
		public readonly Models.BalancyShop.UIItem[] Content;

		[JsonProperty("background")]
		public readonly UnnyObject Background;

		[JsonProperty("button")]
		public readonly UnnyObject Button;

		[JsonIgnore]
		public Models.BalancyShop.Badge Badge => DataEditor.GetModelByUnnyId<Models.BalancyShop.Badge>(unnyIdBadge);

	}
#pragma warning restore 649
}