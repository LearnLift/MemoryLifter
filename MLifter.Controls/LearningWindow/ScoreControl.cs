using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using MLifter.Controls.Properties;

namespace MLifter.Controls.LearningWindow
{
    public partial class ScoreControl : UserControl
    {
        private double score;
        /// <summary>
        /// Gets or sets the score.
        /// </summary>
        /// <value>The score.</value>
        /// <remarks>Documented by Dev05, 2009-04-10</remarks>
        public double Score
        {
            get { return score; }
            set
            {
                score = double.IsNaN(value) ? 0 : value;
                labelKnown.Text = string.Format("{0:##0.##}%", score);
                colorProgressBarKnown.Value = Convert.ToInt32(score);

                if (score < 20)
                {
                    pictureBoxIcon.Image = Resources.star_grey;
                    colorProgressBarKnown.BarColor = Color.Red;
                }
                else if (score < 40)
                {
                    pictureBoxIcon.Image = Resources.star_half;
                    colorProgressBarKnown.BarColor = Color.LightCoral;
                }
                else if (score < 60)
                {
                    pictureBoxIcon.Image = Resources.star;
                    colorProgressBarKnown.BarColor = Color.Gold;
                }
                else if (score < 80)
                {
                    pictureBoxIcon.Image = Resources.flag16;
                    colorProgressBarKnown.BarColor = Color.LightGreen;
                }
                else
                {
                    pictureBoxIcon.Image = Resources.trophy;
                    colorProgressBarKnown.BarColor = Color.LimeGreen;
                }
            }
        }

        /// <summary>
        /// Gets or sets the font of the text displayed by the control.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The <see cref="T:System.Drawing.Font"/> to apply to the text displayed by the control. The default is the value of the <see cref="P:System.Windows.Forms.Control.DefaultFont"/> property.
        /// </returns>
        /// <PermissionSet>
        /// 	<IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// 	<IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// 	<IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        /// 	<IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        /// <remarks>Documented by Dev05, 2009-04-10</remarks>
        public override Font Font
        {
            get { return base.Font; }
            set
            {
                labelKnown.Font = value;
                base.Font = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScoreControl"/> class.
        /// </summary>
        /// <remarks>Documented by Dev05, 2009-04-10</remarks>
        public ScoreControl()
        {
            InitializeComponent();

            ToolTip tTip = new ToolTip();
            tTip.SetToolTip(colorProgressBarKnown, Resources.LEARNING_MODULE_KNOWLEDGE);
            tTip.SetToolTip(labelKnown, Resources.LEARNING_MODULE_KNOWLEDGE);
        }
    }
}
