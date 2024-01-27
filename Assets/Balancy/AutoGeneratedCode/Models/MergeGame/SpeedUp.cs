using Newtonsoft.Json;
using System;

namespace Balancy.Models.MergeGame
{
#pragma warning disable 649

	public class SpeedUp : BaseModel
	{



		[JsonProperty("restore")]
		public readonly int Restore;

		[JsonProperty("price")]
		public readonly int Price;

	}
#pragma warning restore 649
}