using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MLifter.AudioTools;
using MLifter.BusinessLayer;
using MLifter.Controls.Properties;
using MLifter.DAL.Interfaces;

namespace MLifter.Controls
{
	/// <summary>
	/// Used to define or change the global settings of arrayList dictionary.
	/// </summary>
	/// <remarks>Documented by Dev03, 2007-07-19</remarks>
	public partial class PropertiesForm : Form
	{
		#region Private Fields

		private AudioPlayer player;
		private Dictionary<CommentarySoundIdentifier, IMedia> commentarySounds = new Dictionary<CommentarySoundIdentifier, IMedia>();
		private static MLifter.BusinessLayer.Dictionary dictionary;

		#endregion

		#region Private Properties

		/// <summary>
		/// Gets the ID of the selected commentary sound of the ListViews.
		/// </summary>
		/// <value>The selected sound.</value>
		/// <remarks>Documented by Dev02, 2008-01-10</remarks>
		private CommentarySoundIdentifier? SelectedSoundID
		{
			get
			{
				if (!(listboxCommentaryQuestion.SelectedItems.Count > 0 || listboxCommentaryAnswer.SelectedItems.Count > 0))
					return null;

				Side side = listboxCommentaryQuestion.SelectedItems.Count > 0 ? Side.Question : Side.Answer;
				int selectedIndex = listboxCommentaryQuestion.SelectedItems.Count > 0 ? listboxCommentaryQuestion.SelectedIndex : listboxCommentaryAnswer.SelectedIndex;

				ECommentarySoundType type;

				switch (selectedIndex)
				{
					case 0:
						type = ECommentarySoundType.RightStandAlone;
						break;
					case 1:
						type = ECommentarySoundType.WrongStandAlone;
						break;
					case 2:
						type = ECommentarySoundType.AlmostStandAlone;
						break;
					case 3:
						type = ECommentarySoundType.Right;
						break;
					case 4:
						type = ECommentarySoundType.Wrong;
						break;
					case 5:
						type = ECommentarySoundType.Almost;
						break;
					default:
						return null;
				}

				return CommentarySoundIdentifier.Create(side, type);
			}
		}

		/// <summary>
		/// Gets the name of the selected sound.
		/// </summary>
		/// <value>The name of the selected sound.</value>
		/// <remarks>Documented by Dev02, 2008-01-10</remarks>
		private string SelectedSoundName
		{
			get
			{
				if (listboxCommentaryQuestion.SelectedIndex >= 0)
					return groupBoxQuestion.Text + " " + listboxCommentaryQuestion.SelectedItem as string;
				else if (listboxCommentaryAnswer.SelectedIndex >= 0)
					return groupBoxAnswer.Text + " " + listboxCommentaryAnswer.SelectedItem as string;
				else
					return string.Empty;
			}
		}

		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertiesForm"/> class.
		/// </summary>
		/// <remarks>Documented by Dev08, 2009-03-27</remarks>
		public PropertiesForm(string helpfile)
		{
			InitializeComponent();
			MainHelp.HelpNamespace = helpfile;
		}

		/// <summary>
		/// Valids the learning module index entry.
		/// </summary>
		/// <param name="module">The module.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev08, 2009-03-27</remarks>
		private static bool ValidLearningModuleIndexEntry(LearningModulesIndexEntry module)
		{
			if (module == null)
				return false;
			if (!module.IsAccessible || !module.IsVerified)
				return false;
			if (module.NotAccessibleReason != LearningModuleNotAccessibleReason.IsAccessible)
				return false;
			if (module.ConnectionString.Typ == MLifter.DAL.DatabaseType.Xml)
				return false;

			return true;
		}

		#region Events

