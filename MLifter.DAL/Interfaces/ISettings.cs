using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using MLifter.DAL.DB;
using MLifter.DAL.Security;
using MLifter.DAL.Tools;
using MLifter.DAL.XML;

namespace MLifter.DAL.Interfaces
{
    /// <summary>
    /// Save all Settings for MemoryLifter (GlobalSettings, LM AllowedSettings, UserSettings)
    /// </summary>
    /// <remarks>Documented by Dev11, 2008-08-08</remarks>
    public interface ISettings : ICopy, IParent, ISecurity
    {
        /// <summary>
        /// Interface that defines the available query directions for a dictionary.
        /// </summary>
        /// <remarks>Documented by Dev03, 2008-01-08</remarks>
        IQueryDirections QueryDirections { get; set; }
        /// <summary>
        /// Gets or sets the Learning modes (How MemoryLifter ask the questions)
        /// </summary>
        /// <value>The query types.</value>
        /// <remarks>Documented by Dev11, 2008-08-08</remarks>
        IQueryType QueryTypes { get; set; }
        // User settings 
        /// <summary>
        /// Gets or sets the multiple choice options.
        /// </summary>
        /// <value>The multiple choice options.</value>
        /// <remarks>Documented by Dev03, 2008-01-08</remarks>
        IQueryMultipleChoiceOptions MultipleChoiceOptions { get; set; }
        /// <summary>
        /// Enumeration which defines the values for the 'Typing mistakes' options:
        /// AllCorrect - only promote when all correct,
        /// HalfCorrect - promote when more than 50% were typed correctly,
        /// NoneCorrect - always promote,
        /// Prompt - prompt
        /// </summary>
        /// <value>The grade typing.</value>
        /// <remarks>Documented by Dev11, 2008-08-08</remarks>
        IGradeTyping GradeTyping { get; set; }
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
        IGradeSynonyms GradeSynonyms { get; set; }
        /// <summary>
        /// Gets or sets the style.
        /// </summary>
        /// <value>The style.</value>
        /// <remarks>Documented by Dev05, 2008-08-19</remarks>
        ICardStyle Style { get; set; }
        /// <summary>
        /// Gets or sets the question stylesheet.
        /// </summary>
        /// <value>The question stylesheet.</value>
        /// <remarks>Documented by Dev05, 2008-08-19</remarks>
        [ValueCopy]
        CompiledTransform? QuestionStylesheet { get; set; }
        /// <summary>
        /// Gets or sets the answer stylesheet.
        /// </summary>
        /// <value>The answer stylesheet.</value>
        /// <remarks>Documented by Dev05, 2008-08-19</remarks>
        [ValueCopy]
        CompiledTransform? AnswerStylesheet { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether standard audio files should be played automatically.
        /// </summary>
        /// <value><c>true</c> if [autoplay audio]; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev03, 2007-09-04</remarks>
        [ValueCopy]
        bool? AutoplayAudio { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [case sensitive].
        /// </summary>
        /// <value><c>true</c> if [case sensitive]; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev03, 2007-09-04</remarks>
        [ValueCopy]
        bool? CaseSensitive { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether user confirmation is required to confirm demote.
        /// </summary>
        /// <value><c>true</c> if [confirm demote]; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev03, 2007-09-04</remarks>
        [ValueCopy]
        bool? ConfirmDemote { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether commentary sounds are enabled.
        /// </summary>
        /// <value><c>true</c> if [enable commentary]; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev03, 2007-09-04</remarks>
        [ValueCopy]
        bool? EnableCommentary { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [correct on the fly].
        /// </summary>
        /// <value><c>true</c> if [correct on the fly]; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev03, 2007-09-04</remarks>
        [ValueCopy]
        bool? CorrectOnTheFly { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the time should be enabled.
        /// </summary>
        /// <value><c>true</c> if [enable timer]; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev03, 2007-09-04</remarks>
        [ValueCopy]
        bool? EnableTimer { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether cards should be drawn randomly from pool.
        /// </summary>
        /// <value><c>true</c> if [random pool]; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev03, 2007-09-04</remarks>
        [ValueCopy]
        bool? RandomPool { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether self assessment mode is active.
        /// </summary>
        /// <value><c>true</c> if [self assessment]; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev03, 2007-09-04</remarks>
        [ValueCopy]
        bool? SelfAssessment { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [show images].
        /// </summary>
        /// <value><c>true</c> if [show images]; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev03, 2007-09-04</remarks>
        [ValueCopy]
        bool? ShowImages { get; set; }
        /// <summary>
        /// Gets or sets wether the pool emty message was shown.
        /// </summary>
        /// <value>The pool emty message shown.</value>
        /// <remarks>Documented by Dev05, 2008-08-18</remarks>
        [ValueCopy]
        bool? PoolEmptyMessageShown { get; set; }
        /// <summary>
        /// Gets or sets the use LM stylesheets.
        /// </summary>
        /// <value>The use LM stylesheets.</value>
        /// <remarks>Documented by Dev05, 2008-08-18</remarks>
        [ValueCopy]
        bool? UseLMStylesheets { get; set; }
        /// <summary>
        /// Gets or sets the the auto box size.
        /// </summary>
        /// <value>The size of the auto box.</value>
        /// <remarks>Documented by Dev05, 2008-08-19</remarks>
        [ValueCopy]
        bool? AutoBoxSize { get; set; }
        /// <summary>
        /// Gets or sets the strip chars.
        /// </summary>
        /// <value>The strip chars.</value>
        /// <remarks>Documented by Dev03, 2007-09-04</remarks>
        [ValueCopy]
        string StripChars { get; set; }
        /// <summary>
        /// Gets or sets the question culture.
        /// </summary>
        /// <value>The question culture.</value>
        /// <remarks>Documented by Dev03, 2007-12-18</remarks>
        [ValueCopy]
        CultureInfo QuestionCulture { get; set; }
        /// <summary>
        /// Gets or sets the answer culture.
        /// </summary>
        /// <value>The answer culture.</value>
        /// <remarks>Documented by Dev03, 2007-12-18</remarks>
        [ValueCopy]
        CultureInfo AnswerCulture { get; set; }
        /// <summary>
        /// Gets or sets the question caption.
        /// </summary>
        /// <value>The question caption.</value>
        /// <remarks>Documented by Dev03, 2007-09-03</remarks>
        [ValueCopy]
        string QuestionCaption { get; set; }
        /// <summary>
        /// Gets or sets the answer caption.
        /// </summary>
        /// <value>The answer caption.</value>
        /// <remarks>Documented by Dev03, 2007-09-03</remarks>
        [ValueCopy]
        string AnswerCaption { get; set; }
        /// <summary>
        /// Gets or sets the commentary sounds.
        /// </summary>
        /// <value>The commentary sounds.</value>
        /// <remarks>Documented by Dev11, 2008-08-08</remarks>
        Dictionary<CommentarySoundIdentifier, IMedia> CommentarySounds { get; set; }
        /// <summary>
        /// Gets or sets the logo.
        /// </summary>
        /// <value>The logo.</value>
        /// <remarks>Documented by Dev03, 2008-01-11</remarks>
        IMedia Logo { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [show statistics].
        /// </summary>
        /// <value><c>true</c> if [show statistics]; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev03, 2007-09-04</remarks>
        [ValueCopy]
        bool? ShowStatistics { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [skip correct answers].
        /// </summary>
        /// <value><c>true</c> if [skip correct answers]; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev03, 2007-09-04</remarks>
        [ValueCopy]
        bool? SkipCorrectAnswers { get; set; }
        /// <summary>
        /// Gets or sets the snooze options (MemoryLifter minimize after a few cards)
        /// </summary>
        /// <value>The snooze options.</value>
        /// <remarks>Documented by Dev11, 2008-08-08</remarks>
        ISnoozeOptions SnoozeOptions { get; set; }
        /// <summary>
        /// Gets or sets the selected learn chapters.
        /// </summary>
        /// <value>The selected learn chapters.</value>
        /// <remarks>Documented by Dev05, 2008-08-18</remarks>
        IList<int> SelectedLearnChapters { get; set; }
    }

    internal static class SettingsHelper
    {
        public static void Copy(ISettings source, ISettings target, CopyToProgress progressDelegate)
        {
            try
            {
                if (!(source is XmlAllowedSettings) && source.Style != null && target.Style == null)
                    target.Style = target.Parent.GetParentDictionary().CreateCardStyle();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }

            CopyBase.Copy(source, target, typeof(ISettings), progressDelegate);

            try
            {
                if (source is XmlChapterSettings || source is XmlCardSettings || source is XmlAllowedSettings || source.Logo == null)
                    target.Logo = null;
                else
                    target.Logo = GetNewMedia(source.Logo, target);
            }
            catch { }

            try
            {
                if (!(source is XmlChapterSettings || source is XmlCardSettings || source is XmlAllowedSettings))
                {
                    Dictionary<CommentarySoundIdentifier, IMedia> cSounds = new Dictionary<CommentarySoundIdentifier, IMedia>();
                    foreach (KeyValuePair<CommentarySoundIdentifier, IMedia> pair in source.CommentarySounds)
                        cSounds.Add(pair.Key, GetNewMedia(pair.Value, target));
                    target.CommentarySounds = cSounds;
                }
            }
            catch { }
        }

        private static IMedia GetNewMedia(IMedia oldMedia, ISettings target)
        {
            return target is DbSettings ? (target.Parent.GetParentDictionary() as DbDictionary).CreateNewMediaObject(null, null, oldMedia.MediaType, oldMedia.Filename,
                    oldMedia.Active.Value, oldMedia.Default.Value, oldMedia.Example.Value) :
                    (target.Parent.GetParentDictionary() as XmlDictionary).CreateNewMediaObject(oldMedia.MediaType, oldMedia.Filename,
                    oldMedia.Active.Value, oldMedia.Default.Value, oldMedia.Example.Value);
        }
    }

    /// <summary>
    /// Struct to hold a string/file trasnformer.
    /// </summary>
    /// <remarks>Documented by Dev05, 2009-02-09</remarks>
    public struct CompiledTransform
    {
        /// <summary>
        /// Path to a xsl-file.
        /// </summary>
        public string Filename;
        /// <summary>
        /// The complier-source.
        /// </summary>
        public string XslContent;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompiledTransform"/> struct.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="content">The content.</param>
        /// <remarks>Documented by Dev05, 2009-02-09</remarks>
        public CompiledTransform(string filename, string content)
        {
            Filename = filename;
            XslContent = content;

            if (filename != null)
            {
                if (File.Exists(filename))
                    XslContent = File.ReadAllText(filename);
            }
            else if (content == null)
                throw new ArgumentNullException();
        }
    }

    /// <summary>
    /// An identifier for a commentary sound.
    /// </summary>
    /// <remarks>Documented by Dev05, 2009-02-09</remarks>
    public struct CommentarySoundIdentifier
    {
        /// <summary>
        /// The Side.
        /// </summary>
        public Side Side;
        /// <summary>
        /// The type.
        /// </summary>
        public ECommentarySoundType Type;

        /// <summary>
        /// Creates the specified identifier.
        /// </summary>
        /// <param name="side">The side.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2009-02-09</remarks>
        public static CommentarySoundIdentifier Create(Side side, ECommentarySoundType type)
        {
            CommentarySoundIdentifier idf = new CommentarySoundIdentifier();
            idf.Side = side;
            idf.Type = type;

            return idf;
        }
    }

    /// <summary>
    /// The types of commentary sounds.
    /// </summary>
    public enum ECommentarySoundType
    {
        /// <summary>
        /// See name...
        /// </summary>
        RightStandAlone = 0,
        /// <summary>
        /// See name...
        /// </summary>
        WrongStandAlone,
        /// <summary>
        /// See name...
        /// </summary>
        AlmostStandAlone,
        /// <summary>
        /// See name...
        /// </summary>
        Right,
        /// <summary>
        /// See name...
        /// </summary>
        Wrong,
        /// <summary>
        /// See name...
        /// </summary>
        Almost
    }

}
