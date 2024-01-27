using Newtonsoft.Json;
using System;

namespace Balancy.Models
{
#pragma warning disable 649

	public class BattlePass_GameEvent : SmartObjects.GameEvent
	{

		[JsonProperty]
		private string unnyIdBattlePass;


		[JsonIgnore]
		public Models.BattlePass BattlePass => DataEditor.GetModelByUnnyId<Models.BattlePass>(unnyIdBattlePass);

	}
#pragma warning restore 649
}