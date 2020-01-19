using System;
using Gtk;

public partial class QuadrantDialog : Gtk.Dialog
{
	Gdk.Pixbuf wetPicture = Gdk.Pixbuf.LoadFromResource("NovoMundo.graphics.quadviews.quadviewwet.png");
	Gdk.Pixbuf dryPicture = Gdk.Pixbuf.LoadFromResource("NovoMundo.graphics.quadviews.quadview.png");

	public void UpdateCoords(int x, int y)
	{
		this.Title = "Quadrant " + x +", " + y;
	}

	public void SetText (QuadrantInfo quadInfo)
	{
		QuadrantTextView.Buffer.Text = "";

		string biomeString = quadInfo.biomeName;
		QuadrantTextView.Buffer.Text += biomeString + "\n";

		string locationString = quadInfo.x.ToString () + ", " + quadInfo.y.ToString ();
		QuadrantTextView.Buffer.Text += "Location: " + locationString + "\n";

		string coordinateString = quadInfo.coordinateString;
		QuadrantTextView.Buffer.Text += "Coordinates: " + coordinateString + "\n";

		string elevationString = quadInfo.elevation.ToString ();
		QuadrantTextView.Buffer.Text += "Elevation: " + elevationString + " m \n";

		string areaString = (quadInfo.area/1000000).ToString ();
		QuadrantTextView.Buffer.Text += "Surface Area: " + areaString + " km² \n";

		string wayDownString = quadInfo.wayDown.ToString ();
		QuadrantTextView.Buffer.Text += "River Exit: " + wayDownString +  "\n";

		/*
		string cloudtopSunshineString = (quadInfo.cloudtopInsolation/1000000000).ToString ();
		QuadrantTextView.Buffer.Text += "Insolation (Cloudtop): " + cloudtopSunshineString + " GW \n";

		string airHeatFluxString = (quadInfo.atmosphereAbsorption/1000000000).ToString ();
		QuadrantTextView.Buffer.Text += "Atmospheric Absorption Flux: " + airHeatFluxString + " GW \n";
		*/

		string airTempString = (quadInfo.airTemperature).ToString ();
		QuadrantTextView.Buffer.Text += "Average Air Temperature: " + airTempString + "°C \n";

		string precipString = (quadInfo.precipitation).ToString ();
		QuadrantTextView.Buffer.Text += "Average Precipitation: " + precipString + "mm/yr \n";

		string groundSunshineString = (quadInfo.surfaceInsolation/1000000000).ToString ();
		QuadrantTextView.Buffer.Text += "Insolation (Surface): " + groundSunshineString + " GW \n";

		string geoFluxString = (quadInfo.geothermalFlux/1000000).ToString ();
		QuadrantTextView.Buffer.Text += "Geothermal Flux: " + geoFluxString + " MW \n";

	}

	public void SetPicture(bool isWater)
	{
		if (isWater)
		{
			QuadrantImage.Pixbuf = wetPicture;
		} 
		else 
		{
			QuadrantImage.Pixbuf = dryPicture;
		}
	}



	public QuadrantDialog ()
	{
		this.Build ();
}



	protected void OnDeleteQuadEvent (object o, DeleteEventArgs args)
	{
		this.Hide ();
		args.RetVal = true;
	}
}

