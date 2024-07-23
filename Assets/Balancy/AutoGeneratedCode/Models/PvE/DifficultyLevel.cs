using Newtonsoft.Json;
using System;
using Balancy.Localization;

namespace Balancy.Models.PvE
{
#pragma warning disable 649

	public class DifficultyLevel : BaseModel
	{



		[JsonProperty("difficulty")]
		public readonly Models.LevelDifficultyType Difficulty;

		[JsonProperty("description"), JsonConverter(typeof(LocalizedStringConverter))]
		public readonly LocalizedString Description;

		[JsonProperty("expMultiplier")]
		public readonly float ExpMultiplier;

	}
#pragma warning restore 649
}