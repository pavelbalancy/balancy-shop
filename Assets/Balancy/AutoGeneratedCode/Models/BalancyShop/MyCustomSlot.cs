using Newtonsoft.Json;
using System;

namespace Balancy.Models.BalancyShop
{
#pragma warning disable 649

	public class MyCustomSlot : LiveOps.Store.Slot
	{

		[JsonProperty]
		private string unnyIdUIData;


		[JsonIgnore]
		public Models.BalancyShop.UIStoreItem UIData => DataEditor.GetModelByUnnyId<Models.BalancyShop.UIStoreItem>(unnyIdUIData);

	}
#pragma warning restore 649
}