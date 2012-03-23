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
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Xml.Xsl;
using System.Threading;
using System.Diagnostics;

namespace MLifter.Components
{
    /// <summary>
    /// This class handles the different styles of the MLifer UI.
    /// </summary>
    /// <remarks>Documented by Dev05, 2007-08-10</remarks>
    public class StyleHandler
    {
        public readonly string StylesPath;
        private static string m_styleExtension = string.Empty;
        private static string m_defaultStyleName = string.Empty;

        private static XmlSerializer serializer = null;
        private static System.Xml.Xsl.XslCompiledTransform versionTransform10_11 = null;

        private static Thread stylePrepareThread = null;

        /// <summary>
        /// Begins the style preparation.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-09-12</remarks>
        public static void BeginStylePreparation(string stylesheetPath)
        {
            stylePrepareThread = new Thread(delegate()
            {
                try
                {
                    Debug.WriteLine("Style preparation thread started.");
                    serializer = new XmlSerializer(typeof(Style));
                    versionTransform10_11 = new System.Xml.Xsl.XslCompiledTransform();
                    XsltSettings settings = new XsltSettings(true, false); //enable document() function
                    versionTransform10_11.Load(Path.Combine(stylesheetPath, @"System\Transformer\versionTransform10_11.xsl"), settings, null);
                    Debug.WriteLine("Style preparation thread finished successfully.");
                }
                catch (Exception exp)
                {
                    Trace.WriteLine("Style preparation thread crashed: " + exp.ToString());
                }
            });
            stylePrepareThread.IsBackground = true;
            stylePrepareThread.Name = "Style Preparation Thread";
            stylePrepareThread.CurrentCulture = Thread.CurrentThread.CurrentCulture;
            stylePrepareThread.CurrentUICulture = Thread.CurrentThread.CurrentUICulture;
            stylePrepareThread.Start();
        }

        /// <summary>
        /// Waits until the style preparation is finished.
        /// </summary>
        /// <param name="stylesheetPath">The stylesheet path.</param>
        /// <param name="reinitializeOnError">if set to <c>true</c> [reinitialize on error].</param>
        /// <remarks>Documented by Dev02, 2008-09-12</remarks>
        private static void AwaitStylePreparation(string stylesheetPath, bool reinitializeOnError)
        {
            if (stylePrepareThread != null && stylePrepareThread.IsAlive)
            {
                //wait for the preparation thread to finish
                while (stylePrepareThread.IsAlive)
                    stylePrepareThread.Join(20);
            }
            else if (reinitializeOnError && (serializer == null || versionTransform10_11 == null))
            {
                Trace.WriteLine("Warning: The styles are not prepared. Restarting the preparation thread.");
                BeginStylePreparation(stylesheetPath);
                AwaitStylePreparation(stylesheetPath, false);
            }
        }

        /// <summary>
        /// The name of the currently selected Style.
        /// </summary>
        public string CurrentStyleName = m_defaultStyleName;
        /// <summary>
        /// Dictionary containing all available styles.
        /// </summary>
        public Dictionary<string, Style> Styles = new Dictionary<string, Style>();
        /// <summary>
        /// The MainForm ResourceManager.
        /// </summary>
        System.Resources.ResourceManager ResourceManager;

        /// <summary>
        /// Gets the current style.
        /// </summary>
        /// <value>The current style or null, if not found.</value>
        /// <remarks>Documented by Dev05, 2007-08-10</remarks>
        public Style CurrentStyle
        {
            get
            {
                if (!Styles.ContainsKey(CurrentStyleName))
                    CurrentStyleName = m_defaultStyleName;

                if (Styles.ContainsKey(CurrentStyleName))
                    return Styles[CurrentStyleName];
                else
                    return null; //in case the default style does not exist
            }
        }

