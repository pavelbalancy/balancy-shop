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

		[JsonProperty("price")]
		public readonly Models.SmartObjects.Price Price;

		[JsonProperty("reward")]
		public readonly Models.SmartObjects.Reward Reward;

		[JsonProperty("levelType")]
		public readonly Models.PvE.LevelType LevelType;

		[JsonProperty("epicReward")]
		public readonly Models.SmartObjects.Reward EpicReward;

	}
#pragma warning restore 649
}