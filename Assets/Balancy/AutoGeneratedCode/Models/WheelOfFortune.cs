using Newtonsoft.Json;
using System;

namespace Balancy.Models
{
#pragma warning disable 649

	public class WheelOfFortune : BaseModel
	{

		[JsonProperty]
		private string unnyIdCondition;
		[JsonProperty]
		private string[] unnyIdDrop;


		[JsonProperty("name")]
		public readonly string Name;

		[JsonIgnore]
		public Models.SmartObjects.Conditions.Logic Condition => DataEditor.GetModelByUnnyId<Models.SmartObjects.Conditions.Logic>(unnyIdCondition);

		[JsonIgnore]
		public Models.DropItem[] Drop
		{
			get
			{
				if (unnyIdDrop == null)
					return new Models.DropItem[0];
				var drop = new Models.DropItem[unnyIdDrop.Length];
				for (int i = 0;i < unnyIdDrop.Length;i++)
					drop[i] = DataEditor.GetModelByUnnyId<Models.DropItem>(unnyIdDrop[i]);
				return drop;
			}
		}

		[JsonProperty("spinsPerDay")]
		public readonly int SpinsPerDay;

	}
#pragma warning restore 649
}