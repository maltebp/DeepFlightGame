
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using DeepFlight.utility;

// Holds and initialize the loading, of the variious fonts in the game
public static class Fonts {

    public static readonly Font ROBOTO_BOLD_ITALIC = new Font("Roboto", true, true);
    public static readonly Font ROBOTO_BOLD = new Font("Roboto", true, false);
    public static readonly Font ARIAL = new Font("Arial");
    public static readonly Font PIXELLARI = new Font("Pixellari");
    
    public static readonly Font DEFAULT = PIXELLARI;

    // Loads all created Font objects, using the given content
    public static void Load(ContentManager content) {
        Font.LoadAll(content);
    }
}

/// <summary>
/// Defines a Font in a series of different sizes
/// between MIN_SIZE and MAX_SIZE, depending on which
/// Fonts files are created in the Content.
/// </summary>
public class Font {

    private const string FONT_FOLDER = "Content/Fonts/";
    private const int MAX_SIZE = 124;
    private const int MIN_SIZE = 8;

    public string   Name { get; private set; }
    public bool     Bold { get; }
    public bool     Italic { get; }

    public Dictionary<int, SpriteFont> fontMap = new Dictionary<int, SpriteFont>();
    private bool loaded = false;
    private double scaling = 0;

    private static LinkedList<Font> allFonts = new LinkedList<Font>();

    /// <summary>
    /// Create a new Font with the given Name. Font doesn't get loaded
    /// before calling the Load method.
    /// </summary>
    /// <param name="name"> The Font name, as well as the filename
    /// prefix when searching for possible Font sizes.
    /// </param>
    public Font(string name) {
        Name = name;
        allFonts.AddLast(this);
    }

    /// <summary>
    /// Create a new Font with the given Name, and a set of styles. Font doesn't get loaded
    /// before calling the Load method.
    /// </summary>
    /// <param name="name"> The Font name, as well as the filename
    /// prefix when searching for possible Font sizes.
    /// </param>
    public Font(string name, bool bold, bool italic) {
        Name = name;
        allFonts.AddLast(this);
        Bold = bold;
        Italic = italic;
    }

    /// <summary>
    /// Calls the Load method of all created Fonts, using
    /// the given Content.
    /// </summary>
    public static void LoadAll(ContentManager content) {
        foreach( Font font in allFonts) {
            font.Load(content);
        }
    }

    /// <summary>
    /// Load the Font, using the given name as file prefix,
    /// and loads all even sizes between MIN_SIZE and MAX_SIZE.
    /// </summary>
    /// <param name="content"></param>
    public void Load(ContentManager content) {
        if( loaded )
            throw new ContentLoadException(string.Format("Font '{0}' has already been loaded", Name));

        LinkedList<int> loadedSizes = new LinkedList<int>();
        
        // Example: Contents/Fonts/Roboto/Robot_Bold_Italic_12
        string path = FONT_FOLDER + "/" + Name + "/" + Name + (Bold ? "_Bold" : "") + (Italic ? "_Italic" : "");

        // Iterate over font sizes in steps of 2
        for (int i = MIN_SIZE; i <= MAX_SIZE; i += 2) {
            try {
                SpriteFont font = content.Load<SpriteFont>(path + "_" + i);
                fontMap[i] = font;
                loadedSizes.AddLast(i);
            }
            catch (ContentLoadException e) { }
        }

        if (loadedSizes.Count == 0)
            throw new ContentLoadException(string.Format("Couldn't load Font '{0}' in any sizes.", Name));


        // Print successfull load
        Console.Write("Loaded font '{0}' in sizes: ", Name);
        foreach (int size in loadedSizes) {
            Console.Write(size + " ");
        }
        loaded = true;
    }

    /// <summary>
    /// Get the SpriteFont object of the given size,
    /// for this Font.
    /// </summary>
    /// <param name="size"></param>
    /// <returns>SpriteFont object if given size</returns>
    /// <exception cref="NullReferenceException"> If the given size hasn't been loaded yet.</exception>
    public SpriteFont GetFont(int size) {
        if( !loaded )
            throw new NullReferenceException(string.Format("Font '{0}' hasn't been loaded yet.", Name));
        if (!fontMap.ContainsKey(size))
            throw new NullReferenceException( string.Format("Font '{0}' hasn't loaded size {1}.", Name, size));
        return fontMap[size];
    }

    public Dictionary<int, SpriteFont> GetFontMap() {
        return fontMap;
    }


    /// <summary>
    /// Finds the best actual Font for the given size, in
    /// terms of the closest size.
    /// </summary>
    public FontData GetBestFont(double targetSize) {
        int bestSize = 0;
        double bestDiff = 100000;
        bool first = true;
        foreach (var fontSize in fontMap.Keys) {
            var sizeDiff = Math.Abs(targetSize - fontSize);
            if (first || sizeDiff < bestDiff) {
                first = false;
                bestDiff = sizeDiff;
                bestSize = fontSize;
            }
        }
        return new FontData( fontMap[bestSize], bestSize );
    }

    /// <summary>
    /// Calculate the dimensions of the output text with this Font,
    /// at a given size.
    /// </summary>
    public Vector2 MeasureString(string text, double size) {
        var bestFont = GetBestFont(size);
        return bestFont.Sprite.MeasureString(text) * (float) (size / bestFont.Size);        
    }

}

/// <summary>
/// Wraps the SpriteFont object with additional data (size atm).
/// </summary>
public struct FontData {
    public SpriteFont Sprite { get; }
    public int Size { get;  }

    public FontData(SpriteFont sprite, int size) {
        Sprite = sprite;
        Size = size;
    }
}