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
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;

namespace MLifterRecorder.DragNDrop
{
    /// <summary>
    /// The DragAndDropListView control inherits from ListView, and provides native support for dragging and dropping ListItems to reorder them or move them to other DragAndDropListView controls.
    /// http://www.codeproject.com/cs/miscctrl/DragAndDropListView.asp
    /// </summary>
    /// <remarks>Documented by Dev03, 2007-07-19</remarks>
    public class DragAndDropListView : ListView
    {
        #region Private Members

        private ListViewItem m_previousItem;
        private bool m_allowReorder;
        private Color m_lineColor;
        private string m_lock;

        #endregion

        #region Public Properties

        [Category("Behavior")]
        public bool AllowReorder
        {
            get { return m_allowReorder; }
            set { m_allowReorder = value; }
        }

        [Category("Appearance")]
        public Color LineColor
        {
            get { return m_lineColor; }
            set { m_lineColor = value; }
        }

        #endregion

        #region Protected and Public Methods

        public DragAndDropListView()
            : base()
        {
            m_allowReorder = true;
            m_lineColor = Color.Red;
            m_lock = "";
        }

        public void SetLockCode(string lock_code)
        {
            m_lock = lock_code;
        }

        public string GetLockCode()
        {
            return m_lock;
        }

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            if (!m_allowReorder)
            {
                base.OnDragDrop(drgevent);
                return;
            }

            // get the currently hovered row that the items will be dragged to
            Point clientPoint = base.PointToClient(new Point(drgevent.X, drgevent.Y));
            ListViewItem hoverItem = base.GetItemAt(clientPoint.X, clientPoint.Y);

            if (!drgevent.Data.GetDataPresent(typeof(DragItemData).ToString()) || ((DragItemData)drgevent.Data.GetData(typeof(DragItemData).ToString())).ListView == null || ((DragItemData)drgevent.Data.GetData(typeof(DragItemData).ToString())).DragItems.Count == 0)
                return;

            // retrieve the drag item data
            DragItemData data = (DragItemData)drgevent.Data.GetData(typeof(DragItemData).ToString());

            // check if lock condition is ok
            if (m_lock != data.ListView.GetLockCode())
                return;

            if (hoverItem == null)
            {
                // remove all the selected items from the previous list view
                // if the list view was found
                if (data.ListView != null)
                {
                    foreach (ListViewItem itemToRemove in data.ListView.SelectedItems)
                    {
                        data.ListView.Items.Remove(itemToRemove);
                    }
                }
                // the user does not wish to re-order the items, just append to the end
                for (int i = 0; i < data.DragItems.Count; i++)
                {
                    ListViewItem newItem = (ListViewItem)data.DragItems[i];
                    // rename the ListViewItems to their original name
                    newItem.Name = newItem.Name.Replace("copy_of_", "");
                    base.Items.Add(newItem);
                    base.SelectedIndices.Add(base.Items.Count - 1);
                }
            }
            else
            {
                // the user wishes to re-order the items

                // get the index of the hover item
                int hoverIndex = hoverItem.Index;

                // determine if the items to be dropped are from
                // this list view. If they are, perform a hack
                // to increment the hover index so that the items
                // get moved properly.
                if (this == data.ListView)
                {
                    if (hoverIndex > base.SelectedItems[0].Index)
                        hoverIndex--;
                }

                // remove all the selected items from the previous list view
                // if the list view was found
                if (data.ListView != null)
                {
                    foreach (ListViewItem itemToRemove in data.ListView.SelectedItems)
                    {
                        data.ListView.Items.Remove(itemToRemove);
                    }
                }

                // insert the new items into the list view
                // by inserting the items reversely from the array list
                for (int i = data.DragItems.Count - 1; i >= 0; i--)
                {
                    ListViewItem newItem = (ListViewItem)data.DragItems[i];
                    // rename the ListViewItems to their original name
                    newItem.Name = newItem.Name.Replace("copy_of_", "");
                    base.Items.Insert(hoverIndex, newItem);
                    base.SelectedIndices.Add(hoverIndex);
                }
            }

            // set the back color of the previous item, then nullify it
            if (m_previousItem != null)
            {
                m_previousItem = null;
            }

            this.Invalidate();

            // call the base on drag drop to raise the event
            base.OnDragDrop(drgevent);
        }

