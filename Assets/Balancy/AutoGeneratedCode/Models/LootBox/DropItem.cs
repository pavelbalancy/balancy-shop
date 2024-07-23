using Newtonsoft.Json;
using System;

namespace Balancy.Models.LootBox
{
#pragma warning disable 649

	public class DropItem : BaseModel
	{



		[JsonProperty("weight")]
		public readonly int Weight;

		[JsonProperty("item")]
		public readonly Models.SmartObjects.ItemWithAmount Item;

	}
#pragma warning restore 649
}