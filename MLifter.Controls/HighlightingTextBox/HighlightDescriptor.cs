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
using System.Drawing;

namespace MLifter.Controls.SyntaxHighlightingTextBox
{
	public class HighlightDescriptor
	{
		public HighlightDescriptor(string token, Color color, Font font, DescriptorType descriptorType, DescriptorRecognition dr, bool useForAutoComplete)
		{
			if (descriptorType == MLifter.Controls.SyntaxHighlightingTextBox.DescriptorType.ToCloseToken)
			{
				throw new ArgumentException("You may not choose ToCloseToken DescriptorType without specifing an end token.");
			}
			Color = color;
			Font = font;
			Token = token;
			DescriptorType = descriptorType;
			DescriptorRecognition = dr;
			CloseToken = null;
			UseForAutoComplete = useForAutoComplete;
		}
		public HighlightDescriptor(string token, string closeToken, Color color, Font font, DescriptorType descriptorType, DescriptorRecognition dr, bool useForAutoComplete)
		{
			Color = color;
			Font = font;
			Token = token;
			DescriptorType = descriptorType;
			CloseToken = closeToken;
			DescriptorRecognition = dr;
			UseForAutoComplete = useForAutoComplete;
		}
		public readonly Color Color;
		public readonly Font Font;
		public readonly string Token;
		public readonly string CloseToken;
		public readonly DescriptorType DescriptorType;
		public readonly DescriptorRecognition DescriptorRecognition; 
		public readonly bool UseForAutoComplete;
	}

	
	public enum DescriptorType
	{
		/// <summary>
		/// Causes the highlighting of a single word
		/// </summary>
		Word,
		/// <summary>
		/// Causes the entire line from this point on the be highlighted, regardless of other tokens
		/// </summary>
		ToEOL,
		/// <summary>
		/// Highlights all Text until the end token;
		/// </summary>
		ToCloseToken
	}

	public enum DescriptorRecognition
	{
		/// <summary>
		/// Only if the whole token is equal to the word
		/// </summary>
		WholeWord,
		/// <summary>
		/// If the word starts with the token
		/// </summary>
		StartsWith,
		/// <summary>
		/// If the word contains the Token
		/// </summary>
		Contains
	}

}