        protected override void OnDragOver(DragEventArgs drgevent)
        {
            if (!m_allowReorder)
            {
                base.OnDragOver(drgevent);
                return;
            }

            if (!drgevent.Data.GetDataPresent(typeof(DragItemData).ToString()))
            {
                // the item(s) being dragged do not have any data associated
                drgevent.Effect = DragDropEffects.None;
                return;
            }
            if (((DragItemData)drgevent.Data.GetData(typeof(DragItemData).ToString())).ListView == null)
                return;

            // retrieve the drag item data
            DragItemData data = (DragItemData)drgevent.Data.GetData(typeof(DragItemData).ToString());

            // check if lock condition is ok
            if (m_lock != data.ListView.GetLockCode())
            {
                // the item(s) being dragged do not have any data associated
                drgevent.Effect = DragDropEffects.None;
                return;
            }

            if (base.Items.Count > 0)
            {
                // get the currently hovered row that the items will be dragged to
                Point clientPoint = base.PointToClient(new Point(drgevent.X, drgevent.Y));
                ListViewItem hoverItem = base.GetItemAt(clientPoint.X, clientPoint.Y);

                Graphics g = this.CreateGraphics();

                if (hoverItem == null)
                {
                    //MessageBox.Show(base.GetChildAtPoint(new Point(clientPoint.X, clientPoint.Y)).GetType().ToString());

                    // no item was found, so no drop should take place
                    drgevent.Effect = DragDropEffects.Move;

                    if (m_previousItem != null)
                    {
                        m_previousItem = null;
                        Invalidate();
                    }

                    hoverItem = base.Items[base.Items.Count - 1];

                    if (this.View == View.Details || this.View == View.List)
                    {
                        using (Pen pen = new Pen(m_lineColor, 2))
                        {
                            g.DrawLine(pen, new Point(hoverItem.Bounds.X, hoverItem.Bounds.Y + hoverItem.Bounds.Height), new Point(hoverItem.Bounds.X + this.Bounds.Width, hoverItem.Bounds.Y + hoverItem.Bounds.Height));
                        }
                        using (SolidBrush solidBrush = new SolidBrush(m_lineColor))
                        {
                            g.FillPolygon(solidBrush, new Point[] { new Point(hoverItem.Bounds.X, hoverItem.Bounds.Y + hoverItem.Bounds.Height - 5), new Point(hoverItem.Bounds.X + 5, hoverItem.Bounds.Y + hoverItem.Bounds.Height), new Point(hoverItem.Bounds.X, hoverItem.Bounds.Y + hoverItem.Bounds.Height + 5) });
                        }
                        using (SolidBrush solidBrush = new SolidBrush(m_lineColor))
                        {
                            g.FillPolygon(solidBrush, new Point[] { new Point(this.Bounds.Width - 4, hoverItem.Bounds.Y + hoverItem.Bounds.Height - 5), new Point(this.Bounds.Width - 9, hoverItem.Bounds.Y + hoverItem.Bounds.Height), new Point(this.Bounds.Width - 4, hoverItem.Bounds.Y + hoverItem.Bounds.Height + 5) });
                        }
                    }
                    else
                    {
                        using (Pen pen = new Pen(m_lineColor, 2))
                        {
                            g.DrawLine(pen, new Point(hoverItem.Bounds.X + hoverItem.Bounds.Width, hoverItem.Bounds.Y), new Point(hoverItem.Bounds.X + hoverItem.Bounds.Width, hoverItem.Bounds.Y + hoverItem.Bounds.Height));
                        }
                        using (SolidBrush solidBrush = new SolidBrush(m_lineColor))
                        {
                            g.FillPolygon(solidBrush, new Point[] { new Point(hoverItem.Bounds.X + hoverItem.Bounds.Width - 5, hoverItem.Bounds.Y), new Point(hoverItem.Bounds.X + hoverItem.Bounds.Width + 5, hoverItem.Bounds.Y), new Point(hoverItem.Bounds.X + hoverItem.Bounds.Width, hoverItem.Bounds.Y + 5) });
                        }
                        using (SolidBrush solidBrush = new SolidBrush(m_lineColor))
                        {
                            g.FillPolygon(solidBrush, new Point[] { new Point(hoverItem.Bounds.X + hoverItem.Bounds.Width - 5, hoverItem.Bounds.Y + hoverItem.Bounds.Height), new Point(hoverItem.Bounds.X + hoverItem.Bounds.Width + 5, hoverItem.Bounds.Y + hoverItem.Bounds.Height), new Point(hoverItem.Bounds.X + hoverItem.Bounds.Width, hoverItem.Bounds.Y + hoverItem.Bounds.Height - 5) });
                        }
                    }

                    // call the base OnDragOver event
                    base.OnDragOver(drgevent);

                    return;
                }

                // determine if the user is currently hovering over a new
                // item. If so, set the previous item's back color back
                // to the default color.
                if ((m_previousItem != null && m_previousItem != hoverItem) || m_previousItem == null)
                {
                    this.Invalidate();
                }

                // set the background color of the item being hovered
                // and assign the previous item to the item being hovered
                //hoverItem.BackColor = Color.Beige;
                m_previousItem = hoverItem;

                if (this.View == View.Details || this.View == View.List)
                {
                    using (Pen pen = new Pen(m_lineColor, 2))
                    {
                        g.DrawLine(pen, new Point(hoverItem.Bounds.X, hoverItem.Bounds.Y), new Point(hoverItem.Bounds.X + this.Bounds.Width, hoverItem.Bounds.Y));
                    }
                    using (SolidBrush solidBrush = new SolidBrush(m_lineColor))
                    {
                        g.FillPolygon(solidBrush, new Point[] { new Point(hoverItem.Bounds.X, hoverItem.Bounds.Y - 5), new Point(hoverItem.Bounds.X + 5, hoverItem.Bounds.Y), new Point(hoverItem.Bounds.X, hoverItem.Bounds.Y + 5) });
                    }
                    using (SolidBrush solidBrush = new SolidBrush(m_lineColor))
                    {
                        g.FillPolygon(solidBrush, new Point[] { new Point(this.Bounds.Width - 4, hoverItem.Bounds.Y - 5), new Point(this.Bounds.Width - 9, hoverItem.Bounds.Y), new Point(this.Bounds.Width - 4, hoverItem.Bounds.Y + 5) });
                    }
                }
                else
                {
                    using (Pen pen = new Pen(m_lineColor, 2))
                    {
                        g.DrawLine(pen, new Point(hoverItem.Bounds.X, hoverItem.Bounds.Y), new Point(hoverItem.Bounds.X, hoverItem.Bounds.Y + hoverItem.Bounds.Height));
                    }
                    using (SolidBrush solidBrush = new SolidBrush(m_lineColor))
                    {
                        g.FillPolygon(solidBrush, new Point[] { new Point(hoverItem.Bounds.X - 5, hoverItem.Bounds.Y), new Point(hoverItem.Bounds.X + 5, hoverItem.Bounds.Y), new Point(hoverItem.Bounds.X, hoverItem.Bounds.Y + 5) });
                    }
                    using (SolidBrush solidBrush = new SolidBrush(m_lineColor))
                    {
                        g.FillPolygon(solidBrush, new Point[] { new Point(hoverItem.Bounds.X - 5, hoverItem.Bounds.Y + hoverItem.Bounds.Height), new Point(hoverItem.Bounds.X + 5, hoverItem.Bounds.Y + hoverItem.Bounds.Height), new Point(hoverItem.Bounds.X, hoverItem.Bounds.Y + hoverItem.Bounds.Height - 5) });
                    }
                }

                // go through each of the selected items, and if any of the
                // selected items have the same index as the item being
                // hovered, disable dropping.
                foreach (ListViewItem itemToMove in base.SelectedItems)
                {
                    if (itemToMove.Index == hoverItem.Index)
                    {
                        drgevent.Effect = DragDropEffects.None;
                        hoverItem.EnsureVisible();
                        return;
                    }
                }

                // ensure that the hover item is visible
                hoverItem.EnsureVisible();
            }

            // everything is fine, allow the user to move the items
            drgevent.Effect = DragDropEffects.Move;

            // call the base OnDragOver event
            base.OnDragOver(drgevent);
        }

        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            if (!m_allowReorder)
            {
                base.OnDragEnter(drgevent);
                return;
            }

