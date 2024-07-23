using Newtonsoft.Json;
using System;

namespace Balancy.Models.CharacterConfig
{
#pragma warning disable 649

	public class TierLevel : BaseModel
	{



		[JsonProperty("tier")]
		public readonly int Tier;

		[JsonProperty("level")]
		public readonly int Level;

	}
#pragma warning restore 649
}