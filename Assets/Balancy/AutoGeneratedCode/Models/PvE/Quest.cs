using Newtonsoft.Json;
using System;

namespace Balancy.Models.PvE
{
#pragma warning disable 649

	public class Quest : BaseModel
	{

		[JsonProperty]
		private string unnyIdCompletion;
		[JsonProperty]
		private string unnyIdActivation;


		[JsonProperty("name")]
		public readonly string Name;

		[JsonIgnore]
		public Models.SmartObjects.Conditions.Logic Completion => DataEditor.GetModelByUnnyId<Models.SmartObjects.Conditions.Logic>(unnyIdCompletion);

		[JsonProperty("reward")]
		public readonly Models.SmartObjects.Reward Reward;

		[JsonProperty("icon")]
		public readonly UnnyObject Icon;

		[JsonIgnore]
		public Models.SmartObjects.Conditions.Logic Activation => DataEditor.GetModelByUnnyId<Models.SmartObjects.Conditions.Logic>(unnyIdActivation);

	}
#pragma warning restore 649
}