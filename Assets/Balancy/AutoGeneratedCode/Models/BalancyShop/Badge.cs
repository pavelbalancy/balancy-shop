using Newtonsoft.Json;
using System;
using Balancy.Localization;

namespace Balancy.Models.BalancyShop
{
#pragma warning disable 649

	public class Badge : BaseModel
	{



		[JsonProperty("back")]
		public readonly UnnyObject Back;

		[JsonProperty("text"), JsonConverter(typeof(LocalizedStringConverter))]
		public readonly LocalizedString Text;

	}
#pragma warning restore 649
}