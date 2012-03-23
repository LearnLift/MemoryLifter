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

namespace MLifter.DAL.XML
{
    internal class XmlHelper
    {
        private XmlHelper()
        {
        }

        /// <summary>
        /// Sets the value of a specific node.
        /// </summary>
        /// <param name="doc">XML document</param>
        /// <param name="xpath">An XPath query string</param>
        /// <param name="content">The new value of the node</param>
        /// <remarks>Documented by Dev03, 2007-07-30</remarks>
        public static void SetXmlString(XmlDocument doc, string xpath, string content)
        {
            doc.SelectSingleNode(xpath).InnerText = content;
        }

        /// <summary>
        /// Sets the value for a specific node.
        /// </summary>
        /// <param name="doc">XML document</param>
        /// <param name="parent_node">Name of the parent node</param>
        /// <param name="node_name">Node name</param>
        /// <param name="content">The new value of the node</param>
        /// <remarks>Documented by Dev03, 2007-07-30</remarks>
        public static void SetXmlString(XmlDocument doc, XmlNode parent_node, string node_name, string content)
        {
            XmlNode temp_node = parent_node.SelectSingleNode(node_name);
            if (temp_node == null)
            {
                temp_node = doc.CreateElement(node_name);
                parent_node.AppendChild(temp_node);
            }
            temp_node.InnerText = content;
        }

        /// <summary>
        /// Sets the value of a specific node.
        /// Calls SetXmlString(xpath, XmlConvert.ToString(content)).
        /// </summary>
        /// <param name="doc">XML document</param>
        /// <param name="xpath">An XPath query string</param>
        /// <param name="content">The new value of the node</param>
        /// <remarks>Documented by Dev03, 2007-07-30</remarks>
        public static void SetXmlInt32(XmlDocument doc, string xpath, int content)
        {
            SetXmlString(doc, xpath, XmlConvert.ToString(content));
#if DEBUG
            XmlElement configNode = (XmlElement)doc.SelectSingleNode(xpath);
            configNode.SetAttribute("bitmask", XmlConvert.ToInt32(configNode.InnerText).ToString("X"));
#endif
        }

        /// <summary>
        /// Sets the value for a specific node. Calls SetXmlString.
        /// Calls SetXmlString(parent_node, node_name, XmlConvert.ToString(content)).
        /// </summary>
        /// <param name="doc">XML document</param>
        /// <param name="parent_node">Name of the parent node</param>
        /// <param name="node_name">Node name</param>
        /// <param name="content">The new value of the node</param>
        /// <remarks>Documented by Dev03, 2007-07-30</remarks>
        public static void SetXmlInt32(XmlDocument doc, XmlNode parent_node, string node_name, int content)
        {
            SetXmlString(doc, parent_node, node_name, XmlConvert.ToString(content));
#if DEBUG
            XmlElement configNode = (XmlElement)parent_node.SelectSingleNode(node_name);
            configNode.SetAttribute("bitmask", XmlConvert.ToInt32(configNode.InnerText).ToString("X"));
#endif
        }

        /// <summary>
        /// Creates a new XML element along with an attribute and appends it to a parent note.
        /// </summary>
        /// <param name="document">XML document</param>
        /// <param name="xpath_parent">XPath query to localize the parent node</param>
        /// <param name="node_name">Name of the new node</param>
        /// <param name="attribute_name">Name of the attribute</param>
        /// <param name="attribute_value">Value of the attribute</param>
        /// <returns>New XML node</returns>
        /// <remarks>Documented by Dev03, 2007-07-30</remarks>
        public static XmlNode InsertXmlNodeWithAttribute(XmlDocument document, string xpath_parent, string node_name, string attribute_name, string attribute_value)
        {
            XmlNode parent_node = document.SelectSingleNode(xpath_parent);
            XmlElement temp_node = document.CreateElement(node_name);
            parent_node.AppendChild(temp_node);
            temp_node.SetAttribute(attribute_name, attribute_value);
            return temp_node;
        }

        /// <summary>
        /// Creates a new XML element along with an attribute and appends it to a parent note.
        /// </summary>
        /// <param name="parent">Parent node</param>
        /// <param name="node_name">Name of the new node</param>
        /// <param name="attribute_name">Name of the attribute</param>
        /// <returns>New XML node</returns>
        /// <remarks>Documented by Dev03, 2007-07-30</remarks>
        public static XmlElement CreateElementWithAttribute(XmlElement parent, string node_name, string attribute_name)
        {
            return CreateElementWithAttribute(parent, node_name, String.Empty, attribute_name, String.Empty);
        }

        /// <summary>
        /// Creates a new XML element along with an attribute and appends it to a parent note.
        /// </summary>
        /// <param name="parent">Parent node</param>
        /// <param name="node_name">Name of the new node</param>
        /// <param name="node_value">Value of the new node</param>
        /// <param name="attribute_name">Name of the attribute</param>
        /// <param name="attribute_value">Value of the attribute</param>
        /// <returns>New XML node</returns>
        /// <remarks>Documented by Dev03, 2007-07-30</remarks>
        public static XmlElement CreateElementWithAttribute(XmlElement parent, string node_name, string node_value, string attribute_name, string attribute_value)
        {
            XmlElement temp_node = parent.OwnerDocument.CreateElement(node_name);
            temp_node.InnerText = node_value;
            temp_node.SetAttribute(attribute_name, attribute_value);
            parent.AppendChild(temp_node);
            return temp_node;
        }

        /// <summary>
        /// Creates a new element node and appends it to a given parent node.
        /// </summary>
        /// <param name="parent">Parent element node</param>
        /// <param name="name">New element name</param>
        /// <returns>New element node</returns>
        /// <remarks>Documented by Dev03, 2007-08-01</remarks>
        public static XmlElement CreateAndAppendElement(XmlElement parent, string name)
        {
            return CreateAndAppendElement(parent, name, String.Empty);
        }

        /// <summary>
        /// Creates a new element node and appends it to a given parent node.
        /// </summary>
        /// <param name="parent">Parent element node</param>
        /// <param name="name">Element node name</param>
        /// <param name="value">Element node value</param>
        /// <returns>New element node</returns>
        /// <remarks>Documented by Dev03, 2007-08-01</remarks>
        public static XmlElement CreateAndAppendElement(XmlElement parent, string name, string value)
        {
            XmlElement xElement = parent.OwnerDocument.CreateElement(name);
            if (value != String.Empty)
                xElement.InnerText = value;
            parent.AppendChild(xElement);
            return xElement;
        }

        public static XmlAttribute CreateAndAppendAttribute(XmlElement parent, string name)
        {
            return CreateAndAppendAttribute(parent, name, String.Empty);
        }

        public static XmlAttribute CreateAndAppendAttribute(XmlElement parent, string name, string value)
        {
            XmlAttribute xAttribute = parent.OwnerDocument.CreateAttribute(name);
            if (value != String.Empty)
                xAttribute.Value = value;
            parent.SetAttributeNode(xAttribute);
            return xAttribute;
        }

    }
}
