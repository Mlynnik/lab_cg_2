using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace lab_cg_2
{
    public partial class Task3 : Form
    {
        private PictureBox pictureBox;
        private TrackBar sliderHue;
        private TrackBar sliderSaturation;
        private TrackBar sliderValue;
        private Label labelHueValue;
        private Label labelSatValue;
        private Label labelValValue;
        private Button buttonLoad;
        private Button buttonProcess;
        private Button buttonSave;
        private Bitmap? originalImage;
        private Bitmap? processedImage;

        public Task3()
        {
            InitializeComponent();

            Text = "Преобразование RGB изображения в HSV";
            Size = new Size(1200, 800);
            StartPosition = FormStartPosition.CenterScreen;

            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1
            };
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 80));
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));

            pictureBox = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom
            };
            mainPanel.Controls.Add(pictureBox, 0, 0);

            var controlsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                AutoScroll = true
            };

            buttonLoad = new Button
            {
                Text = "Загрузить изображение",
                Width = 200
            };
            buttonLoad.Click += ButtonLoad_Click;
            controlsPanel.Controls.Add(buttonLoad);

            labelHueValue = new Label { Text = "Hue: 0", Width = 200 };
            controlsPanel.Controls.Add(labelHueValue);

            sliderHue = new TrackBar
            {
                Minimum = 0,
                Maximum = 359,
                TickFrequency = 60,
                Width = 200
            };
            sliderHue.ValueChanged += (s, e) =>
                labelHueValue.Text = "Hue: " + sliderHue.Value;
            controlsPanel.Controls.Add(sliderHue);

            labelSatValue = new Label { Text = "Saturation: 0", Width = 200 };
            controlsPanel.Controls.Add(labelSatValue);

            sliderSaturation = new TrackBar
            {
                Minimum = -100,
                Maximum = 100,
                TickFrequency = 20,
                Width = 200
            };
            sliderSaturation.ValueChanged += (s, e) =>
                labelSatValue.Text = "Saturation: " + sliderSaturation.Value;
            controlsPanel.Controls.Add(sliderSaturation);

            labelValValue = new Label { Text = "Value: 0", Width = 200 };
            controlsPanel.Controls.Add(labelValValue);

            sliderValue = new TrackBar
            {
                Minimum = -100,
                Maximum = 100,
                TickFrequency = 20,
                Width = 200
            };
            sliderValue.ValueChanged += (s, e) =>
                labelValValue.Text = "Value: " + sliderValue.Value;
            controlsPanel.Controls.Add(sliderValue);

            buttonProcess = new Button
            {
                Text = "Обработать",
                Width = 200
            };
            buttonProcess.Click += ButtonProcess_Click;
            controlsPanel.Controls.Add(buttonProcess);

            buttonSave = new Button
            {
                Text = "Сохранить результат",
                Width = 200
            };
            buttonSave.Click += ButtonSave_Click;
            controlsPanel.Controls.Add(buttonSave);

            mainPanel.Controls.Add(controlsPanel, 1, 0);
            this.Controls.Add(mainPanel);
        }

        private void ButtonLoad_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Images|*.jpg;*.jpeg;*.png;*.bmp";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    if (originalImage != null) originalImage.Dispose();
                    originalImage = new Bitmap(ofd.FileName);
                    processedImage = new Bitmap(originalImage);
                    pictureBox.Image = processedImage;

                    sliderHue.Value = 0;
                    sliderSaturation.Value = 0;
                    sliderValue.Value = 0;
                }
            }
        }

        private void ButtonProcess_Click(object sender, EventArgs e)
        {
            if (originalImage == null)
            {
                MessageBox.Show("Сначала загрузите изображение!");
                return;
            }

            int hShift = sliderHue.Value;
            double sShift = sliderSaturation.Value / 100.0;
            double vShift = sliderValue.Value / 100.0;

            int width = originalImage.Width;
            int height = originalImage.Height;

            Bitmap source = originalImage;
            bool needDisposeSource = false;

            if (originalImage.PixelFormat != PixelFormat.Format32bppArgb)
            {
                source = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                using (Graphics g = Graphics.FromImage(source))
                {
                    g.DrawImage(originalImage, 0, 0, width, height);
                }
                needDisposeSource = true;
            }

            Bitmap result = new Bitmap(width, height, PixelFormat.Format32bppArgb);

            Rectangle rect = new Rectangle(0, 0, width, height);
            BitmapData srcData = null;
            BitmapData dstData = null;

            srcData = source.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            dstData = result.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            int stride = Math.Abs(srcData.Stride);
            int bytes = stride * height;

            byte[] srcBuffer = new byte[bytes];
            byte[] dstBuffer = new byte[bytes];

            Marshal.Copy(srcData.Scan0, srcBuffer, 0, bytes);

            for (int y = 0; y < height; y++)
            {
                int rowOffset = y * stride;

                for (int x = 0; x < width; x++)
                {
                    int i = rowOffset + x * 4;

                    // В 32bppArgb порядок в памяти: B, G, R, A
                    byte b = srcBuffer[i];
                    byte g = srcBuffer[i + 1];
                    byte r = srcBuffer[i + 2];
                    byte a = srcBuffer[i + 3];

                    double h, s, v;
                    RgbToHsv(r, g, b, out h, out s, out v);

                    h = (h + hShift + 360) % 360;
                    s = Math.Min(1, Math.Max(0, s + sShift));
                    v = Math.Min(1, Math.Max(0, v + vShift));

                    Color newColor = HsvToRgb(h, s, v);

                    dstBuffer[i] = newColor.B;
                    dstBuffer[i + 1] = newColor.G;
                    dstBuffer[i + 2] = newColor.R;
                    dstBuffer[i + 3] = a;
                }
            }

            Marshal.Copy(dstBuffer, 0, dstData.Scan0, bytes);
            if (srcData != null) source.UnlockBits(srcData);
            if (dstData != null) result.UnlockBits(dstData);

            if (needDisposeSource)
                source.Dispose();

            if (processedImage != null)
                processedImage.Dispose();

            processedImage = result;
            pictureBox.Image = processedImage;
        }

        private void ButtonSave_Click(object sender, EventArgs e)
        {
            if (processedImage == null)
            {
                MessageBox.Show("Нет изображения для сохранения!");
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "JPEG Image|*.jpg|PNG Image|*.png|Bitmap|*.bmp";
                sfd.FileName = "processed_image.jpg";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    processedImage.Save(sfd.FileName);
                    MessageBox.Show("Изображение сохранено: " + sfd.FileName);
                }
            }
        }


        private void RgbToHsv(byte rByte, byte gByte, byte bByte, out double h, out double s, out double v)
        {
            double r = rByte / 255.0;
            double g = gByte / 255.0;
            double b = bByte / 255.0;

            double max = Math.Max(r, Math.Max(g, b));
            double min = Math.Min(r, Math.Min(g, b));
            v = max;

            double delta = max - min;

            if (max == 0)
                s = 0;
            else
                s = 1 - (min / max);

            if (delta == 0)
                h = 0;
            else if (max == r)
            {
                if (g >= b)
                    h = 60 * ((g - b) / delta);
                else
                    h = 60 * ((g - b) / delta) + 360;
            }
            else if (max == g)
                h = 60 * ((b - r) / delta) + 120;
            else
                h = 60 * ((r - g) / delta) + 240;
        }


        private Color HsvToRgb(double h, double s, double v)
        {

            int hi = (int)Math.Floor(h / 60.0) % 6;
            double f = (h / 60.0) - Math.Floor(h / 60.0);

            double p = v * (1 - s);
            double q = v * (1 - f * s);
            double t = v * (1 - (1 - f) * s);

            double r = 0, g = 0, b = 0;

            switch (hi)
            {
                case 0: r = v; g = t; b = p; break;
                case 1: r = q; g = v; b = p; break;
                case 2: r = p; g = v; b = t; break;
                case 3: r = p; g = q; b = v; break;
                case 4: r = t; g = p; b = v; break;
                case 5: r = v; g = p; b = q; break;
            }

            return Color.FromArgb((int)Math.Round(r * 255), (int)Math.Round(g * 255), (int)Math.Round(b * 255));
        }
    }
}
