using Newtonsoft.Json;
using System;

namespace Balancy.Models.MergeGame
{
#pragma warning disable 649

	public class SpawnerEnergy : BaseModel
	{



		[JsonProperty("start")]
		public readonly int Start;

		[JsonProperty("max")]
		public readonly int Max;

		[JsonProperty("restore")]
		public readonly int Restore;

		[JsonProperty("time")]
		public readonly int Time;

	}
#pragma warning restore 649
}