		/// <summary>
		/// Handles the Click event of the btnOK control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-01-10</remarks>
		private void buttonOK_Click(object sender, EventArgs e)
		{
			if (!learnModes.ValidateInput())
				return;

			if (dictionary.CanModify)
			{
				//save LearningModule captions
				dictionary.DefaultSettings.QuestionCaption = dictionaryCaptions.QuestionTitle;
				dictionary.DefaultSettings.AnswerCaption = dictionaryCaptions.AnswerTitle;
				dictionary.DefaultSettings.QuestionCulture = dictionaryCaptions.QuestionCulture;
				dictionary.DefaultSettings.AnswerCulture = dictionaryCaptions.AnswerCulture;

				//save LearningModule properties
				dictionary.Author = dictionaryProperties.DictionaryAuthor;
				dictionary.Description = dictionaryProperties.DictionaryDescription;
				dictionary.Category = dictionaryProperties.DictionaryCategory;

				//save commentary sounds
				dictionary.DefaultSettings.CommentarySounds = commentarySounds;

				IQueryDirections queryDirections = dictionary.AllowedQueryDirections;
				learnModes.GetQueryDirections(ref queryDirections);
				dictionary.AllowedQueryDirections = queryDirections;

				IQueryType queryTypes = dictionary.AllowedQueryTypes;
				learnModes.GetQueryTypes(ref queryTypes);
				dictionary.AllowedQueryTypes = queryTypes;

				dictionary.Save();
			}

			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		/// <summary>
		/// Handles the Click event of the btnCancel control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-01-10</remarks>
		private void buttonCancel_Click(object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		/// <summary>
		/// Handles the DoubleClick event of the listboxCommentary control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-01-10</remarks>
		private void listboxCommentary_DoubleClick(object sender, EventArgs e)
		{
			buttonChange.PerformClick();
		}

		/// <summary>
		/// Handles the SelectedIndexChanged event of the listbox control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-01-10</remarks>
		private void listbox_SelectedIndexChanged(object sender, EventArgs e)
		{
			//from both lists only one item is allowed to be selected
			if (sender is ListBox)
			{
				ListBox listbox = sender as ListBox;

				if (listbox.SelectedIndex >= 0)
				{
					if (listbox == listboxCommentaryQuestion)
						listboxCommentaryAnswer.SelectedIndex = -1;
					else if (listbox == listboxCommentaryAnswer)
						listboxCommentaryQuestion.SelectedIndex = -1;
				}
			}

			if (SelectedSoundID.HasValue && commentarySounds.ContainsKey(SelectedSoundID.Value) && commentarySounds[SelectedSoundID.Value].Filename != string.Empty)
			{
				textBoxSoundFile.Text = commentarySounds[SelectedSoundID.Value].Filename;
				buttonDelete.Enabled = dictionary.CanModify;
				buttonPlay.Enabled = MLifter.AudioTools.SoundDevicesAvailable.SoundOutDeviceAvailable();
			}
			else
			{
				textBoxSoundFile.Text = string.Empty;
				buttonDelete.Enabled = false;
				buttonPlay.Enabled = false;
			}           
		}

		/// <summary>
		/// Handles the Click event of the buttonDelete control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-01-10</remarks>
		private void buttonDelete_Click(object sender, EventArgs e)
		{
			if (SelectedSoundID.HasValue && commentarySounds.ContainsKey(SelectedSoundID.Value))
				commentarySounds.Remove(SelectedSoundID.Value);
			listbox_SelectedIndexChanged(null, new EventArgs());
		}

		/// <summary>
		/// Handles the Click event of the buttonChange control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-01-10</remarks>
		private void buttonChange_Click(object sender, EventArgs e)
		{
			string soundFile = string.Empty;
			CommentarySoundIdentifier? identifier = SelectedSoundID;

			if (identifier.HasValue && commentarySounds.ContainsKey(identifier.Value))
				soundFile = commentarySounds[identifier.Value].Filename;

			MLifter.Controls.AudioDialog audioDialog = new MLifter.Controls.AudioDialog();
			audioDialog.Path = soundFile;
			audioDialog.DisplayText = SelectedSoundName;
			audioDialog.ShowDialog();

			if (audioDialog.Path != soundFile)
			{
				if (audioDialog.Path == string.Empty)
				{
					if (identifier.HasValue && commentarySounds.ContainsKey(identifier.Value))
						commentarySounds.Remove(identifier.Value);
				}
				else
				{
					commentarySounds[identifier.Value] = dictionary.CreateMedia(EMedia.Audio, audioDialog.Path, true, true, false);
				}
				listbox_SelectedIndexChanged(null, new EventArgs());
			}
		}

		/// <summary>
		/// Handles the Ending event of the player control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-01-11</remarks>
		private void player_Ending(object sender, EventArgs e)
		{
			buttonPlay.Image = Resources.mediaPlaybackStart;
			buttonChange.Enabled = buttonDelete.Enabled = dictionary.CanModify;
			player = null;
		}

		/// <summary>
		/// Handles the Click event of the buttonPlay control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-01-10</remarks>
		private void buttonPlay_Click(object sender, EventArgs e)
		{
			if (player == null && SelectedSoundID.HasValue &&
				commentarySounds.ContainsKey(SelectedSoundID.Value) && commentarySounds[SelectedSoundID.Value].Filename != string.Empty)
			{
				player = new AudioPlayer();
				player.Ending += new EventHandler(player_Ending);
				player.Play(commentarySounds[SelectedSoundID.Value].Filename, true);
				buttonPlay.Image = Resources.mediaPlaybackStop;
				buttonChange.Enabled = buttonDelete.Enabled = false;
			}
			else
			{
				player.Stop();
				player_Ending(null, new EventArgs());
			}
		}

		/// <summary>
		/// Handles the Click event of the buttonStyle control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev08, 2009-03-27</remarks>
		private void buttonStyle_Click(object sender, EventArgs e)
		{
			ICard card;
			if (dictionary.Cards.Cards.Count > 0)
			{
				card = dictionary.Cards.Cards[0];
			}
			else
			{
				if (dictionary.Chapters.Chapters.Count == 0)
					return;
				card = new MLifter.DAL.Preview.PreviewCard(null);
				card.Chapter = dictionary.Chapters.Chapters[0].Id;
				card.Answer.AddWord(card.Answer.CreateWord(Properties.Resources.EXAMPLE_CARD_ANSWER, WordType.Word, true));
				card.AnswerExample.AddWord(card.AnswerExample.CreateWord(Properties.Resources.EXAMPLE_CARD_ANSWER_EXAMPLE, WordType.Sentence, false));
				card.Question.AddWord(card.Question.CreateWord(Properties.Resources.EXAMPLE_CARD_QUESTION, WordType.Word, true));
				card.QuestionExample.AddWord(card.QuestionExample.CreateWord(Properties.Resources.EXAMPLE_CARD_QUESTION_EXAMPLE, WordType.Sentence, false));
			}

			MLifter.Controls.CardStyleEditor editor = new MLifter.Controls.CardStyleEditor();
			editor.LoadStyle(card, dictionary.Settings.Style, dictionary, dictionary.DictionaryDAL);

			switch (editor.ShowDialog())
			{
				case DialogResult.Abort:
					dictionary.Settings.Style = null;
					break;
				case DialogResult.Cancel:
					break;
				default:
					dictionary.UseDictionaryStyleSheets = true;
					break;
			}
			//cleanup
			if (card != null) card.Dispose();
		}

		#endregion

		#region Public Interface

		/// <summary>
		/// Initializes the dictionary form for the currently loaded dictionary
		/// </summary>
		/// <returns>True if OK</returns>
		/// <remarks>Documented by Dev03, 2007-07-19</remarks>
		public static bool LoadDictionary(Dictionary dict, string helpfile)
		{
			dictionary = dict;
			PropertiesForm propertiesForm = new PropertiesForm(helpfile);

			//learning modules cultures and captions
			propertiesForm.tabControlProperties.SelectedIndex = 0;
			propertiesForm.dictionaryCaptions.QuestionTitle = dictionary.DefaultSettings.QuestionCaption;
			propertiesForm.dictionaryCaptions.AnswerTitle = dictionary.DefaultSettings.AnswerCaption;
			propertiesForm.dictionaryCaptions.QuestionCulture = dictionary.DefaultSettings.QuestionCulture;
			propertiesForm.dictionaryCaptions.AnswerCulture = dictionary.DefaultSettings.AnswerCulture;

			propertiesForm.groupBoxQuestion.Text = dictionary.QuestionCaption;
			propertiesForm.groupBoxAnswer.Text = dictionary.AnswerCaption;

			//commentary sounds
			propertiesForm.commentarySounds.Clear();
			foreach (KeyValuePair<CommentarySoundIdentifier, IMedia> commentarySound in dictionary.DefaultSettings.CommentarySounds)
				propertiesForm.commentarySounds.Add(commentarySound.Key, commentarySound.Value);

			//Learning Module LearnModes
			propertiesForm.learnModes.MultipleDirections = true;
			propertiesForm.learnModes.QuestionCaption = dictionary.DefaultSettings.QuestionCaption;
			propertiesForm.learnModes.AnswerCaption = dictionary.DefaultSettings.AnswerCaption;
			propertiesForm.learnModes.SetQueryDirections(dictionary.AllowedQueryDirections);
			propertiesForm.learnModes.SetQueryTypes(dictionary.AllowedQueryTypes);

			//Learning Module Properties
			propertiesForm.dictionaryProperties.Title = dictionary.DictionaryTitle;
			propertiesForm.dictionaryProperties.DictionaryName = dictionary.DictionaryTitle;
			propertiesForm.dictionaryProperties.DictionaryAuthor = dictionary.Author;
			propertiesForm.dictionaryProperties.DictionaryCategory = dictionary.Category;
			propertiesForm.dictionaryProperties.DictionaryDescription = dictionary.Description;

			propertiesForm.dictionaryInfos.SetInfo(dictionary);

			//Load dictionary logos
			if (dictionary.Logo != null && dictionary.Logo is Stream)
			{
				Image logo = Bitmap.FromStream(dictionary.Logo);
				propertiesForm.dictionaryProperties.LeftImageSizeMode = propertiesForm.dictionaryInfos.LeftImageSizeMode = PictureBoxSizeMode.Zoom;
				propertiesForm.dictionaryProperties.LeftImage = propertiesForm.dictionaryInfos.LeftImage = logo;
			}

			propertiesForm.buttonCancel.Enabled = true;
			propertiesForm.listboxCommentaryAnswer.SelectedIndex = 0;

			//[ML-2317]  Properties / Add/edit styles crashes
			propertiesForm.buttonAddEditStyle.Visible = dictionary.CurrentLearnLogic != null;

			//[ML-1837] - disable the allowed learning options for protected content
			if (dictionary.DictionaryContentProtected)
			{
				propertiesForm.learnModes.EditableControlsEnabled = false;
			}

			//Enable/Disable controls depending on current permission-role:
			if (!dictionary.CanModify)
			{
				propertiesForm.dictionaryProperties.EditableControlsEnabled = false;
				propertiesForm.dictionaryCaptions.EditableControlsEnabled = false;
				propertiesForm.learnModes.EditableControlsEnabled = false;
				propertiesForm.buttonChange.Enabled = false;
				propertiesForm.buttonAddEditStyle.Enabled = false;
			}

			return propertiesForm.ShowDialog() == DialogResult.OK ? true : false;
		}

		/// <summary>
		/// Loads the dictionary.
		/// </summary>
		/// <param name="module">The module.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev08, 2009-03-27</remarks>
		public static bool LoadDictionary(LearningModulesIndexEntry module, string helpfile)
		{
			if (!ValidLearningModuleIndexEntry(module))
				return false;

			return LoadDictionary(new Dictionary(module.Dictionary, null), helpfile);
		}

		#endregion
	}
}

