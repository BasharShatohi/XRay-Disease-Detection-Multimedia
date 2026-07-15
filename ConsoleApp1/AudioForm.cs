using System;
using System.Drawing;
using System.IO;
using System.Media;
using System.Windows.Forms;
using NAudio.Wave;

namespace ImageApp
{
    public class AudioForm : Form
    {
        private PictureBox pictureBox;
        private Button loadImageButton;
        private Button recordAudioButton;
        private Button stopRecordingButton;
        private Button playAudioButton;
        private Button stopAudioButton; // زر لإيقاف التشغيل
        private Label recordingStatusLabel; // Label لإظهار حالة التسجيل
        private Bitmap loadedImage;
        private WaveInEvent waveIn;
        private WaveFileWriter writer;
        private string audioFilePath;
        private string imagePath; // متغير للاحتفاظ بمسار ملف الصورة
        private SoundPlayer player; // مشغل الصوت

        public AudioForm()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "Record Audio";
            this.Size = new Size(800, 400);

            loadImageButton = new Button();
            loadImageButton.Text = "Load Image";
            loadImageButton.Size = new Size(100, 30);
            loadImageButton.Location = new Point(50, 10);
            loadImageButton.Click += LoadImageButton_Click;
            this.Controls.Add(loadImageButton);

            recordAudioButton = new Button();
            recordAudioButton.Text = "Record Audio";
            recordAudioButton.Size = new Size(100, 30);
            recordAudioButton.Location = new Point(160, 10);
            recordAudioButton.Click += RecordAudioButton_Click;
            this.Controls.Add(recordAudioButton);

            stopRecordingButton = new Button();
            stopRecordingButton.Text = "Stop Recording";
            stopRecordingButton.Size = new Size(120, 30);
            stopRecordingButton.Location = new Point(270, 10);
            stopRecordingButton.Click += StopRecordingButton_Click;
            this.Controls.Add(stopRecordingButton);

            playAudioButton = new Button();
            playAudioButton.Text = "Play Audio";
            playAudioButton.Size = new Size(100, 30);
            playAudioButton.Location = new Point(400, 10);
            playAudioButton.Click += PlayAudioButton_Click;
            this.Controls.Add(playAudioButton);

            stopAudioButton = new Button();
            stopAudioButton.Text = "Stop Audio";
            stopAudioButton.Size = new Size(100, 30);
            stopAudioButton.Location = new Point(510, 10);
            stopAudioButton.Click += StopAudioButton_Click;
            this.Controls.Add(stopAudioButton);

            recordingStatusLabel = new Label();
            recordingStatusLabel.Text = "";
            recordingStatusLabel.Size = new Size(200, 30);
            recordingStatusLabel.Location = new Point(620, 10);
            this.Controls.Add(recordingStatusLabel);

            pictureBox = new PictureBox();
            pictureBox.Size = new Size(700, 300);
            pictureBox.Location = new Point(50, 50);
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(pictureBox);

            waveIn = new WaveInEvent();
            waveIn.DeviceNumber = 0; // اختيار الجهاز الافتراضي
            waveIn.WaveFormat = new WaveFormat(44100, 1); // 44100 هرتز، قناة واحدة (مونو)
            waveIn.DataAvailable += WaveIn_DataAvailable;
            waveIn.RecordingStopped += WaveIn_RecordingStopped;
        }

        private void LoadImageButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files(*.jpg; *.jpeg; *.png)|*.jpg; *.jpeg; *.png";
            openFileDialog.Title = "Select an Image";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                imagePath = openFileDialog.FileName; // حفظ مسار ملف الصورة
                loadedImage = new Bitmap(imagePath);
                pictureBox.Image = loadedImage;
            }
        }

        private void RecordAudioButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(imagePath))
            {
                MessageBox.Show("Please load an image first.");
                return;
            }

            string directory = Path.GetDirectoryName(imagePath);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(imagePath);
            audioFilePath = Path.Combine(directory, fileNameWithoutExtension + ".wav"); // توليد مسار ملف الصوت باستخدام اسم ملف الصورة

            writer = new WaveFileWriter(audioFilePath, waveIn.WaveFormat);
            waveIn.StartRecording();
            recordingStatusLabel.Text = "Recording..."; // تحديث النص عند بدء التسجيل
        }

        private void StopRecordingButton_Click(object sender, EventArgs e)
        {
            waveIn.StopRecording();
            recordingStatusLabel.Text = ""; // إعادة تعيين النص عند إيقاف التسجيل
        }

        private void PlayAudioButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(audioFilePath) || !File.Exists(audioFilePath))
            {
                MessageBox.Show("No audio file to play.");
                return;
            }

            player = new SoundPlayer(audioFilePath);
            player.Play();
        }

        private void StopAudioButton_Click(object sender, EventArgs e)
        {
            if (player != null)
            {
                player.Stop();
            }
        }

        private void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (writer != null)
            {
                writer.Write(e.Buffer, 0, e.BytesRecorded);
                writer.Flush();
            }
        }

        private void WaveIn_RecordingStopped(object sender, StoppedEventArgs e)
        {
            if (writer != null)
            {
                writer.Dispose();
                writer = null;
            }
        }
    }
}
