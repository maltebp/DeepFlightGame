
using System;


/// <summary>
/// Custom struct to represent a set of double coordinates
/// </summary>
public struct Point {

    public double X { get; }
    public double Y { get; }

    public Point( double x, double y) {
        X = x;
        Y = y;
    }

    public double DistanceTo(Point point) {
        return Math.Sqrt(Math.Pow(X - point.X, 2) + Math.Pow(Y - point.Y, 2));
    }

    public Point Rotate(Point origin, double angle) {
        double cos = Math.Cos(angle);
        double sin = Math.Sin(angle);
        Point diffPoint = this - origin;

        return new Point(
            cos * diffPoint.X - sin * diffPoint.Y + origin.X,
            sin * diffPoint.X + cos * diffPoint.Y + origin.Y
        );
    }

    public override string ToString() {
        return $"Point({X:F2},{Y:F2})";
    }

    public static Point operator -(Point p1, Point p2) {
        return new Point(p1.X - p2.X, p1.Y - p2.Y);
    }

    public static Point operator +(Point p1, Point p2) {
        return new Point(p1.X + p2.X, p1.Y + p2.Y);
    }

    public static Point operator *(Point p1, Point p2) {
        return new Point(p1.X * p2.X, p1.Y * p2.Y);
    }

    public static Point operator /(Point p1, Point p2) {
        return new Point(p1.X / p2.X, p1.Y / p2.Y);
    }

    public static bool operator ==(Point p1, Point p2) {
        return p1.X == p2.X && p1.Y == p2.Y;
    }

    public static bool operator !=(Point p1, Point p2) {
        return p1.X != p2.X || p1.Y != p2.Y;
    }

}
