using Newtonsoft.Json;
using System;

namespace Balancy.Models.BalancyShop
{
#pragma warning disable 649

	public class MyOffer : SmartObjects.GameOffer
	{

		[JsonProperty]
		private string unnyIdUIStoreSlotData;


		[JsonIgnore]
		public Models.BalancyShop.UIStoreItem UIStoreSlotData => DataEditor.GetModelByUnnyId<Models.BalancyShop.UIStoreItem>(unnyIdUIStoreSlotData);

	}
#pragma warning restore 649
}