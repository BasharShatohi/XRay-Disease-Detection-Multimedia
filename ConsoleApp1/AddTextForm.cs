using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ImageApp
{
    public class AddTextForm : Form
    {
        private PictureBox pictureBox;
        private TextBox textBox;
        private Button loadImageButton;
        private Button addTextButton;
        private Button saveImageButton;
        private Button changeFontButton;
        private Button changeColorButton;
        private Button autoSaveButton;  // زر الحفظ التلقائي الجديد
        private Label instructionsLabel;
        private Bitmap loadedImage;
        private string loadedImagePath;  // مسار الصورة الأصلية
        private string addedText = string.Empty;
        private PointF textPosition = new PointF(10, 10);
        private Font textFont = new Font("Arial", 20);
        private Color textColor = Color.Red;

        public AddTextForm()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "Add Text to Image";
            this.Size = new Size(1000, 500);

            loadImageButton = new Button();
            loadImageButton.Text = "Load Image";
            loadImageButton.Size = new Size(100, 30);
            loadImageButton.Location = new Point(50, 10);
            loadImageButton.Click += LoadImageButton_Click;
            this.Controls.Add(loadImageButton);

            textBox = new TextBox();
            textBox.Size = new Size(200, 30);
            textBox.Location = new Point(160, 10);
            this.Controls.Add(textBox);

            addTextButton = new Button();
            addTextButton.Text = "Add Text";
            addTextButton.Size = new Size(100, 30);
            addTextButton.Location = new Point(370, 10);
            addTextButton.Click += AddTextButton_Click;
            this.Controls.Add(addTextButton);

            changeFontButton = new Button();
            changeFontButton.Text = "Change Font";
            changeFontButton.Size = new Size(100, 30);
            changeFontButton.Location = new Point(480, 10);
            changeFontButton.Click += ChangeFontButton_Click;
            this.Controls.Add(changeFontButton);

            changeColorButton = new Button();
            changeColorButton.Text = "Change Color";
            changeColorButton.Size = new Size(100, 30);
            changeColorButton.Location = new Point(590, 10);
            changeColorButton.Click += ChangeColorButton_Click;
            this.Controls.Add(changeColorButton);

            saveImageButton = new Button();
            saveImageButton.Text = "Save Image";
            saveImageButton.Size = new Size(100, 30);
            saveImageButton.Location = new Point(700, 10);
            saveImageButton.Click += SaveImageButton_Click;
            this.Controls.Add(saveImageButton);

            autoSaveButton = new Button();
            autoSaveButton.Text = "Auto Save";
            autoSaveButton.Size = new Size(100, 30);
            autoSaveButton.Location = new Point(810, 10);
            autoSaveButton.Click += AutoSaveButton_Click; 
            this.Controls.Add(autoSaveButton);

            instructionsLabel = new Label();
            instructionsLabel.Text = "Use W, A, S, D keys to move the text.";
            instructionsLabel.Size = new Size(300, 30);
            instructionsLabel.Location = new Point(50, 40);
            this.Controls.Add(instructionsLabel);

            pictureBox = new PictureBox();
            pictureBox.Size = new Size(700, 350);
            pictureBox.Location = new Point(50, 80);
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(pictureBox);

            this.KeyPreview = true;
            this.KeyDown += AddTextForm_KeyDown;
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
                pictureBox.Image = loadedImage;
                addedText = string.Empty; 
                textPosition = new PointF(10, 10); 
            }
        }

        private void AddTextButton_Click(object sender, EventArgs e)
        {
            if (loadedImage != null && !string.IsNullOrEmpty(textBox.Text))
            {
                addedText = textBox.Text;
                UpdateImageWithText();
            }
            else
            {
                MessageBox.Show("Please load an image and enter text.");
            }
        }

        private void UpdateImageWithText()
        {
            Bitmap bitmapWithText = new Bitmap(loadedImage);
            using (Graphics g = Graphics.FromImage(bitmapWithText))
            {
                using (SolidBrush brush = new SolidBrush(textColor))
                {
                    SizeF textSize = g.MeasureString(addedText, textFont);
                    float x = textPosition.X;
                    float y = textPosition.Y;

                    if (textPosition.X < 0)
                    {
                        x = 0;
                    }
                    else if (textPosition.X > loadedImage.Width - textSize.Width)
                    {
                        x = loadedImage.Width - textSize.Width;
                    }

                    if (textPosition.Y < 0)
                    {
                        y = 0;
                    }
                    else if (textPosition.Y > loadedImage.Height - textSize.Height)
                    {
                        y = loadedImage.Height - textSize.Height;
                    }

                    g.DrawString(addedText, textFont, brush, x, y);
                }
            }
            pictureBox.Image = bitmapWithText;
        }

        private void AddTextForm_KeyDown(object sender, KeyEventArgs e)
        {
            const float moveStep = 5.0f;

            switch (e.KeyCode)
            {
                case Keys.W:
                    textPosition.Y -= moveStep;
                    break;
                case Keys.S:
                    textPosition.Y += moveStep;
                    break;
                case Keys.A:
                    textPosition.X -= moveStep;
                    break;
                case Keys.D:
                    textPosition.X += moveStep;
                    break;
            }
            UpdateImageWithText();
        }

        private void ChangeFontButton_Click(object sender, EventArgs e)
        {
            FontDialog fontDialog = new FontDialog();
            if (fontDialog.ShowDialog() == DialogResult.OK)
            {
                textFont = fontDialog.Font;
                UpdateImageWithText();
            }
        }

        private void ChangeColorButton_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                textColor = colorDialog.Color;
                UpdateImageWithText();
            }
        }

        private void SaveImageButton_Click(object sender, EventArgs e)
        {
            if (pictureBox.Image != null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "PNG Image|*.png|JPEG Image|*.jpg";
                saveFileDialog.Title = "Save an Image File";
                saveFileDialog.FileName = "image_with_text";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (saveFileDialog.FileName != "")
                    {
                        using (FileStream fs = (FileStream)saveFileDialog.OpenFile())
                        {
                            switch (saveFileDialog.FilterIndex)
                            {
                                case 1:
                                    pictureBox.Image.Save(fs, System.Drawing.Imaging.ImageFormat.Png);
                                    break;
                                case 2:
                                    pictureBox.Image.Save(fs, System.Drawing.Imaging.ImageFormat.Jpeg);
                                    break;
                            }
                        }
                        MessageBox.Show("Image saved successfully.");
                    }
                }
            }
            else
            {
                MessageBox.Show("No image to save.");
            }
        }

        private void AutoSaveButton_Click(object sender, EventArgs e)
        {
            if (loadedImage == null)
            {
                MessageBox.Show("No image loaded.");
                return;
            }
            string directory = Path.GetDirectoryName(loadedImagePath);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(loadedImagePath);
            string extension = Path.GetExtension(loadedImagePath);
            string autoSaveFileName = Path.Combine(directory, $"{fileNameWithoutExtension}_text{extension}");

            try
            {
                pictureBox.Image.Save(autoSaveFileName);
                MessageBox.Show($"Image saved automatically as {autoSaveFileName}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving image: {ex.Message}");
            }
        }
    }
}   
