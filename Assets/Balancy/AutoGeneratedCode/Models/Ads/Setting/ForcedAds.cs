using Newtonsoft.Json;
using System;

namespace Balancy.Models.Ads.Setting
{
#pragma warning disable 649

	public class ForcedAds : Ads.Setting.Base
	{



		[JsonProperty("afterLogin")]
		public readonly bool AfterLogin;

		[JsonProperty("afterWin")]
		public readonly bool AfterWin;

		[JsonProperty("afterLoose")]
		public readonly bool AfterLoose;

		[JsonProperty("dailyLimit")]
		public readonly int DailyLimit;

	}
#pragma warning restore 649
}