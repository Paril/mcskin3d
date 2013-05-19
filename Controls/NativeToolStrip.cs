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
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

// Thanks for fixes:
//  * Marco Minerva, jachymko - http://www.codeplex.com/windowsformsaero
//  * Ben Ryves - http://www.benryves.com/

namespace Szotar.WindowsForms
{
	public enum ToolbarTheme
	{
		Toolbar,
		MediaToolbar,
		CommunicationsToolbar,
		BrowserTabBar,
		HelpBar
	}

	/// <summary>Renders a toolstrip using the UxTheme API via VisualStyleRenderer and a specific style.</summary>
	/// <remarks>Perhaps surprisingly, this does not need to be disposable.</remarks>
	public class ToolStripAeroRenderer : ToolStripSystemRenderer
	{
		private const int RebarBackground = 6;
		private VisualStyleRenderer renderer;

		public ToolStripAeroRenderer(ToolbarTheme theme)
		{
			Theme = theme;
		}

		public ToolbarTheme Theme { get; set; }

		private string RebarClass
		{
			get { return SubclassPrefix + "Rebar"; }
		}

		private string ToolbarClass
		{
			get { return SubclassPrefix + "ToolBar"; }
		}

		private string MenuClass
		{
			get { return SubclassPrefix + "Menu"; }
		}

		private string SubclassPrefix
		{
			get
			{
				switch (Theme)
				{
					case ToolbarTheme.MediaToolbar:
						return "Media::";
					case ToolbarTheme.CommunicationsToolbar:
						return "Communications::";
					case ToolbarTheme.BrowserTabBar:
						return "BrowserTabBar::";
					case ToolbarTheme.HelpBar:
						return "Help::";
					default:
						return string.Empty;
				}
			}
		}

		public bool IsSupported
		{
			get
			{
				if (!VisualStyleRenderer.IsSupported)
					return false;

				// Needs a more robust check. It seems mono supports very different style sets.
				return
					VisualStyleRenderer.IsElementDefined(
						VisualStyleElement.CreateElement("Menu",
						                                 (int) MenuParts.BarBackground,
						                                 (int) MenuBarStates.Active));
			}
		}

		private Padding GetThemeMargins(IDeviceContext dc, MarginTypes marginType)
		{
			NativeMethods.MARGINS margins;
			try
			{
				IntPtr hDC = dc.GetHdc();
				if (0 ==
				    NativeMethods.GetThemeMargins(renderer.Handle, hDC, renderer.Part, renderer.State, (int) marginType, IntPtr.Zero,
				                                  out margins))
					return new Padding(margins.cxLeftWidth, margins.cyTopHeight, margins.cxRightWidth, margins.cyBottomHeight);
				return new Padding(0);
			}
			finally
			{
				dc.ReleaseHdc();
			}
		}

		private static int GetItemState(ToolStripItem item)
		{
			bool hot = item.Selected;

			if (item.IsOnDropDown)
			{
				if (item.Enabled)
					return hot ? (int) MenuPopupItemStates.Hover : (int) MenuPopupItemStates.Normal;
				return hot ? (int) MenuPopupItemStates.DisabledHover : (int) MenuPopupItemStates.Disabled;
			}
			else
			{
				if (item.Pressed)
					return item.Enabled ? (int) MenuBarItemStates.Pushed : (int) MenuBarItemStates.DisabledPushed;
				if (item.Enabled)
					return hot ? (int) MenuBarItemStates.Hover : (int) MenuBarItemStates.Normal;
				return hot ? (int) MenuBarItemStates.DisabledHover : (int) MenuBarItemStates.Disabled;
			}
		}

		private VisualStyleElement Subclass(VisualStyleElement element)
		{
			return VisualStyleElement.CreateElement(SubclassPrefix + element.ClassName,
			                                        element.Part, element.State);
		}

		private bool EnsureRenderer()
		{
			if (!IsSupported)
				return false;

			if (renderer == null)
				renderer = new VisualStyleRenderer(VisualStyleElement.Button.PushButton.Normal);

			return true;
		}

		// Gives parented ToolStrips a transparent background.
		protected override void Initialize(ToolStrip toolStrip)
		{
			if (toolStrip.Parent is ToolStripPanel)
				toolStrip.BackColor = Color.Transparent;

			base.Initialize(toolStrip);
		}

		// Using just ToolStripManager.Renderer without setting the Renderer individually per ToolStrip means
		// that the ToolStrip is not passed to the Initialize method. ToolStripPanels, however, are. So we can 
		// simply initialize it here too, and this should guarantee that the ToolStrip is initialized at least 
		// once. Hopefully it isn't any more complicated than this.
		protected override void InitializePanel(ToolStripPanel toolStripPanel)
		{
			foreach (Control control in toolStripPanel.Controls)
			{
				if (control is ToolStrip)
					Initialize((ToolStrip) control);
			}

			base.InitializePanel(toolStripPanel);
		}

		protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
		{
			if (EnsureRenderer())
			{
				renderer.SetParameters(MenuClass, (int) MenuParts.PopupBorders, 0);
				if (e.ToolStrip.IsDropDown)
				{
					Region oldClip = e.Graphics.Clip;

					// Tool strip borders are rendered *after* the content, for some reason.
					// So we have to exclude the inside of the popup otherwise we'll draw over it.
					Rectangle insideRect = e.ToolStrip.ClientRectangle;
					insideRect.Inflate(-1, -1);
					e.Graphics.ExcludeClip(insideRect);

					renderer.DrawBackground(e.Graphics, e.ToolStrip.ClientRectangle, e.AffectedBounds);

					// Restore the old clip in case the Graphics is used again (does that ever happen?)
					e.Graphics.Clip = oldClip;
				}
			}
			else
				base.OnRenderToolStripBorder(e);
		}

		private Rectangle GetBackgroundRectangle(ToolStripItem item)
		{
			if (!item.IsOnDropDown)
				return new Rectangle(new Point(), item.Bounds.Size);

			// For a drop-down menu item, the background rectangles of the items should be touching vertically.
			// This ensures that's the case.
			Rectangle rect = item.Bounds;

			// The background rectangle should be inset two pixels horizontally (on both sides), but we have 
			// to take into account the border.
			rect.X = item.ContentRectangle.X + 1;
			rect.Width = item.ContentRectangle.Width - 1;

			// Make sure we're using all of the vertical space, so that the edges touch.
			rect.Y = 0;
			return rect;
		}

		protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
		{
			if (EnsureRenderer())
			{
				int partID = e.Item.IsOnDropDown ? (int) MenuParts.PopupItem : (int) MenuParts.BarItem;
				renderer.SetParameters(MenuClass, partID, GetItemState(e.Item));

				Rectangle bgRect = GetBackgroundRectangle(e.Item);
				renderer.DrawBackground(e.Graphics, bgRect, bgRect);
			}
			else
				base.OnRenderMenuItemBackground(e);
		}

		protected override void OnRenderToolStripPanelBackground(ToolStripPanelRenderEventArgs e)
		{
			if (EnsureRenderer())
			{
				// Draw the background using Rebar & RP_BACKGROUND (or, if that is not available, fall back to
				// Rebar.Band.Normal)
				if (VisualStyleRenderer.IsElementDefined(VisualStyleElement.CreateElement(RebarClass, RebarBackground, 0)))
					renderer.SetParameters(RebarClass, RebarBackground, 0);
				else
					renderer.SetParameters(RebarClass, 0, 0);

				if (renderer.IsBackgroundPartiallyTransparent())
					renderer.DrawParentBackground(e.Graphics, e.ToolStripPanel.ClientRectangle, e.ToolStripPanel);

				renderer.DrawBackground(e.Graphics, e.ToolStripPanel.ClientRectangle);

				e.Handled = true;
			}
			else
				base.OnRenderToolStripPanelBackground(e);
		}

		// Render the background of an actual menu bar, dropdown menu or toolbar.
		protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
		{
			if (EnsureRenderer())
			{
				if (e.ToolStrip.IsDropDown)
					renderer.SetParameters(MenuClass, (int) MenuParts.PopupBackground, 0);
				else
				{
					// It's a MenuStrip or a ToolStrip. If it's contained inside a larger panel, it should have a
					// transparent background, showing the panel's background.

					if (e.ToolStrip.Parent is ToolStripPanel)
					{
						// The background should be transparent, because the ToolStripPanel's background will be visible.
						// (Of course, we assume the ToolStripPanel is drawn using the same theme, but it's not my fault
						// if someone does that.)
						return;
					}
					else
					{
						// A lone toolbar/menubar should act like it's inside a toolbox, I guess.
						// Maybe I should use the MenuClass in the case of a MenuStrip, although that would break
						// the other themes...
						if (VisualStyleRenderer.IsElementDefined(VisualStyleElement.CreateElement(RebarClass, RebarBackground, 0)))
							renderer.SetParameters(RebarClass, RebarBackground, 0);
						else
							renderer.SetParameters(RebarClass, 0, 0);
					}
				}

				if (renderer.IsBackgroundPartiallyTransparent())
					renderer.DrawParentBackground(e.Graphics, e.ToolStrip.ClientRectangle, e.ToolStrip);

				renderer.DrawBackground(e.Graphics, e.ToolStrip.ClientRectangle, e.AffectedBounds);
			}
			else
				base.OnRenderToolStripBackground(e);
		}

