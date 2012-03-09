using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using MLifter.BusinessLayer;
using MLifter.Controls.Properties;

namespace MLifter.Controls.LearningWindow
{
    public partial class StatusBar : UserControl, ILearnUserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatusBar"/> class.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-04-24</remarks>
        public StatusBar()
        {
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
        /// Gets or sets the control's Text.
        /// </summary>
        /// <value></value>
        /// <remarks>Documented by Dev02, 2008-04-24</remarks>
        public override string Text
        {
            get { return labelStatus.Text; }
            set { labelStatus.Text = value; }
        }

        /// <summary>
        /// Handles the CardStateChanged event of the learnlogic control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MLifter.BusinessLayer.CardStateChangedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-04-24</remarks>
        void learnlogic_CardStateChanged(object sender, CardStateChangedEventArgs e)
        {
            if (e is CardStateChangedNewCardEventArgs ||
                (e is CardStateChangedShowResultEventArgs && ((CardStateChangedShowResultEventArgs)e).slideshow))
            {
                Text = String.Format(Resources.STATUSBAR_CURRENTCHAPTER, e.dictionary.Cards.GetChapterName(e.cardid));
            }
            else if (e is CardStateChangedShowResultEventArgs)
            {
                if (e.dictionary.Settings.SelfAssessment.Value)
                    Text = Resources.STATUSBAR_SELFASSESSMENT;
                else
                {
                    CardStateChangedShowResultEventArgs args = (CardStateChangedShowResultEventArgs)e;
                    AnswerResult result = args.promoted ? AnswerResult.Correct : (args.result == AnswerResult.Almost ? AnswerResult.Almost : AnswerResult.Wrong);
                    switch (result)
                    {
                        case AnswerResult.Correct:
                            Text = Resources.STATUSBAR_RESPONSE_CORRECT;
                            break;
                        case AnswerResult.Wrong:
                            Text = Resources.STATUSBAR_RESPONSE_INCORRECT;
                            break;
                        case AnswerResult.Almost:
                            Text = Resources.STATUSBAR_RESPONSE_ALMOST;
                            break;
                    }
                }
            }
        }
    }
}
