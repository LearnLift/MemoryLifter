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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using MLifter.BusinessLayer;
using MLifter.CardCollector;
using MLifter.Components;
using MLifter.Controls;
using MLifter.Controls.Wizards.DictionaryCreator;
using MLifter.DAL;
using MLifter.DAL.Interfaces;
using MLifter.Generics;
using MLifter.ImportExport;
using MLifter.Properties;
using System.Runtime.InteropServices;
using MLifter.Controls.LearningModulesPage;

namespace MLifter
{
	/// <summary>
	/// Contains the user interface of MemoryLifter.
	/// </summary>
	/// <remarks>Documented by Dev01, 2007-07-20</remarks>
	public partial class MainForm : System.Windows.Forms.Form
	{
		#region Fields/Designer Variables

		private static LearnLogic learnLogic = null;
		public static LearnLogic LearnLogic
		{
			get
			{
				if (learnLogic != null && learnLogic.UserSessionAlive)
					return learnLogic;

				return null;
			}
			set { learnLogic = value; }
		}

		private static bool OnStickMode;
		private bool newLM = false;
		public Components.DriveDetector driveDetector;
		private HelpProvider MainHelp;
		private FormWindowState mainFormWindowState;
		private bool loadWindowSettings = false;

		/// <summary>
		/// Occurs when [mainform is loaded and ready for action.].
		/// </summary>
		/// <remarks>Documented by Dev02, 2009-06-29</remarks>
		private event EventHandler MainformLoadedEvent;

		private List<EventHandler> MainformLoadedDelegates = new List<EventHandler>();

		/// <summary>
		/// Occurs when [mainform loaded event].
		/// </summary>
		/// <remarks>Documented by Dev02, 2009-06-29</remarks>
		public event EventHandler MainformLoaded
		{
			add
			{
				MainformLoadedEvent += value;
				MainformLoadedDelegates.Add(value);
			}
			remove
			{
				MainformLoadedEvent -= value;
				MainformLoadedDelegates.Remove(value);
			}
		}

		//[ML-694] TaskDialog must be initialized or the UICultureChanger will fail (DAC, 2008-03-04)
		private MLifter.Controls.EmulatedTaskDialog driveRemoved = new MLifter.Controls.EmulatedTaskDialog();
		private System.Windows.Forms.OpenFileDialog OpenDialog;
		private System.Windows.Forms.SaveFileDialog SaveDialog;
		private System.ComponentModel.IContainer components;

		private string commandLineParam = string.Empty;
		private int trayWaitMinutesLeft;

		public static MLifter.Components.StyleHandler styleHandler = null;

		private UICultureChanger uiCultureChanger;
		private ToolStrip toolStripTop;
		private ToolStripButton toolStripButtonNew;
		private ToolStripButton toolStripButtonOpen;
		private ToolStripButton toolStripButtonPrint;
		private ToolStripSeparator toolStripSeparator1;
		private ToolStripButton toolStripButtonNewCard;
		private ToolStripButton toolStripButtonMaintain;
		private ToolStripSeparator toolStripSeparator2;
		private ToolStripButton toolStripButtonStatistics;
		private ToolStripButton toolStripButtonBoxSize;
		private ToolStripButton toolStripButtonCharMap;
		private ToolStripButton toolStripButtonOptions;
		private ToolStripButton toolStripButtonHelp;
		private MLifter.Controls.LearningWindow.LearningWindow learningWindow;
		private MenuStrip menuStripMain;
		private ToolStripMenuItem fileToolStripMenuItem;
		private ToolStripMenuItem newToolStripMenuItem;
		private ToolStripMenuItem openToolStripMenuItem;
		private ToolStripMenuItem closeToolStripMenuItem;
		private ToolStripSeparator toolStripSeparator5;
		private ToolStripMenuItem saveAsToolStripMenuItem;
		private ToolStripSeparator toolStripSeparator10;
		private ToolStripMenuItem propertiesToolStripMenuItem;
		private ToolStripSeparator toolStripSeparatorAboveRecentFiles;
		private ToolStripSeparator toolStripSeparatorBelowRecentFiles;
		private ToolStripMenuItem exitToolStripMenuItem;
		private ToolStripMenuItem editToolStripMenuItem;
		private ToolStripMenuItem addCardToolStripMenuItem;
		private ToolStripSeparator toolStripSeparator14;
		private ToolStripMenuItem editChaptersToolStripMenuItem;
		private ToolStripMenuItem learnToolStripMenuItem;
		private ToolStripMenuItem selectChaptersToolStripMenuItem;
		private ToolStripMenuItem selectBoxToolStripMenuItem;
		private ToolStripSeparator toolStripSeparator16;
		private ToolStripMenuItem showBoxSizeToolStripMenuItem;
		private ToolStripMenuItem showStatisticsToolStripMenuItem;
		private ToolStripMenuItem restartLearningToolStripMenuItem;
		private ToolStripSeparator toolStripSeparator17;
		private ToolStripMenuItem learningOptionsToolStripMenuItem;
		private ToolStripMenuItem toolsToolStripMenuItem;
		private ToolStripMenuItem characterMapToolStripMenuItem;
		private ToolStripMenuItem snoozeModeMemorylifterToolStripMenuItem;
		private ToolStripSeparator toolStripSeparator18;
		private ToolStripMenuItem switchSkinToolStripMenuItem;
		private ToolStripMenuItem switchLanguageToolStripMenuItem;
		private ToolStripMenuItem styleEditorToolStripMenuItem;
		private ToolStripSeparator toolStripSeparator22;
		private ToolStripMenuItem helpToolStripMenuItem;
		private ToolStripMenuItem firstStepsToolStripMenuItem;
		private ToolStripMenuItem fAQToolStripMenuItem;
		private ToolStripSeparator toolStripSeparator23;
		private ToolStripMenuItem homepageToolStripMenuItem;
		private ToolStripMenuItem recommendToAFriendToolStripMenuItem;
		private ToolStripSeparator toolStripSeparator24;
		private ToolStripMenuItem aboutMemoryLifterToolStripMenuItem;
		private ToolStripMenuItem printToolStripMenuItem;
		private ToolStripMenuItem maintainCardsToolStripMenuItem;
		private ToolStripMenuItem changeLMFolderToolStripMenuItem;
		private CommandDemo.UICommandProvider uiCommandProvider;
		private CommandDemo.UICommand uiCommandLMOpenedMenu;
		private CommandDemo.UICommand uiCommandEditCards;
		private CommandDemo.UICommand uiCommandCanExport;
		private CommandDemo.UICommand uiCommandCanSaveCopy;
		private CommandDemo.UICommand uiCommandCanModify;
		private CommandDemo.UICommand uiCommandCanModifyChapters;
		private CommandDemo.UICommand uiCommandCanModifySettings;
		private NotifyIcon TrayIcon;
		private System.Windows.Forms.Timer timerSnooze;
		private ContextMenuStrip menuStripTray;
		private ToolStripMenuItem continueToolStripMenuItem;
		private ToolStripSeparator toolStripSeparator8;
		private ToolStripMenuItem exitToolStripMenuItem1;
		private MLifter.Controls.LearningWindow.CharacterMapComponent characterMapComponent;
		private ToolStripSeparator toolStripSeparator6;
		private ToolStripMenuItem importToolStripMenuItem;
		private ToolStripMenuItem exportToolStripMenuItem;
		private ToolStripMenuItem recentToolStripMenuItem;
		private ToolStripMenuItem fileImportToolStripMenuItem;
		private ToolStripMenuItem clipboardImportToolStripMenuItem;
		private ToolStripMenuItem helpIndexToolStripMenuItem;
		private ToolStripMenuItem helpSearchToolStripMenuItem;
		private ToolStripButton slideShowToolStripButton;
		private ToolStripSeparator toolStripSeparator3;
		private ToolStripMenuItem slideShowModeToolStripMenuItem;
		private ToolStripDropDownButton toolStripDropDownButtonChangeLearnModus;
		private ToolStripMenuItem toolStripMenuItemStandard;
		private ToolStripMenuItem toolStripMenuItemMultipleChoice;
		private ToolStripMenuItem toolStripMenuItemSentences;
		private ToolStripMenuItem toolStripMenuItemListeningComprehension;
		private ToolStripMenuItem toolStripMenuItemImageRecognition;
		private ToolStripDropDown toolStripDropDownChangeLearnModus;
		private ToolStripMenuItem testToolStripMenuItem;
		private ToolStripMenuItem toolStripMenuItemCustom;
		private ToolStripSeparator toolStripSeparator4;
		private CommandDemo.UICommand uiCommandCanPrint;
		private ToolStripMenuItem updatesToolStripMenuItem;
		private ToolStripMenuItem toolStripMenuItemCheckForUpdates;
		private ToolStripMenuItem toolStripMenuItemCheckForBetaUpdates;
		private ToolTip ttToolStripTop = new ToolTip();
		#endregion


