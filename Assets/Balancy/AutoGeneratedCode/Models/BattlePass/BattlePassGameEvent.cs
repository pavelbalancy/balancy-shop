using Newtonsoft.Json;
using System;

namespace Balancy.Models.BattlePass
{
#pragma warning disable 649

	public class BattlePassGameEvent : SmartObjects.GameEvent
	{

		[JsonProperty]
		private string unnyIdBattlePass;


		[JsonIgnore]
		public Models.BattlePass.BattlePass BattlePass => DataEditor.GetModelByUnnyId<Models.BattlePass.BattlePass>(unnyIdBattlePass);

	}
#pragma warning restore 649
}