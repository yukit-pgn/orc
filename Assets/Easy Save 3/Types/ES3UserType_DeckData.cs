using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("name", "cardList")]
	public class ES3UserType_DeckData : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_DeckData() : base(typeof(Main.Data.DeckData)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (Main.Data.DeckData)obj;
			
			writer.WriteProperty("name", instance.name, ES3Type_string.Instance);
			writer.WriteProperty("cardList", instance.cardList);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (Main.Data.DeckData)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "name":
						instance.name = reader.Read<System.String>(ES3Type_string.Instance);
						break;
					case "cardList":
						instance.cardList = reader.Read<System.Collections.Generic.List<Main.Data.CardData>>();
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new Main.Data.DeckData();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_DeckDataArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_DeckDataArray() : base(typeof(Main.Data.DeckData[]), ES3UserType_DeckData.Instance)
		{
			Instance = this;
		}
	}
}