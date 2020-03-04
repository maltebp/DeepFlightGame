
// My custom math functions
public static class MathExtension {

    /// <summary>
    /// Does a special type of modulo, which is different from % when using negative numbers.
    /// </summary>
    // Source: https://stackoverflow.com/questions/1082917/mod-of-negative-number-is-melting-my-brain
    public static int Mod(int x, int m) {
        int r = x % m;
        return r < 0 ? r + m : r;
    }

    /// <summary>
    /// Does a special type of modulo, which is different from % when using negative numbers.
    /// </summary>
    public static float Mod(float x, float m) {
        float r = x % m;
        return r < 0 ? r + m : r;
    }

}