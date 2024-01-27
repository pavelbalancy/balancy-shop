using Newtonsoft.Json;
using System;

namespace Balancy.Models.Ads.Setting
{
#pragma warning disable 649

	public class DailyBonus : Ads.Setting.Base
	{



		[JsonProperty("multiplier")]
		public readonly int Multiplier;

	}
#pragma warning restore 649
}