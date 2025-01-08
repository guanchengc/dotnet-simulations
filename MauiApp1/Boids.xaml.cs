using MauiApp1.Resources.Drawables;
using System.Timers;

namespace MauiApp1;

public partial class Boids : ContentPage
{
	public GraphicsView graphicsView;

	public Boids()
	{
		InitializeComponent();
		graphicsView = this.Canvas;
		var canvasDrawable = (BoidsDrawable)graphicsView.Drawable;
		canvasDrawable.Initialise();
		var timer = new System.Timers.Timer(10);
		timer.Elapsed += new ElapsedEventHandler(RedrawGraphics);
		timer.Start();
	}

	private void ContentPage_Loaded(object sender, EventArgs e)
	{

	}

	public void RedrawGraphics(object source, ElapsedEventArgs e)
	{
		var canvasDrawable = (BoidsDrawable)graphicsView.Drawable;

		graphicsView.Invalidate();
	}
}