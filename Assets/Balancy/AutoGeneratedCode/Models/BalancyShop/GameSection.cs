using Newtonsoft.Json;
using System;
using Balancy.Localization;

namespace Balancy.Models.BalancyShop
{
#pragma warning disable 649

	public class GameSection : BaseModel
	{

		[JsonProperty]
		private string unnyIdBadge;


		[JsonProperty("icon")]
		public readonly UnnyObject Icon;

		[JsonProperty("text"), JsonConverter(typeof(LocalizedStringConverter))]
		public readonly LocalizedString Text;

		[JsonProperty("order")]
		public readonly int Order;

		[JsonProperty("defaultBack")]
		public readonly UnnyObject DefaultBack;

		[JsonProperty("selectedBack")]
		public readonly UnnyObject SelectedBack;

		[JsonProperty("type")]
		public readonly Models.GameShop.WindowType Type;

		[JsonIgnore]
		public Models.BalancyShop.Badge Badge => DataEditor.GetModelByUnnyId<Models.BalancyShop.Badge>(unnyIdBadge);

	}
#pragma warning restore 649
}