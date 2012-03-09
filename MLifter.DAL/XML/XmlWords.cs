using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;
using MLifter.DAL;
using MLifter.DAL.Interfaces;
using System.ComponentModel;
using MLifter.DAL.Tools;

namespace MLifter.DAL.XML
{
	internal abstract class XmlWords : IWords
	{
		private XmlDictionary m_oDictionary;
		private XmlElement m_card;
		protected CultureInfo m_Culture = null;
		private List<IWord> m_words = new List<IWord>();
		protected string m_basePath = null;

		public XmlWords(ParentClass parent)
		{
			this.parent = parent;
		}

		#region IWords Members

		/// <summary>
		/// Gets or sets the culture for the words.
		/// </summary>
		/// <value>The culture.</value>
		/// <remarks>Documented by Dev03, 2007-11-30</remarks>
		public CultureInfo Culture
		{
			get
			{
				return m_Culture;
			}
		}

		/// <summary>
		/// Gets or sets the items.
		/// </summary>
		/// <value>The items.</value>
		/// <remarks>Documented by Dev03, 2007-08-27</remarks>
		public IList<IWord> Words
		{
			get
			{
				BindingList<IWord> bindingListWords = new BindingList<IWord>(m_words);
				bindingListWords.ListChanged += new ListChangedEventHandler(bindingListWords_ListChanged);
				return bindingListWords;
			}
		}

		/// <summary>
		/// Handles the PropertyChanged event of the word control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-06-05</remarks>
		void word_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (sender is IWord && this.Words.Contains((IWord)sender))
				FlushToDOM();
		}

		/// <summary>
		/// Handles the ListChanged event of the bindingListWords control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.ComponentModel.ListChangedEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-06-05</remarks>
		void bindingListWords_ListChanged(object sender, ListChangedEventArgs e)
		{
			if (e.ListChangedType == ListChangedType.ItemAdded)
				AssignWordPropertyChanged(Words[e.NewIndex]);
			FlushToDOM();
		}

		/// <summary>
		/// Creates the word.
		/// </summary>
		/// <param name="word">The word.</param>
		/// <param name="type">The type.</param>
		/// <param name="isDefault">if set to <c>true</c> [is default].</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev03, 2007-08-27</remarks>
		public IWord CreateWord(string word, WordType type, bool isDefault)
		{
			return new XmlWord(word, type, isDefault, Parent.GetChildParentClass(this));
		}

		/// <summary>
		/// Adds the specified word.
		/// </summary>
		/// <param name="word">The word.</param>
		/// <remarks>Documented by Dev03, 2007-08-27</remarks>
		public void AddWord(IWord word)
		{
			if (word == null)
			{
				throw new System.NullReferenceException(Properties.Resources.EXCEPTION_WORD_IS_NULL);
			}
			AssignWordPropertyChanged(word);
			this.Words.Add(word);
		}

		private void AssignWordPropertyChanged(IWord word)
		{
			//add property changed events
			if (word is INotifyPropertyChanged)
			{
				((INotifyPropertyChanged)word).PropertyChanged -= new PropertyChangedEventHandler(word_PropertyChanged);
				((INotifyPropertyChanged)word).PropertyChanged += new PropertyChangedEventHandler(word_PropertyChanged);
			}
		}

		/// <summary>
		/// Adds multiple words. The first word will be marked as default.
		/// </summary>
		/// <param name="words">The words.</param>
		/// <remarks>Documented by Dev03, 2007-10-02</remarks>
		[Obsolete("Use the the generic parameter overload!")]
		public void AddWords(string[] words)
		{
			if (words == null)
			{
				throw new System.NullReferenceException(Properties.Resources.EXCEPTION_WORD_LIST_IS_NULL);
			}
			bool isDefault = true;
			foreach (string wordString in words)
			{
				IWord word = CreateWord(wordString, WordType.Sentence, isDefault);
				AddWord(word);
				isDefault = false;
			}
		}

		/// <summary>
		/// Adds multiple words.
		/// </summary>
		/// <param name="words">The words.</param>
		/// <remarks>Documented by Dev03, 2008-01-08</remarks>
		public void AddWords(List<IWord> words)
		{
			if (words == null)
			{
				throw new System.NullReferenceException(Properties.Resources.EXCEPTION_WORD_LIST_IS_NULL);
			}
			bool isDefault = true;
			foreach (IWord word in words)
			{
				word.Default = isDefault;
				AddWord(word);
				isDefault = false;
			}
		}

		/// <summary>
		/// Clears all words from this instance.
		/// </summary>
		/// <remarks>Documented by Dev03, 2007-08-27</remarks>
		public void ClearWords()
		{
			this.Words.Clear();
		}

		#endregion

		/// <summary>
		/// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
		/// </returns>
		/// <remarks>Documented by Dev03, 2007-08-27</remarks>
		public override string ToString()
		{
			string[] words = new string[m_words.Count];
			for (int i = 0; i < m_words.Count; i++)
				words[i] = m_words[i].Word;
			return String.Join(", ", words);
		}

