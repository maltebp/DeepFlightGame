
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

// Holds the various textures in the game
static class Fonts {

    public static readonly Font ARIAL = new Font("Arial");

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

    public string Name { get; private set; }
    public Dictionary<int, SpriteFont> fontMap = new Dictionary<int, SpriteFont>();
    private bool loaded = false;

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

        // Iterate over sizes in steps of 2
        for (int i = MIN_SIZE; i <= MAX_SIZE; i += 2) {
            try {
                SpriteFont font = content.Load<SpriteFont>(FONT_FOLDER + Name + "_" + i);
                fontMap[i] = font;
                loadedSizes.AddLast(i);
            }
            catch (ContentLoadException e) { }
        }

        if (loadedSizes.Count == 0)
            throw new ContentLoadException(string.Format("Couldn't load Font '{0}' in any sizes.", Name));

        // Print successfull load
        Console.Write("Loaded font 'Arial' in sizes: ");
        foreach (int size in loadedSizes) {
            Console.Write(size + " ");
        }
        Console.WriteLine("\n");
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
}