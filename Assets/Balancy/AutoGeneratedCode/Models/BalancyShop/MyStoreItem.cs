using Newtonsoft.Json;
using System;
using Balancy.Localization;

namespace Balancy.Models.BalancyShop
{
#pragma warning disable 649

	public class MyStoreItem : SmartObjects.StoreItem
	{



		[JsonProperty("description"), JsonConverter(typeof(LocalizedStringConverter))]
		public readonly LocalizedString Description;

	}
#pragma warning restore 649
}