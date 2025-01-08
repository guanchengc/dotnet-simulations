using MauiApp1.Resources.Drawables;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Dispatching;
using Microsoft.Maui.Graphics;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Timers;

namespace MauiApp1;

public partial class NewPage1 : ContentPage
{
    public GraphicsView graphicsView;

	public NewPage1()
	{
		InitializeComponent();
        graphicsView = this.Canvas;
        var canvasDrawable = (CanvasDrawable)graphicsView.Drawable;
        canvasDrawable.Initialise();
        var timer = new System.Timers.Timer(50);
        timer.Elapsed += new ElapsedEventHandler(RedrawGraphics);
        timer.Start();
    }

    private void ContentPage_Loaded(object sender, EventArgs e)
    {

    }

	private void OnButtonClicked(object sender, EventArgs e)
	{
		var canvasDrawable = (CanvasDrawable)graphicsView.Drawable;

		graphicsView.Invalidate();
	}

    private void OnButtonUnClick(object sender, EventArgs e)
	{
		var canvasDrawable = (CanvasDrawable)graphicsView.Drawable;
		canvasDrawable.spawn = false;
	}

	public void RedrawGraphics(object source, ElapsedEventArgs e) 
    {
        var canvasDrawable = (CanvasDrawable)graphicsView.Drawable;

        graphicsView.Invalidate();
    }
}

