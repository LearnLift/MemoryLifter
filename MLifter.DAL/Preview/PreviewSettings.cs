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
using System.Text;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Tools;

namespace MLifter.DAL.Preview
{
	/// <summary>
	/// This is a container object which is used to contain the preview settings.
	/// It does not implement any data persistence!
	/// </summary>
	/// <remarks>Documented by Dev03, 2008-08-25</remarks>
	public class PreviewSettings : ISettings
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PreviewSettings"/> class.
		/// </summary>
		/// <param name="parentClass">The parent class.</param>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public PreviewSettings(ParentClass parentClass)
		{
			parent = parentClass;
		}

		#region ISettings Members

		/// <summary>
		/// Interface that defines the available query directions for a dictionary.
		/// </summary>
		/// <value></value>
		/// <remarks>Documented by Dev03, 2008-01-08</remarks>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public IQueryDirections QueryDirections
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Gets or sets the Learning modes (How MemoryLifter ask the questions)
		/// </summary>
		/// <value>The query types.</value>
		/// <remarks>Documented by Dev11, 2008-08-08</remarks>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public IQueryType QueryTypes
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Gets or sets the multiple choice options.
		/// </summary>
		/// <value>The multiple choice options.</value>
		/// <remarks>Documented by Dev03, 2008-01-08</remarks>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public IQueryMultipleChoiceOptions MultipleChoiceOptions
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Enumeration which defines the values for the 'Typing mistakes' options:
		/// AllCorrect - only promote when all correct,
		/// HalfCorrect - promote when more than 50% were typed correctly,
		/// NoneCorrect - always promote,
		/// Prompt - prompt
		/// </summary>
		/// <value>The grade typing.</value>
		/// <remarks>Documented by Dev11, 2008-08-08</remarks>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public IGradeTyping GradeTyping
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Enumeration which defines the values for the 'Synonyms' options:
		/// AllKnown - all synonyms need to be known,
		/// HalfKnown - half need to be known,
		/// OneKnown - one synonym needs to be known,
		/// FirstKnown - the card is promoted when the first synonym is known,
		/// Promp - prompt when not all were correct
		/// </summary>
		/// <value>The grade synonyms.</value>
		/// <remarks>Documented by Dev11, 2008-08-08</remarks>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public IGradeSynonyms GradeSynonyms
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Gets or sets the style.
		/// </summary>
		/// <value>The style.</value>
		/// <remarks>Documented by Dev05, 2008-08-19</remarks>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public ICardStyle Style
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Gets or sets the question stylesheet.
		/// </summary>
		/// <value>The question stylesheet.</value>
		/// <remarks>Documented by Dev05, 2008-08-19</remarks>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public CompiledTransform? QuestionStylesheet
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Gets or sets the answer stylesheet.
		/// </summary>
		/// <value>The answer stylesheet.</value>
		/// <remarks>Documented by Dev05, 2008-08-19</remarks>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public CompiledTransform? AnswerStylesheet
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether standard audio files should be played automatically.
		/// </summary>
		/// <value><c>true</c> if [autoplay audio]; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev03, 2007-09-04</remarks>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public bool? AutoplayAudio
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether [case sensitive].
		/// </summary>
		/// <value><c>true</c> if [case sensitive]; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev03, 2007-09-04</remarks>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public bool? CaseSensitive
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether user confirmation is required to confirm demote.
		/// </summary>
		/// <value><c>true</c> if [confirm demote]; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev03, 2007-09-04</remarks>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public bool? ConfirmDemote
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether commentary sounds are enabled.
		/// </summary>
		/// <value><c>true</c> if [enable commentary]; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev03, 2007-09-04</remarks>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public bool? EnableCommentary
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether [correct on the fly].
		/// </summary>
		/// <value><c>true</c> if [correct on the fly]; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev03, 2007-09-04</remarks>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public bool? CorrectOnTheFly
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the time should be enabled.
		/// </summary>
		/// <value><c>true</c> if [enable timer]; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev03, 2007-09-04</remarks>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public bool? EnableTimer
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether cards should be drawn randomly from pool.
		/// </summary>
		/// <value><c>true</c> if [random pool]; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev03, 2007-09-04</remarks>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public bool? RandomPool
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether self assessment mode is active.
		/// </summary>
		/// <value><c>true</c> if [self assessment]; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev03, 2007-09-04</remarks>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public bool? SelfAssessment
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether [show images].
		/// </summary>
		/// <value><c>true</c> if [show images]; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev03, 2007-09-04</remarks>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public bool? ShowImages
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Gets or sets wether the pool emty message was shown.
		/// </summary>
		/// <value>The pool emty message shown.</value>
		/// <remarks>Documented by Dev05, 2008-08-18</remarks>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public bool? PoolEmptyMessageShown
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Gets or sets the use LM stylesheets.
		/// </summary>
		/// <value>The use LM stylesheets.</value>
		/// <remarks>Documented by Dev05, 2008-08-18</remarks>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public bool? UseLMStylesheets
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Gets or sets the the auto box size.
		/// </summary>
		/// <value>The size of the auto box.</value>
		/// <remarks>Documented by Dev05, 2008-08-19</remarks>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public bool? AutoBoxSize
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Gets or sets the strip chars.
		/// </summary>
		/// <value>The strip chars.</value>
		/// <remarks>Documented by Dev03, 2007-09-04</remarks>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public string StripChars
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Gets or sets the question culture.
		/// </summary>
		/// <value>The question culture.</value>
		/// <remarks>Documented by Dev03, 2007-12-18</remarks>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public System.Globalization.CultureInfo QuestionCulture
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Gets or sets the answer culture.
		/// </summary>
		/// <value>The answer culture.</value>
		/// <remarks>Documented by Dev03, 2007-12-18</remarks>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public System.Globalization.CultureInfo AnswerCulture
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Gets or sets the question caption.
		/// </summary>
		/// <value>The question caption.</value>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public string QuestionCaption
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Gets or sets the answer caption.
		/// </summary>
		/// <value>The answer caption.</value>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public string AnswerCaption
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Gets or sets the commentary sounds.
		/// </summary>
		/// <value>The commentary sounds.</value>
		/// <remarks>Documented by Dev11, 2008-08-08</remarks>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public Dictionary<CommentarySoundIdentifier, IMedia> CommentarySounds
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		IMedia logo = null;
		/// <summary>
		/// Gets or sets the logo.
		/// </summary>
		/// <value>The logo.</value>
		/// <remarks>Documented by Dev03, 2008-01-11</remarks>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public IMedia Logo
		{
			get
			{
				return logo;
			}
			set
			{
				logo = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether [show statistics].
		/// </summary>
		/// <value><c>true</c> if [show statistics]; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev03, 2007-09-04</remarks>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public bool? ShowStatistics
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether [skip correct answers].
		/// </summary>
		/// <value><c>true</c> if [skip correct answers]; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev03, 2007-09-04</remarks>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public bool? SkipCorrectAnswers
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Gets or sets the snooze options (MemoryLifter minimize after a few cards)
		/// </summary>
		/// <value>The snooze options.</value>
		/// <remarks>Documented by Dev11, 2008-08-08</remarks>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public ISnoozeOptions SnoozeOptions
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Gets or sets the selected learn chapters.
		/// </summary>
		/// <value>The selected learn chapters.</value>
		/// <remarks>Documented by Dev05, 2008-08-18</remarks>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public IList<int> SelectedLearnChapters
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		#endregion

		#region ICopy Members

		/// <summary>
		/// Copies to.
		/// </summary>
		/// <param name="target">The target.</param>
		/// <param name="progressDelegate">The progress delegate.</param>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public void CopyTo(MLifter.DAL.Tools.ICopy target, MLifter.DAL.Tools.CopyToProgress progressDelegate)
		{
			throw new NotImplementedException();
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
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public bool HasPermission(string permissionName)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets the permissions for the object.
		/// </summary>
		/// <returns>A list of permissions for the object.</returns>
		/// <remarks>Documented by Dev03, 2009-01-15</remarks>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public List<SecurityFramework.PermissionInfo> GetPermissions()
		{
			throw new NotImplementedException();
		}

		#endregion
	}

	/// <summary>
	/// This is a container object which is used to contain the preview snooze options.
	/// It does not implement any data persistence!
	/// </summary>
	public class PreviewSnoozeOptions : ISnoozeOptions
	{

		#region ISnoozeOptions Members

		/// <summary>
		/// Disables the snooze mode 'Cards'.
		/// </summary>
		public void DisableCards()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Disables the snooze mode 'Rights'.
		/// </summary>
		public void DisableRights()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Disables the snooze mode 'Time'.
		/// </summary>
		public void DisableTime()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Enables snooze mode 'Cards'.
		/// </summary>
		/// <param name="cards">The number of cards.</param>
		public void EnableCards(int cards)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Enables snooze mode 'Rights'.
		/// </summary>
		/// <param name="rights">The number of correctly answered cards.</param>
		public void EnableRights(int rights)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Enables snooze mode 'Time'.
		/// </summary>
		/// <param name="time">The time in minutes.</param>
		public void EnableTime(int time)
		{
			throw new NotImplementedException();
		}

		bool cardsenabled;
		/// <summary>
		/// Gets or sets a value indicating whether this instance is cards enabled.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is cards enabled; otherwise, <c>false</c>.
		/// </value>
		public bool? IsCardsEnabled
		{
			get
			{
				return cardsenabled;
			}
			set
			{
				cardsenabled = value.Value;
			}
		}

		bool rightsenabled;
		/// <summary>
		/// Gets or sets a value indicating whether this instance is rights enabled.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is rights enabled; otherwise, <c>false</c>.
		/// </value>
		public bool? IsRightsEnabled
		{
			get
			{
				return rightsenabled;
			}
			set
			{
				rightsenabled = value.Value;
			}
		}

		bool timeenabled;
		/// <summary>
		/// Gets or sets a value indicating whether this instance is time enabled.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is time enabled; otherwise, <c>false</c>.
		/// </value>
		public bool? IsTimeEnabled
		{
			get
			{
				return timeenabled;
			}
			set
			{
				timeenabled = value.Value;
			}
		}

		/// <summary>
		/// Sets the timeframe within which ML will popup from snooze mode.
		/// </summary>
		/// <param name="lower_time">The lower_time.</param>
		/// <param name="upper_time">The upper_time.</param>
		public void SetSnoozeTimes(int lower_time, int upper_time)
		{
			throw new NotImplementedException();
		}

		ESnoozeMode snoozeMode;
		/// <summary>
		/// Gets or sets the snooze mode (SendToTray or QuitProgram).
		/// </summary>
		/// <value>
		/// The snooze mode.
		/// </value>
		public ESnoozeMode? SnoozeMode
		{
			get
			{
				return snoozeMode;
			}
			set
			{
				snoozeMode = value.Value;
			}
		}
		int snoozeCards;
		/// <summary>
		/// Gets the number of answered cards after which ML will go into snooze mode.
		/// </summary>
		/// <value>
		/// The snooze rights.
		/// </value>
		public int? SnoozeCards
		{
			get
			{
				return snoozeCards;
			}
			set
			{
				snoozeCards = value.Value;
			}
		}

		int snoozeRights;
		/// <summary>
		/// Gets the number of correctly answered cards after which ML will go into snooze mode.
		/// </summary>
		/// <value>
		/// The snooze rights.
		/// </value>
		public int? SnoozeRights
		{
			get
			{
				return snoozeRights;
			}
			set
			{
				snoozeRights = value.Value;
			}
		}

		int snoozeTime;
		/// <summary>
		/// Gets the time (in minutes) after which ML will go into snooze mode.
		/// </summary>
		/// <value>
		/// The snooze time.
		/// </value>
		public int? SnoozeTime
		{
			get
			{
				return snoozeTime;
			}
			set
			{
				snoozeTime = value.Value;
			}
		}

		int snoozeHigh;
		/// <summary>
		/// Gets the higher value (in minutes) after with ML will popup from snooze mode.
		/// </summary>
		/// <value>
		/// The snooze high.
		/// </value>
		public int? SnoozeHigh
		{
			get
			{
				return snoozeHigh;
			}
			set
			{
				snoozeHigh = value.Value;
			}
		}

		int snoozeLow;
		/// <summary>
		/// Gets the lower value (in minutes) after with ML will popup from snooze mode.
		/// </summary>
		/// <value>
		/// The snooze low.
		/// </value>
		public int? SnoozeLow
		{
			get
			{
				return snoozeLow;
			}
			set
			{
				snoozeLow = value.Value;
			}
		}

		#endregion

		#region ICopy Members

		/// <summary>
		/// Copies this instance to the specified target.
		/// </summary>
		/// <param name="target">The target.</param>
		/// <param name="progressDelegate">The progress delegate.</param>
		public void CopyTo(ICopy target, CopyToProgress progressDelegate)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IParent Members

		/// <summary>
		/// Gets the parent.
		/// </summary>
		public ParentClass Parent
		{
			get { throw new NotImplementedException(); }
		}

		#endregion
	}
}
