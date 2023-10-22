using Newtonsoft.Json;
using System;

namespace Balancy.Models.BalancyShop
{
#pragma warning disable 649

	public class MyItem : SmartObjects.Item
	{



		[JsonProperty("icon")]
		public readonly UnnyObject Icon;

	}
#pragma warning restore 649
}