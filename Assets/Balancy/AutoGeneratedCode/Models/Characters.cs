using Newtonsoft.Json;
using System;

namespace Balancy.Data
{
#pragma warning disable 649

	public class Characters : BaseData
	{

		[JsonProperty, JsonConverter(typeof(SmartListConverter<Data.CharacterData>))]
		private SmartList<Data.CharacterData> collection;


		[JsonIgnore]
		public SmartList<Data.CharacterData> Collection => collection;

		protected override void InitParams() {
			base.InitParams();

			ValidateData(ref collection);
		}

		public static Characters Instantiate()
		{
			return Instantiate<Characters>();
		}

		protected override void AddAllParamsToCache(string path, IInternalStorageCache cache)
		{
			base.AddAllParamsToCache(path, cache);
			AddCachedItem(path + "Collection", Collection, null, cache);
		}
	}
#pragma warning restore 649
}