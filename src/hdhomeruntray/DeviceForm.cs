﻿//---------------------------------------------------------------------------
// Copyright (c) 2021 Michael G. Brehm
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//---------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace zuki.hdhomeruntray
{
	//-----------------------------------------------------------------------
	// Class DeviceForm (internal)
	//
	// Implements the form that shows detailed information about a device

	abstract partial class DeviceForm : Form
	{
		#region Win32 API Declarations
		private static class NativeMethods
		{
			public const int WS_EX_COMPOSITED = 0x02000000;

			public enum DWMWINDOWATTRIBUTE
			{
				DWMWA_WINDOW_CORNER_PREFERENCE = 33
			}

			public enum DWM_WINDOW_CORNER_PREFERENCE
			{
				DWMWCP_DEFAULT = 0,
				DWMWCP_DONOTROUND = 1,
				DWMWCP_ROUND = 2,
				DWMWCP_ROUNDSMALL = 3
			}

			[DllImport("dwmapi.dll")]
			public static extern long DwmSetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE attribute, ref DWM_WINDOW_CORNER_PREFERENCE pvAttribute, uint cbAttribute);
		}
		#endregion

		// Instance Constructor (private)
		//
		private DeviceForm()
		{
			InitializeComponent();

			m_layoutpanel.EnableDoubleBuferring();

			// WINDOWS 11
			//
			if(VersionHelper.IsWindows11OrGreater())
			{
				// Apply rounded corners to the form
				var attribute = NativeMethods.DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE;
				var preference = NativeMethods.DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUND;
				NativeMethods.DwmSetWindowAttribute(Handle, attribute, ref preference, sizeof(uint));
			}

			// Scale the padding based on the form DPI
			Padding = Padding.ScaleDPI(Handle);
			m_layoutpanel.Margin = m_layoutpanel.Margin.ScaleDPI(Handle);
			m_layoutpanel.Padding = m_layoutpanel.Padding.ScaleDPI(Handle);
		}

		// Instance Constructor (protected)
		//
		protected DeviceForm(PopupForm form, PopupItemControl item) : this()
		{
			if(form == null) throw new ArgumentNullException(nameof(form));
			if(item == null) throw new ArgumentNullException(nameof(item));

			m_popupformbounds = form.Bounds;
			m_popupitembounds = item.Bounds;
		}

		//-------------------------------------------------------------------
		// Control Overrides
		//-------------------------------------------------------------------

		// CreateParams
		//
		// Gets the required creation parameters when the control handle is created
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				cp.ExStyle |= NativeMethods.WS_EX_COMPOSITED;
				return cp;
			}
		}

		//-------------------------------------------------------------------
		// Event Handlers
		//-------------------------------------------------------------------

		// OnSizeChanged
		//
		// Invoked when the size of the form has changed
		private void OnSizeChanged(object sender, EventArgs args)
		{
			// This should work acceptably well given that the screen/monitor that will
			// display this form is the same one with the taskbar, but there are better ways
			// in .NET 4.7 and/or Windows 10/11 to figure out how to scale this value
			float scalefactor = (SystemInformation.SmallIconSize.Height / 16.0F);

			// The item's coordinates will be relative to the parent form
			var itemleft = m_popupformbounds.Left + m_popupitembounds.Left;

			// Move the form so that it's centered above the item that was used to open it
			var top = m_popupformbounds.Top - Size.Height - (int)(4.0F * scalefactor);
			var left = (itemleft + (m_popupitembounds.Width / 2)) - (Width / 2);

			// Adjust the left margin of the form if necessary
			if(left < m_popupformbounds.Left) left = m_popupformbounds.Left;

			// Adjust the right margin of the form if necessary
			var right = left + Width;
			if(right > m_popupformbounds.Right) left -= (right - m_popupformbounds.Right);

			// Set the location of the form
			Location = new Point(left, top);
		}

		//-------------------------------------------------------------------
		// Member Variables
		//-------------------------------------------------------------------

		private readonly Rectangle m_popupformbounds;
		private readonly Rectangle m_popupitembounds;
	}
}
