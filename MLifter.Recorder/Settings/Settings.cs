using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace MLifter.AudioTools
{
	/// <summary>
	/// This class holds the informations about the actual recording session.
	/// </summary>
	/// <remarks>Documented by Dev05, 2007-08-03</remarks>
	[Serializable]
	public class Settings
	{
		/// <summary>
		/// The actual opend dictionary.
		/// </summary>
		public string Dictionary = string.Empty;
		/// <summary>
		/// The actual selected card.
		/// </summary>
		public int ActualCard = 0;
		/// <summary>
		/// The selected font size.
		/// </summary>
		public FontSizes FontSize = FontSizes.Medium;

		/// <summary>
		/// The current side.
		/// </summary>
		public MLifter.DAL.Interfaces.Side Side = MLifter.DAL.Interfaces.Side.Question;
		/// <summary>
		/// If true, currently the sentence is recording, otherwise the word.
		/// </summary>
		public bool Sentence = false;
		/// <summary>
		/// The recording order.
		/// </summary>
		public MediaItemType[] RecordingOrder = 
			{ 
				MediaItemType.Question, 
				MediaItemType.QuestionExample,
				MediaItemType.Answer,
				MediaItemType.AnswerExample
			};
		/// <summary>
		/// The actual step in the recording order.
		/// </summary>
		public int ActualStep = 0;

		public bool RecordQuestion = true;
		public bool RecordQuestionExample = true;
		public bool RecordAnswer = false;
		public bool RecordAnswerExample = false;

		/// <summary>
		/// The Delay befor starting the recording.
		/// </summary>
		public int StartDelay = CONSTANTS.START_RECORDING_DELAY;
		/// <summary>
		/// The delay to cut at the end of a recording.
		/// </summary>
		public int StopDelay = CONSTANTS.STOP_RECORDING_DELAY;
		/// <summary>
		/// If true, the delays are active.
		/// </summary>
		public bool DelaysActive = CONSTANTS.DELAYS_ACTIVE;

		/// <summary>
		/// The codec settings.
		/// </summary>
		public string CodecSettings;

		/// <summary>
		/// The selected encoder.
		/// </summary>
		public string SelectedEncoder = string.Empty;

		/// <summary>
		/// If true, the encoder window is visible.
		/// </summary>
		public bool ShowEncoderWindow = false;

		/// <summary>
		/// If true, the encoder window will be minimized.
		/// </summary>
		public bool MinimizeEncoderWindow = true;

		/// <summary>
		/// The sampling rate the source provides.
		/// </summary>
		public int SamplingRate = 44100;
		public int Channels = 1;

		/// <summary>
		/// Select the advanced view.
		/// </summary>
		public bool AdvancedView = STANDARD_APPEARANCE.ADVANCEDVIEW;
		/// <summary>
		/// Select the keyboard layout.
		/// </summary>
		public bool KeyboardLayout = STANDARD_APPEARANCE.KEYBOARDLAYOUT;
		/// <summary>
		/// Ask for the Layout at startup?
		/// </summary>
		public bool AskLayoutAtStartup = true;
		/// <summary>
		/// true if the Presenter is activated.
		/// </summary>
		public bool PresenterActivated = CONSTANTS.PRESENTER_ACTIVATED;
		
		/// <summary>
		/// If ture, a function could be assignet to more then one key.
		/// </summary>
		public bool AllowMultipleAssignment = CONSTANTS.ALLOW_MULTIPLE_ASSIGNMENT;
		/// <summary>
		/// The assignet Key-Functions.
		/// </summary>
		public SerializableDictionary<Keys, Function> KeyFunctions = new SerializableDictionary<Keys,Function>();
		/// <summary>
		/// The assignet keayoard functions.
		/// </summary>
		public SerializableDictionary<Keys, Function> KeyboardFunctions = new SerializableDictionary<Keys, Function>();
		/// <summary>
		/// The asignet presenter functions.
		/// </summary>
		public SerializableDictionary<Keys, Function> PresenterKeyFunctions = new SerializableDictionary<Keys, Function>();

		/// <summary>
		/// The time to sleep before changing the NumLock state.
		/// </summary>
		public int NumLockSwitchSleepTime = 500;

		/// <summary>
		/// Initializes a new instance of the <see cref="Settings"/> class.
		/// </summary>
		/// <remarks>Documented by Dev05, 2007-08-03</remarks>
		public Settings() 
		{
			CodecSettings = (new Codecs.Codecs()).XMLString;
		}

		/// <summary>
		/// Saves the class to the specified filename.
		/// </summary>
		/// <param name="filename">The filename.</param>
		/// <remarks>Documented by Dev05, 2007-08-03</remarks>
		public void Save(string filename)
		{
			string dir = Path.GetDirectoryName(filename);
			if (!Directory.Exists(dir))
				Directory.CreateDirectory(dir);
			Stream outputStream = File.Create(filename);
			XmlSerializer serializer = new XmlSerializer(typeof(Settings));
			serializer.Serialize(outputStream, this);
			outputStream.Close();
		}

		/// <summary>
		/// Loads the class from the specified filname.
		/// </summary>
		/// <param name="filname">The filname.</param>
		/// <remarks>Documented by Dev05, 2007-08-03</remarks>
		public void Load(string filname)
		{
			Stream inputStream = File.OpenRead(filname);
			XmlSerializer serializer = new XmlSerializer(typeof(Settings));
			Settings tmpSettings;
			try
			{
				tmpSettings = (Settings)serializer.Deserialize(inputStream);

				ActualCard = tmpSettings.ActualCard;
				Dictionary = tmpSettings.Dictionary;
				FontSize = tmpSettings.FontSize;

				Side = tmpSettings.Side;
				Sentence = tmpSettings.Sentence;
				RecordingOrder = tmpSettings.RecordingOrder;
				ActualStep = tmpSettings.ActualStep;

				RecordQuestion = tmpSettings.RecordQuestion;
				RecordQuestionExample = tmpSettings.RecordQuestionExample;
				RecordAnswer = tmpSettings.RecordAnswer;
				RecordAnswerExample = tmpSettings.RecordAnswerExample;

				StartDelay = tmpSettings.StartDelay;
				StopDelay = tmpSettings.StopDelay;
				DelaysActive = tmpSettings.DelaysActive;

				CodecSettings = tmpSettings.CodecSettings;
				SelectedEncoder = tmpSettings.SelectedEncoder;
				SamplingRate = tmpSettings.SamplingRate;
				Channels = tmpSettings.Channels;

				AdvancedView = tmpSettings.AdvancedView;
				KeyboardLayout = tmpSettings.KeyboardLayout;
				AskLayoutAtStartup = tmpSettings.AskLayoutAtStartup;
				PresenterActivated = tmpSettings.PresenterActivated;

				AllowMultipleAssignment = tmpSettings.AllowMultipleAssignment;

				NumLockSwitchSleepTime = tmpSettings.NumLockSwitchSleepTime;

				foreach (KeyValuePair<Keys, Function> keyValuePair in tmpSettings.KeyFunctions)
					KeyFunctions[keyValuePair.Key] = keyValuePair.Value;
				foreach (KeyValuePair<Keys, Function> keyValuePair in tmpSettings.KeyboardFunctions)
					KeyboardFunctions[keyValuePair.Key] = keyValuePair.Value;

				if (tmpSettings.PresenterKeyFunctions.Count > 0)
				{
					PresenterKeyFunctions.Clear();
					foreach (KeyValuePair<Keys, Function> keyValuePair in tmpSettings.PresenterKeyFunctions)
						PresenterKeyFunctions.Add(keyValuePair.Key, keyValuePair.Value);
				}
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message);
			}
			finally
			{
				inputStream.Close();
			}
		}
	}
}
