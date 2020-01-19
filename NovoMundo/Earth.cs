using System;

public class Earth
{

	//geometry
	static int radius = 6371008; //m
	static int width {get; set;} = 360;
	static int height {get; set;} = 180;

	public enum Direction {NORTH, EAST, SOUTH, WEST};

	//hydrosphere
	public static int SeaLevel {get; set;} = 0;
	public static int ShallowSeaLevel {get; set;} = -200;


	//atmosphere
	public static int SolarConstant {get; set;} = 1388; //W/m^2
	public static float CloudtopAlbedo {get; set;} = 0.27f; //proportion
	public static float AtmosphericAbsorption {get; set;} = 0.2f; //prop'n
		
	//biosphere

	public static Quadrant[,] grid {get; set;} = new Quadrant[width,height];

	public Earth()
	{
		string[,] heightArray = new string[180,360];

		System.IO.StreamReader reader = System.IO.File.OpenText ("/home/pthag/Desktop/mundonovo/NovoMundo/NovoMundo/mapmaker/worldarray");
		string line;
		int k = 0;
		while ((line = reader.ReadLine ()) != null)
		{
			string[] items = line.Split (' ');
			Console.WriteLine (items.Length);
			for (int n = 0; n < 360; n++) 
			{
				heightArray [k, n] = items [n];
			}
			k++;

		}

		//first pass
		for (int i=0; i < width; i++ )
		{
			for (int j=0; j < height; j++)
			{
				grid [i,j ] = new Quadrant(xToLongitude(i),yToLatitude(j));

				Quadrant thisQuad = grid [i, j];

				thisQuad.area = (long)CalculateArea(i,j);

				string thisString = heightArray [j, i];
				int thisInt;
				if (!int.TryParse (heightArray [j, i], out thisInt))
				{
					thisInt = 0;
				}

				thisQuad.elevation = (thisInt * 28) - 350;

				thisQuad.Update ();

			}
		}

		//second pass
		for (int i=0; i < width; i++ )
		{
			for (int j=0; j < height; j++)
			{
				Quadrant thisQuad = grid [i, j];

				//initialise neighbourArrays w/ neighbouring elevation deltas in order of Direction
				int[] neighbourArray = new int[4];

				if (j > 0)
				{
					neighbourArray [0] = grid [i, j - 1].elevation - thisQuad.elevation;
				}
				else
				{
					neighbourArray[0] = System.Int32.MaxValue;
				}

				if (i < width-1)
				{
					neighbourArray[1] = grid[i+1,j].elevation - thisQuad.elevation;
				}
				else
				{
					neighbourArray[1] = System.Int32.MaxValue;
				}

				if (j < height-1)
				{
					neighbourArray [2] = grid [i, j + 1].elevation - thisQuad.elevation;
				}
				else
				{
					neighbourArray[2] = System.Int32.MaxValue;
				}

				if (i > 0)
				{
					neighbourArray [3] = grid [i - 1, j].elevation - thisQuad.elevation;
				}
				else
				{
					neighbourArray[3] = System.Int32.MaxValue;
				}
			
				//get index of lowest in array = value of enum Direction
				int lowestIndex = 0;
				for (int ix = 0; ix < neighbourArray.Length; ix++)
				{
					if( neighbourArray[ix] < neighbourArray[lowestIndex]) {lowestIndex = ix; };
				}
				thisQuad.steepestDecline = (Direction)lowestIndex;
			}
		}

	}

	public Quadrant GetQuadrant(int x, int y)
	{
		return grid [x, y];
	}

	//geometric functions

	static double DegreeToRadians(double angle)
	{
		return Math.PI * angle / 180.0;
	}

	static double xToLongitude(int x)
	{
		double longitude;

		if (x < width / 2)
		{
			longitude = (360 / width) * x;
		}
		else if (x == width / 2)
		{
			longitude = 180;
		}
		else if (x > width / 2)
		{
			longitude = -(360 - (360 / width) * x);
		}
		else
		{
			throw new Exception();
		}

		return longitude; 
	}

	static double yToLatitude(int y)
	{
		double latitude;

		if (y < height/2)
		{
			latitude = 90 - (180 / height) * y;
		}
		else if (y == height / 2)
		{
			latitude = 0;
		}
		else if (y > height / 2)
		{
			latitude = -( 90 - (180 - (180 / height) * y));
		}
		else
		{
			throw new Exception();
		}

		return latitude;
	}

	static double CalculateArea(int x, int y)
	{
		/*
		 * some correct values for 1 latlong quadrants:
		 * 80N: 2,166 km^2
		 * 45N: 8,753 km^2
		 * 0N: 12,309 km^2
		 */

		double thisLat = yToLatitude (y);
		double circumferenceAtLatitude = 2 * Math.PI * radius * Math.Cos(DegreeToRadians (thisLat));

		double widthMeters = circumferenceAtLatitude / 360.0;

		double heightMeters = 2 * Math.PI * radius / 360;

		return  widthMeters * heightMeters;                                        
	}
	//hydrological functions
}