        /// <summary>
        /// Initializes arrayList new instance of the <see cref="styleHandler"/> class.
        /// </summary>
        /// <param name="stylesPath">The styles path.</param>
        /// <param name="appdataFolderDesigns">The appdata folder designs.</param>
        /// <remarks>Documented by Dev05, 2007-08-10</remarks>
        public StyleHandler(string stylesPath, string appdataFolderDesigns, System.Resources.ResourceManager resourceManager, bool runFromStick)
        {

            //get resource strings
            this.ResourceManager = resourceManager;

            string styleExtension = resourceManager.GetString("STYLE_STYLE_EXTENSION");
            m_styleExtension = string.IsNullOrEmpty(styleExtension) ? ".style" : styleExtension;

            string defaultStyleName = resourceManager.GetString("STYLE_DEFAULT_STYLENAME");
            m_defaultStyleName = string.IsNullOrEmpty(defaultStyleName) ? "Default" : defaultStyleName;

            if (runFromStick)
            {
                string[] files = null;
                //add own skins
                if (Directory.Exists(stylesPath))
                {
                    files = Directory.GetFiles(stylesPath, "*" + m_styleExtension);
                    AddStyles(stylesPath, files);
                }
                //add already installed skins
                if (Directory.Exists(appdataFolderDesigns))
                {
                    files = Directory.GetFiles(appdataFolderDesigns, "*" + m_styleExtension);
                    AddStyles(appdataFolderDesigns, files);
                }

                StylesPath = stylesPath;
            }
            else
            {
                //create default skin folder
                if (!Directory.Exists(appdataFolderDesigns))
                    Directory.CreateDirectory(appdataFolderDesigns);

                StylesPath = appdataFolderDesigns;
#if !DEBUG
            if (!Directory.Exists(appdataFolderDesigns + @"\System"))
#endif
                {
                    Directory.CreateDirectory(appdataFolderDesigns);
                    if (Directory.Exists(stylesPath))
                        CopyFolder(stylesPath, appdataFolderDesigns);
                }
                string[] files = Directory.GetFiles(appdataFolderDesigns, "*" + m_styleExtension);
                AddStyles(appdataFolderDesigns, files);
                if (Directory.Exists(stylesPath))
                {
                    files = Directory.GetFiles(stylesPath, "*" + m_styleExtension);
                    AddStyles(stylesPath, files);
                }
            }

            //add empty default style (as fallback)
            if (!Styles.ContainsKey(m_defaultStyleName))
            {
                Style defaultStyle = new Style();
                CheckStyles(stylesPath, defaultStyle, ResourceManager);
                defaultStyle.AnswerStylesheetPath = Path.Combine(appdataFolderDesigns, defaultStyle.AnswerStylesheetPath);
                defaultStyle.QuestionStylesheetPath = Path.Combine(appdataFolderDesigns, defaultStyle.QuestionStylesheetPath);
                defaultStyle.StylePath = Path.Combine(appdataFolderDesigns, defaultStyle.StylePath);
                Styles.Add(m_defaultStyleName, defaultStyle);
            }


        }

        /// <summary>
        /// Adds the style from the given file.
        /// </summary>
        /// <param name="filepath">The filepath.</param>
        /// <remarks>Documented by Dev02, 2009-07-03</remarks>
        public void AddStyle(string filepath)
        {
            if (!Path.IsPathRooted(filepath))
                filepath = Path.Combine(StylesPath, filepath);

            AddStyles(StylesPath, new string[] { filepath });
        }

