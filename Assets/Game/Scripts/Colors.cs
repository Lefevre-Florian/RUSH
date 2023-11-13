using System.Collections.Generic;
using UnityEngine;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush
{
    public enum Colors
    {
        RED = 0,
        CYAN = 1,
        GREEN = 2,
        YELLOW = 3,
        VIOLET = 4,
        ORANGE = 5
    }

    public static class ColorLibrary
    {
        private static Dictionary<Colors, Color> _Library = new Dictionary<Colors, Color>()
        {
            {Colors.RED, Color.red },
            {Colors.CYAN, Color.cyan },
            {Colors.GREEN, Color.green },
            {Colors.VIOLET, Color.magenta },
            {Colors.YELLOW, Color.yellow },
            {Colors.ORANGE, new Color(255f,127f,0f) }
        };

        public static Dictionary<Colors, Color> Library
        {
            get { return _Library; }
            private set { _Library = value; }
        }
    }
}
