using Microsoft.Maui.Animations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Resources.Drawables;
public class LorenzDrawable : IDrawable
{
	float persp = 0;
	float rot = 0;
	float dist = 70;
	Vector2 center = new Vector2(500, 500);
	Vector3 camera = new Vector3(0, 0, 60);

	public List<Dot3d> CanvasItems = new List<Dot3d>();
	public List<Vector3> Dots = new List<Vector3>();

	public LorenzDrawable()
	{

	}

	public void Initialise()
	{
		CanvasItems.Add(new Dot3d(0.01f, 0, 0));
	}
	public void Draw(ICanvas canvas, RectF dirtyRect)
	{
		rot += 0.01f;
		camera = new Vector3(100f * (float)Math.Sin(rot), 0, 100f * (float)Math.Cos(rot));
		if (CanvasItems.Count > 0)
		{
			foreach (var item in CanvasItems)
			{
				item.Draw(canvas, Dots);
			}
		}
		if (Dots.Count > 0)
		{
			PathF path = new PathF();
			path.MoveTo(Projection(Dots[0]) + center);
			foreach (var item in Dots)
			{
				path.LineTo(1000 * Projection(item) + center);
				//path.LineTo(5 * new Vector2(item.X, item.Y) + center);
			}
			canvas.StrokeColor = Colors.Black;
			canvas.StrokeSize = 1;
			canvas.DrawPath(path);
		}
	}

	public Vector3 Rotate(Vector3 point)
	{
		return new Vector3((float)(point.X * Math.Cos(rot) - point.Z * Math.Sin(rot)), point.Y, (float)(point.X * Math.Sin(rot) + point.Z * Math.Cos(rot)));
	}

	public Vector3 Rotate2(Vector3 point)
	{
		var newP = Rotate(point);
		newP = new Vector3(newP.X, (float)(newP.Y * Math.Cos(persp) - newP.Z * Math.Sin(persp)), (float)(newP.Y * Math.Sin(persp) + newP.Z * Math.Cos(persp)));
		return newP;
	}

	public Vector2 Projection(Vector3 point)
	{
		var newP = Rotate2(point - camera);
		return new Vector2(newP.X / newP.Z, newP.Y / newP.Z);
	}
}

public class Dot3d
{
	public float x = 0;
	public float y = 0;
	public float z = 0;

	float dx = 0;
	float dy = 0;
	float dz = 0;
	float s = 10;
	float rho = 28;
	float b = 8 / 3;



	public Dot3d(float x, float y, float z) 
	{
		this.x = x;
		this.y = y;	
		this.z = z;
	}

	public void Draw(ICanvas canvas, List<Vector3> li)
	{
		float dt = 0.01f;

		dx = (s * (y - x)) * dt;
		dy = (x * (rho - z) - y) * dt;
		dz = (x * y - b * z) * dt;
		x += dx;
		y += dy;
		z += dz;

		li.Add(new Vector3(x, y, z));
	}

}

