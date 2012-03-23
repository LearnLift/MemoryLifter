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
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using MLifter.Generics;
using MLifter.Components.Properties;
using System.Net;
using System.Xml.Serialization;
using System.ServiceModel.Syndication;
using System.Xml;
using System.Threading;
using MLifter.BusinessLayer;

namespace MLifter.Components
{
	public class FeedTreeNode : TreeNode
	{
		private Thread loadingThread = null;

		/// <summary>
		/// Gets the feed connection.
		/// </summary>
		/// <remarks>CFI, 2012-03-03</remarks>
		public ModuleFeedConnection Feed { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="FeedTreeNode"/> class.
		/// </summary>
		/// <remarks>Documented by Dev05, 2009-06-26</remarks>
		public FeedTreeNode(ModuleFeedConnection feed)
			: base()
		{
			Feed = feed;
			Text = String.IsNullOrWhiteSpace(feed.Name) ? Resources.FeedTreeNode_Name : feed.Name;
			ImageIndex = 7;
			SelectedImageIndex = 7;
		}

		/// <summary>
		/// Gets a value indicating whether this instance is loading.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is loading; otherwise, <c>false</c>.
		/// </value>
		/// <remarks>Documented by Dev05, 2009-06-26</remarks>
		public bool IsLoading { get { return loadingThread != null && loadingThread.ThreadState == System.Threading.ThreadState.Running; } }
		/// <summary>
		/// Gets or sets a value indicating whether this instance is list loaded.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is list loaded; otherwise, <c>false</c>.
		/// </value>
		/// <remarks>Documented by Dev05, 2009-06-29</remarks>
		public bool IsListLoaded { get; private set; }
		/// <summary>
		/// Gets or sets a value indicating whether this instance is loaded.
		/// </summary>
		/// <value><c>true</c> if this instance is loaded; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev05, 2009-06-26</remarks>
		public bool IsLoaded { get; private set; }

		/// <summary>
		/// Gets the category nodes.
		/// </summary>
		/// <value>The category nodes.</value>
		/// <remarks>Documented by Dev05, 2009-08-11</remarks>
		internal Dictionary<int, FeedCategoryTreeNode> CategoryNodes { get { return nodes; } }

		/// <summary>
		/// Gets the modules.
		/// </summary>
		/// <value>The modules.</value>
		/// <remarks>Documented by Dev05, 2009-06-26</remarks>
		public List<ListViewItem> Modules
		{
			get
			{
				List<ListViewItem> list = new List<ListViewItem>();
				foreach (TreeNode node in Nodes)
					list.AddRange((node as FeedCategoryTreeNode).Modules);
				return list;
			}
		}

		/// <summary>
		/// Occurs when content loaded.
		/// </summary>
		/// <remarks>Documented by Dev05, 2009-06-26</remarks>
		public event EventHandler ContentLoaded;
		/// <summary>
		/// Raises the <see cref="E:ContentLoaded"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2009-06-26</remarks>
		protected virtual void OnContentLoaded(EventArgs e)
		{
			if (ContentLoaded != null)
				ContentLoaded(this, e);
		}

		/// <summary>
		/// Begins the load of the web content.
		/// </summary>
		/// <remarks>Documented by Dev05, 2009-06-26</remarks>
		public void BeginLoadWebContent(string userConfigurationPath)
		{
			AbortLoadWebContent();

			loadingThread = new Thread(new ParameterizedThreadStart(LoadWebContent));
			loadingThread.CurrentCulture = Thread.CurrentThread.CurrentCulture;
			loadingThread.CurrentUICulture = Thread.CurrentThread.CurrentUICulture;
			loadingThread.Name = "Loading Shop-Content Thread";
			loadingThread.IsBackground = true;
			loadingThread.Priority = ThreadPriority.Lowest;
			loadingThread.Start(userConfigurationPath);
		}

		List<ModuleCategory> categories = new List<ModuleCategory>();
		/// <summary>
		/// Gets the categories.
		/// </summary>
		/// <remarks>CFI, 2012-03-03</remarks>
		public List<ModuleCategory> Categories { get { return categories; } }

		Dictionary<int, FeedCategoryTreeNode> nodes;
		/// <summary>
		/// Loads the content of the web.
		/// </summary>
		/// <remarks>Documented by Dev05, 2009-06-26</remarks>
		public void LoadWebContent(object path)
		{
			IsListLoaded = false;
			IsLoaded = false;

			string configPath = path as string;

			nodes = new Dictionary<int, FeedCategoryTreeNode>();
			if (TreeView.InvokeRequired)
				TreeView.Invoke((MethodInvoker)delegate { Nodes.Clear(); });
			else
				Nodes.Clear();

			try
			{
				if (TreeView.InvokeRequired)
					TreeView.Invoke((MethodInvoker)delegate
					{
						Text = String.IsNullOrWhiteSpace(Feed.Name) ? Resources.FeedTreeNode_Name : Feed.Name;
					});
				else
					Text = String.IsNullOrWhiteSpace(Feed.Name) ? Resources.FeedTreeNode_Name : Feed.Name;

				#region read categories

				XmlReader categoryFeedReader = XmlReader.Create(Feed.CategoriesUri);
				SyndicationFeed categoryFeed = SyndicationFeed.Load(categoryFeedReader);

				List<ModuleCategory> categoriesToAdd = new List<ModuleCategory>();
				foreach (SyndicationItem item in categoryFeed.Items)
				{
					ModuleCategory category = new ModuleCategory()
					{
						Id = Convert.ToInt32(item.Id),
						Title = item.Title.Text,
						ParentCategory = Convert.ToInt32(item.Links[0].Title)
					};
					categoriesToAdd.Add(category);
				}
				categoriesToAdd.Sort((a, b) => a.Id.CompareTo(b.Id));
				categoriesToAdd.ForEach(c => AddCategoryNode(c));

				#endregion

				#region read modules

				XmlReader moduleFeedReader = XmlReader.Create(Feed.ModulesUri);
				SyndicationFeed moduleFeed = SyndicationFeed.Load(moduleFeedReader);

				List<ModuleInfo> modules = new List<ModuleInfo>();
				XmlSerializer infoSerializer = new XmlSerializer(typeof(ModuleInfo));
				Dictionary<string, SyndicationItem> items = new Dictionary<string, SyndicationItem>();
				foreach (SyndicationItem item in moduleFeed.Items)
				{
					ModuleInfo info;
					if (item.Summary != null)
						info = (ModuleInfo)infoSerializer.Deserialize(XmlReader.Create(new StringReader(WebUtility.HtmlDecode(item.Summary.Text))));
					else
						info = new ModuleInfo();
					info.Id = item.Id;
					info.Title = item.Title.Text;
					info.EditDate = item.LastUpdatedTime.ToString();
					if (item.Contributors.Count > 0)
					{
						info.Author = item.Contributors[0].Name;
						info.AuthorMail = item.Contributors[0].Email;
						info.AuthorUrl = item.Contributors[0].Uri;
					}
					if (item.Content is TextSyndicationContent)
						info.Description = (item.Content as TextSyndicationContent).Text;
					info.Categories = new SerializableList<string>();
					foreach (SyndicationCategory cat in item.Categories)
						info.Categories.Add(cat.Label);
					foreach (SyndicationLink link in item.Links)
					{
						if (link.RelationshipType == AtomLinkRelationshipType.Module.ToString())
						{
							info.Size = link.Length;
							info.DownloadUrl = link.Uri.AbsoluteUri;
						}
					}
					modules.Add(info);
					items.Add(info.Id, item);
				}

				categories.ForEach(c => nodes[c.Id].SetModuleList(modules.FindAll(p => p.Categories.Contains(c.Id.ToString()))));

				#endregion

				if (TreeView.InvokeRequired)
					TreeView.Invoke((MethodInvoker)delegate { Expand(); });
				else
					Expand();
				IsListLoaded = true;

				OnContentLoaded(EventArgs.Empty);

				#region read images

				using (PersistentMemoryCache<ModuleInfoCacheItem> cache = new PersistentMemoryCache<ModuleInfoCacheItem>("Feed_" + moduleFeed.Id))
				{
					WebClient webClient = new WebClient();
					foreach (ModuleInfo basicInfo in modules)
					{
						try
						{
							ModuleInfo info = basicInfo;
							SyndicationItem item = items[basicInfo.Id];
							string cacheKey = String.Format("{0}##{1}##{2}", moduleFeed.Id, item.Id, item.LastUpdatedTime);

							if (cache.Contains(cacheKey))
							{
								ModuleInfoCacheItem cacheItem = (ModuleInfoCacheItem)cache[cacheKey];
								info.IconSmall = Convert.FromBase64String(cacheItem.IconSmall);
								info.IconBig = Convert.FromBase64String(cacheItem.IconBig);
								info.Preview = Convert.FromBase64String(cacheItem.Preview);
							}
							else
							{
								ModuleInfoCacheItem cacheItem = new ModuleInfoCacheItem(info.Id);
								foreach (SyndicationLink link in item.Links)
								{
									if (link.RelationshipType == AtomLinkRelationshipType.IconSmall.ToString())
										cacheItem.IconSmall = Convert.ToBase64String(info.IconSmall = webClient.DownloadData(link.Uri));
									if (link.RelationshipType == AtomLinkRelationshipType.IconBig.ToString())
										cacheItem.IconBig = Convert.ToBase64String(info.IconBig = webClient.DownloadData(link.Uri));
									if (link.RelationshipType == AtomLinkRelationshipType.Preview.ToString())
										cacheItem.Preview = Convert.ToBase64String(info.Preview = webClient.DownloadData(link.Uri));
								}
								cache.Set(cacheKey, cacheItem, DateTime.Now.AddYears(1));
							}

							if (TreeView.InvokeRequired)
								TreeView.Invoke((MethodInvoker)delegate { LoadDetails(info); });
							else
								LoadDetails(info);
						}
						catch (Exception exp) { Trace.WriteLine(exp.ToString()); }
					}
					cache.Dispose();
				}

				#endregion

				IsLoaded = true;
			}
			catch (Exception)
			{
				try
				{
					if (TreeView.InvokeRequired)
						TreeView.Invoke((MethodInvoker)delegate { Text = Resources.FeedTreeNode_Text_Offline; });
					else
						Text = Resources.FeedTreeNode_Text_Offline;
				}
				catch (ObjectDisposedException) { }
			}
		}

		private void AddCategoryNode(ModuleCategory category)
		{
			if (nodes.ContainsKey(category.Id))
				return;

			FeedCategoryTreeNode node = new FeedCategoryTreeNode(category, this);
			nodes.Add(category.Id, node);
			if (category.ParentCategory <= 0)
			{
				if (TreeView.InvokeRequired)
					TreeView.Invoke((MethodInvoker)delegate { Nodes.Add(node); });
				else
				{
					Nodes.Add(node);
				}
			}
			else
			{
				if (TreeView.InvokeRequired)
					TreeView.Invoke((MethodInvoker)delegate { nodes[category.ParentCategory].Nodes.Add(node); });
				else
					nodes[category.ParentCategory].Nodes.Add(node);
			}
			categories.Add(category);
		}
		private void LoadDetails(ModuleInfo details)
		{
			try
			{
				foreach (string category in details.Categories)
				{
					int cat = Convert.ToInt32(category);
					if (nodes.ContainsKey(cat))
						LoadDetailsIntoItem(details, nodes[cat].OwnModules.Find(p => ((ModuleInfo)p.Tag).Id == details.Id), cat);
				}
			}
			catch (Exception e) { Trace.WriteLine(e.ToString()); }
		}

		private void LoadDetailsIntoItem(ModuleInfo details, ListViewItem item, int category)
		{
			try
			{
				item.SubItems[0].Text = details.Title;
				item.SubItems[1].Text = details.Author;
				item.SubItems[2].Text = categories.Find(c => c.Id == category).Title;
				item.SubItems[3].Text = details.Cards.ToString();
				item.SubItems[4].Text = Methods.GetFileSize(details.Size);
				item.Tag = details;

				if (item.ListView == null)
					return;

				if (item.ListView.Tag is bool && (bool)item.ListView.Tag)
				{
					item.Group = null;

					foreach (ListViewGroup group in item.ListView.Groups)
					{
						if (group.Header == details.Author)
						{
							item.Group = group;
							break;
						}
					}

					if (item.Group == null)
					{
						ListViewGroup grp = new ListViewGroup(details.Author);
						item.ListView.Groups.Add(grp);
						item.Group = grp;
					}
				}

				if (details.IconSmall.Length > 0)
				{
					item.ListView.SmallImageList.Images.Add(Image.FromStream(new MemoryStream(details.IconSmall)));
					item.ImageIndex = item.ListView.LargeImageList.Images.Count - 1;
				}
				if (details.IconBig.Length > 0)
				{
					item.ListView.LargeImageList.Images.Add(Image.FromStream(new MemoryStream(details.IconBig)));
					item.ImageIndex = item.ListView.LargeImageList.Images.Count - 1;
				}
			}
			catch (Exception e) { Trace.WriteLine(e.ToString()); }
		}

		/// <summary>
		/// Aborts the content of the load web.
		/// </summary>
		/// <remarks>Documented by Dev05, 2009-06-26</remarks>
		public void AbortLoadWebContent()
		{
			if (loadingThread != null && loadingThread.ThreadState == System.Threading.ThreadState.Running)
				loadingThread.Abort();
		}
	}
}
