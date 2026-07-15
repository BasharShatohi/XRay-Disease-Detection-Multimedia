using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ImageApp
{
    public class CropForm : Form
    {
        private PictureBox originalPictureBox;
        private PictureBox croppedPictureBox;
        private Button loadImageButton;
        private Button cropButton;
        private Button saveImageButton;
        private Button autoSaveButton;  
        private Rectangle selectedRegion;
        private bool isSelecting;
        private Bitmap loadedImage;
        private Bitmap croppedImage;
        private string loadedImagePath;  

        public CropForm()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "Crop Image";
            this.Size = new Size(800, 400);

            loadImageButton = new Button();
            loadImageButton.Text = "Load Image";
            loadImageButton.Size = new Size(100, 30);
            loadImageButton.Location = new Point(50, 10);
            loadImageButton.Click += LoadImageButton_Click;
            this.Controls.Add(loadImageButton);

            originalPictureBox = new PictureBox();
            originalPictureBox.Size = new Size(300, 300);
            originalPictureBox.Location = new Point(50, 50);
            originalPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            originalPictureBox.BorderStyle = BorderStyle.FixedSingle;
            originalPictureBox.MouseDown += PictureBox_MouseDown;
            originalPictureBox.MouseMove += PictureBox_MouseMove;
            originalPictureBox.MouseUp += PictureBox_MouseUp;
            originalPictureBox.Paint += PictureBox_Paint;
            this.Controls.Add(originalPictureBox);

            croppedPictureBox = new PictureBox();
            croppedPictureBox.Size = new Size(300, 300);
            croppedPictureBox.Location = new Point(450, 50);
            croppedPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            croppedPictureBox.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(croppedPictureBox);

            cropButton = new Button();
            cropButton.Text = "Crop";
            cropButton.Size = new Size(100, 30);
            cropButton.Location = new Point(350, 180);
            cropButton.Click += CropButton_Click;
            this.Controls.Add(cropButton);

            saveImageButton = new Button();
            saveImageButton.Text = "Save Image";
            saveImageButton.Size = new Size(100, 30);
            saveImageButton.Location = new Point(350, 220);
            saveImageButton.Click += SaveImageButton_Click;
            this.Controls.Add(saveImageButton);

            autoSaveButton = new Button();
            autoSaveButton.Text = "Auto Save";
            autoSaveButton.Size = new Size(100, 30);
            autoSaveButton.Location = new Point(350, 260); 
            autoSaveButton.Click += AutoSaveButton_Click;
            this.Controls.Add(autoSaveButton);
        }

        private void LoadImageButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files(*.jpg; *.jpeg; *.png)|*.jpg; *.jpeg; *.png";
            openFileDialog.Title = "Select an Image";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                loadedImage = new Bitmap(openFileDialog.FileName);
                loadedImagePath = openFileDialog.FileName; 
                originalPictureBox.Image = loadedImage;
            }
        }

        private void CropButton_Click(object sender, EventArgs e)
        {
            if (selectedRegion != Rectangle.Empty && selectedRegion.Width > 0 && selectedRegion.Height > 0)
            {
                if (loadedImage != null)
                {
                    Rectangle scaledSelectedRegion = GetScaledRectangle(originalPictureBox, loadedImage, selectedRegion);
                    croppedImage = CropImage(loadedImage, scaledSelectedRegion);
                    croppedPictureBox.Image = croppedImage;
                    MessageBox.Show("Image cropped successfully.");
                }
                else
                {
                    MessageBox.Show("Please load an image first.");
                }
            }
            else
            {
                MessageBox.Show("Please select a region to crop.");
            }
        }

        private void SaveImageButton_Click(object sender, EventArgs e)
        {
            if (croppedImage != null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Image Files(*.jpg; *.jpeg; *.png)|*.jpg; *.jpeg; *.png";
                saveFileDialog.Title = "Save an Image File";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    croppedImage.Save(saveFileDialog.FileName);
                    MessageBox.Show("Image saved successfully.");
                }
            }
            else
            {
                MessageBox.Show("No image to save. Please crop an image first.");
            }
        }

        private Bitmap CropImage(Bitmap source, Rectangle cropRegion)
        {
            Bitmap croppedImage = new Bitmap(cropRegion.Width, cropRegion.Height);
            using (Graphics g = Graphics.FromImage(croppedImage))
            {
                g.DrawImage(source, 0, 0, cropRegion, GraphicsUnit.Pixel);
            }
            return croppedImage;
        }

        private Rectangle GetScaledRectangle(PictureBox pictureBox, Bitmap image, Rectangle selectedRegion)
        {
            float xRatio = (float)image.Width / pictureBox.ClientSize.Width;
            float yRatio = (float)image.Height / pictureBox.ClientSize.Height;

            int x = (int)(selectedRegion.X * xRatio);
            int y = (int)(selectedRegion.Y * yRatio);
            int width = (int)(selectedRegion.Width * xRatio);
            int height = (int)(selectedRegion.Height * yRatio);

            return new Rectangle(x, y, width, height);
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
                originalPictureBox.Invalidate();
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

        private void AutoSaveButton_Click(object sender, EventArgs e)
        {
            if (croppedImage == null)
            {
                MessageBox.Show("No cropped image to save. Please crop an image first.");
                return;
            }

            string directory = Path.GetDirectoryName(loadedImagePath);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(loadedImagePath);
            string extension = Path.GetExtension(loadedImagePath);
            string autoSaveFileName = Path.Combine(directory, $"{fileNameWithoutExtension}_Crop{extension}");

            try
            {
                croppedImage.Save(autoSaveFileName);
                MessageBox.Show($"Image saved automatically as {autoSaveFileName}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving image: {ex.Message}");
            }
        }
    }
}
