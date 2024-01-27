using Newtonsoft.Json;
using System;

namespace Balancy.Models.MergeGame
{
#pragma warning disable 649

	public class GameItem : SmartObjects.Item
	{

		[JsonProperty]
		private string[] unnyIdItemEffects;
		[JsonProperty]
		private string unnyIdMergeItem;


		[JsonProperty("utilizationPrice")]
		public readonly int UtilizationPrice;

		[JsonProperty("level")]
		public readonly int Level;

		[JsonIgnore]
		public Models.MergeGame.ItemEffectBase[] ItemEffects
		{
			get
			{
				if (unnyIdItemEffects == null)
					return new Models.MergeGame.ItemEffectBase[0];
				var itemEffects = new Models.MergeGame.ItemEffectBase[unnyIdItemEffects.Length];
				for (int i = 0;i < unnyIdItemEffects.Length;i++)
					itemEffects[i] = DataEditor.GetModelByUnnyId<Models.MergeGame.ItemEffectBase>(unnyIdItemEffects[i]);
				return itemEffects;
			}
		}

		[JsonIgnore]
		public Models.SmartObjects.Item MergeItem => DataEditor.GetModelByUnnyId<Models.SmartObjects.Item>(unnyIdMergeItem);

	}
#pragma warning restore 649
}