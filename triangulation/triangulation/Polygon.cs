using System;
using System.Drawing;
using System.Windows.Forms;

namespace triangulation
{
    class Polygon
    {
        private PointF[] points; //вершины нашего многоугольника
        private Triangle[] triangles; //треугольники, на которые разбит наш многоугольник
        private bool[] taken; //была ли рассмотрена i-ая вершина многоугольника

        public Polygon(float[] points) //points - х и y координаты
        {
            this.points = new PointF[points.Length / 2]; //преобразуем координаты в вершины
            for (int i = 0; i < points.Length; i += 2)
                this.points[i / 2] = new PointF(points[i], points[i + 1]);

            triangles = new Triangle[this.points.Length - 2];

            taken = new bool[this.points.Length];

            triangulate(); //триангуляция
        }

        private void triangulate() //триангуляция
        {
            int trainPos = 0; //
            int leftPoints = points.Length; //сколько осталось рассмотреть вершин

            //текущие вершины рассматриваемого треугольника
            int ai = findNextNotTaken(0);
            int bi = findNextNotTaken(ai + 1);
            int ci = findNextNotTaken(bi + 1);

            int count = 0; //количество шагов

            while (leftPoints > 3) //пока не остался один треугольник
            {
                if (isLeft(points[ai], points[bi], points[ci]) && canBuildTriangle(ai, bi, ci)) //если можно построить треугольник
                {
                    triangles[trainPos++] = new Triangle(points[ai], points[bi], points[ci]); //новый треугольник
                    taken[bi] = true; //исключаем вершину b
                    leftPoints--;
                    bi = ci;
                    ci = findNextNotTaken(ci + 1); //берем следующую вершину
                }
                else
                { //берем следущие три вершины
                    ai = findNextNotTaken(ai + 1);
                    bi = findNextNotTaken(ai + 1);
                    ci = findNextNotTaken(bi + 1);
                }

                if (count > points.Length * points.Length)
                { //если по какой-либо причине (например, многоугольник задан по часовой стрелке) триангуляцию провести невозможно, выходим
                    triangles = null;
                    break;
                }

                count++;
            }

            if (triangles != null) //если триангуляция была проведена успешно
                triangles[trainPos] = new Triangle(points[ai], points[bi], points[ci]);
        }

        private int findNextNotTaken(int startPos) //найти следущую нерассмотренную вершину
        {
            startPos %= points.Length;
            if (!taken[startPos])
                return startPos;

            int i = (startPos + 1) % points.Length;
            while (i != startPos)
            {
                if (!taken[i])
                    return i;
                i = (i + 1) % points.Length;
            }

            return -1;
        }

        private bool isLeft(PointF a, PointF b, PointF c) //левая ли тройка векторов
        {
            float abX = b.X - a.X;
            float abY = b.Y - a.Y;
            float acX = c.X - a.X;
            float acY = c.Y - a.Y;

            return abX * acY - acX * abY < 0;
        }

        private bool isPointInside(PointF a, PointF b, PointF c, PointF p) //находится ли точка p внутри треугольника abc
        {
            float ab = (a.X - p.X) * (b.Y - a.Y) - (b.X - a.X) * (a.Y - p.Y);
            float bc = (b.X - p.X) * (c.Y - b.Y) - (c.X - b.X) * (b.Y - p.Y);
            float ca = (c.X - p.X) * (a.Y - c.Y) - (a.X - c.X) * (c.Y - p.Y);

            return (ab >= 0 && bc >= 0 && ca >= 0) || (ab <= 0 && bc <= 0 && ca <= 0);
        }

        private bool canBuildTriangle(int ai, int bi, int ci) //false - если внутри есть вершина
        {
            for (int i = 0; i < points.Length; i++) //рассмотрим все вершины многоугольника
                if (i != ai && i != bi && i != ci) //кроме троих вершин текущего треугольника
                    if (isPointInside(points[ai], points[bi], points[ci], points[i]))
                        return false;
            return true;
        }

        public PointF[] getPoints() //возвращает вершины
        {
            return points;
        }

        public Triangle[] getTriangles() //возвращает треугольники
        {
            return triangles;
        }

    }

    public class Triangle //треугольник
    {
        private PointF a, b, c;

        public Triangle(PointF a, PointF b, PointF c)
        {
            this.a = a;
            this.b = b;
            this.c = c;
        }

        public PointF getA()
        {
            return a;
        }

        public PointF getB()
        {
            return b;
        }

        public PointF getC()
        {
            return c;
        }
    }
}
