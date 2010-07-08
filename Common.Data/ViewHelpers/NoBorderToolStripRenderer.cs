using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

namespace Common.Data {
    public class NoBorderToolStripRenderer : ToolStripSystemRenderer {

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e) {
            if (e.ToolStrip is ToolStripDropDownMenu)
                base.OnRenderToolStripBorder(e);
        }

    }
}
