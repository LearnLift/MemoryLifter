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
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using MLifter.BusinessLayer;
using MLifter.Components;
using MLifter.Controls.Properties;
using MLifter.DAL;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Interfaces.DB;
using MLifter.DAL.Tools;
using MLifter.AudioTools;

namespace MLifter.Controls
{
	public partial class CardEdit : UserControl
	{
		LoadStatusMessage statusDialog = null;

		private bool modified = false;
		private List<VideoPlayer> playingVideos = new List<VideoPlayer>();
		private string helpNamespace = string.Empty;

		/// <summary>
		/// true if the card was modified.
		/// </summary>
		public bool Modified
		{
			get { return modified; }
			set
			{
				modified = value;
				buttonAddEdit.Enabled = (modified || preloadedCard);
			}
		}

		/// <summary>
		/// Gets or sets the help namespace.
		/// </summary>
		/// <value>The help namespace.</value>
		/// <remarks>Documented by Dev02, 2008-03-07</remarks>
		public string HelpNamespace
		{
			get { return helpNamespace; }
			set { helpNamespace = value; }
		}

		/// <summary>
		/// The id of the current card.
		/// </summary>
		public int CardID = 0;

		/// <summary>
		/// Gets or sets the selected chapter.
		/// </summary>
		/// <value>The selected chapter.</value>
		/// <remarks>Documented by Dev05, 2007-12-04</remarks>
		[ReadOnly(true), Browsable(false)]
		public int SelectedChapter
		{
			get { return ((IChapter)comboBoxChapter.SelectedItem).Id; }
			set
			{
				if (value >= 0)
					comboBoxChapter.SelectedItem = Dictionary.Chapters.GetChapterByID(value);
			}
		}

