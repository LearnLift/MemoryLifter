using System;
using System.Collections.Generic;
using System.Text;
using MLifter.Components;
using System.Windows.Forms;
using System.Xml.Serialization;
using XPExplorerBar;
using System.IO;
using MLifter.Controls.Properties;
using System.Diagnostics;
using System.Drawing;

namespace MLifter.Controls.LearningModulesPage
{
    public partial class LearningModulesPage
    {
        internal bool PersistencyLoaded = false;
        internal bool Loading = false;

        private static XmlSerializer persistencySerializer = new XmlSerializer(typeof(LearningModulesPagePersitencyData));

        internal Size PersistancySize { get; set; }

        private void LoadPersistencyData()
        {
            try
            {
                Loading = true;
                TextReader reader = new StringReader(Settings.Default.StartPagePersistencyData);
                LearningModulesPagePersitencyData data = persistencySerializer.Deserialize(reader) as LearningModulesPagePersitencyData;

                this.Maximized = data.Maximized;
                this.Size = data.PageSize;
                PersistancySize = data.PageSize;
                SetListViewView(data.ListViewView);
                SetListViewGrouping(data.OrderType);
                checkBoxShowTreeView.Checked = data.HideXPExplorerBar;
                SetLeftBarView(data.HideXPExplorerBar ? LeftBarView.TreeView : LeftBarView.XPExplorerBar);

                foreach (KeyValuePair<int, int> pair in data.ExpandoPositions)
                    taskPaneInformations.Expandos.Move(CollabsidableExpandos[pair.Key], pair.Value);
                foreach (KeyValuePair<int, bool> pair in data.ExpandoStates)
                    CollabsidableExpandos[pair.Key].Collapsed = pair.Value;

                learningModulesTreeViewControl.ShowLearningModulesOfSubFolder = data.ShowLearningModulesOfSubFolder;

                CheckHeight(null);

                PersistencyLoaded = true;
            }
            catch (Exception e) { Trace.WriteLine(e.ToString()); }
            finally { Loading = false; }
        }

        private void SavePersistencyData()
        {
            LearningModulesPagePersitencyData data = new LearningModulesPagePersitencyData();
            data.ListViewView = listViewLearningModules.View;
            data.Maximized = this.Maximized;
            data.PageSize = this.Size;
            data.OrderType = categoryToolStripMenuItem.Checked ? ItemOrderType.Category : authorToolStripMenuItem.Checked ? ItemOrderType.Author : ItemOrderType.Location;
            data.HideXPExplorerBar = checkBoxShowTreeView.Checked;
            data.ShowLearningModulesOfSubFolder = learningModulesTreeViewControl.ShowLearningModulesOfSubFolder;
            foreach (Expando exp in taskPaneInformations.Expandos)
            {
                data.ExpandoPositions.Add(CollabsidableExpandos.IndexOf(exp), taskPaneInformations.Expandos.IndexOf(exp));
                data.ExpandoStates.Add(CollabsidableExpandos.IndexOf(exp), exp.Collapsed);
            }

            TextWriter writer = new StringWriter();
            persistencySerializer.Serialize(writer, data);
            Settings.Default.StartPagePersistencyData = writer.ToString();
            Settings.Default.Save();
        }
    }

    /// <summary>
    /// This class holds all persistency data about the start page.
    /// </summary>
    /// <remarks>Documented by Dev05, 2009-03-16</remarks>
    [Serializable]
    public class LearningModulesPagePersitencyData
    {
        /// <summary>
        /// Gets or sets a value indicating whether show learning modules of sub folder.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if show learning modules of sub folder; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Documented by Dev05, 2009-03-16</remarks>
        public bool ShowLearningModulesOfSubFolder { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether show XP explorer bar or not.
        /// </summary>
        /// <value><c>true</c> if show XP explorer bar; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev05, 2009-03-16</remarks>
        public bool HideXPExplorerBar { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="LearningModulesPagePersitencyData"/> is maximized.
        /// </summary>
        /// <value><c>true</c> if maximized; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev05, 2009-05-25</remarks>
        public bool Maximized { get; set; }
        /// <summary>
        /// Gets or sets the size of the page.
        /// </summary>
        /// <value>The size of the page.</value>
        /// <remarks>Documented by Dev05, 2009-03-23</remarks>
        public Size PageSize { get; set; }
        /// <summary>
        /// Gets or sets the list view view.
        /// </summary>
        /// <value>The list view view.</value>
        /// <remarks>Documented by Dev05, 2009-03-16</remarks>
        public View ListViewView { get; set; }
        /// <summary>
        /// Gets or sets the type of the order.
        /// </summary>
        /// <value>The type of the order.</value>
        /// <remarks>Documented by Dev05, 2009-03-16</remarks>
        public ItemOrderType OrderType { get; set; }
        /// <summary>
        /// Gets or sets the expando positions.
        /// </summary>
        /// <value>The expando positions.</value>
        /// <remarks>Documented by Dev05, 2009-03-16</remarks>
        public SerializableDictionary<int, int> ExpandoPositions { get; set; }
        /// <summary>
        /// Gets or sets the expando states.
        /// </summary>
        /// <value>The expando states.</value>
        /// <remarks>Documented by Dev05, 2009-03-16</remarks>
        public SerializableDictionary<int, bool> ExpandoStates { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LearningModulesPagePersitencyData"/> class.
        /// </summary>
        /// <remarks>Documented by Dev05, 2009-03-16</remarks>
        public LearningModulesPagePersitencyData()
        {
            HideXPExplorerBar = true;
            ListViewView = View.Tile;
            OrderType = ItemOrderType.Location;
            ExpandoPositions = new SerializableDictionary<int, int>();
            ExpandoStates = new SerializableDictionary<int, bool>();
        }
    }

    /// <summary>
    /// OrderType of the Items.
    /// </summary>
    public enum ItemOrderType
    {
        /// <summary>
        /// Order by location.
        /// </summary>
        Location,
        /// <summary>
        /// Order by category.
        /// </summary>
        Category,
        /// <summary>
        /// Order by author.
        /// </summary>
        Author
    }
}
