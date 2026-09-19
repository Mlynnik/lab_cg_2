using System;
using System.Drawing;
using System.Windows.Forms;

namespace lab_cg_2
{
    public partial class Task2 : Form
    {
        private readonly PictureBox pictureOriginal;
        private readonly PictureBox pictureRed;
        private readonly PictureBox pictureGreen;
        private readonly PictureBox pictureBlue;

        private readonly PictureBox histRed;
        private readonly PictureBox histGreen;
        private readonly PictureBox histBlue;

        private readonly Button buttonLoad;

        public Task2()
        {
            InitializeComponent();

            Text = "RGB каналы изображения";
            Size = new Size(1200, 800);
            StartPosition = FormStartPosition.CenterScreen;

            buttonLoad = new Button
            {
                Text = "Загрузить изображение",
                Dock = DockStyle.Top,
                Height = 40
            };

            buttonLoad.Click += ButtonLoad_Click;

            pictureOriginal = CreatePicture();
            pictureRed = CreatePicture();
            pictureGreen = CreatePicture();
            pictureBlue = CreatePicture();

            histRed = CreatePicture();
            histGreen = CreatePicture();
            histBlue = CreatePicture();

            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 3,
                ColumnCount = 1
            };

            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 34));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33));

            var imagesPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1
            };

            var histPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1
            };

            for (int i = 0; i < 3; i++)
            {
                imagesPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / 3));
                histPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / 3));
            }

            imagesPanel.Controls.Add(pictureRed, 0, 0);
            imagesPanel.Controls.Add(pictureGreen, 1, 0);
            imagesPanel.Controls.Add(pictureBlue, 2, 0);

            histPanel.Controls.Add(histRed, 0, 0);
            histPanel.Controls.Add(histGreen, 1, 0);
            histPanel.Controls.Add(histBlue, 2, 0);

            mainPanel.Controls.Add(pictureOriginal, 0, 0);
            mainPanel.Controls.Add(imagesPanel, 0, 1);
            mainPanel.Controls.Add(histPanel, 0, 2);

            Controls.Add(mainPanel);
            Controls.Add(buttonLoad);
        }

        private PictureBox CreatePicture()
        {
            return new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.White
            };
        }

        private void ButtonLoad_Click(object sender, EventArgs e)
        {
            using OpenFileDialog dialog = new OpenFileDialog();

            dialog.Filter = "Изображения|*.jpg;*.jpeg;*.png;*.bmp";

            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            using Bitmap original = new Bitmap(dialog.FileName);

            ProcessImage(original);
        }

        private void ProcessImage(Bitmap original)
        {
            int width = original.Width;
            int height = original.Height;

            Bitmap redImage = new Bitmap(width, height);
            Bitmap greenImage = new Bitmap(width, height);
            Bitmap blueImage = new Bitmap(width, height);

            int[] redHistogram = new int[256];
            int[] greenHistogram = new int[256];
            int[] blueHistogram = new int[256];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color color = original.GetPixel(x, y);

                    redImage.SetPixel(x, y, Color.FromArgb(color.R, 0, 0));
                    greenImage.SetPixel(x, y, Color.FromArgb(0, color.G, 0));
                    blueImage.SetPixel(x, y, Color.FromArgb(0, 0, color.B));

                    redHistogram[color.R]++;
                    greenHistogram[color.G]++;
                    blueHistogram[color.B]++;
                }
            }

            SetPicture(pictureOriginal, new Bitmap(original));

            SetPicture(pictureRed, redImage);
            SetPicture(pictureGreen, greenImage);
            SetPicture(pictureBlue, blueImage);

            SetPicture(histRed, CreateHistogram(redHistogram, Color.Red));
            SetPicture(histGreen, CreateHistogram(greenHistogram, Color.Green));
            SetPicture(histBlue, CreateHistogram(blueHistogram, Color.Blue));
        }

        private void SetPicture(PictureBox picture, Bitmap image)
        {
            Image previous = picture.Image;

            picture.Image = image;

            previous?.Dispose();
        }

        private Bitmap CreateHistogram(int[] histogram, Color color)
        {
            int width = 512;
            int height = 220;

            int left = 15;
            int right = 15;
            int top = 15;
            int bottom = 25;

            int maxValue = 0;

            for (int i = 0; i < histogram.Length; i++)
            {
                if (histogram[i] > maxValue)
                    maxValue = histogram[i];
            }

            Bitmap result = new Bitmap(width, height);

            using Graphics g = Graphics.FromImage(result);
            using SolidBrush brush = new SolidBrush(color);
            using Pen axisPen = new Pen(Color.Black);

            g.Clear(Color.White);

            int graphWidth = width - left - right;
            int graphHeight = height - top - bottom;
            int baseY = height - bottom;

            g.DrawLine(axisPen, left, baseY, width - right, baseY);

            for (int i = 0; i < histogram.Length; i++)
            {
                int x1 = left + i * graphWidth / 256;
                int x2 = left + (i + 1) * graphWidth / 256;

                int barWidth = Math.Max(1, x2 - x1);

                int barHeight = 0;

                if (maxValue != 0)
                    barHeight = (int)((double)histogram[i] / maxValue * graphHeight);

                if (barHeight > 0)
                    g.FillRectangle(brush, x1, baseY - barHeight, barWidth, barHeight);
            }

            using Font font = new Font("Arial", 9);

            g.DrawString("0", font, Brushes.Black, left, baseY + 3);
            g.DrawString("128", font, Brushes.Black, left + graphWidth / 2 - 10, baseY + 3);
            g.DrawString("255", font, Brushes.Black, width - right - 25, baseY + 3);

            return result;
        }
    }
}