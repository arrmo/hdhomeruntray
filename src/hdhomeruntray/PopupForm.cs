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
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using zuki.hdhomeruntray.discovery;

namespace zuki.hdhomeruntray
{
	//-----------------------------------------------------------------------
	// Class PopupForm
	//
	// Form displayed as the popup from the notify icon, provides a display
	// of each discovered device and the status of the tuners/recordings

	internal partial class PopupForm : Form
	{
		#region Win32 API Declarations
		/// <summary>
		/// Win32 API Declarations
		/// </summary>
		private static class NativeMethods
		{
			[DllImport("gdi32.dll", ExactSpelling = true)]
			public static extern IntPtr CreateRoundRectRgn(int x1, int y1, int x2, int y2, int w, int h);

			[DllImport("gdi32.dll", ExactSpelling = true)]
			[return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
			public static extern bool DeleteObject(IntPtr hObject);
		}
		#endregion

		// Instance Constructor
		//
		public PopupForm()
		{
			InitializeComponent();
		}

		// Instance Constructor
		//
		public PopupForm(DeviceList devices) : this()
		{
			if(devices == null) throw new ArgumentNullException();

			// If no devices were detected, place a dummy item in the list
			if(devices.Count == 0)
			{
				m_layoutpanel.Controls.Add(PopupItemControl.NoDevices());
				return;
			}

			// Add each device as a PopupItemControl into the layout panel
			foreach(Device device in devices)
			{
				if(device is TunerDevice tuner) m_layoutpanel.Controls.Add(new PopupItemControl(tuner));
				else if(device is StorageDevice storage) m_layoutpanel.Controls.Add(new PopupItemControl(storage));
			}
		}

		// Dispose
		//
		// Releases unmanaged resources and optionally releases managed resources
		protected override void Dispose(bool disposing)
		{
			// Dispose managed state
			if(disposing && (components != null))
			{
				components.Dispose();
			}

			// Dispose unmanaged state
			if(m_hrgn != IntPtr.Zero) NativeMethods.DeleteObject(m_hrgn);

			base.Dispose(disposing);
		}

		//-------------------------------------------------------------------
		// Member Functions
		//-------------------------------------------------------------------

		// ShowFromNotifyIcon
		//
		// Shows the form at a position based on the working area and the
		// bounding rectangle of the notify icon instance
		public void ShowFromNotifyIcon(ShellNotifyIcon icon)
		{
			if(icon == null) throw new ArgumentNullException("icon");

			// Get the boundaries of the notify icon and the associated Screen
			Rectangle iconbounds = icon.GetBounds();
			Screen screen = Screen.FromPoint(iconbounds.Location);

			// This should work acceptably well given that the screen/monitor that will
			// display this form is the same one with the taskbar, but there are better ways
			// in .NET 4.7 and/or Windows 10/11 to figure out how to scale this value
			float scalefactor = ((float)SystemInformation.SmallIconSize.Height / 16.0F);

			// Move the form to the desired position before showing it; it should be right-aligned
			// with the right edge of the icon and sit above the taskbar
			var top = screen.WorkingArea.Height - this.Size.Height - (int)(12.0F * scalefactor);
			//this.Location = new Point(iconbounds.Right - this.Size.Width, top);

			// TODO: testing if I like this better; lower right corner of the screen
			var left = screen.WorkingArea.Width - this.Size.Width - (int)(12.0F * scalefactor);
			this.Location = new Point(left, top);

			this.Show();					// Show the form at the calculated position
		}

		//-------------------------------------------------------------------
		// Event Handlers
		//-------------------------------------------------------------------

		// OnSizeChanged
		//
		// Invoked when the size of the form has changed
		private void OnSizeChanged(object sender, EventArgs args)
		{
			IntPtr hrgnprev = m_hrgn;           // Save previous HRGN

			// Create a new region for the form based on the new width and height
			m_hrgn = NativeMethods.CreateRoundRectRgn(0, 0, Width, Height, 20, 20);
			Region = Region.FromHrgn(m_hrgn);

			// Release the previously set HRGN if it wasn't NULL
			if(hrgnprev != IntPtr.Zero) NativeMethods.DeleteObject(hrgnprev);
		}

		//-------------------------------------------------------------------
		// Member Variables
		//-------------------------------------------------------------------

		private IntPtr m_hrgn;					// Unmanaged HRGN object
	}
}