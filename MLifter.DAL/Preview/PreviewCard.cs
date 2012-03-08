using System;
using System.Collections.Generic;
using System.Text;

using MLifter.DAL.Interfaces;
using MLifter.DAL.Tools;
using MLifter.DAL.XML;

namespace MLifter.DAL.Preview
{
	/// <summary>
	/// This is a container object which is used to preview a card.
	/// It does not implement any data persistence!
	/// </summary>
	/// <remarks>Documented by Dev03, 2008-08-25</remarks>
	public class PreviewCard : Interfaces.ICard
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PreviewCard"/> class.
		/// </summary>
		/// <param name="parentClass">The parent class.</param>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public PreviewCard(ParentClass parentClass)
		{
			parent = parentClass;
			settings = new XmlPreviewCardSettings(this, null);
			Initialize();
		}

		/// <summary>
		/// Initializes this instance.
		/// </summary>
		/// <remarks>Documented by Dev02, 2008-09-23</remarks>
		private void Initialize()
		{
			question = new PreviewWords(null);
			questionExample = new PreviewWords(null);
			questionDistractors = new PreviewWords(null);

			answer = new PreviewWords(null);
			answerExample = new PreviewWords(null);
			answerDistractors = new PreviewWords(null);
		}

		#region ICard Members

		/// <summary>
		/// Occurs when [create media progress changed].
		/// </summary>
		/// <remarks>Documented by Dev03, 2008-08-21</remarks>
		public event StatusMessageEventHandler CreateMediaProgressChanged
		{
			add { }
			remove { }
		}

