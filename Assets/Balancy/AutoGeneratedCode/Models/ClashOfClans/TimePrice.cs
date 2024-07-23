using Newtonsoft.Json;
using System;

namespace Balancy.Models.ClashOfClans
{
#pragma warning disable 649

	public class TimePrice : BaseModel
	{



		[JsonProperty("time")]
		public readonly int Time;

		[JsonProperty("gems")]
		public readonly int Gems;

	}
#pragma warning restore 649
}