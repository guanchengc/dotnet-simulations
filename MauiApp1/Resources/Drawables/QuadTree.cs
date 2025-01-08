using Microsoft.Maui.Primitives;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Resources.Drawables
{
	public interface IPosition 
	{
		Vector2 pos { get; set; }

		void Draw(ICanvas canvas, QuadTree<FlyingBody> quadTree, List<FlyingBody> flyingBodies);
	}
	public class QuadTree<T> where T : IPosition
	{
		Vector2 position = new Vector2();
		Vector2 dimention = new Vector2();
		RectF boundary = new RectF();

		float minSize = 10f;

		int capacity = 0;
		List<T> listOfItems = new List<T>();
		QuadTree<T> nw;
		QuadTree<T> sw;
		QuadTree<T> se;
		QuadTree<T> ne;

		
		bool divided = false;

		public QuadTree(Vector2 pos, Vector2 dim, int cap) 
		{
			position = pos;
			dimention = dim;
			capacity = cap;
			boundary = new RectF(pos.X - dimention.X / 2, pos.Y - dimention.Y / 2, dimention.X, dimention.Y);
		}

		public QuadTree() { }

		private void Subdivide()
		{
			Vector2 newDim = dimention / 2;
			nw = new QuadTree<T>(position - newDim / 2, newDim, capacity);
			sw = new QuadTree<T>(new Vector2(position.X - newDim.X / 2, position.Y + newDim.Y / 2), newDim, capacity);
			se = new QuadTree<T>(position + newDim / 2, newDim, capacity);
			ne = new QuadTree<T>(new Vector2(position.X + newDim.X / 2, position.Y - newDim.Y/ 2), newDim, capacity);

			divided = true;
		}

		public bool Insert(T item, Vector2 pos) 
		{
			if (pos.X >= position.X + dimention.X / 2 || pos.Y >= position.Y + dimention.Y / 2 || pos.X < pos.X - dimention.X / 2 || pos.Y < pos.Y - dimention.Y / 2) 
			{
				return false;
			}

			if (dimention.X / 2 < minSize)
			{
				listOfItems.Add(item);
				return true;
			}

			if (listOfItems.Count < capacity)
			{
				listOfItems.Add(item);
				return true;
			}
			else 
			{
				if (!divided)
				{
					Subdivide();
				}

				if (nw.Insert(item, pos))
				{
					return true;
				}
				else if (sw.Insert(item, pos))
				{
					return true;
				}
				else if (ne.Insert(item, pos))
				{
					return true;
				}
				else
				{
					se.Insert(item, pos);
					return true;
				}
			}
		}
		public void Draw(ICanvas canvas)
		{
			canvas.StrokeColor = Colors.Black;
			canvas.StrokeSize = 3;
			canvas.DrawRectangle(boundary);

			if (divided)
			{
				sw.Draw(canvas);
				nw.Draw(canvas);
				se.Draw(canvas);
				ne.Draw(canvas);
			}
		}

		/*
		public bool Query(Vector2 range)
		{
			if (range.X )
			{
				return true;
			}
		}

		bool Intersects(Vector2 posit, Vector2 size)
		{
			Vector4 range = new Vector4(posit - size / 2, posit.X + size.X / 2, posit.Y + size.Y / 2);

			return !(range.X > boundary.X + boundary ||
				range.Y >
				)
		}*/
	}
}