		/// <summary>
		/// Returns a comma delimited string of all words as quoted string.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Documented by Dev03, 2007-08-27</remarks>
		public string ToQuotedString()
		{
			string[] words = new string[m_words.Count];
			for (int i = 0; i < m_words.Count; i++)
				words[i] = m_words[i].Word;
			return Helper.ToQuotedCommaString(words);
		}

		/// <summary>
		/// Returns all words as a list of strings.
		/// </summary>
		/// <returns>The words.</returns>
		/// <remarks>Documented by Dev03, 2009-04-14</remarks>
		/// <remarks>Documented by Dev03, 2009-04-14</remarks>
		public IList<string> ToStringList()
		{
			string[] words = new string[m_words.Count];
			for (int i = 0; i < m_words.Count; i++)
				words[i] = m_words[i].Word;
			IList<string> result = new List<string>();
			(result as List<string>).AddRange(words);
			return result;
		}

		/// <summary>
		/// Returns all words as newline-delimited string.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Documented by Dev02, 2008-01-29</remarks>
		public string ToNewlineString()
		{
			string[] words = new string[m_words.Count];
			for (int i = 0; i < m_words.Count; i++)
				words[i] = m_words[i].Word;
			return String.Join(Environment.NewLine, words);
		}

		/// <summary>
		/// Flushes all words to the DOM.
		/// </summary>
		/// <remarks>Documented by Dev03, 2007-08-27</remarks>
		internal void FlushToDOM()
		{
			string[] words = new string[m_words.Count];
			for (int i = 0; i < m_words.Count; i++)
				words[i] = m_words[i].Word;

			if (m_words.Count > 1)
				m_card.SelectSingleNode(m_basePath).InnerText = Helper.ToQuotedCommaString(words);
			else
				m_card.SelectSingleNode(m_basePath).InnerText = String.Join(", ", words);
		}

		protected void Initialize(XmlCard card, WordType wordType)
		{
			m_oDictionary = card.Dictionary;
			m_card = card.Xml;

			if (wordType == WordType.Word)
			{
				string[] words;
				words = Helper.SplitWordList(m_card.SelectSingleNode(m_basePath).InnerText);
				for (int i = 0; i < words.Length; i++)
				{
					AddWord(new XmlWord(words[i], wordType, false, Parent.GetChildParentClass(this)));
				}
			}
			else
			{
				string sSentence = m_card.SelectSingleNode(m_basePath).InnerText.Trim();
				if (sSentence.Length > 0)
				{
					AddWord(new XmlWord(m_card.SelectSingleNode(m_basePath).InnerText, WordType.Sentence, false, Parent.GetChildParentClass(this)));
				}
			}
		}


		#region ICopy Members

		public void CopyTo(ICopy target, CopyToProgress progressDelegate)
		{
			WordsHelper.CopyWords(this, target as IWords);
		}

		#endregion

		#region IParent Members

		private ParentClass parent;

		public ParentClass Parent
		{
			get { return parent; }
		}

		#endregion
	}

	/// <summary>
	/// Question words.
	/// </summary>
	/// <remarks>Documented by Dev03, 2007-08-27</remarks>
	internal sealed class XmlQuestion : XmlWords
	{
		private new const string m_basePath = "question";

		internal XmlQuestion(XmlCard card, ParentClass parent)
			: base(parent)
		{
			base.m_basePath = m_basePath;
			this.m_Culture = card.Dictionary.QuestionCulture;
			Initialize(card, WordType.Word);
		}
	}

	/// <summary>
	/// Question example words.
	/// </summary>
	/// <remarks>Documented by Dev03, 2007-08-27</remarks>
	internal sealed class XmlQuestionExample : XmlWords
	{
		private new const string m_basePath = "questionexample";

		internal XmlQuestionExample(XmlCard card, ParentClass parent)
			: base(parent)
		{
			base.m_basePath = m_basePath;
			this.m_Culture = card.Dictionary.QuestionCulture;
			Initialize(card, WordType.Sentence);
		}
	}

	/// <summary>
	/// Answer words.
	/// </summary>
	/// <remarks>Documented by Dev03, 2007-08-27</remarks>
	internal sealed class XmlAnswer : XmlWords
	{
		private new const string m_basePath = "answer";

		internal XmlAnswer(XmlCard card, ParentClass parent)
			: base(parent)
		{
			base.m_basePath = m_basePath;
			this.m_Culture = card.Dictionary.AnswerCulture;
			Initialize(card, WordType.Word);
		}
	}

	/// <summary>
	/// Answer example
	/// </summary>
	/// <remarks>Documented by Dev03, 2007-08-27</remarks>
	internal sealed class XmlAnswerExample : XmlWords
	{
		private new const string m_basePath = "answerexample";

		internal XmlAnswerExample(XmlCard card, ParentClass parent)
			: base(parent)
		{
			base.m_basePath = m_basePath;
			this.m_Culture = card.Dictionary.AnswerCulture;
			Initialize(card, WordType.Sentence);
		}
	}
}
