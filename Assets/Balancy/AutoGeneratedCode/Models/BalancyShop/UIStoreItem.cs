using Newtonsoft.Json;
using System;

namespace Balancy.Models.BalancyShop
{
#pragma warning disable 649

	public class UIStoreItem : BaseModel
	{

		[JsonProperty]
		private string unnyIdBadge;


		[JsonProperty("content")]
		public readonly Models.BalancyShop.UIItem[] Content;

		[JsonProperty("background")]
		public readonly UnnyObject Background;

		[JsonProperty("name")]
		public readonly string Name;

		[JsonProperty("asset")]
		public readonly UnnyAsset Asset;

		[JsonIgnore]
		public Models.BalancyShop.Badge Badge => DataEditor.GetModelByUnnyId<Models.BalancyShop.Badge>(unnyIdBadge);

		[JsonProperty("button")]
		public readonly UnnyObject Button;

	}
#pragma warning restore 649
}