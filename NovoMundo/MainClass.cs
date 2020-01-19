using System;
using System.Collections.Generic;

using Gtk;

class MainClass
{
	public static Dictionary<string, Gdk.Pixbuf> BiomeTileDict = new Dictionary<string, Gdk.Pixbuf> ();

	public static QuadrantInfo RequestQuadrantInfo(int x, int y)
	{
		Quadrant thisQuadrant = Earth.grid [x, y];

		QuadrantInfo quadInfo = new QuadrantInfo ();
		quadInfo.x = x;
		quadInfo.y = y;
		quadInfo.latitude = thisQuadrant.latitude;
		quadInfo.longitude = thisQuadrant.longitude;
		quadInfo.coordinateString = thisQuadrant.coordinateString;
		quadInfo.elevation = thisQuadrant.elevation;
		quadInfo.area = thisQuadrant.area;

		quadInfo.wayDown = thisQuadrant.steepestDecline;

		quadInfo.isWater = thisQuadrant.isWater;
		quadInfo.precipitation = thisQuadrant.precipitation;

		quadInfo.biomeName = thisQuadrant.quadrantBiome.BiomeName;

		quadInfo.cloudtopInsolation = thisQuadrant.cloudtopInsolation;
		quadInfo.surfaceInsolation = thisQuadrant.surfaceInsolation;

		quadInfo.atmosphereAbsorption = thisQuadrant.atmosphereAbsorption;
		quadInfo.airTemperature = thisQuadrant.airTemperature;

		quadInfo.geothermalFlux = thisQuadrant.GeothermalHeat;

		return quadInfo;
	}

	static byte[] ImageToByteArray(System.Drawing.Image img)
	{
		System.Drawing.ImageConverter imageConverter = new System.Drawing.ImageConverter ();
		byte[] xByte = (Byte[])imageConverter.ConvertTo (img, typeof(byte[]));
		return xByte;
	}

	public static void Main (string[] args)
	{
		Earth NewEarth = new Earth ();

		Application.Init ();
		MainWindow win = new MainWindow ();
		win.Show ();

		Application.Run ();

	}
}