		// Clean up any resources being used.
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}

			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.OpenDialog = new System.Windows.Forms.OpenFileDialog();
			this.SaveDialog = new System.Windows.Forms.SaveFileDialog();
			this.uiCultureChanger = new MLifter.Components.UICultureChanger();
			this.toolStripTop = new System.Windows.Forms.ToolStrip();
			this.toolStripButtonNew = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonOpen = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonPrint = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripButtonNewCard = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonMaintain = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.slideShowToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonStatistics = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonBoxSize = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonOptions = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripButtonCharMap = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonHelp = new System.Windows.Forms.ToolStripButton();
			this.toolStripDropDownButtonChangeLearnModus = new System.Windows.Forms.ToolStripDropDownButton();
			this.toolStripMenuItemStandard = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemMultipleChoice = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemSentences = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemListeningComprehension = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemImageRecognition = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItemCustom = new System.Windows.Forms.ToolStripMenuItem();
			this.MainHelp = new System.Windows.Forms.HelpProvider();
			this.learningWindow = new MLifter.Controls.LearningWindow.LearningWindow();
			this.menuStripMain = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
			this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.printToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
			this.propertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparatorAboveRecentFiles = new System.Windows.Forms.ToolStripSeparator();
			this.recentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparatorBelowRecentFiles = new System.Windows.Forms.ToolStripSeparator();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.addCardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.maintainCardsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
			this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.fileImportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.clipboardImportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
			this.editChaptersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.learnToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.slideShowModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.selectChaptersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.selectBoxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator16 = new System.Windows.Forms.ToolStripSeparator();
			this.showStatisticsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.restartLearningToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.showBoxSizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator17 = new System.Windows.Forms.ToolStripSeparator();
			this.learningOptionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.characterMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.snoozeModeMemorylifterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator18 = new System.Windows.Forms.ToolStripSeparator();
			this.switchSkinToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.switchLanguageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator22 = new System.Windows.Forms.ToolStripSeparator();
			this.styleEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.changeLMFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.updatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemCheckForUpdates = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemCheckForBetaUpdates = new System.Windows.Forms.ToolStripMenuItem();
			this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.firstStepsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.helpIndexToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.helpSearchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator23 = new System.Windows.Forms.ToolStripSeparator();
			this.homepageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.fAQToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.recommendToAFriendToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator24 = new System.Windows.Forms.ToolStripSeparator();
			this.aboutMemoryLifterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.uiCommandProvider = new CommandDemo.UICommandProvider(this.components);
			this.uiCommandCanPrint = new CommandDemo.UICommand(this.components);
			this.uiCommandEditCards = new CommandDemo.UICommand(this.components);
			this.uiCommandLMOpenedMenu = new CommandDemo.UICommand(this.components);
			this.uiCommandCanModifySettings = new CommandDemo.UICommand(this.components);
			this.uiCommandCanSaveCopy = new CommandDemo.UICommand(this.components);
			this.uiCommandCanExport = new CommandDemo.UICommand(this.components);
			this.uiCommandCanModifyChapters = new CommandDemo.UICommand(this.components);
			this.menuStripTray = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.continueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
			this.exitToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripDropDownChangeLearnModus = new System.Windows.Forms.ToolStripDropDown();
			this.testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.uiCommandCanModify = new CommandDemo.UICommand(this.components);
			this.TrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
			this.timerSnooze = new System.Windows.Forms.Timer(this.components);
			this.characterMapComponent = new MLifter.Controls.LearningWindow.CharacterMapComponent(this.components);
			this.toolStripTop.SuspendLayout();
			this.menuStripMain.SuspendLayout();
			this.menuStripTray.SuspendLayout();
			this.SuspendLayout();
			// 
			// OpenDialog
			// 
			resources.ApplyResources(this.OpenDialog, "OpenDialog");
			// 
			// SaveDialog
			// 
			this.SaveDialog.DefaultExt = "odx";
			resources.ApplyResources(this.SaveDialog, "SaveDialog");
			// 
			// uiCultureChanger
			// 
			this.uiCultureChanger.ApplyToolTip = true;
			this.uiCultureChanger.AddForm(this);
			// 
			// toolStripTop
			// 
			this.toolStripTop.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStripTop.ImageScalingSize = new System.Drawing.Size(22, 22);
			this.toolStripTop.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.toolStripButtonNew,
			this.toolStripButtonOpen,
			this.toolStripButtonPrint,
			this.toolStripSeparator1,
			this.toolStripButtonNewCard,
			this.toolStripButtonMaintain,
			this.toolStripSeparator2,
			this.slideShowToolStripButton,
			this.toolStripButtonStatistics,
			this.toolStripButtonBoxSize,
			this.toolStripButtonOptions,
			this.toolStripSeparator3,
			this.toolStripButtonCharMap,
			this.toolStripButtonHelp,
			this.toolStripDropDownButtonChangeLearnModus});
			resources.ApplyResources(this.toolStripTop, "toolStripTop");
			this.toolStripTop.Name = "toolStripTop";
			this.toolStripTop.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
			this.uiCommandProvider.SetUICommand(this.toolStripTop, null);
			// 
			// toolStripButtonNew
			// 
			this.toolStripButtonNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(this.toolStripButtonNew, "toolStripButtonNew");
			this.toolStripButtonNew.Name = "toolStripButtonNew";
			this.uiCommandProvider.SetUICommand(this.toolStripButtonNew, null);
			this.toolStripButtonNew.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
			// 
			// toolStripButtonOpen
			// 
			this.toolStripButtonOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonOpen.Image = global::MLifter.Properties.Resources.documentOpen;
			this.toolStripButtonOpen.Name = "toolStripButtonOpen";
			resources.ApplyResources(this.toolStripButtonOpen, "toolStripButtonOpen");
			this.uiCommandProvider.SetUICommand(this.toolStripButtonOpen, null);
			this.toolStripButtonOpen.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
			// 
			// toolStripButtonPrint
			// 
			this.toolStripButtonPrint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(this.toolStripButtonPrint, "toolStripButtonPrint");
			this.toolStripButtonPrint.Image = global::MLifter.Properties.Resources.documentPrint;
			this.toolStripButtonPrint.Name = "toolStripButtonPrint";
			this.uiCommandProvider.SetUICommand(this.toolStripButtonPrint, this.uiCommandCanPrint);
			this.toolStripButtonPrint.Click += new System.EventHandler(this.printToolStripMenuItem_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
			this.uiCommandProvider.SetUICommand(this.toolStripSeparator1, null);
			// 
			// toolStripButtonNewCard
			// 
			this.toolStripButtonNewCard.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(this.toolStripButtonNewCard, "toolStripButtonNewCard");
			this.toolStripButtonNewCard.Image = global::MLifter.Properties.Resources.list_add;
			this.toolStripButtonNewCard.Name = "toolStripButtonNewCard";
			this.uiCommandProvider.SetUICommand(this.toolStripButtonNewCard, this.uiCommandEditCards);
			this.toolStripButtonNewCard.Click += new System.EventHandler(this.addCardToolStripMenuItem_Click);
			// 
			// toolStripButtonMaintain
			// 
			this.toolStripButtonMaintain.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(this.toolStripButtonMaintain, "toolStripButtonMaintain");
			this.toolStripButtonMaintain.Image = global::MLifter.Properties.Resources.accessoriesTextEditor;
			this.toolStripButtonMaintain.Name = "toolStripButtonMaintain";
			this.uiCommandProvider.SetUICommand(this.toolStripButtonMaintain, this.uiCommandEditCards);
			this.toolStripButtonMaintain.Click += new System.EventHandler(this.maintainCardsToolStripMenuItem_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
			this.uiCommandProvider.SetUICommand(this.toolStripSeparator2, null);
			// 
			// slideShowToolStripButton
			// 
			this.slideShowToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.slideShowToolStripButton.Image = global::MLifter.Properties.Resources.toolbar_slideshow;
			resources.ApplyResources(this.slideShowToolStripButton, "slideShowToolStripButton");
			this.slideShowToolStripButton.Name = "slideShowToolStripButton";
			this.uiCommandProvider.SetUICommand(this.slideShowToolStripButton, this.uiCommandLMOpenedMenu);
			this.slideShowToolStripButton.Click += new System.EventHandler(this.slideshowModeToolStripMenuItem_Click);
			this.slideShowToolStripButton.MouseLeave += new System.EventHandler(this.slideShowToolStripButton_MouseLeave);
			// 
			// toolStripButtonStatistics
			// 
			this.toolStripButtonStatistics.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonStatistics.Image = global::MLifter.Properties.Resources.officeSpreadsheet;
			this.toolStripButtonStatistics.Name = "toolStripButtonStatistics";
			resources.ApplyResources(this.toolStripButtonStatistics, "toolStripButtonStatistics");
			this.uiCommandProvider.SetUICommand(this.toolStripButtonStatistics, this.uiCommandLMOpenedMenu);
			this.toolStripButtonStatistics.Click += new System.EventHandler(this.showStatisticsToolStripMenuItem_Click);
			// 
			// toolStripButtonBoxSize
			// 
			this.toolStripButtonBoxSize.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonBoxSize.Image = global::MLifter.Properties.Resources.systemFileManager;
			this.toolStripButtonBoxSize.Name = "toolStripButtonBoxSize";
			resources.ApplyResources(this.toolStripButtonBoxSize, "toolStripButtonBoxSize");
			this.uiCommandProvider.SetUICommand(this.toolStripButtonBoxSize, this.uiCommandLMOpenedMenu);
			this.toolStripButtonBoxSize.Click += new System.EventHandler(this.showBoxSizeToolStripMenuItem_Click);
			// 
			// toolStripButtonOptions
			// 
			this.toolStripButtonOptions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(this.toolStripButtonOptions, "toolStripButtonOptions");
			this.toolStripButtonOptions.Image = global::MLifter.Properties.Resources.preferencesSystem;
			this.toolStripButtonOptions.Name = "toolStripButtonOptions";
			this.uiCommandProvider.SetUICommand(this.toolStripButtonOptions, this.uiCommandCanModifySettings);
			this.toolStripButtonOptions.Click += new System.EventHandler(this.learningOptionsToolStripMenuItem_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
			this.uiCommandProvider.SetUICommand(this.toolStripSeparator3, null);
			// 
			// toolStripButtonCharMap
			// 
			this.toolStripButtonCharMap.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonCharMap.Image = global::MLifter.Properties.Resources.accessoriesCharacterMap;
			this.toolStripButtonCharMap.Name = "toolStripButtonCharMap";
			resources.ApplyResources(this.toolStripButtonCharMap, "toolStripButtonCharMap");
			this.uiCommandProvider.SetUICommand(this.toolStripButtonCharMap, this.uiCommandLMOpenedMenu);
			this.toolStripButtonCharMap.Click += new System.EventHandler(this.characterMapToolStripMenuItem_Click);
			// 
			// toolStripButtonHelp
			// 
			this.toolStripButtonHelp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonHelp.Image = global::MLifter.Properties.Resources.helpBrowser;
			this.toolStripButtonHelp.Name = "toolStripButtonHelp";
			resources.ApplyResources(this.toolStripButtonHelp, "toolStripButtonHelp");
			this.uiCommandProvider.SetUICommand(this.toolStripButtonHelp, null);
			this.toolStripButtonHelp.Click += new System.EventHandler(this.memoryLifterHelpToolStripMenuItem_Click);
			// 
			// toolStripDropDownButtonChangeLearnModus
			// 
			this.toolStripDropDownButtonChangeLearnModus.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.toolStripDropDownButtonChangeLearnModus.DropDownDirection = System.Windows.Forms.ToolStripDropDownDirection.BelowLeft;
			this.toolStripDropDownButtonChangeLearnModus.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.toolStripMenuItemStandard,
			this.toolStripMenuItemMultipleChoice,
			this.toolStripMenuItemSentences,
			this.toolStripMenuItemListeningComprehension,
			this.toolStripMenuItemImageRecognition,
			this.toolStripSeparator4,
			this.toolStripMenuItemCustom});
			this.toolStripDropDownButtonChangeLearnModus.Image = global::MLifter.Properties.Resources.gadu_grau;
			resources.ApplyResources(this.toolStripDropDownButtonChangeLearnModus, "toolStripDropDownButtonChangeLearnModus");
			this.toolStripDropDownButtonChangeLearnModus.Name = "toolStripDropDownButtonChangeLearnModus";
			this.uiCommandProvider.SetUICommand(this.toolStripDropDownButtonChangeLearnModus, this.uiCommandLMOpenedMenu);
			this.toolStripDropDownButtonChangeLearnModus.DropDownClosed += new System.EventHandler(this.toolStripDropDownButtonChangeLearnModus_DropDownClosed);
			// 
			// toolStripMenuItemStandard
			// 
			resources.ApplyResources(this.toolStripMenuItemStandard, "toolStripMenuItemStandard");
			this.toolStripMenuItemStandard.Image = global::MLifter.Properties.Resources.gYellow;
			this.toolStripMenuItemStandard.Name = "toolStripMenuItemStandard";
			this.uiCommandProvider.SetUICommand(this.toolStripMenuItemStandard, this.uiCommandLMOpenedMenu);
			this.toolStripMenuItemStandard.Click += new System.EventHandler(this.toolStripMenuItemStandard_Click);
			// 
			// toolStripMenuItemMultipleChoice
			// 
			resources.ApplyResources(this.toolStripMenuItemMultipleChoice, "toolStripMenuItemMultipleChoice");
			this.toolStripMenuItemMultipleChoice.Image = global::MLifter.Properties.Resources.gPurple;
			this.toolStripMenuItemMultipleChoice.Name = "toolStripMenuItemMultipleChoice";
			this.uiCommandProvider.SetUICommand(this.toolStripMenuItemMultipleChoice, this.uiCommandLMOpenedMenu);
			this.toolStripMenuItemMultipleChoice.Click += new System.EventHandler(this.toolStripMenuItemMultipleChoice_Click);
			// 
			// toolStripMenuItemSentences
			// 
			this.toolStripMenuItemSentences.Image = global::MLifter.Properties.Resources.gBlue;
			resources.ApplyResources(this.toolStripMenuItemSentences, "toolStripMenuItemSentences");
			this.toolStripMenuItemSentences.Name = "toolStripMenuItemSentences";
			this.uiCommandProvider.SetUICommand(this.toolStripMenuItemSentences, this.uiCommandLMOpenedMenu);
			this.toolStripMenuItemSentences.Click += new System.EventHandler(this.toolStripMenuItemSentences_Click);
			// 
			// toolStripMenuItemListeningComprehension
			// 
			this.toolStripMenuItemListeningComprehension.Image = global::MLifter.Properties.Resources.gOrange;
			resources.ApplyResources(this.toolStripMenuItemListeningComprehension, "toolStripMenuItemListeningComprehension");
			this.toolStripMenuItemListeningComprehension.Name = "toolStripMenuItemListeningComprehension";
			this.uiCommandProvider.SetUICommand(this.toolStripMenuItemListeningComprehension, this.uiCommandLMOpenedMenu);
			this.toolStripMenuItemListeningComprehension.Click += new System.EventHandler(this.toolStripMenuItemListeningComprehension_Click);
			// 
			// toolStripMenuItemImageRecognition
			// 
			this.toolStripMenuItemImageRecognition.Image = global::MLifter.Properties.Resources.gGreen;
			resources.ApplyResources(this.toolStripMenuItemImageRecognition, "toolStripMenuItemImageRecognition");
			this.toolStripMenuItemImageRecognition.Name = "toolStripMenuItemImageRecognition";
			this.uiCommandProvider.SetUICommand(this.toolStripMenuItemImageRecognition, this.uiCommandLMOpenedMenu);
			this.toolStripMenuItemImageRecognition.Click += new System.EventHandler(this.toolStripMenuItemImageRecognition_Click);
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
			this.uiCommandProvider.SetUICommand(this.toolStripSeparator4, null);
			// 
			// toolStripMenuItemCustom
			// 
			this.toolStripMenuItemCustom.Image = global::MLifter.Properties.Resources.menu_preferences;
			resources.ApplyResources(this.toolStripMenuItemCustom, "toolStripMenuItemCustom");
			this.toolStripMenuItemCustom.Name = "toolStripMenuItemCustom";
			this.uiCommandProvider.SetUICommand(this.toolStripMenuItemCustom, null);
			this.toolStripMenuItemCustom.Click += new System.EventHandler(this.toolStripMenuItemCustom_Click);
			// 
			// learningWindow
			// 
			this.learningWindow.BackColor = System.Drawing.SystemColors.Control;
			resources.ApplyResources(this.learningWindow, "learningWindow");
			this.learningWindow.Name = "learningWindow";
			this.uiCommandProvider.SetUICommand(this.learningWindow, null);
			this.learningWindow.FileDropped += new MLifter.Controls.LearningWindow.FileDroppedEventHandler(this.learningWindow_FileDropped);
			this.learningWindow.BrowserKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.learningWindow_BrowserKeyDown);
			// 
			// menuStripMain
			// 
			this.menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.fileToolStripMenuItem,
			this.editToolStripMenuItem,
			this.learnToolStripMenuItem,
			this.toolsToolStripMenuItem,
			this.helpToolStripMenuItem});
			resources.ApplyResources(this.menuStripMain, "menuStripMain");
			this.menuStripMain.Name = "menuStripMain";
			this.menuStripMain.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
			this.uiCommandProvider.SetUICommand(this.menuStripMain, null);
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.newToolStripMenuItem,
			this.openToolStripMenuItem,
			this.closeToolStripMenuItem,
			this.toolStripSeparator5,
			this.saveAsToolStripMenuItem,
			this.printToolStripMenuItem,
			this.toolStripSeparator10,
			this.propertiesToolStripMenuItem,
			this.toolStripSeparatorAboveRecentFiles,
			this.recentToolStripMenuItem,
			this.toolStripSeparatorBelowRecentFiles,
			this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			resources.ApplyResources(this.fileToolStripMenuItem, "fileToolStripMenuItem");
			this.uiCommandProvider.SetUICommand(this.fileToolStripMenuItem, null);
			// 
			// newToolStripMenuItem
			// 
			this.newToolStripMenuItem.Image = global::MLifter.Properties.Resources.menu_new;
			this.newToolStripMenuItem.Name = "newToolStripMenuItem";
			resources.ApplyResources(this.newToolStripMenuItem, "newToolStripMenuItem");
			this.uiCommandProvider.SetUICommand(this.newToolStripMenuItem, null);
			this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
			// 
			// openToolStripMenuItem
			// 
			this.openToolStripMenuItem.Image = global::MLifter.Properties.Resources.menu_open;
			this.openToolStripMenuItem.Name = "openToolStripMenuItem";
			resources.ApplyResources(this.openToolStripMenuItem, "openToolStripMenuItem");
			this.uiCommandProvider.SetUICommand(this.openToolStripMenuItem, null);
			this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
			// 
			// closeToolStripMenuItem
			// 
			this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
			resources.ApplyResources(this.closeToolStripMenuItem, "closeToolStripMenuItem");
			this.uiCommandProvider.SetUICommand(this.closeToolStripMenuItem, this.uiCommandLMOpenedMenu);
			this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
			// 
			// toolStripSeparator5
			// 
			this.toolStripSeparator5.Name = "toolStripSeparator5";
			resources.ApplyResources(this.toolStripSeparator5, "toolStripSeparator5");
			this.uiCommandProvider.SetUICommand(this.toolStripSeparator5, null);
			// 
			// saveAsToolStripMenuItem
			// 
			resources.ApplyResources(this.saveAsToolStripMenuItem, "saveAsToolStripMenuItem");
			this.saveAsToolStripMenuItem.Image = global::MLifter.Properties.Resources.menu_saveAs;
			this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
			this.uiCommandProvider.SetUICommand(this.saveAsToolStripMenuItem, this.uiCommandCanSaveCopy);
			this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
			// 
			// printToolStripMenuItem
			// 
			resources.ApplyResources(this.printToolStripMenuItem, "printToolStripMenuItem");
			this.printToolStripMenuItem.Image = global::MLifter.Properties.Resources.menu_print;
			this.printToolStripMenuItem.Name = "printToolStripMenuItem";
			this.uiCommandProvider.SetUICommand(this.printToolStripMenuItem, this.uiCommandCanPrint);
			this.printToolStripMenuItem.Click += new System.EventHandler(this.printToolStripMenuItem_Click);
			// 
			// toolStripSeparator10
			// 
			this.toolStripSeparator10.Name = "toolStripSeparator10";
			resources.ApplyResources(this.toolStripSeparator10, "toolStripSeparator10");
			this.uiCommandProvider.SetUICommand(this.toolStripSeparator10, null);
			// 
			// propertiesToolStripMenuItem
			// 
			this.propertiesToolStripMenuItem.Name = "propertiesToolStripMenuItem";
			resources.ApplyResources(this.propertiesToolStripMenuItem, "propertiesToolStripMenuItem");
			this.uiCommandProvider.SetUICommand(this.propertiesToolStripMenuItem, this.uiCommandLMOpenedMenu);
			this.propertiesToolStripMenuItem.Click += new System.EventHandler(this.propertiesToolStripMenuItem_Click);
			// 
			// toolStripSeparatorAboveRecentFiles
			// 
			this.toolStripSeparatorAboveRecentFiles.Name = "toolStripSeparatorAboveRecentFiles";
			resources.ApplyResources(this.toolStripSeparatorAboveRecentFiles, "toolStripSeparatorAboveRecentFiles");
			this.uiCommandProvider.SetUICommand(this.toolStripSeparatorAboveRecentFiles, null);
			// 
			// recentToolStripMenuItem
			// 
			this.recentToolStripMenuItem.Name = "recentToolStripMenuItem";
			resources.ApplyResources(this.recentToolStripMenuItem, "recentToolStripMenuItem");
			this.uiCommandProvider.SetUICommand(this.recentToolStripMenuItem, null);
			// 
			// toolStripSeparatorBelowRecentFiles
			// 
			this.toolStripSeparatorBelowRecentFiles.Name = "toolStripSeparatorBelowRecentFiles";
			resources.ApplyResources(this.toolStripSeparatorBelowRecentFiles, "toolStripSeparatorBelowRecentFiles");
			this.uiCommandProvider.SetUICommand(this.toolStripSeparatorBelowRecentFiles, null);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Image = global::MLifter.Properties.Resources.menu_exit;
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			resources.ApplyResources(this.exitToolStripMenuItem, "exitToolStripMenuItem");
			this.uiCommandProvider.SetUICommand(this.exitToolStripMenuItem, null);
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// editToolStripMenuItem
			// 
			this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.addCardToolStripMenuItem,
			this.maintainCardsToolStripMenuItem,
			this.toolStripSeparator6,
			this.importToolStripMenuItem,
			this.exportToolStripMenuItem,
			this.toolStripSeparator14,
			this.editChaptersToolStripMenuItem});
			this.editToolStripMenuItem.Name = "editToolStripMenuItem";
			resources.ApplyResources(this.editToolStripMenuItem, "editToolStripMenuItem");
			this.uiCommandProvider.SetUICommand(this.editToolStripMenuItem, this.uiCommandLMOpenedMenu);
			// 
			// addCardToolStripMenuItem
			// 
			resources.ApplyResources(this.addCardToolStripMenuItem, "addCardToolStripMenuItem");
			this.addCardToolStripMenuItem.Image = global::MLifter.Properties.Resources.menu_add;
			this.addCardToolStripMenuItem.Name = "addCardToolStripMenuItem";
			this.uiCommandProvider.SetUICommand(this.addCardToolStripMenuItem, this.uiCommandEditCards);
			this.addCardToolStripMenuItem.Click += new System.EventHandler(this.addCardToolStripMenuItem_Click);
			// 
			// maintainCardsToolStripMenuItem
			// 
			resources.ApplyResources(this.maintainCardsToolStripMenuItem, "maintainCardsToolStripMenuItem");
			this.maintainCardsToolStripMenuItem.Image = global::MLifter.Properties.Resources.menu_addEditCards;
			this.maintainCardsToolStripMenuItem.Name = "maintainCardsToolStripMenuItem";
			this.uiCommandProvider.SetUICommand(this.maintainCardsToolStripMenuItem, this.uiCommandEditCards);
			this.maintainCardsToolStripMenuItem.Click += new System.EventHandler(this.maintainCardsToolStripMenuItem_Click);
			// 
			// toolStripSeparator6
			// 
			this.toolStripSeparator6.Name = "toolStripSeparator6";
			resources.ApplyResources(this.toolStripSeparator6, "toolStripSeparator6");
			this.uiCommandProvider.SetUICommand(this.toolStripSeparator6, null);
			// 
			// importToolStripMenuItem
			// 
			this.importToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.fileImportToolStripMenuItem,
			this.clipboardImportToolStripMenuItem});
			resources.ApplyResources(this.importToolStripMenuItem, "importToolStripMenuItem");
			this.importToolStripMenuItem.Image = global::MLifter.Properties.Resources.menu_import;
			this.importToolStripMenuItem.Name = "importToolStripMenuItem";
			this.uiCommandProvider.SetUICommand(this.importToolStripMenuItem, this.uiCommandEditCards);
			// 
			// fileImportToolStripMenuItem
			// 
			resources.ApplyResources(this.fileImportToolStripMenuItem, "fileImportToolStripMenuItem");
			this.fileImportToolStripMenuItem.Name = "fileImportToolStripMenuItem";
			this.uiCommandProvider.SetUICommand(this.fileImportToolStripMenuItem, this.uiCommandEditCards);
			this.fileImportToolStripMenuItem.Click += new System.EventHandler(this.fileImportToolStripMenuItem_Click);
			// 
			// clipboardImportToolStripMenuItem
			// 
			resources.ApplyResources(this.clipboardImportToolStripMenuItem, "clipboardImportToolStripMenuItem");
			this.clipboardImportToolStripMenuItem.Name = "clipboardImportToolStripMenuItem";
			this.uiCommandProvider.SetUICommand(this.clipboardImportToolStripMenuItem, this.uiCommandEditCards);
			this.clipboardImportToolStripMenuItem.Click += new System.EventHandler(this.clipboardImportToolStripMenuItem_Click);
			// 
			// exportToolStripMenuItem
			// 
			resources.ApplyResources(this.exportToolStripMenuItem, "exportToolStripMenuItem");
			this.exportToolStripMenuItem.Image = global::MLifter.Properties.Resources.menu_export;
			this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
			this.uiCommandProvider.SetUICommand(this.exportToolStripMenuItem, this.uiCommandCanExport);
			this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportToolStripMenuItem_Click);
			// 
			// toolStripSeparator14
			// 
			this.toolStripSeparator14.Name = "toolStripSeparator14";
			resources.ApplyResources(this.toolStripSeparator14, "toolStripSeparator14");
			this.uiCommandProvider.SetUICommand(this.toolStripSeparator14, null);
			// 
			// editChaptersToolStripMenuItem
			// 
			resources.ApplyResources(this.editChaptersToolStripMenuItem, "editChaptersToolStripMenuItem");
			this.editChaptersToolStripMenuItem.Image = global::MLifter.Properties.Resources.menu_chapters;
			this.editChaptersToolStripMenuItem.Name = "editChaptersToolStripMenuItem";
			this.uiCommandProvider.SetUICommand(this.editChaptersToolStripMenuItem, this.uiCommandCanModifyChapters);
			this.editChaptersToolStripMenuItem.Click += new System.EventHandler(this.editChaptersToolStripMenuItem1_Click);
			// 
			// learnToolStripMenuItem
			// 
			this.learnToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.slideShowModeToolStripMenuItem,
			this.selectChaptersToolStripMenuItem,
			this.selectBoxToolStripMenuItem,
			this.toolStripSeparator16,
			this.showStatisticsToolStripMenuItem,
			this.restartLearningToolStripMenuItem,
			this.showBoxSizeToolStripMenuItem,
			this.toolStripSeparator17,
			this.learningOptionsToolStripMenuItem});
			this.learnToolStripMenuItem.Name = "learnToolStripMenuItem";
			resources.ApplyResources(this.learnToolStripMenuItem, "learnToolStripMenuItem");
			this.uiCommandProvider.SetUICommand(this.learnToolStripMenuItem, this.uiCommandLMOpenedMenu);
			// 
			// slideShowModeToolStripMenuItem
			// 
			this.slideShowModeToolStripMenuItem.Image = global::MLifter.Properties.Resources.menu_slideshow;
			this.slideShowModeToolStripMenuItem.Name = "slideShowModeToolStripMenuItem";
			resources.ApplyResources(this.slideShowModeToolStripMenuItem, "slideShowModeToolStripMenuItem");
			this.uiCommandProvider.SetUICommand(this.slideShowModeToolStripMenuItem, this.uiCommandLMOpenedMenu);
			this.slideShowModeToolStripMenuItem.Click += new System.EventHandler(this.slideshowModeToolStripMenuItem_Click);
			// 
			// selectChaptersToolStripMenuItem
			// 
			this.selectChaptersToolStripMenuItem.Image = global::MLifter.Properties.Resources.menu_chapters;
			this.selectChaptersToolStripMenuItem.Name = "selectChaptersToolStripMenuItem";
			resources.ApplyResources(this.selectChaptersToolStripMenuItem, "selectChaptersToolStripMenuItem");
			this.uiCommandProvider.SetUICommand(this.selectChaptersToolStripMenuItem, null);
			this.selectChaptersToolStripMenuItem.Click += new System.EventHandler(this.selectChaptersToolStripMenuItem_Click);
			// 
			// selectBoxToolStripMenuItem
			// 
			this.selectBoxToolStripMenuItem.Image = global::MLifter.Properties.Resources.menu_boxsetup;
			this.selectBoxToolStripMenuItem.Name = "selectBoxToolStripMenuItem";
			resources.ApplyResources(this.selectBoxToolStripMenuItem, "selectBoxToolStripMenuItem");
			this.uiCommandProvider.SetUICommand(this.selectBoxToolStripMenuItem, null);
			// 
			// toolStripSeparator16
			// 
			this.toolStripSeparator16.Name = "toolStripSeparator16";
			resources.ApplyResources(this.toolStripSeparator16, "toolStripSeparator16");
			this.uiCommandProvider.SetUICommand(this.toolStripSeparator16, null);
			// 
			// showStatisticsToolStripMenuItem
			// 
			this.showStatisticsToolStripMenuItem.Image = global::MLifter.Properties.Resources.menu_statistics;
			this.showStatisticsToolStripMenuItem.Name = "showStatisticsToolStripMenuItem";
			resources.ApplyResources(this.showStatisticsToolStripMenuItem, "showStatisticsToolStripMenuItem");
			this.uiCommandProvider.SetUICommand(this.showStatisticsToolStripMenuItem, null);
			this.showStatisticsToolStripMenuItem.Click += new System.EventHandler(this.showStatisticsToolStripMenuItem_Click);
			// 
			// restartLearningToolStripMenuItem
			// 
			this.restartLearningToolStripMenuItem.Name = "restartLearningToolStripMenuItem";
			resources.ApplyResources(this.restartLearningToolStripMenuItem, "restartLearningToolStripMenuItem");
			this.uiCommandProvider.SetUICommand(this.restartLearningToolStripMenuItem, null);
			this.restartLearningToolStripMenuItem.Click += new System.EventHandler(this.restartLearningToolStripMenuItem_Click);
			// 
			// showBoxSizeToolStripMenuItem
			// 
			this.showBoxSizeToolStripMenuItem.Image = global::MLifter.Properties.Resources.menu_boxsetup;
			this.showBoxSizeToolStripMenuItem.Name = "showBoxSizeToolStripMenuItem";
			resources.ApplyResources(this.showBoxSizeToolStripMenuItem, "showBoxSizeToolStripMenuItem");
			this.uiCommandProvider.SetUICommand(this.showBoxSizeToolStripMenuItem, null);
			this.showBoxSizeToolStripMenuItem.Click += new System.EventHandler(this.showBoxSizeToolStripMenuItem_Click);
			// 
			// toolStripSeparator17
			// 
			this.toolStripSeparator17.Name = "toolStripSeparator17";
			resources.ApplyResources(this.toolStripSeparator17, "toolStripSeparator17");
			this.uiCommandProvider.SetUICommand(this.toolStripSeparator17, null);
			// 
			// learningOptionsToolStripMenuItem
			// 
			this.learningOptionsToolStripMenuItem.Image = global::MLifter.Properties.Resources.menu_preferences;
			this.learningOptionsToolStripMenuItem.Name = "learningOptionsToolStripMenuItem";
			resources.ApplyResources(this.learningOptionsToolStripMenuItem, "learningOptionsToolStripMenuItem");
			this.uiCommandProvider.SetUICommand(this.learningOptionsToolStripMenuItem, this.uiCommandLMOpenedMenu);
			this.learningOptionsToolStripMenuItem.Click += new System.EventHandler(this.learningOptionsToolStripMenuItem_Click);
			// 
			// toolsToolStripMenuItem
			// 
			this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.characterMapToolStripMenuItem,
			this.snoozeModeMemorylifterToolStripMenuItem,
			this.toolStripSeparator18,
			this.switchSkinToolStripMenuItem,
			this.switchLanguageToolStripMenuItem,
			this.toolStripSeparator22,
			this.styleEditorToolStripMenuItem,
			this.changeLMFolderToolStripMenuItem,
			this.updatesToolStripMenuItem});
			this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
			resources.ApplyResources(this.toolsToolStripMenuItem, "toolsToolStripMenuItem");
			this.uiCommandProvider.SetUICommand(this.toolsToolStripMenuItem, null);
			// 
			// characterMapToolStripMenuItem
			// 
			this.characterMapToolStripMenuItem.Image = global::MLifter.Properties.Resources.menu_characterMap;
			this.characterMapToolStripMenuItem.Name = "characterMapToolStripMenuItem";
			resources.ApplyResources(this.characterMapToolStripMenuItem, "characterMapToolStripMenuItem");
			this.uiCommandProvider.SetUICommand(this.characterMapToolStripMenuItem, this.uiCommandLMOpenedMenu);
			this.characterMapToolStripMenuItem.Click += new System.EventHandler(this.characterMapToolStripMenuItem_Click);
			// 
			// snoozeModeMemorylifterToolStripMenuItem
			// 
			this.snoozeModeMemorylifterToolStripMenuItem.Image = global::MLifter.Properties.Resources.menu_snooze;
			this.snoozeModeMemorylifterToolStripMenuItem.Name = "snoozeModeMemorylifterToolStripMenuItem";
			resources.ApplyResources(this.snoozeModeMemorylifterToolStripMenuItem, "snoozeModeMemorylifterToolStripMenuItem");
			this.uiCommandProvider.SetUICommand(this.snoozeModeMemorylifterToolStripMenuItem, this.uiCommandLMOpenedMenu);
			this.snoozeModeMemorylifterToolStripMenuItem.Click += new System.EventHandler(this.snoozeModeMemorylifterToolStripMenuItem_Click);
			// 
			// toolStripSeparator18
			// 
			this.toolStripSeparator18.Name = "toolStripSeparator18";
			resources.ApplyResources(this.toolStripSeparator18, "toolStripSeparator18");
			this.uiCommandProvider.SetUICommand(this.toolStripSeparator18, null);
			// 
			// switchSkinToolStripMenuItem
			// 
			this.switchSkinToolStripMenuItem.Name = "switchSkinToolStripMenuItem";
			resources.ApplyResources(this.switchSkinToolStripMenuItem, "switchSkinToolStripMenuItem");
			this.uiCommandProvider.SetUICommand(this.switchSkinToolStripMenuItem, this.uiCommandLMOpenedMenu);
			// 
			// switchLanguageToolStripMenuItem
			// 
			this.switchLanguageToolStripMenuItem.Name = "switchLanguageToolStripMenuItem";
			resources.ApplyResources(this.switchLanguageToolStripMenuItem, "switchLanguageToolStripMenuItem");
			this.uiCommandProvider.SetUICommand(this.switchLanguageToolStripMenuItem, null);
			// 
			// toolStripSeparator22
			// 
			this.toolStripSeparator22.Name = "toolStripSeparator22";
			resources.ApplyResources(this.toolStripSeparator22, "toolStripSeparator22");
			this.uiCommandProvider.SetUICommand(this.toolStripSeparator22, null);
			// 
			// styleEditorToolStripMenuItem
			// 
			this.styleEditorToolStripMenuItem.Image = global::MLifter.Properties.Resources.menu_editStyle;
			this.styleEditorToolStripMenuItem.Name = "styleEditorToolStripMenuItem";
			resources.ApplyResources(this.styleEditorToolStripMenuItem, "styleEditorToolStripMenuItem");
			this.uiCommandProvider.SetUICommand(this.styleEditorToolStripMenuItem, this.uiCommandLMOpenedMenu);
			this.styleEditorToolStripMenuItem.Click += new System.EventHandler(this.styleEditorToolStripMenuItem_Click);
			// 
			// changeLMFolderToolStripMenuItem
			// 
			this.changeLMFolderToolStripMenuItem.Name = "changeLMFolderToolStripMenuItem";
			resources.ApplyResources(this.changeLMFolderToolStripMenuItem, "changeLMFolderToolStripMenuItem");
			this.uiCommandProvider.SetUICommand(this.changeLMFolderToolStripMenuItem, null);
			this.changeLMFolderToolStripMenuItem.Click += new System.EventHandler(this.changeLMFolderToolStripMenuItem_Click);
			// 
			// updatesToolStripMenuItem
			// 
			this.updatesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.toolStripMenuItemCheckForUpdates,
			this.toolStripMenuItemCheckForBetaUpdates});
			this.updatesToolStripMenuItem.Name = "updatesToolStripMenuItem";
			resources.ApplyResources(this.updatesToolStripMenuItem, "updatesToolStripMenuItem");
			this.uiCommandProvider.SetUICommand(this.updatesToolStripMenuItem, null);
			// 
			// toolStripMenuItemCheckForUpdates
			// 
			this.toolStripMenuItemCheckForUpdates.Name = "toolStripMenuItemCheckForUpdates";
			resources.ApplyResources(this.toolStripMenuItemCheckForUpdates, "toolStripMenuItemCheckForUpdates");
			this.uiCommandProvider.SetUICommand(this.toolStripMenuItemCheckForUpdates, null);
			this.toolStripMenuItemCheckForUpdates.Click += new System.EventHandler(this.toolStripMenuItemCheckForUpdates_Click);
			// 
			// toolStripMenuItemCheckForBetaUpdates
			// 
			this.toolStripMenuItemCheckForBetaUpdates.Name = "toolStripMenuItemCheckForBetaUpdates";
			resources.ApplyResources(this.toolStripMenuItemCheckForBetaUpdates, "toolStripMenuItemCheckForBetaUpdates");
			this.uiCommandProvider.SetUICommand(this.toolStripMenuItemCheckForBetaUpdates, null);
			this.toolStripMenuItemCheckForBetaUpdates.Click += new System.EventHandler(this.toolStripMenuItemCheckForBetaUpdates_Click);
			// 
			// helpToolStripMenuItem
			// 
			this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.firstStepsToolStripMenuItem,
			this.helpIndexToolStripMenuItem,
			this.helpSearchToolStripMenuItem,
			this.toolStripSeparator23,
			this.homepageToolStripMenuItem,
			this.fAQToolStripMenuItem,
			this.recommendToAFriendToolStripMenuItem,
			this.toolStripSeparator24,
			this.aboutMemoryLifterToolStripMenuItem});
			this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			resources.ApplyResources(this.helpToolStripMenuItem, "helpToolStripMenuItem");
			this.uiCommandProvider.SetUICommand(this.helpToolStripMenuItem, null);
			// 
			// firstStepsToolStripMenuItem
			// 
			this.firstStepsToolStripMenuItem.Image = global::MLifter.Properties.Resources.dialogInformation;
			this.firstStepsToolStripMenuItem.Name = "firstStepsToolStripMenuItem";
			resources.ApplyResources(this.firstStepsToolStripMenuItem, "firstStepsToolStripMenuItem");
			this.uiCommandProvider.SetUICommand(this.firstStepsToolStripMenuItem, null);
			this.firstStepsToolStripMenuItem.Click += new System.EventHandler(this.firstStepsToolStripMenuItem_Click);
			// 
			// helpIndexToolStripMenuItem
			// 
			this.helpIndexToolStripMenuItem.Image = global::MLifter.Properties.Resources.menu_help;
			this.helpIndexToolStripMenuItem.Name = "helpIndexToolStripMenuItem";
			resources.ApplyResources(this.helpIndexToolStripMenuItem, "helpIndexToolStripMenuItem");
			this.uiCommandProvider.SetUICommand(this.helpIndexToolStripMenuItem, null);
			this.helpIndexToolStripMenuItem.Click += new System.EventHandler(this.helpIndexToolStripMenuItem_Click);
			// 
			// helpSearchToolStripMenuItem
			// 
			this.helpSearchToolStripMenuItem.Image = global::MLifter.Properties.Resources.menu_search;
			this.helpSearchToolStripMenuItem.Name = "helpSearchToolStripMenuItem";
			resources.ApplyResources(this.helpSearchToolStripMenuItem, "helpSearchToolStripMenuItem");
			this.uiCommandProvider.SetUICommand(this.helpSearchToolStripMenuItem, null);
			this.helpSearchToolStripMenuItem.Click += new System.EventHandler(this.helpSearchToolStripMenuItem_Click);
			// 
			// toolStripSeparator23
			// 
			this.toolStripSeparator23.Name = "toolStripSeparator23";
			resources.ApplyResources(this.toolStripSeparator23, "toolStripSeparator23");
			this.uiCommandProvider.SetUICommand(this.toolStripSeparator23, null);
			// 
			// homepageToolStripMenuItem
			// 
			this.homepageToolStripMenuItem.Image = global::MLifter.Properties.Resources.menu_homepage;
			this.homepageToolStripMenuItem.Name = "homepageToolStripMenuItem";
			resources.ApplyResources(this.homepageToolStripMenuItem, "homepageToolStripMenuItem");
			this.uiCommandProvider.SetUICommand(this.homepageToolStripMenuItem, null);
			this.homepageToolStripMenuItem.Click += new System.EventHandler(this.homepageToolStripMenuItem_Click);
			// 
			// fAQToolStripMenuItem
			// 
			this.fAQToolStripMenuItem.Name = "fAQToolStripMenuItem";
			resources.ApplyResources(this.fAQToolStripMenuItem, "fAQToolStripMenuItem");
			this.uiCommandProvider.SetUICommand(this.fAQToolStripMenuItem, null);
			this.fAQToolStripMenuItem.Click += new System.EventHandler(this.fAQToolStripMenuItem_Click);
			// 
			// recommendToAFriendToolStripMenuItem
			// 
			this.recommendToAFriendToolStripMenuItem.Name = "recommendToAFriendToolStripMenuItem";
			resources.ApplyResources(this.recommendToAFriendToolStripMenuItem, "recommendToAFriendToolStripMenuItem");
			this.uiCommandProvider.SetUICommand(this.recommendToAFriendToolStripMenuItem, null);
			this.recommendToAFriendToolStripMenuItem.Click += new System.EventHandler(this.recommendToAFriendToolStripMenuItem_Click);
			// 
			// toolStripSeparator24
			// 
			this.toolStripSeparator24.Name = "toolStripSeparator24";
			resources.ApplyResources(this.toolStripSeparator24, "toolStripSeparator24");
			this.uiCommandProvider.SetUICommand(this.toolStripSeparator24, null);
			// 
			// aboutMemoryLifterToolStripMenuItem
			// 
			this.aboutMemoryLifterToolStripMenuItem.Image = global::MLifter.Properties.Resources.menu_mlicon;
			this.aboutMemoryLifterToolStripMenuItem.Name = "aboutMemoryLifterToolStripMenuItem";
			resources.ApplyResources(this.aboutMemoryLifterToolStripMenuItem, "aboutMemoryLifterToolStripMenuItem");
			this.uiCommandProvider.SetUICommand(this.aboutMemoryLifterToolStripMenuItem, null);
			this.aboutMemoryLifterToolStripMenuItem.Click += new System.EventHandler(this.aboutMemoryLifterToolStripMenuItem_Click);
			// 
			// uiCommandCanPrint
			// 
			this.uiCommandCanPrint.Enabled = false;
			// 
			// uiCommandEditCards
			// 
			this.uiCommandEditCards.Enabled = false;
			// 
			// uiCommandLMOpenedMenu
			// 
			this.uiCommandLMOpenedMenu.Enabled = true;
			// 
			// uiCommandCanModifySettings
			// 
			this.uiCommandCanModifySettings.Enabled = false;
			// 
			// uiCommandCanSaveCopy
			// 
			this.uiCommandCanSaveCopy.Enabled = false;
			// 
			// uiCommandCanExport
			// 
			this.uiCommandCanExport.Enabled = false;
			// 
			// uiCommandCanModifyChapters
			// 
			this.uiCommandCanModifyChapters.Enabled = false;
			// 
			// menuStripTray
			// 
			this.menuStripTray.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.continueToolStripMenuItem,
			this.toolStripSeparator8,
			this.exitToolStripMenuItem1});
			this.menuStripTray.Name = "menuStripTray";
			resources.ApplyResources(this.menuStripTray, "menuStripTray");
			this.uiCommandProvider.SetUICommand(this.menuStripTray, null);
			this.menuStripTray.Opening += new System.ComponentModel.CancelEventHandler(this.menuStripTray_Opening);
			// 
			// continueToolStripMenuItem
			// 
			resources.ApplyResources(this.continueToolStripMenuItem, "continueToolStripMenuItem");
			this.continueToolStripMenuItem.Name = "continueToolStripMenuItem";
			this.uiCommandProvider.SetUICommand(this.continueToolStripMenuItem, null);
			this.continueToolStripMenuItem.Click += new System.EventHandler(this.continueToolStripMenuItem_Click);
			// 
			// toolStripSeparator8
			// 
			this.toolStripSeparator8.Name = "toolStripSeparator8";
			resources.ApplyResources(this.toolStripSeparator8, "toolStripSeparator8");
			this.uiCommandProvider.SetUICommand(this.toolStripSeparator8, null);
			// 
			// exitToolStripMenuItem1
			// 
			this.exitToolStripMenuItem1.Name = "exitToolStripMenuItem1";
			resources.ApplyResources(this.exitToolStripMenuItem1, "exitToolStripMenuItem1");
			this.uiCommandProvider.SetUICommand(this.exitToolStripMenuItem1, null);
			this.exitToolStripMenuItem1.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// toolStripDropDownChangeLearnModus
			// 
			this.toolStripDropDownChangeLearnModus.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
			this.toolStripDropDownChangeLearnModus.Name = "toolStripDropDownChangeLearnModus";
			resources.ApplyResources(this.toolStripDropDownChangeLearnModus, "toolStripDropDownChangeLearnModus");
			this.uiCommandProvider.SetUICommand(this.toolStripDropDownChangeLearnModus, null);
			// 
			// testToolStripMenuItem
			// 
			this.testToolStripMenuItem.Name = "testToolStripMenuItem";
			resources.ApplyResources(this.testToolStripMenuItem, "testToolStripMenuItem");
			this.uiCommandProvider.SetUICommand(this.testToolStripMenuItem, null);
			// 
			// uiCommandCanModify
			// 
			this.uiCommandCanModify.Enabled = false;
			// 
			// TrayIcon
			// 
			this.TrayIcon.ContextMenuStrip = this.menuStripTray;
			resources.ApplyResources(this.TrayIcon, "TrayIcon");
			this.TrayIcon.DoubleClick += new System.EventHandler(this.TrayIcon_DoubleClick);
			// 
			// timerSnooze
			// 
			this.timerSnooze.Interval = 60000;
			this.timerSnooze.Tick += new System.EventHandler(this.timerSnooze_Tick);
			// 
			// MainForm
			// 
			this.AllowDrop = true;
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.learningWindow);
			this.Controls.Add(this.toolStripTop);
			this.Controls.Add(this.menuStripMain);
			this.MainHelp.SetHelpKeyword(this, resources.GetString("$this.HelpKeyword"));
			this.MainHelp.SetHelpNavigator(this, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("$this.HelpNavigator"))));
			this.IsMdiContainer = true;
			this.Name = "MainForm";
			this.MainHelp.SetShowHelp(this, ((bool)(resources.GetObject("$this.ShowHelp"))));
			this.uiCommandProvider.SetUICommand(this, null);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_Closing);
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.ResizeBegin += new System.EventHandler(this.MainForm_ResizeBegin);
			this.ResizeEnd += new System.EventHandler(this.MainForm_ResizeEnd);
			this.SizeChanged += new System.EventHandler(this.MainForm_SizeChanged);
			this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
			this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
			this.Resize += new System.EventHandler(this.MainForm_Resize);
			this.toolStripTop.ResumeLayout(false);
			this.toolStripTop.PerformLayout();
			this.menuStripMain.ResumeLayout(false);
			this.menuStripMain.PerformLayout();
			this.menuStripTray.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		/// <summary>
		/// Gets or sets the command line param.
		/// </summary>
		/// <value>The command line param.</value>
		/// <remarks>Documented by Dev03, 2009-04-28</remarks>
		internal string CommandLineParam
		{
			get { return commandLineParam; }
			set { commandLineParam = (value != null) ? value.Trim() : String.Empty; }
		}
		/// <summary>
		/// Gets or sets a value indicating whether this instance is in tray.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is in tray; otherwise, <c>false</c>.
		/// </value>
		/// <remarks>Documented by Dev02, 2008-03-18</remarks>
		private bool IsInTray
		{
			get
			{
				return TrayIcon.Visible;
			}
			set
			{
				if (value != IsInTray)
				{
					if (value)
					{
						TrayIcon.Visible = true;
						this.Region = new Region(new Rectangle(0, 0, 0, 0));
						this.ShowInTaskbar = false;
						this.trayWaitMinutesLeft = 0;

						if (LearnLogic.LearningModuleLoaded)
						{
							Random rand_generator = new Random();
							if (LearnLogic.LearningModuleLoaded)
							{
								int high = LearnLogic.Dictionary.Settings.SnoozeOptions.SnoozeHigh.GetValueOrDefault();
								int low = LearnLogic.Dictionary.Settings.SnoozeOptions.SnoozeLow.GetValueOrDefault();

								while (high <= low)
									high++;

								trayWaitMinutesLeft = rand_generator.Next(Math.Abs(high - low)) + low;
							}
							else
								trayWaitMinutesLeft = 0;
							LearnLogic.SaveLearningModule();
							timerSnooze.Start();
						}
					}
					else
					{
						TrayIcon.Visible = false;

						this.Region = null;
						this.ShowInTaskbar = true;

						timerSnooze.Stop();

						PlayStartupEndSound(false, false);
					}
				}
			}
		}


		/// <summary>
		/// Changes the view settings if no dictionary has been loaded.
		/// Disables Toolbar and the CurrentQuestion/CurrentAnswer window.
		/// </summary>
		/// <remarks>Documented by Dev01, 2007-07-20</remarks>
		private void NoDictionaryLoaded()
		{
			this.Text = string.Format("{0} {1}", AssemblyData.Title, AssemblyData.Version);
			DisableMenustrips();

			//reset style in case it was a LM-style
			if (Properties.Settings.Default.CurrentStyle != styleHandler.CurrentStyleName)
			{
				LoadStyle();
				RefreshStyleMenu();
			}
		}

		/// <summary>
		/// Changes the view settings if arrayList dictionary has been loaded.
		/// Enables Toolbar and the question/answer window.
		/// </summary>
		/// <remarks>Documented by Dev01, 2007-07-20</remarks>
		private void DictionaryLoaded()
		{
			this.Text = string.Format("{0} {1}", AssemblyData.Title, AssemblyData.Version) + " - " + LearnLogic.Dictionary.DictionaryDisplayTitle;

			if (LearnLogic.Dictionary.IsFileDB)
			{
				//add dictionary drive to the watched drives
				driveDetector_WatchDevice(new DirectoryInfo(LearnLogic.Dictionary.DictionaryPath).Root.FullName);
			}

			// if (!LearnLogic.CurrentLearningModule.ConnectionString.ProtectedLm || LearnLogic.CurrentLearningModule.ConnectionString.SyncType == SyncType.NotSynchronized)
			Setup.AddRecentLearningModule(LearnLogic.CurrentLearningModule);

			displayLearningModuleOptions();
			EnableMenustrips();
		}

		/// <summary>
		/// Disables the menustrips, in case the LM got closed.
		/// </summary>
		/// <remarks>Documented by Dev04, 2007-07-23</remarks>
		private void DisableMenustrips()
		{
			uiCommandLMOpenedMenu.Enabled = false;
			uiCommandEditCards.Enabled = false;
			uiCommandCanExport.Enabled = false;
			uiCommandCanPrint.Enabled = false;
			uiCommandCanSaveCopy.Enabled = false;
			uiCommandCanModify.Enabled = false;
			uiCommandCanModifyChapters.Enabled = false;
			uiCommandCanModifySettings.Enabled = false;
		}

		/// <summary>
		/// Enables the menustrips, in case a new LM got opened.
		/// </summary>
		/// <remarks>Documented by Dev04, 2007-07-23</remarks>
		private void EnableMenustrips()
		{
			uiCommandLMOpenedMenu.Enabled = true;
			//Set permissions in the interface.
			//file
			uiCommandCanPrint.Enabled = LearnLogic.Dictionary.CanPrint;
			uiCommandCanExport.Enabled = LearnLogic.Dictionary.CanExport;
			uiCommandCanSaveCopy.Enabled = LearnLogic.Dictionary.CanSaveCopy;
			//edit
			uiCommandEditCards.Enabled = LearnLogic.Dictionary.CanModifyCards && LearnLogic.Dictionary.IsWriteable;
			uiCommandCanModify.Enabled = LearnLogic.Dictionary.CanModify && LearnLogic.Dictionary.IsWriteable;
			uiCommandCanModifyChapters.Enabled = LearnLogic.Dictionary.CanModifyChapters && LearnLogic.Dictionary.IsWriteable;
			//learn
			uiCommandCanModifySettings.Enabled = LearnLogic.Dictionary.CanModifySettings;
			//Update
			toolStripMenuItemCheckForBetaUpdates.Checked = Settings.Default.CheckForBetaUpdates;

			UpdateLearningModesDropDownList();
		}

		/// <summary>
		/// Updates the learning modes drop down list.
		/// </summary>
		/// <remarks>Documented by Dev05, 2009-05-22</remarks>
		private void UpdateLearningModesDropDownList()
		{
			if (LearnLogic.Dictionary.AllowedQueryTypes.ImageRecognition.HasValue)
			{
				if (!LearnLogic.Dictionary.AllowedQueryTypes.ImageRecognition.Value && toolStripMenuItemImageRecognition.Checked)
					toolStripMenuItemImageRecognition_Click(null, null);
				toolStripMenuItemImageRecognition.Enabled = LearnLogic.Dictionary.AllowedQueryTypes.ImageRecognition.Value;
			}
			if (LearnLogic.Dictionary.AllowedQueryTypes.ListeningComprehension.HasValue)
			{
				if (!LearnLogic.Dictionary.AllowedQueryTypes.ListeningComprehension.Value && toolStripMenuItemListeningComprehension.Checked)
					toolStripMenuItemListeningComprehension_Click(null, null);
				toolStripMenuItemListeningComprehension.Enabled = LearnLogic.Dictionary.AllowedQueryTypes.ListeningComprehension.Value;
			}
			if (LearnLogic.Dictionary.AllowedQueryTypes.MultipleChoice.HasValue)
			{
				if (!LearnLogic.Dictionary.AllowedQueryTypes.MultipleChoice.Value && toolStripMenuItemMultipleChoice.Checked)
					toolStripMenuItemMultipleChoice_Click(null, null);
				toolStripMenuItemMultipleChoice.Enabled = LearnLogic.Dictionary.AllowedQueryTypes.MultipleChoice.Value;
			}
			if (LearnLogic.Dictionary.AllowedQueryTypes.Sentence.HasValue)
			{
				if (!LearnLogic.Dictionary.AllowedQueryTypes.Sentence.Value && toolStripMenuItemSentences.Checked)
					toolStripMenuItemSentences_Click(null, null);
				toolStripMenuItemSentences.Enabled = LearnLogic.Dictionary.AllowedQueryTypes.Sentence.Value;
			}
			if (LearnLogic.Dictionary.AllowedQueryTypes.Word.HasValue)
			{
				if (!LearnLogic.Dictionary.AllowedQueryTypes.Word.Value && toolStripMenuItemStandard.Checked)
					toolStripMenuItemStandard_Click(null, null);
				toolStripMenuItemStandard.Enabled = LearnLogic.Dictionary.AllowedQueryTypes.Word.Value;
			}

			getToolStripDropDownChangeLearningModeImage();

			if (UpdateToolStripLearningModes())
			{
				OnLMOptionsChanged();
			}

			displayLearningModuleOptions();

			getToolStripDropDownChangeLearningModeImage();
		}

		/// <summary>
		/// this methode is needed to "update" the color values after the style was changed
		/// </summary>
		/// <remarks>Documented by Dev06, 2006-08-30</remarks>
		private void SetStyle()
		{
			if (styleHandler.CurrentStyle != null)
			{
				this.SuspendLayout();
				StyleControls(learningWindow.Controls);
				this.ResumeLayout();

				LearnLogic.AnswerStyleSheet = styleHandler.CurrentStyle.AnswerStylesheetPath;
				LearnLogic.QuestionStyleSheet = styleHandler.CurrentStyle.QuestionStylesheetPath;
				learningWindow.SetStackCardBackColors(styleHandler);

				OnLMOptionsChanged();
			}
		}

		/// <summary>
		/// Styles the controls.
		/// </summary>
		/// <param name="controls">The controls.</param>
		/// <remarks>Documented by Dev05, 2007-08-10</remarks>
		private void StyleControls(Control.ControlCollection controls)
		{
			if (styleHandler.CurrentStyle != null)
			{
				foreach (Control control in controls)
				{

					if (control.Name == "glassButtonPreviousCard" || control.Name == "glassButtonNextCard"
						|| control.Name == "glassButtonQuestion" || control.Name == "glassButtonSelfAssesmentDontKnow"
						|| control.Name == "glassButtonSelfAssesmentDoKnow"
						|| control.Name == "pictureBoxImageAnswerPanel")
					{
						PropertyInfo property = control.GetType().GetProperty("Image");
						property.SetValue(control, null, null);
						property = control.GetType().GetProperty("ImageDisabled");
						property.SetValue(control, null, null);
						property = control.GetType().GetProperty("ImageEnabled");
						property.SetValue(control, null, null);
					}
					if (control.Name == "answerPanel" || control.Name == "stackFlow")
					{
						PropertyInfo property = null;
						if (control.Name == "answerPanel")
						{
							property = control.GetType().GetProperty("ImageRightCorner");
							property.SetValue(control, null, null);
						}
						property = control.GetType().GetProperty("BackgroundImage");
						property.SetValue(control, null, null);

					}
					if (control.Name == "multipleChoice")
					{
						PropertyInfo property = control.GetType().GetProperty("BackGroundCheckBox");
						property.SetValue(control, null, null);

					}
					if (control.Controls.Count > 0)
						StyleControls(control.Controls);

					if (styleHandler.CurrentStyle.StyledControls.ContainsKey(control.Name))
					{
						if (styleHandler.CurrentStyle.StyledControls[control.Name].BackColor != Color.Empty)
							control.BackColor = styleHandler.CurrentStyle.StyledControls[control.Name].BackColor;
						if (styleHandler.CurrentStyle.StyledControls[control.Name].ForeColor != Color.Empty)
							control.ForeColor = styleHandler.CurrentStyle.StyledControls[control.Name].ForeColor;
						if (styleHandler.CurrentStyle.StyledControls[control.Name].Font != null)
							control.Font = styleHandler.CurrentStyle.StyledControls[control.Name].Font;
						if (styleHandler.CurrentStyle.StyledControls[control.Name].CustomProperties.Count > 0)
						{
							//go through each custom property, check and apply it to the current control
							foreach (KeyValuePair<string, object> propertyvalue in styleHandler.CurrentStyle.StyledControls[control.Name].CustomProperties)
							{
								PropertyInfo property = control.GetType().GetProperty(propertyvalue.Key);
								if (property != null && property.PropertyType == propertyvalue.Value.GetType())
									property.SetValue(control, propertyvalue.Value, null);
								//BugFix:
								else if (property != null && property.PropertyType == typeof(Image) && propertyvalue.Value.GetType() == typeof(Bitmap))
									property.SetValue(control, propertyvalue.Value as Image, null);
							}
						}
						if (control.Visible)
							control.Refresh();
					}
				}
			}

		}

		/// <summary>
		/// Shows the current learning statistics.
		/// </summary>
		/// <returns>False, if the user clicked on Cancel.</returns>
		/// <remarks>Documented by Dev02, 2008-05-05</remarks>
		private bool ShowLearningStatistics()
		{
			if (!this.IsInTray && LearnLogic != null && LearnLogic.Dictionary.Settings.ShowStatistics.Value && LearnLogic.CardStack.Count > 0)
			{
				QueryStatsForm stats = new MLifter.QueryStatsForm();
				stats.ShowStats(LearnLogic.CardStack.SessionDuration, LearnLogic.CardStack.RightCount, LearnLogic.CardStack.WrongCount);
				if (stats.DialogResult == DialogResult.Cancel)
					return false;
			}
			return true;
		}

		/// <summary>
		/// Handles the Click event of the recentFileMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-04-29</remarks>
		void recentFileMenuItem_Click(object sender, EventArgs e)
		{
			if (sender is ToolStripMenuItem && ((ToolStripMenuItem)sender).Tag is LearningModulesIndexEntry)
			{
				LearningModulesIndexEntry entry = ((ToolStripMenuItem)sender).Tag as LearningModulesIndexEntry;
				OpenEntry(entry);
			}
		}

		/// <summary>
		/// Opens the entry.
		/// </summary>
		/// <param name="entry">The entry.</param>
		/// <remarks>Documented by Dev05, 2009-05-04</remarks>
		public void OpenEntry(LearningModulesIndexEntry entry)
		{
			//do not open the LM in case it is already opened
			if (LearnLogic.LearningModuleLoaded && LearnLogic.CurrentLearningModule != null)
				if (entry.ConnectionString.ConnectionString == LearnLogic.CurrentLearningModule.ConnectionString.ConnectionString &&
					entry.ConnectionString.LmId == LearnLogic.CurrentLearningModule.ConnectionString.LmId)
					return;

			if (entry.ConnectionString.SyncType != SyncType.NotSynchronized)
			{
				LearnLogic.CloseLearningModule();

				if (entry.ConnectionString.SyncType != SyncType.FullSynchronized)
				{
					try
					{
						foreach (KeyValuePair<IConnectionString, IUser> pair in LearningModulesIndex.ConnectionUsers)
						{
							if (pair.Key.ConnectionString == entry.ConnectionString.ConnectionString)
							{
								entry.User = pair.Value;
								entry.User.Login();
								break;
							}
						}
						if (entry.User == null)
						{
							if (!LearnLogic.User.Authenticate(LearnLogic.GetLoginDelegate, entry.ConnectionString, LearnLogic.DataAccessErrorDelegate))
								return;
							entry.User = LearnLogic.User.GetBaseUser();
						}
					}
					catch (NoValidUserException) { return; }
				}

				SyncLearningModule(entry);
			}

			OpenLearningModule(entry);
		}

		#region Functions related to the form (creation, closing)

		/// <summary>
		/// Loads saved style or default style and applies these settings.
		/// </summary>
		/// <returns>No return value</returns>
		/// <exceptions>Does not throw any exception.</exceptions>
		/// <remarks>Documented by Dev00, 2007-07-26</remarks>
		public void LoadStyle()
		{
			styleHandler.CurrentStyleName = Settings.Default.CurrentStyle;

			//apply style settings to mainform
			SetStyle();
		}

		/// <summary>
		/// Constructor of class. Opens/Creates registry key, creates "learn particular box" menu, VocEdit, character box, stacks
		/// </summary>
		/// <returns>No return value</returns>
		/// <exception cref="ex"></exceptions>
		/// <remarks>Documented by Dev00, 2007-07-26</remarks>
		public MainForm()
		{
			//check stick and load settings (must be first as all following code depends on settings)
			OnStickMode = Setup.RunningFromStick();

			if (OnStickMode)
				Setup.LoadSettingsFromStick();

			Setup.UpgradeFromEarlierVersion();

			//load language and culture
			try
			{
				CultureInfo currentCulture = new CultureInfo(Settings.Default.CurrentLanguage);
				Thread.CurrentThread.CurrentUICulture = currentCulture;
				if (!currentCulture.IsNeutralCulture) //neutral cultures cannot be used for the thread culture
					Thread.CurrentThread.CurrentCulture = currentCulture;
			}
			catch (Exception e)
			{
				Trace.WriteLine("Culture appliance failed: " + e.ToString());
			}
			Thread.CurrentThread.Name = "MLifter Main-Thread";

			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			// take care of the closing event of the dropdown menu for changing the learningmodi
			this.toolStripDropDownButtonChangeLearnModus.DropDown.Closing += new ToolStripDropDownClosingEventHandler(toolStripDropDownButtonChangeLearnModus_Closing);

			//set up character map
			characterMapComponent.RegisterControl(this);
			characterMapComponent.VisibleChanged += new EventHandler(characterMapComponent_VisibleChanged);
			Application.EnterThreadModal += new EventHandler(Application_EnterThreadModal);
			Application.LeaveThreadModal += new EventHandler(Application_LeaveThreadModal);

			//set up recent files list event
			RecentLearningModules.Restore(Setup.RecentLearningModulesPath);
			UpdateRecentFiles();
			RecentLearningModules.ListChanged += new EventHandler(RecentFiles_ListChanged);

			//initialize learnlogic
			if (LearnLogic == null)
			{
				LearnLogic = new LearnLogic(LoginForm.OpenLoginForm, DataAccessError);
				learnLogic.SyncedLearningModulesPath = Setup.SyncedModulesPath;
				LearnLogic.LearningModuleSyncRequest += new EventHandler(LearnLogic_LearningModuleSyncRequest);
			}

			//set up business logic events
			LearnLogic.LearningModuleOptionsChanged += new EventHandler(learnLogic_LearningModuleOptionsChanged);
			LearnLogic.LearningModuleOpened += new EventHandler(learnLogic_LearningModuleOpened);
			LearnLogic.LearningModuleConnectionLost += new EventHandler(LearnLogic_LearningModuleConnectionLost);
			LearnLogic.LearningModuleClosed += new EventHandler(learnLogic_LearningModuleClosed);
			LearnLogic.LearningModuleClosing += new LearnLogic.LearningModuleClosingEventHandler(learnLogic_LearningModuleClosing);
			LearnLogic.CardStack.StackChanged += new EventHandler(learnLogic_CardStack_StackChanged);
			LearnLogic.SnoozeModeQuitProgram += new EventHandler(LearnLogic_SnoozeModeQuitProgram);
			LearnLogic.UserSessionClosed += new EventHandler(LearnLogic_UserSessionClosed);

			//register the LearnLogic to the LearningWindow
			learningWindow.RegisterLearnLogic(MainForm.LearnLogic);

			//trayicon title
			TrayIcon.Text = AssemblyData.Title;

			//set up drive detector to react properly when a memory stick gets unplugged
			driveDetector_WatchDevice(Setup.ApplicationRoot);
			MLifter.Classes.Help.SetHelpNameSpace(MainHelp);
		}

		/// <summary>
		/// Handles the ExecuteExtension event of the Extensions control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="MLifter.BusinessLayer.ExtensionEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2009-07-03</remarks>
		void Extensions_ExecuteExtension(object sender, ExtensionEventArgs e)
		{
			IExtension extension = e.Extension;
			if (extension.Type == ExtensionType.Skin)
			{
				styleHandler.AddStyle(extension.StartFile);
				styleHandler.CurrentStyleName = Path.GetFileNameWithoutExtension(extension.StartFile);
				SetStyle();
				RefreshStyleMenu();

				if (e.Action.Execution == ExtensionActionExecution.Once)
					Settings.Default.CurrentStyle = styleHandler.CurrentStyleName;
				else if (e.Action.Execution == ExtensionActionExecution.Always)
					foreach (ToolStripMenuItem item in switchSkinToolStripMenuItem.DropDownItems)
						item.Enabled = false;
			}
		}

		/// <summary>
		/// Handles the InformUser event of the Extensions control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="MLifter.BusinessLayer.ExtensionEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2009-07-09</remarks>
		void Extensions_InformUser(object sender, ExtensionEventArgs e)
		{
			string userMessage = Resources.EXTENSION_INFORMUSER_MESSAGE_INSTALLED;
			foreach (ExtensionAction action in e.Extension.Actions)
			{
				if (action.Execution == ExtensionActionExecution.Never)
					continue;

				if (action.Kind == ExtensionActionKind.Force)
					userMessage = Resources.EXTENSION_INFORMUSER_MESSAGE_SELECTED;
			}

			string localizedType = string.Empty;
			switch (e.Extension.Type)
			{
				case ExtensionType.Skin:
					localizedType = Resources.EXTENSION_INFORMUSER_MESSAGE_TYPE_SKIN;
					break;
				case ExtensionType.Unknown:
				default:
					localizedType = Resources.EXTENSION_INFORMUSER_MESSAGE_TYPE_UNKNOWN;
					break;
			}

			MessageBox.Show(string.Format(userMessage, localizedType, e.Extension.Name), Resources.EXTENSION_INFORMUSER_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
		}

		/// <summary>
		/// Handles the LearningModuleConnectionLost event of the LearnLogic control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2009-05-15</remarks>
		private void LearnLogic_LearningModuleConnectionLost(object sender, EventArgs e)
		{
			ShowConnectionLostMessage();
		}

		/// <summary>
		/// Shows the connection lost message.
		/// </summary>
		/// <remarks>Documented by Dev05, 2009-05-15</remarks>
		private static void ShowConnectionLostMessage()
		{
			TaskDialog.MessageBox(Resources.LEARNING_WINDOW_SERVER_OFFLINE_DIALOG_TITLE, Resources.LEARNING_WINDOW_SERVER_OFFLINE_DIALOG_MAIN, Resources.LEARNING_WINDOW_SERVER_OFFLINE_DIALOG_CONTENT,
				 TaskDialogButtons.OK, TaskDialogIcons.Error);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MainForm"/> class.
		/// </summary>
		/// <param name="args">The args.</param>
		/// <remarks>Documented by Dev02, 2008-06-26</remarks>
		public MainForm(string[] args)
			: this()
		{
			if (args.Length > 0 && args[0] != null)
				this.CommandLineParam = args[0];
		}

		/// <summary>
		/// Handles the SnoozeModeQuitProgram event of the LearnLogic control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-05-14</remarks>
		void LearnLogic_SnoozeModeQuitProgram(object sender, EventArgs e)
		{
			//snooze mode issued program closing
			this.Close();
		}

		/// <summary>
		/// Handles the UserSessionClosed event of the LearnLogic control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2008-11-18</remarks>
		void LearnLogic_UserSessionClosed(object sender, EventArgs e)
		{
			TaskDialog.MessageBox(Resources.SESSION_INVALID_TITLE, Resources.SESSION_INVALID_MAIN, Resources.SESSION_INVALID_DETAIL,
				 TaskDialogButtons.OK, TaskDialogIcons.Error);
			try
			{
				foreach (Form frm in Application.OpenForms)
				{
					if (!(frm is MainForm))
						frm.Close();
				}
			}
			catch (System.Exception exp)
			{
				Trace.WriteLine(exp.ToString());
			}
			learnLogic.CloseLearningModuleWithoutSaving();
		}

		/// <summary>
		/// This field remembers whether the character map was visible when the dialog entered the modal state.
		/// </summary>
		private bool modalFormCharacterMapWasVisible = false;

		/// <summary>
		/// Handles the EnterThreadModal event of the Application control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-05-13</remarks>
		void Application_EnterThreadModal(object sender, EventArgs e)
		{
			modalFormCharacterMapWasVisible = characterMapComponent.Visible;
			characterMapComponent.Visible = false;
		}

		/// <summary>
		/// Handles the LeaveThreadModal event of the Application control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-05-13</remarks>
		void Application_LeaveThreadModal(object sender, EventArgs e)
		{
			if (modalFormCharacterMapWasVisible)
				characterMapComponent.Visible = true;
		}

		/// <summary>
		/// Logs all LayoutChanges.
		/// </summary>
		/// <param name="start">The start.</param>
		/// <remarks>Documented by Dev02, 2008-05-07</remarks>
		private void SnapAllLayout(Control start)
		{
			start.Layout += new LayoutEventHandler(snap_Layout);

			foreach (Control c in start.Controls)
			{
				SnapAllLayout(c);
			}
		}

		/// <summary>
		/// Handles the Layout event of the snap control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.LayoutEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-05-07</remarks>
		void snap_Layout(object sender, LayoutEventArgs e)
		{
			Control c = sender as Control;

			System.Diagnostics.Debug.WriteLine(String.Format("Control: {0}\r\nBounds: {1}\r\nReason {2}\r\n Where {3}",
				c.Name,
				c.Bounds,
				e.AffectedProperty,
				new System.Diagnostics.StackTrace().ToString()));
		}

		/// <summary>
		/// Handles the LearningModuleClosing event of the LearnLogic control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="MLifter.BusinessLayer.LearningModuleClosingEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-05-06</remarks>
		void learnLogic_LearningModuleClosing(object sender, LearningModuleClosingEventArgs e)
		{
			if (!ShowLearningStatistics())
				e.Cancel = true;
		}

		/// <summary>
		/// Handles the LearningModuleClosed event of the LearnLogic control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-05-06</remarks>
		void learnLogic_LearningModuleClosed(object sender, EventArgs e)
		{
			NoDictionaryLoaded();
		}

		/// <summary>
		/// Handles the LearningModuleOpened event of the LearnLogic control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-05-06</remarks>
		void learnLogic_LearningModuleOpened(object sender, EventArgs e)
		{
			DictionaryLoaded();
		}

		/// <summary>
		/// Handles the StackChanged event of the learnLogic_CardStack control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-05-13</remarks>
		void learnLogic_CardStack_StackChanged(object sender, EventArgs e)
		{
			//card stack changed => card boxes changed => refresh boxes menu
			RefreshBoxMenu();
		}

		/// <summary>
		/// Handles the LearningModuleOptionsChanged event of the LearnLogic control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-04-30</remarks>
		void learnLogic_LearningModuleOptionsChanged(object sender, EventArgs e)
		{
			if (LearnLogic.Dictionary != null)
				Log.RecalculateBoxSizes(LearnLogic.Dictionary.DictionaryDAL.Parent);
			RebuildBoxMenu(); //rebuild box menu
			Properties.Settings.Default.Slideshow = slideShowModeToolStripMenuItem.Checked = LearnLogic.SlideShow;
			this.IsInTray = LearnLogic.SnoozeModeActive;
			toolStripMenuItemCheckForBetaUpdates.Checked = Settings.Default.CheckForBetaUpdates;
			displayLearningModuleOptions();
			//check if LM is ready for learning
			if (LearnLogic.LearningModuleLoaded && !LearnLogic.DictionaryReadyForLearning)
			{
				if (LearnLogic.DictionaryNoChapters)
					editChaptersToolStripMenuItem.PerformClick();
				if (LearnLogic.DictionaryNoQueryChapters)
					selectChaptersToolStripMenuItem.PerformClick();
				if (LearnLogic.DictionaryNoCards && !newLM)
					LearnLogic.OnUserDialog(new UserNotifyDialogEventArgs(LearnLogic.Dictionary, -1, UserNotifyDialogEventArgs.NotifyDialogKind.NoWords));
			}
		}

		/// <summary>
		/// Displays the learning module options.
		/// </summary>
		/// <remarks>Documented by Dev07, 2009-04-09</remarks>
		private void displayLearningModuleOptions()
		{
			if (LearnLogic.Dictionary != null)
			{
				ShowSelectedLearnModi.LearnModusSelectedClear();
				foreach (object item in toolStripDropDownButtonChangeLearnModus.DropDown.Items)
				{
					if (!(item is ToolStripMenuItem))
						continue;
					(item as ToolStripMenuItem).Checked = false;

				}
				//display the new learning settings in the toolstripdropdownbutton image
				if (LearnLogic.Dictionary.Settings.QueryTypes.ImageRecognition == true)
				{
					ShowSelectedLearnModi.LearnModusSelected(ShowSelectedLearnModi.LearnModi.ImageRecognition);
					toolStripMenuItemImageRecognition.Checked = true;
				}
				if (LearnLogic.Dictionary.Settings.QueryTypes.Sentence == true)
				{
					ShowSelectedLearnModi.LearnModusSelected(ShowSelectedLearnModi.LearnModi.Sentences);
					this.toolStripMenuItemSentences.Checked = true;
				}
				if (LearnLogic.Dictionary.Settings.QueryTypes.ListeningComprehension == true)
				{
					ShowSelectedLearnModi.LearnModusSelected(ShowSelectedLearnModi.LearnModi.ListeningComprehension);
					this.toolStripMenuItemListeningComprehension.Checked = true;
				}
				if (LearnLogic.Dictionary.Settings.QueryTypes.MultipleChoice == true)
				{
					ShowSelectedLearnModi.LearnModusSelected(ShowSelectedLearnModi.LearnModi.MultipleChoice);
					this.toolStripMenuItemMultipleChoice.Checked = true;
				}
				if (LearnLogic.Dictionary.Settings.QueryTypes.Word == true)
				{
					ShowSelectedLearnModi.LearnModusSelected(ShowSelectedLearnModi.LearnModi.Standard);
					this.toolStripMenuItemStandard.Checked = true;
				}
			}
			Image image = toolStripDropDownButtonChangeLearnModus.Image;
			ShowSelectedLearnModi.GetImage(image);
			toolStripDropDownButtonChangeLearnModus.Invalidate();
		}

		/// <summary>
		/// Handles the ListChanged event of the RecentFiles control.
		/// Updates the Recent Files list.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-04-29</remarks>
		void RecentFiles_ListChanged(object sender, EventArgs e)
		{
			UpdateRecentFiles();
		}

		/// <summary>
		/// Updates the recent files.
		/// </summary>
		/// <remarks>Documented by Dev05, 2009-03-02</remarks>
		private void UpdateRecentFiles()
		{
			ToolStripItemCollection recentMenu = recentToolStripMenuItem.DropDownItems;
			recentMenu.Clear();

			int count = 0;
			foreach (LearningModulesIndexEntry entry in RecentLearningModules.GetRecentModules())
			{
				if (count > Properties.Settings.Default.RecentFilesSize)
					break;

				ToolStripMenuItem menuitem = new ToolStripMenuItem(
					(string.Format(Resources.RECENT_LEARNINGMODULES_MENUFORMAT, (++count).ToString(), entry.DisplayName, entry.ConnectionName)).Replace("()", string.Empty),
					entry.Logo);
				menuitem.Click += new EventHandler(recentFileMenuItem_Click);
				menuitem.Tag = entry;
				switch (entry.Type)
				{
					case LearningModuleType.Local:
						menuitem.Image = Resources.learning_16;
						break;
					case LearningModuleType.Remote:
						menuitem.Image = Resources.world_16;
						break;
				}
				recentMenu.Add(menuitem);
			}
		}

		protected const int WM_WINDOWPOSCHANGING = 0x46;
		[StructLayout(LayoutKind.Sequential)]
		public struct WINDOWPOS
		{
			public IntPtr hwnd;
			public IntPtr hwndInsertAfter;
			public int x;
			public int y;
			public int cx;
			public int cy;
			public int flags;
		}
		/// <summary>
		/// Override WndProc to receive windows messages and send them to driveDetector.
		/// </summary>
		/// <param name="m">The m.</param>
		/// <remarks>Documented by Dev02, 2007-12-12</remarks>
		protected override void WndProc(ref Message m)
		{
			if ((m.Msg == (int)0x0112 && m.WParam == (IntPtr)0xF122) ||
				(m.Msg == (int)0x0112 && m.WParam == (IntPtr)0xF032) ||
				(m.Msg == (int)0x0112 && m.WParam == (IntPtr)0xF030) ||
				(m.Msg == (int)0x0112 && m.WParam == (IntPtr)0xF120))
			{
				ResizeStart();
			}
			base.WndProc(ref m);

			//Also send the message to the drivedetector
			if (driveDetector != null)
				driveDetector.WndProc(ref m);
		}

		/// <summary>
		/// Loads main Form (loads INI settings from registry, plays startup sound, hides and disposes splashscreen,
		/// checks registration, loads files in the command line, shows news window)
		/// </summary>
		/// <param name="sender">Sender of object</param>
		/// <param name="e">Contains event data</param>
		/// <returns>No return value</returns>
		/// <exception cref="ex"></exceptions>
		/// <remarks>Documented by Dev00, 2007-07-26</remarks>
		private void MainForm_Load(object sender, System.EventArgs e)
		{
			//[ML-551] Main window not in foreground
			//It is important to focus MainForm once - otherwise, the SplashScreen has focus (because it was here first). 
			//SplashScreen closed => Windows focused the last used program
			Activate();

			//refresh show in taskbar state to ensure that the window gets shown
			if (ShowInTaskbar)
			{
				SuspendLayout();
				ShowInTaskbar = false;
				ShowInTaskbar = true;
				ResumeLayout();
			}

			PlayStartupEndSound(false, false); //moved to the top to avoid DirectX audio freeze

			LearnLogic.CountDownTimerMinimum = Properties.Settings.Default.TIMER_MinSeconds;
			LearnLogic.SlideShow = Properties.Settings.Default.Slideshow;
			LearnLogic.IgnoreOldLearningModuleVersion = Properties.Settings.Default.IgnoreOldDics;
			LearnLogic.SynonymInfoMessage = Properties.Settings.Default.SynonymPromt;

			//load stylehandler
			styleHandler = new MLifter.Components.StyleHandler(
				Path.Combine(Application.StartupPath, Properties.Settings.Default.AppDataFolderDesigns),
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				Path.Combine(Properties.Settings.Default.AppDataFolder, Properties.Settings.Default.AppDataFolderDesigns)),
				Properties.Resources.ResourceManager,
				OnStickMode);

			//load extensions stuff
			Extensions.Restore(Setup.InstalledExtensionsFilePath);
			Extensions.SkinPath = styleHandler.StylesPath;
			Extensions.ExecuteExtension += new Extensions.ExtensionEventHandler(Extensions_ExecuteExtension);
			Extensions.InformUser += new Extensions.ExtensionEventHandler(Extensions_InformUser);

			LoadStyle();
			RefreshStyleMenu();

			RefreshLanguageMenu();
			NoDictionaryLoaded();
			LoadWindowSettings();
			
			BringToFront();
			TopMost = true;
			TopMost = false;

			bool firstUse = false;
			if (Settings.Default.FirstUse)
			{
				Wizard startupWizard = new Wizard(MLifter.Classes.Help.HelpPath);
				startupWizard.StartPosition = FormStartPosition.CenterParent;
				startupWizard.Text = Resources.FIRSTSTART_CAPTION;
				startupWizard.Pages.Add(new Controls.Wizards.Startup.DictionaryPathPage(Setup.DictionaryParentPath, 
					Properties.Resources.DICPATH_DEFAULTNAME, Properties.Resources.DICPATH_DEFAULTNAME_OLD));
				startupWizard.ShowDialog();
				
				Controls.Wizards.Startup.DictionaryPathPage dictionaryPathPage = startupWizard.Pages[0] as Controls.Wizards.Startup.DictionaryPathPage;
				Setup.InitializeProfile(dictionaryPathPage.CopyDemoDictionary, dictionaryPathPage.DictionaryPath);

				Properties.Settings.Default.DicDir = dictionaryPathPage.DictionaryPath;

				ConnectionStringHandler.CreateUncConnection(Resources.DEFAULT_CONNECTION_NAME, Settings.Default.DicDir, 
					Setup.UserConfigPath, Resources.DEFAULT_CONNECTION_FILE, true, OnStickMode);
				
				Settings.Default.FirstUse = false;
				Settings.Default.Save();

				firstUse = true;
			}
			
			Setup.CheckDicDir();

			toolStripMenuItemCheckForBetaUpdates.Checked = Settings.Default.CheckForBetaUpdates;
			while (!Program.UpdateChecked) Thread.Sleep(10);

			Program.SuspendIPC = false; //re-enable remote services / signal ready

			//check for unsent error reports
			Classes.ErrorReportGenerator.ProcessPendingReports();

			if (MainformLoadedEvent != null)
			{
				LearningModulesIndex index = new LearningModulesIndex(Setup.GlobalConfigPath, Setup.UserConfigPath, LoginForm.OpenLoginForm, delegate { return; }, Setup.SyncedModulesPath);
				OnMainformLoaded();
			}
			else if (Settings.Default.ShowStartPage && CommandLineParam == string.Empty)
			{
				ShowLearningModulesPage(firstUse);
			}
			else
			{
				LearningModulesIndex index = new LearningModulesIndex(Setup.GlobalConfigPath, Setup.UserConfigPath, LoginForm.OpenLoginForm, delegate { return; }, Setup.SyncedModulesPath);

				//check for news
				MLifter.Controls.News news = new MLifter.Controls.News();
				news.Prepare(true);

				//open most recent LM
				CheckAndLoadStartUpDic();
			}
		}

		/// <summary>
		/// Called when [mainform loaded].
		/// </summary>
		/// <remarks>Documented by Dev02, 2009-06-29</remarks>
		private void OnMainformLoaded()
		{
			Program.SuspendIPC = false;
			this.Refresh();

			MainformLoadedEvent(this, EventArgs.Empty);

			foreach (EventHandler e in MainformLoadedDelegates)
				MainformLoadedEvent -= e;

			MainformLoadedDelegates.Clear();
		}

		/// <summary>
		/// Shows the learning modules page.
		/// </summary>
		/// <remarks>Documented by Dev03, 2009-04-28</remarks>
		private void ShowLearningModulesPage()
		{
			ShowLearningModulesPage(false);
		}
		/// <summary>
		/// Shows the learning modules page.
		/// </summary>
		/// <param name="showStartupHelp">set to <c>true</c> for first use.</param>
		/// <remarks>Documented by Dev05, 2008-12-09</remarks>
		private void ShowLearningModulesPage(bool firstUse)
		{
			using (LearningModulesForm lmf = new LearningModulesForm(MLifter.Classes.Help.HelpPath))
			{
				lmf.GeneralConfigurationPath = Setup.GlobalConfigPath;
				lmf.UserConfigurationPath = Setup.UserConfigPath;
				lmf.SyncedLearningModulePath = Setup.SyncedModulesPath;
				lmf.ShowStartPageAtStartup = Settings.Default.ShowStartPage;
				lmf.FirstUse = firstUse;

				lmf.LoadLearningModules();
				
				DialogResult dr = lmf.ShowDialog(this);

				if (dr == DialogResult.OK && !lmf.IsUsedDragAndDrop)
					OpenLearningModule(lmf.SelectedConnection);
				else if (dr == DialogResult.OK && lmf.IsUsedDragAndDrop)
					DoDragAndDrop(lmf.SelectedConnection.ConnectionString.ConnectionString);
				else if (dr == DialogResult.Cancel && lmf.CreateNewLearningModule == true)
					CreateNewLearningModule();

				Setup.SetShowStartPage(lmf.ShowStartPageAtStartup);
			}
		}

		/// <summary>
		/// Calculates the initial layout. (Window Location, Size, Splitter positions)
		/// </summary>
		/// <remarks>Documented by Dev03, 2008-02-01</remarks>
		private void LoadWindowSettings()
		{
			loadWindowSettings = true;
			this.SuspendLayout();

			//set window location to center for the first start [ML-519]
			if (MLifter.Properties.Settings.Default.FirstUse)
			{
				if (Screen.GetWorkingArea(this).Height < 750 || Screen.GetWorkingArea(this).Width < 1005)
				{
					this.Height = 580;
					this.Width = 850;
					MLifter.Properties.Settings.Default.LayoutValues = new Size(this.Width / 2 - 3, MLifter.Properties.Settings.Default.LayoutValues.Height);
					Settings.Default.Save();
				}
			}
			else
			{
				this.Location = MLifter.Properties.Settings.Default.Location;
				this.Width = MLifter.Properties.Settings.Default.Size.Width;
				this.Height = MLifter.Properties.Settings.Default.Size.Height;
			}

			//validate screen position
			Screen screen = Screen.FromControl(this);
			if (screen.WorkingArea.X + screen.WorkingArea.Width < this.Location.X || screen.WorkingArea.X > this.Location.X + this.Width)
				this.Location = new Point(10, this.Location.Y);
			if (screen.WorkingArea.Y + screen.WorkingArea.Height < this.Location.Y || screen.WorkingArea.Y > this.Location.Y)
				this.Location = new Point(this.Location.X, 10);

			this.ResumeLayout();

			//Has to be executed AFTER this.ResumeLayout() ... don't ask me why... FabThe
			this.WindowState = MLifter.Properties.Settings.Default.Maximized ? FormWindowState.Maximized : FormWindowState.Normal;
			mainFormWindowState = this.WindowState;
			learningWindow.LayoutValues = MLifter.Properties.Settings.Default.LayoutValues;

			loadWindowSettings = false;
		}

		/// <summary>
		/// Prepares the language menu.
		/// </summary>
		/// <remarks>Documented by Dev03, 2008-02-01</remarks>
		private void RefreshLanguageMenu()
		{
			List<ToolStripMenuItem> languageItems = new List<ToolStripMenuItem>();

			string[] languages = Properties.Settings.Default.AvailableLanguages.Split(';');

			foreach (string language in languages)
			{
				ToolStripMenuItem languageItem = new ToolStripMenuItem();

				languageItem.Name = language;

				CultureInfo oldInfo = Thread.CurrentThread.CurrentUICulture;
				CultureInfo cultureInfo = new CultureInfo(language);

				//get neutral cultureinfo for display
				while (!cultureInfo.IsNeutralCulture && cultureInfo.Parent != null)
					cultureInfo = cultureInfo.Parent;

				languageItem.Text = cultureInfo.EnglishName +
					" (" + cultureInfo.NativeName + ")";

				languageItem.Click += new EventHandler(LanguageChange_Click);

				if (language.ToLowerInvariant() == Thread.CurrentThread.CurrentUICulture.Name.ToLowerInvariant())
					languageItem.Checked = true;

				languageItems.Add(languageItem);
			}

			switchLanguageToolStripMenuItem.DropDownItems.Clear();
			switchLanguageToolStripMenuItem.DropDownItems.AddRange(languageItems.ToArray());

			if (languages.Length <= 1)
				switchLanguageToolStripMenuItem.Visible = false;
		}

		/// <summary>
		/// Handles the Click event of the LanguageChange control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-04-30</remarks>
		private void LanguageChange_Click(object sender, EventArgs e)
		{
			if (sender is ToolStripMenuItem)
			{
				//uncheck all languages
				foreach (ToolStripMenuItem menuitem in switchLanguageToolStripMenuItem.DropDownItems)
					menuitem.Checked = false;

				ToolStripMenuItem item = (ToolStripMenuItem)sender;
				item.Checked = true;

				uiCultureChanger.ApplyCulture(new CultureInfo(item.Name));

				//[ML-2331]
				Thread.CurrentThread.CurrentCulture = new CultureInfo(item.Name);
				Thread.CurrentThread.CurrentUICulture = new CultureInfo(item.Name);

				Settings.Default.CurrentLanguage = item.Name;
				//[ML-922] load new card for updating information bar
				if (LearnLogic.Dictionary != null)
					OnLMOptionsChanged();

				//[ML-924] recreate box menu to update the language specific items
				RebuildBoxMenu();

				learningWindow.UpdateCulture();

				//[ML-1171] [ML-1205] update the help namespace of Mainform and Characterform
				MLifter.Classes.Help.SetHelpNameSpace(MainHelp);
			}
		}

		/// <summary>
		/// Prepares the style menu.
		/// </summary>
		/// <remarks>Documented by Dev03, 2008-02-01</remarks>
		private void RefreshStyleMenu()
		{
			List<ToolStripMenuItem> items = new List<ToolStripMenuItem>();

			foreach (Style style in styleHandler.Styles.Values)
			{
				ToolStripMenuItem item = new ToolStripMenuItem();
				item.Text = item.Name = style.StyleName;
				item.Click += new EventHandler(styleItem_Click);

				if (style.StyleName == styleHandler.CurrentStyleName)
					item.Checked = true;

				items.Add(item);
			}

			switchSkinToolStripMenuItem.DropDownItems.Clear();
			switchSkinToolStripMenuItem.DropDownItems.AddRange(items.ToArray());
			switchSkinToolStripMenuItem.Enabled = true;
		}

		/// <summary>
		/// Handles the Click event of the styleItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-04-30</remarks>
		private void styleItem_Click(object sender, EventArgs e)
		{
			if (sender is ToolStripMenuItem)
			{
				UncheckStyles();
				ToolStripMenuItem item = (ToolStripMenuItem)sender;
				item.Checked = true;
				styleHandler.CurrentStyleName = item.Text;
				SetStyle();
				Settings.Default.CurrentStyle = styleHandler.CurrentStyleName;
			}
		}

		/// <summary>
		/// Unchecks the styles.
		/// </summary>
		/// <remarks>Documented by Dev02, 2008-04-30</remarks>
		private void UncheckStyles()
		{
			foreach (ToolStripMenuItem item in switchSkinToolStripMenuItem.DropDownItems)
				item.Checked = false;
		}

		/// <summary>
		/// Refreshes the box menu.
		/// </summary>
		/// <remarks>Documented by Dev03, 2008-02-01</remarks>
		private void RefreshBoxMenu()
		{
			foreach (ToolStripMenuItem item in selectBoxToolStripMenuItem.DropDownItems)
			{
				if (item.Tag != null && item.Tag is IBox)
				{
					IBox box = (IBox)item.Tag;
					item.Enabled = box.Id == 0 ? true : box.CurrentSize > 0;
					item.Checked = box.Id == LearnLogic.Dictionary.LearningBox;
				}
			}
		}

		/// <summary>
		/// Rebuilds the box menu.
		/// </summary>
		/// <remarks>Documented by Dev02, 2008-05-13</remarks>
		private void RebuildBoxMenu()
		{
			if (LearnLogic.Dictionary != null)
			{
				List<ToolStripMenuItem> items = new List<ToolStripMenuItem>();

				foreach (IBox box in LearnLogic.Dictionary.Boxes)
				{
					ToolStripMenuItem item = new ToolStripMenuItem();
					item.Tag = box;
					item.Text = box.Id == 0 ? Resources.MAINFORM_LEARNBOX_TEXT : string.Format("&{0}", box.Id);
					item.Click += new EventHandler(selectLearnBox_Click);
					items.Add(item);
				}

				selectBoxToolStripMenuItem.DropDownItems.Clear();
				selectBoxToolStripMenuItem.DropDownItems.AddRange(items.ToArray());
				RefreshBoxMenu();
			}
		}

		/// <summary>
		/// Handles the Click event of the selectLearnBox control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-04-30</remarks>
		void selectLearnBox_Click(object sender, EventArgs e)
		{
			if (sender is ToolStripMenuItem && ((ToolStripMenuItem)sender).Tag is IBox)
			{
				int boxid = ((IBox)((ToolStripMenuItem)sender).Tag).Id;
				LearnLogic.Dictionary.LearningBox = boxid;
				OnLMOptionsChanged();
			}
		}


		/// <summary>
		/// Checks for and plays the start up sound.
		/// </summary>
		/// <param name="endsound">if set to <c>true</c> [endsound].</param>
		/// <param name="terminateAudioplayerThread">if set to <c>true</c> [terminate audioplayer thread].</param>
		/// <remarks>Documented by Dev03, 2008-02-01</remarks>
		private void PlayStartupEndSound(bool endsound, bool terminateAudioplayerThread)
		{
			// play start-up sound
			if (Properties.Settings.Default.Play)
			{
				string soundfile = Path.Combine(Application.StartupPath, endsound ? Settings.Default.SOUNDS_Exit : Settings.Default.SOUNDS_Startup);
				if (LearnLogic != null && File.Exists(soundfile))
					LearnLogic.PlayAudioFile(soundfile, true);
				else
					MessageBox.Show(Resources.SOUND_ERROR_TEXT + soundfile, Resources.SOUND_ERROR_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			if (terminateAudioplayerThread && LearnLogic != null)
				LearnLogic.TerminateAudioplayerThread(false);
		}

		/// <summary>
		/// Checks for and loads the start up dictionary.
		/// </summary>
		/// <remarks>Documented by Dev03, 2008-02-01</remarks>
		private void CheckAndLoadStartUpDic()
		{
			// check for files in command line and load them
			if (!CommandLineParam.Equals(String.Empty))
			{
				if (Path.GetExtension(commandLineParam) == Helper.ConfigFileExtension)
					OpenConfigFile(commandLineParam);
				else
					OpenLearningModule(CommandLineParam);
			}
			else
			{
				if (RecentLearningModules.MostRecentLearningModule != null)
					OpenEntry(RecentLearningModules.MostRecentLearningModule);
			}
		}

		/// <summary>
		///  Plays sound, saves dictionary, saves window settings and closes main form
		/// </summary>
		/// <param name="sender">Sender of object - not used</param>
		/// <param name="e">Contains event data</param>
		/// <returns>No return value</returns>
		/// <exception cref="ex"></exceptions>
		/// <remarks>Documented by Dev00, 2007-07-26</remarks>
		private void MainForm_Closing(object sender, FormClosingEventArgs e)
		{
			Program.SuspendIPC = true;

			if (e.CloseReason == CloseReason.WindowsShutDown)
			{
				try
				{
					SaveSettings(true);
					if (LearnLogic != null)
					{
						LearnLogic.SaveLearningModule();
						LearnLogic.CloseLearningModuleWithoutSaving();
						if (LearnLogic.User != null)
							LearnLogic.User.Logout();
					}
					Program.EndIPC();
				}
				finally
				{
					Environment.Exit(-1);
				}
			}

			try
			{
				try
				{
					if (LearnLogic != null)
					{
						if (!LearnLogic.CloseLearningModule())
						{
							e.Cancel = true;
							return;
						}
						if (LearnLogic.User != null)
							LearnLogic.User.Logout();

						PlayStartupEndSound(true, true);
					}
				}
				catch (ConnectionLostException) { ShowConnectionLostMessage(); }

				SaveSettings(true);
				Program.EndIPC();

				e.Cancel = false;
			}
			finally
			{
				//reactivate IPC server in case the closing process was canceled
				Program.SuspendIPC = false;
			}
		}

		/// <summary>
		/// Saves the settings.
		/// </summary>
		/// <remarks>Documented by Dev02, 2007-12-10</remarks>
		private void SaveSettings(bool saveLearningWindowValues)
		{
			//[ML-1869] ML doesn't save the correct size
			if (this.WindowState == FormWindowState.Maximized)
			{
				MLifter.Properties.Settings.Default.Maximized = true;
			}
			else if (this.WindowState == FormWindowState.Minimized)
			{
				Size s = learningWindow.LayoutValues;
				this.WindowState = FormWindowState.Normal;
				MLifter.Properties.Settings.Default.Location = this.Location;
				MLifter.Properties.Settings.Default.Size = this.Size;
			}
			else
			{
				MLifter.Properties.Settings.Default.Maximized = false;
				MLifter.Properties.Settings.Default.Location = this.Location;
				MLifter.Properties.Settings.Default.Size = this.Size;
			}

			//[ML-1893] Weird skin flashing on resize / move
			//Save LearningWindow LayoutValues only if the learning window is displayed (when dictionary is loaded)
			if (saveLearningWindowValues)
				MLifter.Properties.Settings.Default.LayoutValues = learningWindow.LayoutValues;
			MLifter.Properties.Settings.Default.Slideshow = LearnLogic.SlideShow;
			MLifter.Properties.Settings.Default.IgnoreOldDics = LearnLogic.IgnoreOldLearningModuleVersion;
			MLifter.Properties.Settings.Default.SynonymPromt = LearnLogic.SynonymInfoMessage;

			//reset the last import timestamp when the setting should not be session persistent
			if (!MLifter.Properties.Settings.Default.LastImportSessionPersistent)
			{
				try
				{
					MLifter.Properties.Settings.Default.LastImportTimestamp =
						Convert.ToDateTime(MLifter.Properties.Settings.Default.Properties["LastImportTimestamp"].DefaultValue);
				}
				catch
				{
					MLifter.Properties.Settings.Default.LastImportTimestamp = new DateTime();
				}
			}

			MLifter.Properties.Settings.Default.Save();

			//save extensions
			Extensions.Dump(Setup.InstalledExtensionsFilePath);

			//write the settings to the stick when in memory stick mode
			if (OnStickMode)
				Setup.SaveSettingsToStick();
		}

		/// <summary>
		/// Watches a drive to detect removal.
		/// </summary>
		/// <param name="drive">The drive.</param>
		/// <remarks>Documented by Dev02, 2007-12-13</remarks>
		void driveDetector_WatchDevice(string drive)
		{
			if (driveDetector == null)
			{
				driveDetector = new DriveDetector(this);
				driveDetector.DeviceRemoved += new MLifter.Components.DriveDetectorEventHandler(driveDetector_DeviceRemoved);
				driveDetector.QueryRemove += new DriveDetectorEventHandler(driveDetector_QueryRemove);
			}
			driveDetector.EnableQueryRemove(drive);

			//prepare message (so that ressources are loaded while the stick is still plugged in)
			if (LearnLogic.Dictionary != null)
			{
				driveRemoved = new MLifter.Controls.EmulatedTaskDialog();
				driveRemoved.Title = Properties.Resources.MAINFORM_STICKREMOVED_CAPTION;
				driveRemoved.MainInstruction = Properties.Resources.MAINFORM_STICKREMOVED_MAININSTRUCTION;
				driveRemoved.Content = string.Format(Properties.Resources.MAINFORM_STICKREMOVED_DESCRIPTION, LearnLogic.Dictionary.DictionaryPath);
				driveRemoved.CommandButtons = Properties.Resources.MAINFORM_STICKREMOVED_CLOSEDIC;
				driveRemoved.Buttons = MLifter.Controls.TaskDialogButtons.None;
				driveRemoved.MainIcon = MLifter.Controls.TaskDialogIcons.Warning;
				driveRemoved.TopMost = true;
				driveRemoved.BuildForm();
			}
		}

		/// <summary>
		/// Handles the QueryRemove event of the driveDetector control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="MLifter.Components.DriveDetectorEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2007-12-11</remarks>
		void driveDetector_QueryRemove(object sender, MLifter.Components.DriveDetectorEventArgs e)
		{
			//prevent drive removal is memory lifter is running from it or if it has a dictionary open from it
			if (e.Drive == Setup.ApplicationRoot)
				e.Cancel = true;

			if (LearnLogic.Dictionary != null && e.Drive == new DirectoryInfo(LearnLogic.Dictionary.DictionaryPath).Root.FullName)
			{
				try
				{
					if (LearnLogic.LearningModuleLoaded && !LearnLogic.CloseLearningModule())
						e.Cancel = true;
					else
						e.Cancel = false;
				}
				catch
				{
					e.Cancel = true;
				}
			}
		}

		/// <summary>
		/// Handles the DeviceRemoved event of the driveDetector control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="MLifter.Components.DriveDetectorEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2007-12-11</remarks>
		void driveDetector_DeviceRemoved(object sender, MLifter.Components.DriveDetectorEventArgs e)
		{
			//clean up after removal of the drive
			if (LearnLogic.Dictionary != null && driveRemoved != null && !driveRemoved.Visible)
			{
				if (LearnLogic.CurrentLearningModule.ConnectionString.Typ == DatabaseType.MsSqlCe)
				{
					if (LearnLogic.CurrentLearningModule.ConnectionString.ConnectionString.StartsWith(e.Drive))
					{
						try { LearnLogic.CloseLearningModuleWithoutSaving(); }
						catch (Exception exp) { Trace.WriteLine(exp.ToString()); }
					}
				}
				else
				{
					FileInfo dictionaryFile = new FileInfo(LearnLogic.Dictionary.DictionaryPath);
					dictionaryFile.Refresh();
					if (e.Drive == new DirectoryInfo(LearnLogic.Dictionary.DictionaryPath).Root.FullName && !dictionaryFile.Exists)
					{
						//disable all open forms
						foreach (Form form in Application.OpenForms)
						{
							//[ML-753] - make sure that the driveremoved dialog does not get disabled itself
							if (form != driveRemoved)
								form.Enabled = false;
						}
						Enabled = false;

						if (!this.IsInTray)
						{
							driveRemoved.Enabled = true;
							driveRemoved.Show();

							while (true)
							{
								//check for stick presence
								dictionaryFile.Refresh();
								if (dictionaryFile.Exists)
									break;
								if (driveRemoved.DialogResult != DialogResult.None)
									break;
								System.Threading.Thread.CurrentThread.Join(50);
								Application.DoEvents();
							}
						}

						if (!dictionaryFile.Exists)
						{
							//close dictionary without saving
							LearnLogic.CloseLearningModuleWithoutSaving();
							//close all open forms
							//[ML-754] - The openForms collection must not be modified during the enumeration operation
							System.Collections.Generic.List<Form> openForms = new System.Collections.Generic.List<Form>();
							foreach (Form form in Application.OpenForms)
								openForms.Add(form);
							foreach (Form form in openForms)
							{
								if (form != this && form != driveRemoved)
									form.Close();
							}
						}

						//reenable all open forms
						foreach (Form form in Application.OpenForms)
						{
							form.Enabled = true;
						}
						Enabled = true;
						driveRemoved.DialogResult = DialogResult.None;
						driveRemoved.Hide();
					}
				}
			}

			if ((e.Drive == Setup.ApplicationRoot && !new DirectoryInfo(Setup.ApplicationRoot).Exists) || IsInTray)
			{
				try
				{
					if (IsInTray)
						TrayIcon.Visible = false; //do not use IsInTray=false, since it would try to access the removed device
					if (LearnLogic.LearningModuleLoaded)
						LearnLogic.SaveLearningModule();
				}
				catch { }
				finally
				{
					Environment.Exit(-1);
				}
			}
		}
		#endregion

		# region Drag&Drop
		/// <summary>
		/// Handles the DragEnter event of the MainForm control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.DragEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-11-28</remarks>
		private void MainForm_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				if (Helper.CheckFileName(((string[])e.Data.GetData(DataFormats.FileDrop))[0]) && !OtherFormsOpen && !LearningModulesPageOpen)
					e.Effect = DragDropEffects.Link;
				else
					e.Effect = DragDropEffects.None;
			}
		}

		/// <summary>
		/// Gets a value indicating whether [other forms (than mainform, learning modules form, splashscreen or charactermap) are open].
		/// </summary>
		/// <value><c>true</c> if [other forms open]; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev02, 2008-03-13</remarks>
		public bool OtherFormsOpen
		{
			get
			{
				foreach (Form form in Application.OpenForms)
				{
					if (form != null && form.Visible)
						if (form != this && !(form is CharacterForm) && !(form is LearningModulesForm) && !(form is Splash))
							return true;
				}
				return false;
			}
		}

		/// <summary>
		/// Gets a value indicating whether [learning modules page open].
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [learning modules page open]; otherwise, <c>false</c>.
		/// </value>
		/// <remarks>Documented by Dev02, 2009-08-10</remarks>
		public bool LearningModulesPageOpen
		{
			get
			{
				foreach (Form form in Application.OpenForms)
				{
					if (form != null && form.Visible)
						if (form is LearningModulesForm)
							return true;
				}
				return false;
			}
		}

		/// <summary>
		/// Closes the learning modules page, in case it is open.
		/// </summary>
		/// <remarks>Documented by Dev02, 2009-06-26</remarks>
		public void CloseLearningModulesPage()
		{
			List<Form> learningModulesForms = new List<Form>();
			foreach (Form form in Application.OpenForms)
			{
				if (form != null && form.Visible && form is LearningModulesForm)
					learningModulesForms.Add(form);
			}
			foreach (Form form in learningModulesForms)
			{
				form.Hide(); //hide and refresh is required because the close call is very greedy and can take its time...
				this.Refresh();

				form.Close();
			}
		}

		/// <summary>
		/// Handles the DragDrop event of the MainForm control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.DragEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-11-28</remarks>
		private void MainForm_DragDrop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				string file = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
				ActivateFileDropTimer(file);
			}
		}

		/// <summary>
		/// Activates the file drop timer, which opens the file after a certain time.
		/// </summary>
		/// <param name="file">The file.</param>
		/// <remarks>Documented by Dev02, 2008-02-21</remarks>
		private void ActivateFileDropTimer(string file)
		{
			//fix for [ML-642] TaskDialog window not visible / File dropping blocks explorer process
			System.Windows.Forms.Timer FileDropTimer = new System.Windows.Forms.Timer();
			FileDropTimer.Interval = 50;
			FileDropTimer.Tick += new EventHandler(FileDropTimer_Tick);
			FileDropTimer.Tag = file;
			FileDropTimer.Start();
		}

		/// <summary>
		/// Handles the Tick event of the FileDropTimer control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-02-21</remarks>
		/// <remarks>Documented by Dev08, 2009-03-02</remarks>
		void FileDropTimer_Tick(object sender, EventArgs e)
		{
			//fix for [ML-642] TaskDialog window not visible / File dropping blocks explorer process
			System.Windows.Forms.Timer timer = sender as System.Windows.Forms.Timer;
			timer.Stop();
			if (timer.Tag == null || !(timer.Tag is string))
			{
				timer.Dispose();
				return;
			}

			string file = (string)timer.Tag;
			if (!Helper.CheckFileName(file))
			{
				timer.Dispose();
				return;
			}

			this.Activate();
			if (Helper.IsLearningModuleFileName(file))
				DoDragAndDrop(file);
			else
				OpenConfigFile(file);

			timer.Dispose();
		}

		/// <summary>
		/// GUI logic for Drag and Drop
		/// </summary>
		/// <param name="file">The file.</param>
		/// <remarks>Documented by Dev08, 2009-03-02</remarks>
		public void DoDragAndDrop(string file)
		{
			bool draggedLearningModuleIsOpen = false;
			string appConfigFile = Setup.GlobalConfigPath;
			string usrConfigFile = Setup.UserConfigPath;
			ConnectionStringHandler handler = new ConnectionStringHandler(appConfigFile, usrConfigFile);
			IConnectionString defaultConString = handler.ConnectionStrings.Find(c => c.IsDefault || c.ConnectionType == DatabaseType.Unc);
			TaskDialogResult result;

			//No default connection available
			if (defaultConString == null)
			{
				TaskDialog.MessageBox(Resources.DRAG_NO_DEFAULT_CONNECTION_AVAILABLE_TITLE, Resources.DRAG_NO_DEFAULT_CONNECTION_AVAILABLE_MAININTRODUCTION,
					Resources.DRAG_NO_DEFAULT_CONNECTION_AVAILABLE_CONTENT, TaskDialogButtons.OK, TaskDialogIcons.Error);
				return;
			}
			//User draged a LM from a available UNC connection to the ML
			else if (ConnectionStringHandler.IsFileInUncConnection(file, appConfigFile, usrConfigFile))   //(Path.GetDirectoryName(file) == defaultConString.ConnectionString)
			{
				OpenLearningModule(file);
				return;
			}
			//File already exists
			else if (File.Exists(Path.Combine(defaultConString.ConnectionString, Path.GetFileName(file))))
			{
				//User draged the LM which is currently opened into MemoryLifter. (only possible while user is learning)
				//[Check if the future place of the LM is currently opened (e.g. the older LM is open)]
				if (LearnLogic != null && LearnLogic.CurrentLearningModule != null &&
					LearnLogic.CurrentLearningModule.ConnectionString.ConnectionString == Path.Combine(defaultConString.ConnectionString, Path.GetFileName(file)))
					draggedLearningModuleIsOpen = true;

				TaskDialogResult resultFileExists = ShowFileExistsDragAndDropDialog();

				switch (resultFileExists.CommandButtonsIndex)
				{
					// Replace
					case 0:
						if (draggedLearningModuleIsOpen)
						{
							if (!LearnLogic.CloseLearningModule())
								return;
						}
						try
						{
							ConnectionStringHandler.PutLearningModuleToDefaultUNC(file, defaultConString.ConnectionString, false, true);
						}
						catch (Exception exc)
						{
							TaskDialog.MessageBox(Resources.DRAG_COPY_ERROR_TITLE, Resources.DRAG_COPY_ERROR_MAININTRODUCTION,
								exc.Message, exc.ToString(), string.Empty, string.Empty, TaskDialogButtons.OK, TaskDialogIcons.Error, TaskDialogIcons.Error);
						}

						OpenLearningModule(Path.Combine(defaultConString.ConnectionString, Path.GetFileName(file)));
						break;

					//Just learn
					case 1:
						OpenLearningModule(file);
						break;

					//Cancel
					case 2:
					default:
						break;
				}

				return;
			}
			else if (Path.GetExtension(file) == Helper.DzpExtension)
				result = new TaskDialogResult() { CommandButtonsIndex = 2, VerificationChecked = false };
			else
				result = ShowDefaultDragAndDropDialog();

			//User draged the LM which is currently opened into MemoryLifter. (only possible while user is learning)
			//[Check if currently ]
			if (LearnLogic != null && LearnLogic.CurrentLearningModule != null &&
				LearnLogic.CurrentLearningModule.ConnectionString.ConnectionString == file)
				draggedLearningModuleIsOpen = true;

			switch (result.CommandButtonsIndex)
			{
				//Copy
				case 0:
					try
					{
						ConnectionStringHandler.PutLearningModuleToDefaultUNC(file, defaultConString.ConnectionString, false);
					}
					catch (Exception exc)
					{
						TaskDialog.MessageBox(Resources.DRAG_COPY_ERROR_TITLE, Resources.DRAG_COPY_ERROR_MAININTRODUCTION,
							exc.Message, exc.ToString(), string.Empty, string.Empty, TaskDialogButtons.OK, TaskDialogIcons.Error, TaskDialogIcons.Error);
					}
					OpenLearningModule(Path.Combine(defaultConString.ConnectionString, Path.GetFileName(file)));
					break;

				//Move
				case 1:
					if (draggedLearningModuleIsOpen)
					{
						if (!LearnLogic.CloseLearningModule())
							return;
					}
					try
					{
						ConnectionStringHandler.PutLearningModuleToDefaultUNC(file, defaultConString.ConnectionString, true);
					}
					catch (Exception exc)
					{
						TaskDialog.MessageBox(Resources.DRAG_COPY_ERROR_TITLE, Resources.DRAG_COPY_ERROR_MAININTRODUCTION,
							exc.Message, exc.ToString(), string.Empty, string.Empty, TaskDialogButtons.OK, TaskDialogIcons.Error, TaskDialogIcons.Error);
					}
					OpenLearningModule(Path.Combine(defaultConString.ConnectionString, Path.GetFileName(file)));
					break;

				//Just open
				case 2:
					OpenLearningModule(file);
					break;

				//Cancel
				case 3:
				default:
					break;
			}
		}

		/// <summary>
		/// Shows the default drag and drop dialog.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Documented by Dev08, 2009-03-02</remarks>
		private TaskDialogResult ShowDefaultDragAndDropDialog()
		{

			EmulatedTaskDialog dialog = new EmulatedTaskDialog();
			dialog.Title = Resources.DRAG_DIALOG_TITLE;
			dialog.MainInstruction = Resources.DRAG_DIALOG_MAININTRODUCTION;
			dialog.Content = string.Empty;
			dialog.CommandButtons = Resources.DRAG_DIALOG_BUTTONS;
			dialog.Buttons = TaskDialogButtons.None;
			dialog.MainIcon = TaskDialogIcons.Question;
			dialog.MainImages = new Image[] { Resources.edit_copy, Resources.edit_cut, Resources.MLIcon32, Resources.process_stop };
			dialog.HoverImages = new Image[] { Resources.edit_copy, Resources.edit_cut, Resources.MLIcon32, Resources.process_stop };
			dialog.CenterImages = true;
			dialog.BuildForm();
			DialogResult dialogResult = dialog.ShowDialog();

			TaskDialogResult result = new TaskDialogResult();
			result.CommandButtonsIndex = dialog.CommandButtonClickedIndex;

			return result;
		}

		/// <summary>
		/// Shows the file exists drag and drop dialog.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Documented by Dev08, 2009-03-02</remarks>
		private TaskDialogResult ShowFileExistsDragAndDropDialog()
		{
			EmulatedTaskDialog dialog = new EmulatedTaskDialog();
			dialog.Title = Resources.DRAG_FILE_EXISTS_TITLE;
			dialog.MainInstruction = Resources.DRAG_LM_EXISTS_DIALOG_MAININTRODUCTION;
			dialog.Content = string.Empty;
			dialog.CommandButtons = Resources.DRAG_LM_EXISTS_DIALOG_BUTTONS;
			dialog.Buttons = TaskDialogButtons.None;
			dialog.MainIcon = TaskDialogIcons.Question;
			dialog.MainImages = new Image[] { Resources.edit_find_replace, Resources.MLIcon32, Resources.process_stop };
			dialog.HoverImages = new Image[] { Resources.edit_find_replace, Resources.MLIcon32, Resources.process_stop };
			dialog.CenterImages = true;
			dialog.BuildForm();
			DialogResult dialogResult = dialog.ShowDialog();

			TaskDialogResult result = new TaskDialogResult();
			result.CommandButtonsIndex = dialog.CommandButtonClickedIndex;

			return result;
		}
		#endregion

		#region Menu/Toolbar item events

		/// <summary>
		/// Handles the NewLearningModulePressed event of the lmf control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev08, 2008-12-15</remarks>
		void lmf_NewLearningModulePressed(object sender, EventArgs e)
		{
			newToolStripMenuItem_Click(sender, e);
		}

		/// <summary>
		/// Handles the Click event of the newToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-04-25</remarks>
		/// <remarks>Documented by Dev05, 2009-02-27</remarks>
		private void newToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CreateNewLearningModule();
		}

		private void CreateNewLearningModule()
		{
			try
			{
				if (LearningModulesIndex.WritableConnections.Count <= 0)
					throw new NoWritableConnectionAvailableException();

				//Close the current learning module
				if (LearnLogic.LearningModuleLoaded)
					if (!LearnLogic.CloseLearningModule())
						return;

				//Create the wizard
				Wizard dicWizard = new Wizard();
				dicWizard.HelpFile = MLifter.Classes.Help.HelpPath;
				dicWizard.Text = Resources.NEWDIC_CAPTION;

				SourceSelectionPage sourceSelectionPage = null;
				WelcomePage welcomePage = new WelcomePage();
				SideSettingsPage sideSettingsPage = new SideSettingsPage();

				// only one connection available?
				if (LearningModulesIndex.WritableConnections.Count <= 0)
					throw new NoWritableConnectionAvailableException();
				else if (LearningModulesIndex.WritableConnections.Count > 1)
				{
					sourceSelectionPage = new SourceSelectionPage();
					dicWizard.Pages.Add(sourceSelectionPage);
				}
				dicWizard.Pages.Add(welcomePage);
				dicWizard.Pages.Add(sideSettingsPage);

				//Show the Wizards
				if (dicWizard.ShowDialog() == DialogResult.OK)
				{
					try
					{
						newLM = true;
						IConnectionString connectionString;
						if (sourceSelectionPage != null)
							connectionString = sourceSelectionPage.ConnectionString;
						else
							connectionString = LearningModulesIndex.WritableConnections[0];

						string dicName = welcomePage.DictionaryName;

						if (!LearningModulesIndex.ConnectionUsers.ContainsKey(connectionString))
						{
							FolderIndexEntry folderEntry = new FolderIndexEntry(connectionString is UncConnectionStringBuilder ? connectionString.ConnectionString : string.Empty,
								connectionString.Name, connectionString, null, Setup.SyncedModulesPath, LearnLogic.GetLoginDelegate, LearnLogic.DataAccessErrorDelegate);
							LearningModulesIndex.ConnectionUsers[connectionString] = folderEntry.CurrentUser;
						}
						LearnLogic.User.SetBaseUser(LearningModulesIndex.ConnectionUsers[connectionString]);
						ConnectionStringStruct connectionStringStruct;
						using (MLifter.DAL.Interfaces.IDictionary newDic = LearnLogic.User.CreateLearningModule(welcomePage.DictionaryCategory.Id, welcomePage.DictionaryName))
						{
							newDic.Author = welcomePage.DictionaryAuthor;
							newDic.Category = welcomePage.DictionaryCategory;
							newDic.Description = welcomePage.DictionaryDescription;

							newDic.DefaultSettings.AnswerCaption = sideSettingsPage.AnswerTitle;
							newDic.DefaultSettings.QuestionCaption = sideSettingsPage.QuestionTitle;
							newDic.DefaultSettings.AnswerCulture = sideSettingsPage.AnswerCulture;
							newDic.DefaultSettings.QuestionCulture = sideSettingsPage.QuestionCulture;
							newDic.Save();

							connectionStringStruct = new ConnectionStringStruct(connectionString.ConnectionType == DatabaseType.Unc ? DatabaseType.MsSqlCe : connectionString.ConnectionType,
								newDic.Connection, newDic.Id, LearnLogic.User.SessionId);
							connectionStringStruct.LearningModuleFolder = connectionString.ConnectionString;
						}
						LearningModulesIndexEntry entry = new LearningModulesIndexEntry();
						entry.ConnectionString = connectionStringStruct;
						entry.Author = welcomePage.DictionaryAuthor;
						entry.DisplayName = welcomePage.DictionaryName;
						entry.Description = welcomePage.DictionaryDescription;
						entry.Connection = connectionString;
						entry.User = LearningModulesIndex.ConnectionUsers[connectionString];

						OpenLearningModule(entry);

						LearnLogic.SaveLearningModule();
						this.Activate(); //[ML-763] Window is not focused after creation of a new dictionary
						OnLMOptionsChanged();

						newLM = false;
					}
					catch (Exception exp)
					{
						Trace.WriteLine(exp.ToString());
						TaskDialog.MessageBox(Resources.NEW_DICT_CAPTION, Resources.NEW_DICT_CAPTION, Resources.NEW_DICT_TEXT, exp.ToString(),
							string.Empty, string.Empty, TaskDialogButtons.OK, TaskDialogIcons.Error, TaskDialogIcons.Error);
					}
				}
				else
					ShowLearningModulesPage();
			}
			catch (NoWritableConnectionAvailableException)
			{
				TaskDialog.MessageBox(Resources.NEW_DIC_NO_CONNECTION_CAPTION, Resources.NEW_DIC_NO_CONNECTION_CAPTION, Resources.NEW_DIC_NO_CONNECTION_TEXT, TaskDialogButtons.OK, TaskDialogIcons.Error);
			}
		}

		/// <summary>
		/// Handles the Click event of the openToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-04-25</remarks>
		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (LearnLogic.CloseLearningModule())
				ShowLearningModulesPage();

			//OpenDialog.InitialDirectory = Settings.Default.DicDir;
			//if (OpenDialog.ShowDialog() == DialogResult.OK)
			//{
			//    OpenLearningModule(OpenDialog.FileName);
			//}
		}

		/// <summary>
		/// Handles the Click event of the closeToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-04-25</remarks>
		private void closeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				LearnLogic.CloseLearningModule();
			}
			catch (ConnectionLostException) { ShowConnectionLostMessage(); }
		}

		/// <summary>
		/// Handles the Click event of the saveAsToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-04-25</remarks>
		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!File.Exists(LearnLogic.Dictionary.DictionaryDAL.Connection))
				return;

			EmulatedTaskDialog dialog = null;
			if (Properties.Settings.Default.ShowBackupDialog)
			{
				// display notice that LM must be closed and reopened  to make backup
				dialog = new EmulatedTaskDialog();
				dialog.Title = Resources.MAINFORM_CREATE_BACKUP_DIALOG_TITLE;
				dialog.MainInstruction = Resources.MAINFORM_CREATE_BACKUP_DIALOG_MAIN;
				dialog.CommandButtons = Resources.MAINFORM_CREATE_BACKUP_DIALOG_BUTTONS;
				dialog.Buttons = TaskDialogButtons.None;
				dialog.MainIcon = TaskDialogIcons.Information;
				dialog.CenterImages = true;
				dialog.VerificationText = Resources.MAINFORM_CREATE_BACKUP_DIALOG_VERIFICATION;
				dialog.BuildForm();

				dialog.ShowDialog();
				if (dialog.CommandButtonClickedIndex == 1) return;
			}

			SaveDialog.Title = Resources.MAINFORM_CREATE_BACKUP_DIALOG_TITLE;
			SaveDialog.FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), Path.GetFileName(LearnLogic.Dictionary.DictionaryPath));
			SaveDialog.Filter = String.Format(Resources.MAINFORM_CREATE_BACKUP_DIALOG_FILTER, DAL.Helper.EmbeddedDbExtension);


			if (SaveDialog.ShowDialog() == DialogResult.OK)
			{
				string sourcePath = LearnLogic.Dictionary.DictionaryPath;
				LearnLogic.CloseLearningModule();
				this.Enabled = false;
				Copy copy = new Copy();
				copy.SaveCopyAs(sourcePath, SaveDialog.FileName);
				this.Enabled = true;
				this.OpenLearningModule(RecentLearningModules.MostRecentLearningModule);
				//save "don't show this message again"
				if (Properties.Settings.Default.ShowBackupDialog == true && dialog != null)
				{
					if (dialog.VerificationCheckBoxChecked)
					{
						Properties.Settings.Default.ShowBackupDialog = false;
						Properties.Settings.Default.Save();
					}
				}
			}
		}

		/// <summary>
		/// Handles the Click event of the fileImportToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev09, 2009-03-05</remarks>
		private void fileImportToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ImportForm importer = new ImportForm();
			importer.ShowDialog();
			OnLMOptionsChanged();
		}

		/// <summary>
		/// Handles the Click event of the exportToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-04-25</remarks>
		private void exportToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (LearnLogic.Dictionary.DictionaryDAL.ContentProtected)
				return;

			ExportForm exportForm = new ExportForm();
			exportForm.ShowDialog();
		}

		/// <summary>
		/// Handles the Click event of the printToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-04-25</remarks>
		private void printToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DialogResult result = DialogResult.None;

			do
			{
				if (result == DialogResult.Ignore)
					Settings.Default.UsePrintWizard = !Settings.Default.UsePrintWizard;

				if (Settings.Default.UsePrintWizard)
				{
					//new print wizard
					Wizard printWizard = new Wizard();
					printWizard.Text = Resources.PRINT_WIZARD_TITLE;
					printWizard.HelpFile = MLifter.Classes.Help.HelpPath;

					Controls.Wizards.Print.WelcomePage wPage = new MLifter.Controls.Wizards.Print.WelcomePage();
					(wPage.IndividualPage as Controls.Wizards.Print.IndividualSelectionPage).Dictionary = LearnLogic.Dictionary;
					(wPage.ChapterPage as Controls.Wizards.Print.ChapterSelectionPage).Dictionary = LearnLogic.Dictionary;
					(wPage.BoxPage as Controls.Wizards.Print.BoxSelectionPage).Dictionary = LearnLogic.Dictionary;
					printWizard.Pages.Add(wPage);

					Controls.Wizards.Print.PrintPage pPage = new MLifter.Controls.Wizards.Print.PrintPage();
					pPage.StyleHandler = MainForm.styleHandler;
					pPage.Dictionary = LearnLogic.Dictionary;
					printWizard.Pages.Add(pPage);

					printWizard.ShowDialog();
					result = printWizard.DialogResult;
				}
				else
				{
					//old print dialog
					PrintForm PrintForm = new PrintForm();
					PrintForm.ShowDialog();
					result = PrintForm.DialogResult;
				}
			}
			while (result == DialogResult.Ignore);
		}

		/// <summary>
		/// Handles the Click event of the propertiesToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-04-25</remarks>
		private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!learnLogic.UserSessionAlive)
				return;

			if (PropertiesForm.LoadDictionary(LearnLogic.Dictionary, MLifter.Classes.Help.HelpPath))
			{
				UpdateLearningModesDropDownList();
				OnLMOptionsChanged();
			}
		}

		/// <summary>
		/// Handles the Click event of the exitToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-04-25</remarks>
		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			IsInTray = false;
			this.Close();
		}

		/// <summary>
		/// Handles the Click event of the addCardToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-04-25</remarks>
		private void addCardToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ShowMaintain(true);
		}

		/// <summary>
		/// Handles the Click event of the maintainCardsToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-04-25</remarks>
		private void maintainCardsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ShowMaintain(false);
		}

		/// <summary>
		/// Handles the Click event of the editChaptersToolStripMenuItem1 control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-04-25</remarks>
		private void editChaptersToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			if (!learnLogic.UserSessionAlive)
				return;

			SetupChaptersForm frmChapters = new SetupChaptersForm();
			frmChapters.ShowDialog();
			OnLMOptionsChanged();
		}

		/// <summary>
		/// Handles the Click event of the selectChaptersToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-04-25</remarks>
		private void selectChaptersToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!learnLogic.UserSessionAlive)
				return;

			TChapterForm ChapterForm = new TChapterForm();
			if (ChapterForm.ShowDialog() == DialogResult.OK)
				OnLMOptionsChanged();
		}

		/// <summary>
		/// Selects the learning chapters.
		/// </summary>
		/// <param name="chapterIds">The chapter ids.</param>
		/// <remarks>Documented by Dev02, 2009-06-26</remarks>
		public void SelectLearningChapters(int[] chapterIds)
		{
			if (!LearnLogic.LearningModuleLoaded || LearnLogic.CurrentLearningModule == null || chapterIds.Length < 1)
				return;

			List<int> validChapterIds = new List<int>();
			foreach (IChapter chapter in LearnLogic.Dictionary.Chapters.Chapters)
				validChapterIds.Add(chapter.Id);

			LearnLogic.Dictionary.QueryChapters.Clear();
			foreach (int chapterId in chapterIds)
			{
				if (validChapterIds.Contains(chapterId))
					LearnLogic.Dictionary.QueryChapters.Add(chapterId);
			}

			OnLMOptionsChanged();
		}

		/// <summary>
		/// Handles the Click event of the showStatisticsToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-04-25</remarks>
		private void showStatisticsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!learnLogic.UserSessionAlive)
				return;

			TStatsForm.ShowStats();
		}

		/// <summary>
		/// Handles the Click event of the resetStatisticsToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-04-25</remarks>
		private void restartLearningToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!learnLogic.UserSessionAlive)
				return;

			if (MessageBox.Show(Resources.RESTART_WARNING_TEXT, Resources.RESTART_WARNING_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
				LearnLogic.ResetLearningProgress();
		}

		/// <summary>
		/// Handles the Click event of the showBoxSizeToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-04-25</remarks>
		private void showBoxSizeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!learnLogic.UserSessionAlive)
				return;

			SetupBoxesForm frmBoxSize = new SetupBoxesForm();
			frmBoxSize.ShowDialog();
			OnLMOptionsChanged();
		}

		/// <summary>
		/// Handles the Click event of the learningOptionsToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-04-25</remarks>
		private void learningOptionsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!learnLogic.UserSessionAlive)
				return;

			QueryOptionsForm frmOptions = new QueryOptionsForm();
			if (frmOptions.ShowDialog() == DialogResult.OK)
			{
				OnLMOptionsChanged();
			}
		}

		/// <summary>
		/// Handles the Click event of the characterMapToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-04-25</remarks>
		private void characterMapToolStripMenuItem_Click(object sender, EventArgs e)
		{
			characterMapComponent.Visible = !characterMapComponent.Visible;
			this.Focus();
		}

		/// <summary>
		/// Handles the VisibleChanged event of the characterMapComponent control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-05-08</remarks>
		private void characterMapComponent_VisibleChanged(object sender, EventArgs e)
		{
			characterMapToolStripMenuItem.Checked = characterMapComponent.Visible;
			toolStripButtonCharMap.Checked = characterMapComponent.Visible;
		}


		/// <summary>
		/// Handles the Click event of the studyModeToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-04-25</remarks>
		private void slideshowModeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!learnLogic.UserSessionAlive)
				return;

			LearnLogic.SlideShow = !LearnLogic.SlideShow;
			slideShowToolStripButton.Checked = LearnLogic.SlideShow;
			slideShowModeToolStripMenuItem.Checked = LearnLogic.SlideShow;
			string tooltip = string.Empty;
			if (LearnLogic.SlideShow)
			{
				tooltip = Resources.RETURN_TO_LEARNING_MODE;
				if (Resources.RETURN_TO_LEARNING_MODE.Length < Resources.SWITCH_TO_SLIDESHOW_MODE.Length)  //add whitespace
				{
					do
					{
						tooltip += " ";
					} while (tooltip.Length < Resources.RETURN_TO_LEARNING_MODE.Length);
				}
			}
			else
			{
				tooltip = Resources.SWITCH_TO_SLIDESHOW_MODE;
				if (Resources.SWITCH_TO_SLIDESHOW_MODE.Length < Resources.RETURN_TO_LEARNING_MODE.Length)  //add whitespace
				{
					do
					{
						tooltip += " ";
					} while (tooltip.Length < Resources.RETURN_TO_LEARNING_MODE.Length);
				}
			}
			slideShowToolStripButton.ToolTipText = tooltip;
			ttToolStripTop.SetToolTip(toolStripTop, slideShowToolStripButton.ToolTipText);
			toolStripTop.Refresh();
		}

		/// <summary>
		/// Handles the Click event of the snoozeModeMemorylifterToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-04-25</remarks>
		private void snoozeModeMemorylifterToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!learnLogic.UserSessionAlive)
				return;

			LearnLogic.SnoozeModeActive = true;
		}

		/// <summary>
		/// Handles the Click event of the styleEditorToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-04-25</remarks>
		private void styleEditorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!learnLogic.UserSessionAlive)
				return;

			StyleEditor previewer = new StyleEditor();
			previewer.EditXSL(LearnLogic.CurrentCardID);
			OnLMOptionsChanged();
		}

		/// <summary>
		/// Handles the Click event of the changeLMFolderToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-04-25</remarks>
		private void changeLMFolderToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!learnLogic.UserSessionAlive)
				return;

			string path = Settings.Default.DicDir;
			FolderBrowserDialog DirDialog = new FolderBrowserDialog();
			DirDialog.SelectedPath = path;

			if (DirDialog.ShowDialog() == DialogResult.OK)
				path = DirDialog.SelectedPath;

			Setup.CreateDefaultConfig(path);
		}

		/// <summary>
		/// Handles the Click event of the memoryLifterHelpToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-04-25</remarks>
		private void memoryLifterHelpToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Help.ShowHelp(this, MLifter.Classes.Help.HelpPath, HelpNavigator.TableOfContents);
		}

		/// <summary>
		/// Handles the Click event of the aboutMemoryLifterToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-04-25</remarks>
		private void aboutMemoryLifterToolStripMenuItem_Click(object sender, EventArgs e)
		{
			AboutBox AboutMLifter = new MLifter.AboutBox();
			AboutMLifter.ShowDialog();
			AboutMLifter.Dispose();
		}

		/// <summary>
		/// Handles the Click event of the firstStepsToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-04-25</remarks>
		private void firstStepsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Help.ShowHelp(this, MLifter.Classes.Help.HelpPath, HelpNavigator.Topic, Resources.HELP_FIRSTSTEPS);
		}

		/// <summary>
		/// Handles the Click event of the fAQToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-04-25</remarks>
		private void fAQToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				System.Diagnostics.Process.Start(Resources.URL_MLIFTER_FAQ);
			}
			catch
			{
				MessageBox.Show(Resources.ERROR_LAUNCH_EXTERNAL_APPLICATION_TEXT, Resources.ERROR_LAUNCH_EXTERNAL_APPLICATION_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}

		/// <summary>
		/// Handles the Click event of the homepageToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-04-25</remarks>
		private void homepageToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				System.Diagnostics.Process.Start(Resources.URL_MLIFTER_HOMEPAGE);
			}
			catch
			{
				MessageBox.Show(Resources.ERROR_LAUNCH_EXTERNAL_APPLICATION_TEXT, Resources.ERROR_LAUNCH_EXTERNAL_APPLICATION_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}
		
		/// <summary>
		/// Handles the Click event of the recommendToAFriendToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-04-25</remarks>
		private void recommendToAFriendToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				System.Diagnostics.Process.Start(Resources.URL_MLIFTER_REFERPAGE);
			}
			catch
			{
				MessageBox.Show(Resources.ERROR_LAUNCH_EXTERNAL_APPLICATION_TEXT, Resources.ERROR_LAUNCH_EXTERNAL_APPLICATION_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}



		#endregion

		#region Menu helper functions

		/// <summary>
		/// Opens the config file.
		/// </summary>
		/// <param name="file">The file.</param>
		/// <remarks>Documented by Dev08, 2009-02-27</remarks>
		public void OpenConfigFile(string file)
		{
			EmulatedTaskDialog dialog = new EmulatedTaskDialog();
			dialog.Owner = this;
			dialog.StartPosition = FormStartPosition.CenterParent;
			dialog.Title = Resources.DRAG_CFG_DIALOG_TITLE;
			dialog.MainInstruction = Resources.DRAG_CFG_DIALOG_MAIN;
			dialog.Content = Resources.DRAG_CFG_DIALOG_CONTENT;
			dialog.CommandButtons = Resources.DRAG_CFG_DIALOG_BUTTONS;
			dialog.Buttons = TaskDialogButtons.None;
			dialog.MainIcon = TaskDialogIcons.Question;
			dialog.MainImages = new Image[] { Resources.applications_system, Resources.process_stop };
			dialog.HoverImages = new Image[] { Resources.applications_system, Resources.process_stop };
			dialog.CenterImages = true;
			dialog.BuildForm();
			DialogResult dialogResult = dialog.ShowDialog();

			//[ML-1773] Imported connection file is not saved on stick
			string appConfigFile = Setup.GlobalConfigPath;
			string usrConfigFile = Setup.UserConfigPath;

			switch (dialog.CommandButtonClickedIndex)
			{
				//Import
				case 0:
					int successfulImportedConnections = 0;
					try
					{
						successfulImportedConnections = ConnectionStringHandler.ImportConfigFile(file, appConfigFile, usrConfigFile);
					}
					catch (InvalidConfigFileException)
					{
						TaskDialog.MessageBox(Resources.DRAG_INVALID_CFG_FILE_TITLE, Resources.DRAG_INVALID_CFG_FILE_MAININSTRUCTION, Resources.DRAG_INVALID_CFG_FILE_CONTENT,
											  TaskDialogButtons.OK, TaskDialogIcons.Error);
						return;
					}
					catch (Exception exc)
					{
						TaskDialog.MessageBox(Resources.DRAG_CFG_GENERAL_ERROR_TITLE, Resources.DRAG_CFG_GENERAL_ERROR_MAININSTRUCTION,
							exc.Message, exc.ToString(), string.Empty, string.Empty, TaskDialogButtons.OK, TaskDialogIcons.Error, TaskDialogIcons.Error);
						return;
					}

					if (successfulImportedConnections == 0)
						TaskDialog.MessageBox(Resources.DRAG_CFG_SUCCESS_TITLE, Resources.DRAG_CFG_SUCCESS_MAIN_NOTHING_IMPORTED, 
							Resources.DRAG_CFG_SUCCESS_MAIN_NOTHING_IMPORTED_DETAIL, TaskDialogButtons.OK, TaskDialogIcons.Information);
					else if (successfulImportedConnections == 1)
						TaskDialog.MessageBox(Resources.DRAG_CFG_SUCCESS_TITLE, Resources.DRAG_CFG_SUCCESS_MAIN, Resources.DRAG_CFG_SUCCESS_CONTENT_SING,
							TaskDialogButtons.OK, TaskDialogIcons.Information);
					else
						TaskDialog.MessageBox(Resources.DRAG_CFG_SUCCESS_TITLE, Resources.DRAG_CFG_SUCCESS_MAIN, string.Format(Resources.DRAG_CFG_SUCCESS_CONTENT_PLUR,
							successfulImportedConnections), TaskDialogButtons.OK, TaskDialogIcons.Information);

					break;
				//Cancel
				case 1:
				default:
					return;
			}
		}

		/// <summary>
		/// Opens the dictionary-file.
		/// </summary>
		/// <param name="filename">The filename.</param>
		/// <remarks>Documented by Dev05, 2009-01-26</remarks>
		public void OpenLearningModule(string filename)
		{
			LearningModulesIndexEntry entry = new LearningModulesIndexEntry();
			entry.ConnectionString = new ConnectionStringStruct(filename.EndsWith(Helper.EmbeddedDbExtension) ? DatabaseType.MsSqlCe : DatabaseType.Xml, filename, true);
			entry.DisplayName = Path.GetFileName(filename);
			entry.ConnectionName = Path.GetDirectoryName(filename);

			OpenLearningModule(entry);
		}

		/// <summary>
		/// Opens the dictionary.
		/// </summary>
		/// <param name="connection">The connection.</param>
		/// <remarks>Documented by Dev05, 2008-12-05</remarks>
		public void OpenLearningModule(LearningModulesIndexEntry module)
		{
			if (module.DisplayName == string.Empty && module.ConnectionString.Typ == DatabaseType.MsSqlCe)
				module.DisplayName = Path.GetFileNameWithoutExtension(module.ConnectionString.ConnectionString);

			bool converting = false;
			try
			{
				this.Enabled = false;
				learnLogic.OpenLearningModule(module);
			}
			#region Unpacking
			catch (NeedToUnPackException exp)
			{
				string fileName = exp.module.ConnectionString.ConnectionString;
				this.Enabled = false;
				Pack frmPack = new Pack();
				MLifter.Classes.Convert converter = new MLifter.Classes.Convert();
				converter.ConvertingFinished += new EventHandler<MLifter.Classes.ConvertingEventArgs>(converter_ConvertingFinished);
				try
				{
					fileName = converter.FromOdx(frmPack.UnpackDic(fileName), true);
				}
				catch
				{
					this.Enabled = true;
					return;
				}
				converting = true;
			}
			#endregion
			#region ODF/ODX converting
			catch (IsOdfFormatException exp)
			{
				MLifter.Controls.TaskDialog.ShowTaskDialogBox(Resources.CONVERT_REQUEST_CAPTION, Resources.CONVERT_REQUEST_TEXT, Resources.CONVERT_REQUEST_CONTENT,
					string.Empty, string.Empty, string.Empty, string.Empty, Resources.CONVERT_REQUEST_OPTION_YES + "|" + Resources.CONVERT_REQUEST_OPTION_NO,
					MLifter.Controls.TaskDialogButtons.None, MLifter.Controls.TaskDialogIcons.Question, MLifter.Controls.TaskDialogIcons.Warning);

				if (MLifter.Controls.TaskDialog.CommandButtonResult != 0)
					return;

				string fileName = exp.module.ConnectionString.ConnectionString;
				MLifter.Classes.Convert converter = new MLifter.Classes.Convert();
				converter.ConvertingFinished += new EventHandler<MLifter.Classes.ConvertingEventArgs>(converter_ConvertingFinished);
				try
				{
					fileName = converter.FromOdx(converter.FromOdf(fileName), true);
				}
				catch
				{
					return;
				}
				converting = true;
			}
			catch (IsOdxFormatException exp)
			{
				MLifter.Controls.TaskDialog.ShowTaskDialogBox(Resources.CONVERT_REQUEST_CAPTION, Resources.CONVERT_REQUEST_TEXT, Resources.CONVERT_REQUEST_CONTENT,
					string.Empty, string.Empty, string.Empty, string.Empty, Resources.CONVERT_REQUEST_OPTION_YES + "|" + Resources.CONVERT_REQUEST_OPTION_NO,
					MLifter.Controls.TaskDialogButtons.None, MLifter.Controls.TaskDialogIcons.Question, MLifter.Controls.TaskDialogIcons.Warning);

				if (MLifter.Controls.TaskDialog.CommandButtonResult != 0)
					return;

				string fileName = exp.module.ConnectionString.ConnectionString;
				MLifter.Classes.Convert converter = new MLifter.Classes.Convert();
				converter.ConvertingFinished += new EventHandler<MLifter.Classes.ConvertingEventArgs>(converter_ConvertingFinished);
				try
				{
					fileName = converter.FromOdx(fileName, false);
				}
				catch
				{
					return;
				}
				converting = true;
			}
			#endregion
			#region Other exceptions (LM can't be opened)
			catch (DipNotSupportedException)
			{
				TaskDialog.MessageBox(Resources.DIP_NOT_SUPPORTED_CAPTION, Resources.DIP_NOT_SUPPORTED_CAPTION, Resources.DIP_NOT_SUPPORTED_TEXT, TaskDialogButtons.OK, TaskDialogIcons.Error);
			}
			catch (MLifter.DAL.InvalidDictionaryException exp)
			{
				TaskDialog.MessageBox(Resources.DIC_ERROR_LOADING_CAPTION, Resources.DIC_ERROR_LOADING_CAPTION, string.Format(Resources.DIC_ERROR_LOADING_TEXT, module.DisplayName),
					exp.ToString(), string.Empty, string.Empty, TaskDialogButtons.OK, TaskDialogIcons.Error, TaskDialogIcons.Error);
			}
			catch (MLifter.DAL.DictionaryNotDecryptedException exp)
			{
				TaskDialog.MessageBox(Resources.DIC_ERROR_NOT_DECRYPTED_CAPTION, Resources.DIC_ERROR_NOT_DECRYPTED_CAPTION, Resources.DIC_ERROR_NOT_DECRYPTED_TEXT,
					exp.ToString(), string.Empty, string.Empty, TaskDialogButtons.OK, TaskDialogIcons.Error, TaskDialogIcons.Error);
			}
			catch (System.Xml.XmlException exp)
			{
				TaskDialog.MessageBox(Resources.DIC_ERROR_LOADING_CAPTION, Resources.DIC_ERROR_LOADING_CAPTION, string.Format(Resources.DIC_ERROR_LOADING_TEXT, module.DisplayName),
					exp.ToString(), string.Empty, string.Empty, TaskDialogButtons.OK, TaskDialogIcons.Error, TaskDialogIcons.Error);
			}
			catch (System.IO.IOException exp)
			{
				TaskDialog.MessageBox(Resources.DIC_ERROR_LOADING_LOCKED_CAPTION, Resources.DIC_ERROR_LOADING_LOCKED_CAPTION, string.Format(Resources.DIC_ERROR_LOADING_LOCKED_TEXT, module.DisplayName),
					exp.ToString(), string.Empty, string.Empty, TaskDialogButtons.OK, TaskDialogIcons.Error, TaskDialogIcons.Error);
			}
			catch (DatabaseVersionNotSupported exp)
			{
				if (!IsMLifterOlder(exp.DalVersion, exp.VersionInfo))
				{
					if (module.ConnectionString.Typ == DatabaseType.MsSqlCe)
					{
						DialogResult res = DialogResult.OK;
						if (!exp.SilentUpgrade)
							res = TaskDialog.MessageBox(Resources.DBVERSION_NOT_SUPPORTED_CAPTION, Resources.DBVERSION_NOT_SUPPORTED_MAIN, Resources.DBVERSION_NOT_SUPPORTED_CONTENT,
								TaskDialogButtons.OKCancel, TaskDialogIcons.Warning);

						if (res != DialogResult.OK)
							return;

						try
						{
							if (LearnLogic.User.UpgradeDatabase(exp.DatabaseVersion))
							{
								learnLogic.OpenLearningModule(module);
							}
							else
								TaskDialog.MessageBox(Resources.DBVERSION_NOT_SUPPORTED_UPGRADE_FAILED_CAPTION, Resources.DBVERSION_NOT_SUPPORTED_UPGRADE_FAILED_MAIN, Resources.DBVERSION_NOT_SUPPORTED_UPGRADE_FAILED_CONTENT,
									TaskDialogButtons.OK, TaskDialogIcons.Error);
						}
						catch (Exception exp2)
						{
							TaskDialog.MessageBox(Resources.DIC_ERROR_LOADING_CAPTION, Resources.DIC_ERROR_LOADING_CAPTION, string.Format(Resources.DIC_ERROR_LOADING_TEXT, module.DisplayName),
								exp2.ToString(), string.Empty, string.Empty, TaskDialogButtons.OK, TaskDialogIcons.Error, TaskDialogIcons.Error);
						}
					}
					else
					{
						TaskDialog.MessageBox(Resources.TASK_DIALOG_IncompatibleDatabaseVersion_Title, Resources.TASK_DIALOG_IncompatibleDatabaseVersion_Caption, Resources.TASK_DIALOG_IncompatibleDatabaseVersion_Text,
							 TaskDialogButtons.OK, TaskDialogIcons.Error);
					}
				}
				else
				{
					TaskDialog.MessageBox(Resources.TASK_DIALOG_MemoryLifterTooOld_Title, Resources.TASK_DIALOG_MemoryLifterTooOld_Caption, Resources.TASK_DIALOG_MemoryLifterTooOld_Text,
						 TaskDialogButtons.OK, TaskDialogIcons.Error);
				}
			}
			catch (ProtectedLearningModuleException)
			{
				TaskDialog.MessageBox(
					Resources.TASK_DIALOG_LEARNING_MODULE_PROTECTED_TITLE,
					Resources.TASK_DIALOG_LEARNING_MODULE_PROTECTED_CAPTION,
					Resources.TASK_DIALOG_LEARNING_MODULE_PROTECTED_TEXT,
					TaskDialogButtons.OK, TaskDialogIcons.Error);
			}
			catch (NoValidUserException exp) { Trace.WriteLine(exp.ToString()); }
			catch (NotEnoughtDiskSpaceException)
			{
				TaskDialog.MessageBox(Resources.TASK_DIALOG_DISK_FULL_TITLE, Resources.TASK_DIALOG_DISK_FULL_CAPTION, Resources.TASK_DIALOG_DISK_FULL_TEXT, TaskDialogButtons.OK, TaskDialogIcons.Error);
			}
			catch (ServerOfflineException)
			{
				TaskDialog.MessageBox(Resources.DIC_ERROR_LOADING_CAPTION, Resources.DIC_ERROR_LOADING_CAPTION, Resources.DIC_LOADING_ERROR_OFFLINE,
					TaskDialogButtons.OK, TaskDialogIcons.Error);
			}
			catch (Exception exp)
			{
				TaskDialog.MessageBox(Resources.DIC_ERROR_LOADING_CAPTION, Resources.DIC_ERROR_LOADING_CAPTION, string.Format(Resources.DIC_ERROR_LOADING_TEXT, module.DisplayName),
					exp.ToString(), string.Empty, string.Empty, TaskDialogButtons.OK, TaskDialogIcons.Error, TaskDialogIcons.Error);
			}
			#endregion
			finally
			{
				if (!converting)
				{
					this.Enabled = true;
					this.Activate(); //[ML-850] It is important to set the focus for the modal taskdialog to find the right parent
				}
			}
		}

		/// <summary>
		/// Determines whether is Mlifter older the specified dal version.
		/// </summary>
		/// <param name="dalVersion">The dal version.</param>
		/// <param name="supportedVersions">The supported versions.</param>
		/// <returns>
		/// 	<c>true</c> if [is M lifter older] [the specified dal version]; otherwise, <c>false</c>.
		/// </returns>
		/// <remarks>Documented by Dev05, 2009-07-10</remarks>
		private bool IsMLifterOlder(Version dalVersion, List<DataLayerVersionInfo> supportedVersions)
		{
			foreach (DataLayerVersionInfo version in supportedVersions)
			{
				switch (version.Type)
				{
					case VersionType.LowerBound:
						if (version.Version <= dalVersion)
							return false;
						break;
					case VersionType.UpperBound:
						if (version.Version <= dalVersion)
							return false;
						break;
					case VersionType.Equal:
						if (version.Version == dalVersion)
							return false;
						break;
					default:
						throw new ArgumentException();
				}
			}

			if (!supportedVersions.Exists(v => v.Type == VersionType.LowerBound) && !supportedVersions.Exists(v => v.Type == VersionType.UpperBound))
			{
				return supportedVersions.TrueForAll(v => v.Version > dalVersion);
			}

			return true;
		}

		/// <summary>
		/// Handles the ConvertingFinished event of the converter control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="MLifter.Classes.ConvertingEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2009-03-03</remarks>
		private void converter_ConvertingFinished(object sender, MLifter.Classes.ConvertingEventArgs e)
		{
			(sender as MLifter.Classes.Convert).ConvertingFinished -= converter_ConvertingFinished;

			this.Invoke((MethodInvoker)delegate { OpenLearningModule(e.ConvertedFile); });
		}

		/// <summary>
		/// Handles the LearningModuleSyncRequest event of the LearnLogic control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2009-03-02</remarks>
		void LearnLogic_LearningModuleSyncRequest(object sender, EventArgs e)
		{
			SyncLearningModule(sender as LearningModulesIndexEntry);
		}

		/// <summary>
		/// Syncs the current module.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <remarks>Documented by Dev05, 2009-04-29</remarks>
		private void SyncLearningModule(LearningModulesIndexEntry entry)
		{
			try
			{
				if (entry.ConnectionString.ProtectedLm == true)
					throw new SynchronizationFailedException(new Exception(Resources.TASK_DIALOG_LEARNING_MODULE_PROTECTED_CAPTION)); //Not supported in ML >2.3
				LearningModulesPage.Sync(entry, (MLifter.Controls.LearningModulesPage.LearningModulesPage.SyncStatusReportingDelegate)SyncStatusUpdate, 
					Setup.SyncedModulesPath);
			}
			catch (NotEnoughtDiskSpaceException)
			{
				TaskDialog.MessageBox(Resources.TASK_DIALOG_DISK_FULL_TITLE, Resources.TASK_DIALOG_DISK_FULL_CAPTION, Resources.TASK_DIALOG_DISK_FULL_TEXT, TaskDialogButtons.OK, TaskDialogIcons.Error);
			}
			catch (SynchronizationFailedException)
			{
				TaskDialog.MessageBox(Resources.SYNCHRONIZATION_FAILED_TITLE, Resources.SYNCHRONIZATION_SYNC_FAILED_MAIN,
					Resources.SYNCHRONIZATION_FAILED_CONTENT, TaskDialogButtons.OK, TaskDialogIcons.Warning);
			}
		}

		LoadStatusMessage syncStatusMessage = new LoadStatusMessage(string.Empty, 100, false);
		private void SyncStatusUpdate(double percentage, string message)
		{
			if (syncStatusMessage == null)
				syncStatusMessage = new LoadStatusMessage(string.Empty, 100, false);
			if (!syncStatusMessage.Visible)
				syncStatusMessage.Show();

			syncStatusMessage.Invoke((MethodInvoker)delegate()
				{
					syncStatusMessage.InfoMessage = message;
					if (percentage <= 100 && percentage >= 0)
					{
						syncStatusMessage.EnableProgressbar = true;
						syncStatusMessage.SetProgress(Convert.ToInt32(percentage));
					}
					else
					{
						syncStatusMessage.EnableProgressbar = false;
						Application.DoEvents();
					}
				});
			if (percentage >= 100)
			{
				syncStatusMessage.Close();
				syncStatusMessage.Dispose();
				syncStatusMessage = null;
			}
		}

		private void DataAccessError(object sender, Exception exp)
		{
			throw exp;
		}

		/// <summary>
		/// Shows the maintain dialog.
		/// </summary>
		/// <param name="AddCard">if set to <c>true</c> [add card], else maintain existing cards.</param>
		/// <remarks>Documented by Dev02, 2008-05-14</remarks>
		private void ShowMaintain(bool AddCard)
		{
			if (!learnLogic.UserSessionAlive)
				return;

			MaintainCardForm maintainCardForm = new MaintainCardForm(AddCard ? -1 : LearnLogic.CurrentCardID);
			if (maintainCardForm.ShowDialog() == DialogResult.OK)
				OnLMOptionsChanged();
		}

		/// <summary>
		/// Called when [learning module or learning options were changed].
		/// </summary>
		/// <remarks>Documented by Dev02, 2008-04-25</remarks>
		private void OnLMOptionsChanged()
		{
			if (MainForm.LearnLogic != null)
				MainForm.LearnLogic.OnLearningModuleOptionsChanged();
		}

		/// <summary>
		/// Handles the Click event of the continueToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-04-29</remarks>
		private void continueToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LearnLogic.SnoozeModeActive = false;
		}
		#endregion

		/// <summary>
		/// Handles the Tick event of the timerSnooze control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-04-29</remarks>
		private void timerSnooze_Tick(object sender, EventArgs e)
		{
			//subtract a minute and check if it is time to stop the tray mode
			if (--trayWaitMinutesLeft <= 0)
				continueToolStripMenuItem.PerformClick();
		}

		/// <summary>
		/// Handles the Opening event of the menuStripTray control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-04-29</remarks>
		private void menuStripTray_Opening(object sender, CancelEventArgs e)
		{
			//show the target snooze time
			continueToolStripMenuItem.Text = String.Format(Resources.SNOOZE_CONEXTMENU_TIME_TEXT, trayWaitMinutesLeft);
		}

		/// <summary>
		/// Handles the DoubleClick event of the TrayIcon control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-04-29</remarks>
		private void TrayIcon_DoubleClick(object sender, EventArgs e)
		{
			continueToolStripMenuItem.PerformClick();
		}

		/// <summary>
		/// Handles the FileDropped event of the learningWindow control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="MLifter.Controls.LearningWindow.FileDroppedEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-05-08</remarks>
		private void learningWindow_FileDropped(object sender, MLifter.Controls.LearningWindow.FileDroppedEventArgs e)
		{
			ActivateFileDropTimer(e.filename);
		}

		/// <summary>
		/// Handles the BrowserKeyDown event of the learningWindow control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.PreviewKeyDownEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-05-13</remarks>
		private void learningWindow_BrowserKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			//scan all menu items and compare the shortcut
			foreach (ToolStripItem menuitem in menuStripMain.Items)
			{
				if (menuitem is ToolStripMenuItem)
					foreach (ToolStripItem dropdownitem in ((ToolStripMenuItem)menuitem).DropDownItems)
					{
						if (dropdownitem is ToolStripMenuItem && ((ToolStripMenuItem)dropdownitem).ShortcutKeys == e.KeyData)
						{
							dropdownitem.PerformClick();
							break;
						}
					}
			}
		}

		/// <summary>
		/// Handles the Click event of the clipboardImportToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev09, 2009-03-05</remarks>
		private void clipboardImportToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (LearnLogic.Dictionary != null)
			{
				using (CollectorForm collector = new CollectorForm(LearnLogic.Dictionary))
				{
					collector.HelpFile = MLifter.Classes.Help.HelpPath;
					collector.ShowDialog();
					OnLMOptionsChanged();
				}
			}
		}

		private void helpIndexToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Help.ShowHelp(this, MLifter.Classes.Help.HelpPath, HelpNavigator.Index);
		}

		private void helpSearchToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Help.ShowHelp(this, MLifter.Classes.Help.HelpPath, HelpNavigator.Find, String.Empty);
		}


		private void slideShowToolStripButton_MouseLeave(object sender, EventArgs e)
		{
			ttToolStripTop.RemoveAll();
		}

		/// <summary>
		/// Handles the Closing event of the toolStripDropDownLearnModus control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.ToolStripDropDownClosingEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev07, 2009-04-09</remarks>
		void toolStripDropDownButtonChangeLearnModus_Closing(object sender, ToolStripDropDownClosingEventArgs e)
		{
			if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked)
			{
				// Cancel the Close operation to keep the menu open.
				e.Cancel = true;
			}
			else
			{
				// Allow the ToolStripDropDown to close.
				// Don't cancel the Close operation.
				e.Cancel = false;
			}
		}

		/// <summary>
		/// Handles the Click event of the toolStripMenuItemStandard control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev07, 2009-04-09</remarks>
		private void toolStripMenuItemStandard_Click(object sender, EventArgs e)
		{
			toolStripMenuItemStandard.Checked = !toolStripMenuItemStandard.Checked;
			UpdateToolStripLearningModeStandardIcon();
		}
		private void UpdateToolStripLearningModeStandardIcon()
		{
			if (toolStripMenuItemStandard.Checked)
				ShowSelectedLearnModi.LearnModusSelected(ShowSelectedLearnModi.LearnModi.Standard);
			else
				ShowSelectedLearnModi.LearnModusDeselected(ShowSelectedLearnModi.LearnModi.Standard);
		}

		/// <summary>
		/// Handles the Click event of the toolStripMenuItemMultipleChoice control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev07, 2009-04-09</remarks>
		private void toolStripMenuItemMultipleChoice_Click(object sender, EventArgs e)
		{
			toolStripMenuItemMultipleChoice.Checked = !toolStripMenuItemMultipleChoice.Checked;
			UpdateToolStripLearningModeMCIcon();
		}
		private void UpdateToolStripLearningModeMCIcon()
		{
			if (toolStripMenuItemMultipleChoice.Checked)
				ShowSelectedLearnModi.LearnModusSelected(ShowSelectedLearnModi.LearnModi.MultipleChoice);
			else
				ShowSelectedLearnModi.LearnModusDeselected(ShowSelectedLearnModi.LearnModi.MultipleChoice);
		}

		/// <summary>
		/// Handles the Click event of the toolStripMenuItemSentences control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev07, 2009-04-09</remarks>
		private void toolStripMenuItemSentences_Click(object sender, EventArgs e)
		{
			toolStripMenuItemSentences.Checked = !toolStripMenuItemSentences.Checked;
			UpdateToolStripLearningModeSentenceIcon();
		}
		private void UpdateToolStripLearningModeSentenceIcon()
		{
			if (toolStripMenuItemSentences.Checked)
				ShowSelectedLearnModi.LearnModusSelected(ShowSelectedLearnModi.LearnModi.Sentences);
			else
				ShowSelectedLearnModi.LearnModusDeselected(ShowSelectedLearnModi.LearnModi.Sentences);
		}

		/// <summary>
		/// Handles the Click event of the toolStripMenuItemListeningComprehension control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev07, 2009-04-09</remarks>
		private void toolStripMenuItemListeningComprehension_Click(object sender, EventArgs e)
		{
			toolStripMenuItemListeningComprehension.Checked = !toolStripMenuItemListeningComprehension.Checked;
			UpdateToolStripLearningModeListeningIcon();
		}
		private void UpdateToolStripLearningModeListeningIcon()
		{
			if (toolStripMenuItemListeningComprehension.Checked)
				ShowSelectedLearnModi.LearnModusSelected(ShowSelectedLearnModi.LearnModi.ListeningComprehension);
			else
				ShowSelectedLearnModi.LearnModusDeselected(ShowSelectedLearnModi.LearnModi.ListeningComprehension);
		}

		/// <summary>
		/// Handles the Click event of the toolStripMenuItemImageRecognition control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev07, 2009-04-09</remarks>
		private void toolStripMenuItemImageRecognition_Click(object sender, EventArgs e)
		{
			toolStripMenuItemImageRecognition.Checked = !toolStripMenuItemImageRecognition.Checked;
			UpdateToolStripLearningModeImageIcon();
		}
		private void UpdateToolStripLearningModeImageIcon()
		{
			if (toolStripMenuItemImageRecognition.Checked)
				ShowSelectedLearnModi.LearnModusSelected(ShowSelectedLearnModi.LearnModi.ImageRecognition);
			else
				ShowSelectedLearnModi.LearnModusDeselected(ShowSelectedLearnModi.LearnModi.ImageRecognition);
		}

		/// <summary>
		/// Gets the tool strip drop down change learning mode image.
		/// </summary>
		/// <remarks>Documented by Dev07, 2009-04-09</remarks>
		private void getToolStripDropDownChangeLearningModeImage()
		{
			//one item has to be checked
			bool atLeastOneChecked = false;

			foreach (object item in toolStripDropDownButtonChangeLearnModus.DropDown.Items)
			{
				if (!(item is ToolStripMenuItem))
					continue;
				if ((item as ToolStripMenuItem).Checked)
				{
					atLeastOneChecked = true;
					break;
				}
			}
			if (!atLeastOneChecked)
			{
				if (toolStripMenuItemStandard.Enabled)
					toolStripMenuItemStandard_Click(null, null);
				else if (toolStripMenuItemMultipleChoice.Enabled)
					toolStripMenuItemMultipleChoice_Click(null, null);
				else if (toolStripMenuItemSentences.Enabled)
					toolStripMenuItemSentences_Click(null, null);
				else if (toolStripMenuItemImageRecognition.Enabled)
					toolStripMenuItemImageRecognition_Click(null, null);
				else if (toolStripMenuItemListeningComprehension.Enabled)
					toolStripMenuItemListeningComprehension_Click(null, null);
				else
					toolStripMenuItemStandard_Click(null, null);
			}
			//get the image
			Image image = toolStripDropDownButtonChangeLearnModus.Image;
			ShowSelectedLearnModi.GetImage(image);
		}

		/// <summary>
		/// Opens the Learning Options Dialog!
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev07, 2009-04-09</remarks>
		private void toolStripMenuItemCustom_Click(object sender, EventArgs e)
		{
			if (!learnLogic.UserSessionAlive)
				return;

			UpdateToolStripLearningModes();

			QueryOptionsForm frmOptions = new QueryOptionsForm();
			frmOptions.showLearningModesTab();
			if (frmOptions.ShowDialog() == DialogResult.OK)
			{
				OnLMOptionsChanged();
			}
		}

		/// <summary>
		/// Handles the DropDownClosed event of the toolStripDropDownButtonChangeLearnModus control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev03, 2009-05-19</remarks>
		private void toolStripDropDownButtonChangeLearnModus_DropDownClosed(object sender, EventArgs e)
		{
			if (UpdateToolStripLearningModes())
			{
				//throw learningOptionChangeEvent to load a different card                
				LearnLogic.OnLearningModuleOptionsChanged();
				//change the image
				getToolStripDropDownChangeLearningModeImage();
			}
		}

		/// <summary>
		/// Updates the learning modes settings for the tool strip menu.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Documented by Dev03, 2009-05-19</remarks>
		private bool UpdateToolStripLearningModes()
		{
			bool optionsChanged = false;
			if (LearnLogic != null)
			{
				//check what is checked now and if smt is changed at all
				if (LearnLogic.Dictionary.Settings.QueryTypes.Word != ShowSelectedLearnModi.has(ShowSelectedLearnModi.LearnModi.Standard))
					optionsChanged = true;

				if (!optionsChanged)
				{
					if (LearnLogic.Dictionary.Settings.QueryTypes.ImageRecognition != ShowSelectedLearnModi.has(ShowSelectedLearnModi.LearnModi.ImageRecognition))
						optionsChanged = true;
				}
				if (!optionsChanged)
				{
					if (LearnLogic.Dictionary.Settings.QueryTypes.ListeningComprehension != ShowSelectedLearnModi.has(ShowSelectedLearnModi.LearnModi.ListeningComprehension))
						optionsChanged = true;
				}
				if (!optionsChanged)
				{
					if (LearnLogic.Dictionary.Settings.QueryTypes.MultipleChoice != ShowSelectedLearnModi.has(ShowSelectedLearnModi.LearnModi.MultipleChoice))
						optionsChanged = true;
				}
				if (!optionsChanged)
				{
					if (LearnLogic.Dictionary.Settings.QueryTypes.Sentence != ShowSelectedLearnModi.has(ShowSelectedLearnModi.LearnModi.Sentences))
						optionsChanged = true;
				}
				if (optionsChanged)
				{
					LearnLogic.Dictionary.Settings.QueryTypes.Word = ShowSelectedLearnModi.has(ShowSelectedLearnModi.LearnModi.Standard);
					LearnLogic.Dictionary.Settings.QueryTypes.ImageRecognition = ShowSelectedLearnModi.has(ShowSelectedLearnModi.LearnModi.ImageRecognition);
					LearnLogic.Dictionary.Settings.QueryTypes.Sentence = ShowSelectedLearnModi.has(ShowSelectedLearnModi.LearnModi.Sentences);
					LearnLogic.Dictionary.Settings.QueryTypes.MultipleChoice = ShowSelectedLearnModi.has(ShowSelectedLearnModi.LearnModi.MultipleChoice);
					LearnLogic.Dictionary.Settings.QueryTypes.ListeningComprehension = ShowSelectedLearnModi.has(ShowSelectedLearnModi.LearnModi.ListeningComprehension);
				}
			}
			return optionsChanged;
		}

		private void MainForm_ResizeBegin(object sender, EventArgs e)
		{
			ResizeStart();
		}
		private void ResizeStart()
		{
			ComponentsHelper.IsResizing = true;
			Refresh();
		}

		private void MainForm_ResizeEnd(object sender, EventArgs e)
		{
			SaveSettings(false);
			ResizeStop();
		}
		private void MainForm_Resize(object sender, EventArgs e) { }
		private void ResizeStop()
		{
			ComponentsHelper.IsResizing = false;
			Refresh();
		}

		/// [ML-1869] ML doesn't save the correct size
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev08, 2009-05-26</remarks>
		private void MainForm_SizeChanged(object sender, EventArgs e)
		{
			if (loadWindowSettings)
				return;

			if (this.WindowState != mainFormWindowState)     //WindowState has changed
			{
				if (this.WindowState == FormWindowState.Minimized)
				{
					MLifter.Properties.Settings.Default.Maximized = mainFormWindowState == FormWindowState.Maximized ? true : false;
					MLifter.Properties.Settings.Default.Save();
					if (OnStickMode)
						Setup.SaveSettingsToStick();
				}
				if (this.WindowState == FormWindowState.Maximized && LearnLogic.Dictionary != null)
				{
					MLifter.Properties.Settings.Default.LayoutValues = learningWindow.LayoutValues;
					MLifter.Properties.Settings.Default.Save();
					if (OnStickMode)
						Setup.SaveSettingsToStick();

				}
				ComponentsHelper.IsResizing = false;
				mainFormWindowState = this.WindowState;
			}
			Refresh();
		}

		/// <summary>
		/// Handles the Click event of the toolStripMenuItemCheckForBetaUpdates control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev07, 2009-07-14</remarks>
		private void toolStripMenuItemCheckForBetaUpdates_Click(object sender, EventArgs e)
		{
			toolStripMenuItemCheckForBetaUpdates.Checked = !toolStripMenuItemCheckForBetaUpdates.Checked;
			Settings.Default.CheckForBetaUpdates = toolStripMenuItemCheckForBetaUpdates.Checked;
			Settings.Default.Save();
		}

		/// <summary>
		/// Handles the Click event of the toolStripMenuItemCheckForUpdates control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev07, 2009-07-14</remarks>
		private void toolStripMenuItemCheckForUpdates_Click(object sender, EventArgs e)
		{
			Program.CheckForUpdate();

			TaskDialog.MessageBox(Resources.TASK_DIALOG_NoNewVersionFound_Title, Resources.TASK_DIALOG_NoNewVersionFound_Caption, Resources.TASK_DIALOG_NoNewVersionFound_Text,
				TaskDialogButtons.OK, TaskDialogIcons.Information);
		}
	}
}
