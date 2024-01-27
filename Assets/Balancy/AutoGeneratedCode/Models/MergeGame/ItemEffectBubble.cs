using Newtonsoft.Json;
using System;

namespace Balancy.Models.MergeGame
{
#pragma warning disable 649

	public class ItemEffectBubble : MergeGame.ItemEffectBase
	{



		[JsonProperty("chance")]
		public readonly int Chance;

		[JsonProperty("price")]
		public readonly int Price;

	}
#pragma warning restore 649
}