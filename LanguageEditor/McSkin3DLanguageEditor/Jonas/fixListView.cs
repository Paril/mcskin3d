using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MCSkin3DLanguageEditor.Jonas
{
    class fixListView : ListView
    {
        public fixListView()
        {
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }
    }
}
