using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MLifter.Controls.Properties;
using MLifter.BusinessLayer;

namespace MLifter.Controls.Wizards.Print
{
    public partial class BoxSelectionPage : MLifter.WizardPage
    {
        private Dictionary dic;
        /// <summary>
        /// Gets or sets the dictionary.
        /// </summary>
        /// <value>The dictionary.</value>
        /// <remarks>Documented by Dev05, 2007-12-27</remarks>
        public Dictionary Dictionary
        {
            get { return dic; }
            set { dic = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoxSelectionPage"/> class.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-11-21</remarks>
        /// <remarks>Documented by Dev05, 2007-12-27</remarks>
        public BoxSelectionPage()
        {
            InitializeComponent();
        }

        public override bool GoNext()
        {
            (ParentWizard.Tag as PrintSettings).IDs.Clear();

            if (radioButtonAll.Checked)
            {
                for (int i = 0; i <= 10; i++)
                {
                    (ParentWizard.Tag as PrintSettings).IDs.Add(i);
                }
            }
            else if (radioButtonPool.Checked)
                (ParentWizard.Tag as PrintSettings).IDs.Add(0);
            else
            {
                string[] blocks = textBoxNumbers.Text.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                if (blocks.Length == 0)
                {
                    ShowInvalidValuesError();

                    return false;
                }

                foreach (string str in blocks)
                {
                    if (!str.Contains("-"))
                        try { (ParentWizard.Tag as PrintSettings).IDs.Add(Convert.ToInt32(str)); }
                        catch { }
                    else
                    {
                        string[] borders = str.Split(new char[] { '-' });

                        try
                        {
                            int min = Convert.ToInt32(borders[0]);
                            int max = Convert.ToInt32(borders[1]);

                            do { (ParentWizard.Tag as PrintSettings).IDs.Add(min++); }
                            while (min <= max);
                        }
                        catch { }
                    }
                }

                foreach (int id in (ParentWizard.Tag as PrintSettings).IDs)
                {
                    if (!dic.Boxes.Exists(delegate(MLifter.DAL.Interfaces.IBox match)
                    {
                        return match.Id == id;
                    }))
                    {
                        ShowInvalidValuesError();
                        return false;
                    }
                }

                if ((ParentWizard.Tag as PrintSettings).IDs.Count == 0)
                {
                    ShowInvalidValuesError();

                    return false;
                }
            }

            return base.GoNext();
        }

        /// <summary>
        /// Shows the invalid values error.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-12-27</remarks>
        private void ShowInvalidValuesError()
        {
            MessageBox.Show(Resources.PRINT_BOXSELECTION_ERROR_TEXT, Resources.PRINT_BOXSELECTION_ERROR_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
            textBoxNumbers.Focus();
        }

        /// <summary>
        /// Handles the TextChanged event of the textBoxNumbers control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-01-24</remarks>
        private void textBoxNumbers_TextChanged(object sender, EventArgs e)
        {
            if (textBoxNumbers.Text != string.Empty)
            {
                radioButtonNumbers.Checked = true;
            }
        }

        /// <summary>
        /// Called if the Help Button is clicked.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-11-21</remarks>
        /// <remarks>Documented by Dev03, 2008-02-22</remarks>
        public override void ShowHelp()
        {
            Help.ShowHelp(this.ParentForm, this.ParentWizard.HelpFile, HelpNavigator.Topic, "/html/memo4f78.htm");
        }
    }
}

