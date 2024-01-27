using Newtonsoft.Json;
using System;

namespace Balancy.Models.MergeGame
{
#pragma warning disable 649

	public class Spawner : MergeGame.GameItem
	{



		[JsonProperty("energy")]
		public readonly Models.MergeGame.SpawnerEnergy Energy;

		[JsonProperty("speedUp")]
		public readonly Models.MergeGame.SpeedUp SpeedUp;

		[JsonProperty("ads")]
		public readonly Models.MergeGame.SpeedUpAds Ads;

		[JsonProperty("drop")]
		public readonly Models.MergeGame.DropInfo[] Drop;

	}
#pragma warning restore 649
}