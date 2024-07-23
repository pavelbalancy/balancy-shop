using Newtonsoft.Json;
using System;

namespace Balancy.Models.CharacterConfig
{
#pragma warning disable 649

	public class LevelExp : BaseModel
	{



		[JsonProperty("level")]
		public readonly int Level;

		[JsonProperty("exp")]
		public readonly int Exp;

	}
#pragma warning restore 649
}