        /// <summary>
        /// Adds the styles from the given files.
        /// </summary>
        /// <param name="stylesheetPath">The stylesheet path, where the stylesheets of the styles could be found.</param>
        /// <param name="files">The files.</param>
        /// <remarks>Documented by Dev05, 2007-08-20</remarks>
        private void AddStyles(string stylesheetPath, string[] files)
        {
            string backupWorkingDir = Directory.GetCurrentDirectory();
            foreach (string file in files)
            {
                try
                {
                    AwaitStylePreparation(stylesheetPath, true);
                    FileInfo info = new FileInfo(file);
                    string path = Path.GetDirectoryName(file) + @"\" + info.Name.Substring(0, info.Name.Length - info.Extension.Length);
                    Directory.SetCurrentDirectory(path);
                    using (Stream input = File.OpenRead(file))
                    {
                        Style style = (Style)serializer.Deserialize(input);

                        if (style.StyleVersion == "1.0")
                        {
                            //transform stylesheet version
                            input.Position = 0;
                            MemoryStream transformed = new MemoryStream();

                            XmlReader reader = XmlReader.Create(input);
                            XmlWriter writer = XmlWriter.Create(transformed);

                            versionTransform10_11.Transform(reader, writer);
                            transformed.Position = 0;
                            style = (Style)serializer.Deserialize(transformed);
                            style.StyleVersion = "1.1";
                        }

                        CheckStyles(stylesheetPath, style, ResourceManager);

                        style.AnswerStylesheetPath = Path.Combine(stylesheetPath, style.AnswerStylesheetPath);
                        style.QuestionStylesheetPath = Path.Combine(stylesheetPath, style.QuestionStylesheetPath);
                        style.StylePath = Path.Combine(stylesheetPath, style.StylePath);

                        if (!Styles.ContainsKey(style.StyleName))
                            Styles.Add(style.StyleName, style);
                    }
                }
                catch (InvalidOperationException exp)
                {
                    System.Diagnostics.Trace.WriteLine("StyleHandler - InvalidOperationException: " + exp.ToString());
                }
                finally
                {
                    Directory.SetCurrentDirectory(backupWorkingDir);
                }
            }
        }

        /// <summary>
        /// Completes the styles paths, adds the default styles if necessary, adds the resourceManager for using localizable style names.
        /// </summary>
        /// <param name="stylesheetPath">The stylesheet path.</param>
        /// <param name="style">The style.</param>
        /// <remarks>Documented by Dev02, 2008-01-21</remarks>
        private static void CheckStyles(string stylesheetPath, Style style, System.Resources.ResourceManager resourceManager)
        {
            AddDefaultPrintStyles(style, resourceManager);

            List<StyleInfo> invalidStyles = new List<StyleInfo>();
            foreach (StyleInfo styleinfo in style.PrintStyles)
            {
                styleinfo.Path = Path.Combine(stylesheetPath, styleinfo.Path);
                styleinfo.resourceManager = resourceManager;

                if (!File.Exists(styleinfo.Path))
                    invalidStyles.Add(styleinfo);
            }

            //remove invalid styles
            foreach (StyleInfo styleinfo in invalidStyles)
                style.PrintStyles.Remove(styleinfo);
        }

        /// <summary>
        /// Copies the folder.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <remarks>Documented by Dev05, 2007-09-25</remarks>
        public static void CopyFolder(string source, string destination)
        {
            try
            {
                foreach (string folder in Directory.GetDirectories(source))
                {
                    int index = folder.LastIndexOf(Path.DirectorySeparatorChar);
                    string cop = folder.Substring(index + 1);
                    Directory.CreateDirectory(destination + Path.DirectorySeparatorChar + cop);
                    CopyFolder(folder, Path.Combine(destination, cop));
                }

                foreach (string filename in Directory.GetFiles(source))
                {
                    int index = filename.LastIndexOf(Path.DirectorySeparatorChar);
                    string cop = filename.Substring(index + 1);
                    if (File.Exists(Path.Combine(destination, cop)))
                    {
                        File.Delete(Path.Combine(destination, cop));
                        File.Copy(filename, Path.Combine(destination, cop), true);
                    }
                    else
                    {
                        File.Copy(filename, Path.Combine(destination, cop), true);
                    }
                }
            }
            catch (Exception exp)
            {
                System.Diagnostics.Debug.WriteLine("Error on style copying: " + exp.ToString());
                return;
            }
        }

