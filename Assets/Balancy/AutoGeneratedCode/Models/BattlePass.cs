using Newtonsoft.Json;
using System;

namespace Balancy.Models
{
#pragma warning disable 649

	public class BattlePass : BaseModel
	{

		[JsonProperty]
		private string unnyIdCondition;
		[JsonProperty]
		private string[] unnyIdPoints;


		[JsonProperty("name")]
		public readonly string Name;

		[JsonIgnore]
		public Models.SmartObjects.Conditions.Logic Condition => DataEditor.GetModelByUnnyId<Models.SmartObjects.Conditions.Logic>(unnyIdCondition);

		[JsonIgnore]
		public Models.BattlePassPoint[] Points
		{
			get
			{
				if (unnyIdPoints == null)
					return new Models.BattlePassPoint[0];
				var points = new Models.BattlePassPoint[unnyIdPoints.Length];
				for (int i = 0;i < unnyIdPoints.Length;i++)
					points[i] = DataEditor.GetModelByUnnyId<Models.BattlePassPoint>(unnyIdPoints[i]);
				return points;
			}
		}

	}
#pragma warning restore 649
}