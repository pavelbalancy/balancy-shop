using Newtonsoft.Json;
using System;

namespace Balancy.Models.Ads
{
#pragma warning disable 649

	public class Config : BaseModel
	{

		[JsonProperty]
		private string unnyIdCondition;
		[JsonProperty]
		private string[] unnyIdSettings;


		[JsonProperty("name")]
		public readonly string Name;

		[JsonProperty("priority")]
		public readonly int Priority;

		[JsonProperty("default")]
		public readonly bool Default;

		[JsonIgnore]
		public Models.SmartObjects.Conditions.Logic Condition => DataEditor.GetModelByUnnyId<Models.SmartObjects.Conditions.Logic>(unnyIdCondition);

		[JsonIgnore]
		public Models.Ads.Setting.Base[] Settings
		{
			get
			{
				if (unnyIdSettings == null)
					return new Models.Ads.Setting.Base[0];
				var settings = new Models.Ads.Setting.Base[unnyIdSettings.Length];
				for (int i = 0;i < unnyIdSettings.Length;i++)
					settings[i] = DataEditor.GetModelByUnnyId<Models.Ads.Setting.Base>(unnyIdSettings[i]);
				return settings;
			}
		}

	}
#pragma warning restore 649
}