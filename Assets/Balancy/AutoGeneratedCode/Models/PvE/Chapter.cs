using Newtonsoft.Json;
using System;
using Balancy.Localization;

namespace Balancy.Models.PvE
{
#pragma warning disable 649

	public class Chapter : BaseModel
	{

		[JsonProperty]
		private string[] unnyIdMissions;
		[JsonProperty]
		private string unnyIdCondition;


		[JsonProperty("description"), JsonConverter(typeof(LocalizedStringConverter))]
		public readonly LocalizedString Description;

		[JsonProperty("name"), JsonConverter(typeof(LocalizedStringConverter))]
		public readonly LocalizedString Name;

		[JsonIgnore]
		public Models.PvE.Mission[] Missions
		{
			get
			{
				if (unnyIdMissions == null)
					return new Models.PvE.Mission[0];
				var missions = new Models.PvE.Mission[unnyIdMissions.Length];
				for (int i = 0;i < unnyIdMissions.Length;i++)
					missions[i] = DataEditor.GetModelByUnnyId<Models.PvE.Mission>(unnyIdMissions[i]);
				return missions;
			}
		}

		[JsonProperty("unlocked")]
		public readonly bool Unlocked;

		[JsonIgnore]
		public Models.SmartObjects.Conditions.Logic Condition => DataEditor.GetModelByUnnyId<Models.SmartObjects.Conditions.Logic>(unnyIdCondition);

	}
#pragma warning restore 649
}