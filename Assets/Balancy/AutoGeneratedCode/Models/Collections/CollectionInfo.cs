using Newtonsoft.Json;
using System;
using Balancy.Localization;

namespace Balancy.Models.Collections
{
#pragma warning disable 649

	public class CollectionInfo : BaseModel
	{

		[JsonProperty]
		private string[] unnyIdItems;


		[JsonProperty("reward")]
		public readonly Models.SmartObjects.ItemWithAmount[] Reward;

		[JsonProperty("icon")]
		public readonly UnnyObject Icon;

		[JsonProperty("name"), JsonConverter(typeof(LocalizedStringConverter))]
		public readonly LocalizedString Name;

		[JsonProperty("description"), JsonConverter(typeof(LocalizedStringConverter))]
		public readonly LocalizedString Description;

		[JsonIgnore]
		public Models.Collections.CollectionItem[] Items
		{
			get
			{
				if (unnyIdItems == null)
					return new Models.Collections.CollectionItem[0];
				var items = new Models.Collections.CollectionItem[unnyIdItems.Length];
				for (int i = 0;i < unnyIdItems.Length;i++)
					items[i] = DataEditor.GetModelByUnnyId<Models.Collections.CollectionItem>(unnyIdItems[i]);
				return items;
			}
		}

	}
#pragma warning restore 649
}