using Newtonsoft.Json;
using System;
using Balancy.Localization;

namespace Balancy.Models.BalancyShop
{
#pragma warning disable 649

	public class BadgeInfo : BaseModel
	{



		[JsonProperty("type")]
		public readonly Models.LiveOps.Store.SlotType Type;

		[JsonProperty("text"), JsonConverter(typeof(LocalizedStringConverter))]
		public readonly LocalizedString Text;

		[JsonProperty("shopButtonIcon")]
		public readonly UnnyObject ShopButtonIcon;

	}
#pragma warning restore 649
}