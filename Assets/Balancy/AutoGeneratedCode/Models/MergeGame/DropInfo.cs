using Newtonsoft.Json;
using System;

namespace Balancy.Models.MergeGame
{
#pragma warning disable 649

	public class DropInfo : BaseModel
	{

		[JsonProperty]
		private string unnyIdItem;


		[JsonIgnore]
		public Models.SmartObjects.Item Item => DataEditor.GetModelByUnnyId<Models.SmartObjects.Item>(unnyIdItem);

		[JsonProperty("chance")]
		public readonly int Chance;

	}
#pragma warning restore 649
}