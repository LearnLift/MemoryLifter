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
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;

namespace MLifter.Components
{
    /// <summary>
    /// An enhanced ListView control that supports custom RightToLeft values for each Column.
    /// </summary>
    /// <remarks>Documented by Dev02, 2008-07-15</remarks>
    public class ColumnRightToLeftListView : ScrollListView
    {
        private List<int> _RightToLeftEnabledColumnIndexes = new List<int>();

        /// <summary>
        /// Gets the right to left enabled column indexes.
        /// </summary>
        /// <value>The right to left enabled column indexes.</value>
        /// <remarks>Documented by Dev02, 2008-07-15</remarks>
        [Category("Appearance"), Description("The right to left enabled column indexes.")]
        public List<int> RightToLeftEnabledColumnIndexes
        {
            get { return _RightToLeftEnabledColumnIndexes; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnRTLListview"/> class.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-07-15</remarks>
        public ColumnRightToLeftListView()
        {
            this.DoubleBuffered = true;
            this.OwnerDraw = true;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.ListView.DrawColumnHeader"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.DrawListViewColumnHeaderEventArgs"/> that contains the event data.</param>
        /// <remarks>Documented by Dev02, 2008-07-15</remarks>
        protected override void OnDrawColumnHeader(DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
            base.OnDrawColumnHeader(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.ListView.DrawItem"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.DrawListViewItemEventArgs"/> that contains the event data.</param>
        /// <remarks>Documented by Dev02, 2008-07-15</remarks>
        protected override void OnDrawItem(DrawListViewItemEventArgs e)
        {
            if (this.View != View.Details || _RightToLeftEnabledColumnIndexes.Count == 0)
            {
                e.DrawDefault = true;
            }
            else
            {
                //e.DrawBackground();

                if (e.Item.Selected && (e.Item.ListView.Focused || !e.Item.ListView.HideSelection))
                    e.Graphics.FillRectangle(new SolidBrush(e.Item.ListView.Focused ? SystemColors.Highlight : SystemColors.Control), e.Bounds);

                //e.DrawFocusRectangle();
            }

            base.OnDrawItem(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.ListView.DrawSubItem"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.DrawListViewSubItemEventArgs"/> that contains the event data.</param>
        /// <remarks>Documented by Dev02, 2008-07-15</remarks>
        protected override void OnDrawSubItem(DrawListViewSubItemEventArgs e)
        {
            if (this.View != View.Details || !_RightToLeftEnabledColumnIndexes.Contains(e.ColumnIndex))
            {
                e.DrawDefault = true;
            }
            else
            {
                TextFormatFlags flags = TextFormatFlags.RightToLeft | TextFormatFlags.EndEllipsis;
                //e.DrawText(flags); //cannot be used: always draws with ForeColor, regardless of Item State (Selected/Gray...)
                Color forecolor = !e.Item.ListView.Enabled ? SystemColors.GrayText :
                    (e.Item.Selected && e.Item.ListView.Focused) ? SystemColors.HighlightText : e.SubItem.ForeColor;
                TextRenderer.DrawText(e.Graphics, e.SubItem.Text, e.SubItem.Font, e.Bounds, forecolor, flags);
            }

            base.OnDrawSubItem(e);
        }

        // Forces each row to repaint itself the first time the mouse moves over 
        // it, compensating for an extra DrawItem event sent by the wrapped 
        // Win32 control. 
        // from: http://msdn.microsoft.com/de-de/library/system.windows.forms.drawlistviewsubitemeventargs(VS.80).aspx
        protected override void OnMouseMove(MouseEventArgs e)
        {
            ListViewItem lvi = this.GetItemAt(e.X, e.Y);

            if (lvi != null)
                this.Invalidate(lvi.Bounds);

            base.OnMouseMove(e);
        }
    }
}