		protected Dictionary dictionary;
		protected Dictionary Dictionary
		{
			get { return dictionary; }
			set
			{
				if (dictionary != null)
					dictionary.CreateMediaProgressChanged -= new StatusMessageEventHandler(Dictionary_CreateMediaProgressChanged);

				if (dictionary == null || dictionary != value) //only do this if the dictionary has changed
				{
					dictionary = value;
					groupBoxQuestion.Text = LimitTextSize(dictionary.QuestionCaption, groupBoxQuestion.Font, groupBoxQuestion.Width);
					groupBoxAnswer.Text = LimitTextSize(dictionary.AnswerCaption, groupBoxAnswer.Font, groupBoxAnswer.Width);
					comboBoxChapter.Items.Clear();
					IChapter[] chapterArray = new IChapter[Dictionary.Chapters.Chapters.Count];
					Dictionary.Chapters.Chapters.CopyTo(chapterArray, 0);
					comboBoxChapter.Items.AddRange(chapterArray);
					if (comboBoxChapter.Items.Count > 0)
						comboBoxChapter.SelectedIndex = 0;
				}

				if (dictionary != null)
					dictionary.CreateMediaProgressChanged += new StatusMessageEventHandler(Dictionary_CreateMediaProgressChanged);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether [right to left question].
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [right to left question]; otherwise, <c>false</c>.
		/// </value>
		/// <remarks>Documented by Dev02, 2008-07-15</remarks>
		[Browsable(false)]
		public bool RightToLeftQuestion
		{
			get
			{
				return (textBoxQuestion.RightToLeft != RightToLeft.Yes);
			}
			set
			{
				textBoxQuestion.RightToLeft = listViewQuestion.RightToLeft = textBoxQuestionExample.RightToLeft =
					value ? RightToLeft.Yes : RightToLeft.No;

				listViewQuestion.RightToLeftLayout = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether [right to left answer].
		/// </summary>
		/// <value><c>true</c> if [right to left answer]; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev02, 2008-07-15</remarks>
		[Browsable(false)]
		public bool RightToLeftAnswer
		{
			get
			{
				return (textBoxAnswer.RightToLeft != RightToLeft.Yes);
			}
			set
			{
				textBoxAnswer.RightToLeft = listViewAnswer.RightToLeft = textBoxAnswerExample.RightToLeft =
					value ? RightToLeft.Yes : RightToLeft.No;

				listViewAnswer.RightToLeftLayout = value;
			}
		}

		/// <summary>
		/// Limits the size of the Text according to the supplied font and width.
		/// </summary>
		/// <param name="Text">The Text.</param>
		/// <param name="font">The font.</param>
		/// <param name="width">The width.</param>
		/// <returns>The Text limited in size.</returns>
		/// <remarks>Documented by Dev02, 2008-03-05</remarks>
		private string LimitTextSize(string text, Font font, int width)
		{
			int charcount = text.Length;
			const int cutcharscount = 3;
			const string ellipsis = "...";

			Graphics g = Graphics.FromHwnd(this.Handle);
			int ellipsislength = (int)g.MeasureString(ellipsis, font).Width;

			while (g.MeasureString(text, font).Width + ellipsislength + 20 > width)
			{
				if (text.Length > cutcharscount)
					text = text.Substring(0, text.Length - cutcharscount);
				else
					break;
			}

			if (text.Length < charcount)
				text += ellipsis;

			return text;
		}

		//[ML-630] Collect cards --> confirm with <Enter> (ML crashes) 
		public bool IsCardLoaded
		{
			get
			{
				if (CardCollector)
				{
					return !LoadingCard;
				}
				else
				{
					return true;
				}
			}
		}
		public bool LoadingCard = false;
		public bool CardCollector = false;

		private bool multiselect = false;

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="CardEdit"/> is multiselect.
		/// </summary>
		/// <value><c>true</c> if multiselect; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev02, 2008-03-13</remarks>
		public bool Multiselect
		{
			get { return multiselect; }
			set { multiselect = value; buttonPreview.Enabled = !multiselect; }
		}

		public bool mouseDown = false;

		private bool newCard = false;
		private bool resizeShown = false;
		private bool changingCheckstate = false;
		private bool preloadedCard = false;
		private bool checkBoxSynonymEventEnabled = true;

		private ResizeMode QuestionResizeMode = ResizeMode.None;
		private ResizeMode AnswerResizeMode = ResizeMode.None;

		private enum ResizeMode
		{
			None,
			Small,
			Medium,
			Large
		}

		/// <summary>
		/// Occurs when a card is added.
		/// </summary>
		/// <remarks>Documented by Dev05, 2007-10-11</remarks>
		public event EventHandler Add;
		/// <summary>
		/// Occurs when a card is edited.
		/// </summary>
		/// <remarks>Documented by Dev05, 2007-10-11</remarks>
		public event EventHandler Edit;
		/// <summary>
		/// Occurs when a style is added to a card.
		/// </summary>
		/// <remarks>Documented by Dev05, 2007-10-11</remarks>
		public event EventHandler AddStyle;
		/// <summary>
		/// Raises the <see cref="E:Add"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-10-11</remarks>
		protected virtual void OnAdd(EventArgs e)
		{
			if (Add != null)
				Add(this, e);
		}
		/// <summary>
		/// Raises the <see cref="E:Edit"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-10-11</remarks>
		protected virtual void OnEdit(EventArgs e)
		{
			if (Edit != null)
				Edit(this, e);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CardEdit"/> class.
		/// </summary>
		/// <remarks>Documented by Dev05, 2007-10-11</remarks>
		public CardEdit()
		{
			InitializeComponent();

			buttonAnswerImage.Tag = pictureBoxAnswer;
			buttonQuestionImage.Tag = pictureBoxQuestion;

			pictureBoxAnswer.AllowDrop = true;
			pictureBoxQuestion.AllowDrop = true;

			ToolTip toolTip = new ToolTip();
			toolTip.SetToolTip(pictureBoxAnswer, Properties.Resources.EDITCARD_OPENPICTURE);
			toolTip.SetToolTip(pictureBoxQuestion, Properties.Resources.EDITCARD_OPENPICTURE);
			toolTip.SetToolTip(checkBoxResizeAnswer, Properties.Resources.CARD_EDIT_RESIZE);
			toolTip.SetToolTip(checkBoxResizeQuestion, Properties.Resources.CARD_EDIT_RESIZE);

			toolTip.SetToolTip(checkBoxSynonymQuestion, Resources.CARDEDIT_TOOLTIP_SYNONYMMODE);
			toolTip.SetToolTip(checkBoxSynonymAnswer, Resources.CARDEDIT_TOOLTIP_SYNONYMMODE);

			toolTip.SetToolTip(checkBoxCharacterMapQuestion, Resources.CARDEDIT_TOOLTIP_CHARACTERMAP);
			toolTip.SetToolTip(checkBoxCharacterMapAnswer, Resources.CARDEDIT_TOOLTIP_CHARACTERMAP);

			ChangeSynonymMode(false, Side.Question);
			ChangeSynonymMode(false, Side.Answer);
		}

		/// <summary>
		/// Overrides the CreateControl event to register to the parentform.
		/// (The parentform is not defined earlier, e.g. in the constructor.)
		/// </summary>
		/// <remarks>Documented by Dev02, 2008-05-13</remarks>
		protected override void OnCreateControl()
		{
			base.OnCreateControl();
			if (this.ParentForm != null)
			{
				characterMapComponent.RegisterForm(this.ParentForm, true);
				characterMapComponent.VisibleChanged += new EventHandler(characterMapComponent_VisibleChanged);
			}
		}

		/// <summary>
		/// Handles the VisibleChanged event of the characterMapComponent control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-05-08</remarks>
		void characterMapComponent_VisibleChanged(object sender, EventArgs e)
		{
			checkBoxCharacterMapAnswer.Checked = checkBoxCharacterMapQuestion.Checked = characterMapComponent.Visible;
		}

		/// <summary>
		/// Handles the Click event of the Media button controls.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-10-11</remarks>
		private void buttonMedia_Click(object sender, EventArgs e)
		{
			Button button = (Button)sender;

			contextMenuStripMedia.Items[0].Visible = true; //this is the 'Play' entry

			if (button.Tag as IAudio != null || button.Name.Contains("Audio"))
			{
				AudioDialog audioDialog = new AudioDialog();
				audioDialog.DisplayText = (button == buttonAnswerAudio ? GetWord(Side.Answer) :
					(button == buttonAnswerExampleAudio ? textBoxAnswerExample.Text :
					(button == buttonQuestionAudio ? GetWord(Side.Question) : textBoxQuestionExample.Text)));
				string audioPath = string.Empty;
				if (button.Tag as IAudio != null)
					audioDialog.Path = audioPath = ((IAudio)button.Tag).Filename;

				audioDialog.ShowDialog();

				if (audioDialog.Path != audioPath)
				{
					Modified = true;
					if (audioDialog.Path == string.Empty)
					{
						button.Tag = null;
						button.Image = Resources.Audio;
					}
					else
					{
						button.Tag = CreateMedia(EMedia.Audio, audioDialog.Path, true, true, button.Name.Contains("Example"));
						button.Image = Resources.AudioAvailable;
					}
				}

				return;
			}
			else if (button.Tag as IVideo != null)
			{
				contextMenuStripMedia.Items[0].Enabled = true;
			}
			else
			{
				contextMenuStripMedia.Items[0].Enabled = false;
			}

			contextMenuStripMedia.Items[1].Enabled = contextMenuStripMedia.Items[0].Enabled;
			contextMenuStripMedia.Tag = button;

			if (button.Name.Contains("Image"))
			{
				contextMenuStripMedia.Items[0].Visible = false;
				contextMenuStripMedia.Tag = button.Tag;
				if (((PictureBox)contextMenuStripMedia.Tag).Tag is IImage)
					contextMenuStripMedia.Items[1].Enabled = true;
			}

			contextMenuStripMedia.Show((Control)sender, 0, 0);
		}

		/// <summary>
		/// Handles the Click event of the browseToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-10-11</remarks>
		private void browseToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();

			if (contextMenuStripMedia.Tag is PictureBox || ((Control)contextMenuStripMedia.Tag).Name.Contains("Image"))
				openFileDialog.Filter = Properties.Resources.IMAGE_FILE_FILTER;
			else if (((Control)contextMenuStripMedia.Tag).Name.Contains("Audio"))
				openFileDialog.Filter = Properties.Resources.AUDIO_FILE_FILTER;
			else
				openFileDialog.Filter = Properties.Resources.VIDEO_FILE_FILTER;

			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				try
				{
					EMedia type = EMedia.Unknown;
					bool isExample = false;

					if (contextMenuStripMedia.Tag is PictureBox)
					{
						string path = openFileDialog.FileName;
						LoadImage(contextMenuStripMedia.Tag as PictureBox, path);
						type = EMedia.Image;
					}
					if (((Control)contextMenuStripMedia.Tag).Name.Contains("Audio"))
					{
						((Button)contextMenuStripMedia.Tag).Image = Properties.Resources.AudioAvailable;
						type = EMedia.Audio;
						isExample = ((Control)contextMenuStripMedia.Tag).Name.Contains("Example");
					}
					else if (((Control)contextMenuStripMedia.Tag).Name.Contains("Video"))
					{
						((Button)contextMenuStripMedia.Tag).Image = Properties.Resources.VideoAvailable;
						type = EMedia.Video;

						//check if video is playing
						PictureBox selectedpicturebox = ((Control)contextMenuStripMedia.Tag).Name.Contains("Answer") ? pictureBoxAnswer : pictureBoxQuestion;
						VideoPlayer playing = PlayingVideo(selectedpicturebox);
						if (playing != null)
							playing.Stop();
					}

					IMedia media = CreateMedia(type, openFileDialog.FileName, true, true, isExample);
					((Control)contextMenuStripMedia.Tag).Tag = media;

					Modified = true;
				}
				catch
				{
					((Control)contextMenuStripMedia.Tag).Tag = null;
				}
			}
		}

		/// <summary>
		/// A helper function to create Media objects.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="filename">The filename.</param>
		/// <param name="isActive">if set to <c>true</c> [is active].</param>
		/// <param name="isDefault">if set to <c>true</c> [is default].</param>
		/// <param name="isExample">if set to <c>true</c> [is example].</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev02, 2008-08-11</remarks>
		private IMedia CreateMedia(EMedia type, string filename, bool isActive, bool isDefault, bool isExample)
		{
			IMedia media = (new MLifter.DAL.Preview.PreviewCard(null)).CreateMedia(type, filename, isActive, isDefault, isExample);
			return media;
		}

		/// <summary>
		/// Handles the Click event of the removeToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-10-11</remarks>
		private void removeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (contextMenuStripMedia.Tag is PictureBox)
				((PictureBox)contextMenuStripMedia.Tag).Image = null;
			else if (((Control)contextMenuStripMedia.Tag).Name.Contains("Audio"))
				((Button)contextMenuStripMedia.Tag).Image = Properties.Resources.Audio;
			else
			{
				((Button)contextMenuStripMedia.Tag).Image = Properties.Resources.Video;
				//check if video is playing
				PictureBox selectedpicturebox = ((Control)contextMenuStripMedia.Tag).Name.Contains("Answer") ? pictureBoxAnswer : pictureBoxQuestion;
				VideoPlayer playing = PlayingVideo(selectedpicturebox);
				if (playing != null)
					playing.Stop();
			}

			((Control)contextMenuStripMedia.Tag).Tag = null;
			UpdateSamePicture();
			Modified = true;
		}

		/// <summary>
		/// Handles the Click event of the playToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-10-11</remarks>
		private void playToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (((Control)contextMenuStripMedia.Tag).Name.Contains("Audio"))
			{
				try
				{
					IAudio audioobject = ((Control)contextMenuStripMedia.Tag).Tag as IAudio;
					if (audioobject != null)
					{
						AudioPlayer player = new AudioPlayer();
						player.Play(audioobject.Filename, true);
					}
				}
				catch (Exception exp)
				{
					System.Windows.Forms.MessageBox.Show(Properties.Resources.AUDIOPLAYER_CRASHED_TEXT, Properties.Resources.AUDIOPLAYER_CRASHED_CAPTION,
							System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
					System.Diagnostics.Trace.WriteLine("PlayToolStrip Audio Player crashed: " + exp.ToString());
				}
			}
			else //Video
			{
				PictureBox selectedpicturebox = ((Control)contextMenuStripMedia.Tag).Name.Contains("Answer") ? pictureBoxAnswer : pictureBoxQuestion;
				VideoPlayer playing = PlayingVideo(selectedpicturebox);
				if (playing != null)
				{
					playing.Stop();
				}
				else
				{
					try
					{
						IVideo videoobject = ((Control)contextMenuStripMedia.Tag).Tag as IVideo;
						if (videoobject != null)
						{
							VideoPlayer video = new VideoPlayer(selectedpicturebox);
							video.Ending += new EventHandler(Video_Ending);
							video.Stopping += new EventHandler(Video_Ending);
							video.Play(videoobject.Filename);
							
							playingVideos.Add(video);
						}
					}
					catch (Exception exp)
					{
						System.Diagnostics.Trace.WriteLine("PlayToolStrip Video Player crashed: " + exp.ToString());
					}
				}
			}
		}

		/// <summary>
		/// Handles the Ending event of the video control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-10-01</remarks>
		private void Video_Ending(object sender, EventArgs e)
		{
			try
			{
				VideoPlayer video = sender as VideoPlayer;
				if (video != null)
				{
					video.Owner = panelVideoParker;
					if (playingVideos.Contains(video))
						playingVideos.Remove(video);
				}
			}
			catch (Exception exp)
			{
				System.Diagnostics.Trace.WriteLine("Video_Ending crashed: " + exp.ToString());
			}
		}

		/// <summary>
		/// Stops the running videos.
		/// </summary>
		/// <remarks>Documented by Dev02, 2008-02-26</remarks>
		public void StopPlayingVideos()
		{
			if (playingVideos != null)
			{
				foreach (VideoPlayer video in playingVideos)
				{
					if (video != null && video.IsPlaying)
						video.Stop();
				}
			}
		}

		/// <summary>
		/// Researches the playing video. Returns null if none.
		/// </summary>
		/// <param name="picturebox">The picturebox.</param>
		/// <returns>Researches the playing video. Returns null if none.</returns>
		/// <remarks>Documented by Dev02, 2008-02-26</remarks>
		private VideoPlayer PlayingVideo(PictureBox picturebox)
		{
			VideoPlayer playing = null;

			foreach (VideoPlayer video in playingVideos)
			{
				if (video.Owner == picturebox)
					playing = video;
			}

			return playing;
		}

		/// <summary>
		/// Handles the Click event of the pictureBoxQuestion control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-10-01</remarks>
		private void pictureBoxQuestion_Click(object sender, EventArgs e)
		{
		}

		/// <summary>
		/// Handles the Click event of the pictureBoxAnswer control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-10-01</remarks>
		private void pictureBoxAnswer_Click(object sender, EventArgs e)
		{
		}

		/// <summary>
		/// Handles the MouseUp event of the pictureBoxQuestion control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-10-15</remarks>
		private void pictureBoxQuestion_MouseUp(object sender, MouseEventArgs e)
		{
			PictureBox picturebox = sender as PictureBox;

			PictureBoxMouseUp(e, picturebox);
		}

		/// <summary>
		/// Handles the MouseDown event of the pictureBoxQuestion control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-11-22</remarks>
		private void pictureBoxQuestion_MouseDown(object sender, MouseEventArgs e)
		{
			mouseDown = true;
		}

		/// <summary>
		/// Handles the MouseUp event of the pictureBoxAnswer control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-10-15</remarks>
		private void pictureBoxAnswer_MouseUp(object sender, MouseEventArgs e)
		{
			PictureBox picturebox = sender as PictureBox;

			PictureBoxMouseUp(e, picturebox);
		}

		/// <summary>
		/// Handles the MouseUp event of the pictureBox control.
		/// </summary>
		/// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
		/// <param name="picturebox">The picturebox.</param>
		/// <remarks>Documented by Dev02, 2008-02-26</remarks>
		private void PictureBoxMouseUp(MouseEventArgs e, PictureBox picturebox)
		{
			try
			{
				//stop video in case one is playing
				VideoPlayer playing = PlayingVideo(picturebox);
				if (playing != null)
				{
					playing.Stop();
					return;
				}

				if (e.Button == MouseButtons.Left)
				{
					if (!mouseDown)
						return;

					OpenFileDialog openFileDialog = new OpenFileDialog();
					openFileDialog.Filter = Properties.Resources.IMAGE_FILE_FILTER;
					if (openFileDialog.ShowDialog() == DialogResult.OK)
					{
						string path = openFileDialog.FileName;
						LoadImage(picturebox, path);
						picturebox.Tag = CreateMedia(EMedia.Image, path, true, true, false);
					}
				}
				else
				{
					contextMenuStripMedia.Items[0].Enabled = false;
					contextMenuStripMedia.Items[1].Enabled = (picturebox.Tag is IImage);

					contextMenuStripMedia.Tag = picturebox;
					contextMenuStripMedia.Show(picturebox, e.X, e.Y);
				}
			}
			finally
			{
				mouseDown = false;
			}
		}

		/// <summary>
		/// Handles the MouseDown event of the pictureBoxAnswer control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-11-22</remarks>
		private void pictureBoxAnswer_MouseDown(object sender, MouseEventArgs e)
		{
			mouseDown = true;
		}

		/// <summary>
		/// Handles the DragEnter event of the pictureBoxAnswer control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.DragEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-10-11</remarks>
		private void pictureBox_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.Bitmap) || e.Data.GetDataPresent(DataFormats.FileDrop))
				e.Effect = DragDropEffects.Copy;
		}

