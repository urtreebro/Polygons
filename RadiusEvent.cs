using System;

namespace Polygons;

public delegate void RadiusDelegate(object sender, RadiusEventArgs e);

public class RadiusEventArgs : EventArgs
{
    public double Radius;

    public RadiusEventArgs(double r)
    {
        Radius = r;
    }
}