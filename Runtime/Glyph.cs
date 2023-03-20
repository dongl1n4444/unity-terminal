using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTerminal
{
    public class Glyph
    {
        public int ch;
        public Color fore;
        public Color back;

        public Glyph(char ch, Color? fore = null, Color? back = null)
        {
            this.ch = ch;
            this.fore = fore ?? Color.white;
            this.back = back ?? Color.black;
        }


        public Glyph(int ch, Color? fore = null, Color? back = null)
        {
            this.ch = ch;
            this.fore = fore ?? Color.white;
            this.back = back ?? Color.black;
        }

        public bool isEqual(int ch, Color? fore, Color? back)
        {
            return this.ch == ch && 
                this.fore == fore &&
                this.back == back;
        }

        public bool isEqual(Glyph other)
        {
            if (other == null)
                return false;
            return isEqual(other.ch, other.fore, other.back);
        }
    }
}