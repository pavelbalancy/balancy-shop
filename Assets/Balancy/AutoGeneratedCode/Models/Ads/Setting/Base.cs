using Newtonsoft.Json;
using System;

namespace Balancy.Models.Ads.Setting
{
#pragma warning disable 649

	public abstract class Base : BaseModel
	{



		[JsonProperty("enabled")]
		public readonly bool Enabled;

	}
#pragma warning restore 649
}