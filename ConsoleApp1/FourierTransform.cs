using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex32;
using Complex32 = MathNet.Numerics.Complex32;

namespace ImageApp
{
    public class FourierTransform : Form
    {
        private Button loadButton;
        private Button applyButton;
        private Button saveButton;
        private Button autoSaveButton;
        private PictureBox originalPictureBox;
        private PictureBox transformedPictureBox;
        private Bitmap loadedImage;
        private Bitmap transformedImage;
        private string loadedImagePath;

        public FourierTransform()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "Fourier Transform Filter";
            this.Size = new Size(1200, 700);

            loadButton = new Button();
            loadButton.Text = "Load Image";
            loadButton.Location = new Point(20, 20);
            loadButton.Click += LoadButton_Click;
            this.Controls.Add(loadButton);

            applyButton = new Button();
            applyButton.Text = "Apply Fourier Transform";
            applyButton.Location = new Point(120, 20);
            applyButton.Click += ApplyButton_Click;
            this.Controls.Add(applyButton);

            saveButton = new Button();
            saveButton.Text = "Save Image";
            saveButton.Location = new Point(220, 20);
            saveButton.Click += SaveButton_Click;
            this.Controls.Add(saveButton);

            autoSaveButton = new Button();
            autoSaveButton.Text = "Auto Save Image";
            autoSaveButton.Location = new Point(320, 20);
            autoSaveButton.Click += AutoSaveButton_Click;
            this.Controls.Add(autoSaveButton);

            originalPictureBox = new PictureBox();
            originalPictureBox.Location = new Point(20, 60);
            originalPictureBox.Size = new Size(500, 500);
            originalPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            originalPictureBox.BorderStyle = BorderStyle.Fixed3D;
            this.Controls.Add(originalPictureBox);

            transformedPictureBox = new PictureBox();
            transformedPictureBox.Location = new Point(540, 60);
            transformedPictureBox.Size = new Size(500, 500);
            transformedPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            transformedPictureBox.BorderStyle = BorderStyle.Fixed3D;
            this.Controls.Add(transformedPictureBox);
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                loadedImagePath = openFileDialog.FileName;
                loadedImage = new Bitmap(openFileDialog.FileName);
                originalPictureBox.Image = loadedImage;
            }
        }

        private void ApplyButton_Click(object sender, EventArgs e)
        {
            if (loadedImage == null)
            {
                MessageBox.Show("Please load an image first.");
                return;
            }

            transformedImage = ApplyFourierTransform(loadedImage);
            transformedPictureBox.Image = transformedImage;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (transformedImage == null)
            {
                MessageBox.Show("No transformed image to save.");
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG Image|*.png|JPEG Image|*.jpg";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                transformedImage.Save(saveFileDialog.FileName, ImageFormat.Png);
            }
        }

        private void AutoSaveButton_Click(object sender, EventArgs e)
        {
            if (transformedImage == null)
            {
                MessageBox.Show("No transformed image to save.");
                return;
            }

            if (string.IsNullOrEmpty(loadedImagePath))
            {
                MessageBox.Show("No original image loaded.");
                return;
            }

            string directory = Path.GetDirectoryName(loadedImagePath);
            string filename = Path.GetFileNameWithoutExtension(loadedImagePath);
            string extension = Path.GetExtension(loadedImagePath);
            string newFilename = $"{filename}_Fourier{extension}";
            string newPath = Path.Combine(directory, newFilename);

            transformedImage.Save(newPath, ImageFormat.Png);
            MessageBox.Show($"Image saved to {newPath}");
        }

        private Bitmap ApplyFourierTransform(Bitmap image)
        {
            FourierOptions options = FourierOptions.Default;

            int width = image.Width;
            int height = image.Height;
            Complex32[,] data = new Complex32[width, height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color pixelColor = image.GetPixel(x, y);
                    float gray = (pixelColor.R + pixelColor.G + pixelColor.B) / 3.0f;
                    data[x, y] = new Complex32(gray, 0);
                }
            }

            var matrix = DenseMatrix.OfArray(data);

            for (int i = 0; i < matrix.RowCount; i++)
            {
                var row = matrix.Row(i).ToArray();
                Fourier.Forward(row, options);
                matrix.SetRow(i, DenseVector.OfArray(row));
            }

            for (int j = 0; j < matrix.ColumnCount; j++)
            {
                var col = matrix.Column(j).ToArray();
                Fourier.Forward(col, options);
                matrix.SetColumn(j, DenseVector.OfArray(col));
            }

            Bitmap filteredImage = ApplyGaussianSharpenFilter(image);

            Bitmap resultImage = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(resultImage))
            {
                g.DrawImage(filteredImage, Point.Empty);
            }

            return resultImage;
        }

        private Bitmap ApplyGaussianSharpenFilter(Bitmap image)
        {
            AForge.Imaging.Filters.GaussianSharpen filter = new AForge.Imaging.Filters.GaussianSharpen();
            filter.Size = 5; 
            filter.Sigma = 3;
            return filter.Apply(image);
        }




    }
}
