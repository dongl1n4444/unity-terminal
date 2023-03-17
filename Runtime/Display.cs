using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTerminal
{
  // 重新渲染变更的地方，但是使用 sprite 之后没有该问题
  // 替换地方
  // typedef void RenderGlyph(int x, int y, Glyph glyph);

  /// The "backing store" that a renderable terminal uses to tell which glyphs
  /// actually need rendering.
  ///
  /// Drawing a glyph on screen is the most expensive operation a terminal
  /// performs, so we want to avoid doing that when not necessary. The simplest
  /// solution is to render the glyphs that are drawn when terminal draw call is
  /// made. However, it's common for a given terminal cell to be drawn multiple
  /// times between updates. Often, the public result is the same as what was
  /// previously on screen.
  ///
  /// For example, consider a terminal for a game. It renders a "." floor tile,
  /// then renders the "/" on top of that for an item on the floor, then a "M"
  /// for the monster standing on that tile. The end user only sees the "M". The
  /// next frame, the process repeats and the ".", "/", and "M" are drawn.
  ///
  /// If we render eagerly, that's a ton of wasted effort to end up with the same
  /// pixels that are on screen. What we want is to let the code modify as many
  /// glyphs as it wants as many times as it wants. Once that entire draw process
  /// is complete, we see which glyphs ended up different from the last time the
  /// terminal was shown to the user and just render those.
  ///
  /// That's what this class does. It maintains two arrays of glyphs. One
  /// represents what was last shown to the user. The other represents what
  /// modifications have been made to the terminal since then. When the terminal
  /// is written to, this keeps track of which glyphs are actually different from
  /// the last render and which are the same.
  ///
  /// Once that's done, you can call [render]. That will invoke the callback to
  /// actually draw a glyph, but only for the ones that are actually modified.
  class Display {
    /// The current display state. The glyphs here mirror what has been rendered.
    public Array2D<Glyph> _glyphs;

    /// The glyphs that have been modified since the last call to [render].
    public Array2D<Glyph> _changedGlyphs;

    public int width => _glyphs.width;
    public int height => _glyphs.height;
    // public Vector2Int size => _glyphs.size;

    public Display(int width, int height)
    {
      _glyphs = new Array2D<Glyph>(width, height, Glyph.clear);
      _changedGlyphs = new Array2D<Glyph>(width, height, Glyph.clear);
    }

    /// Sets the cell at [x], [y], to [glyph].
    public void setGlyph(int x, int y, Glyph glyph) {
      if (x < 0) return;
      if (x >= width) return;
      if (y < 0) return;
      if (y >= height) return;

      var oldGlyph = _glyphs.Get(x, y);
      if (oldGlyph == null || oldGlyph.isNotEqual(glyph)) {
        _changedGlyphs.Set(x, y, glyph);
      } else {
        _changedGlyphs.Set(x, y, null);
      }
    }

    /// Calls [renderGlyph] for every glyph that has changed since the last call
    /// to [render].
    public void render(System.Action<int, int, Glyph> renderGlyph) {
      for (var y = 0; y < height; y++) {
        for (var x = 0; x < width; x++) {
          var glyph = _changedGlyphs.Get(x, y);

          // Only draw glyphs that are different since the last call.
          if (glyph == null) continue;

          renderGlyph(x, y, glyph);

          // It's up to date now.
          _glyphs.Set(x, y, glyph);
          _changedGlyphs.Set(x, y, null);
        }
      }
    }
  }
}