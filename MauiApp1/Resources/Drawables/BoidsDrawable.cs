

using System.Numerics;

namespace MauiApp1.Resources.Drawables;
public class BoidsDrawable : IDrawable
{
	FlyingBody FlyingBody = new FlyingBody(new Vector2(500, 500), new Vector2(3, 3));
	float detectRad = 10;
	QuadTree<FlyingBody> quadTree;
	Random random = new Random();


	List<FlyingBody> ListOfFlying = new List<FlyingBody>();

	public BoidsDrawable()
	{

	}
	public void Initialise()
	{
		//quadTree = new QuadTree<FlyingBody>(new Vector2(400, 400), new Vector2(800, 800), 5);
		for (int i = 0; i < 100; i++)
		{
			Vector2 pos = new Vector2((float)random.NextDouble() * 100, (float)random.NextDouble() * 100);
			FlyingBody temp = new FlyingBody(pos, new Vector2((float)random.NextDouble() * 10 - 5, (float)random.NextDouble() * 10 - 5));
			ListOfFlying.Add(temp);
			//quadTree.Insert(temp, pos);
		}
	}

	public void Draw(ICanvas canvas, RectF dirtyRect)
	{
		//quadTree.Draw(canvas);
		//QuadTree<FlyingBody> tempQuadTree = new QuadTree<FlyingBody>(new Vector2(400, 400), new Vector2(800, 800), 5);

		foreach (var item in ListOfFlying)
		{
			item.Draw(canvas, null, ListOfFlying);
		}

		//quadTree = tempQuadTree;
		//tempQuadTree = null;
	}

}

public class FlyingBody : IPosition
{ 
	Vector2 pos = new Vector2();
	Vector2 vel = new Vector2();
	Vector2 acel = new Vector2();
	float detectRange = 30;
	float maxVel = 4;
	float maxForce = 0.2f;

	Vector2 IPosition.pos { get => pos; set => pos = value; }

	List<FlyingBody> neighbours = new List<FlyingBody>();

	public FlyingBody(Vector2 pos, Vector2 vel) 
	{
		this.pos = pos;
		this.vel = vel;
	}

	void Boundary()
	{
		if (pos.X > 800)
		{
			pos.X = 0;
		}
		else if (pos.X < 0)
		{
			pos.X = 800;
		}
		if (pos.Y > 800)
		{
			pos.Y = 0;
		}
		else if (pos.Y < 0)
		{
			pos.Y = 800;
		}
	}

	void ApplyForce(List<FlyingBody> flyingBodies)
	{
		int count = 0;
		Vector2 neiVel = new Vector2();
		Vector2 neiAvg = new Vector2();
		Vector2 neiAvoid = new Vector2();

		foreach (var item in flyingBodies)
		{
			var dist = Vector2.Distance(item.pos, pos);
			if (item != this && dist <= detectRange)
			{
				neiVel += item.vel;
				neiAvg += item.pos;

				var dif = pos - item.pos;
				dif *= 1 / dist;
				neiAvoid += dif;

				count += 1;
			}
		}

		if (count > 0)
		{
			neiVel /= count;
			neiAvg /= count;
			neiAvoid /= count;

			neiAvg -= pos;
			neiVel *= maxVel / neiVel.Length();
			neiAvg *= maxVel / neiAvg.Length();
			neiAvoid *= maxVel / neiAvoid.Length();

			neiVel -= vel;
			neiAvg -= vel;
			neiAvoid -= vel;
			if (neiVel.Length() > maxForce)
			{
				neiVel *= maxForce / neiVel.Length();
			}
			if (neiAvg.Length() > maxForce)
			{
				neiAvg *= maxForce / neiAvg.Length();
			}
			if (neiAvoid.Length() > maxForce)
			{
				neiAvoid *= maxForce / neiAvoid.Length();
			}

			acel = neiAvg + neiVel + neiAvoid;


			if (acel.Length() > maxForce)
			{
				acel *= maxForce / acel.Length();
			}
		}
	}

	public void Draw(ICanvas canvas, QuadTree<FlyingBody> quadTree, List<FlyingBody> flyingBodies)
	{
		ApplyForce(flyingBodies);
		pos += vel;
		vel += acel;

		if (vel.Length() > maxVel)
		{
			vel *= maxVel / vel.Length();
		}

		Boundary();
		canvas.FillColor = Colors.Black;
		canvas.FillCircle(pos, 5);
		acel = new Vector2();

		//quadTree.Insert(this, pos);
	}
}