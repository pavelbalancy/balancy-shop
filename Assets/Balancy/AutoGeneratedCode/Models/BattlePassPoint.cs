using Newtonsoft.Json;
using System;

namespace Balancy.Models
{
#pragma warning disable 649

	public class BattlePassPoint : BaseModel
	{



		[JsonProperty("scores")]
		public readonly int Scores;

		[JsonProperty("freeReward")]
		public readonly Models.SmartObjects.Reward FreeReward;

		[JsonProperty("premiumReward")]
		public readonly Models.SmartObjects.Reward PremiumReward;

	}
#pragma warning restore 649
}