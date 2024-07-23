using Newtonsoft.Json;
using System;

namespace Balancy.Models.Ads.Setting
{
#pragma warning disable 649

	public class ForcedAds : Ads.Setting.Base
	{



		[JsonProperty("dailyLimit")]
		public readonly int DailyLimit;

		[JsonProperty("afterLogin")]
		public readonly bool AfterLogin;

		[JsonProperty("afterWin")]
		public readonly bool AfterWin;

		[JsonProperty("afterLoose")]
		public readonly bool AfterLoose;

	}
#pragma warning restore 649
}