		/// <summary>
		/// Handles the DragDrop event of the pictureBoxAnswer control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.DragEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-10-11</remarks>
		private void pictureBox_DragDrop(object sender, DragEventArgs e)
		{
			PictureBox picBox = (PictureBox)sender;
			if (e.Data.GetDataPresent(DataFormats.Bitmap))
				MessageBox.Show(Resources.CARDEDIT_BITMAPERROR_TEXT, Resources.CARDEDIT_BITMAPERROR_CAPTION);
			else if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				string file = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
				// check for allowed file extensions
				if (Properties.Resources.IMAGE_FILE_FILTER.IndexOf(System.IO.Path.GetExtension(file.ToLowerInvariant())) > -1)
				{
					string path = file;
					LoadImage(picBox, path);
					picBox.Tag = CreateMedia(EMedia.Image, path, true, true, false);
				}
			}
		}

		/// <summary>
		/// Loads the image into the picturebox.
		/// </summary>
		/// <param name="picBox">The pic box.</param>
		/// <param name="file">The file.</param>
		/// <remarks>Documented by Dev05, 2007-11-29</remarks>
		private void LoadImage(PictureBox picBox, string filename)
		{
			LoadImage(picBox, new MemoryStream(File.ReadAllBytes(filename)));
		}

		/// <summary>
		/// Loads the image into the picturebox.
		/// </summary>
		/// <param name="picBox">The pic box.</param>
		/// <param name="file">The file.</param>
		/// <remarks>Documented by Dev05, 2007-11-29</remarks>
		private void LoadImage(PictureBox picBox, Stream file)
		{
			try
			{
				//picBox.Load(file); //replaced because of problems with animated gifs
				Image image = Image.FromStream(file);
				CheckAndRestoreGIF(ref image);
				picBox.Image = image;

				UpdateSamePicture();

				if (!resizeShown && (picBox.Image.Width > Settings.Default.resizeLarge.Width || picBox.Image.Height > Settings.Default.resizeLarge.Height))
				{
					if (picBox.Name.Contains("Answer") && !checkBoxResizeAnswer.Checked)
					{
						resizeShown = true;
						checkBoxResizeAnswer.Checked = true; //triggers the resize dialog
					}
					else
					{
						resizeShown = true;
						checkBoxResizeQuestion.Checked = true; //triggers the resize dialog
					}
				}

				Modified = true;
			}
			catch { }
		}

		/// <summary>
		/// Checks and restores an animated GIF.
		/// </summary>
		/// <param name="image">The image.</param>
		/// <remarks>Documented by Dev03, 2008-03-21</remarks>
		private void CheckAndRestoreGIF(ref Image image)
		{
			if (!image.RawFormat.Guid.Equals(ImageFormat.Gif.Guid))
				return;

			List<int> badframeIndexes = new List<int>();
			int count = 0;

			count = image.GetFrameCount(FrameDimension.Time);

			if (count > 1)
			{
				//string fn = Path.GetTempFileName();
				//image.Save(fn);
				//image = Image.FromFile(fn);

				//MemoryStream str = new MemoryStream();
				//image.Save(str, ImageFormat.Gif);
				//str.Seek(0, SeekOrigin.Begin);
				//image = Image.FromStream(str);
			}

			for (int i = 0; i < count; i++)
			{
				try
				{
					image.SelectActiveFrame(FrameDimension.Time, i);
				}
				catch
				{
					badframeIndexes.Add(i);
				}
			}
			if (badframeIndexes.Count > 0)
			{
				string indexes = String.Empty;
				for (int i = 0; i < badframeIndexes.Count; i++)
				{
					indexes += badframeIndexes[i].ToString() + ",";
				}
				indexes = indexes.Substring(0, indexes.Length - 1);
				System.Diagnostics.Debug.Write("Frames No." + indexes + " are damaged!");
			}
			else
			{
				return;
			}

			if ((badframeIndexes.Count) > 0 && (badframeIndexes.Count < count))
			{
				using (MemoryStream stream = new MemoryStream())
				{
					EncoderParameters encoderpara = new EncoderParameters(1);

					for (int i = 0; i < count; i++)
					{
						if (!badframeIndexes.Contains(i))
						{
							image.SelectActiveFrame(FrameDimension.Time, i);
							image.Save(stream, ImageFormat.Png);
							break;
						}
					}
					image = Bitmap.FromStream(stream);
				}
			}
		}


		/// <summary>
		/// Handles the TextChanged event of the textBox controls.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-10-12</remarks>
		private void textBox_TextChanged(object sender, EventArgs e)
		{
			Modified = true;
		}

		/// <summary>
		/// Checks the modified.
		/// </summary>
		/// <remarks>Documented by Dev05, 2007-11-12</remarks>
		public virtual void CheckModified()
		{
			if (Modified)
			{
				Modified = false;
				if (MessageBox.Show(Properties.Resources.CARDEDIT_SAVE_TEXT, Properties.Resources.CARDEDIT_SAVE_CAPION, MessageBoxButtons.YesNo, MessageBoxIcon.Question)
					== DialogResult.Yes)
				{
					buttonAddEdit_Click(this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Loads the new card.
		/// </summary>
		/// <remarks>Documented by Dev05, 2007-10-11</remarks>
		public void LoadNewCard(Dictionary currentDictionary)
		{
			buttonStyle.Enabled = false;

			CheckModified();
			Multiselect = false;
			Dictionary = currentDictionary;

			StopPlayingVideos();

			buttonAddEdit.Image = Properties.Resources.newDoc;
			buttonAddEdit.Text = Properties.Resources.EDITCARD_NEW;

			SetWord(Side.Question, string.Empty);
			SetWord(Side.Answer, string.Empty);

			textBoxQuestionExample.Text = string.Empty;
			textBoxAnswerExample.Text = string.Empty;

			checkBoxActive.Checked = true;
			checkBoxActive.CheckState = CheckState.Checked;
			comboBoxCardBox.SelectedIndex = 0;
			comboBoxCardBox.DropDownStyle = ComboBoxStyle.DropDownList;

			if (comboBoxChapter.Items.Contains(Properties.Resources.MAINTAIN_UNCHANGED))
			{
				comboBoxChapter.Items.Remove(Properties.Resources.MAINTAIN_UNCHANGED);
				if (!(comboBoxChapter.SelectedItem is IChapter) && comboBoxChapter.Items.Count > 0)
					comboBoxChapter.SelectedIndex = 0;
			}

			pictureBoxAnswer.Image = null;
			pictureBoxAnswer.Tag = null;
			pictureBoxQuestion.Image = null;
			pictureBoxQuestion.Tag = null;

			buttonQuestionAudio.Image = Properties.Resources.Audio;
			buttonQuestionAudio.Tag = null;
			buttonQuestionExampleAudio.Image = Properties.Resources.Audio;
			buttonQuestionExampleAudio.Tag = null;
			buttonQuestionVideo.Image = Properties.Resources.Video;
			buttonQuestionVideo.Tag = null;
			buttonAnswerAudio.Image = Properties.Resources.Audio;
			buttonAnswerAudio.Tag = null;
			buttonAnswerExampleAudio.Image = Properties.Resources.Audio;
			buttonAnswerExampleAudio.Tag = null;
			buttonAnswerVideo.Image = Properties.Resources.Video;
			buttonAnswerVideo.Tag = null;

			checkBoxSamePicture.Checked = false;

			CardLoaded();

			newCard = true;
			preloadedCard = false;
			Modified = false;
		}

		/// <summary>
		/// Loads the new card.
		/// </summary>
		/// <param name="question">The question.</param>
		/// <param name="questionExample">The question example.</param>
		/// <param name="answer">The answer.</param>
		/// <param name="answerExample">The answer example.</param>
		/// <param name="questionImage">The question image.</param>
		/// <param name="answerImage">The answer image.</param>
		/// <param name="questionAudio">The question audio.</param>
		/// <param name="questionExampleAudio">The question example audio.</param>
		/// <param name="questionVideo">The question video.</param>
		/// <param name="answerAudio">The answer audio.</param>
		/// <param name="answerExampleAudio">The answer example audio.</param>
		/// <param name="answerVideo">The answer video.</param>
		/// <remarks>Documented by Dev05, 2007-10-15</remarks>
		/// <remarks>Documented by Dev08, 2008-09-25</remarks>
		public void LoadNewCard(Dictionary currentDictionary, string question, string questionExample, string answer, string answerExample, string questionImage, string answerImage,
			string questionAudio, string questionExampleAudio, string questionVideo, string answerAudio, string answerExampleAudio, string answerVideo)
		{
			CheckModified();
			Multiselect = false;
			Dictionary = currentDictionary;

			StopPlayingVideos();

			buttonAddEdit.Image = Properties.Resources.newDoc;
			buttonAddEdit.Text = Properties.Resources.EDITCARD_NEW;

			pictureBoxAnswer.Image = null;
			pictureBoxAnswer.Tag = null;
			pictureBoxQuestion.Image = null;
			pictureBoxQuestion.Tag = null;

			buttonQuestionAudio.Image = Properties.Resources.Audio;
			buttonQuestionAudio.Tag = null;
			buttonQuestionExampleAudio.Image = Properties.Resources.Audio;
			buttonQuestionExampleAudio.Tag = null;
			buttonQuestionVideo.Image = Properties.Resources.Video;
			buttonQuestionVideo.Tag = null;
			buttonAnswerAudio.Image = Properties.Resources.Audio;
			buttonAnswerAudio.Tag = null;
			buttonAnswerExampleAudio.Image = Properties.Resources.Audio;
			buttonAnswerExampleAudio.Tag = null;
			buttonAnswerVideo.Image = Properties.Resources.Video;
			buttonAnswerVideo.Tag = null;

			//BugFix: Trim Synonyms by FabThe
			string[] splitchars = new string[] { " ,", " ;", ", ", "; ", ",", ";" };

			foreach (string splitchar in splitchars)
			{
				question = question.Replace(splitchar, Environment.NewLine);
				answer = answer.Replace(splitchar, Environment.NewLine);
			}

			SetWord(Side.Question, question);
			SetWord(Side.Answer, answer);

			textBoxQuestionExample.Text = questionExample;
			textBoxAnswerExample.Text = answerExample;

			checkBoxSamePicture.Checked = false;

			if (comboBoxChapter.Items.Count == 0)
			{
				IChapter[] chapterArray = new IChapter[Dictionary.Chapters.Chapters.Count];
				Dictionary.Chapters.Chapters.CopyTo(chapterArray, 0);
				comboBoxChapter.Items.AddRange(chapterArray);
				comboBoxChapter.SelectedIndex = 0;
				checkBoxActive.Checked = true;
			}
			if (comboBoxChapter.Items.Contains(Properties.Resources.MAINTAIN_UNCHANGED))
			{
				comboBoxChapter.Items.Remove(Properties.Resources.MAINTAIN_UNCHANGED);
				if (!(comboBoxChapter.SelectedItem is IChapter) && comboBoxChapter.Items.Count > 0)
					comboBoxChapter.SelectedIndex = 0;
			}

			comboBoxCardBox.SelectedIndex = 0;
			comboBoxCardBox.DropDownStyle = ComboBoxStyle.DropDownList;

			try
			{
				pictureBoxQuestion.Load(questionImage);
				pictureBoxQuestion.Tag = CreateMedia(EMedia.Image, questionImage, true, true, false);
			}
			catch { }
			try
			{
				pictureBoxAnswer.Load(answerImage);
				pictureBoxAnswer.Tag = CreateMedia(EMedia.Image, answerImage, true, true, false);
			}
			catch { }
			try
			{
				string path = questionAudio;
				if (Helper.GetMediaType(path) == EMedia.Audio)
				{
					buttonQuestionAudio.Tag = CreateMedia(EMedia.Audio, path, true, true, false);
					buttonQuestionAudio.Image = Properties.Resources.AudioAvailable;
				}
			}
			catch { }
			try
			{
				string path = questionExampleAudio;
				if (Helper.GetMediaType(path) == EMedia.Audio)
				{
					buttonQuestionExampleAudio.Tag = CreateMedia(EMedia.Audio, path, true, true, true);
					buttonQuestionExampleAudio.Image = Properties.Resources.AudioAvailable;
				}
			}
			catch { }
			try
			{
				string path = answerAudio;
				if (Helper.GetMediaType(path) == EMedia.Audio)
				{
					buttonAnswerAudio.Tag = CreateMedia(EMedia.Audio, path, true, true, false);
					buttonAnswerAudio.Image = Properties.Resources.AudioAvailable;
				}
			}
			catch { }
			try
			{
				string path = answerExampleAudio;
				if (Helper.GetMediaType(path) == EMedia.Audio)
				{
					buttonAnswerExampleAudio.Tag = CreateMedia(EMedia.Audio, path, true, true, true);
					buttonAnswerExampleAudio.Image = Properties.Resources.AudioAvailable;
				}
			}
			catch { }
			try
			{
				string path = questionVideo;
				if (Helper.GetMediaType(path) == EMedia.Video)
				{
					buttonQuestionVideo.Tag = CreateMedia(EMedia.Video, path, true, true, false);
					buttonQuestionVideo.Image = Properties.Resources.VideoAvailable;
				}
			}
			catch { }
			try
			{
				string path = answerVideo;
				if (Helper.GetMediaType(path) == EMedia.Video)
				{
					buttonAnswerVideo.Tag = CreateMedia(EMedia.Video, path, true, true, false);
					buttonAnswerVideo.Image = Properties.Resources.VideoAvailable;
				}
			}
			catch { }

			CardLoaded();

			newCard = true;
			preloadedCard = true;
			Modified = false;
		}

		/// <summary>
		/// Loads the card for editing.
		/// </summary>
		/// <param name="cardID">The card ID.</param>
		/// <remarks>Documented by Dev05, 2007-10-11</remarks>
		public void LoadCardForEditing(Dictionary currentDictionary, int cardID)
		{
			Card card = currentDictionary.Cards.GetCardByID(cardID);
			LoadCardForEditing(currentDictionary, card.BaseCard);
			ApplyPermissions(card);
		}

		/// <summary>
		/// Loads the card for editing.
		/// </summary>
		/// <remarks>Documented by Dev05, 2007-09-27</remarks>
		public void LoadCardForEditing(Dictionary currentDictionary, ICard card)
		{
			buttonStyle.Enabled = true;

			CheckModified();
			Multiselect = false;

			Dictionary = currentDictionary;

			StopPlayingVideos();

			checkBoxSamePicture.Checked = false;

			pictureBoxQuestion.Tag = null;
			pictureBoxQuestion.Image = null;
			pictureBoxAnswer.Tag = null;
			pictureBoxAnswer.Image = null;

			buttonAnswerAudio.Tag = null;
			buttonAnswerExampleAudio.Tag = null;
			buttonQuestionAudio.Tag = null;
			buttonQuestionExampleAudio.Tag = null;

			buttonAnswerVideo.Tag = null;
			buttonQuestionVideo.Tag = null;

			CardID = card.Id;
			buttonAddEdit.Text = Properties.Resources.EDITCARD_EDIT;
			buttonAddEdit.Image = Properties.Resources.texteditor;

			buttonQuestionAudio.Image = Properties.Resources.Audio;
			buttonQuestionExampleAudio.Image = Properties.Resources.Audio;
			buttonQuestionVideo.Image = Properties.Resources.Video;
			buttonAnswerAudio.Image = Properties.Resources.Audio;
			buttonAnswerExampleAudio.Image = Properties.Resources.Audio;
			buttonAnswerVideo.Image = Properties.Resources.Video;

			SetWord(Side.Question, card.Question.ToNewlineString());
			SetWord(Side.Answer, card.Answer.ToNewlineString());
			SetDistractors(Side.Question, DistractorsToStringList(card.QuestionDistractors.Words));
			SetDistractors(Side.Answer, DistractorsToStringList(card.AnswerDistractors.Words));

			textBoxQuestionExample.Text = card.QuestionExample.ToString();
			textBoxAnswerExample.Text = card.AnswerExample.ToString();

			if (comboBoxChapter.Items.Contains(Properties.Resources.MAINTAIN_UNCHANGED))
				comboBoxChapter.Items.Remove(Properties.Resources.MAINTAIN_UNCHANGED);
			comboBoxChapter.SelectedItem = Dictionary.Chapters.GetChapterByID(card.Chapter);

			checkBoxActive.CheckState = card.Active ? CheckState.Checked : CheckState.Unchecked;
			comboBoxCardBox.SelectedIndex = card.Box != -1 ? card.Box : 0;
			comboBoxCardBox.DropDownStyle = ComboBoxStyle.DropDownList;

			//Images
			IImage image = Dictionary.Cards.GetImageObject(card, Side.Question, true) as IImage;
			if (image != null && image.Filename != string.Empty)
			{
				Image picture = Bitmap.FromStream(image.Stream);
				CheckAndRestoreGIF(ref picture);
				pictureBoxQuestion.Image = picture;
				pictureBoxQuestion.Tag = image;
			}

			image = Dictionary.Cards.GetImageObject(card, Side.Answer, true) as IImage;
			if (image != null && image.Filename != string.Empty)
			{
				if ((pictureBoxQuestion.Tag as IImage) != null && image.Filename == ((IMedia)pictureBoxQuestion.Tag).Filename)
				{
					checkBoxSamePicture.Checked = true;
					pictureBoxAnswer.Image = pictureBoxQuestion.Image;
				}
				else
				{
					Image picture = Bitmap.FromStream(image.Stream);
					CheckAndRestoreGIF(ref picture);
					pictureBoxAnswer.Image = picture;
					pictureBoxAnswer.Tag = image;
				}
			}

			//Audio
			IAudio audio = Dictionary.Cards.GetAudioObject(card, Side.Question, true, false, true) as IAudio;
			if (audio != null && audio.Filename != string.Empty)
			{
				buttonQuestionAudio.Tag = audio;
				buttonQuestionAudio.Image = Properties.Resources.AudioAvailable;
			}
			audio = Dictionary.Cards.GetAudioObject(card, Side.Question, false, true, true) as IAudio;
			if (audio != null && audio.Filename != string.Empty)
			{
				buttonQuestionExampleAudio.Tag = audio;
				buttonQuestionExampleAudio.Image = Properties.Resources.AudioAvailable;
			}
			audio = Dictionary.Cards.GetAudioObject(card, Side.Answer, true, false, true) as IAudio;
			if (audio != null && audio.Filename != string.Empty)
			{
				buttonAnswerAudio.Tag = audio;
				buttonAnswerAudio.Image = Properties.Resources.AudioAvailable;
			}
			audio = Dictionary.Cards.GetAudioObject(card, Side.Answer, false, true, true) as IAudio;
			if (audio != null && audio.Filename != string.Empty)
			{
				buttonAnswerExampleAudio.Tag = audio;
				buttonAnswerExampleAudio.Image = Properties.Resources.AudioAvailable;
			}

			//Video
			IVideo video = Dictionary.Cards.GetVideoObject(card, Side.Question, true) as IVideo;
			if (video != null && video.Filename != string.Empty)
			{
				buttonQuestionVideo.Tag = video;
				buttonQuestionVideo.Image = Properties.Resources.VideoAvailable;
			}
			video = Dictionary.Cards.GetVideoObject(card, Side.Answer, true) as IVideo;
			if (video != null && video.Filename != string.Empty)
			{
				buttonAnswerVideo.Tag = video;
				buttonAnswerVideo.Image = Properties.Resources.VideoAvailable;
			}

			CardLoaded();

			newCard = false;
			preloadedCard = false;
			Modified = false;
		}

		/// <summary>
		/// Applies the permissions to the interface.
		/// </summary>
		/// <param name="card">The card.</param>
		/// <remarks>Documented by Dev03, 2009-01-19</remarks>
		public void ApplyPermissions(Card card)
		{
			//TODO implement CardEdit.ApplyPermissions()
		}

		/// <summary>
		/// Card is loaded.
		/// </summary>
		/// <remarks>Documented by Dev05, 2008-01-04</remarks>
		private void CardLoaded()
		{
			ListViewsModified();
		}

		/// <summary>
		/// Handles the Click event of the buttonAddEdit control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-10-11</remarks>
		private void buttonAddEdit_Click(object sender, EventArgs e)
		{
			if (newCard && !Multiselect)
			{
				if (IsCardLoaded)
				{
					if (CardCollector)
					{
						LoadingCard = true;
						Application.DoEvents();
					}
					AddCard();
				}
			}
			else
				EditCard();
		}

		/// <summary>
		/// Adds the card.
		/// </summary>
		/// <remarks>Documented by Dev05, 2007-10-01</remarks>
		private ICard AddCard()
		{
			if (Multiselect)
				return null;

			ICard card = Dictionary.Cards.AddNew();

			SetCardValues(card);

			OnAdd(EventArgs.Empty);

			return card;
		}

		/// <summary>
		/// Edits the card.
		/// </summary>
		/// <remarks>Documented by Dev05, 2007-10-11</remarks>
		private void EditCard()
		{
			if (Multiselect)
				return;

			ICard card = Dictionary.Cards.GetCardByID(CardID).BaseCard;

			SetCardValues(card);

			OnEdit(EventArgs.Empty);
		}

		/// <summary>
		/// Sets the card values.
		/// </summary>
		/// <param name="card">The card.</param>
		/// <remarks>Documented by Dev05, 2007-10-12</remarks>
		private void SetCardValues(ICard card)
		{
			card.Answer.ClearWords();
			card.Question.ClearWords();
			card.Answer.AddWords(GetWord(Side.Answer).Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries));
			card.Question.AddWords(GetWord(Side.Question).Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries));
			card.AnswerDistractors.ClearWords();
			card.QuestionDistractors.ClearWords();
			card.AnswerDistractors.AddWords(GetDistractors(Side.Answer).ToArray());
			card.QuestionDistractors.AddWords(GetDistractors(Side.Question).ToArray());

			card.AnswerExample.ClearWords();
			card.AnswerExample.AddWord(card.AnswerExample.CreateWord(textBoxAnswerExample.Text, WordType.Sentence, false));
			card.QuestionExample.ClearWords();
			card.QuestionExample.AddWord(card.QuestionExample.CreateWord(textBoxQuestionExample.Text, WordType.Sentence, false));

			if (comboBoxChapter.SelectedItem != null && comboBoxChapter.SelectedItem is IChapter)
				card.Chapter = ((IChapter)comboBoxChapter.SelectedItem).Id;
			card.Active = checkBoxActive.Checked;

			card.ClearAllMedia(false);

			try
			{
				this.Parent.Enabled = false;        //ML-2413, maintain doesn't block user inputs while loading stuff into db
				//currently only the DB can show a progress bar
				card.CreateMediaProgressChanged += new StatusMessageEventHandler(card_CreateMediaProgressChanged);
				statusDialog = new LoadStatusMessage(Resources.CARDEDIT_LOADING_MEDIA_TO_DB, 100, dictionary.IsDB);
				if (!(card is DAL.Preview.PreviewCard)) statusDialog.Show();
				statusDialog.SetProgress(0);

				//Image
				if (pictureBoxAnswer.Tag as IImage != null)
				{
					if (checkBoxResizeAnswer.Checked)
						ResizePicture(pictureBoxAnswer);
					card.AddMedia(pictureBoxAnswer.Tag as IMedia, Side.Answer);
				}
				if (pictureBoxQuestion.Tag as IImage != null)
				{
					if (checkBoxResizeQuestion.Checked)
						ResizePicture(pictureBoxQuestion);

					IMedia media = pictureBoxQuestion.Tag as IMedia;
					media = card.AddMedia(media, Side.Question);

					if (checkBoxSamePicture.Checked)
						card.AddMedia(media, Side.Answer);
				}

				//Audio
				if (buttonAnswerAudio.Tag as IAudio != null)
					card.AddMedia(buttonAnswerAudio.Tag as IMedia, Side.Answer);
				if (buttonAnswerExampleAudio.Tag as IAudio != null)
					card.AddMedia(buttonAnswerExampleAudio.Tag as IMedia, Side.Answer);
				if (buttonQuestionAudio.Tag as IAudio != null)
					card.AddMedia(buttonQuestionAudio.Tag as IMedia, Side.Question);
				if (buttonQuestionExampleAudio.Tag as IAudio != null)
					card.AddMedia(buttonQuestionExampleAudio.Tag as IMedia, Side.Question);

				//video
				if (buttonAnswerVideo.Tag as IVideo != null)
					card.AddMedia(buttonAnswerVideo.Tag as IMedia, Side.Answer);
				if (buttonQuestionVideo.Tag as IVideo != null)
					card.AddMedia(buttonQuestionVideo.Tag as IMedia, Side.Question);

				if (card.Active)
				{
					int oldBox = card.Box;
					card.Box = comboBoxCardBox.SelectedIndex;
					int newBox = card.Box;

					if (oldBox != newBox)
					{
						LearnLogStruct lls = new LearnLogStruct();
						lls.CardsID = card.Id;
						lls.SessionID = Log.LastSessionID;
						lls.MoveType = MoveType.Manual;
						lls.OldBox = oldBox;
						lls.NewBox = newBox;
						//Dummy values:
						lls.TimeStamp = DateTime.Now;
						lls.Duration = 0;
						lls.CaseSensitive = false;
						lls.CorrectOnTheFly = false;
						lls.Direction = EQueryDirection.Question2Answer;
						lls.LearnMode = EQueryType.Word;

						Log.CreateLearnLogEntry(lls, dictionary.DictionaryDAL.Parent);
					}
				}

				CardID = card.Id;
				Modified = false;
			}
			catch
			{
				throw;
			}
			finally
			{
				this.Parent.Enabled = true;
				card.CreateMediaProgressChanged -= new StatusMessageEventHandler(card_CreateMediaProgressChanged);
				if (statusDialog != null)
				{
					statusDialog.Close();
					statusDialog = null;
				}
			}
		}

		/// <summary>
		/// Handles the CreateMediaProgressChanged event of the card control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="args">The <see cref="MLifter.DAL.Tools.StatusMessageEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev03, 2008-08-25</remarks>
		private void card_CreateMediaProgressChanged(object sender, StatusMessageEventArgs args)
		{
			if (statusDialog != null)
			{
				statusDialog.SetProgress(args.ProgressPercentage);

				//ML-1767 Extremely long time to add large video file to card (and no accurate progress meter)
				if (args.ProgressPercentage >= 99)
				{
					if (statusDialog.EnableProgressbar)
					{
						statusDialog.EnableProgressbar = false;
						statusDialog.Refresh();
					}
				}
				else
				{
					if (!statusDialog.EnableProgressbar)
					{
						statusDialog.EnableProgressbar = true;
						statusDialog.Refresh();
					}
				}
			}
		}

		/// <summary>
		/// Handles the CreateMediaProgressChanged event of the Dictionary control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="args">The <see cref="MLifter.DAL.Tools.StatusMessageEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev03, 2008-08-22</remarks>
		void Dictionary_CreateMediaProgressChanged(object sender, MLifter.DAL.Tools.StatusMessageEventArgs args)
		{
			if (statusDialog != null)
				statusDialog.SetProgress(args.ProgressPercentage);
		}

		/// <summary>
		/// Handles the CheckedChanged event of the checkBoxActive control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-10-15</remarks>
		private void checkBoxActive_CheckedChanged(object sender, EventArgs e)
		{
			Modified = true;
		}

		/// <summary>
		/// Handles the SelectedIndexChanged event of the comboBoxCardBox control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2007-12-19</remarks>
		private void comboBoxCardBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			Modified = true;
		}

		/// <summary>
		/// Handles the ValueChanged event of the numericUpDownBox control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-10-15</remarks>
		private void numericUpDownBox_ValueChanged(object sender, EventArgs e)
		{
			Modified = true;
		}

		/// <summary>
		/// Handles the SelectedIndexChanged event of the comboBoxChapter control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-10-15</remarks>
		private void comboBoxChapter_SelectedIndexChanged(object sender, EventArgs e)
		{
			Modified = true;
		}

		/// <summary>
		/// Refreshes the ListView data and sets the modified state.
		/// </summary>
		/// <remarks>Documented by Dev05, 2007-10-17</remarks>
		private void ListViewsModified()
		{
			CheckListViewLines(listViewQuestion);
			CheckListViewLines(listViewAnswer);

			Modified = true;
		}

		/// <summary>
		/// Checks the list view lines.
		/// </summary>
		/// <param name="listView">The list view.</param>
		/// <remarks>Documented by Dev02, 2007-11-30</remarks>
		private static void CheckListViewLines(ListView listView)
		{
			StripListViewLines(listView);

			listView.BeginUpdate();

			//check all lines for missing tags
			foreach (ListViewItem lvi in listView.Items)
			{
				if (lvi.Tag == null || !(lvi.Tag is WordType))
					lvi.Tag = WordType.Word;
			}

			//check for lines to add
			if (listView.Items.Count == 0 || (listView.Items.Count > 0 && !string.IsNullOrEmpty(listView.Items[listView.Items.Count - 1].Text)))
				listView.Items.Add(string.Empty);

			//highlight distractors
			foreach (ListViewItem item in listView.Items)
			{
				if (item.Tag is WordType && ((WordType)item.Tag == WordType.Distractor))
				{
					item.BackColor = Properties.Settings.Default.distractorColor;
					item.ToolTipText = Properties.Resources.CARDEDIT_TOOLTIP_DISTRACTOR;
				}
				else
					item.BackColor = Color.Transparent;
			}

			//highlight each second line
			int line = 0;
			foreach (ListViewItem lvi in listView.Items)
			{
				if (lvi.BackColor != Properties.Settings.Default.distractorColor)
				{
					if (line % 2 > 0)
						lvi.BackColor = Properties.Settings.Default.secondLineColor;
					else
						lvi.BackColor = listView.BackColor;
				}
				line++;
			}

			listView.EndUpdate();
			listView.Invalidate();
		}

		/// <summary>
		/// Removes all empty list view lines.
		/// </summary>
		/// <param name="listView">The list view.</param>
		/// <remarks>Documented by Dev02, 2008-01-28</remarks>
		private static void StripListViewLines(ListView listView)
		{
			listView.BeginUpdate();

			//check for lines to delete
			List<ListViewItem> lvisToDelete = new List<ListViewItem>();
			foreach (ListViewItem lvi in listView.Items)
				if (lvi == null || string.IsNullOrEmpty(lvi.Text))
					lvisToDelete.Add(lvi);
			foreach (ListViewItem lvi in lvisToDelete)
				listView.Items.Remove(lvi);

			listView.EndUpdate();
			listView.Invalidate();
		}

		/// <summary>
		/// Sends the char to a listview.
		/// </summary>
		/// <param name="symbol">The symbol.</param>
		/// <param name="control">The control.</param>
		/// <remarks>Documented by Dev05, 2007-10-31</remarks>
		public void SendListViewChar(string symbol, ListView listView)
		{
			if (listView != null)
			{
				if (listView.SelectedItems.Count > 0)
				{
					listView.SelectedItems[0].Text += symbol;
					ListViewsModified();
				}
			}
		}

		/// <summary>
		/// Handles the Click event of the buttonStyle control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-10-31</remarks>
		private void buttonStyle_Click(object sender, EventArgs e)
		{
			EditCardStyle();
		}

		/// <summary>
		/// Edits the card style.
		/// </summary>
		/// <remarks>Documented by Dev05, 2007-10-31</remarks>
		protected virtual void EditCardStyle()
		{
			Card styleCard = Dictionary.Cards.GetCardByID(CardID);

			if (newCard || (styleCard == null))
				return;

			ICard card = styleCard.BaseCard;
			if (card.Settings == null) card.Settings = Dictionary.CreateSettings();
			if (card.Settings.Style == null)
				card.Settings.Style = card.CreateCardStyle();
			CardStyleEditor editor = new CardStyleEditor();
			editor.HelpNamespace = this.HelpNamespace;
			editor.LoadStyle(card, card.Settings.Style, Dictionary, card);
			switch (editor.ShowDialog())
			{
				case DialogResult.Abort:
					card.Settings.Style = null;
					break;
				case DialogResult.Cancel:
					break;
				default:
					AddStyle(this, EventArgs.Empty);
					break;
			}
		}

		/// <summary>
		/// Handles the CheckedChanged event of the checkBoxSamePicture control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-11-14</remarks>
		private void checkBoxSamePicture_CheckedChanged(object sender, EventArgs e)
		{
			if (!(sender as CheckBox).Checked)
			{
				//checkbox was unchecked, delete image from answer side
				pictureBoxAnswer.Image = null;
				pictureBoxAnswer.Tag = null;
			}
			Modified = true;
			UpdateSamePicture();
		}

		/// <summary>
		/// Updates the same picture in case the corresponding checkbox is checked.
		/// </summary>
		/// <remarks>Documented by Dev02, 2008-01-11</remarks>
		private void UpdateSamePicture()
		{
			if (checkBoxSamePicture.Checked)
			{
				pictureBoxAnswer.Image = pictureBoxQuestion.Image;
				pictureBoxAnswer.Tag = null;
				pictureBoxAnswer.Enabled = false;
				buttonAnswerImage.Enabled = false;
			}
			else
			{
				pictureBoxAnswer.Enabled = true;
				buttonAnswerImage.Enabled = true;
			}
		}

		/// <summary>
		/// Begins the edit of the question.
		/// </summary>
		/// <remarks>Documented by Dev05, 2007-11-22</remarks>
		public void BeginEdit()
		{
			if (SynonymModeEnabled(Side.Question))
			{
				if (listViewQuestion.Items.Count > 0)
				{
					listViewQuestion.SelectedItems.Clear();
					listViewQuestion.Items[0].Selected = true;
				}

				listViewQuestion.Focus();
			}
			else
			{
				textBoxQuestion.Focus();
			}
		}

		/// <summary>
		/// Handles the LoadCompleted event of the pictureBox control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.ComponentModel.AsyncCompletedEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-11-23</remarks>
		private void ResizePicture(PictureBox pic)
		{
			IMedia previousMedia = pic.Tag as IMedia;
			if (previousMedia == null)
				return;

			int maxWidth, maxHeight;

			ResizeMode mode = pic.Name.Contains("Answer") ? AnswerResizeMode : QuestionResizeMode;
			switch (mode)
			{
				case ResizeMode.Small:
					maxWidth = Settings.Default.resizeSmall.Width;
					maxHeight = Settings.Default.resizeSmall.Height;
					break;
				case ResizeMode.Medium:
					maxWidth = Settings.Default.resizeMedium.Width;
					maxHeight = Settings.Default.resizeMedium.Height;
					break;
				case ResizeMode.Large:
					maxWidth = Settings.Default.resizeLarge.Width;
					maxHeight = Settings.Default.resizeLarge.Height;
					break;
				case ResizeMode.None:
				default:
					return;
			}

			if (pic.Image.Width <= maxWidth && pic.Image.Height <= maxHeight)
				return;

			int width, height;

			if (pic.Image.Width / maxWidth > pic.Image.Height / maxHeight)
			{
				width = maxWidth;
				height = (int)(maxWidth * 1.0 / pic.Image.Width * pic.Image.Height);
			}
			else
			{
				height = maxHeight;
				width = (int)(maxHeight * 1.0 / pic.Image.Height * pic.Image.Width);
			}

			string tmpPath = Path.GetTempFileName();
			tmpPath = Path.Combine(Path.GetDirectoryName(tmpPath), Path.GetFileNameWithoutExtension(tmpPath) + Path.GetExtension(((IImage)pic.Tag).Extension));

			Image newImage = new Bitmap(pic.Image, width, height);
			newImage.Save(tmpPath, pic.Image.RawFormat);

			pic.Load(tmpPath);
			previousMedia.Filename = tmpPath;
		}

		/// <summary>
		/// Handles the CheckedChanged event of the checkBoxResizeQuestion control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-11-29</remarks>
		private void checkBoxResizeQuestion_CheckedChanged(object sender, EventArgs e)
		{
			if (changingCheckstate)
				return;

			if (!checkBoxResizeQuestion.Checked)
			{
				QuestionResizeMode = ResizeMode.None;
				checkBoxResizeQuestion.Image = Resources.resize;
			}
			else
			{
				TaskDialogResult result = ShowResizeDialog();
				int commandButtonResult = result.CommandButtonsIndex;

				switch (commandButtonResult)
				{
					case 0:
						QuestionResizeMode = ResizeMode.Small;
						checkBoxResizeQuestion.Image = Resources.resizeS;
						Modified = true;
						break;
					case 1:
						QuestionResizeMode = ResizeMode.Medium;
						checkBoxResizeQuestion.Image = Resources.resizeM;
						Modified = true;
						break;
					case 2:
						QuestionResizeMode = ResizeMode.Large;
						checkBoxResizeQuestion.Image = Resources.resizeL;
						Modified = true;
						break;
					case 3:
					default:
						QuestionResizeMode = ResizeMode.None;
						checkBoxResizeQuestion.Image = Resources.resize;
						break;
				}

				if (result.VerificationChecked)
				{
					AnswerResizeMode = QuestionResizeMode;
					checkBoxResizeAnswer.Image = checkBoxResizeQuestion.Image;
				}
			}

			checkBoxResizeQuestion.Checked = (QuestionResizeMode != ResizeMode.None);
		}

		/// <summary>
		/// Handles the CheckedChanged event of the checkBoxResizeAnswer control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-11-29</remarks>
		private void checkBoxResizeAnswer_CheckedChanged(object sender, EventArgs e)
		{
			if (changingCheckstate)
				return;

			if (!checkBoxResizeAnswer.Checked)
			{
				AnswerResizeMode = ResizeMode.None;
				checkBoxResizeAnswer.Image = Resources.resize;
			}
			else
			{
				TaskDialogResult result = ShowResizeDialog();
				int commandButtonResult = result.CommandButtonsIndex;

				switch (commandButtonResult)
				{
					case 0:
						AnswerResizeMode = ResizeMode.Small;
						checkBoxResizeAnswer.Image = Resources.resizeS;
						Modified = true;
						break;
					case 1:
						AnswerResizeMode = ResizeMode.Medium;
						checkBoxResizeAnswer.Image = Resources.resizeM;
						Modified = true;
						break;
					case 2:
						AnswerResizeMode = ResizeMode.Large;
						checkBoxResizeAnswer.Image = Resources.resizeL;
						Modified = true;
						break;
					case 3:
					default:
						AnswerResizeMode = ResizeMode.None;
						checkBoxResizeAnswer.Image = Resources.resize;
						Modified = true;
						break;
				}

				if (result.VerificationChecked)
				{
					QuestionResizeMode = AnswerResizeMode;
					checkBoxResizeQuestion.Image = checkBoxResizeAnswer.Image;
				}
			}

			checkBoxResizeAnswer.Checked = (AnswerResizeMode != ResizeMode.None);
		}

		/// <summary>
		/// Shows the resize dialog.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2007-11-29</remarks>
		private TaskDialogResult ShowResizeDialog()
		{
			EmulatedTaskDialog dialog = new EmulatedTaskDialog();
			dialog.Title = Resources.CARD_EDIT_RESIZE_TITLE;
			dialog.MainInstruction = Resources.CARD_EDIT_RESIZE_TEXT;
			dialog.Content = Resources.CARD_EDIT_RESIZE_DESCRIPTION;
			dialog.CommandButtons = string.Format(Resources.CARD_EDIT_RESIZE_BUTTONS,
				new object[]
					{
						Settings.Default.resizeSmall.Width,
						Settings.Default.resizeSmall.Height,
						Settings.Default.resizeMedium.Width,
						Settings.Default.resizeMedium.Height,
						Settings.Default.resizeLarge.Width,
						Settings.Default.resizeLarge.Height
					});
			dialog.VerificationText = Resources.CARD_EDIT_RESIZE_CHECKBOX;
			dialog.VerificationCheckBoxChecked = true;
			dialog.Buttons = TaskDialogButtons.Cancel;
			dialog.MainIcon = TaskDialogIcons.Question;
			dialog.FooterIcon = TaskDialogIcons.Warning;
			dialog.MainImages = new Image[] { Resources.resizeS, Resources.resizeM, Resources.resizeL, Resources.resize };
			dialog.HoverImages = new Image[] { Resources.resizeS, Resources.resizeM, Resources.resizeL, Resources.resize };
			dialog.CenterImages = true;
			dialog.BuildForm();
			DialogResult dialogResult = dialog.ShowDialog();

			if (dialog.VerificationCheckBoxChecked && dialogResult != DialogResult.Cancel)
			{
				changingCheckstate = true;
				checkBoxResizeAnswer.Checked = (dialog.CommandButtonClickedIndex < 3);
				checkBoxResizeQuestion.Checked = (dialog.CommandButtonClickedIndex < 3);
				changingCheckstate = false;
			}

			TaskDialogResult result = new TaskDialogResult();
			result.CommandButtonsIndex = dialog.CommandButtonClickedIndex;
			result.VerificationChecked = dialog.VerificationCheckBoxChecked;

			return result;
		}

		/// <summary>
		/// Handles the AfterLabelEdit event of the listView control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.LabelEditEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2007-11-30</remarks>
		private void listView_AfterLabelEdit(object sender, LabelEditEventArgs e)
		{
			ListView listView = sender as ListView;

			//AfterLabelEdit event gets called when the user finished his works, but the new item Text is not copied to the listview item yet
			//Workaround: Copy the changed Text manually and cancel the edit, so that the ListViewsModified function can work with the latest changes
			if (e.Label != null)
			{
				if (listView.SelectedItems.Count > 0)
				{
					listView.SelectedItems[0].Text = e.Label;
					e.CancelEdit = true;
				}
			}

			ListViewsModified();

			//select the next item
			if (listView.SelectedItems.Count > 0)
			{
				if (listView.SelectedIndices[0] < listView.Items.Count - 1)
				{
					int newselectionindex = listView.SelectedIndices[0] + 1;
					listView.SelectedIndices.Clear();
					listView.Items[newselectionindex].Selected = true;
					listView.Items[newselectionindex].Focused = true;
					listView.Items[newselectionindex].EnsureVisible();
				}
			}
		}

		/// <summary>
		/// Handles the KeyDown event of the listView control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2007-11-30</remarks>
		private void listView_KeyDown(object sender, KeyEventArgs e)
		{
			//preserve the functions of some special keys
			ListView lv = sender as ListView;
			if (lv.SelectedItems.Count > 0)
			{
				if (!e.Handled && e.KeyCode == Keys.Delete)
				{
					//DEL Key
					contextMenuStripSynonyms_Opening(null, null);
					deleteToolStripMenuItem.PerformClick();
					e.Handled = true;
				}
				else if (!e.Handled && (e.KeyCode == Keys.C || e.KeyCode == Keys.X) && e.Control)
				{
					//STRG+C Copy,STRG+X Cut Key
					contextMenuStripSynonyms_Opening(null, null);
					if (e.KeyCode == Keys.C)
					{
						copyToolStripMenuItem.PerformClick();
					}
					else if (e.KeyCode == Keys.X)
					{
						cutToolStripMenuItem.PerformClick();
					}
					e.Handled = true;
				}
			}

			if (!e.Handled && e.KeyCode == Keys.V && e.Control && Clipboard.ContainsText())
			{
				//STRG+V Paste Key
				contextMenuStripSynonyms_Opening(null, null);
				pasteToolStripMenuItem.PerformClick();
				e.Handled = true;
			}

			if (!e.Handled && e.KeyCode == Keys.A && e.Control)
			{
				//Select all
				lv.SelectedItems.Clear();
				foreach (ListViewItem lvi in lv.Items)
					if (!string.IsNullOrEmpty(lvi.Text))
						lvi.Selected = true;
				e.Handled = true;
			}
		}

		/// <summary>
		/// Copies or cuts a listview entry to the clipboard.
		/// </summary>
		/// <param name="lv">The lv.</param>
		/// <param name="cut">if set to <c>true</c> [cut].</param>
		/// <remarks>Documented by Dev02, 2008-01-29</remarks>
		private void CopyCut(ListView lv, bool cut)
		{
			if (lv != null)
			{
				List<string> text = new List<string>();
				List<ListViewItem> remove = new List<ListViewItem>();
				foreach (ListViewItem lvi in lv.SelectedItems)
				{
					text.Add(lvi.Text);
					if (cut)
						lvi.Text = string.Empty;
				}

				if (text.Count > 0)
				{
					Clipboard.SetText(string.Join(Environment.NewLine, text.ToArray()));
				}

				if (cut)
				{
					ListViewsModified();
					RefillListview(lv);
				}
			}
		}

		/// <summary>
		/// Handles the Click event of the cutToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-01-29</remarks>
		private void cutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ListView listView = contextMenuStripSynonyms.Tag as ListView;

			CopyCut(listView, true);
		}

		/// <summary>
		/// Handles the Click event of the copyToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-01-29</remarks>
		private void copyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ListView listView = contextMenuStripSynonyms.Tag as ListView;

			CopyCut(listView, false);
		}

		/// <summary>
		/// Handles the Click event of the pasteToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-01-29</remarks>
		private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ListView lv = contextMenuStripSynonyms.Tag as ListView;

			if (lv != null)
			{
				string text = Clipboard.GetText();
				if (text != string.Empty)
				{
					string[] items = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

					foreach (string item in items)
					{
						if (item != string.Empty)
						{
							ListViewItem lvi = new ListViewItem();
							lvi.Text = item;
							lvi.Tag = WordType.Word;
							int index = -1;
							if (lv.SelectedIndices.Count > 0)
								index = lv.SelectedIndices[0];
							if (index >= 0 && index < lv.Items.Count)
								lv.Items.Insert(index, lvi);
							else
								lv.Items.Add(lvi);
						}
					}
					ListViewsModified();
					listView_ClientSizeChanged(lv, null);
				}
			}

		}

		/// <summary>
		/// Handles the Click event of the deleteToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-01-29</remarks>
		private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ListView lv = contextMenuStripSynonyms.Tag as ListView;

			if (lv != null)
			{
				foreach (ListViewItem lvi in lv.SelectedItems)
					if (!string.IsNullOrEmpty(lvi.Text)) lvi.Text = string.Empty;

				ListViewsModified();

				//refill listview, this is necessary because the listview leaves an empty line after deleting all items 
				//except for the first one (probably a scroll problem)
				RefillListview(lv);

				System.Windows.Forms.SendKeys.SendWait("{DOWN}");
			}
		}

		/// <summary>
		/// Refills the listview.
		/// </summary>
		/// <param name="lv">The lv.</param>
		/// <remarks>Documented by Dev02, 2008-01-29</remarks>
		private static void RefillListview(ListView lv)
		{
			ListViewItem[] items = new ListViewItem[lv.Items.Count];
			lv.Items.CopyTo(items, 0);
			lv.Items.Clear();
			lv.Items.AddRange(items);
		}

		const int VK_SHIFT = 0x10;

		[DllImport("user32.dll")]
		static extern bool GetKeyboardState(byte[] lpKeyState);

		[DllImport("user32.dll")]
		static extern bool SetKeyboardState(byte[] lpKeyState);

		/// <summary>
		/// Handles the KeyPress event of the listView control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.KeyPressEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2007-11-30</remarks>
		private void listView_KeyPress(object sender, KeyPressEventArgs e)
		{
			//start editing at the end of the line
			ListView lv = sender as ListView;
			if (lv.SelectedItems.Count == 1 && !e.Handled && !char.IsControl(e.KeyChar))
			{
				//supress shift key
				byte[] keys = new byte[255];
				GetKeyboardState(keys);
				if (keys[VK_SHIFT] != 0)
				{
					keys[VK_SHIFT] = 0;
					SetKeyboardState(keys);
				}

				lv.SelectedItems[0].BeginEdit();
				System.Windows.Forms.SendKeys.SendWait("{END}");
				System.Windows.Forms.SendKeys.SendWait(ReplaceSendKeysCharacters(e.KeyChar.ToString()));
				e.Handled = true;
			}
		}

		/// <summary>
		/// Masks the characters that must not be send to SendKeys. [ML-1248]
		/// </summary>
		/// <param name="Text">The Text.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev02, 2008-05-27</remarks>
		private static string ReplaceSendKeysCharacters(string text)
		{
			return System.Text.RegularExpressions.Regex.Replace(text, @"([\+\^\%\~\(\)\[\]\{\}]{1})", "{$1}");
		}

		/// <summary>
		/// Handles the Enter event of the listView control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2007-12-04</remarks>
		private void listView_Enter(object sender, EventArgs e)
		{
			//autoselecting the first row, so that the user can begin to edit
			ListView listview = sender as ListView;
			if (listview.SelectedItems.Count < 1 && listview.Items.Count > 0)
				listview.SelectedIndices.Add(0);

			//characterForm.CurrentControl = listview;
			contextMenuStripSynonyms.Tag = listview;
		}

		/// <summary>
		/// Handles the Opening event of the contextMenuStripSynonyms control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2008-01-04</remarks>
		private void contextMenuStripSynonyms_Opening(object sender, CancelEventArgs e)
		{
			contextMenuStripSynonyms.RightToLeft = RightToLeft.No;
			List<ListViewItem> items = new List<ListViewItem>();
			ListView listView = contextMenuStripSynonyms.Tag as ListView;

			if (listView != null)
			{
				foreach (ListViewItem item in listView.SelectedItems)
					if (!string.IsNullOrEmpty(item.Text))
						items.Add(item);

				int distractors = 0;
				foreach (ListViewItem item in items)
					if (item.Tag is WordType && ((WordType)item.Tag == WordType.Distractor))
						distractors++;

				if (distractorToolStripMenuItem.Enabled = items.Count > 0)
				{
					if (distractors == items.Count)
						distractorToolStripMenuItem.CheckState = CheckState.Checked;
					else if (distractors > 0)
						distractorToolStripMenuItem.CheckState = CheckState.Indeterminate;
					else
						distractorToolStripMenuItem.CheckState = CheckState.Unchecked;
				}

				pasteToolStripMenuItem.Enabled = Clipboard.ContainsText();
				copyToolStripMenuItem.Enabled = cutToolStripMenuItem.Enabled = deleteToolStripMenuItem.Enabled = items.Count > 0;
			}
		}

		/// <summary>
		/// Handles the Click event of the distractorToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2008-01-04</remarks>
		private void distractorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ListView listView = contextMenuStripSynonyms.Tag as ListView;

			foreach (ListViewItem item in listView.SelectedItems)
			{
				if (item.Tag != null && item.Tag is WordType)
				{
					switch ((WordType)item.Tag)
					{
						case WordType.Word:
							item.Tag = WordType.Distractor;
							break;
						case WordType.Sentence:
							break;
						case WordType.Distractor:
							item.Tag = WordType.Word;
							break;
						default:
							break;
					}
				}
			}

			ListViewsModified();
		}

		/// <summary>
		/// Handles the Click event of the buttonPreview control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2008-01-04</remarks>
		private void buttonPreview_Click(object sender, EventArgs e)
		{
			Card card, styleCard = null;

			//[ML-783] Maintain card: preview only after saving changes! - allways create a new 
			card = new Card(new MLifter.DAL.Preview.PreviewCard(null), dictionary);
			if (!newCard)
				styleCard = Dictionary.Cards.GetCardByID(this.CardID);  //[ML-917] Preview of maintain card doesn't work correct

			bool modified = this.Modified; //[ML-734] SetCardValues resets the Modified-value and disables the add-button
			int cardId = this.CardID;   //[ML-908] Maintain card; Add/Edit Style crashes
			SetCardValues(card.BaseCard);
			this.Modified = modified;
			this.CardID = cardId;

			//[ML-917] Preview of maintain card doesn't work correct
			if (!newCard && (styleCard != null) && (styleCard.BaseCard != null) && (styleCard.BaseCard.Settings != null) && (styleCard.BaseCard.Settings.Style != null))
				card.BaseCard.Settings.Style = styleCard.BaseCard.Settings.Style;

			CardPreview preview = new CardPreview(card, dictionary);
			preview.ShowDialog();
			//cleanup
			if ((card != null) && (card.BaseCard != null)) card.BaseCard.Dispose();
		}

		/// <summary>
		/// Handles the ClientSizeChanged event of the listView control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-01-08</remarks>
		private void listView_ClientSizeChanged(object sender, EventArgs e)
		{
			//Resize the main column so that a horizontal scrollbar does not appear
			ListView listView = sender as ListView;

			if (listView.Columns.Count > 0)
				listView.Columns[0].Width = listView.ClientSize.Width;

			this.Invalidate();
		}

		/// <summary>
		/// Handles the Click event of the whatsThisToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-01-08</remarks>
		private void whatsThisToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Help.ShowHelp(this, this.HelpNamespace, HelpNavigator.Topic, "/html/Distractors.htm");
		}

		/// <summary>
		/// Handles the DoubleClick event of the listView control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-01-22</remarks>
		private void listView_DoubleClick(object sender, EventArgs e)
		{
			//start editing of the selected line
			ListView lv = sender as ListView;
			if (lv.SelectedItems.Count == 1)
			{
				lv.SelectedItems[0].BeginEdit();
				//System.Windows.Forms.SendKeys.SendWait("{END}");
			}
		}

		/// <summary>
		/// Handles the CheckedChanged event of the checkBoxEditor control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-01-28</remarks>
		private void checkBoxSynonym_CheckedChanged(object sender, EventArgs e)
		{
			if (checkBoxSynonymEventEnabled)
			{
				if (sender == checkBoxSynonymQuestion)
					ChangeSynonymMode((sender as CheckBox).Checked, Side.Question);
				else if (sender == checkBoxSynonymAnswer)
					ChangeSynonymMode((sender as CheckBox).Checked, Side.Answer);
			}
		}

		/// <summary>
		/// Returns true, if the synonym Text edit mode is enabled, else false.
		/// </summary>
		/// <param name="cardside">The cardside.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev02, 2008-01-28</remarks>
		protected bool SynonymModeEnabled(Side cardside)
		{
			CheckBox checkbox;

			switch (cardside)
			{
				case Side.Question:
					checkbox = checkBoxSynonymQuestion;
					break;
				case Side.Answer:
					checkbox = checkBoxSynonymAnswer;
					break;
				default:
					return true;
			}

			return checkbox.Checked;
		}

		/// <summary>
		/// Changes the mode between Synonym mode and Word mode.
		/// </summary>
		/// <param name="enabled">if set to <c>true</c> [synonym mode], else [word mode].</param>
		/// <param name="cardside">The cardside.</param>
		/// <remarks>Documented by Dev02, 2008-01-28</remarks>
		protected void ChangeSynonymMode(bool enabled, Side cardside)
		{
			checkBoxSynonymEventEnabled = false;
			TextBox wordTextbox;
			ListView synonymListview;
			CheckBox checkbox;

			GetSideControls(cardside, out wordTextbox, out synonymListview, out checkbox);

			try
			{
				//update controls
				checkbox.Checked = enabled;
				wordTextbox.Visible = wordTextbox.Enabled = wordTextbox.TabStop = !enabled;
				synonymListview.Visible = synonymListview.Enabled = synonymListview.TabStop = enabled;

				//copy contents
				if (enabled)
				{
					GetSynonyms(cardside);
					//if (characterForm.CurrentControl == wordTextbox)
					//    characterForm.CurrentControl = synonymListview;
					contextMenuStripSynonyms.Tag = synonymListview;
					listView_ClientSizeChanged(synonymListview, new EventArgs());
				}
				else
				{
					WritebackSynonyms(cardside);
					//if (characterForm.CurrentControl == synonymListview)
					//    characterForm.CurrentControl = wordTextbox;
				}
			}
			finally
			{
				checkBoxSynonymEventEnabled = true;
				if (synonymListview != null)
					CheckListViewLines(synonymListview);
			}
		}

		/// <summary>
		/// Gets the synonyms from the textbox.
		/// </summary>
		/// <param name="cardside">The cardside.</param>
		/// <remarks>Documented by Dev02, 2008-01-29</remarks>
		protected void GetSynonyms(Side cardside)
		{
			bool modified = this.Modified;

			TextBox textbox;
			ListView listview;
			GetSideControls(cardside, out textbox, out listview);

			try
			{
				listview.Items.Clear();
				string[] lines = textbox.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
				foreach (string line in lines)
				{
					ListViewItem lvi = new ListViewItem();
					lvi.Text = line;
					lvi.Tag = WordType.Word;
					listview.Items.Add(lvi);
				}
				if (textbox.Tag is List<string>)
				{
					List<string> distractors = textbox.Tag as List<string>;
					foreach (string distractor in distractors)
					{
						ListViewItem lvi = new ListViewItem();
						lvi.Text = distractor;
						lvi.Tag = WordType.Distractor;
						listview.Items.Add(lvi);
					}
				}
			}
			finally
			{
				CheckListViewLines(listview);
				this.Modified = modified;
			}
		}

		/// <summary>
		/// Writes the synonyms back to the textbox.
		/// </summary>
		/// <param name="cardside">The cardside.</param>
		/// <remarks>Documented by Dev02, 2008-01-29</remarks>
		protected void WritebackSynonyms(Side cardside)
		{
			bool modified = this.Modified;

			TextBox textbox;
			ListView listview;
			GetSideControls(cardside, out textbox, out listview);

			StripListViewLines(listview);
			List<string> distractors = new List<string>();

			List<string> words = new List<string>();
			foreach (ListViewItem lvi in listview.Items)
			{
				if (lvi.Tag is WordType && ((WordType)lvi.Tag) == WordType.Word)
				{
					words.Add(lvi.Text);
				}
				else if (lvi.Tag is WordType && ((WordType)lvi.Tag) == WordType.Distractor)
				{
					distractors.Add(lvi.Text);
				}
			}
			textbox.Text = string.Join(Environment.NewLine, words.ToArray());
			textbox.Tag = distractors;
			this.Modified = modified;
		}

		/// <summary>
		/// Gets the side controls.
		/// </summary>
		/// <param name="cardside">The cardside.</param>
		/// <param name="textbox">The textbox.</param>
		/// <param name="listview">The listview.</param>
		/// <remarks>Documented by Dev02, 2008-01-29</remarks>
		protected void GetSideControls(Side cardside, out TextBox textbox, out ListView listview)
		{
			CheckBox checkbox;
			GetSideControls(cardside, out textbox, out listview, out checkbox);
		}

		/// <summary>
		/// Gets the side controls.
		/// </summary>
		/// <param name="cardside">The cardside.</param>
		/// <param name="textbox">The textbox.</param>
		/// <param name="listview">The listview.</param>
		/// <param name="checkbox">The checkbox.</param>
		/// <remarks>Documented by Dev02, 2008-01-29</remarks>
		protected void GetSideControls(Side cardside, out TextBox textbox, out ListView listview, out CheckBox checkbox)
		{
			switch (cardside)
			{
				case Side.Question:
					textbox = textBoxQuestion;
					listview = listViewQuestion;
					checkbox = checkBoxSynonymQuestion;
					break;
				case Side.Answer:
					textbox = textBoxAnswer;
					listview = listViewAnswer;
					checkbox = checkBoxSynonymAnswer;
					break;
				default:
					textbox = null;
					listview = null;
					checkbox = null;
					return;
			}
		}

		/// <summary>
		/// Gets the word.
		/// </summary>
		/// <param name="cardside">The cardside.</param>
		/// <remarks>Documented by Dev02, 2008-01-29</remarks>
		protected string GetWord(Side cardside)
		{
			if (SynonymModeEnabled(cardside))
				WritebackSynonyms(cardside);

			TextBox textbox;
			ListView listview;
			GetSideControls(cardside, out textbox, out listview);

			return textbox.Text;
		}

		/// <summary>
		/// Gets the distractors.
		/// </summary>
		/// <param name="cardside">The cardside.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev02, 2008-01-29</remarks>
		protected List<string> GetDistractors(Side cardside)
		{
			TextBox textbox;
			ListView listview;
			GetSideControls(cardside, out textbox, out listview);

			if (textbox.Tag is List<string>)
				return textbox.Tag as List<string>;
			else
				return new List<string>();
		}

		/// <summary>
		/// Sets the word.
		/// </summary>
		/// <param name="cardside">The cardside.</param>
		/// <remarks>Documented by Dev02, 2008-01-29</remarks>
		protected void SetWord(Side cardside, string word)
		{
			TextBox textbox;
			ListView listview;
			GetSideControls(cardside, out textbox, out listview);

			textbox.Text = word;
			textbox.Tag = null;

			if (SynonymModeEnabled(cardside))
				GetSynonyms(cardside);
		}

		/// <summary>
		/// Sets the distractors.
		/// </summary>
		/// <param name="cardside">The cardside.</param>
		/// <param name="distractors">The distractors.</param>
		/// <remarks>Documented by Dev02, 2008-01-29</remarks>
		protected void SetDistractors(Side cardside, List<string> distractors)
		{
			TextBox textbox;
			ListView listview;
			GetSideControls(cardside, out textbox, out listview);

			textbox.Tag = distractors;

			if (SynonymModeEnabled(cardside))
				GetSynonyms(cardside);
		}

		/// <summary>
		/// Distractors to string list.
		/// </summary>
		/// <param name="iwords">The iwords.</param>
		/// <returns>The string list containing the distractors.</returns>
		/// <remarks>Documented by Dev02, 2008-01-29</remarks>
		protected List<string> DistractorsToStringList(IList<IWord> iwords)
		{
			List<string> strings = new List<string>();
			foreach (IWord word in iwords)
				if (word.Type == WordType.Distractor)
					strings.Add(word.Word);
			return strings;
		}

		/// <summary>
		/// Handles the CheckedChanged event of the checkBoxCharacterMap control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-05-08</remarks>
		private void checkBoxCharacterMap_CheckedChanged(object sender, EventArgs e)
		{
			characterMapComponent.Visible = (sender as CheckBox).Checked;
		}

		/// <summary>
		/// Handles the DragEnter event of the CardEdit control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.DragEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev03, 2009-05-19</remarks>
		private void CardEdit_DragEnter(object sender, DragEventArgs e)
		{
			e.Effect = DragDropEffects.None;
		}
	}

	public class TaskDialogResult
	{
		public int CommandButtonsIndex;
		public bool VerificationChecked;
	}
}
