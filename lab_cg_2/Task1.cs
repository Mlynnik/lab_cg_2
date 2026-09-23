using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace lab_cg_2
{
    public partial class Task1 : Form
    {
        private Button buttonLoad;
        private PictureBox pictureOriginal;
        private PictureBox pictureGray1;
        private PictureBox pictureGray2;
        private PictureBox pictureDiff;
        private PictureBox pictureHist1;
        private PictureBox pictureHist2;

        public Task1()
        {
            InitializeComponent();

            this.Text = "RGB в оттенки серого";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;

            buttonLoad = new Button();
            buttonLoad.Text = "Загрузить изображение";
            buttonLoad.Dock = DockStyle.Top;
            buttonLoad.Height = 40;
            buttonLoad.Click += ButtonLoad_Click;

            pictureOriginal = MakePicture();
            pictureGray1 = MakePicture();
            pictureGray2 = MakePicture();
            pictureDiff = MakePicture();
            pictureHist1 = MakePicture();
            pictureHist2 = MakePicture();

            var mainPanel = new TableLayoutPanel();
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.RowCount = 3;
            mainPanel.ColumnCount = 1;
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 34));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33));

            var imagesPanel = new TableLayoutPanel();
            imagesPanel.Dock = DockStyle.Fill;
            imagesPanel.RowCount = 1;
            imagesPanel.ColumnCount = 3;
            for (int i = 0; i < 3; i++)
                imagesPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));

            var histPanel = new TableLayoutPanel();
            histPanel.Dock = DockStyle.Fill;
            histPanel.RowCount = 1;
            histPanel.ColumnCount = 2;
            histPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            histPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

            imagesPanel.Controls.Add(WithCaption(pictureGray1, "0.299R + 0.587G + 0.114B"), 0, 0);
            imagesPanel.Controls.Add(WithCaption(pictureGray2, "0.2126R + 0.7152G + 0.0722B"), 1, 0);
            imagesPanel.Controls.Add(WithCaption(pictureDiff, "разница"), 2, 0);

            histPanel.Controls.Add(WithCaption(pictureHist1, "гистограмма 1"), 0, 0);
            histPanel.Controls.Add(WithCaption(pictureHist2, "гистограмма 2"), 1, 0);

            mainPanel.Controls.Add(WithCaption(pictureOriginal, "исходное"), 0, 0);
            mainPanel.Controls.Add(imagesPanel, 0, 1);
            mainPanel.Controls.Add(histPanel, 0, 2);

            this.Controls.Add(mainPanel);
            this.Controls.Add(buttonLoad);
        }

        private PictureBox MakePicture()
        {
            PictureBox pb = new PictureBox();
            pb.Dock = DockStyle.Fill;
            pb.SizeMode = PictureBoxSizeMode.Zoom;
            pb.BackColor = Color.White;
            return pb;
        }

        private Control WithCaption(PictureBox pb, string text)
        {
            Panel panel = new Panel();
            panel.Dock = DockStyle.Fill;

            Label label = new Label();
            label.Text = text;
            label.Dock = DockStyle.Top;
            label.Height = 22;
            label.TextAlign = ContentAlignment.MiddleCenter;

            pb.Dock = DockStyle.Fill;
            panel.Controls.Add(pb);
            panel.Controls.Add(label);
            return panel;
        }

        private void ButtonLoad_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Изображения|*.jpg;*.jpeg;*.png;*.bmp";
                if (ofd.ShowDialog() == DialogResult.OK)
                    ProcessImage(ofd.FileName);
            }
        }

        private void ProcessImage(string path)
        {
            Bitmap original = new Bitmap(path);
            pictureOriginal.Image = original;

            Bitmap gray1 = Rgb2Gray1(original);
            Bitmap gray2 = Rgb2Gray2(original);
            Bitmap diff = Difference(gray1, gray2);

            pictureGray1.Image = gray1;
            pictureGray2.Image = gray2;
            pictureDiff.Image = diff;

            pictureHist1.Image = BuildHistogram(gray1);
            pictureHist2.Image = BuildHistogram(gray2);
        }

        // ntsc / pal
        private Bitmap Rgb2Gray1(Bitmap img)
        {
            Bitmap gray = new Bitmap(img.Width, img.Height);
            for (int y = 0; y < img.Height; y++)
            {
                for (int x = 0; x < img.Width; x++)
                {
                    Color c = img.GetPixel(x, y);
                    int val = (int)(0.299 * c.R + 0.587 * c.G + 0.114 * c.B);
                    gray.SetPixel(x, y, Color.FromArgb(val, val, val));
                }
            }
            return gray;
        }

        // hdtv
        private Bitmap Rgb2Gray2(Bitmap img)
        {
            Bitmap gray = new Bitmap(img.Width, img.Height);
            for (int y = 0; y < img.Height; y++)
            {
                for (int x = 0; x < img.Width; x++)
                {
                    Color c = img.GetPixel(x, y);
                    int val = (int)(0.2126 * c.R + 0.7152 * c.G + 0.0722 * c.B);
                    gray.SetPixel(x, y, Color.FromArgb(val, val, val));
                }
            }
            return gray;
        }

        private Bitmap Difference(Bitmap img1, Bitmap img2)
        {
            Bitmap diff = new Bitmap(img1.Width, img1.Height);
            for (int y = 0; y < img1.Height; y++)
            {
                for (int x = 0; x < img1.Width; x++)
                {
                    int v1 = img1.GetPixel(x, y).R;
                    int v2 = img2.GetPixel(x, y).R;
                    int d = Math.Abs(v1 - v2);
                    d = d * 8;
                    if (d > 255)
                        d = 255;
                    diff.SetPixel(x, y, Color.FromArgb(d, d, d));
                }
            }
            return diff;
        }

        private Bitmap BuildHistogram(Bitmap grayImg)
        {
            int[] histogram = new int[256];
            for (int y = 0; y < grayImg.Height; y++)
                for (int x = 0; x < grayImg.Width; x++)
                    histogram[grayImg.GetPixel(x, y).R]++;

            int maxVal = 0;
            for (int i = 0; i < histogram.Length; i++)
                if (histogram[i] > maxVal)
                    maxVal = histogram[i];

            int width = 400;
            int height = 200;
            Bitmap histImg = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(histImg);
            g.Clear(Color.White);

            for (int i = 0; i < 256; i++)
            {
                int barHeight = (int)((double)histogram[i] / maxVal * (height - 30));
                g.DrawLine(Pens.Black, i + 10, height - 20, i + 10, height - 20 - barHeight);
            }

            g.DrawLine(Pens.Black, 10, height - 20, 270, height - 20);
            g.DrawString("0", new Font("Arial", 8), Brushes.Black, 6, height - 18);
            g.DrawString("255", new Font("Arial", 8), Brushes.Black, 250, height - 18);

            g.Dispose();
            return histImg;
        }
    }
}
