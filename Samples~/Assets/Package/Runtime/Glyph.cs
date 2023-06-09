using System;
using System.Collections.Generic;

namespace UnityTerminal
{
    [Serializable]
    public class Glyph
    {
        public int ch;
        public Color fore;
        public Color back;

        public Glyph(char ch, Color? fore = null, Color? back = null)
        {
            Set(ch, fore, back);
        }


        public Glyph(int ch, Color? fore = null, Color? back = null)
        {
            Set(ch, fore, back);
        }

        public void Set(int ch, Color? fore = null, Color? back = null)
        {
            this.ch = ch;
            this.fore = fore ?? Color.white;
            this.back = back ?? Color.black;
        }

        public bool IsEqual(int ch, Color? fore, Color? back)
        {
            return this.ch == ch &&
                this.fore.Equals(fore) &&
                this.back.Equals(back);
        }

        public bool IsEqual(Glyph other)
        {
            if (other == null)
                return false;
            return IsEqual(other.ch, other.fore, other.back);
        }
    }
}