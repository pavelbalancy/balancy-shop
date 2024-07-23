using Newtonsoft.Json;
using System;

namespace Balancy.Models.Equipment
{
#pragma warning disable 649

	public class Weapons : Equipment.BaseEquip
	{



		[JsonProperty("equipType")]
		public readonly Models.Equipment.EquipType EquipType;

	}
#pragma warning restore 649
}