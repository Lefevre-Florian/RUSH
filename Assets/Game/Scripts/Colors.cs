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
        PINK = 5
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
            {Colors.PINK, new Color(255f,0f,212f) }
        };

        public static Dictionary<Colors, Color> Library
        {
            get { return _Library; }
            private set { _Library = value; }
        }
    }
}
