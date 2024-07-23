using Newtonsoft.Json;
using System;

namespace Balancy.Models
{
#pragma warning disable 649

	public class ActivatedOffer : BaseModel
	{

		[JsonProperty]
		private string unnyIdOffer;


		[JsonIgnore]
		public Models.SmartObjects.GameOffer Offer => DataEditor.GetModelByUnnyId<Models.SmartObjects.GameOffer>(unnyIdOffer);

		[JsonProperty("wasActive")]
		public readonly bool WasActive;

	}
#pragma warning restore 649
}