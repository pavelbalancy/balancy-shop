using Newtonsoft.Json;
using System;
using Balancy.Localization;

namespace Balancy.Models.Collections
{
#pragma warning disable 649

	public class CollectionItem : BalancyShop.MyItem
	{



		[JsonProperty("description"), JsonConverter(typeof(LocalizedStringConverter))]
		public readonly LocalizedString Description;

	}
#pragma warning restore 649
}