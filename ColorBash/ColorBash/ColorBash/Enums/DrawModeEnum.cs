using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ColorBash
{
    /// <summary>
    /// Represents the Plate Draw Mode which determines the desired fill color signalisation drawing
    /// </summary>
    public enum DrawMode
    {
        Regular = 1,
        CenterRotate,
        TopLeftCenterRotate,
        TopLeftRotate,
        BotLeft,
        BotRight
    }
}
