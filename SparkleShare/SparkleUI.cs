//   SparkleShare, an instant update workflow to Git.
//   Copyright (C) 2010  Hylke Bons <hylkebons@gmail.com>
//
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details.
//
//   You should have received a copy of the GNU General Public License
//   along with this program. If not, see <http://www.gnu.org/licenses/>.

using Gtk;
using Mono.Unix;
using Mono.Unix.Native;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace SparkleShare {

	public class SparkleUI {
		
		public static SparkleStatusIcon StatusIcon;
		public static List <SparkleLog> OpenLogs;


		// Short alias for the translations
		public static string _(string s)
		{
			return Catalog.GetString (s);
		}


		public SparkleUI ()
		{

			// Initialize the application
			Application.Init ();

			// Create the statusicon
			StatusIcon = new SparkleStatusIcon ();
			
			// Keep track of event logs are open
			SparkleUI.OpenLogs = new List <SparkleLog> ();

			SparkleShare.Controller.OnFirstRun += delegate {
				Application.Invoke (delegate {

					SparkleIntro intro = new SparkleIntro ();
					intro.ShowAll ();

				});
			};

			SparkleShare.Controller.OnInvitation += delegate (string invitation_file_path) {
				Application.Invoke (delegate {

					SparkleInvitation invitation = new SparkleInvitation (invitation_file_path);
					invitation.Present ();				

				});
			};

			// Show a bubble when there are new changes
			SparkleShare.Controller.NotificationRaised += delegate (string author, string email, string message,
				string repository_path) {

				Application.Invoke (delegate {

					SparkleBubble bubble = new SparkleBubble (author, message) {
						Icon = SparkleUIHelpers.GetAvatar (email, 32)				
					};

					bubble.AddAction ("", "Show Events", delegate {
				
						SparkleLog log = new SparkleLog (repository_path);
						log.ShowAll ();
				
					});

						bubble.Show ();

				});

			};

			// Show a bubble when there was a conflict
			SparkleShare.Controller.ConflictNotificationRaised += delegate {
				Application.Invoke (delegate {

					string title   = _("Ouch! Mid-air collision!");
					string subtext = _("Don't worry, SparkleShare made a copy of each conflicting file.");

					SparkleBubble bubble = new SparkleBubble(title, subtext);
					bubble.Show ();

				});
			};

		}


		// Runs the application
		public void Run ()
		{

			Application.Run ();

		}

	}

}
