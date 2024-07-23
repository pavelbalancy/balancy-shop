using Newtonsoft.Json;
using System;

namespace Balancy.Models.CharacterConfig
{
#pragma warning disable 649

	public class AbilityLevel : BaseModel
	{

		[JsonProperty]
		private string unnyIdAbility;


		[JsonIgnore]
		public Models.PlayableCharacterConfig.Ability Ability => DataEditor.GetModelByUnnyId<Models.PlayableCharacterConfig.Ability>(unnyIdAbility);

		[JsonProperty("level")]
		public readonly int Level;

	}
#pragma warning restore 649
}