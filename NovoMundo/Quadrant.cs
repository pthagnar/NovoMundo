using System;

public class Biome
{
	public string BiomeName { get; set;}
	public float Albedo {get; set;}
	public string TileResource { get; set;}

	public void CheckTileDict()
	{
		if (!MainClass.BiomeTileDict.ContainsKey(TileResource))
			{
			MainClass.BiomeTileDict.Add (TileResource, Gdk.Pixbuf.LoadFromResource (TileResource));
			}
	}

	public Biome()
	{
		;
	}
}

public class Quadrant
	{
		//geometry
		public double latitude { get; set; }
		public double longitude { get; set; }

		public string coordinateString{ get; set; }

		public Earth.Direction steepestDecline { get; set;}

		public long area { get; set;} //m^2

		//hydrology
		public int elevation { get; set; } //m
		public bool isWater { get; set; }
		public int depth { get; set; } //m

		//atmosphere
		public long GeothermalHeat // W
		{
			get
			{
			return (long)(area * 0.0916);
			}
			set
			{
			this.GeothermalHeat = value;
			}

		}
		
		public long cloudtopInsolation // W
		{
			get 
			{
			return (long)(area * Earth.SolarConstant 
							   * Math.Cos(Math.PI * latitude / 180.0) // lower incidence rays at higher latitude 
				               / 2);  //it is dark at night
			}

			set 
			{
				this.cloudtopInsolation = value;
			}
		}

		public long atmosphereInsolation // W
		{
			get 
			{
			return (long)(this.cloudtopInsolation * (1 - Earth.CloudtopAlbedo));
			}

			set 
			{
				this.atmosphereInsolation = value;
			}
		}

		public long atmosphereAbsorption // W
		{
			get 
			{
			return (long)(this.atmosphereInsolation * (1 - Earth.AtmosphericAbsorption));
			}

			set 
			{
				this.atmosphereAbsorption = value;
			}
		}
		
		public float airTemperature // degC -- very crude empirical linear fit to heat flux -20 => 0 GW 30 => 5000 GW
			{
				get
				{	
					long absorbGigaWatts = atmosphereAbsorption / 1000000000;
					float seaLevelTemp = (float)((absorbGigaWatts * 0.02) - 20);

					float reliefTemp;

					if (elevation > Earth.SeaLevel)
					{
						reliefTemp = seaLevelTemp - elevation * 0.0065f;
					} 
					else 
					{
						reliefTemp = seaLevelTemp;
					}

					return reliefTemp;
				}

				set
				{
					this.airTemperature = value;
				}
			}

		public long surfaceIncidentInsolation // W -- BEFORE SURFACE ALBEDO
		{
			get 
			{
				return this.atmosphereInsolation - this.atmosphereAbsorption;
			}

			set 
			{
				this.surfaceIncidentInsolation = value;
			}
		}

		public long surfaceInsolation // W -- AFTER SURFACE ALBEDO ~~bioavailable~~
		{
			get 
			{
				return (long)(surfaceIncidentInsolation * (1 - quadrantBiome.Albedo));
			}

			set 
			{
				this.surfaceInsolation = value;
			}
		}

			public double cloudCover { get; set; } //between 0 and 1

			public int precipitation //mm/yr
			{ 
				get 
				{
					return (int)Math.Abs (1750 * -Math.Cos (3 * (Math.PI * latitude / 180)));
				}

				set 
				{
					this.precipitation = value;
				}
			} 

		//biology

	public Biome quadrantBiome  { get; set; }

	// BEGIN METHODS

		// geometry
		public string GetCoordinateString()
		{
			string latletter;
			string longletter;

			if (longitude > 0)
			{
				longletter = "E";
			}
			else if (longitude <= 0)
			{
				longletter = "W";
			}
			else
			{
				longletter = "";
			}

			if (latitude > 0)
			{
				latletter = "N";
			}
			else if (latitude <= 0)
			{
				latletter = "S";
			}
			else
			{
				latletter = "";
			}

			string s = Math.Abs(latitude) + "°" + latletter + " "
				+ Math.Abs(longitude) + "°" + longletter;

			return s;

		}

		// hydrology
		void SetWater()
		{
			if (elevation <= Earth.SeaLevel) 
			{
				isWater = true;
				depth = elevation - Earth.SeaLevel;
			}
		}

		// biology

		Biome DetermineBiome()
		{
			Biome thisBiome = new Biome ();

		//lock ice caps up before all
		if (airTemperature < -15 && elevation <= Earth.SeaLevel) 
		{
			thisBiome.BiomeName = "ice cap";
			thisBiome.Albedo = 0.6f;
			thisBiome.TileResource = "NovoMundo.graphics.tiles.ice.gif";
		}
		else if (airTemperature < -15 && elevation > Earth.SeaLevel)
		{
			thisBiome.BiomeName = "glacier";
			thisBiome.Albedo = 0.6f;
			thisBiome.TileResource = "NovoMundo.graphics.tiles.civ2tiles.glacier.png";
		}
		else if (elevation < Earth.ShallowSeaLevel)
		{
			thisBiome.BiomeName = "deep ocean";
			thisBiome.Albedo = 0.06f;
			thisBiome.TileResource = "NovoMundo.graphics.tiles.civ2tiles.ocean.png";
		}
		else if (elevation >= Earth.ShallowSeaLevel && elevation <= Earth.SeaLevel)
		{
			thisBiome.BiomeName = "shallow ocean";
			thisBiome.Albedo = 0.06f;
			thisBiome.TileResource = "NovoMundo.graphics.tiles.civ2tiles.shallows.png";
		}
		else if (elevation >= 1000 && elevation < 2000)
		{
			thisBiome.BiomeName = "highlands";
			thisBiome.Albedo = 0.5f;
			thisBiome.TileResource = "NovoMundo.graphics.tiles.civ2tiles.deserthill.png";
		}	
		else if (elevation >= 2000)
		{
			thisBiome.BiomeName = "montane";
			thisBiome.Albedo = 0.5f;
			thisBiome.TileResource = "NovoMundo.graphics.tiles.civ2tiles.mountain.png";
		}
		else
		{
			thisBiome.BiomeName = "lowlands";
			thisBiome.Albedo = 0.5f;
			thisBiome.TileResource = "NovoMundo.graphics.tiles.civ2tiles.desert.png";
		}

		/*else 
		{
			//elevation biomes
			if (elevation < Earth.ShallowSeaLevel)
			{
				thisBiome.BiomeName = "deep ocean";
				thisBiome.Albedo = 0.06f;
				thisBiome.TileResource = "NovoMundo.graphics.tiles.civ2tiles.ocean.png";
			}
			else if (elevation >= Earth.ShallowSeaLevel && elevation <= Earth.SeaLevel)
			{
				thisBiome.BiomeName = "shallow ocean";
				thisBiome.Albedo = 0.06f;
				thisBiome.TileResource = "NovoMundo.graphics.tiles.civ2tiles.shallows.png";
			}

			//biotic biomes
			else 
			{
					if (elevation > 2000 && airTemperature >= 10 && precipitation >= 1500) {
						thisBiome.BiomeName = "montane forest";
						thisBiome.Albedo = 0.16f;
						thisBiome.TileResource = "NovoMundo.graphics.tiles.civ2tiles.boreal.png";
					}
				else if (elevation > 2000 && ((airTemperature >= 6 && airTemperature < 10) || precipitation < 1500))
					{
						thisBiome.BiomeName = "alpine tundra";
						thisBiome.Albedo = 0.4f;
						thisBiome.TileResource = "NovoMundo.graphics.tiles.civ2tiles.hill.png";	
					}
				else if ((elevation > 2000 && airTemperature < 6) || elevation > 5000)
					{
						thisBiome.BiomeName = "mountain peaks";
						thisBiome.Albedo = 0.8f;
						thisBiome.TileResource = "NovoMundo.graphics.tiles.civ2tiles.mountain.png";	
					}
					else if (precipitation >= 1500 && airTemperature > 5) 
					{
						thisBiome.BiomeName = "woodland";
						thisBiome.Albedo = 0.16f;
					thisBiome.TileResource = "NovoMundo.graphics.tiles.civ2tiles.broadleaf.png";
					}
					else if (precipitation >= 1500 && airTemperature <= 5) 
					{
						thisBiome.BiomeName = "boreal forest";
						thisBiome.Albedo = 0.4f;
						thisBiome.TileResource = "NovoMundo.graphics.tiles.civ2tiles.boreal.png";
					}
					else if (precipitation >= 600 && airTemperature <= -10) 
					{
						thisBiome.BiomeName = "tundra";
						thisBiome.Albedo = 0.4f;
						thisBiome.TileResource = "NovoMundo.graphics.tiles.civ2tiles.tundra.png";
					}
					else if (precipitation < 1500 && precipitation >= 600) 
					{
						thisBiome.BiomeName = "grassland";
						thisBiome.Albedo = 0.25f;
						thisBiome.TileResource = "NovoMundo.graphics.tiles.civ2tiles.grassland.png";
					}
					else 
					{
						thisBiome.BiomeName = "desert";
						thisBiome.Albedo = 0.25f;
					thisBiome.TileResource = "NovoMundo.graphics.tiles.civ2tiles.desert.png";
					}*/
			return thisBiome;

		}

		public int GetPrimaryHash()
		{
			return coordinateString.GetHashCode ();
		}

		public int GetSecondaryHash()
		{
			return (int)(latitude * longitude);
		}



		public void Update()
		{
			SetWater ();
			quadrantBiome = DetermineBiome();
			quadrantBiome.CheckTileDict ();
		}

		public Quadrant(double x, double y)
		{
			latitude = y;
			longitude = x;

			coordinateString = GetCoordinateString();

			quadrantBiome = DetermineBiome ();
			quadrantBiome.CheckTileDict ();

		}

	}

	