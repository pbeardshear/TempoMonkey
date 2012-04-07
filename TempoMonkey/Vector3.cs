using System;
using System.Linq;

class Vector3
{
    public double _x, _y, _z;
    public Vector3(double x, double y, double z)
    {
        _x = x;
        _y = y;
        _z = z;
    }

    public static Vector3 operator -(Vector3 a, Vector3 b)
    {
        return new Vector3(
            a._x - b._x,
            a._y - b._y,
            a._z - b._z);
    }

    //Cross Product
    public static Vector3 operator ^(Vector3 a, Vector3 b)
    {
        return new Vector3(
            a._y * b._z - a._z * b._y,
            a._z * b._x - a._x * b._z,
            a._x * b._y - a._y * b._x);
    }

    //Dot Product
    public static double operator *(Vector3 a, Vector3 b)
    {
        return a._x * b._x + a._y * b._y + a._z * b._z;
    }

    public void normalize()
    {
        double mag = Math.Sqrt(_x * _x + _y * _y + _z * _z);
        if (mag != 0)
        {
            _x /= mag;
            _y /= mag;
            _z /= mag;
        }
    }

    public static double angleBetween(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        Vector3 v1tov2 = v1 - v2;
        Vector3 v2tov3 = v2 - v3;
        v1tov2.normalize();
        v2tov3.normalize();

        Vector3 crossProduct = v1tov2 ^ v2tov3;
        double crossProductLength = crossProduct._z;

        double dotProduct = v1tov2 * v2tov3;
        double segmentAngle = Math.Atan2(crossProductLength, dotProduct);

        // Convert the result to degrees.
        double degrees = (180 - segmentAngle * (180 / Math.PI)) % 360;
        return degrees;
    }
}