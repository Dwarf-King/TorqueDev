/*

This file is part of TorqueDev.

TorqueDev is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by the 
Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

TorqueDev is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with TorqueDev.  If not, see <http://www.gnu.org/licenses>

EXCEPTIONS TO THE GPL: TorqueDev links in a number of third party libraries,
which are exempt from the license.  If you want to write closed-source
third party modules that you are going to link into TorqueDev, you may do so
without restriction.  I acknowledge that this technically allows for
one to bypass the open source provisions of the GPL, but the real goal
is to keep the core of TorqueDev free and open.  The rest is up to you.

*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace TSDev {
	public partial class frmLicManager : Form {

		int step = 1;

		public frmLicManager() {
			InitializeComponent();
		}

		private void frmLicManager_Load(object sender, EventArgs e) {

		}

		private void cmdNext_Click(object sender, EventArgs e) {
			if (step == 1) {
				// Going to step 2
				pnlStep1.Visible = false;
				pnlStep2.Visible = true;

				cmdPrevious.Enabled = true;
				lblTopTitle.Text = "License Manager - Import License File";
				step++;
			} else if (step == 2) {
				// Going to step 3 (Verify the license and display the
				// proper panel)
				if (txtLicFile.Text.Trim() == "") {
					MessageBox.Show("Please enter a license file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					return;
				} else if (!File.Exists(txtLicFile.Text)) {
					MessageBox.Show("The filename entered does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					return;
				}

				// Try to load the license
				LicenseManager license = null;

				try {
					license = new LicenseManager(txtLicFile.Text);
				} catch {
					// Go to step 3 failure
					cmdNext.Text = "&Finish";
					pnlStep3Fail.Visible = true;
					lblTopTitle.Text = "License Manager - Failed to Import License";

					step++;
					return;
				}

				// Get the license data and populate the fields
				lblLicTo.Text = license.LicensedUser;
				lblLicCompany.Text = license.LicensedCompany;
				lblLicExpires.Text = ((license.LicenseExpires.Year > 1) ? license.LicenseExpires.ToLongDateString() : "Never");
				lblLicRestr.Text = ((license.LicenseLimit == "Unlimited") ? "None" : ("Licensed to " + license.LicenseLimit + " Computer(s)"));
				lblLicSerial.Text = license.LicenseSerial;

				// Check the license type
				switch (license.LicenseSerial.Substring(0, 3)) {
					case "CWF":
						lblLicType.Text = "Standard License";
						break;
					case "CWP":
						lblLicType.Text = "Professional License";
						break;
					case "CPS":
						lblLicType.Text = "Professional Site License";
						break;
					case "CWM":
						lblLicType.Text = "Professional Education License";
						break;
					case "CMS":
						lblLicType.Text = "Professional Education Site License";
						break;
					case "CWD":
						lblLicType.Text = "Donator Site License";
						break;
					default:
						lblLicType.Text = "Special License";
						break;
				}

				// Copy over the license file
				File.Copy(txtLicFile.Text, Application.StartupPath + "\\license.xml", true);

				// Display the OK thing
				cmdNext.Text = "&Finish";
				pnlStep3Success.Visible = true;
				lblTopTitle.Text = "License Manager - Import Successful";

				// Set the back and cancel button to disabled
				cmdCancel.Enabled = false;
				cmdPrevious.Enabled = false;

				step++;
			} else if (step == 3) {
				// Going to step 4 (Finish)
				this.Close();
			}
		}

		private void cmdPrevious_Click(object sender, EventArgs e) {
			if (step == 2) {
				// Going to step 1
				pnlStep2.Visible = false;
				pnlStep1.Visible = true;

				cmdPrevious.Enabled = false;
				lblTopTitle.Text = "License Manager - Piracy Notice";
				step--;
			} else if (step == 3) {
				// Going to step 2
				pnlStep2.Visible = true;
				pnlStep3Fail.Visible = false;
				pnlStep3Success.Visible = false;

				cmdNext.Text = "&Next ►";

				lblTopTitle.Text = "License Manager - Import License File";
				step--;
			}
		}

		private void cmdCancel_Click(object sender, EventArgs e) {
			this.Close();
		}

		private void cmdBrowse_Click(object sender, EventArgs e) {
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.CheckFileExists = true;
			ofd.Title = "Select License File";
			ofd.Filter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*";
			ofd.RestoreDirectory = true;

			DialogResult result = ofd.ShowDialog();

			if (result == DialogResult.Cancel)
				return;

			txtLicFile.Text = ofd.FileName;

			ofd.Dispose();
			ofd = null;
		}
	}
}