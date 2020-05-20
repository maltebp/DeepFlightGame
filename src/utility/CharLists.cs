
namespace DeepFlight.utility {
    
    /// <summary>
    /// String lists of character sets (i.e. used to determine allowed input
    /// characters)
    /// </summary>
    public static class CharLists {
        public static readonly string LOWER_CASE    = "abcdefghijklmnopqrstuvwxyz";
        public static readonly string UPPER_CASE    = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public static readonly string NUMBERS       = "1234567890";
        public static readonly string SYMBOLS       = "=-?+/&%$@!.,;:_*'()[]{}#½<>";
        public static readonly string DEFAULT       = LOWER_CASE + UPPER_CASE + NUMBERS;
    }
}
