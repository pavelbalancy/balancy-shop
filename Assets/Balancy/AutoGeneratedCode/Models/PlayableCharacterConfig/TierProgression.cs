using Newtonsoft.Json;
using System;

namespace Balancy.Models.PlayableCharacterConfig
{
#pragma warning disable 649

	public class TierProgression : BaseModel
	{

		[JsonProperty]
		private string[] unnyIdTiers;


		[JsonProperty("name")]
		public readonly string Name;

		[JsonIgnore]
		public Models.CharacterConfig.TierLevel[] Tiers
		{
			get
			{
				if (unnyIdTiers == null)
					return new Models.CharacterConfig.TierLevel[0];
				var tiers = new Models.CharacterConfig.TierLevel[unnyIdTiers.Length];
				for (int i = 0;i < unnyIdTiers.Length;i++)
					tiers[i] = DataEditor.GetModelByUnnyId<Models.CharacterConfig.TierLevel>(unnyIdTiers[i]);
				return tiers;
			}
		}

	}
#pragma warning restore 649
}