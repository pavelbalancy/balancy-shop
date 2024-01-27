using Newtonsoft.Json;
using System;

namespace Balancy.Models.Ads.Setting
{
#pragma warning disable 649

	public class DoubleRewards : Ads.Setting.Base
	{



		[JsonProperty("minimalLevel")]
		public readonly int MinimalLevel;

		[JsonProperty("dailyLimit")]
		public readonly int DailyLimit;

	}
#pragma warning restore 649
}