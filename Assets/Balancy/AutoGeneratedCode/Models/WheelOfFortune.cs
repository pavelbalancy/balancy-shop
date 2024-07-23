using Newtonsoft.Json;
using System;

namespace Balancy.Models
{
#pragma warning disable 649

	public class WheelOfFortune : BaseModel
	{

		[JsonProperty]
		private string unnyIdCondition;


		[JsonProperty("name")]
		public readonly string Name;

		[JsonIgnore]
		public Models.SmartObjects.Conditions.Logic Condition => DataEditor.GetModelByUnnyId<Models.SmartObjects.Conditions.Logic>(unnyIdCondition);

		[JsonProperty("spinsPerDay")]
		public readonly int SpinsPerDay;

	}
#pragma warning restore 649
}