        /// <summary>
        /// Adds the default print styles.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <remarks>Documented by Dev02, 2008-01-21</remarks>
        public static void AddDefaultPrintStyles(Style style, System.Resources.ResourceManager ResourceManager)
        {
            StyleInfo info = new StyleInfo();
            info.resourceManager = ResourceManager;
            info.Path = @"System\Print\default.xsl";
            string infoname = ResourceManager.GetString("PRINT_STYLE_PRINTALL");
            info.Name = string.IsNullOrEmpty(infoname) ? "Print All" : infoname;
            info.ResourceID = "PRINT_STYLE_PRINTALL";

            if (!ContainsStyle(style.PrintStyles, info))
                style.PrintStyles.Add(info);

            info = new StyleInfo();
            info.resourceManager = ResourceManager;
            info.Path = @"System\Print\withoutMedia.xsl";
            infoname = ResourceManager.GetString("PRINT_STYLE_WITHOUTMEDIA");
            info.Name = string.IsNullOrEmpty(infoname) ? "Print without Media" : infoname;
            info.ResourceID = "PRINT_STYLE_WITHOUTMEDIA";

            if (!ContainsStyle(style.PrintStyles, info))
                style.PrintStyles.Add(info);
        }

        /// <summary>
        /// Determines whether the specified style infos are considered to be equal.
        /// </summary>
        /// <param name="styleInfo1">The style info 1.</param>
        /// <param name="styleInfo2">The style info 2.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-01-21</remarks>
        private static bool SameStyle(StyleInfo styleInfo1, StyleInfo styleInfo2)
        {
            return styleInfo1.Path.ToLower() == styleInfo2.Path.ToLower();
        }

        /// <summary>
        /// Determines whether the specified style infos contains style.
        /// </summary>
        /// <param name="styleInfos">The style infos.</param>
        /// <param name="styleInfo">The style info.</param>
        /// <returns>
        /// 	<c>true</c> if the specified style infos contains style; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>Documented by Dev02, 2008-01-21</remarks>
        private static bool ContainsStyle(List<StyleInfo> styleInfos, StyleInfo styleInfo)
        {
            foreach (StyleInfo si in styleInfos)
            {
                if (SameStyle(si, styleInfo))
                    return true;
            }
            return false;
        }
    }

    /// <summary>
    /// This class hold one style.
    /// </summary>
    /// <remarks>Documented by Dev05, 2007-08-10</remarks>
    [Serializable]
    public class Style
    {
        public string StyleName = "Default";
        public string StylePath = "Default";
        public string StyleVersion = "1.0"; //default version for 'old styles' that have no version declared
        public string QuestionStylesheetPath = "Default\\question.xsl";
        public string AnswerStylesheetPath = "Default\\answer.xsl";

        public SerializableDictionary<string, ControlSettings> StyledControls = new SerializableDictionary<string, ControlSettings>();
        public List<StyleInfo> PrintStyles = new List<StyleInfo>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Style"/> class.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-05-13</remarks>
        public Style()
        { }
    }

    /// <summary>
    /// Stores all stylable setting of arrayList control.
    /// </summary>
    /// <remarks>Documented by Dev05, 2007-08-13</remarks>
    [Serializable]
    public class ControlSettings : IXmlSerializable
    {
        public Color ForeColor = Color.Empty;
        public Color BackColor = Color.Empty;
        public Font Font = null;
        public Dictionary<string, object> CustomProperties = new Dictionary<string, object>();

        /// <summary>
        /// Initializes arrayList new instance of the <see cref="ControlSettings"/> class.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-08-13</remarks>
        public ControlSettings() { }

