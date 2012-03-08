using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MLifter.Generics;

namespace MLifter.Components
{
	/// <summary>
	/// Represents a category read from a module feed.
	/// </summary>
	/// <remarks>Documented by Dev05, 2012-02-08</remarks>
	public class FeedCategoryTreeNode : TreeNode
	{
		private FeedTreeNode mainNode { get; set; }
		private ModuleCategory Category { get; set; }
		private ListViewGroup Group { get; set; }
		private List<ModuleInfo> ModuleBaseList { get; set; }

		/// <summary>
		/// Gets or sets the own modules.
		/// </summary>
		/// <value>The own modules.</value>
		/// <remarks>Documented by Dev05, 2009-08-11</remarks>
		public List<ListViewItem> OwnModules { get; set; }
		/// <summary>
		/// Gets or sets the own sub category modules.
		/// </summary>
		/// <value>The own sub category modules.</value>
		/// <remarks>Documented by Dev05, 2009-08-11</remarks>
		public List<ListViewItem> OwnSubCategoryModules { get; set; }
		/// <summary>
		/// Gets or sets the modules.
		/// </summary>
		/// <value>The modules.</value>
		/// <remarks>Documented by Dev05, 2009-06-26</remarks>
		public List<ListViewItem> Modules
		{
			get
			{
				List<ListViewItem> items = new List<ListViewItem>(OwnModules);
				foreach (TreeNode item in Nodes)
					if (item is FeedCategoryTreeNode)
						items.AddRange((item as FeedCategoryTreeNode).Modules);
				return items;
			}
		}
		/// <summary>
		/// Gets the sub category modules.
		/// </summary>
		/// <value>The sub category modules.</value>
		/// <remarks>Documented by Dev05, 2009-08-11</remarks>
		public List<ListViewItem> SubCategoryModules
		{
			get
			{
				List<ListViewItem> items = new List<ListViewItem>(OwnSubCategoryModules);
				foreach (TreeNode item in Nodes)
					if (item is FeedCategoryTreeNode)
						items.AddRange((item as FeedCategoryTreeNode).SubCategoryModules);
				return items;
			}
		}

		/// <summary>
		/// Gets the categories.
		/// </summary>
		/// <value>The categories.</value>
		/// <remarks>Documented by Dev05, 2009-08-11</remarks>
		public List<ModuleCategory> Categories
		{
			get
			{
				List<ModuleCategory> cats = new List<ModuleCategory>() { Category };

				foreach (TreeNode item in Nodes)
					if (item is FeedCategoryTreeNode)
						cats.AddRange((item as FeedCategoryTreeNode).Categories);

				return cats;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FeedCategoryTreeNode"/> class.
		/// </summary>
		/// <param name="category">The category.</param>
		/// <remarks>Documented by Dev05, 2009-06-26</remarks>
		public FeedCategoryTreeNode(ModuleCategory category, FeedTreeNode mainNode)
			: base()
		{
			OwnModules = new List<ListViewItem>();
			OwnSubCategoryModules = new List<ListViewItem>();

			this.mainNode = mainNode;
			Category = category;
			Group = new ListViewGroup(category.Title);

			Text = category.Title;
			ImageIndex = 1;
			SelectedImageIndex = 1;
		}

		/// <summary>
		/// Sets the modules list.
		/// </summary>
		/// <param name="modules">The modules.</param>
		/// <remarks>Documented by Dev05, 2009-06-26</remarks>
		public void SetModuleList(List<ModuleInfo> modules)
		{
			ModuleBaseList = modules;

			foreach (ModuleInfo info in modules)
			{
				ListViewItem item = new ListViewItem(Group);
				while (item.SubItems.Count < 6) item.SubItems.Add(new ListViewItem.ListViewSubItem());
				item.SubItems[0].Text = info.Title;
				item.SubItems[1].Text = info.Author;
				item.SubItems[2].Text = Categories.Find(c => c.Id == Convert.ToInt32(info.Categories[0])).Title;
				item.SubItems[3].Text = info.Cards.ToString();
				item.SubItems[4].Text = Methods.GetFileSize(info.Size);
				item.Tag = info;
				item.ImageIndex = 0;
				OwnModules.Add(item);

				foreach (string catString in info.Categories.ToArray())
				{
					int cat = Convert.ToInt32(catString);
					if (mainNode.CategoryNodes.ContainsKey(cat))
					{
						ListViewItem cloneItem = item.Clone() as ListViewItem;
						cloneItem.SubItems[2].Text = Categories.Find(c => c.Id == cat).Title;
						mainNode.CategoryNodes[cat].OwnSubCategoryModules.Add(cloneItem);
					}
				}
			}
		}
	}
}
