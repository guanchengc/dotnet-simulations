using System.Numerics;
using System;
using System.Timers;
using System.Collections.Generic;
using Microsoft.Maui.Graphics;
using Serilog.Core;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui;
using System.Reflection;

namespace MauiApp1.Resources.Drawables;

public class CanvasDrawable : IDrawable
{
    public List<Particle> CanvasItems = new List<Particle>();
    public Vector2 pos = new Vector2(200f, 200f);
    public bool spawn = false;
	public float size = 5;
	public Grids<Particle> gridItems = new Grids<Particle>(1000 / 5, 1000 / 5);
    public int indexing = 0;

	private ILogger logger = MauiProgram.CreateLogger<CanvasDrawable>();

	public void Initialise()
    {
        logger.LogDebug("Starting");
        Spawner();
	}

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
		//canvas.FillColor = Colors.Black;
		// canvas.FillCircle(450f, 450f, 400f);


		canvas.FillColor = Colors.Black;
        canvas.FillRectangle(dirtyRect);

		if (CanvasItems.Count > 0)
		{
			for (int i = 0; i < 5; i++)
			{
                
                gridItems.Clear();
                Parallel.ForEach(CanvasItems, item =>
                {
                    item.DrawPos(canvas, 0.05f, CanvasItems, gridItems, size);
                });

                //SolveCollision(CanvasItems);
                //SolveCollisionGrid(gridItems, new Vector2(0,0), new Vector2(gridItems.Width, gridItems.Height));
                SolveColGridPara(10, 10, gridItems);
			}
            foreach (var item in CanvasItems)
            {
                if (item.color == new Color(0, 0, 0)) continue;
                canvas.FillColor = item.color;
				canvas.FillCircle(item.pos.X, item.pos.Y, item.size);
			}
		}
    }

    private void Spawner()
	{
        var spawner = size + 1;
        indexing = 0;
        for (int x = 0; x < 400 / (2*spawner); x++)
        {
            for (int y = 0; y < 50; y++)
            {
                CanvasItems.Add(new Particle(new Vector2(spawner + x * spawner * 2, 1000 - spawner - y * spawner * 2), indexing, spawner));
                indexing ++;
            }
        }
	}

	public void SolveCollision(List<Particle> Origin, List<Particle> CanvasItems)
	{
		foreach (var item in Origin)
		{
            foreach (var item2 in CanvasItems)
            {
                if (item2 == null || item == null) continue;
                if (item2.pos == item.pos) continue;
                var dist = Vector2.Distance(item.pos, item2.pos);
                if (dist < (item2.size + item.size))
                {
                    Vector2 colAxis = item.pos - item2.pos;
                    Vector2 n = colAxis / dist;
                    float delta = item.size + item2.size - dist;
                    item.pos += 0.4f * n * delta;
                    item2.pos -= 0.4f * n * delta;
                    item.CalcTemp(item2);
				}
            }
		}
	}

	private void SolveCollisionGrid(Grids<Particle> grid, Vector2 start, Vector2 end)
	{
        for (int x = (int)start.X; x < end.X + 1; x++)
        {
            for (int y = (int)start.Y; y < end.Y + 1; y++)
			{

				var currentCell = grid.Get(x, y);


				if (currentCell.Count > 0)
				{
					for (int dx = -1; dx <= 1; dx++)
					{
						for (int dy = -1; dy <= 1; dy++)
						{
							//iterates through all surrounding cells
							var a = grid.Get(x + dx, y + dy);
							if (a.Count > 0)
							{
								SolveCollision(currentCell, a);
							}
						}
					}
				}
			}
        }
	}
    private void SolveColGridPara(int countx, int county, Grids<Particle> grid)
    {
        var wi = grid.Width / countx;
        var he = grid.Height / county;
        wi = (int)Math.Ceiling((decimal)wi);

		he = (int)Math.Ceiling((decimal)he);

        Parallel.For(0, county, i =>
        {

            Parallel.For(0, countx, index =>
            {
                SolveCollisionGrid(grid, new Vector2(index * wi, i * he), new Vector2((index + 1) * wi, (i + 1) * he));
            });

        });
		
        
    }

}

public class Particle
{
    public Vector2 pos = new Vector2(100f, 100f);
    public Vector2 prevPos = new Vector2(100f, 100f);
    private float termVel = 2f;

    public Vector2 vel = new Vector2(0f, 0f);
    public Vector2 accel = new Vector2(0f, 0f);
    public Vector2 border = new Vector2(1000f, 1000f);
    public Vector2 gravity = new Vector2(0f, 1f);

    public float temperature;
    public Color color;
    public Vector2 prevGridPos;
	public Vector2 gridPos;

	private int substeps = 8;
    public int id = 0;

    private float fric = -0.9f;
    public float size = 10;
    private float orginSize = 10;

    public Particle(Vector2 pos, int id, float sizeN)
    {
        this.pos = pos;
        this.pos += new Vector2((float)new Random().NextDouble());
        this.prevPos = pos;
        this.size = sizeN;
        this.orginSize = this.size;
        this.id = id;
        this.temperature = 0;
        this.CalcColor();

	}

    public void DrawPos(ICanvas canvas, float dt, List<Particle> Items, Grids<Particle> grid, float pubSize)
    {
        var subDt = dt / substeps;
		this.ApplyGrav();
		this.CalcPos(subDt);
		this.ApplyConst();

		this.gridPos = new Vector2((float)Math.Floor(this.pos.X / (pubSize * 2)), (float)Math.Floor(this.pos.Y / (pubSize * 2)));
		grid.Store(this.gridPos, this);

	}

