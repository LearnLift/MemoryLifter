using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using MLifter.Generics;
using System.IO;
using MLifter.ModuleService.Properties;

namespace MLifter.ModuleService
{
	/// <summary>
	/// Summary description for ModuleService
	/// </summary>
	[WebService(Namespace = "http://services.memorylifter.com/",
		Description = "This service provides the necessary feeds to list modules inside MemoryLifter >2.3.")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.ComponentModel.ToolboxItem(false)]
	public class ModuleService : System.Web.Services.WebService
	{
		XmlSerializer infoSerializer;
		XmlSerializer categoriesSerializer;

		XmlDocument modulesDocument;
		XmlDocument categoriesDocument;

		SerializableList<ModuleCategory> categories;

		/// <summary>
		/// Initializes a new instance of the <see cref="ModuleService"/> class.
		/// </summary>
		/// <remarks>Documented by Dev05, 2012-02-07</remarks>
		public ModuleService()
		{
			infoSerializer = new XmlSerializer(typeof(ModuleInfo));
			categoriesSerializer = new XmlSerializer(typeof(SerializableList<ModuleCategory>));

			categoriesDocument = GetCategoriesDocument();
			modulesDocument = GetModulesDocument();

			FileSystemWatcher moduleWatcher = new FileSystemWatcher(Server.MapPath(Settings.Default.ModulesFolder), "*.mlm");
			moduleWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName;
			moduleWatcher.Changed += new FileSystemEventHandler(Watcher_Changed);
			moduleWatcher.Created += new FileSystemEventHandler(Watcher_Changed);
			moduleWatcher.Deleted += new FileSystemEventHandler(Watcher_Changed);
			moduleWatcher.Renamed += new RenamedEventHandler(Watcher_Renamed);
			moduleWatcher.EnableRaisingEvents = true;

			FileSystemWatcher infoWatcher = new FileSystemWatcher(Server.MapPath(Settings.Default.InfoFolder), "*.xml");
			infoWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName;
			infoWatcher.Changed += new FileSystemEventHandler(Watcher_Changed);
			infoWatcher.Created += new FileSystemEventHandler(Watcher_Changed);
			infoWatcher.Deleted += new FileSystemEventHandler(Watcher_Changed);
			infoWatcher.Renamed += new RenamedEventHandler(Watcher_Renamed);
			infoWatcher.EnableRaisingEvents = true;

			FileSystemWatcher screenshotWatcher = new FileSystemWatcher(Server.MapPath(Settings.Default.ScreenshotFolder), "*.png");
			screenshotWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName;
			screenshotWatcher.Changed += new FileSystemEventHandler(Watcher_Changed);
			screenshotWatcher.Created += new FileSystemEventHandler(Watcher_Changed);
			screenshotWatcher.Deleted += new FileSystemEventHandler(Watcher_Changed);
			screenshotWatcher.Renamed += new RenamedEventHandler(Watcher_Renamed);
			screenshotWatcher.EnableRaisingEvents = true;
		}

		/// <summary>
		/// Handles the Renamed event of the Watcher control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.IO.RenamedEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2012-02-07</remarks>
		protected void Watcher_Renamed(object sender, RenamedEventArgs e)
		{
			categoriesDocument = GetCategoriesDocument();
			modulesDocument = GetModulesDocument();
		}
		/// <summary>
		/// Handles the Changed event of the Watcher control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.IO.FileSystemEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2012-02-07</remarks>
		protected void Watcher_Changed(object sender, FileSystemEventArgs e)
		{
			categoriesDocument = GetCategoriesDocument();
			modulesDocument = GetModulesDocument();
		}

		/// <summary>
		/// Gets the base address of this service.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2012-02-07</remarks>
		public string BaseAddress
		{
			get
			{
				string baseAddress = HttpContext.Current.Request.Url.AbsoluteUri
					.Substring(0, HttpContext.Current.Request.Url.AbsoluteUri.LastIndexOf('/'));
				return baseAddress.Substring(0, baseAddress.LastIndexOf('/') + 1);
			}
		}

		/// <summary>
		/// Lists all Categories on this server.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2012-02-07</remarks>
		[WebMethod]
		public XmlDocument Categories() { return categoriesDocument; }

		/// <summary>
		/// Lists all the modules on this server.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2012-02-07</remarks>
		[WebMethod]
		public XmlDocument Modules() { return modulesDocument; }

