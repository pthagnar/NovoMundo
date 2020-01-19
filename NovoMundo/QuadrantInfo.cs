using System;

/* used by MainClass to shuttle information
 * about quadrants from the Earth grid to
 * the QuadrantDialogue */


public class QuadrantInfo
{
	//geometry

	public int x { get; set;}
	public int y { get; set;}
	public double latitude { get; set; }
	public double longitude {get; set; }
	public string coordinateString { get; set;}
	public int elevation { get; set;}
	public long area { get; set;}

	public Earth.Direction wayDown { get; set;}

	//atmosphere

	public long cloudtopInsolation{ get; set;}
	public long surfaceInsolation { get; set;}

	public long atmosphereAbsorption { get; set;}
	public float airTemperature { get; set;}

	public long geothermalFlux{ get; set;}

	//hydrology

	public bool isWater {get; set;}
	public int  precipitation {get; set;}



	//biology

	public string biomeName { get; set;}

	public QuadrantInfo ()
	{
	}
}

