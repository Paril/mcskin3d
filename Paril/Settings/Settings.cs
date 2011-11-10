using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.ComponentModel;

namespace Paril.Settings
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	public sealed class SavableAttribute : Attribute
	{
	}

	public interface ISavable
	{
		string SaveHeader { get; }
	}

	public interface ITypeSerializer
	{
		string Serialize(object obj);
		object Deserialize(string str);
	}

	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited=true, AllowMultiple=false)]
	public class TypeSerializerAttribute : Attribute
	{
		public readonly string TypeName;
		public readonly bool DeserializeDefault;

		public TypeSerializerAttribute(Type type, bool deserializeDefault)
		{
			TypeName = type.FullName;
			DeserializeDefault = deserializeDefault;
		}
	}

	public class StringSerializer
	{
		public virtual string Serialize(object field, object obj)
		{
			try
			{
				if (field is PropertyInfo)
				{
					PropertyInfo info = (PropertyInfo)field;

					if (info.GetCustomAttributes(typeof(TypeSerializerAttribute), false).Length != 0)
					{
						TypeSerializerAttribute converter = (TypeSerializerAttribute)info.GetCustomAttributes(typeof(TypeSerializerAttribute), false)[0];
						var type = Type.GetType(converter.TypeName);

						if (type != null)
						{
							ITypeSerializer conv = (ITypeSerializer)(type.GetConstructors()[0].Invoke(null));
							return conv.Serialize(obj);
						}
					}
				}

				return TypeDescriptor.GetConverter(obj.GetType()).ConvertToString(obj);
			}
			catch (Exception ex)
			{
				if (field == null)
					throw new Exception("Wat, field is null");
				else
					throw new Exception("Failed to serialize member " + ((MemberInfo)field).Name + " [" + ((MemberInfo)field).MemberType.ToString() + "]", ex);
			}
		}

		public virtual object Deserialize(object field, string str, Type t)
		{
			if (field is PropertyInfo)
			{
				PropertyInfo info = (PropertyInfo)field;

				if (info.GetCustomAttributes(typeof(TypeSerializerAttribute), false).Length != 0)
				{
					TypeSerializerAttribute converter = (TypeSerializerAttribute)info.GetCustomAttributes(typeof(TypeSerializerAttribute), false)[0];
					var type = Type.GetType(converter.TypeName);

					if (type != null)
					{
						ITypeSerializer conv = (ITypeSerializer)(type.GetConstructors()[0].Invoke(null));
						return conv.Deserialize(str);
					}
				}
			}

			return TypeDescriptor.GetConverter(t).ConvertFromString(str);
		}
	}

	public class Settings
	{
		public List<object> Structures = new List<object>();
		StringSerializer _serializer = new StringSerializer();

		public void Save(string fileName)
		{
			StreamWriter writer = new StreamWriter(fileName);

			foreach (var v in Structures)
			{
				if (!(v is Type))
					continue;

				Type type = (Type)v;

				writer.WriteLine("[" + type.Name + "]");

				foreach (var prop in type.GetProperties(BindingFlags.Static | BindingFlags.Public))
				{
					if (prop.GetCustomAttributes(typeof(SavableAttribute), false).Length != 0)
					{
						var str = _serializer.Serialize(prop, prop.GetValue(null, null));
						writer.WriteLine(prop.Name + "=" + str);
					}
				}

				writer.WriteLine();
			}

			writer.Close();
		}

		public void LoadDefaults()
		{
			foreach (var str in Structures)
			{
				if (str is Type)
				{
					Type type = (Type)str;

					foreach (var prop in type.GetProperties(BindingFlags.Static | BindingFlags.Public))
					{
						var attribs = prop.GetCustomAttributes(typeof(DefaultValueAttribute), false);
						if (attribs.Length != 0)
						{
							DefaultValueAttribute dva = (DefaultValueAttribute)attribs[0];

							var converters = prop.GetCustomAttributes(typeof(TypeSerializerAttribute), false);

							if (converters.Length != 0)
							{
								TypeSerializerAttribute serialize = (TypeSerializerAttribute)converters[0];

								if (serialize.DeserializeDefault)
									prop.SetValue(null, _serializer.Deserialize(prop, dva.Value.ToString(), prop.PropertyType), null);
								else
									prop.SetValue(null, dva.Value, null);
							}
							else
								prop.SetValue(null, dva.Value, null);
						}
					}
				}
			}
		}

		public void Load(string fileName)
		{
			LoadDefaults();

			if (!File.Exists(fileName))
			{
				Save(fileName);
				return;
			}

			StreamReader reader = new StreamReader(fileName);

			object currentObject = null;
			while (!reader.EndOfStream)
			{
				var line = reader.ReadLine().Trim();

				if (string.IsNullOrEmpty(line))
					continue;

				if (line.StartsWith("[") && line.EndsWith("]"))
				{
					var header = line.Split(new char[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries)[0];

					foreach (var v in Structures)
					{
						if (!(v is Type))
							continue;

						if (header == ((Type)v).Name)
						{
							currentObject = (Type)v;
							break;
						}
					}
				}
				else if (currentObject != null)
				{
					string[] pair = new string[2];
					int split = line.IndexOf('=');
					pair[0] = line.Substring(0, split);
					pair[1] = line.Substring(split + 1);

					if (currentObject is Type)
					{
						Type type = (Type)currentObject;

						foreach (var prop in type.GetProperties(BindingFlags.Static | BindingFlags.Public))
						{
							if (prop.GetCustomAttributes(typeof(SavableAttribute), false).Length != 0 &&
								prop.Name == pair[0])
							{
								object val = _serializer.Deserialize(prop, pair[1], prop.PropertyType);
								prop.SetValue(null, val, null);
							}
						}
					}
				}
			}

			reader.Close();
		}
	}
}
