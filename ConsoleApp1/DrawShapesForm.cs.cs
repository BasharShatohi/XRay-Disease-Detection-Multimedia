using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.IO;

namespace ImageApp
{
    public class DrawShapesForm : Form
    {
        private Bitmap originalImage;
        private Bitmap workingImage;
        private PictureBox pictureBox;
        private Button loadImageButton;
        private Button drawRectangleButton;
        private Button drawCircleButton;
        private Button drawSquareButton;
        private Button drawTriangleButton;
        private Button saveSelectionButton;
        private Button autoSaveButton;
        private Button removeDrawButton; 
        private Point startPoint;
        private Point endPoint;
        private bool isDrawing;
        private string currentShape = "Rectangle";
        private string originalImageLocation;

        public DrawShapesForm()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "Draw Shapes on Image";
            this.Size = new Size(1200, 600);

            Size buttonSize = new Size(120, 20);

            loadImageButton = new Button { Text = "Load Image", Location = new Point(10, 10), Size = buttonSize };
            drawRectangleButton = new Button { Text = "Draw Rectangle", Location = new Point(140, 10), Size = buttonSize };
            drawCircleButton = new Button { Text = "Draw Circle", Location = new Point(270, 10), Size = buttonSize };
            drawSquareButton = new Button { Text = "Draw Square", Location = new Point(400, 10), Size = buttonSize };
            drawTriangleButton = new Button { Text = "Draw Triangle", Location = new Point(530, 10), Size = buttonSize };
            saveSelectionButton = new Button { Text = "Save Selection", Location = new Point(660, 10), Size = buttonSize };
            autoSaveButton = new Button { Text = "Auto Save", Location = new Point(790, 10), Size = buttonSize };
            removeDrawButton = new Button { Text = "Remove Draw", Location = new Point(920, 10), Size = buttonSize }; 

            pictureBox = new PictureBox { Location = new Point(10, 60), Size = new Size(760, 500), BorderStyle = BorderStyle.FixedSingle };

            loadImageButton.Click += LoadImageButton_Click;
            drawRectangleButton.Click += (s, e) => currentShape = "Rectangle";
            drawCircleButton.Click += (s, e) => currentShape = "Circle";
            drawSquareButton.Click += (s, e) => currentShape = "Square";
            drawTriangleButton.Click += (s, e) => currentShape = "Triangle";
            saveSelectionButton.Click += SaveSelectionButton_Click;
            autoSaveButton.Click += AutoSaveButton_Click;
            removeDrawButton.Click += RemoveDrawButton_Click; 

            pictureBox.MouseDown += PictureBox_MouseDown;
            pictureBox.MouseMove += PictureBox_MouseMove;
            pictureBox.MouseUp += PictureBox_MouseUp;
            pictureBox.Paint += PictureBox_Paint;

            this.Controls.Add(loadImageButton);
            this.Controls.Add(drawRectangleButton);
            this.Controls.Add(drawCircleButton);
            this.Controls.Add(drawSquareButton);
            this.Controls.Add(drawTriangleButton);
            this.Controls.Add(saveSelectionButton);
            this.Controls.Add(autoSaveButton);
            this.Controls.Add(removeDrawButton); 
            this.Controls.Add(pictureBox);
        }

        private void LoadImageButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files(*.jpg; *.jpeg; *.png)|*.jpg; *.jpeg; *.png";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    originalImage = new Bitmap(openFileDialog.FileName);
                    workingImage = new Bitmap(originalImage);
                    pictureBox.Image = workingImage;
                    originalImageLocation = openFileDialog.FileName;
                }
            }
        }

        private void PictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDrawing = true;
                startPoint = e.Location;
            }
        }

        private void PictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                endPoint = e.Location;
                pictureBox.Invalidate();
            }
        }

        private void PictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                isDrawing = false;
                endPoint = e.Location;
                DrawShape(workingImage, currentShape, startPoint, endPoint);
                pictureBox.Invalidate();
            }
        }

        private void PictureBox_Paint(object sender, PaintEventArgs e)
        {
            if (isDrawing)
            {
                DrawShape(e.Graphics, currentShape, startPoint, endPoint);
            }
        }

        private void DrawShape(Graphics g, string shape, Point start, Point end)
        {
            Pen pen = new Pen(Color.Red, 2);
            int x = Math.Min(start.X, end.X);
            int y = Math.Min(start.Y, end.Y);
            int width = Math.Abs(start.X - end.X);
            int height = Math.Abs(start.Y - end.Y);

            if (shape == "Rectangle")
            {
                g.DrawRectangle(pen, x, y, width, height);
            }
            else if (shape == "Circle")
            {
                g.DrawEllipse(pen, x, y, width, height);
            }
            else if (shape == "Square")
            {
                int side = Math.Min(width, height);
                g.DrawRectangle(pen, x, y, side, side);
            }
            else if (shape == "Triangle")
            {
                Point[] points = GetTriangle(start, end);
                g.DrawPolygon(pen, points);
            }
        }

        private void DrawShape(Bitmap bitmap, string shape, Point start, Point end)
        {
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                DrawShape(g, shape, start, end);
            }
        }

        private Point[] GetTriangle(Point start, Point end)
        {
            Point p1 = start;
            Point p2 = new Point(start.X, end.Y);
            Point p3 = end;
            return new Point[] { p1, p2, p3 };
        }

        private void SaveSelectionButton_Click(object sender, EventArgs e)
        {
            if (startPoint == Point.Empty || endPoint == Point.Empty)
            {
                MessageBox.Show("Please select an area first.");
                return;
            }

            int x = Math.Min(startPoint.X, endPoint.X);
            int y = Math.Min(startPoint.Y, endPoint.Y);
            int width = Math.Abs(startPoint.X - endPoint.X);
            int height = Math.Abs(startPoint.Y - endPoint.Y);

            if (width == 0 || height == 0)
            {
                MessageBox.Show("Selected area is invalid.");
                return;
            }

            Rectangle selectedRect = new Rectangle(x, y, width, height);
            Bitmap selectedImage = new Bitmap(selectedRect.Width, selectedRect.Height);

            using (Graphics g = Graphics.FromImage(selectedImage))
            {
                g.DrawImage(originalImage, new Rectangle(0, 0, selectedImage.Width, selectedImage.Height), selectedRect, GraphicsUnit.Pixel);
            }

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "PNG Image|*.png|JPEG Image|*.jpg;*.jpeg";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    selectedImage.Save(saveFileDialog.FileName);
                }
            }
        }

        private void AutoSaveButton_Click(object sender, EventArgs e)
        {
            if (originalImage == null)
            {
                MessageBox.Show("No image loaded.");
                return;
            }

            string directory = Path.GetDirectoryName(originalImageLocation);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalImageLocation);
            string extension = Path.GetExtension(originalImageLocation);
            string autoSaveFileName = Path.Combine(directory, $"{fileNameWithoutExtension}_draw{extension}");

            try
            {
                workingImage.Save(autoSaveFileName);
                MessageBox.Show($"Image saved automatically as {autoSaveFileName}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving image: {ex.Message}");
            }
        }

        private void RemoveDrawButton_Click(object sender, EventArgs e)
        {
            if (originalImage == null)
            {
                MessageBox.Show("No image loaded.");
                return;
            }

            workingImage = new Bitmap(originalImage);
            pictureBox.Image = workingImage;
            pictureBox.Invalidate();
        }
    }
}
