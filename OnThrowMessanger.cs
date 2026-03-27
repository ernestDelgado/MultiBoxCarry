using System;
using System.Collections.Generic;
using System.Text;

namespace MultiBoxCarry
{
    internal static class OnThrowMessanger
    {
        public static bool hasMessage;

        public static void GaveMessage()
        {
            hasMessage = false;
        }

        public static void WriteMessage()
        {
            hasMessage = true;
        }

    }
}
