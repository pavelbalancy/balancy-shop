using Newtonsoft.Json;
using System;

namespace Balancy.Models.BalancyShop
{
#pragma warning disable 649

	public class UIStoreItem : BaseModel
	{

		[JsonProperty]
		private string unnyIdBadge;
		[JsonProperty]
		private string unnyIdContentHolder;


		[JsonProperty("name")]
		public readonly string Name;

		[JsonProperty("asset")]
		public readonly UnnyAsset Asset;

		[JsonProperty("background")]
		public readonly UnnyObject Background;

		[JsonProperty("button")]
		public readonly UnnyObject Button;

		[JsonIgnore]
		public Models.BalancyShop.Badge Badge => DataEditor.GetModelByUnnyId<Models.BalancyShop.Badge>(unnyIdBadge);

		[JsonIgnore]
		public Models.ContentHolder ContentHolder => DataEditor.GetModelByUnnyId<Models.ContentHolder>(unnyIdContentHolder);

	}
#pragma warning restore 649
}