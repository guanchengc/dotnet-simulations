using MauiApp1.Resources.Drawables;
using System.Timers;

namespace MauiApp1;

public partial class Lorenz : ContentPage
{
	public GraphicsView graphicsView;

	public Lorenz()
	{
		InitializeComponent();
		graphicsView = this.Canvas;
		var canvasDrawable = (LorenzDrawable)graphicsView.Drawable;
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
		var canvasDrawable = (LorenzDrawable)graphicsView.Drawable;

		graphicsView.Invalidate();
	}
}