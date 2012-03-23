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

namespace MLifter.DAL.Interfaces
{
    public interface ICardStyle
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
        /// Returns the CSS for this style using the given base path for media URIs.
        /// </summary>
        /// <param name="basepath">The media basepath.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2008-02-22</remarks>
        string ToString(string basepath);
    }

    public interface ITextStyle
    {
        /// <summary>
        /// Gets or sets the color of the text.
        /// </summary>
        /// <value>The color of the fore.</value>
        /// <remarks>Documented by Dev05, 2007-10-29</remarks>
        Color ForeColor { get; set; }
        /// <summary>
        /// Gets or sets the color of the background.
        /// </summary>
        /// <value>The color of the background.</value>
        /// <remarks>Documented by Dev05, 2007-10-29</remarks>
        Color BackgroundColor { get; set; }
        /// <summary>
        /// Gets or sets the font-family.
        /// </summary>
        /// <value>The font-family.</value>
        /// <remarks>Documented by Dev05, 2007-10-29</remarks>
        FontFamily FontFamily { get; set; }
        /// <summary>
        /// Gets or sets the font style.
        /// </summary>
        /// <value>The font style.</value>
        /// <remarks>Documented by Dev05, 2007-10-30</remarks>
        CSSFontStyle FontStyle { get; set; }
        /// <summary>
        /// Gets or sets the size of the font.
        /// </summary>
        /// <value>The size of the font.</value>
        /// <remarks>Documented by Dev05, 2007-10-30</remarks>
        int FontSize { get; set; }
        /// <summary>
        /// Gets or sets the font size unit.
        /// </summary>
        /// <value>The font size unit.</value>
        /// <remarks>Documented by Dev05, 2007-10-30</remarks>
        FontSizeUnit FontSizeUnit { get; set; }
        /// <summary>
        /// Gets or sets the horizontal alignment.
        /// </summary>
        /// <value>The horizontal align.</value>
        /// <remarks>Documented by Dev05, 2007-10-29</remarks>
        HorizontalAlignment HorizontalAlign { get; set; }
        /// <summary>
        /// Gets or sets the vertical alignment.
        /// </summary>
        /// <value>The vertical align.</value>
        /// <remarks>Documented by Dev05, 2007-10-29</remarks>
        VerticalAlignment VerticalAlign { get; set; }
        /// <summary>
        /// Gets or sets the other elements.
        /// </summary>
        /// <value>The other elements.</value>
        /// <remarks>Documented by Dev05, 2007-10-30</remarks>
        SerializableDictionary<string, string> OtherElements { get; set; }

        /// <summary>
        /// Returns the CSS for this style using the given base path for media URIs.
        /// </summary>
        /// <param name="basepath">The media basepath.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2008-02-22</remarks>
        string ToString(string basepath);
    }

    public enum VerticalAlignment
    {
        Top,
        Middle,
        Bottom,
        None
    }

    public enum HorizontalAlignment
    {
        Left,
        Center,
        Right,
        None
    }

    // Summary:
    //     Specifies style information applied to text.
    [Flags]
    public enum CSSFontStyle
    {
        // Summary:
        //     Normal text.
        Regular = 0,
        //
        // Summary:
        //     Bold text.
        Bold = 1,
        //
        // Summary:
        //     Italic text.
        Italic = 2,
        //
        // Summary:
        //     Underlined text.
        Underline = 4,
        //
        // Summary:
        //     Text with a line through the middle.
        Strikeout = 8,
        //
        // Summary:
        //     Do not add information.
        None = 16,
    }

    public enum FontSizeUnit
    {
        Pixel,
        Percent
    }
}
