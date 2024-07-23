using Newtonsoft.Json;
using System;

namespace Balancy.Models.NPCConfig
{
#pragma warning disable 649

	public class BossNPC : NPCConfig.RegularNPC
	{

		[JsonProperty]
		private string[] unnyIdAbilities;


		[JsonIgnore]
		public Models.PlayableCharacterConfig.Ability[] Abilities
		{
			get
			{
				if (unnyIdAbilities == null)
					return new Models.PlayableCharacterConfig.Ability[0];
				var abilities = new Models.PlayableCharacterConfig.Ability[unnyIdAbilities.Length];
				for (int i = 0;i < unnyIdAbilities.Length;i++)
					abilities[i] = DataEditor.GetModelByUnnyId<Models.PlayableCharacterConfig.Ability>(unnyIdAbilities[i]);
				return abilities;
			}
		}

	}
#pragma warning restore 649
}