using Newtonsoft.Json;
using System;
using Balancy.Localization;

namespace Balancy.Models.BalancyShop
{
#pragma warning disable 649

	public class UIItem : BaseModel
	{



		[JsonProperty("icon")]
		public readonly UnnyObject Icon;

		[JsonProperty("background")]
		public readonly UnnyObject Background;

		[JsonProperty("asset")]
		public readonly UnnyAsset Asset;

		[JsonProperty("text"), JsonConverter(typeof(LocalizedStringConverter))]
		public readonly LocalizedString Text;

	}
#pragma warning restore 649
}