		// The only purpose of this override is to change the arrow colour.
		// It's OK to just draw over the default arrow since we also pass down arrow drawing to the system renderer.
		protected override void OnRenderSplitButtonBackground(ToolStripItemRenderEventArgs e)
		{
			if (EnsureRenderer())
			{
				var sb = (ToolStripSplitButton) e.Item;
				base.OnRenderSplitButtonBackground(e);

				// It doesn't matter what colour of arrow we tell it to draw. OnRenderArrow will compute it from the item anyway.
				OnRenderArrow(new ToolStripArrowRenderEventArgs(e.Graphics, sb, sb.DropDownButtonBounds, Color.Red,
				                                                ArrowDirection.Down));
			}
			else
				base.OnRenderSplitButtonBackground(e);
		}

		private Color GetItemTextColor(ToolStripItem item)
		{
			int partId = item.IsOnDropDown ? (int) MenuParts.PopupItem : (int) MenuParts.BarItem;
			renderer.SetParameters(MenuClass, partId, GetItemState(item));
			return renderer.GetColor(ColorProperty.TextColor);
		}

		protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
		{
			if (EnsureRenderer())
				e.TextColor = GetItemTextColor(e.Item);

			base.OnRenderItemText(e);
		}

		protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
		{
			if (EnsureRenderer())
			{
				if (e.ToolStrip.IsDropDown)
				{
					renderer.SetParameters(MenuClass, (int) MenuParts.PopupGutter, 0);
					// The AffectedBounds is usually too small, way too small to look right. Instead of using that,
					// use the AffectedBounds but with the right width. Then narrow the rectangle to the correct edge
					// based on whether or not it's RTL. (It doesn't need to be narrowed to an edge in LTR mode, but let's
					// do that anyway.)
					// Using the DisplayRectangle gets roughly the right size so that the separator is closer to the text.
					Padding margins = GetThemeMargins(e.Graphics, MarginTypes.Sizing);
					int extraWidth = (e.ToolStrip.Width - e.ToolStrip.DisplayRectangle.Width - margins.Left - margins.Right - 1) -
					                 e.AffectedBounds.Width;
					Rectangle rect = e.AffectedBounds;
					rect.Y += 2;
					rect.Height -= 4;
					int sepWidth = renderer.GetPartSize(e.Graphics, ThemeSizeType.True).Width;
					if (e.ToolStrip.RightToLeft == RightToLeft.Yes)
					{
						rect = new Rectangle(rect.X - extraWidth, rect.Y, sepWidth, rect.Height);
						rect.X += sepWidth;
					}
					else
						rect = new Rectangle(rect.Width + extraWidth - sepWidth, rect.Y, sepWidth, rect.Height);
					renderer.DrawBackground(e.Graphics, rect);
				}
			}
			else
				base.OnRenderImageMargin(e);
		}

		protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
		{
			if (e.ToolStrip.IsDropDown && EnsureRenderer())
			{
				renderer.SetParameters(MenuClass, (int) MenuParts.PopupSeparator, 0);
				var rect = new Rectangle(e.ToolStrip.DisplayRectangle.Left, 0, e.ToolStrip.DisplayRectangle.Width, e.Item.Height);
				renderer.DrawBackground(e.Graphics, rect, rect);
			}
			else
				base.OnRenderSeparator(e);
		}

		protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
		{
			if (EnsureRenderer())
			{
				Rectangle bgRect = GetBackgroundRectangle(e.Item);
				bgRect.Width = bgRect.Height;

				// Now, mirror its position if the menu item is RTL.
				if (e.Item.RightToLeft == RightToLeft.Yes)
				{
					bgRect = new Rectangle(e.ToolStrip.ClientSize.Width - bgRect.X - bgRect.Width, bgRect.Y, bgRect.Width,
					                       bgRect.Height);
				}

				renderer.SetParameters(MenuClass, (int) MenuParts.PopupCheckBackground,
				                       e.Item.Enabled
				                       	? (int) MenuPopupCheckBackgroundStates.Normal
				                       	: (int) MenuPopupCheckBackgroundStates.Disabled);
				renderer.DrawBackground(e.Graphics, bgRect);

				Rectangle checkRect = e.ImageRectangle;
				checkRect.X = bgRect.X + bgRect.Width / 2 - checkRect.Width / 2;
				checkRect.Y = bgRect.Y + bgRect.Height / 2 - checkRect.Height / 2;

				// I don't think ToolStrip even supports radio box items, so no need to render them.
				renderer.SetParameters(MenuClass, (int) MenuParts.PopupCheck,
				                       e.Item.Enabled
				                       	? (int) MenuPopupCheckStates.CheckmarkNormal
				                       	: (int) MenuPopupCheckStates.CheckmarkDisabled);

				renderer.DrawBackground(e.Graphics, checkRect);
			}
			else
				base.OnRenderItemCheck(e);
		}

		protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
		{
			// The default renderer will draw an arrow for us (the UXTheme API seems not to have one for all directions),
			// but it will get the colour wrong in many cases. The text colour is probably the best colour to use.
			if (EnsureRenderer())
				e.ArrowColor = GetItemTextColor(e.Item);
			base.OnRenderArrow(e);
		}

		protected override void OnRenderOverflowButtonBackground(ToolStripItemRenderEventArgs e)
		{
			if (EnsureRenderer())
			{
				// BrowserTabBar::Rebar draws the chevron using the default background. Odd.
				string rebarClass = RebarClass;
				if (Theme == ToolbarTheme.BrowserTabBar)
					rebarClass = "Rebar";

				int state = VisualStyleElement.Rebar.Chevron.Normal.State;
				if (e.Item.Pressed)
					state = VisualStyleElement.Rebar.Chevron.Pressed.State;
				else if (e.Item.Selected)
					state = VisualStyleElement.Rebar.Chevron.Hot.State;

				renderer.SetParameters(rebarClass, VisualStyleElement.Rebar.Chevron.Normal.Part, state);
				renderer.DrawBackground(e.Graphics, new Rectangle(Point.Empty, e.Item.Size));
			}
			else
				base.OnRenderOverflowButtonBackground(e);
		}

		#region Nested type: MarginTypes

		private enum MarginTypes
		{
			Sizing = 3601,
			Content = 3602,
			Caption = 3603
		}

		#endregion

		#region Nested type: MenuBarItemStates

		private enum MenuBarItemStates
		{
			Normal = 1,
			Hover = 2,
			Pushed = 3,
			Disabled = 4,
			DisabledHover = 5,
			DisabledPushed = 6
		}

		#endregion

		#region Nested type: MenuBarStates

		private enum MenuBarStates
		{
			Active = 1,
			Inactive = 2
		}

		#endregion

		#region Nested type: MenuParts

		private enum MenuParts
		{
			ItemTMSchema = 1,
			DropDownTMSchema = 2,
			BarItemTMSchema = 3,
			BarDropDownTMSchema = 4,
			ChevronTMSchema = 5,
			SeparatorTMSchema = 6,
			BarBackground = 7,
			BarItem = 8,
			PopupBackground = 9,
			PopupBorders = 10,
			PopupCheck = 11,
			PopupCheckBackground = 12,
			PopupGutter = 13,
			PopupItem = 14,
			PopupSeparator = 15,
			PopupSubmenu = 16,
			SystemClose = 17,
			SystemMaximize = 18,
			SystemMinimize = 19,
			SystemRestore = 20
		}

		#endregion

		#region Nested type: MenuPopupCheckBackgroundStates

		private enum MenuPopupCheckBackgroundStates
		{
			Disabled = 1,
			Normal = 2,
			Bitmap = 3
		}

		#endregion

		#region Nested type: MenuPopupCheckStates

		private enum MenuPopupCheckStates
		{
			CheckmarkNormal = 1,
			CheckmarkDisabled = 2,
			BulletNormal = 3,
			BulletDisabled = 4
		}

		#endregion

		#region Nested type: MenuPopupItemStates

		private enum MenuPopupItemStates
		{
			Normal = 1,
			Hover = 2,
			Disabled = 3,
			DisabledHover = 4
		}

		#endregion

		#region Nested type: MenuPopupSubMenuStates

		private enum MenuPopupSubMenuStates
		{
			Normal = 1,
			Disabled = 2
		}

		#endregion

		#region Nested type: NativeMethods

		/// <summary>
		/// It shouldn't be necessary to P/Invoke like this, however VisualStyleRenderer.GetMargins
		/// misses out a parameter in its own P/Invoke.
		/// </summary>
		internal static class NativeMethods
		{
			[DllImport("uxtheme.dll")]
			public static extern int GetThemeMargins(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, int iPropId,
			                                         IntPtr rect, out MARGINS pMargins);

			#region Nested type: MARGINS

			[StructLayout(LayoutKind.Sequential)]
			public struct MARGINS
			{
				public int cxLeftWidth;
				public int cxRightWidth;
				public int cyTopHeight;
				public int cyBottomHeight;
			}

			#endregion
		}

		#endregion
	}

	public class NativeToolStrip : ToolStrip
	{
		public NativeToolStrip()
		{
			Renderer = new ToolStripAeroRenderer(ToolbarTheme.Toolbar);
		}
	}
}