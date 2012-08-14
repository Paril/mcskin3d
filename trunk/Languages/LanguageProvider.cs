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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.ComponentModel.Design.Serialization;
using System.ComponentModel.Design;

namespace MCSkin3D.Languages
{
	internal class LanguageControlLink
	{
		public object Object;
		public string OriginalString;
		public PropertyInfo[] PropertyNames;
		public string[] TextNames;
		public bool TextNamesSet;

		public LanguageControlLink(string names, object obj, bool design = false)
		{
			try
			{
				TextNamesSet = false;
				Object = obj;
				OriginalString = names;

				if (string.IsNullOrEmpty(OriginalString))
					return;

				string[] propNames = OriginalString.Split(';');

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
			catch (Exception ex)
			{
				throw new Exception("Something happened for " + ((Control)obj).Name, ex);
			}
		}

		public void SetTextNames()
		{
			for (int i = 0; i < PropertyNames.Length; ++i)
			{
				TextNames[i] = (string)PropertyNames[i].GetValue(Object, null);

				foreach (var c in TextNames[i])
					if (!char.IsUpper(c) && !char.IsPunctuation(c) && !char.IsNumber(c))
						throw new Exception();
			}

			TextNamesSet = true;
		}
	}

	// This UITypeEditor can be associated with Int32, Double and Single
	// properties to provide a design-mode angle selection interface.
	[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
	public class LanguageEditor : UITypeEditor
	{
		// Indicates whether the UITypeEditor provides a form-based (modal) dialog, 
		// drop down dialog, or no UI outside of the properties window.
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.DropDown;
		}

		// Displays the UI for value selection.
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			// Return the value if the value is not of type Int32, Double and Single.
			//if (value.GetType() != typeof(double) && value.GetType() != typeof(float) && value.GetType() != typeof(int))
			//	return value;

			// Uses the IWindowsFormsEditorService to display a 
			// drop-down UI in the Properties window.
			var edSvc = (IWindowsFormsEditorService) provider.GetService(typeof (IWindowsFormsEditorService));
			if (edSvc != null)
			{
				// Display an angle selection control and retrieve the value.
				var dropDown = new LanguagePropertyChecker(context.Instance, (string) value);
				edSvc.DropDownControl(dropDown);

				return dropDown.FinalValues;
			}
			return value;
		}

		// Indicates whether the UITypeEditor supports painting a 
		// representation of a property's value.
		public override bool GetPaintValueSupported(ITypeDescriptorContext context)
		{
			return false;
		}
	}

	internal class LanguagePropertyChecker : UserControl
	{
		private readonly List<CheckBox> _boxes = new List<CheckBox>();

		public LanguagePropertyChecker(object obj, string currentValues)
		{
			string[] vals = currentValues.Split(';');
			int y = 0;
			var props = new List<PropertyInfo>();

			foreach (PropertyInfo prop in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
			{
				if (prop.PropertyType != typeof (string))
					continue;

				props.Add(prop);
			}

			AutoScroll = true;

			for (int i = 0; i < props.Count; ++i)
			{
				var box = new CheckBox();
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

				foreach (CheckBox box in _boxes)
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
	[ProvideProperty("PropertyNames", typeof (object))]
	public class LanguageProvider : Component, IExtenderProvider
	{
		private readonly Dictionary<object, LanguageControlLink> _properties = new Dictionary<object, LanguageControlLink>();

		#region IExtenderProvider Members

		public bool CanExtend(object extendee)
		{
			if (extendee is LanguageProvider)
				return false;

			if (extendee is Control || extendee is Component)
				return true;

			return false;
		}

		#endregion

		protected override void Dispose(bool disposing)
		{
			LanguageHandler.UnregisterProvider(this);

			base.Dispose(disposing);
		}

		Language _language;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Language Language
		{
			get { return _language; }
			set
			{
				_language = value;

				foreach (var obj in _properties)
				{
					if (!obj.Value.TextNamesSet)
						obj.Value.SetTextNames();

					for (int i = 0; i < obj.Value.PropertyNames.Length; ++i)
					{
						string strn = obj.Value.TextNames[i];

						if (_language.StringTable.ContainsKey(strn))
							obj.Value.PropertyNames[i].SetValue(obj.Value.Object, _language.StringTable[strn], null);
#if BETA
						else
						{
							MessageBox.Show("Stringtable string not found: " + strn);
						}
					}
#endif
				}
			}
		}

		Control _baseControl;
		public Control BaseControl
		{
			get { return _baseControl; }
			set { _baseControl = value; LanguageHandler.RegisterProvider(this); }
		}

		public void SetPropertyNames(object control, string names)
		{
			if (!_properties.ContainsKey(control))
				_properties.Add(control, new LanguageControlLink(names, control, DesignMode));
			else
				_properties[control] = new LanguageControlLink(names, control, DesignMode);
		}

		[DefaultValue("")]
		[Editor(typeof (LanguageEditor), typeof (UITypeEditor))]
		public string GetPropertyNames(object control)
		{
			if (!_properties.ContainsKey(control))
				return string.Empty;

			return _properties[control].OriginalString;
		}
	}
}