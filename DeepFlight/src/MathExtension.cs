
public static class MathExtension {

    // Source: https://stackoverflow.com/questions/1082917/mod-of-negative-number-is-melting-my-brain
    public static int Mod(int x, int m) {
        int r = x % m;
        return r < 0 ? r + m : r;
    }

    public static float Mod(float x, float m) {
        float r = x % m;
        return r < 0 ? r + m : r;
    }

}