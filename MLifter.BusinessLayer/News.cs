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
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using System.IO;
using System.Diagnostics;

namespace MLifter.BusinessLayer
{
    public class News
    {
        XslCompiledTransform xslTransformer = new XslCompiledTransform();

        /// <summary>
        /// Initializes a new instance of the <see cref="News"/> class.
        /// </summary>
        /// <param name="RssFeedTransformer">The RSS feed transformer.</param>
        /// <remarks>Documented by Dev02, 2007-11-28</remarks>
        public News(string RssFeedTransformer)
        {
            XmlDocument rssFeedTransformer = new XmlDocument();
            XsltSettings settings = new XsltSettings(false, true);     //disable scripts and enable document()
            rssFeedTransformer.LoadXml(RssFeedTransformer);
            xslTransformer.Load(rssFeedTransformer.CreateNavigator(), settings, new XmlUrlResolver());
        }

        /// <summary>
        /// Gets the news feed.
        /// </summary>
        /// <param name="newsHtmlText">The news HTML text.</param>
        /// <param name="newsDate">The news date.</param>
        /// <param name="newNewsCount">The new news count.</param>
        /// <param name="newsFeedURL">The news feed URL.</param>
        /// <returns>
        /// A bool value indicating whether the news fetch was successful.
        /// </returns>
        /// <remarks>Documented by Dev02, 2009-03-12</remarks>
        public bool GetNewsFeed(out string newsHtmlText, ref DateTime newsDate, out int newNewsCount, string newsFeedURL)
        {
            return GetNewsFeed(out newsHtmlText, ref newsDate, out newNewsCount, newsFeedURL, -1);
        }

        /// <summary>
        /// Gets the news feed.
        /// </summary>
        /// <param name="newsHtmlText">The news HTML text.</param>
        /// <param name="newsDate">The news date.</param>
        /// <param name="newNewsCount">The new news count.</param>
        /// <param name="newsFeedURL">The news feed URL.</param>
        /// <param name="numberOfNews">The number of news, or a negative value for all.</param>
        /// <returns>
        /// A bool value indicating whether the news fetch was successful.
        /// </returns>
        /// <remarks>Documented by Dev03, 2008-12-02</remarks>
        public bool GetNewsFeed(out string newsHtmlText, ref DateTime newsDate, out int newNewsCount, string newsFeedURL, int numberOfNews)
        {
            newsHtmlText = String.Empty;
            List<DateTime> newsDates = new List<DateTime>();
            newNewsCount = 0;

            XmlDocument rssFeed = new XmlDocument();

            try
            {
                rssFeed.Load(newsFeedURL);

                foreach (XmlNode date in rssFeed.SelectNodes("//item/pubDate/text()"))
                    newsDates.Add(Convert.ToDateTime(date.InnerText));
                newsDates.Sort((x, y) => -x.CompareTo(y)); //sort with datetime descending

                newsHtmlText = TransformFeed(rssFeed, numberOfNews);

                foreach (DateTime current in newsDates)
                {
                    //the datetime from the settings is only exact to the minute
                    if (current < newsDate.AddMinutes(2))
                        break;
                    newNewsCount++;
                    if (newNewsCount >= numberOfNews)
                        break;
                }
                newsDate = newsDates[0];

                return true;
            }
            catch (Exception e)
            {
                Trace.WriteLine("Newsfeed fetching error: " + e.ToString());
                return false;
            }

        }

        /// <summary>
        /// Transforms the RSS feed xml to the HTML text.
        /// </summary>
        /// <param name="rssFeed">The RSS feed.</param>
        /// <param name="numberOfNews">The number of news.</param>
        /// <returns>The news HTML text.</returns>
        /// <remarks>Documented by Dev02, 2009-03-12</remarks>
        private string TransformFeed(XmlDocument rssFeed, int numberOfNews)
        {
            XsltArgumentList xsltArguments = new XsltArgumentList();
            xsltArguments.AddParam("displayNumber", String.Empty, numberOfNews > 0 ? numberOfNews : 100);

            StringBuilder htmlContent = new StringBuilder();
            StringWriter htmlWriter = new StringWriter(htmlContent);
            XmlTextWriter htmlTextWriter = new XmlTextWriter(htmlWriter);

            xslTransformer.Transform(rssFeed, xsltArguments, htmlTextWriter);

            return htmlContent.ToString();

        }
    }
}