        # region Serialization
        /// <summary>
        /// This property is reserved, apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute"></see> to the class instead.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Xml.Schema.XmlSchema"></see> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)"></see> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)"></see> method.
        /// </returns>
        /// <remarks>Documented by Dev05, 2007-08-08</remarks>
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader"></see> stream from which the object is deserialized.</param>
        /// <remarks>Documented by Dev05, 2007-08-08</remarks>
        public void ReadXml(XmlReader reader)
        {
            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();

            if (wasEmpty)
            {
                return;
            }

            while (reader.NodeType != XmlNodeType.EndElement)
            {
                switch (reader.Name)
                {
                    case "ForeColor":
                        reader.ReadStartElement("ForeColor");
                        ForeColor = (Color)GetValue(typeof(Color), reader.ReadContentAsString());
                        reader.ReadEndElement();
                        break;
                    case "BackColor":
                        reader.ReadStartElement("BackColor");
                        BackColor = (Color)GetValue(typeof(Color), reader.ReadContentAsString());
                        reader.ReadEndElement();
                        break;
                    case "Font":
                        reader.ReadStartElement("Font");
                        Font = (Font)GetValue(typeof(Font), reader.ReadContentAsString());
                        reader.ReadEndElement();
                        break;
                    default:
                        string propertyname = reader.Name;
                        string typestring = reader.GetAttribute("Type");
                        reader.ReadStartElement(propertyname);
                        if (!string.IsNullOrEmpty(typestring))
                        {
                            object value = null;
                            try
                            {
                                if (typestring == "Font")
                                {
                                    string valuestring = reader.ReadContentAsString();
                                    value = (Font)GetValue(typeof(Font), valuestring);
                                }
                                else if (typestring == "Color")
                                {
                                    string valuestring = reader.ReadContentAsString();
                                    value = (Color)GetValue(typeof(Color), valuestring);
                                }
                                else if (typestring == "Gradient")
                                {
                                    XmlSerializer gradientSerializer = new XmlSerializer(typeof(PanelGradient));
                                    value = (PanelGradient)gradientSerializer.Deserialize(reader);
                                }
                                else if (typestring == "BorderStyle")
                                {
                                    try
                                    {
                                        value = Enum.Parse(typeof(BorderStyle), reader.ReadContentAsString());
                                    }
                                    catch (Exception e) { Trace.WriteLine(e.ToString()); }
                                }
                                else if (typestring == "String")
                                {
                                    value = reader.ReadContentAsString();
                                }
                                else if (typestring == "Boolean")
                                {
                                    value = reader.ReadContentAsBoolean();
                                }
                                else if (typestring == "SpecialSymbols")
                                {
                                    try
                                    {
                                        value = Enum.Parse(typeof(MLifter.Components.GlassButton.SpecialSymbols), reader.ReadContentAsString());
                                    }
                                    catch (Exception e) { Trace.WriteLine(e.ToString()); }
                                }
                                else if (typestring == "Image")
                                {
                                    try
                                    {
                                        string filePath = Path.Combine(Directory.GetCurrentDirectory(), reader.ReadContentAsString());
                                        value = Image.FromFile(filePath) as Image;
                                    }
                                    catch (Exception e) { Trace.WriteLine(e.ToString()); }
                                }
                                else if (typestring == "ImageLayout")
                                {
                                    try
                                    {
                                        value = Enum.Parse(typeof(ImageLayout), reader.ReadContentAsString());
                                    }
                                    catch (Exception e) { Trace.WriteLine(e.ToString()); }
                                }
                            }
                            catch
                            {
                            }
                            if (value != null)
                                CustomProperties.Add(propertyname, value);
                        }

                        reader.ReadEndElement();
                        break;
                }
            }
            reader.ReadEndElement();
        }

