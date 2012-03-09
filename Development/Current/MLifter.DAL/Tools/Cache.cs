//# define debug_output

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using MLifter.DAL.Interfaces;

namespace MLifter.DAL.Tools
{
	/// <summary>
	/// Class which implements a caching mechanism.
	/// </summary>
	/// <remarks>Documented by Dev03, 2008-11-27</remarks>
	public class Cache : Dictionary<ObjectIdentifier, object>
	{
		/// <summary>
		/// Default timespan for a cache value to expire.
		/// </summary>
		public static TimeSpan DefaultValidationTime = new TimeSpan(0, 1, 0);
		/// <summary>
		/// Default timespan for a settings cache to expire.
		/// </summary>
		public static TimeSpan DefaultSettingsValidationTime = new TimeSpan(0, 10, 0);
		/// <summary>
		/// Default timespan for a statistics cache to expire.
		/// </summary>
		public static TimeSpan DefaultStatisticValidationTime = new TimeSpan(23, 59, 59);

		private Thread cleanupThread;
		private System.Timers.Timer cleanupTimer = new System.Timers.Timer();

		private Dictionary<CacheObject, Dictionary<int, DateTime>> deathTimeList = new Dictionary<CacheObject, Dictionary<int, DateTime>>();

		/// <summary>
		/// Gets or sets a value indicating whether the caches auto clean up is enabled.
		/// </summary>
		/// <value><c>true</c> if auto clean up is enabled; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev05, 2008-08-01</remarks>
		public bool AutoCleanUp
		{
			get { return cleanupTimer.Enabled; }
			set { cleanupTimer.Enabled = value; }
		}
		/// <summary>
		/// Gets or sets the clean up interval in minutes.
		/// </summary>
		/// <value>The clean up interval.</value>
		/// <remarks>Documented by Dev05, 2008-08-01</remarks>
		public int CleanUpInterval
		{
			get { return Convert.ToInt32(cleanupTimer.Interval / 60000); }
			set
			{
				if (value > 0)
					cleanupTimer.Interval = value * 60000;
				else
					throw new ArgumentException("Interval must be greater than zero!");
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Cache"/> class.
		/// </summary>
		/// <param name="autoCleanUp">if set to <c>true</c> auto clean up is enabled.</param>
		/// <remarks>Documented by Dev05, 2008-08-01</remarks>		
		public Cache(bool autoCleanUp)
		{
			PrepareDeathtimeList();

			cleanupTimer.Elapsed += new System.Timers.ElapsedEventHandler(cleanupTimer_Elapsed);
			GC.KeepAlive(cleanupTimer);

			CleanUpInterval = 1;
			AutoCleanUp = autoCleanUp;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="Cache"/> class. Auto clean up will be enabled automatically!
		/// </summary>
		/// <param name="cleanUpInterval">The clean up interval in minutes.</param>
		/// <remarks>Documented by Dev05, 2008-08-01</remarks>
		public Cache(int cleanUpInterval)
		{
			PrepareDeathtimeList();

			cleanupTimer.Elapsed += new System.Timers.ElapsedEventHandler(cleanupTimer_Elapsed);
			GC.KeepAlive(cleanupTimer);

			CleanUpInterval = cleanUpInterval;
			AutoCleanUp = true;
		}
		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="Cache"/> is reclaimed by garbage collection.
		/// </summary>
		/// <remarks>Documented by Dev03, 2008-11-27</remarks>
		~Cache()
		{
			cleanupTimer.Enabled = false;

			if (cleanupThread != null && cleanupThread.IsAlive)
				cleanupThread.Abort();
		}

		private void PrepareDeathtimeList()
		{
			foreach (CacheObject cacheObject in Enum.GetValues(typeof(CacheObject)))
				deathTimeList.Add(cacheObject, new Dictionary<int, DateTime>());
		}

		/// <summary>
		/// Gets or sets the <see cref="System.Object"/> with the specified key.
		/// </summary>
		/// <value></value>
		/// <remarks>Documented by Dev03, 2008-11-27</remarks>
		public new object this[ObjectIdentifier key]
		{
			get
			{
				lock (deathTimeList)
				{
					if (!base.ContainsKey(key) || deathTimeList[key.Type][key.Id] <= DateTime.Now)
					{
						Uncache(key);
						return null;
					}
					else
					{
#if DEBUG && debug_output
						Debug.WriteLine("Returning from cache: " + key.Type.ToString() + " " + key.Id.ToString());
#endif
						return base[key];
					}
				}
			}
			set
			{
				lock (deathTimeList)
				{
					AddObjectToDeathTimeList(ObjectLifetimeIdentifier.Create(key.Type, key.Id));
					this[key] = value;
				}
			}
		}
		/// <summary>
		/// Sets the <see cref="System.Object"/> with the specified key.
		/// </summary>
		/// <value></value>
		/// <remarks>Documented by Dev03, 2008-11-27</remarks>
		public object this[ObjectLifetimeIdentifier key]
		{
			set
			{
				lock (deathTimeList)
				{
					AddObjectToDeathTimeList(key);
					base[key.Identifier] = value;
				}
			}
		}

		private void AddObjectToDeathTimeList(ObjectLifetimeIdentifier oli)
		{
			deathTimeList[oli.Identifier.Type][oli.Identifier.Id] = oli.DeathTime;
		}

		private string cleanupLocker = string.Empty;
		void cleanupTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			lock (cleanupLocker)
			{
				if (cleanupThread == null)
				{
					cleanupThread = new Thread(new ThreadStart(CleanUpLauncher));
					cleanupThread.Name = "Cache Cleanup Thread";
					cleanupThread.CurrentCulture = Thread.CurrentThread.CurrentCulture;
					cleanupThread.CurrentUICulture = Thread.CurrentThread.CurrentUICulture;
					cleanupThread.IsBackground = true;
					cleanupThread.Priority = ThreadPriority.Lowest;
					cleanupThread.Start();
				}
			}
		}
		private void CleanUpLauncher()
		{
			CleanUp();
			cleanupThread = null;
		}

		/// <summary>
		/// Cleans the cache from outdated items.
		/// </summary>
		/// <remarks>Documented by Dev05, 2008-08-04</remarks>
		public void CleanUp()
		{
			List<ObjectIdentifier> deathObjects = new List<ObjectIdentifier>();
			lock (deathTimeList)
			{
				int count = 0;
				int totalCount = 0;
				foreach (KeyValuePair<CacheObject, Dictionary<int, DateTime>> dic in deathTimeList)
				{
					totalCount += dic.Value.Count;
					foreach (KeyValuePair<int, DateTime> pair in dic.Value)
						if (pair.Value < DateTime.Now)
							deathObjects.Add(ObjectLifetimeIdentifier.GetIdentifier(dic.Key, pair.Key));
					count++;
#if DEBUG && debug_output
						Debug.WriteLine(string.Format("Cache cleanup scanned {0}/{1}. Totaly listed: {2}", count, deathTimeList.Count, totalCount));
#endif

					Monitor.Wait(deathTimeList, 10);
				}
			}

			deathObjects.ForEach(oi => Uncache(oi));
			GC.Collect(); //request garbage collection
#if DEBUG && debug_output
				Debug.WriteLine("Cache cleanup finished.");
#endif
		}

		/// <summary>
		/// Begins to uncache the given item asyncron.
		/// </summary>
		/// <param name="identifier">The identifier.</param>
		/// <remarks>Documented by Dev05, 2008-08-04</remarks>
		public void BeginUncache(ObjectIdentifier identifier)
		{
			Thread uncache = new Thread(new ParameterizedThreadStart(BeginUncache));
			uncache.CurrentCulture = Thread.CurrentThread.CurrentCulture;
			uncache.CurrentUICulture = Thread.CurrentThread.CurrentUICulture;
			uncache.Start(identifier);
		}
		private void BeginUncache(object identifier)
		{
			Uncache((ObjectIdentifier)identifier);
		}
		/// <summary>
		/// Uncaches the specified item.
		/// </summary>
		/// <param name="identifier">The identifier.</param>
		/// <remarks>Documented by Dev05, 2008-08-04</remarks>
		public void Uncache(ObjectIdentifier identifier)
		{
			lock (deathTimeList)
			{
				if (!base.ContainsKey(identifier))
					return;
#if DEBUG && debug_output
				Debug.WriteLine("Removing from cache: " + identifier.Type.ToString() + " " + identifier.Id.ToString());
#endif
				deathTimeList[identifier.Type].Remove(identifier.Id);
				base.Remove(identifier);
			}
		}

		/// <summary>
		/// Removes all items from the cache.
		/// </summary>
		/// <remarks>Documented by Dev05, 2008-09-15</remarks>
		public new void Clear()
		{
			lock (deathTimeList)
			{
				base.Clear();

				foreach (KeyValuePair<CacheObject, Dictionary<int, DateTime>> dic in deathTimeList)
				{
					dic.Value.Clear();
				}
			}
		}
	}

	/// <summary>
	/// Identifier of an object.
	/// </summary>
	/// <remarks>Documented by Dev03, 2008-11-27</remarks>
	public struct ObjectIdentifier
	{
		/// <summary>
		/// The id of the object.
		/// </summary>
		public int Id;
		/// <summary>
		/// The object type.
		/// </summary>
		public CacheObject Type;
	}

	/// <summary>
	/// Lifetime identifier of an object.
	/// </summary>
	/// <remarks>Documented by Dev03, 2008-11-27</remarks>
	public class ObjectLifetimeIdentifier
	{
		/// <summary>
		/// The identifier of the object.
		/// </summary>
		public ObjectIdentifier Identifier;
		/// <summary>
		/// The time were the object is removed from cache.
		/// </summary>
		public DateTime DeathTime;

		private ObjectLifetimeIdentifier(CacheObject type, int id)
		{
			Identifier = new ObjectIdentifier();
			Identifier.Id = id;
			Identifier.Type = type;

			DeathTime = DateTime.Now.Add(Cache.DefaultValidationTime);
		}
		/// <summary>
		/// Creates the specified type.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-02-09</remarks>
		public static ObjectLifetimeIdentifier Create(CacheObject type, int id)
		{
			return new ObjectLifetimeIdentifier(type, id);
		}
		private ObjectLifetimeIdentifier(CacheObject type, int id, DateTime deathTime)
		{
			Identifier = new ObjectIdentifier();
			Identifier.Id = id;
			Identifier.Type = type;

			DeathTime = deathTime;
		}
		/// <summary>
		/// Creates the specified type.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="id">The id.</param>
		/// <param name="deathTime">The death time.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-02-09</remarks>
		public static ObjectLifetimeIdentifier Create(CacheObject type, int id, DateTime deathTime)
		{
			return new ObjectLifetimeIdentifier(type, id, deathTime);
		}
		private ObjectLifetimeIdentifier(CacheObject type, int id, TimeSpan lifetime)
		{
			Identifier = new ObjectIdentifier();
			Identifier.Id = id;
			Identifier.Type = type;

			DeathTime = DateTime.Now + lifetime;
		}
		/// <summary>
		/// Creates the specified type.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="id">The id.</param>
		/// <param name="lifetime">The lifetime.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-02-09</remarks>
		public static ObjectLifetimeIdentifier Create(CacheObject type, int id, TimeSpan lifetime)
		{
			return new ObjectLifetimeIdentifier(type, id, lifetime);
		}
		/// <summary>
		/// Gets the identifier.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-02-09</remarks>
		public static ObjectIdentifier GetIdentifier(CacheObject type, int id)
		{
			ObjectIdentifier idf = new ObjectIdentifier();
			idf.Id = id;
			idf.Type = type;
			return idf;
		}

		/// <summary>
		/// Gets the cache object.
		/// </summary>
		/// <param name="side">The side.</param>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-02-09</remarks>
		public static CacheObject GetCacheObject(Side side, WordType type)
		{
			switch (side)
			{
				case Side.Question:
					switch (type)
					{
						case WordType.Word:
							return CacheObject.QuestionWords;
						case WordType.Sentence:
							return CacheObject.QuestionExampleWords;
						case WordType.Distractor:
							return CacheObject.QuestionDistractorWords;
					}
					break;
				case Side.Answer:
					switch (type)
					{
						case WordType.Word:
							return CacheObject.AnswerWords;
						case WordType.Sentence:
							return CacheObject.AnswerExampleWords;
						case WordType.Distractor:
							return CacheObject.AnswerDistractorWords;
					}
					break;
			}

			return CacheObject.Word;
		}
	}

	/// <summary>
	/// Definition of the CacheObject
	/// </summary>
	/// <remarks>Documented by Dev08, 2008-10-15</remarks>
	public enum CacheObject
	{
		/// <summary>
		/// ChaptersList
		/// </summary>
		ChaptersList,
		/// <summary>
		/// ChapterTitle
		/// </summary>
		ChapterTitle,
		/// <summary>
		/// ChapterDescription
		/// </summary>
		ChapterDescription,
		/// <summary>
		/// Setting of the chapter
		/// </summary>
		ChapterSetting,
		/// <summary>
		/// Word
		/// </summary>
		Word,
		/// <summary>
		/// QuestionWords
		/// </summary>
		QuestionWords,
		/// <summary>
		/// QuestionExampleWords
		/// </summary>
		QuestionExampleWords,
		/// <summary>
		/// QuestionDistractorWords
		/// </summary>
		QuestionDistractorWords,
		/// <summary>
		/// AnswerWords
		/// </summary>
		AnswerWords,
		/// <summary>
		/// AnswerExampleWords
		/// </summary>
		AnswerExampleWords,
		/// <summary>
		/// AnswerDistractorWords
		/// </summary>
		AnswerDistractorWords,
		/// <summary>
		/// CardChapterList
		/// </summary>
		CardChapterList,
		/// <summary>
		/// CardsList
		/// </summary>
		CardsList,
		/// <summary>
		/// Setting of the card
		/// </summary>
		CardSetting,
		/// <summary>
		/// CardIdsList
		/// </summary>
		CardIdsList,
		/// <summary>
		/// LearningModuleTitle
		/// </summary>
		LearningModuleTitle,
		/// <summary>
		/// LearningModuleAuthor
		/// </summary>
		LearningModuleAuthor,
		/// <summary>
		/// DefaultLearningModuleSettings
		/// </summary>
		DefaultLearningModuleSettings,
		/// <summary>
		/// AllowedLearningModuleSettings
		/// </summary>
		AllowedLearningModuleSettings,
		/// <summary>
		/// UserLearningModuleSettings
		/// </summary>
		UserLearningModuleSettings,
		/// <summary>
		/// Database version
		/// </summary>
		DataBaseVersion,
		/// <summary>
		/// Supported data layer versions
		/// </summary>
		SupportedDataLayerVersions,
		/// <summary>
		/// Database supports list authentication
		/// </summary>
		ListAuthentication,
		/// <summary>
		/// Database supports forms authentication
		/// </summary>
		FormsAuthentication,
		/// <summary>
		/// Database supports local directory authentication
		/// </summary>
		LocalDirectoryAuthentication,
		/// <summary>
		/// Local directory type authentication
		/// </summary>
		LocalDirectoryType,
		/// <summary>
		/// Local directory server
		/// </summary>
		LdapServer,
		/// <summary>
		/// Local directory server port
		/// </summary>
		LdapPort,
		/// <summary>
		/// Local directory username
		/// </summary>
		LdapUser,
		/// <summary>
		/// Local directory user password
		/// </summary>
		LdapPassword,
		/// <summary>
		/// Local directory context
		/// </summary>
		LdapContext,
		/// <summary>
		/// Use SSL for local directory connection
		/// </summary>
		LdapUseSsl,
		/// <summary>
		/// UserList
		/// </summary>
		UserList,
		/// <summary>
		/// CardState
		/// </summary>
		CardState,
		/// <summary>
		/// CurrentBoxSizes
		/// </summary>
		CurrentBoxSizes,
		/// <summary>
		/// BoxSizes
		/// </summary>
		BoxSizes,
		/// <summary>
		/// MaximalBoxSizes
		/// </summary>
		MaximalBoxSizes,
		/// <summary>
		/// DefaultBoxSizes
		/// </summary>
		DefaultBoxSizes,
		/// <summary>
		/// CardMedia
		/// </summary>
		CardMedia,
		/// <summary>
		/// MediaProperties
		/// </summary>
		MediaProperties,
		/// <summary>
		/// SettingsAutoPlayAudio
		/// </summary>
		SettingsAutoPlayAudio,
		/// <summary>
		/// SettingsCaseSensetive
		/// </summary>
		SettingsCaseSensetive,
		/// <summary>
		/// SettingsConfirmDemote
		/// </summary>
		SettingsConfirmDemote,
		/// <summary>
		/// SettingsEnableCommentary
		/// </summary>
		SettingsEnableCommentary,
		/// <summary>
		/// SettingsCorrectOnTheFly
		/// </summary>
		SettingsCorrectOnTheFly,
		/// <summary>
		/// SettingsEnableTimer
		/// </summary>
		SettingsEnableTimer,
		/// <summary>
		/// SettingsSynonymGradingsFk
		/// </summary>
		SettingsSynonymGradingsFk,
		/// <summary>
		/// SettingsTypeGradingsFk
		/// </summary>
		SettingsTypeGradingsFk,
		/// <summary>
		/// SettingsMultipleChoiceOptionsFk
		/// </summary>
		SettingsMultipleChoiceOptionsFk,
		/// <summary>
		/// SettingsQueryDirectionsFk
		/// </summary>
		SettingsQueryDirectionsFk,
		/// <summary>
		/// SettingsQueryTypesFk
		/// </summary>
		SettingsQueryTypesFk,
		/// <summary>
		/// SettingsRandomPool
		/// </summary>
		SettingsRandomPool,
		/// <summary>
		/// SettingsSelfAssessment
		/// </summary>
		SettingsSelfAssessment,
		/// <summary>
		/// SettingsShowImages
		/// </summary>
		SettingsShowImages,
		/// <summary>
		/// SettingsStripchars
		/// </summary>
		SettingsStripchars,
		/// <summary>
		/// SettingsQuestionCulture
		/// </summary>
		SettingsQuestionCulture,
		/// <summary>
		/// SettingsAnswerCulture
		/// </summary>
		SettingsAnswerCulture,
		/// <summary>
		/// SettingsQuestionCaption
		/// </summary>
		SettingsQuestionCaption,
		/// <summary>
		/// SettingsAnswerCaption
		/// </summary>
		SettingsAnswerCaption,
		/// <summary>
		/// SettingsLogoFk
		/// </summary>
		SettingsLogoFk,
		/// <summary>
		/// SettingsQuestionStylesheetFk
		/// </summary>
		SettingsQuestionStylesheetFk,
		/// <summary>
		/// SettingsAnswerStylesheetFk
		/// </summary>
		SettingsAnswerStylesheetFk,
		/// <summary>
		/// SettingsAutoBoxsize
		/// </summary>
		SettingsAutoBoxsize,
		/// <summary>
		/// SettingsPoolEmptyMessageShown
		/// </summary>
		SettingsPoolEmptyMessageShown,
		/// <summary>
		/// 
		/// </summary>
		SettingsShowStatistics,
		/// <summary>
		/// 
		/// </summary>
		SettingsSkipCorrectAnswers,
		/// <summary>
		/// 
		/// </summary>
		SettingsSnoozeOptionsFk,
		/// <summary>
		/// 
		/// </summary>
		SettingsUseLearningModuleStylesheet,
		/// <summary>
		/// 
		/// </summary>
		SettingsCardStyleFk,
		/// <summary>
		/// 
		/// </summary>
		SettingsBoxesFk,
		/// <summary>
		/// 
		/// </summary>
		SettingsIsCached,

		/// <summary>
		/// 
		/// </summary>
		SettingsSnoozeOptionsId,
		/// <summary>
		/// 
		/// </summary>
		SettingsSnoozeCardsEnabled,
		/// <summary>
		/// 
		/// </summary>
		SettingsSnoozeRightsEnabled,
		/// <summary>
		/// 
		/// </summary>
		SettingsSnoozeTimeEnabled,
		/// <summary>
		/// 
		/// </summary>
		SettingsSnoozeCards,
		/// <summary>
		/// 
		/// </summary>
		SettingsSnoozeHigh,
		/// <summary>
		/// 
		/// </summary>
		SettingsSnoozeLow,
		/// <summary>
		/// 
		/// </summary>
		SettingsSnoozeMode,
		/// <summary>
		/// 
		/// </summary>
		SettingsSnoozeRights,
		/// <summary>
		/// 
		/// </summary>
		SettingsSnoozeTime,

		/// <summary>
		/// 
		/// </summary>
		SettingsBoxSizeId,
		/// <summary>
		/// 
		/// </summary>
		SettingsBox1Size,
		/// <summary>
		/// 
		/// </summary>
		SettingsBox2Size,
		/// <summary>
		/// 
		/// </summary>
		SettingsBox3Size,
		/// <summary>
		/// 
		/// </summary>
		SettingsBox4Size,
		/// <summary>
		/// 
		/// </summary>
		SettingsBox5Size,
		/// <summary>
		/// 
		/// </summary>
		SettingsBox6Size,
		/// <summary>
		/// 
		/// </summary>
		SettingsBox7Size,
		/// <summary>
		/// 
		/// </summary>
		SettingsBox8Size,
		/// <summary>
		/// 
		/// </summary>
		SettingsBox9Size,

		/// <summary>
		/// 
		/// </summary>
		SettingsStyleSheetsId,
		/// <summary>
		/// 
		/// </summary>
		SettingsStyleSheetsQuestionValue,
		/// <summary>
		/// 
		/// </summary>
		SettingsStyleSheetsAnswerValue,
		/// <summary>
		/// 
		/// </summary>
		SettingsCardStyleValue,

		/// <summary>
		/// 
		/// </summary>
		SettingsTypeGradingsId,
		/// <summary>
		/// 
		/// </summary>
		SettingsTypeGradingsAllCorrect,
		/// <summary>
		/// 
		/// </summary>
		SettingsTypeGradingsHalfCorrect,
		/// <summary>
		/// 
		/// </summary>
		SettingsTypeGradingsNoneCorrect,
		/// <summary>
		/// 
		/// </summary>
		SettingsTypeGradingsPrompt,

		/// <summary>
		/// 
		/// </summary>
		SettingsMultipleChoiceOptionsId,
		/// <summary>
		/// 
		/// </summary>
		SettingsMultipleChoiceOptionsAllowMultipleCorrectAnswers,
		/// <summary>
		/// 
		/// </summary>
		SettingsMultipleChoiceOptionsAllowRandomDistractors,
		/// <summary>
		/// 
		/// </summary>
		SettingsMultipleChoiceOptionsMaxCorrectAnswers,
		/// <summary>
		/// 
		/// </summary>
		SettingsMultipleChoiceOptionsNumberOfChoices,

		/// <summary>
		/// 
		/// </summary>
		SettingsQueryTypesId,
		/// <summary>
		/// 
		/// </summary>
		SettingsQueryTypesImageRecognition,
		/// <summary>
		/// 
		/// </summary>
		SettingsQueryTypesListeningComprehension,
		/// <summary>
		/// 
		/// </summary>
		SettingsQueryTypesMultipleChoice,
		/// <summary>
		/// 
		/// </summary>
		SettingsQueryTypesSentence,
		/// <summary>
		/// 
		/// </summary>
		SettingsQueryTypesWord,

		/// <summary>
		/// 
		/// </summary>
		SettingsSynonymGradingsId,
		/// <summary>
		/// 
		/// </summary>
		SettingsSynonymGradingsAllKnown,
		/// <summary>
		/// 
		/// </summary>
		SettingsSynonymGradingsHalfKnown,
		/// <summary>
		/// 
		/// </summary>
		SettingsSynonymGradingsOneKnown,
		/// <summary>
		/// 
		/// </summary>
		SettingsSynonymGradingsFirstKnown,
		/// <summary>
		/// 
		/// </summary>
		SettingsSynonymGradingsPrompt,

		/// <summary>
		/// 
		/// </summary>
		SettingsQueryDirectionsId,
		/// <summary>
		/// 
		/// </summary>
		SettingsQueryDirectionsQuestion2Answer,
		/// <summary>
		/// 
		/// </summary>
		SettingsQueryDirectionsAnswer2Question,
		/// <summary>
		/// 
		/// </summary>
		SettingsQueryDirectionsMixed,
		/// <summary>
		/// contains a Dictionary(key,value) full of Sounds
		/// </summary>
		SettingsCommentarySounds,
		/// <summary>
		/// 
		/// </summary>
		SettingsSelectedLearnChapterList,

		/// <summary>
		/// 
		/// </summary>
		StatisticWrongCards,
		/// <summary>
		/// 
		/// </summary>
		StatisticCorrectCards,
		/// <summary>
		/// 
		/// </summary>
		StatisticContentOfBoxes,
		/// <summary>
		/// 
		/// </summary>
		StatisticStartTime,
		/// <summary>
		/// 
		/// </summary>
		StatisticEndTime,
		/// <summary>
		/// 
		/// </summary>
		StatisticsLearnSessions,
		/// <summary>
		/// 
		/// </summary>
		Score,
		/// <summary>
		/// SessionAlive
		/// </summary>
		SessionAlive,
		/// <summary>
		/// CardCacheInitialized
		/// </summary>
		CardCacheInitialized
	}

	/// <summary>
	/// A simple conversation function. (a nullable extension of Convert.ToSomething() )
	/// </summary>
	/// <remarks>Documented by Dev08, 2008-10-24</remarks>
	public class DbValueConverter
	{
		/// <summary>
		/// Converts the specified value.
		/// </summary>
		/// <typeparam name="T">The type to convert to.</typeparam>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2008-10-27</remarks>
		public static T? Convert<T>(object value) where T : struct
		{
			if (typeof(Enum).IsAssignableFrom(typeof(T)))
				return (value == null || value is DBNull) ? (T?)null : (T?)Enum.Parse(typeof(T), value.ToString());
			else
				return (value == null || value is DBNull) ? (T?)null : (T?)System.Convert.ChangeType(value, typeof(T));
		}

		/// <summary>
		/// Converts the specified value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2008-10-27</remarks>
		public static string Convert(object value)
		{
			return (value == null || value is DBNull) ? string.Empty : value.ToString();
		}
	}
}
