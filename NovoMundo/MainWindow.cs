using System;
using Gtk;
using GLib;


public partial class MainWindow: Gtk.Window
{

	QuadrantDialog quad = new QuadrantDialog();

	bool queryModeOn = false;
	bool running = false;
	bool halt = false;

	int simulationSpeed = 1000; //second by default
	int desiredSimulationSpeed = 1000;

	int tickCount = 0;
	int yearCount = 0;

	int zoomLevel {get; set;} = 16;

	public void PlotTile(string tileResource, int x, int y)
	{
		Plotter(MainClass.BiomeTileDict[tileResource], x, y);
	}

	void Plotter(Gdk.Pixbuf tileName, int x, int y)
	{
		tileName.Composite(TileMap.Pixbuf,
			x, y,
			zoomLevel,  zoomLevel,
			(float)x, (float)y,
			1.0, 1.0,
			Gdk.InterpType.Bilinear,
			255);
	}

	public void PlotQuad (int x, int y)
		{
			Quadrant thisQuad = Earth.grid [x, y];
			PlotTile (thisQuad.quadrantBiome.TileResource, x * zoomLevel, y * zoomLevel);
			thisQuad.Update ();

		}

	void PlotCity(int x, int y)
	{
		Gdk.Pixbuf city = Gdk.Pixbuf.LoadFromResource("NovoMundo.graphics.civ2cities.classical.png");
		Plotter (city, x*zoomLevel, y*zoomLevel);
	}

	public MainWindow () : base (Gtk.WindowType.Toplevel)
	{
		Build ();

		TileMap.AppPaintable = true;

		//pixbuf seems to need a blank file the size of the map, so we give it one
		TileMap.Pixbuf = Gdk.Pixbuf.LoadFromResource("NovoMundo.graphics.large.x1PixCanvasWhite.png")
			.ScaleSimple (360 * zoomLevel, 180 * zoomLevel, Gdk.InterpType.Nearest);


		for (int i = 0; i < 360; i++) 
		{
			for (int j = 0; j < 180; j++)
			{
				Quadrant thisQuad = Earth.grid [i, j];
				PlotQuad (i, j);
			}
		}
			
		//hide the quadrant dialog until we ask for it
		quad.Hide ();

		OutputBox.Buffer.Text = "";
		UpdateTimeStatusBar ();

	}
		
	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

	protected void OnQuit (object sender, EventArgs e)
	{
		Application.Quit ();
	}

	protected void UpdateQuadrantDialog (QuadrantInfo quadInfo)
	{

		quad.UpdateCoords (quadInfo.x, quadInfo.y);
		quad.SetPicture (quadInfo.isWater);
		quad.SetText (quadInfo);
		quad.Show ();

	}

	protected void UpdateTimeStatusBar()
	{
		TimeStatusBar.Push (1, "T = " + tickCount + ", " +
			yearCount + " MY");
	}

	public void AppendToBox( string addend)
	{
		OutputBox.Buffer.Text += addend;
	}

	public void AppendLineToBox( string addend)
	{
		AppendToBox(addend + "\n");
	}

	protected void OnClearBox (object sender, EventArgs e)
	{
		OutputBox.Buffer.Text = "";

	}

	protected void OnTileMapBoxButtonEvent (object o, ButtonPressEventArgs args)
	{
		int xTile = (int)Math.Floor(args.Event.X/zoomLevel);
		int yTile = (int)Math.Floor(args.Event.Y/zoomLevel);
		if (queryModeOn) 
		
		{
			StatusBar.Push (1, xTile.ToString () + ", " + yTile.ToString ());
			QuadrantInfo thisInfo = MainClass.RequestQuadrantInfo (xTile, yTile);
			UpdateQuadrantDialog (thisInfo);
		}
	}
	protected void OnQueryButtonToggled (object sender, EventArgs e)
	{
		PlotCity (177, 36);
		queryModeOn = !queryModeOn;
		if (!queryModeOn)
		{
			quad.Hide ();
		}
	}

	//ADD ALL SIMULATION UPDATE STUFF HERE
	bool updateWorld()
	{
		halt = false;
		tickCount++;
		UpdateTimeStatusBar ();
		TileMap.QueueDraw ();

		if (running)
		{
			return true;
		}
		else 
		{
			return false;
		}


	}
	protected void OnPresButanToggled (object sender, EventArgs e)
	{
		if (!running) {
			GLib.Timeout.Add ((uint)simulationSpeed, new GLib.TimeoutHandler (updateWorld));
		}
			running = !running;
			
	}
		
	protected void OnSpeedUp (object sender, EventArgs e)
	{
		if (simulationSpeed < 10000)
		{
			desiredSimulationSpeed += 500;
		}
	}
	protected void OnSlowDown (object sender, EventArgs e)
	{
		if (simulationSpeed > 1000) 
		{
			desiredSimulationSpeed -= 500;
		}
	}
}