        /// <summary>
        /// Parses the specified string read form the XML and returns the value
        /// it represents as the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2007-08-08</remarks>
        private object GetValue(Type type, string value)
        {
            if (type == typeof(string))
            {
                return value;
            }
            else if (type == typeof(bool))
            {
                return Convert.ToBoolean(value);
            }
            else if (type.IsEnum)
            {
                return Enum.Parse(type, value, true);
            }
            else if (type == typeof(Color))
            {
                //Regex regEx = new Regex(@"A=(?<AV>\d{1,3}),\s*R=(?<RV>\d{1,3}),\s*G=(?<GV>\d{1,3}),\s*B=(?<BV>\d{1,3})");
                //Match match = regEx.Match(value);

                //if (match.Success)
                //    return Color.FromArgb(Convert.ToInt32(match.Groups["AV"].Value), Convert.ToInt32(match.Groups["RV"].Value),
                //        Convert.ToInt32(match.Groups["GV"].Value), Convert.ToInt32(match.Groups["BV"].Value));
                //else
                //{
                //    regEx = new Regex(@"\[(?<NAME>\w+)\]");
                //    match = regEx.Match(value);

                //    if (match.Success)
                //        return Color.FromName(match.Groups["NAME"].Value);
                //    else
                //        return Color.Empty;
                //}

                return GetFromInvariantString(type, value);
            }
            else if (type == typeof(DateTime))
            {
                if (value.Length == 0)
                {
                    return DateTime.MinValue;
                }
                else
                {
                    return DateTime.Parse(value);
                }
            }
            else if (type == typeof(TimeSpan))
            {
                return TimeSpan.Parse(value);
            }
            else if (type == typeof(Int16))
            {
                return Convert.ToInt16(value);
            }
            else if (type == typeof(Int32))
            {
                return Convert.ToInt32(value);
            }
            else if (type == typeof(Int64))
            {
                return Convert.ToInt64(value);
            }
            else if (type == typeof(float))
            {
                return Convert.ToSingle(value);
            }
            else if (type == typeof(double))
            {
                return Convert.ToDouble(value);
            }
            else if (type == typeof(decimal))
            {
                return Convert.ToDecimal(value);
            }
            else if (type == typeof(char))
            {
                return Convert.ToChar(value);
            }
            else if (type == typeof(byte))
            {
                return Convert.ToByte(value);
            }
            else if (type == typeof(UInt16))
            {
                return Convert.ToUInt16(value);
            }
            else if (type == typeof(UInt32))
            {
                return Convert.ToUInt32(value);
            }
            else if (type == typeof(UInt64))
            {
                return Convert.ToUInt64(value);
            }
            else if (type == typeof(Guid))
            {
                return new Guid(value);
            }
            else
            {
                return GetFromInvariantString(type, value);
            }
        }

        /// <summary>
        /// Gets the object from the invariant string.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2007-08-13</remarks>
        private object GetFromInvariantString(Type type, string value)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(type);
            return converter.ConvertFromInvariantString(value);
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"></see> stream to which the object is serialized.</param>
        /// <remarks>Documented by Dev05, 2007-08-08</remarks>
        public void WriteXml(XmlWriter writer)
        {
            if (ForeColor != Color.Empty)
            {
                writer.WriteStartElement("ForeColor");
                writer.WriteString(GetString(typeof(Color), ForeColor));
                writer.WriteEndElement();
            }

            if (BackColor != Color.Empty)
            {
                writer.WriteStartElement("BackColor");
                writer.WriteString(GetString(typeof(Color), BackColor));
                writer.WriteEndElement();
            }

            if (Font != null)
            {
                writer.WriteStartElement("Font");
                writer.WriteString(GetString(typeof(Font), Font));
                writer.WriteEndElement();
            }

            if (CustomProperties.Count > 0)
            {
                foreach (KeyValuePair<string, object> property in CustomProperties)
                {
                    writer.WriteStartElement(property.Key);
                    writer.WriteAttributeString("Type", property.Value.GetType().Name);
                    if (property.Value is PanelGradient)
                    {
                        XmlSerializer gradientSerializer = new XmlSerializer(typeof(PanelGradient));
                        gradientSerializer.Serialize(writer, property.Value);
                    }
                    else
                        writer.WriteString(GetString(property.Value.GetType(), property.Value));
                    writer.WriteEndElement();
                }
            }
        }