            if (!drgevent.Data.GetDataPresent(typeof(DragItemData).ToString()))
            {
                // the item(s) being dragged do not have any data associated
                drgevent.Effect = DragDropEffects.None;
                return;
            }

            // everything is fine, allow the user to move the items
            drgevent.Effect = DragDropEffects.Move;

            // call the base OnDragEnter event
            base.OnDragEnter(drgevent);
        }

        protected override void OnItemDrag(ItemDragEventArgs e)
        {
            if (!m_allowReorder)
            {
                base.OnItemDrag(e);
                return;
            }

            // call the DoDragDrop method
            base.DoDragDrop(GetDataForDragDrop(), DragDropEffects.Move);

            // call the base OnItemDrag event
            base.OnItemDrag(e);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            // reset the selected items background and remove the previous item
            ResetOutOfRange();

            Invalidate();

            // call the OnLostFocus event
            base.OnLostFocus(e);
        }

        protected override void OnDragLeave(EventArgs e)
        {
            // reset the selected items background and remove the previous item
            ResetOutOfRange();

            Invalidate();

            // call the base OnDragLeave event
            base.OnDragLeave(e);
        }

        #endregion

        #region Private Methods

        private DragItemData GetDataForDragDrop()
        {
            // create a drag item data object that will be used to pass along with the drag and drop
            DragItemData data = new DragItemData(this);

            // go through each of the selected items and 
            // add them to the drag items collection
            // by creating a clone of the list item
            foreach (ListViewItem item in this.SelectedItems)
            {
                object copied_item = item.Clone();
                (copied_item as ListViewItem).Name = "copy_of_" + item.Name;
                //System.Diagnostics.Debug.Print("\n*********************************************\nName-"+(copied_item as ListViewItem).Name+"-End Name\n*********************************************\n");
                data.DragItems.Add(copied_item);
            }

            return data;
        }

        private void ResetOutOfRange()
        {
            // determine if the previous item exists,
            // if it does, reset the background and release 
            // the previous item
            if (m_previousItem != null)
            {
                m_previousItem = null;
            }

        }

        #endregion

        #region DragItemData Class

        private class DragItemData
        {
            #region Private Members

            private DragAndDropListView m_listView;
            private ArrayList m_dragItems;

            #endregion

            #region Public Properties

            public DragAndDropListView ListView
            {
                get { return m_listView; }
            }

            public ArrayList DragItems
            {
                get { return m_dragItems; }
            }

            #endregion

            #region Public Methods and Implementation

            public DragItemData(DragAndDropListView listView)
            {
                m_listView = listView;
                m_dragItems = new ArrayList();
            }

            #endregion
        }

        #endregion
    }
}
