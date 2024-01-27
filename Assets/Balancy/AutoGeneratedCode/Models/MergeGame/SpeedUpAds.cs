using Newtonsoft.Json;
using System;

namespace Balancy.Models.MergeGame
{
#pragma warning disable 649

	public class SpeedUpAds : BaseModel
	{



		[JsonProperty("restore")]
		public readonly int Restore;

		[JsonProperty("limit")]
		public readonly int Limit;

	}
#pragma warning restore 649
}