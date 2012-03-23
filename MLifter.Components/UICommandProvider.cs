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
    /// UICommandProvider allows most UI elements (those derived from Control and ToolStripItem)
    /// to have a UICommand property.  When several UI elements have the same UICommand property,
    /// they can all be enabled/disabled at the same time via UICommand.Enabled and they all raise
    /// the same UICommand.Execute event when clicked.
    /// </summary>
    [ProvideProperty("UICommand", typeof(Component))]
    public partial class UICommandProvider : Component, IExtenderProvider {
        public UICommandProvider() {
            InitializeComponent();
        }

        public UICommandProvider(IContainer container) {
            container.Add(this);

            InitializeComponent();
        }

        private Dictionary<Component, UICommand> _dict = new Dictionary<Component,UICommand>();
        
        // If CanExtend says we can support a given object, the UICommand class must also be able
        // to support it.
        bool IExtenderProvider.CanExtend(object control) {
            return (
                control is ToolStripItem ||
                control is Control
            );
        }

        /// <summary>
        /// This sets the UICommand instance of the specified control.
        /// </summary>
        public void SetUICommand(Component control, UICommand cmd) {
            // If the control already has a non-null UICommand, we must detach the control from
            // that UICommand.
            UICommand oldCmd = null;
            if (_dict.TryGetValue(control, out oldCmd)) {
                oldCmd.Remove(control);
            }

            // If the new UICommand value is null, just remove it from the dictionary.  We never
            // allow null entries in the dictionary.
            if (cmd == null) {
                _dict.Remove(control);
            } else {
                _dict[control] = cmd;
                cmd.Add(control);
            }
        }

        /// <summary>
        /// This gets the UICommand instance (possibly null) for the specified control.
        /// <param name="control"></param>
        public UICommand GetUICommand(Component control) {
            // Return null if there is no entry for the control in the dictionary.
            UICommand ret = null;
            _dict.TryGetValue(control, out ret);
            return ret;
        }
    }
}
