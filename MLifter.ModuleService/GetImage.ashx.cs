using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MLifter.Generics;
using System.IO;
using System.Xml.Serialization;
using MLifter.ModuleService.Properties;
using System.Drawing;

namespace MLifter.ModuleService
{
	/// <summary>
	/// Summary description for GetImage
	/// </summary>
	public class GetImage : IHttpHandler
	{
		XmlSerializer infoSerializer;

		/// <summary>
		/// Initializes a new instance of the <see cref="ModuleService"/> class.
		/// </summary>
		/// <remarks>Documented by Dev05, 2012-02-07</remarks>
		public GetImage()
		{
			infoSerializer = new XmlSerializer(typeof(ModuleInfo));
		}

		/// <summary>
		/// Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler"/> interface.
		/// </summary>
		/// <param name="context">An <see cref="T:System.Web.HttpContext"/> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests.</param>
		/// <remarks>Documented by Dev05, 2012-02-07</remarks>
		public void ProcessRequest(HttpContext context)
		{
			string module = HttpUtility.UrlDecode(HttpContext.Current.Request["module"]);
			int size = Convert.ToInt32(HttpContext.Current.Request["size"]);

			string xmlFile = Path.Combine(HttpContext.Current.Server.MapPath(Settings.Default.InfoFolder), HttpUtility.UrlDecode(module) + ".xml");

			if (!File.Exists(xmlFile)) //if there is no xml-info file check if there is a screenshot available
			{
				string imgFile = Path.Combine(HttpContext.Current.Server.MapPath(Settings.Default.ScreenshotFolder), HttpUtility.UrlDecode(module) + ".png");
				if (!File.Exists(imgFile))
					return;

				context.Response.ContentType = "image/png";
				byte[] img = ResizeImage(File.ReadAllBytes(imgFile), size);
				context.Response.OutputStream.Write(img, 0, img.Length);
				return;
			}

			Stream fStream = File.OpenRead(xmlFile);
			ModuleInfo info = (ModuleInfo)infoSerializer.Deserialize(fStream);
			fStream.Close();

			byte[] image;
			switch (size)
			{
				case 16:
					image = info.IconSmall;
					break;
				case 32:
					image = info.IconBig;
					break;
				default:
					image = info.Preview;
					break;
			}

			context.Response.ContentType = "image/png";
			context.Response.OutputStream.Write(image, 0, image.Length);
		}

		/// <summary>
		/// Resizes the image.
		/// </summary>
		/// <param name="image">The image.</param>
		/// <param name="sideLength">Length of the side.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2012-02-07</remarks>
		private byte[] ResizeImage(byte[] image, int sideLength)
		{
			using (Image sourceImage = Image.FromStream(new MemoryStream(image)))
			{
				int newWidth = sideLength;

				int newHeight = sourceImage.Height * newWidth / sourceImage.Width;
				if (newHeight > sideLength)
				{
					// Resize with height instead
					newWidth = sourceImage.Width * sideLength / sourceImage.Height;
					newHeight = sideLength;
				}

				using (Image targetImage = new Bitmap(newWidth, newHeight))
				{
					Graphics g = Graphics.FromImage(targetImage);
					g.Clear(Color.White);
					g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
					g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
					g.DrawImage(sourceImage, 0, 0, newWidth, newHeight);
					g.Flush();

					using (MemoryStream stream = new MemoryStream())
					{
						targetImage.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
						return stream.ToArray();
					}
				}
			}
		}

		/// <summary>
		/// Gets a value indicating whether another request can use the <see cref="T:System.Web.IHttpHandler"/> instance.
		/// </summary>
		/// <returns>true if the <see cref="T:System.Web.IHttpHandler"/> instance is reusable; otherwise, false.</returns>
		/// <remarks>Documented by Dev05, 2012-02-07</remarks>
		public bool IsReusable
		{
			get
			{
				return false;
			}
		}
	}
}