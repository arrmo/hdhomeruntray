﻿//---------------------------------------------------------------------------
// Copyright (c) 2021-2022 Michael G. Brehm
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
using System.Windows.Forms;

using zuki.hdhomeruntray.discovery;

namespace zuki.hdhomeruntray
{
	//--------------------------------------------------------------------------
	// Class TunerDeviceFooterControl (internal)
	//
	// User control that implements the footer for a tuner device in the DeviceForm

	internal partial class TunerDeviceFooterControl : UserControl
	{
		// Instance Constructor
		//
		private TunerDeviceFooterControl(SizeF scalefactor)
		{
			InitializeComponent();

			// THEME
			//
			m_appthemechanged = new EventHandler(OnApplicationThemeChanged);
			ApplicationTheme.Changed += m_appthemechanged;
			OnApplicationThemeChanged(this, EventArgs.Empty);

			m_layoutpanel.EnableDoubleBuferring();

			Padding = Padding.ScaleDPI(scalefactor);
			m_layoutpanel.Margin = m_layoutpanel.Margin.ScaleDPI(scalefactor);
			m_layoutpanel.Padding = m_layoutpanel.Padding.ScaleDPI(scalefactor);
			m_layoutpanel.Radii = m_layoutpanel.Radii.ScaleDPI(scalefactor);

			// WINDOWS 11
			//
			if(VersionHelper.IsWindows11OrGreater())
			{
				m_firmwareversion.Font = new Font("Segoe UI Variable Small", m_firmwareversion.Font.Size, m_firmwareversion.Font.Style);
				m_updateavailable.Font = new Font("Segoe UI Variable Small", m_updateavailable.Font.Size, m_updateavailable.Font.Style);
			}
		}

		// Instance Constructor
		//
		public TunerDeviceFooterControl(TunerDevice device, SizeF scalefactor) : this(scalefactor)
		{
			if(device == null) throw new ArgumentNullException(nameof(device));

			// Save the base URL string in case an update is available
			m_baseurl = device.BaseURL;

			// Just copy the data from the device instance into the appropriate controls
			m_firmwareversion.Text = "Firmware " + device.FirmwareVersion;
			m_updateavailable.Text = (device.FirmwareUpdateAvailable) ? "Update Available" : string.Empty;
		}

		// Dispose
		//
		// Releases unmanaged resources and optionally releases managed resources
		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				// Dispose managed state
				if(m_appthemechanged != null) ApplicationTheme.Changed -= m_appthemechanged;
				if(components != null) components.Dispose();
			}

			base.Dispose(disposing);
		}

		//-------------------------------------------------------------------
		// Event Handlers
		//-------------------------------------------------------------------

		// OnApplicationThemeChanged
		//
		// Invoked when the application theme has changed
		private void OnApplicationThemeChanged(object sender, EventArgs args)
		{
			m_layoutpanel.BackColor = ApplicationTheme.PanelBackColor;
			m_layoutpanel.ForeColor = ApplicationTheme.PanelForeColor;
			m_updateavailable.ActiveLinkColor = m_updateavailable.LinkColor = m_updateavailable.VisitedLinkColor = ApplicationTheme.LinkColor;
		}

		// OnUpdateClicked
		//
		// Invoked when the update link has been clicked
		private void OnUpdateClicked(object sender, LinkLabelLinkClickedEventArgs args)
		{
			using(Process process = new Process())
			{
				process.StartInfo.FileName = m_baseurl;
				process.StartInfo.UseShellExecute = true;
				process.StartInfo.Verb = "open";
				process.Start();
			}
		}

		//-------------------------------------------------------------------
		// Member Variables
		//-------------------------------------------------------------------

		private readonly string m_baseurl;
		private readonly EventHandler m_appthemechanged;
	}
}
