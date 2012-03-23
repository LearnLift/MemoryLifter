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
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace CommandDemo {
    /// <summary>
    /// Several UI controls (e.g. a menu item, toolbar button, and a regular
    /// button) can be associated with a single UICommand.  Enabling/disabling the UICommand
    /// object enables/disables the UI controls.  Clicking one of the UI controls raises the
    /// UICommand.Execute event.  Another class (UICommandProvider) allows the programmer to
    /// specify a UICommand instance for each UI control.
    /// </summary>
    public partial class UICommand : Component {
        public UICommand() {
            InitializeComponent();
        }

        public UICommand(IContainer container) {
            container.Add(this);

            InitializeComponent();
            ClickForwarderDelegate = new EventHandler(ClickForwarder);
        }

        // This event is raised whenever one of the attached controls is clicked.
        public event EventHandler Execute;

        // Sets/gets the Enabled property of the attached controls.
        public bool Enabled {
            get { return _enabled; }
            set { 
                _enabled = value;
                foreach (Component c in _components) {
                    if (c is Control) ((Control)c).Enabled = _enabled;
                    else if (c is ToolStripItem) ((ToolStripItem)c).Enabled = _enabled;
                    else throw new ApplicationException("Object has unexpected type " + c.GetType());
                }
            }
        }

        private bool _enabled;

        // The list of UI contols attached to this UICommand.  Since menu items and toolbar
        // buttons don't derive from Control, it's a list of Components instead of Controls.
        private List<Component> _components = new List<Component>();

        // The attached controls have their Click events mapped to this, which
        // forwards the click event to the Execute event.
        private void ClickForwarder(object sender, EventArgs args) {
            if (Execute != null) Execute(sender, args);
        }

        private EventHandler ClickForwarderDelegate;

        // This attaches the specified control to this UICommand. 
        internal void Add(Component component) {
            // We must be able to handle any object that UICommandProvider.CanExtend returns true for.
            if (component is Control) {
                ((Control)component).Click += ClickForwarderDelegate;
                ((Control)component).Enabled = _enabled;
            } else if (component is ToolStripItem) {
                ((ToolStripItem)component).Click += ClickForwarderDelegate;
                ((ToolStripItem)component).Enabled = _enabled;
            } else throw new ApplicationException("Object has unexpected type " + component.GetType());
            
            _components.Add(component);
        }

        // This removes the specified control from this UICommand. 
        internal void Remove(Component component) {
            // We must be able to handle any object that UICommandProvider.CanExtend returns true for.
            _components.Remove(component);

            if (component is Control) ((Control)component).Click -= ClickForwarderDelegate;
            else if (component is ToolStripItem) ((ToolStripItem)component).Click -= ClickForwarderDelegate;
            else throw new ApplicationException("Object has unexpected type " + component.GetType());
        }
    }
}
