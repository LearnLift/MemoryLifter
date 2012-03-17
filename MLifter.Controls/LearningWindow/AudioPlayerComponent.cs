using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using MLifter.BusinessLayer;
using MLifter.DAL.Interfaces;

namespace MLifter.Controls.LearningWindow
{
	public partial class AudioPlayerComponent : Component, ILearnUserControl
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AudioPlayerComponent"/> class.
		/// </summary>
		/// <remarks>Documented by Dev02, 2008-04-24</remarks>
		public AudioPlayerComponent()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AudioPlayerComponent"/> class.
		/// </summary>
		/// <param name="container">The container.</param>
		/// <remarks>Documented by Dev02, 2008-04-24</remarks>
		public AudioPlayerComponent(IContainer container)
		{
			container.Add(this);

			InitializeComponent();
		}

		LearnLogic learnlogic = null;

		/// <summary>
		/// Registers the learn logic.
		/// </summary>
		/// <param name="learnlogic">The learnlogic.</param>
		/// <remarks>Documented by Dev02, 2008-04-22</remarks>
		public void RegisterLearnLogic(LearnLogic learnlogic)
		{
			this.learnlogic = learnlogic;
			this.learnlogic.CardStateChanged += new LearnLogic.CardStateChangedEventHandler(learnlogic_CardStateChanged);
		}

		/// <summary>
		/// Handles the CardStateChanged event of the learnlogic control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="MLifter.BusinessLayer.CardStateChangedEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-04-24</remarks>
		void learnlogic_CardStateChanged(object sender, CardStateChangedEventArgs e)
		{
			if (e.dictionary.Settings.AutoplayAudio.Value)
			{
				bool example = (e as CardStateChangedEventArgs).dictionary.LearnMode == MLifter.BusinessLayer.LearnModes.Sentence;

				if (e is CardStateChangedNewCardEventArgs ||
					(e is CardStateChangedShowResultEventArgs && ((CardStateChangedShowResultEventArgs)e).slideshow))
				{
					if (e is CardStateChangedShowResultEventArgs && ((CardStateChangedShowResultEventArgs)e).preview)
						return;

					// [ML-1747]
					if (e.dictionary.LearnMode == MLifter.BusinessLayer.LearnModes.ImageRecognition)
						return;

					//play question side sound
					PlayMediaFile(e.dictionary.Cards.GetAudioFile(e.cardid, Side.Question, example), true);
				}

				if (e is CardStateChangedShowResultEventArgs)
				{
					CardStateChangedShowResultEventArgs args = (CardStateChangedShowResultEventArgs)e;
					if (args.preview)
						return;

					//play answer side sound and commentary sounds
					AnswerResult result = args.promoted ? AnswerResult.Correct : (args.result == AnswerResult.Almost ? AnswerResult.Almost : AnswerResult.Wrong);

					if (FileValid(args.dictionary.Cards.GetAudioFile(args.cardid, Side.Answer, example)))
					{
						if (result != AnswerResult.Correct) PlayCommentarySound(args.dictionary, result, false); //not correct commentary sound is to be played before the cardaudio
						PlayMediaFile(args.dictionary.Cards.GetAudioFile(args.cardid, Side.Answer, example), false);
						if (result == AnswerResult.Correct) PlayCommentarySound(args.dictionary, result, false); //correct commentary sound is to be played after the cardaudio
					}
					else //play the standalone commentary sounds only if the card sound is not available
					{
						PlayCommentarySound(args.dictionary, result, true);
					}
				}
			}
		}

		/// <summary>
		/// Plays the Media file.
		/// </summary>
		/// <param name="filepath">The filepath.</param>
		/// <param name="clearQueue">if set to <c>true</c> [clear queue].</param>
		/// <remarks>Documented by Dev02, 2008-04-24</remarks>
		void PlayMediaFile(string filepath, bool clearQueue)
		{
			if (learnlogic != null)
				learnlogic.PlayAudioFile(filepath, clearQueue);
		}

		/// <summary>
		/// Determines wheter the audio file can be played.
		/// </summary>
		/// <param name="filepath">The filepath.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev02, 2008-04-30</remarks>
		private bool FileValid(string filepath)
		{
			return (!string.IsNullOrEmpty(filepath));
		}

		/// <summary>
		/// Plays the commentary sound.
		/// </summary>
		/// <param name="soundid">The commentary sound id: 0 -> correct, 1 -> wrong, 2 -> almost.</param>
		/// <param name="standalone">if set to <c>true</c>, the [standalone commentary sound gets played].</param>
		/// <remarks>Documented by Dev02, 2008-03-17</remarks>
		private void PlayCommentarySound(Dictionary dictionary, AnswerResult result, bool standalone)
		{
			if (dictionary.Settings.EnableCommentary.Value)
			{
				Side side = dictionary.CurrentQueryDirection == EQueryDirection.Question2Answer ? Side.Answer : Side.Question;
				ECommentarySoundType type;
				switch (result)
				{
					case AnswerResult.Correct:
						type = standalone ? ECommentarySoundType.RightStandAlone : ECommentarySoundType.Right;
						break;
					case AnswerResult.Wrong:
						type = standalone ? ECommentarySoundType.WrongStandAlone : ECommentarySoundType.Wrong;
						break;
					case AnswerResult.Almost:
						type = standalone ? ECommentarySoundType.AlmostStandAlone : ECommentarySoundType.Almost;
						break;
					default:
						return;
				}
				CommentarySoundIdentifier identifier = CommentarySoundIdentifier.Create(side, type);

				if (dictionary.CommentarySound.ContainsKey(identifier))
					PlayMediaFile(dictionary.CommentarySound[identifier].Filename, result != AnswerResult.Correct);
			}
		}
	}
}
