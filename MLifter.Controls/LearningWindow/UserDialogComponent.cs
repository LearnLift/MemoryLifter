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
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using MLifter.BusinessLayer;
using MLifter.Controls.Properties;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace MLifter.Controls.LearningWindow
{
    public partial class UserDialogComponent : Component, ILearnUserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserDialogComponent"/> class.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-04-29</remarks>
        public UserDialogComponent()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserDialogComponent"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <remarks>Documented by Dev02, 2008-04-29</remarks>
        public UserDialogComponent(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        #region ILearnUserControl Members

        private LearnLogic learnlogic;

        /// <summary>
        /// Registers the learn logic to this control.
        /// </summary>
        /// <param name="learnlogic">The learnlogic.</param>
        /// <remarks>Documented by Dev02, 2008-04-22</remarks>
        /// <remarks>Documented by Dev02, 2008-04-29</remarks>
        public void RegisterLearnLogic(MLifter.BusinessLayer.LearnLogic learnlogic)
        {
            this.learnlogic = learnlogic;
            this.learnlogic.UserDialog += new LearnLogic.UserDialogEventHandler(learnlogic_UserDialog);
        }

        /// <summary>
        /// Handles the UserDialog event of the learnlogic control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MLifter.BusinessLayer.UserDialogEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-04-29</remarks>
        void learnlogic_UserDialog(object sender, UserDialogEventArgs e)
        {
            if (e is UserGradingDialogEventArgs)
            {
                int cardbox = e.dictionary.Cards.GetCardByID(e.cardid).BaseCard.Box;
                int boxcount = e.dictionary.Boxes.Count;

                UserGradingDialogEventArgs args = (UserGradingDialogEventArgs)e;
                switch (args.dialogkind)
                {
                    case UserGradingDialogEventArgs.GradingDialogKind.ConfirmDemote:
                        args.promote = ShowGradeDialog(
                            Resources.CONFIRM_DEMOTE_TITLE, Resources.CONFIRM_DEMOTE_TEXT, Resources.CONFIRM_DEMOTE_DESCRIPTION, string.Empty,
                            false, cardbox, boxcount);
                        break;
                    case UserGradingDialogEventArgs.GradingDialogKind.GradeSynonym:
                        args.promote = ShowGradeDialog(
                            Resources.SYNONYM_GRADING_CAPTION, Resources.SYNONYM_GRADING_TEXT, string.Empty, Resources.SYNONYM_GRADING_EXPANDEDINFO,
                            true, cardbox, boxcount);
                        break;
                    case UserGradingDialogEventArgs.GradingDialogKind.GradeTypo:
                        args.promote = ShowGradeDialog(
                            Resources.ALMOST_CAPTION, Resources.ALMOST_TEXT, string.Empty, Resources.ALMOST_EXPANDEDINFO,
                            true, cardbox, boxcount);
                        break;
                }
            }
            else if (e is UserNotifyDialogEventArgs)
            {
                UserNotifyDialogEventArgs args = (UserNotifyDialogEventArgs)e;
                switch (args.dialogkind)
                {
                    case UserNotifyDialogEventArgs.NotifyDialogKind.NoWords:
                        ShowNoWordsMessage();
                        break;
                    case UserNotifyDialogEventArgs.NotifyDialogKind.PoolEmpty:
                    //gets displayed in LearningWindow
                    case UserNotifyDialogEventArgs.NotifyDialogKind.NotEnoughMultipleChoices:
                    //gets displayed in LearningWindow  
                    default:
                        break;
                }
            }
            else if (e is UserModuleNotSavedDialogEventArgs)
            {
                UserModuleNotSavedDialogEventArgs args = (UserModuleNotSavedDialogEventArgs)e;

                int command = TaskDialog.ShowCommandBox(Resources.MAIN_CLOSEWITHOUTSAVE_CAPTION, Resources.MAIN_CLOSEWITHOUTSAVE_TEXT,
                            Resources.MAIN_CLOSEWITHOUTSAVE_CONTENT, Resources.MAIN_CLOSEWITHOUTSAVE_COMMANDS, false);
                switch (command)
                {
                    case 0:
                        args.tryagain = true;
                        break;
                    case 1:
                        args.cancelClosing = true;
                        break;
                    case 2:
                        break;
                }
            }
            else if (e is BackupCompletedNotifyDialogEventArgs)
            {
                ShowBackupCompletedMessage(((BackupCompletedNotifyDialogEventArgs)e).backupFilename);
            }
        }

        #endregion

        #region Dialogs/Messages
        /// <summary>
        /// Shows the grade  confirmation dialog.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="maininstruction">The maininstruction.</param>
        /// <param name="content">The content.</param>
        /// <param name="expandedinfo">The expandedinfo.</param>
        /// <param name="promoteIsFirst">if set to <c>true</c> [promote is first].</param>
        /// <param name="cardbox">The cardbox.</param>
        /// <param name="boxcount">The boxcount.</param>
        /// <returns>The dialog result (promote).</returns>
        /// <remarks>Documented by Dev02, 2008-02-05</remarks>
        public static bool ShowGradeDialog(string title, string maininstruction, string content, string expandedinfo, bool promoteIsFirst, int cardbox, int boxcount)
        {
            int box = cardbox;
            if (box == 0) box = 1; //Pool: card goes box 0 --> box 2
            box = (box < (boxcount - 1)) ? ++box : box;

            EmulatedTaskDialog dialog = new EmulatedTaskDialog();
            dialog.Title = title;
            dialog.MainInstruction = maininstruction;
            dialog.Content = content;
            dialog.Buttons = MLifter.Controls.TaskDialogButtons.None;
            dialog.CommandButtons = promoteIsFirst ? String.Format(Resources.CONFIRM_DEMOTE_BUTTON_PROMOTE, box) + "|" + Resources.CONFIRM_DEMOTE_BUTTON_DEMOTE :
                Resources.CONFIRM_DEMOTE_BUTTON_DEMOTE + "|" + string.Format(Resources.CONFIRM_DEMOTE_BUTTON_PROMOTE, box);
            dialog.MainIcon = MLifter.Controls.TaskDialogIcons.Question;
            dialog.MainImages = promoteIsFirst ? new Image[] { Resources.promoteCard, Resources.demoteCard } : new Image[] { Resources.demoteCard, Resources.promoteCard };
            dialog.HoverImages = dialog.MainImages;
            dialog.CenterImages = true;
            dialog.BuildForm();
            DialogResult dialogResult = dialog.ShowDialog();

            bool result = promoteIsFirst ? dialog.CommandButtonClickedIndex == 0 : dialog.CommandButtonClickedIndex == 1;
            return result;
        }

        /// <summary>
        /// Shows the no words message.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-04-23</remarks>
        public static void ShowNoWordsMessage()
        {
            MessageBox.Show(Resources.NO_WORDS_TEXT, Resources.NO_WORDS_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Shows the backup completed message.
        /// </summary>
        /// <param name="backupFilename">The backup filename.</param>
        /// <remarks>Documented by Dev02, 2008-09-08</remarks>
        public static void ShowBackupCompletedMessage(string backupFilename)
        {
            MessageBox.Show(String.Format(Resources.RESTART_BACKUP_TEXT, Path.GetFileName(backupFilename)), Resources.RESTART_BACKUP_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion
    }
}
