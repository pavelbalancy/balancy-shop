using Newtonsoft.Json;
using System;
using Balancy.Localization;

namespace Balancy.Models.NPCConfig
{
#pragma warning disable 649

	public abstract class BaseNPC : BaseModel
	{



		[JsonProperty("name"), JsonConverter(typeof(LocalizedStringConverter))]
		public readonly LocalizedString Name;

		[JsonProperty("description")]
		public readonly string Description;

		[JsonProperty("level")]
		public readonly int Level;

		[JsonProperty("difficulty")]
		public readonly Models.LevelDifficultyType Difficulty;

	}
#pragma warning restore 649
}