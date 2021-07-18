using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("cost", "conditionID", "effect1ID", "effect2ID")]
	public class ES3UserType_CardData : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_CardData() : base(typeof(Main.Data.CardData)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (Main.Data.CardData)obj;
			
			writer.WriteProperty("cost", instance.cost, ES3Type_int.Instance);
			writer.WriteProperty("conditionID", instance.conditionID, ES3Type_int.Instance);
			writer.WriteProperty("effect1ID", instance.effect1ID, ES3Type_int.Instance);
			writer.WriteProperty("effect2ID", instance.effect2ID, ES3Type_int.Instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (Main.Data.CardData)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "cost":
						instance.cost = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "conditionID":
						instance.conditionID = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "effect1ID":
						instance.effect1ID = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "effect2ID":
						instance.effect2ID = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new Main.Data.CardData();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_CardDataArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_CardDataArray() : base(typeof(Main.Data.CardData[]), ES3UserType_CardData.Instance)
		{
			Instance = this;
		}
	}
}