//
//    MCSkin3D, a 3d skin management studio for Minecraft
//    Copyright (C) 2011-2012 Altered Softworks & MCSkin3D Team
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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Reflection;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Drawing;
using System.IO;

namespace MCSkin3D.Language
{
	class LanguageControlLink
	{
		public bool TextNamesSet;
		public string[] TextNames;
		public object Object;
		public PropertyInfo[] PropertyNames;
		public string OriginalString;

		public LanguageControlLink(string names, object obj)
		{
			TextNamesSet = false;
			Object = obj;
			OriginalString = names;

			if (string.IsNullOrEmpty(OriginalString))
				return;

			var propNames = OriginalString.Split(';');

			PropertyNames = new PropertyInfo[propNames.Length];
			TextNames = new string[propNames.Length];

			for (int i = 0; i < propNames.Length; ++i)
			{
				PropertyNames[i] = Object.GetType().GetProperty(propNames[i]);

				if (PropertyNames[i] == null)
					throw new Exception("Property \"" + propNames[i] + "\" not found!");
				if (PropertyNames[i].PropertyType != typeof(string))
					throw new Exception("Property \"" + propNames[i] + "\" is not a string!");
			}
		}

		public void SetTextNames()
		{
			for (int i = 0; i < PropertyNames.Length; ++i)
				TextNames[i] = (string)PropertyNames[i].GetValue(Object, null);

			TextNamesSet = true;
		}
	}

	// This UITypeEditor can be associated with Int32, Double and Single
	// properties to provide a design-mode angle selection interface.
	[System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
	public class LanguageEditor : System.Drawing.Design.UITypeEditor
	{
		public LanguageEditor()
		{
		}

		// Indicates whether the UITypeEditor provides a form-based (modal) dialog, 
		// drop down dialog, or no UI outside of the properties window.
		public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.DropDown;
		}

		// Displays the UI for value selection.
		public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, object value)
		{
			// Return the value if the value is not of type Int32, Double and Single.
			//if (value.GetType() != typeof(double) && value.GetType() != typeof(float) && value.GetType() != typeof(int))
			//	return value;

			// Uses the IWindowsFormsEditorService to display a 
			// drop-down UI in the Properties window.
			IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
			if (edSvc != null)
			{
				// Display an angle selection control and retrieve the value.
				var dropDown = new LanguagePropertyChecker(context.Instance, (string)value);
				edSvc.DropDownControl(dropDown);

				return dropDown.FinalValues;
			}
			return value;
		}

		// Indicates whether the UITypeEditor supports painting a 
		// representation of a property's value.
		public override bool GetPaintValueSupported(System.ComponentModel.ITypeDescriptorContext context)
		{
			return false;
		}
	}

	// Provides a user interface for adjusting an angle value.
	internal class LanguagePropertyChecker : System.Windows.Forms.UserControl
	{
		List<CheckBox> _boxes = new List<CheckBox>();

		public LanguagePropertyChecker(object obj, string currentValues)
		{
			var vals = currentValues.Split(';');
			int y = 0;
			List<PropertyInfo> props = new List<PropertyInfo>();

			foreach (var prop in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
			{
				if (prop.PropertyType != typeof(string))
					continue;

				props.Add(prop);
			}

			AutoScroll = true;

			for (int i = 0; i < props.Count; ++i)
			{
				CheckBox box = new CheckBox();
				box.Text = props[i].Name;
				box.Location = new Point(4, y);
				box.AutoSize = true;

				if (vals.Contains(props[i].Name))
					box.Checked = true;

				y += 18;

				Controls.Add(box);

				_boxes.Add(box);
			}
		}

		public string FinalValues
		{
			get
			{
				string s = "";

				foreach (var box in _boxes)
				{
					if (box.Checked)
					{
						if (!string.IsNullOrEmpty(s))
							s += ";" + box.Text;
						else
							s = box.Text;
					}
				}

				return s;
			}
		}
	}

	/// <summary>
	/// Class that provides language services to Form controls.
	/// </summary>
	[ProvideProperty("PropertyNames", typeof(object))]
	public class LanguageProvider : Component, IExtenderProvider
	{
		Dictionary<object, LanguageControlLink> _properties = new Dictionary<object, LanguageControlLink>();

		public bool CanExtend(object extendee)
		{
			if (extendee is LanguageProvider)
				return false;

			if (extendee is Control || extendee is Component)
				return true;

			return false;
		}

		public void LanguageChanged(Language lang)
		{
			foreach (var obj in _properties)
			{
				if (!obj.Value.TextNamesSet)
					obj.Value.SetTextNames();

				for (int i = 0; i < obj.Value.PropertyNames.Length; ++i)
				{
					var strn = obj.Value.TextNames[i];

					if (lang.StringTable.ContainsKey(strn))
						obj.Value.PropertyNames[i].SetValue(obj.Value.Object, lang.StringTable[strn], null);
				}
			}
		}

		public void SetPropertyNames(object control, string names)
		{
			if (!_properties.ContainsKey(control))
				_properties.Add(control, new LanguageControlLink(names, control));
			else
				_properties[control] = new LanguageControlLink(names, control);
		}

		[DefaultValue("")]
		[Editor(typeof(LanguageEditor), typeof(UITypeEditor))]
		public string GetPropertyNames(object control)
		{
			if (!_properties.ContainsKey(control))
				return string.Empty;

			return _properties[control].OriginalString;
		}
	}
}
