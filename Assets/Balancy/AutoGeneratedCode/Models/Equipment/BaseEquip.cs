using Newtonsoft.Json;
using System;
using Balancy.Localization;

namespace Balancy.Models.Equipment
{
#pragma warning disable 649

	public abstract class BaseEquip : SmartObjects.Item
	{



		[JsonProperty("icon")]
		public readonly UnnyObject Icon;

		[JsonProperty("asset")]
		public readonly UnnyAsset Asset;

		[JsonProperty("level")]
		public readonly int Level;

		[JsonProperty("description"), JsonConverter(typeof(LocalizedStringConverter))]
		public readonly LocalizedString Description;

		[JsonProperty("tier")]
		public readonly int Tier;

	}
#pragma warning restore 649
}