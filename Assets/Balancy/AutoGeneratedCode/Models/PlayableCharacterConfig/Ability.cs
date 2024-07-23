using Newtonsoft.Json;
using System;
using Balancy.Localization;

namespace Balancy.Models.PlayableCharacterConfig
{
#pragma warning disable 649

	public class Ability : BaseModel
	{



		[JsonProperty("name"), JsonConverter(typeof(LocalizedStringConverter))]
		public readonly LocalizedString Name;

		[JsonProperty("description"), JsonConverter(typeof(LocalizedStringConverter))]
		public readonly LocalizedString Description;

		[JsonProperty("type")]
		public readonly Models.CharacterConfig.AbilityType Type;

	}
#pragma warning restore 649
}