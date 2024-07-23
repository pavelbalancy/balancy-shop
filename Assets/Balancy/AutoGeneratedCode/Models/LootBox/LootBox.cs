using Newtonsoft.Json;
using System;

namespace Balancy.Models.LootBox
{
#pragma warning disable 649

	public class LootBox : SmartObjects.Item
	{

		[JsonProperty]
		private string[] unnyIdLoot;
		[JsonProperty]
		private string unnyIdGuaranteedDrop;
		[JsonProperty]
		private string unnyIdRewardScript;


		[JsonProperty("cooldown")]
		public readonly int Cooldown;

		[JsonProperty("price")]
		public readonly Models.SmartObjects.Price Price;

		[JsonProperty("logicType")]
		public readonly Models.LootBox.RandomiserType LogicType;

		[JsonIgnore]
		public Models.LootBox.DropItem[] Loot
		{
			get
			{
				if (unnyIdLoot == null)
					return new Models.LootBox.DropItem[0];
				var loot = new Models.LootBox.DropItem[unnyIdLoot.Length];
				for (int i = 0;i < unnyIdLoot.Length;i++)
					loot[i] = DataEditor.GetModelByUnnyId<Models.LootBox.DropItem>(unnyIdLoot[i]);
				return loot;
			}
		}

		[JsonIgnore]
		public Models.LootBox.DropItem GuaranteedDrop => DataEditor.GetModelByUnnyId<Models.LootBox.DropItem>(unnyIdGuaranteedDrop);

		[JsonProperty("isAutoOpen")]
		public readonly bool IsAutoOpen;

		[JsonIgnore]
		public VisualScripting.ScriptNode RewardScript => Balancy.LiveOps.General.GetScriptById(unnyIdRewardScript);

	}
#pragma warning restore 649
}