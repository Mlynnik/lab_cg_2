using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace lab_cg_3
{
    public partial class Task3 : Form
    {
        private PictureBox pictureBox1;
        private Button button1;
        private Button button2;
        private Button button3;
        private Button button4;
        private Button button5;

        private List<Point> currentPoints = new List<Point>();

        private class Triangle
        {
            public Point P1, P2, P3;
            public Color C1, C2, C3;
            public Triangle(Point p1, Point p2, Point p3, Color c1, Color c2, Color c3)
            {
                P1 = p1; P2 = p2; P3 = p3; C1 = c1; C2 = c2; C3 = c3;
            }
        }

        private List<Triangle> triangles = new List<Triangle>();

        private Color color1 = Color.Red;
        private Color color2 = Color.Green;
        private Color color3 = Color.Blue;

        private Bitmap Canvas;
        private Graphics g;
        public Task3()
        {
            InitializeComponent();

            this.pictureBox1 = new PictureBox();
            this.button1 = new Button();
            this.button2 = new Button();
            this.button3 = new Button();
            this.button4 = new Button();
            this.button5 = new Button();

            this.pictureBox1.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom)
                | AnchorStyles.Left)
                | AnchorStyles.Right)));
            this.pictureBox1.BackColor = SystemColors.Window;
            this.pictureBox1.BorderStyle = BorderStyle.FixedSingle;
            this.pictureBox1.Location = new Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new Size(560, 476);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;

            this.button1.Anchor = ((AnchorStyles)((AnchorStyles.Top | AnchorStyles.Right)));
            this.button1.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(204)));
            this.button1.Location = new Point(580, 100);
            this.button1.Name = "button1";
            this.button1.Size = new Size(200, 40);
            this.button1.TabIndex = 19;
            this.button1.Text = "Залить";
            this.button1.UseVisualStyleBackColor = true;

            this.button2.Anchor = ((AnchorStyles)((AnchorStyles.Top | AnchorStyles.Right)));
            this.button2.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(204)));
            this.button2.Location = new Point(580, 150);
            this.button2.Name = "button2";
            this.button2.Size = new Size(200, 40);
            this.button2.TabIndex = 20;
            this.button2.Text = "Стереть";
            this.button2.UseVisualStyleBackColor = true;

            this.button3.Anchor = ((AnchorStyles)((AnchorStyles.Top | AnchorStyles.Right)));
            this.button3.Location = new Point(580, 20);
            this.button3.Name = "button3";
            this.button3.Size = new Size(50, 50);
            this.button3.TabIndex = 21;
            this.button3.UseVisualStyleBackColor = false;
            this.button3.FlatStyle = FlatStyle.Flat;
            this.button3.FlatAppearance.BorderSize = 0;

            this.button4.Anchor = ((AnchorStyles)((AnchorStyles.Top | AnchorStyles.Right)));
            this.button4.Location = new Point(640, 20);
            this.button4.Name = "button4";
            this.button4.Size = new Size(50, 50);
            this.button4.TabIndex = 22;
            this.button4.UseVisualStyleBackColor = false;
            this.button4.FlatStyle = FlatStyle.Flat;
            this.button4.FlatAppearance.BorderSize = 0;

            this.button5.Anchor = ((AnchorStyles)((AnchorStyles.Top | AnchorStyles.Right)));
            this.button5.Location = new Point(700, 20);
            this.button5.Name = "button5";
            this.button5.Size = new Size(50, 50);
            this.button5.TabIndex = 23;
            this.button5.UseVisualStyleBackColor = false;
            this.button5.FlatStyle = FlatStyle.Flat;
            this.button5.FlatAppearance.BorderSize = 0;

            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button5);

            pictureBox1.SizeMode = PictureBoxSizeMode.Normal;
            pictureBox1.BackColor = Color.White;

            CreateNewCanvas(pictureBox1.Width, pictureBox1.Height);

            button3.BackColor = color1;
            button4.BackColor = color2;
            button5.BackColor = color3;

            pictureBox1.MouseClick += PictureBox1_MouseClick;
            this.ResizeEnd += Task3_ResizeEnd;

            button1.Click += ButtonFill_Click;
            button2.Click += ButtonClearAll_Click;
            button3.Click += ButtonColor1_Click;
            button4.Click += ButtonColor2_Click;
            button5.Click += ButtonColor3_Click;
        }

        private void CreateNewCanvas(int width, int height)
        {
            if (width <= 0 || height <= 0) return;

            if (pictureBox1 != null)
                pictureBox1.Image = null;

            g?.Dispose();
            Canvas?.Dispose();

            var newCanvas = new Bitmap(width, height);
            using (Graphics gg = Graphics.FromImage(newCanvas))
                gg.Clear(Color.White);

            Canvas = newCanvas;
            g = Graphics.FromImage(Canvas);
            pictureBox1.Image = Canvas;
        }

        private void Task3_ResizeEnd(object sender, EventArgs e)
        {
            if (pictureBox1.Width <= 0 || pictureBox1.Height <= 0)
                return;

            CreateNewCanvas(pictureBox1.Width, pictureBox1.Height);
            RedrawAll();
        }

        private void PictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (currentPoints.Count > 0)
                    currentPoints.RemoveAt(currentPoints.Count - 1);
                RedrawAll();
                return;
            }

            if (e.Button != MouseButtons.Left)
                return;

            if (e.X < 0 || e.Y < 0 || e.X >= Canvas.Width || e.Y >= Canvas.Height)
                return;

            if (currentPoints.Count >= 3)
            {
                MessageBox.Show("Уже выбраны три точки треугольника. Нажмите «Залить» или ПКМ, чтобы удалить последнюю.",
                    "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Point p = new Point(e.X, e.Y);
            currentPoints.Add(p);

            using (SolidBrush br = new SolidBrush(Color.Black))
                g.FillEllipse(br, p.X - 3, p.Y - 3, 6, 6);

            pictureBox1.Invalidate();
        }

        private void ButtonColor1_Click(object sender, EventArgs e)
        {
            using (ColorDialog cd = new ColorDialog())
                if (cd.ShowDialog() == DialogResult.OK)
                {
                    color1 = cd.Color;
                    button3.BackColor = cd.Color;
                }
        }

        private void ButtonColor2_Click(object sender, EventArgs e)
        {
            using (ColorDialog cd = new ColorDialog())
                if (cd.ShowDialog() == DialogResult.OK)
                {
                    color2 = cd.Color;
                    button4.BackColor = cd.Color;
                }
        }

        private void ButtonColor3_Click(object sender, EventArgs e)
        {
            using (ColorDialog cd = new ColorDialog())
                if (cd.ShowDialog() == DialogResult.OK)
                {
                    color3 = cd.Color;
                    button5.BackColor = cd.Color;
                }
        }

        private void ButtonClearAll_Click(object sender, EventArgs e)
        {
            if (Canvas == null)
                return;
            using (Graphics gg = Graphics.FromImage(Canvas))
                gg.Clear(Color.White);
            triangles.Clear();
            currentPoints.Clear();
            pictureBox1.Invalidate();
        }

        private void ButtonFill_Click(object sender, EventArgs e)
        {
            if (currentPoints.Count < 3)
            {
                MessageBox.Show("Нужно поставить три точки на холсте, чтобы образовать треугольник.",
                    "Недостаточно вершин", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var t = new Triangle(currentPoints[0], currentPoints[1], currentPoints[2], color1, color2, color3);
            triangles.Add(t);

            FillTriangleGradient(t.P1, t.P2, t.P3, t.C1, t.C2, t.C3);

            currentPoints.Clear();
            pictureBox1.Invalidate();
        }

        private void RedrawAll()
        {
            if (Canvas == null)
                return;

            using (Graphics gg = Graphics.FromImage(Canvas))
                gg.Clear(Color.White);

            foreach (var tri in triangles)
                FillTriangleGradient(tri.P1, tri.P2, tri.P3, tri.C1, tri.C2, tri.C3);

            foreach (var p in currentPoints)
                using (SolidBrush br = new SolidBrush(Color.Black))
                    g.FillEllipse(br, p.X - 3, p.Y - 3, 6, 6);

            pictureBox1.Invalidate();
        }

        private void FillTriangleGradient(Point p1, Point p2, Point p3, Color c1, Color c2, Color c3)
        {
            Rectangle bounds = GetTriangleBounds(p1, p2, p3);

            int left = Math.Max(0, bounds.Left);
            int right = Math.Min(Canvas.Width - 1, bounds.Right);
            int top = Math.Max(0, bounds.Top);
            int bottom = Math.Min(Canvas.Height - 1, bounds.Bottom);

            float ABCarea = TriangleArea(p1, p2, p3);

            if (Math.Abs(ABCarea) < 1e-6f)
                return;

            using (var fb = new FastBitmap.FastBitmap(Canvas))
            {
                for (int y = top; y <= bottom; y++)
                {
                    for (int x = left; x <= right; x++)
                    {
                        Point p = new Point(x, y);

                        if (!IsPointInTriangle(p, p1, p2, p3))
                            continue;

                        float a = TriangleArea(p, p2, p3) / ABCarea;
                        float b = TriangleArea(p, p3, p1) / ABCarea;
                        float c = TriangleArea(p, p1, p2) / ABCarea;

                        int R = (int)(a * c1.R + b * c2.R + c * c3.R);
                        int G = (int)(a * c1.G + b * c2.G + c * c3.G);
                        int B = (int)(a * c1.B + b * c2.B + c * c3.B);

                        fb[x, y] = Color.FromArgb(R, G, B);
                    }
                }
            }
        }

        private float TriangleArea(Point a, Point b, Point c)
        {
            return Math.Abs((b.X - a.X) * (c.Y - a.Y) - (c.X - a.X) * (b.Y - a.Y)) / 2f;
        }

        private Rectangle GetTriangleBounds(Point p1, Point p2, Point p3)
        {
            int minX = Math.Min(p1.X, Math.Min(p2.X, p3.X));
            int maxX = Math.Max(p1.X, Math.Max(p2.X, p3.X));
            int minY = Math.Min(p1.Y, Math.Min(p2.Y, p3.Y));
            int maxY = Math.Max(p1.Y, Math.Max(p2.Y, p3.Y));
            return Rectangle.FromLTRB(minX, minY, maxX, maxY);
        }

        private bool IsPointInTriangle(Point p, Point p0, Point p1, Point p2)
        {
            float dX = p.X - p2.X;
            float dY = p.Y - p2.Y;

            float dX21 = p2.X - p1.X;
            float dY12 = p1.Y - p2.Y;

            float D = (p1.X - p0.X) * (p2.Y - p0.Y) - (p1.Y - p0.Y) * (p2.X - p0.X);
            float s = dY12 * dX + dX21 * dY;
            float t = (p2.Y - p0.Y) * dX + (p0.X - p2.X) * dY;

            if (D < 0)
                return (s <= 0) && (t <= 0) && (s + t >= D);

            return (s >= 0) && (t >= 0) && (s + t <= D);
        }
    }
}
