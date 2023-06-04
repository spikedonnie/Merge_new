using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("type", "buffCount", "buffDuration", "isActive")]
	public class ES3UserType_BuffData : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_BuffData() : base(typeof(BuffData)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (BuffData)obj;
			
			writer.WriteProperty("type", instance.type, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(BuffType)));
			writer.WriteProperty("buffCount", instance.buffCount, ES3Type_int.Instance);
			writer.WriteProperty("buffDuration", instance.buffDuration, ES3Type_float.Instance);
			writer.WriteProperty("isActive", instance.isActive, ES3Type_bool.Instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (BuffData)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "type":
						instance.type = reader.Read<BuffType>();
						break;
					case "buffCount":
						instance.buffCount = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "buffDuration":
						instance.buffDuration = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "isActive":
						instance.isActive = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new BuffData();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_BuffDataArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_BuffDataArray() : base(typeof(BuffData[]), ES3UserType_BuffData.Instance)
		{
			Instance = this;
		}
	}
}