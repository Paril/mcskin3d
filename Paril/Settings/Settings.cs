//
//    MCSkin3D, a 3d skin management studio for Minecraft
//    Copyright (C) 2013 Altered Softworks & MCSkin3D Team
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;

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

	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
	public class TypeSerializerAttribute : Attribute
	{
		public readonly bool DeserializeDefault;
		public readonly string TypeName;

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
					var info = (PropertyInfo) field;

					if (info.GetCustomAttributes(typeof (TypeSerializerAttribute), false).Length != 0)
					{
						var converter = (TypeSerializerAttribute) info.GetCustomAttributes(typeof (TypeSerializerAttribute), false)[0];
						Type type = Type.GetType(converter.TypeName);

						if (type != null)
						{
							var conv = (ITypeSerializer) (type.GetConstructors()[0].Invoke(null));
							return conv.Serialize(obj);
						}
					}
				}

				if (obj == null)
					return "";

				return TypeDescriptor.GetConverter(obj.GetType()).ConvertToString(obj);
			}
			catch (Exception ex)
			{
				if (field == null)
					throw new Exception("Wat, field is null");
				else
				{
					throw new Exception(
						"Failed to serialize member " + ((MemberInfo) field).Name + " [" + ((MemberInfo) field).MemberType.ToString() +
						"]", ex);
				}
			}
		}

		public virtual object Deserialize(object field, string str, Type t)
		{
			if (field is PropertyInfo)
			{
				var info = (PropertyInfo) field;

				if (info.GetCustomAttributes(typeof (TypeSerializerAttribute), false).Length != 0)
				{
					var converter = (TypeSerializerAttribute) info.GetCustomAttributes(typeof (TypeSerializerAttribute), false)[0];
					Type type = Type.GetType(converter.TypeName);

					if (type != null)
					{
						var conv = (ITypeSerializer) (type.GetConstructors()[0].Invoke(null));
						return conv.Deserialize(str);
					}
				}
			}

			return TypeDescriptor.GetConverter(t).ConvertFromString(str);
		}
	}

	public class StringArraySerializer : ITypeSerializer
	{
		#region ITypeSerializer Members

		public string Serialize(object obj)
		{
			var arr = (string[]) obj;
			string combined = "";

			foreach (string c in arr)
			{
				if (combined == "")
					combined += c;
				else
					combined += ";" + c;
			}

			return combined;
		}

		public object Deserialize(string str)
		{
			return str.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);
		}

		#endregion
	}

	public class Settings
	{
		private readonly StringSerializer _serializer = new StringSerializer();
		public List<object> Structures = new List<object>();

		public void Save(string fileName)
		{
			var writer = new StreamWriter(fileName);

			foreach (object v in Structures)
			{
				if (!(v is Type))
					continue;

				var type = (Type) v;

				writer.WriteLine("[" + type.Name + "]");

				foreach (PropertyInfo prop in type.GetProperties(BindingFlags.Static | BindingFlags.Public))
				{
					if (prop.GetCustomAttributes(typeof (SavableAttribute), false).Length != 0)
					{
						string str = _serializer.Serialize(prop, prop.GetValue(null, null));
						writer.WriteLine(prop.Name + "=" + str);
					}
				}

				writer.WriteLine();
			}

			writer.Close();
		}

		public void LoadDefaults()
		{
			foreach (object str in Structures)
			{
				if (str is Type)
				{
					var type = (Type) str;

					foreach (PropertyInfo prop in type.GetProperties(BindingFlags.Static | BindingFlags.Public))
					{
						object[] attribs = prop.GetCustomAttributes(typeof (DefaultValueAttribute), false);
						if (attribs.Length != 0)
						{
							var dva = (DefaultValueAttribute) attribs[0];

							object[] converters = prop.GetCustomAttributes(typeof (TypeSerializerAttribute), false);

							if (converters.Length != 0)
							{
								var serialize = (TypeSerializerAttribute) converters[0];

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

			var reader = new StreamReader(fileName);

			object currentObject = null;
			while (!reader.EndOfStream)
			{
				string line = reader.ReadLine().Trim();

				if (string.IsNullOrEmpty(line))
					continue;

				if (line.StartsWith("[") && line.EndsWith("]"))
				{
					string header = line.Split(new[] {'[', ']'}, StringSplitOptions.RemoveEmptyEntries)[0];

					foreach (object v in Structures)
					{
						if (!(v is Type))
							continue;

						if (header == ((Type) v).Name)
						{
							currentObject = v;
							break;
						}
					}
				}
				else if (currentObject != null)
				{
					var pair = new string[2];
					int split = line.IndexOf('=');
					pair[0] = line.Substring(0, split);
					pair[1] = line.Substring(split + 1);

					if (currentObject is Type)
					{
						var type = (Type) currentObject;

						foreach (PropertyInfo prop in type.GetProperties(BindingFlags.Static | BindingFlags.Public))
						{
							if (prop.GetCustomAttributes(typeof (SavableAttribute), false).Length != 0 &&
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