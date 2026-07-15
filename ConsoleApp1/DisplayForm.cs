using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace ImageApp
{
    public class DisplayForm : Form
    {
        private PictureBox pictureBox;
        private Bitmap originalImage;
        private Rectangle selectedRegion;
        private Point startPoint;
        private bool isSelecting;

        private NumericUpDown numericR;
        private NumericUpDown numericG;
        private NumericUpDown numericB;
        private TrackBar transparencyBar;
        private Button applyColorButton;
        private Button xrayButton;
        private string originalImagePath; // Variable to store the original image path

        public DisplayForm(string imagePath)
        {
            originalImagePath = imagePath; // Initialize the variable
            InitializeComponents(imagePath);
        }

        private void InitializeComponents(string imagePath)
        {
            // Set form properties
            this.Text = "Displayed Image";
            this.Size = new Size(800, 600);

            // Add save button
            Button saveButton = new Button();
            saveButton.Text = "Save";
            saveButton.Size = new Size(100, 30);
            saveButton.Location = new Point(10, 10);
            saveButton.Click += SaveButton_Click;
            this.Controls.Add(saveButton);

            // Add exit button
            Button exitButton = new Button();
            exitButton.Text = "Exit";
            exitButton.Size = new Size(100, 30);
            exitButton.Location = new Point(10, 50);
            exitButton.Click += ExitButton_Click;
            this.Controls.Add(exitButton);

            // Initialize numeric up-down controls for RGB values
            numericR = new NumericUpDown() { Minimum = 0, Maximum = 255, Location = new Point(10, 90), Size = new Size(60, 30) };
            numericG = new NumericUpDown() { Minimum = 0, Maximum = 255, Location = new Point(10, 130), Size = new Size(60, 30) };
            numericB = new NumericUpDown() { Minimum = 0, Maximum = 255, Location = new Point(10, 170), Size = new Size(60, 30) };

            // Add labels for RGB controls
            this.Controls.Add(new Label() { Text = "R", Location = new Point(75, 90), Size = new Size(20, 30) });
            this.Controls.Add(new Label() { Text = "G", Location = new Point(75, 130), Size = new Size(20, 30) });
            this.Controls.Add(new Label() { Text = "B", Location = new Point(75, 170), Size = new Size(20, 30) });

            // Add RGB controls to form
            this.Controls.Add(numericR);
            this.Controls.Add(numericG);
            this.Controls.Add(numericB);

            // Initialize transparency track bar
            transparencyBar = new TrackBar();
            transparencyBar.Minimum = 0;
            transparencyBar.Maximum = 255;
            transparencyBar.Value = 128; // Default to 50% transparency
            transparencyBar.TickFrequency = 10;
            transparencyBar.Location = new Point(10, 210);
            transparencyBar.Size = new Size(100, 30);
            this.Controls.Add(transparencyBar);

            // Initialize apply color button
            applyColorButton = new Button();
            applyColorButton.Text = "Apply Color";
            applyColorButton.Size = new Size(100, 30);
            applyColorButton.Location = new Point(10, 250);
            applyColorButton.Click += ApplyColorButton_Click;
            this.Controls.Add(applyColorButton);

            // Initialize X-ray button
            xrayButton = new Button();
            xrayButton.Text = "X-Ray";
            xrayButton.Size = new Size(100, 30);
            xrayButton.Location = new Point(10, 290);
            xrayButton.Click += XrayButton_Click;
            this.Controls.Add(xrayButton);

            // Initialize picture box
            pictureBox = new PictureBox();
            pictureBox.Size = new Size(640, 480); // Adjust size
            pictureBox.Location = new Point(150, 10);
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox.Image = LoadImageWithoutIndexedFormat(imagePath);
            pictureBox.MouseDown += PictureBox_MouseDown;
            pictureBox.MouseMove += PictureBox_MouseMove;
            pictureBox.MouseUp += PictureBox_MouseUp;
            pictureBox.Paint += PictureBox_Paint;
            this.Controls.Add(pictureBox);
        }

        private Bitmap LoadImageWithoutIndexedFormat(string imagePath)
        {
            using (Bitmap original = new Bitmap(imagePath))
            {
                Bitmap nonIndexed = new Bitmap(original.Width, original.Height, PixelFormat.Format24bppRgb);
                using (Graphics g = Graphics.FromImage(nonIndexed))
                {
                    g.DrawImage(original, 0, 0, original.Width, original.Height);
                }
                return nonIndexed;
            }
        }

        private void PictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            startPoint = e.Location;
            isSelecting = true;
        }

        private void PictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (isSelecting)
            {
                int x = Math.Min(startPoint.X, e.X);
                int y = Math.Min(startPoint.Y, e.Y);
                int width = Math.Abs(e.X - startPoint.X);
                int height = Math.Abs(e.Y - startPoint.Y);

                // Ensure the rectangle is within image boundaries
                x = Math.Max(0, x);
                y = Math.Max(0, y);
                width = Math.Min(pictureBox.Width - x, width);
                height = Math.Min(pictureBox.Height - y, height);

                selectedRegion = new Rectangle(x, y, width, height);
                pictureBox.Invalidate(); // Redraw to show the rectangle
            }
        }

        private void PictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            isSelecting = false;
            pictureBox.Invalidate(); // Ensure final rectangle is drawn
        }

        private void PictureBox_Paint(object sender, PaintEventArgs e)
        {
            if (isSelecting || selectedRegion != Rectangle.Empty)
            {
                using (Pen pen = new Pen(Color.Red, 2))
                {
                    e.Graphics.DrawRectangle(pen, selectedRegion);
                }
            }
        }

        private void ApplyColorButton_Click(object sender, EventArgs e)
        {
            int r = (int)numericR.Value;
            int g = (int)numericG.Value;
            int b = (int)numericB.Value;
            int transparency = transparencyBar.Value;
            Color selectedColor = Color.FromArgb(transparency, r, g, b);

            if (originalImage == null)
            {
                originalImage = (Bitmap)pictureBox.Image;
            }

            Bitmap modifiedImage = (Bitmap)originalImage.Clone();

            // Get the aspect ratio of the image and the picture box
            float imageAspectRatio = (float)originalImage.Width / originalImage.Height;
            float boxAspectRatio = (float)pictureBox.ClientSize.Width / pictureBox.ClientSize.Height;

            // Determine the scaling factors and offsets
            float scaleX, scaleY;
            int offsetX = 0, offsetY = 0;
            if (imageAspectRatio > boxAspectRatio)
            {
                scaleX = (float)originalImage.Width / pictureBox.ClientSize.Width;
                scaleY = scaleX;
                offsetY = (int)((pictureBox.ClientSize.Height - (originalImage.Height / scaleY)) / 2);
            }
            else
            {
                scaleY = (float)originalImage.Height / pictureBox.ClientSize.Height;
                scaleX = scaleY;
                offsetX = (int)((pictureBox.ClientSize.Width - (originalImage.Width / scaleX)) / 2);
            }

            // Calculate the corresponding coordinates in the original image
            int startX = (int)((selectedRegion.Left - offsetX) * scaleX);
            int startY = (int)((selectedRegion.Top - offsetY) * scaleY);
            int endX = (int)((selectedRegion.Right - offsetX) * scaleX);
            int endY = (int)((selectedRegion.Bottom - offsetY) * scaleY);

            // Ensure the coordinates are within the image bounds
            startX = Math.Max(0, startX);
            startY = Math.Max(0, startY);
            endX = Math.Min(modifiedImage.Width, endX);
            endY = Math.Min(modifiedImage.Height, endY);

            // Iterate through each pixel in the selected region
            for (int y = startY; y < endY; y++)
            {
                for (int x = startX; x < endX; x++)
                {
                    // Get the original color at the current pixel
                    Color originalColor = originalImage.GetPixel(x, y);

                    // Blend the original color with the selected color using the specified transparency
                    int blendedR = (selectedColor.A * selectedColor.R + (255 - selectedColor.A) * originalColor.R) / 255;
                    int blendedG = (selectedColor.A * selectedColor.G + (255 - selectedColor.A) * originalColor.G) / 255;
                    int blendedB = (selectedColor.A * selectedColor.B + (255 - selectedColor.A) * originalColor.B) / 255;

                    Color blendedColor = Color.FromArgb(blendedR, blendedG, blendedB);
                    modifiedImage.SetPixel(x, y, blendedColor);
                }
            }

            // Display the processed image with the selected region
            pictureBox.Image = modifiedImage;
        }

        private void XrayButton_Click(object sender, EventArgs e)
        {
            if (originalImage == null)
            {
                originalImage = (Bitmap)pictureBox.Image;
            }

            Bitmap xrayImage = (Bitmap)originalImage.Clone();

            // Iterate through each pixel in the image
            for (int y = 0; y < xrayImage.Height; y++)
            {
                for (int x = 0; x < xrayImage.Width; x++)
                {
                    // Get the original color at the current pixel
                    Color originalColor = originalImage.GetPixel(x, y);

                    // Apply X-ray effect (invert the colors)
                    Color xrayColor = Color.FromArgb(255 - originalColor.R, 255 - originalColor.G, 255 - originalColor.B);

                    // Set the new color at the current pixel
                    xrayImage.SetPixel(x, y, xrayColor);
                }
            }

            // Display the X-ray processed image
            pictureBox.Image = xrayImage;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (pictureBox.Image != null)
            {
                // Get the directory of the original image
                string originalDirectory = Path.GetDirectoryName(originalImagePath);
                // Get the file name without extension
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalImagePath);
                // Create a new directory with the same name as the original image
                string newDirectory = Path.Combine(originalDirectory, fileNameWithoutExtension);

                if (!Directory.Exists(newDirectory))
                {
                    Directory.CreateDirectory(newDirectory);
                }

                // Define the save path for the modified image
                string savePath = Path.Combine(newDirectory, $"{fileNameWithoutExtension}.png");

                // Save the modified image
                pictureBox.Image.Save(savePath, ImageFormat.Png);
                MessageBox.Show("Image saved successfully in: " + newDirectory);
            }
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