		/// <summary>
		/// Gets the modules document.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2012-02-07</remarks>
		private XmlDocument GetModulesDocument()
		{
			SyndicationFeed feed = new SyndicationFeed();
			feed.Id = Settings.Default.ModuleFeedId.ToString();
			feed.Title = new TextSyndicationContent("MemoryLifter Modules", TextSyndicationContentKind.Plaintext);
			feed.Description = new TextSyndicationContent("Lists all modules available.", TextSyndicationContentKind.Plaintext);
			feed.LastUpdatedTime = new DateTimeOffset(DateTime.Now);

			feed.Authors.Add(new SyndicationPerson("support@memorylifter.com", "OMICRON", "http://www.memorylifter.com"));
			List<SyndicationItem> items = new List<SyndicationItem>();
			feed.Items = items;

			foreach (string file in Directory.GetFiles(Server.MapPath(Settings.Default.ModulesFolder), "*.mlm", SearchOption.AllDirectories))
			{
				SyndicationItem item = new SyndicationItem();
				items.Add(item);

				string filename = Path.GetFileNameWithoutExtension(file);
				string xmlFile = Path.Combine(Server.MapPath(Settings.Default.InfoFolder), filename + ".xml");

				long size = 0;
				if (File.Exists(xmlFile)) //if there is a xml-info-file use the information from it
				{
					Stream fStream = File.OpenRead(xmlFile);
					ModuleInfo info = (ModuleInfo)infoSerializer.Deserialize(fStream);
					fStream.Close();

					item.Id = info.Id;
					item.Title = new TextSyndicationContent(info.Title, TextSyndicationContentKind.Plaintext);
					item.Content = new TextSyndicationContent(info.Description, TextSyndicationContentKind.Plaintext); //This is also shown in feed readers as text
					//As the summary we use a struct which can be deserialized to a ModuleInfo-struct
					item.Summary = new TextSyndicationContent("<ModuleInfo><Cards>" + info.Cards + "</Cards></ModuleInfo>", TextSyndicationContentKind.XHtml);
					foreach (string category in info.Categories)
					{
						ModuleCategory cat = (from c in categories
											  where c.Id.ToString() == category
											  select c).FirstOrDefault();
						if(cat.Id > 0) //if the stored category is actually an Id to a category
							item.Categories.Add(new SyndicationCategory(cat.Title) { Label = category });
						else
							item.Categories.Add(new SyndicationCategory(category));
					}
					item.Contributors.Add(new SyndicationPerson(info.AuthorMail, info.Author, info.AuthorUrl));
					DateTime time;
					if (DateTime.TryParse(info.EditDate, out time))
						item.LastUpdatedTime = new DateTimeOffset(time);
					else
						item.LastUpdatedTime = new DateTimeOffset((new FileInfo(file)).LastWriteTime);
					size = info.Size;
				}
				else // use the information you can get from the file - no SQL CE access on Mono --> if running on IIS/.net you could read it form the file
				{
					item.Id = file.GetHashCode().ToString();
					item.Title = new TextSyndicationContent(filename, TextSyndicationContentKind.Plaintext);
					item.LastUpdatedTime = new DateTimeOffset((new FileInfo(file)).LastWriteTime);
				}
				if (size <= 0)
					size = (new FileInfo(file)).Length;

				item.Links.Add(new SyndicationLink(new Uri(BaseAddress + Settings.Default.ModulesFolder + '/' + Uri.EscapeDataString(Path.GetFileName(file))))
				{
					MediaType = "application/x-mlm",
					RelationshipType = AtomLinkRelationshipType.Module.ToString(),
					Length = size
				});
				item.Links.Add(new SyndicationLink(new Uri(BaseAddress + "GetImage.ashx?size=150&module=" + HttpUtility.UrlEncode(filename)))
				{
					MediaType = "image/png",
					RelationshipType = AtomLinkRelationshipType.Preview.ToString()
				});
				item.Links.Add(new SyndicationLink(new Uri(BaseAddress + "GetImage.ashx?size=32&module=" + HttpUtility.UrlEncode(filename)))
				{
					MediaType = "image/png",
					RelationshipType = AtomLinkRelationshipType.IconBig.ToString()
				});
				item.Links.Add(new SyndicationLink(new Uri(BaseAddress + "GetImage.ashx?size=16&module=" + HttpUtility.UrlEncode(filename)))
				{
					MediaType = "image/png",
					RelationshipType = AtomLinkRelationshipType.IconSmall.ToString()
				});
			}

			StringBuilder result = new StringBuilder();
			XmlWriter writer = XmlWriter.Create(result);
			feed.GetAtom10Formatter().WriteTo(writer);
			writer.Flush();

			XmlDocument doc = new XmlDocument();
			doc.LoadXml(result.ToString());
			return doc;
		}
		/// <summary>
		/// Gets the categories document.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2012-02-07</remarks>
		private XmlDocument GetCategoriesDocument()
		{
			SyndicationFeed feed = new SyndicationFeed();
			feed.Id = Settings.Default.CategoriesFeedId.ToString();
			feed.Title = new TextSyndicationContent("MemoryLifter Module Categories", TextSyndicationContentKind.Plaintext);
			feed.Description = new TextSyndicationContent("Lists all categories with modules available.", TextSyndicationContentKind.Plaintext);
			feed.LastUpdatedTime = new DateTimeOffset(DateTime.Now);

			feed.Authors.Add(new SyndicationPerson("support@memorylifter.com", "OMICRON", "http://www.memorylifter.com"));
			List<SyndicationItem> items = new List<SyndicationItem>();
			feed.Items = items;

			string filename = Server.MapPath(Path.Combine(Settings.Default.InfoFolder, "categories.xml"));
			if (File.Exists(filename))
			{
				Stream file = File.OpenRead(filename);
				categories = categoriesSerializer.Deserialize(file) as SerializableList<ModuleCategory>;
				file.Close();
				
				foreach (ModuleCategory category in categories)
				{
					SyndicationItem item = new SyndicationItem();
					item.Id = category.Id.ToString();
					item.Title = new TextSyndicationContent(category.Title, TextSyndicationContentKind.Plaintext);
					item.Links.Add(new SyndicationLink() { RelationshipType = AtomLinkRelationshipType.Parent.ToString(), Title = category.ParentCategory.ToString() });
					items.Add(item);
				}
			}

			StringBuilder result = new StringBuilder();
			XmlWriter writer = XmlWriter.Create(result);
			feed.GetAtom10Formatter().WriteTo(writer);
			writer.Flush();

			XmlDocument doc = new XmlDocument();
			doc.LoadXml(result.ToString());
			return doc;
		}
	}
}
