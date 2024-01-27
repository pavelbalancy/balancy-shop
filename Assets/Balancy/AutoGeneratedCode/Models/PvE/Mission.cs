using Newtonsoft.Json;
using System;
using Balancy.Localization;

namespace Balancy.Models.PvE
{
#pragma warning disable 649

	public class Mission : BaseModel
	{

		[JsonProperty]
		private string unnyIdDifficuly;


		[JsonProperty("name"), JsonConverter(typeof(LocalizedStringConverter))]
		public readonly LocalizedString Name;

		[JsonProperty("description"), JsonConverter(typeof(LocalizedStringConverter))]
		public readonly LocalizedString Description;

		[JsonIgnore]
		public Models.PvE.DifficultyLevel Difficuly => DataEditor.GetModelByUnnyId<Models.PvE.DifficultyLevel>(unnyIdDifficuly);

		[JsonProperty("reward")]
		public readonly Models.SmartObjects.Reward[] Reward;

		[JsonProperty("unlocked")]
		public readonly bool Unlocked;

	}
#pragma warning restore 649
}