using Newtonsoft.Json;
using System;

namespace Balancy.Models.PlayableCharacterConfig
{
#pragma warning disable 649

	public class LevelProgression : BaseModel
	{



		[JsonProperty("name")]
		public readonly string Name;

		[JsonProperty("levels")]
		public readonly Models.CharacterConfig.LevelExp[] Levels;

	}
#pragma warning restore 649
}