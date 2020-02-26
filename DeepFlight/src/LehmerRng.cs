using System;

public class LehmerRng {
    private const int a = 16807;
    private const int m = 2147483647;
    private const int q = 127773;
    private const int r = 2836;
    private ulong seed;
    public LehmerRng(ulong seed) {
        this.seed = seed;
        //if (seed <= 0 || seed == int.MaxValue)
        //    throw new Exception("Bad seed");
        //this.seed = seed;
    }

    public ulong Next() {
        seed += 0xe120fc15;
        ulong tmp = seed * 0x4a39b70d;
        ulong m1 = ((tmp >> 32) ^ tmp);
        tmp = m1 * 0x12fad5c9;
        ulong m2 = ((tmp >> 32) ^ tmp);
        return m2;

        //int hi = seed / q;
        //int lo = seed % q;
        //seed = (a * lo) - (r * hi);
        //if (seed <= 0)
        //    seed = seed + m;
        //return (seed * 1.0) / m;
    }
}