		/// <summary>
		/// Gets the card.
		/// </summary>
		/// <value>The card.</value>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public System.Xml.XmlElement Card
		{
			get
			{
				return DAL.Helper.GenerateXmlCard(this);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="ICard"/> is active.
		/// </summary>
		/// <value><c>true</c> if active; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public bool Active
		{
			get
			{
				return true;
			}
			set
			{ }
		}

		/// <summary>
		/// Gets or sets the id.
		/// </summary>
		/// <value>The id.</value>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public int Id
		{
			get { return -1; }
		}

		/// <summary>
		/// Gets or sets the box.
		/// </summary>
		/// <value>The box.</value>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public int Box
		{
			get { return 0; }
			set { }
		}

		int chapter;
		/// <summary>
		/// Gets or sets the chapter.
		/// </summary>
		/// <value>The chapter.</value>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public int Chapter
		{
			get { return chapter; }
			set { chapter = value; }
		}

		/// <summary>
		/// Gets or sets the timestamp.
		/// </summary>
		/// <value>The timestamp.</value>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public DateTime Timestamp
		{
			get { return DateTime.Now; }
			set { }
		}

		private IWords question;
		/// <summary>
		/// Gets or sets the question.
		/// </summary>
		/// <value>The question.</value>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public IWords Question
		{
			get { return question; }
			set { throw new Exception("The method or operation is not implemented."); }
		}

		private IWords questionExample;
		/// <summary>
		/// Gets or sets the question example.
		/// </summary>
		/// <value>The question example.</value>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public IWords QuestionExample
		{
			get { return questionExample; }
			set { throw new Exception("The method or operation is not implemented."); }
		}

		string questionStylesheet = string.Empty;
		/// <summary>
		/// Gets or sets the question stylesheet.
		/// </summary>
		/// <value>The question stylesheet.</value>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public string QuestionStylesheet
		{
			get { return questionStylesheet; }
			set { questionStylesheet = value; }
		}

		List<IMedia> questionMedia = new List<IMedia>();
		/// <summary>
		/// Gets or sets the question media.
		/// </summary>
		/// <value>
		/// The question media.
		/// </value>
		public IList<IMedia> QuestionMedia
		{
			get { return questionMedia; }
		}

		private IWords questionDistractors;
		/// <summary>
		/// Gets or sets the question distractors.
		/// </summary>
		/// <value>The question distractors.</value>
		/// <remarks>Documented by Dev03, 2008-01-07</remarks>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public IWords QuestionDistractors
		{
			get { return questionDistractors; }
			set { throw new Exception("The method or operation is not implemented."); }
		}

		private IWords answer;
		/// <summary>
		/// Gets or sets the answer.
		/// </summary>
		/// <value>The answer.</value>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public IWords Answer
		{
			get { return answer; }
			set { throw new Exception("The method or operation is not implemented."); }
		}

		private IWords answerExample;
		/// <summary>
		/// Gets or sets the answer example.
		/// </summary>
		/// <value>The answer example.</value>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public IWords AnswerExample
		{
			get { return answerExample; }
			set { throw new Exception("The method or operation is not implemented."); }
		}

		private string answerStylesheet = string.Empty;
		/// <summary>
		/// Gets or sets the answer stylesheet.
		/// </summary>
		/// <value>The answer stylesheet.</value>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public string AnswerStylesheet
		{
			get { return answerStylesheet; }
			set { answerStylesheet = value; }
		}

		List<IMedia> answerMedia = new List<IMedia>();
		/// <summary>
		/// Gets or sets the answer media.
		/// </summary>
		/// <value>The answer media.</value>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public IList<IMedia> AnswerMedia
		{
			get { return answerMedia; }
		}

		private IWords answerDistractors;
		/// <summary>
		/// Gets or sets the answer distractors.
		/// </summary>
		/// <value>The answer distractors.</value>
		/// <remarks>Documented by Dev03, 2008-01-07</remarks>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public IWords AnswerDistractors
		{
			get { return answerDistractors; }
			set { throw new Exception("The method or operation is not implemented."); }
		}

		ICardStyle style;
		/// <summary>
		/// Gets or sets the style.
		/// </summary>
		/// <value>The style.</value>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public ICardStyle Style
		{
			get { return style; }
			set { style = value; }
		}

		/// <summary>
		/// Adds the media.
		/// </summary>
		/// <param name="media">The media.</param>
		/// <param name="side">The side.</param>
		/// <returns>The media object.</returns>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public IMedia AddMedia(IMedia media, Side side)
		{
			switch (side)
			{
				case Side.Question:
					questionMedia.Add(media);
					break;
				case Side.Answer:
					answerMedia.Add(media);
					break;
			}
			return media;
		}

		/// <summary>
		/// Removes the media.
		/// </summary>
		/// <param name="media">The media.</param>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public void RemoveMedia(IMedia media)
		{
			if (questionMedia.Contains(media))
				questionMedia.Remove(media);
			if (answerMedia.Contains(media))
				answerMedia.Remove(media);
		}

		/// <summary>
		/// Creates the new media object.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="path">The path.</param>
		/// <param name="isActive">if set to <c>true</c> [is active].</param>
		/// <param name="isDefault">if set to <c>true</c> [is default].</param>
		/// <param name="isExample">if set to <c>true</c> [is example].</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public static IMedia CreateNewMediaObject(EMedia type, string path, bool isActive, bool isDefault, bool isExample)
		{
			IMedia media;
			switch (type)
			{
				case EMedia.Audio:
					media = new PreviewAudio(path, isActive, isDefault, isExample, null);
					break;
				case EMedia.Image:
					media = new PreviewImage(path, isActive, isDefault, isExample, null);
					break;
				case EMedia.Video:
					media = new PreviewVideo(path, isActive, isDefault, isExample, null);
					break;
				default:
					media = new PreviewMedia(type, path, isActive, isDefault, isExample, null);
					break;
			}
			return media;
		}

		/// <summary>
		/// Creates the media.
		/// </summary>
		/// <param name="type">The type of the media file.</param>
		/// <param name="path">The path to the media file.</param>
		/// <param name="isActive">if set to <c>true</c> [is active].</param>
		/// <param name="isDefault">if set to <c>true</c> [is default].</param>
		/// <param name="isExample">if set to <c>true</c> [is example].</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public IMedia CreateMedia(EMedia type, string path, bool isActive, bool isDefault, bool isExample)
		{
			return CreateNewMediaObject(type, path, isActive, isDefault, isExample);
		}

		/// <summary>
		/// Clears all media.
		/// </summary>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public void ClearAllMedia()
		{
			ClearAllMedia(false);
		}

		/// <summary>
		/// Clears all media.
		/// </summary>
		/// <param name="removeFiles">if set to <c>true</c> [remove files].</param>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public void ClearAllMedia(bool removeFiles)
		{
			questionMedia.Clear();
			answerMedia.Clear();
		}

		/// <summary>
		/// Creates and returns a card style.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Documented by Dev03, 2008-01-08</remarks>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public ICardStyle CreateCardStyle()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		private ISettings settings;
		/// <summary>
		/// Gets or sets the settings.
		/// </summary>
		/// <value>The settings.</value>
		/// <remarks>Documented by Dev05, 2008-08-19</remarks>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public ISettings Settings
		{
			get
			{
				return settings;
			}
			set
			{
				settings = value;
			}
		}

		#endregion

		#region IDisposable Members

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public void Dispose()
		{ }

		#endregion

		#region ICopy Members

		/// <summary>
		/// Copies to.
		/// </summary>
		/// <param name="target">The target.</param>
		/// <param name="progressDelegate">The progress delegate.</param>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public void CopyTo(ICopy target, CopyToProgress progressDelegate)
		{
			CopyBase.Copy(this, target, typeof(ICard), progressDelegate);

			//copy media objects
			ICard targetCard = target as ICard;
			if (targetCard != null)
			{
				foreach (IMedia media in QuestionMedia)
					targetCard.AddMedia(media, Side.Question);
				foreach (IMedia media in AnswerMedia)
					targetCard.AddMedia(media, Side.Answer);
			}
		}

		#endregion

		#region IParent Members

		private ParentClass parent;
		/// <summary>
		/// Gets the parent.
		/// </summary>
		/// <value>The parent.</value>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public ParentClass Parent { get { return parent; } }

		#endregion

		#region ISecurity Members

		/// <summary>
		/// Determines whether the object has the specified permission.
		/// </summary>
		/// <param name="permissionName">Name of the permission.</param>
		/// <returns>
		/// 	<c>true</c> if the object name has the specified permission; otherwise, <c>false</c>.
		/// </returns>
		/// <remarks>Documented by Dev03, 2009-01-15</remarks>
		/// <remarks>Documented by Dev03, 2009-01-15</remarks>
		public bool HasPermission(string permissionName)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets the permissions for the object.
		/// </summary>
		/// <returns>A list of permissions for the object.</returns>
		/// <remarks>Documented by Dev03, 2009-01-15</remarks>
		/// <remarks>Documented by Dev03, 2009-01-15</remarks>
		public List<SecurityFramework.PermissionInfo> GetPermissions()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
