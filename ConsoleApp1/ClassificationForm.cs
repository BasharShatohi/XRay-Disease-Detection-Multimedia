using System;
using System.Drawing;
using System.Windows.Forms;

namespace ImageApp
{
    public class ClassificationForm : Form
    {
        private PictureBox pictureBox;
        private Button classifyButton;
        private Button exitButton;
        private Label resultLabel;
        private Bitmap loadedImage;
        private Rectangle selectedRegion;
        private bool isSelecting;

        public ClassificationForm(string imagePath)
        {
            InitializeComponents();
            LoadImage(imagePath);
        }

        private void InitializeComponents()
        {
            this.Text = "Classify Image";
            this.Size = new Size(800, 800);

            pictureBox = new PictureBox();
            pictureBox.Size = new Size(700, 500);
            pictureBox.Location = new Point(50, 50);
            pictureBox.BorderStyle = BorderStyle.Fixed3D;
            pictureBox.MouseDown += PictureBox_MouseDown;
            pictureBox.MouseMove += PictureBox_MouseMove;
            pictureBox.MouseUp += PictureBox_MouseUp;
            pictureBox.Paint += PictureBox_Paint;
            this.Controls.Add(pictureBox);

            classifyButton = new Button();
            classifyButton.Text = "Classify";
            classifyButton.Size = new Size(100, 30);
            classifyButton.Location = new Point(50, 10);
            classifyButton.Click += ClassifyButton_Click;
            this.Controls.Add(classifyButton);

            exitButton = new Button();
            exitButton.Text = "Exit";
            exitButton.Size = new Size(100, 30);
            exitButton.Location = new Point(160, 10);
            exitButton.Click += ExitButton_Click;
            this.Controls.Add(exitButton);

            resultLabel = new Label();
            resultLabel.Text = "Result:";
            resultLabel.AutoSize = true;
            resultLabel.Location = new Point(50, 560);
            this.Controls.Add(resultLabel);
        }

        private void LoadImage(string imagePath)
        {
            loadedImage = new Bitmap(imagePath);
            pictureBox.Image = loadedImage;
        }

        private void PictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            selectedRegion = new Rectangle(e.X, e.Y, 0, 0);
            isSelecting = true;
        }

        private void PictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (isSelecting)
            {
                int width = e.X - selectedRegion.X;
                int height = e.Y - selectedRegion.Y;
                selectedRegion.Width = width;
                selectedRegion.Height = height;
                pictureBox.Invalidate();
            }
        }

        private void PictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            isSelecting = false;
            if (selectedRegion.Width < 0)
            {
                selectedRegion.X += selectedRegion.Width;
                selectedRegion.Width = Math.Abs(selectedRegion.Width);
            }
            if (selectedRegion.Height < 0)
            {
                selectedRegion.Y += selectedRegion.Height;
                selectedRegion.Height = Math.Abs(selectedRegion.Height);
            }
        }

        private void PictureBox_Paint(object sender, PaintEventArgs e)
        {
            if (selectedRegion != Rectangle.Empty && selectedRegion.Width > 0 && selectedRegion.Height > 0)
            {
                e.Graphics.DrawRectangle(Pens.Red, selectedRegion);
            }
        }

        private void ClassifyButton_Click(object sender, EventArgs e)
        {
            if (selectedRegion == Rectangle.Empty || selectedRegion.Width <= 0 || selectedRegion.Height <= 0)
            {
                MessageBox.Show("Please select a region to classify.");
                return;
            }

            Bitmap regionBitmap = new Bitmap(selectedRegion.Width, selectedRegion.Height);
            using (Graphics g = Graphics.FromImage(regionBitmap))
            {
                g.DrawImage(loadedImage, new Rectangle(0, 0, regionBitmap.Width, regionBitmap.Height), selectedRegion, GraphicsUnit.Pixel);
            }

            string severity = ClassifyRegion(regionBitmap);
            resultLabel.Text = $"Result: {severity}";
        }

        private string ClassifyRegion(Bitmap regionBitmap)
        {
            int totalIntensity = 0;
            int pixelCount = regionBitmap.Width * regionBitmap.Height;

            for (int y = 0; y < regionBitmap.Height; y++)
            {
                for (int x = 0; x < regionBitmap.Width; x++)
                {
                    Color pixelColor = regionBitmap.GetPixel(x, y);
                    int intensity = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;
                    totalIntensity += intensity;
                }
            }

            if (pixelCount == 0) // Just in case
            {
                return "Error";
            }

            int averageIntensity = totalIntensity / pixelCount;

            if (averageIntensity < 85)
            {
                return "Mild";
            }
            else if (averageIntensity < 140)
            {
                return "Moderate";
            }
            else
            {
                return "Severe";
            }
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