        /// <summary>
        /// Gets the string representing the given object.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2007-08-13</remarks>
        private string GetString(Type type, object value)
        {
            if (type == typeof(string))
            {
                return (string)value;
            }
            else if (type == typeof(bool) || type.IsEnum || type == typeof(DateTime) || type == typeof(TimeSpan) ||
                type == typeof(Int16) || type == typeof(Int32) || type == typeof(Int64) || type == typeof(float) || type == typeof(double) ||
                type == typeof(decimal) || type == typeof(char) || type == typeof(byte) || type == typeof(UInt16) || type == typeof(UInt32) ||
                type == typeof(UInt64) || type == typeof(Guid)/*s|| type == typeof(Color)*/)
            {
                return value.ToString();
            }
            else
            {
                return GetInvariantString(type, value);
            }
        }

        /// <summary>
        /// Gets the invariant string of the object.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2007-08-13</remarks>
        private string GetInvariantString(Type type, object value)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(type);
            return converter.ConvertToInvariantString(value);
        }

        /// <summary>
        /// Works out which types to treat as attibutes and which the treat as child objects.
        /// </summary>
        /// <param name="type">The Type to check.</param>
        /// <returns>true if the Type is atomic (e.g. string, date, enum or number), false if it is arrayList compound sub-object.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <i>type</i> is null (Nothing in Visual Basic).</exception>
        private static bool TypeIsAtomic(Type type)
        {
            if (type == typeof(string) || TypeIsNumeric(type) || type == typeof(bool) || type == typeof(DateTime) ||
                type == typeof(TimeSpan) || type == typeof(char) || type == typeof(byte) || type.IsSubclassOf(typeof(Enum)) ||
                type == typeof(Guid) || type == typeof(Color) || type == typeof(Font))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns true if the specified type is one of the numeric types
        /// (Int16, Int32, Int64, UInt16, UInt32, UInt64, Single, Double, Decimal)
        /// </summary>
        /// <param name="type">The Type to check.</param>
        /// <returns>
        /// true if the specified type is one of the numeric types
        /// (Int16, Int32, Int64, UInt16, UInt32, UInt64, Single, Double, Decimal)
        /// </returns>
        private static bool TypeIsNumeric(Type type)
        {
            if (type == typeof(Int16) || type == typeof(Int32) || type == typeof(Int64) || type == typeof(float) ||
                type == typeof(double) || type == typeof(decimal) || type == typeof(UInt16) || type == typeof(UInt32) ||
                type == typeof(UInt64))
            {
                return true;
            }

            return false;
        }
        # endregion
    }

    /// <summary>
    /// Holds the a styleinfo for a specific printstyle.
    /// Don't forget to set resourceManager for resolving ResourceIDs.
    /// </summary>
    /// <remarks>Documented by Dev05, 2007-08-14</remarks>
    [Serializable]
    public class StyleInfo
    {
        public string Path = string.Empty;
        public string Name = string.Empty;
        public string ResourceID = string.Empty;

        /// <summary>
        /// The Resource Manager to resolve the supplied ResourceIDs.
        /// Don't set to fallback to the StyleInfo Names.
        /// </summary>
        [XmlIgnore, NonSerialized]
        public System.Resources.ResourceManager resourceManager = null;

        /// <summary>
        /// Initializes arrayList new instance of the <see cref="StyleInfo"/> class.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-08-14</remarks>
        public StyleInfo() { }

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        /// <remarks>Documented by Dev02, 2008-01-21</remarks>
        public override string ToString()
        {
            if (resourceManager != null)
            {
                string resourceName;
                if (!string.IsNullOrEmpty(resourceName = resourceManager.GetString(ResourceID)))
                    return resourceName;
            }

            if (!string.IsNullOrEmpty(Name))
                return Name;

            FileInfo path = new FileInfo(Path);
            return path.Name.Substring(0, path.Name.Length - path.Extension.Length);
        }
    }
}
