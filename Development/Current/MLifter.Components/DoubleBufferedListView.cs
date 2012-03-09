using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace MLifter.Components
{
    public class DoubleBufferedListView : ListView
    {
        public DoubleBufferedListView()
            : base()
        {
            this.DoubleBuffered = true;
        }
    }
}
