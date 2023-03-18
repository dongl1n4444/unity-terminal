using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTerminal
{
    public class RetroCanvas : TerminalCanvas
    {
        public static Dictionary<string, Sprite[]> spritesMap = new Dictionary<string, Sprite[]>();

        public SpriteRenderer bg;
        public Transform glyphsRoot;
        public float pixelToUnits = 100; // default

        [Header("RUNTIME")]
        public RetroTerminal terminal;
        public Array2D<GlyphRender> glyphRenders;
        public Sprite[] sprites = null;
        public Dictionary<int, int> char2SpriteIndexs;

        public void Init(RetroTerminal terminal, string resName, Dictionary<int, int> char2SpriteIndexs)
        {
            if (spritesMap.ContainsKey(resName) == false)
            {
                var sprs = Resources.LoadAll<Sprite>(resName);
                spritesMap[resName] = sprs;
            }
            sprites = spritesMap[resName];

            glyphRenders = new Array2D<GlyphRender>(terminal.width, terminal.height, null);
            this.terminal = terminal;
            this.char2SpriteIndexs = char2SpriteIndexs;

            bg.transform.localScale = new Vector3(UnityEngine.Screen.width / pixelToUnits, UnityEngine.Screen.height / pixelToUnits, 1f);
        }

        public override void Render()
        {
           if (terminal == null)
                return;

            for (var x = 0; x < terminal.width; x++) {
                for (var y = 0; y < terminal.height; y++) {
                    var glyph = terminal.glyphs.Get(x, y);
                
                    if (glyph == null || glyph.ch == CharCode.space)
                    {
                        this.Set(x, y, null, Color.white);
                    }
                    else
                    {
                        // Debug.Log($"xx-- render > {x}, {y}, {glyph.ch}");

                        int sprIdx = glyph.ch;
                        if (char2SpriteIndexs.ContainsKey(glyph.ch))
                        {
                            sprIdx = char2SpriteIndexs[glyph.ch];
                        }

                        if (sprIdx < 0 || sprIdx >= sprites.Length)
                        {
                            Debug.LogError("not support glyph > " + glyph.ch);
                            return;
                        }

                        // _display.setGlyph(x, y, glyph);
                        this.Set(x, y, sprites[sprIdx], glyph.fore);
                    }
                }
            }
        }

        public void Set(int x, int y, Sprite spr, Color foreColor)
        {
            var rt = terminal as RetroTerminal;
            if (rt == null)
            {
                Debug.LogError("only support for RetroTerminal");
                return;
            }

            var gr = glyphRenders.Get(x, y);
            if (gr == null)
            {
                gr = GlyphRender.Create(glyphsRoot);
                gr.name = $"SPR_{x}_{y}";
                glyphRenders.Set(x, y, gr);

                gr.transform.localPosition = new Vector3(
                    (x - terminal.width * 0.5f + 0.5f) * rt.charWidth * rt.scale / pixelToUnits, 
                    (rt.height * 0.5f - y - 0.5f) * rt.charHeight * rt.scale / pixelToUnits, 
                    0f);
                gr.transform.localScale = Vector3.one * rt.scale;
            }
            gr.SetSprite(spr);
            gr.SetColor(foreColor);
        }
    }
}

