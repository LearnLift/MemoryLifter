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
using System.Drawing;
using System.Text;
using MLifter.DAL.Security;
using MLifter.DAL.Tools;

namespace MLifter.DAL.Interfaces
{
	/// <summary>
	/// Represents a card style.
	/// </summary>
	public interface ICardStyle : IParent, ICopy, ISecurity
	{
		/// <summary>
		/// Gets or sets the question style.
		/// </summary>
		/// <value>The question.</value>
		/// <remarks>Documented by Dev05, 2007-10-29</remarks>
		ITextStyle Question { get; set; }
		/// <summary>
		/// Gets or sets the question example style.
		/// </summary>
		/// <value>The question example.</value>
		/// <remarks>Documented by Dev05, 2007-10-29</remarks>
		ITextStyle QuestionExample { get; set; }
		/// <summary>
		/// Gets or sets the answer style.
		/// </summary>
		/// <value>The answer.</value>
		/// <remarks>Documented by Dev05, 2007-10-29</remarks>
		ITextStyle Answer { get; set; }
		/// <summary>
		/// Gets or sets the answer example style.
		/// </summary>
		/// <value>The answer example.</value>
		/// <remarks>Documented by Dev05, 2007-10-29</remarks>
		ITextStyle AnswerExample { get; set; }
		/// <summary>
		/// Gets or sets the answer correct style.
		/// </summary>
		/// <value>The answer correct.</value>
		/// <remarks>Documented by Dev05, 2007-10-30</remarks>
		ITextStyle AnswerCorrect { get; set; }
		/// <summary>
		/// Gets or sets the answer wrong style.
		/// </summary>
		/// <value>The answer wrong.</value>
		/// <remarks>Documented by Dev05, 2007-10-30</remarks>
		ITextStyle AnswerWrong { get; set; }

		/// <summary>
		/// Clones this instance.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2007-11-07</remarks>
		ICardStyle Clone();

		/// <summary>
		/// Gets the XML.
		/// </summary>
		/// <value>The XML.</value>
		/// <remarks>Documented by Dev11, 2008-08-27</remarks>
		string Xml { get; }

		/// <summary>
		/// Returns the CSS for this style using the given base path for media URIs.
		/// </summary>
		/// <param name="basepath">The media basepath.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev03, 2008-02-22</remarks>
		string ToString(string basepath);
	}

	/// <summary>
	/// Represents a text style.
	/// </summary>
	public interface ITextStyle : ICopy
	{
		/// <summary>
		/// Gets or sets the color of the text.
		/// </summary>
		/// <value>The color of the fore.</value>
		/// <remarks>Documented by Dev05, 2007-10-29</remarks>
		[ValueCopy]
		Color ForeColor { get; set; }
		/// <summary>
		/// Gets or sets the color of the background.
		/// </summary>
		/// <value>The color of the background.</value>
		/// <remarks>Documented by Dev05, 2007-10-29</remarks>
		[ValueCopy]
		Color BackgroundColor { get; set; }
		/// <summary>
		/// Gets or sets the font-family.
		/// </summary>
		/// <value>The font-family.</value>
		/// <remarks>Documented by Dev05, 2007-10-29</remarks>
		[ValueCopy]
		FontFamily FontFamily { get; set; }
		/// <summary>
		/// Gets or sets the font style.
		/// </summary>
		/// <value>The font style.</value>
		/// <remarks>Documented by Dev05, 2007-10-30</remarks>
		[ValueCopy]
		CSSFontStyle FontStyle { get; set; }
		/// <summary>
		/// Gets or sets the size of the font.
		/// </summary>
		/// <value>The size of the font.</value>
		/// <remarks>Documented by Dev05, 2007-10-30</remarks>
		[ValueCopy]
		int FontSize { get; set; }
		/// <summary>
		/// Gets or sets the font size unit.
		/// </summary>
		/// <value>The font size unit.</value>
		/// <remarks>Documented by Dev05, 2007-10-30</remarks>
		[ValueCopy]
		FontSizeUnit FontSizeUnit { get; set; }
		/// <summary>
		/// Gets or sets the horizontal alignment.
		/// </summary>
		/// <value>The horizontal align.</value>
		/// <remarks>Documented by Dev05, 2007-10-29</remarks>
		[ValueCopy]
		HorizontalAlignment HorizontalAlign { get; set; }
		/// <summary>
		/// Gets or sets the vertical alignment.
		/// </summary>
		/// <value>The vertical align.</value>
		/// <remarks>Documented by Dev05, 2007-10-29</remarks>
		[ValueCopy]
		VerticalAlignment VerticalAlign { get; set; }
		/// <summary>
		/// Gets or sets the other elements.
		/// </summary>
		/// <value>The other elements.</value>
		/// <remarks>Documented by Dev05, 2007-10-30</remarks>
		[ValueCopy]
		SerializableDictionary<string, string> OtherElements { get; set; }

		/// <summary>
		/// Returns the CSS for this style using the given base path for media URIs.
		/// </summary>
		/// <param name="basepath">The media basepath.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev03, 2008-02-22</remarks>
		string ToString(string basepath);
	}

	/// <summary>
	/// The vertical alignment.
	/// </summary>
	public enum VerticalAlignment
	{
		/// <summary>
		/// 
		/// </summary>
		Top,
		/// <summary>
		/// 
		/// </summary>
		Middle,
		/// <summary>
		/// 
		/// </summary>
		Bottom,
		/// <summary>
		/// 
		/// </summary>
		None
	}

	/// <summary>
	/// The Horizontal alignment.
	/// </summary>
	public enum HorizontalAlignment
	{
		/// <summary>
		/// 
		/// </summary>
		Left,
		/// <summary>
		/// 
		/// </summary>
		Center,
		/// <summary>
		/// 
		/// </summary>
		Right,
		/// <summary>
		/// 
		/// </summary>
		None
	}

	/// <summary>
	///     Specifies style information applied to text.
	/// </summary>
	[Flags]
	public enum CSSFontStyle
	{
		/// <summary>
		///     Normal text.
		/// </summary>
		Regular = 0,
		/// <summary>
		///     Bold text.
		/// </summary>
		Bold = 1,
		/// <summary>
		///     Italic text.
		/// </summary>
		Italic = 2,
		/// <summary>
		///     Underlined text.
		/// </summary>
		Underline = 4,
		/// <summary>
		///    Text with a line through the middle.
		/// </summary>
		Strikeout = 8,
		/// <summary>
		///     Do not add information.
		/// </summary>
		None = 16,
	}

	/// <summary>
	/// The unit type of a font size.
	/// </summary>
	public enum FontSizeUnit
	{
		/// <summary>
		/// 
		/// </summary>
		Pixel,
		/// <summary>
		/// 
		/// </summary>
		Percent
	}
}
