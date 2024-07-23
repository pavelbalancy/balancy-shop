using Newtonsoft.Json;
using System;

namespace Balancy.Models.PlayableCharacterConfig
{
#pragma warning disable 649

	public class Character : SmartObjects.Item
	{

		[JsonProperty]
		private string unnyIdTierProgression;
		[JsonProperty]
		private string unnyIdLevelProgression;


		[JsonIgnore]
		public Models.PlayableCharacterConfig.TierProgression TierProgression => DataEditor.GetModelByUnnyId<Models.PlayableCharacterConfig.TierProgression>(unnyIdTierProgression);

		[JsonProperty("abilities")]
		public readonly Models.CharacterConfig.AbilityLevel[] Abilities;

		[JsonProperty("defaultEquip")]
		public readonly Models.SmartObjects.ItemWithAmount[] DefaultEquip;

		[JsonIgnore]
		public Models.PlayableCharacterConfig.LevelProgression LevelProgression => DataEditor.GetModelByUnnyId<Models.PlayableCharacterConfig.LevelProgression>(unnyIdLevelProgression);

		[JsonProperty("startingExp")]
		public readonly int StartingExp;

		[JsonProperty("class")]
		public readonly Models.CharacterConfig.CharacterClass Class;

		[JsonProperty("race")]
		public readonly Models.CharacterConfig.CharacterRace Race;

	}
#pragma warning restore 649
}