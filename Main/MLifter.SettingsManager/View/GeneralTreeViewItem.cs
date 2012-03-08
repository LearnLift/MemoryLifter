using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using MLifter.DAL.Interfaces;

namespace MLifterSettingsManager
{
    public class GeneralTreeViewItem : INotifyPropertyChanged
    {
        #region Private Fields

        private bool isExpanded;
        private bool isSelected;

        #endregion

        public GeneralTreeViewItem()
        {

        }

        public GeneralTreeViewItem(GeneralTreeViewItem parent)
        {
            Parent = parent;
        }

        #region Properties

        /// <summary>
        /// Gets or sets a value, if the TreeViewItem is expanded or not
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is expanded; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Documented by Dev08, 2009-07-15</remarks>
        public bool IsExpanded
        {
            get
            {
                return isExpanded;
            }
            set
            {
                if (isExpanded == value)
                    return;

                isExpanded = value;
                OnPropertyChanged(this, new PropertyChangedEventArgs("IsExpanded"));
            }
        }

        /// <summary>
        /// Gets or sets a value, if the TreeViewItem is selected or not
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is selected; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Documented by Dev08, 2009-07-15</remarks>
        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                if (isSelected == value)
                    return;

                isSelected = value;
                OnPropertyChanged(this, new PropertyChangedEventArgs("IsSelected"));
            }
        }

        /// <summary>
        /// Gets the type of the tree view item.
        /// </summary>
        /// <value>The type of the tree view item.</value>
        /// <remarks>Documented by Dev08, 2009-07-15</remarks>
        public string TreeViewItemType
        {
            get
            {
                Type myType = this.GetType();

                if (myType == typeof(LearningModuleTreeViewItem))
                    return "Learning Module";
                else if (myType == typeof(ChapterTreeViewItem))
                    return "Chapter";
                else if (myType == typeof(CardTreeViewItem))
                    return "checked Card(s)";
                else
                    return string.Empty;
            }
        }

        public GeneralTreeViewItem Parent { get; set; }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler myHandler = PropertyChanged;
            if (myHandler != null)
                myHandler(sender, e);
        }

        #endregion
    }
}
