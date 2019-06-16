using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace triangulation
{
    public partial class MainForm : Form
    {
        private Pen polygonPen; //для рисования контура многоугольника
        private SolidBrush trianBrush; //для рисования треугольников

        private Graphics g;
        private Bitmap bitmap;

        private Polygon polygon; //многоугольник
        private int drawCount = 0; //количество нарисованных треугольников в данный момент
        private Color[] colors; //цвета треугольников

        private float translateX, translateY, scale;

        public MainForm()
        {
            InitializeComponent();
        }

        private void drawPolygon() //рисует многоугольник
        {
            g.TranslateTransform(translateX, translateY); //центрируем
            g.ScaleTransform(scale, scale); //масштабируем

            //рисуем контур
            PointF[] points = polygon.getPoints();
            g.DrawLines(polygonPen, points);
            g.DrawLine(polygonPen, points[0], points[points.Length - 1]);

            GraphicsPath p = new GraphicsPath();
            Triangle[] trians = polygon.getTriangles();

            if (trians == null)
                return;

            //рисуем drawCount треугольников
            for (int i = 0; i < drawCount; i++){
                Triangle t = trians[i];

                p.Reset();
                p.AddLine(t.getA(), t.getB());
                p.AddLine(t.getB(), t.getC());
                p.AddLine(t.getC(), t.getA());

                trianBrush.Color = colors[i];
                g.FillPath(trianBrush, p);
            }
            drawCount = 0;
            
        }


        private void OnClear(object sender, EventArgs e)
        {
            pictureBox1.Image = null;
        }

        private void OnPrevious(object sender, EventArgs e)
        {
            new FigureForm().Show();
        }

        private void OnDraw(object sender, EventArgs e)
        {
            string[] strCoord = textBox1.Text.Split(',');
            float[] fltCoord = Array.ConvertAll(strCoord, float.Parse);
            if(fltCoord.Length % 2 == 1 || fltCoord.Length < 6)
            {
                MessageBox.Show("Введенные координаты не являются многоугольником", "Ошибка!");
            }
            else
            triangulate(fltCoord);
        }

        private void draw()
        {
            bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g = Graphics.FromImage(bitmap);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            drawPolygon();

            pictureBox1.Image = bitmap;
        }

        private void OnRandomize(object sender, EventArgs e)
        {
            byte[] bytes = new byte[1024];
            // Устанавливаем удаленную точку для сокета

            IPAddress ipAddr = IPAddress.Loopback;
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 8080);

            Socket sender1 = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Соединяем сокет с удаленной точкой
            sender1.Connect(ipEndPoint);

         
            
                string message = "true";

                byte[] msg = Encoding.UTF8.GetBytes(message);

                // Отправляем данные через сокет
                int bytesSent = sender1.Send(msg);

                // Получаем ответ от сервера
                int bytesRec = sender1.Receive(bytes);

                string result = Encoding.UTF8.GetString(bytes, 0, bytesRec);

            float[] vrtx = Array.ConvertAll(result.Split(','), float.Parse);
            triangulate(vrtx);
            

            // Освобождаем сокет
            sender1.Shutdown(SocketShutdown.Both);
            sender1.Close();



        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)//обрабатываем на ввод только цифры, бекспейс и запятую
        {
                if (e.KeyChar !=44 && e.KeyChar !=8 && (e.KeyChar < 48 || e.KeyChar > 57))
                    e.Handled = true;
        }

        private void triangulate(float[] arr)
        { 
            polygonPen = new Pen(Color.Black, 1);
            trianBrush = new SolidBrush(Color.Red);

            float[] vertex = arr;
            //х и y координаты многоугольник против часовой стрелки

            ToDB(arr);

            //вершины многоугольника против часовой стрелки
            polygon = new Polygon(vertex);
            if (polygon.getTriangles() != null) //если триангуляция была проведена успешно
            {
                Triangle[] trians = polygon.getTriangles();
                colors = new Color[trians.Length];

                Random rand = new Random();
                for (int i = 0; i < colors.Length; i++) //заполняем палитру случайными цветами
                    colors[i] = Color.FromArgb(rand.Next(25, 225), rand.Next(25, 225), rand.Next(25, 225));
            }

            //разместим многоугольник на весь экран
            //масштабирование
            PointF[] points = polygon.getPoints();

            float minX = int.MaxValue, minY = int.MaxValue;
            float maxX = int.MinValue, maxY = int.MinValue;
            foreach (PointF p in points)
            {
                minX = Math.Min(minX, p.X);
                minY = Math.Min(minY, p.Y);
                maxX = Math.Max(maxX, p.X);
                maxY = Math.Max(maxY, p.Y);
            }

            float width = maxX - minX;
            float height = maxY - minY;

            float scaleX = pictureBox1.Width / width;
            float scaleY = pictureBox1.Height / height;
            scale = Math.Min(scaleX, scaleY) * 0.95f; //коэффициент масштабирования

            //центрирование многоугольника
            translateX = (pictureBox1.Width - width * scale) / 2 - minX * scale;
            translateY = (pictureBox1.Height - height * scale) / 2 - minY * scale;

            draw();

            if (colors != null) //если триангуляция прошла успешно
            {
                drawCount = polygon.getTriangles().Length; //увеличиваем количество отображаемых треугольников
                draw();
            }

        }

        private void ToDB(float[] coords)
        {
            string strCoors = string.Join(",", coords);
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = @"(LocalDB)\MSSQLLocalDB";
            builder.AttachDBFilename = @"C:\Users\Никита\Desktop\Project\triangulation\triangulation\Database.mdf";
            builder.IntegratedSecurity = true;

            using (IDbConnection connection = new SqlConnection(builder.ConnectionString))
            using (DataClasses1DataContext db = new DataClasses1DataContext(connection))
            {
                PolygonCoords row = new PolygonCoords()
                {
                    Coordinates = strCoors
                };

                db.PolygonCoords.InsertOnSubmit(row);
                db.SubmitChanges();

            }

            
        }

    }
}
