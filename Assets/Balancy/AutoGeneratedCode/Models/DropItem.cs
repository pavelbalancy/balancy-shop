using Newtonsoft.Json;
using System;

namespace Balancy.Models
{
#pragma warning disable 649

	public class DropItem : BaseModel
	{



		[JsonProperty("item")]
		public readonly Models.SmartObjects.ItemWithAmount Item;

		[JsonProperty("weight")]
		public readonly int Weight;

	}
#pragma warning restore 649
}