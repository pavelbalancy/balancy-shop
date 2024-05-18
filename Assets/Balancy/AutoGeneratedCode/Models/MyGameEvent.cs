using Newtonsoft.Json;
using System;

namespace Balancy.Models
{
#pragma warning disable 649

	public class MyGameEvent : SmartObjects.GameEvent
	{



		[JsonProperty("icon")]
		public readonly UnnyObject Icon;

	}
#pragma warning restore 649
}