    public void CalcTemp(Particle obj)
	{
		if (this.temperature > obj.temperature)
		{
            var dif = this.temperature - obj.temperature;
			this.temperature -= dif * 0.4f;
			obj.temperature += dif * 0.4f;
		}

        this.size = this.orginSize * ( 1f + 0f * this.temperature / 100f );

        this.CalcColor();
	}
	public void CalcTemp()
	{
		this.size = this.orginSize * (1f + 0f * this.temperature / 100f);

		this.CalcColor();
	}

	public void CalcColor()
    {
        int r;
        int g;
        int b;
        if (this.temperature > 90)
        {
            r = 255;
            g = 255;
            b = (int)Math.Floor((this.temperature - 90d) / 10d * 255d);
        }
        else if (this.temperature > 75)
        {
            r = 255;
			g = (int)Math.Floor((this.temperature - 75d) / 15d * 255d);
            b = 0;
		}
        else if (this.temperature > 50)
        {
			r = (int)Math.Floor((this.temperature - 50d) / 25d * 255d);
			g = 0;
			b = 0;
		}
        else
        {
            r = 0;
            g = 0;
            b = 255;
        }

        this.color = new Color(r, g, b);
    }

    void Bounce()
    {
        if (this.pos.Y > border.Y)
        {
            this.pos.Y = border.Y;
            this.vel.Y *= this.fric;
        }
        if (this.pos.X > border.X)
        {
            this.pos.X = border.X;
            this.vel.X *= this.fric;
        }
        else if (this.pos.X < 0)
        {
            this.pos.X = 0;
            this.vel.X *= this.fric;
        }
        this.pos.X += this.vel.X;
        if (this.pos.Y >= border.Y & Math.Abs(this.vel.Y) <= 10)
        {
            //on the ground
            this.vel.X *= 1 + this.fric * 0.1f;
        }
        else
        {
            this.pos.Y += this.vel.Y;
            this.vel.Y += this.accel.Y;
        }

        this.vel.X += this.accel.X;
    }

    void CalcPos(float dt)
    {
        if (this.temperature > 0)
		{
			this.temperature -= 0.1f;
		}
        this.vel = this.pos - this.prevPos;
        if (this.vel.Length() > termVel)
        {
            this.vel *= termVel / this.vel.Length();
        }
        this.prevPos = this.pos;
        this.pos = this.pos + this.vel  + this.accel * dt;
        this.accel = new Vector2(0, 0);
	}

    void Accel(Vector2 acc) 
    {
        this.accel = acc;
    }

    void ApplyGrav() 
    {
        /*
        if (this.temperature > 50)
        {
			var acc = new Vector2(0, (float)this.temperature * 0.005f);
			this.Accel(- acc);
		}
        else
        {
			this.Accel(gravity);
		}
        */
        var acc = this.temperature - 40;

		this.Accel(gravity - new Vector2(0, acc * acc * acc * 0.0001f));
    }
	void ApplyConst()
	{
		CalcTemp();
		if (this.pos.Y > border.Y - this.size * 3) 
        {
			if (this.temperature < 100)
			{
				this.temperature += 30;
			}

		}

		if (this.pos.Y > border.Y - this.size)
		{
			this.pos.Y = border.Y - this.size;
            this.prevPos = this.pos;
		}
		else if (this.pos.Y < 0 + this.size)
		{
			this.pos.Y = this.size;
			this.prevPos = this.pos;
		}
		if (this.pos.X > border.X - this.size)
        {
            this.pos.X = border.X - this.size;
			//this.prevPos = this.pos;
		}
        else if (this.pos.X < 0 + this.size)
        {
            this.pos.X = this.size;
			//this.prevPos = this.pos;
		}
	}

    void ApplyConstCirc()
    {
        Vector2 position = new Vector2(450f, 450f);
        float rad = 300;
        float dist = Vector2.Distance(this.pos, position);
        Vector2 to_obj = this.pos - position;

        if (dist > rad - this.size)
        {
            Vector2 n = to_obj / dist;
            this.pos = position + n * (rad - this.size);
        }
    }

}

public class Grids<T>
{
    public Vector2 size;
    public int Width;
    public int Height;
    public float gridSize = 6;
    private List<List<List<T>>> gridList = new List<List<List<T>>>();

    public Grids(float x, float y) 
    { 
        this.size = new Vector2(x, y);
        this.Width = (int)x;
        this.Height = (int)y;

        for (int i = 0; i < x; i++)
		{
			var tempL = new List<List<T>>();
			for (int j = 0; j < y; j++)
            {
                tempL.Add(new List<T>());
            }
            this.gridList.Add(tempL);
        }
    }

    public void Store(Vector2 pos, T obj)
    {
		this.gridList[(int)pos.X][(int)pos.Y].Add(obj);
    }

    public void Clear()
    {
        for (int j = 0; j < this.gridList.Count; j++)
		{
			for (int i = 0; i < this.gridList.Count; i++)
			{
                this.gridList[j][i].Clear();

			}

		}
	}

	public List<T> Get(int x, int y)
	{
        if (x < 0 || y < 0 || x >= Width || y >= Height) return new List<T>();
		return this.gridList[x][y];
	}
}