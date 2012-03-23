/***************************************************************************************************************************************
 * Copyright (C) 2001-2012 LearnLift USA																	*
 * Contact: Learnlift USA, 12 Greenway Plaza, Suite 1510, Houston, Texas 77046, support@memorylifter.com					*
 *																								*
 * This library is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License	*
 * as published by the Free Software Foundation; either version 2.1 of the License, or (at your option) any later version.			*
 *																								*
 * This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty	*
 * of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public License for more details.	*
 *																								*
 * You should have received a copy of the GNU Lesser General Public License along with this library; if not,					*
 * write to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA					*
 ***************************************************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MLifter.Controls;
using MLifterUpdateHandler.Properties;
using System.Reflection;
using System.IO;
using MLifterUpdateHandler.MLifterUpdateService;
using System.Net;

namespace MLifterUpdateHandler
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			TaskDialog.ForceEmulationMode = true;
			
			Version newVersion = null;
			string version = string.Empty;
			if (args.Length > 1 && args[0] == "-v")
			{
				version = args[1];
				newVersion = new Version(version);
			}
			else
			{
				TaskDialog.ShowCommandBox(Resources.TaskDialog_UpdateToBeta_Title, Resources.TaskDialog_UpdateToBeta_Caption,
					Resources.TaskDialog_UpdateToBeta_Text, Resources.TaskDialog_UpdateToBeta_Buttons, false);

				AssemblyName assemblyName = AssemblyName.GetAssemblyName(Path.Combine(Application.StartupPath, "MLifter.exe"));
				WebClient client = new WebClient();
				version = client.DownloadString(string.Format("{0}?base={1}&beta={2}&onstick={3}",
					Settings.Default.UpdateVersionUrl, assemblyName.Version.ToString(2), 
					(TaskDialog.CommandButtonResult == 0).ToString(), MLifter.Generics.Methods.IsRunningFromStick()));
				newVersion = new Version(version);

				if (newVersion <= assemblyName.Version)
				{
					TaskDialog.MessageBox(Resources.TASK_DIALOG_NoNewVersionFound_Title, Resources.TASK_DIALOG_NoNewVersionFound_Caption, Resources.TASK_DIALOG_NoNewVersionFound_Text,
						TaskDialogButtons.OK, TaskDialogIcons.Information);
					Environment.Exit(-1);
				}
			}

			FileStream stream = null;
			try
			{
				UpdateService updateService = new UpdateService();
				byte[] dll = updateService.ServeLatestMLVersion(newVersion.ToString(3), MLifter.Generics.Methods.IsRunningFromStick());
				string dllPath = Path.GetTempFileName();
				stream = File.OpenWrite(dllPath);
				stream.Write(dll, 0, dll.Length);
				stream.Close();
				stream = null;

				Assembly assembly = null;
				string typeName = string.Empty;
				Type pluginType = null;

				if (File.Exists(dllPath))
					assembly = Assembly.LoadFile(dllPath);
				else
					throw new FileNotFoundException();

				if (assembly != null)
				{
					foreach (Type type in assembly.GetTypes())
					{
						if (type.IsAbstract) continue;
						if (type.IsDefined(typeof(UpdateHandler), true))
						{
							pluginType = type;
							break;
						}
					}

					if (pluginType != null)
					{
						IUpdateHandler updateHandler = Activator.CreateInstance(pluginType) as IUpdateHandler;
						updateHandler.StartUpdateProcess(newVersion);
					}
					else
						throw new Exception(Resources.Exception_CouldNotFindUpdateLogic);
				}
				else
					throw new Exception(Resources.Exception_ErrorLoadingUpdateLogic);
			}
			catch (Exception e)
			{
				TaskDialog.MessageBox(Resources.TaskDialog_ErrorStartingUpdate_Title, Resources.TaskDialog_ErrorStartingUpdate_Title, Resources.TaskDialog_ErrorStartingUpdate_Content,
					e.ToString(), string.Empty, string.Empty, TaskDialogButtons.OK, TaskDialogIcons.Error, TaskDialogIcons.Error);
			}
			finally
			{
				if (stream != null)
					stream.Close();
			}
		}
	}
}
