﻿using System;

public static class Program {
    [STAThread]
    static void Main() {

        //BlockMapTest.RunTests();

        var game = new GameController();
        game.Run();
    }
}