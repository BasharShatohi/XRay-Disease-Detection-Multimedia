using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ImageApp
{
    public class SearchForm : Form
    {
        private TextBox sizeTextBox;
        private DateTimePicker dateModifiedPicker;
        private Button searchButton;
        private ListBox resultsListBox;
        private TextBox searchPathTextBox;
        private Button browseButton;

        public SearchForm()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "Search Images";
            this.Size = new Size(600, 450);

            Label searchPathLabel = new Label();
            searchPathLabel.Text = "Search Path:";
            searchPathLabel.Location = new Point(20, 20);
            this.Controls.Add(searchPathLabel);

            searchPathTextBox = new TextBox();
            searchPathTextBox.Location = new Point(150, 20);
            searchPathTextBox.Width = 300;
            this.Controls.Add(searchPathTextBox);

            browseButton = new Button();
            browseButton.Text = "Browse";
            browseButton.Location = new Point(460, 20);
            browseButton.Click += BrowseButton_Click;
            this.Controls.Add(browseButton);

            Label sizeLabel = new Label();
            sizeLabel.Text = "Image Size (KB):";
            sizeLabel.Location = new Point(20, 60);
            this.Controls.Add(sizeLabel);

            sizeTextBox = new TextBox();
            sizeTextBox.Location = new Point(150, 60);
            this.Controls.Add(sizeTextBox);

            Label dateModifiedLabel = new Label();
            dateModifiedLabel.Text = "Date Modified:";
            dateModifiedLabel.Location = new Point(20, 100);
            this.Controls.Add(dateModifiedLabel);

            dateModifiedPicker = new DateTimePicker();
            dateModifiedPicker.Format = DateTimePickerFormat.Short;
            dateModifiedPicker.Location = new Point(150, 100);
            this.Controls.Add(dateModifiedPicker);

            searchButton = new Button();
            searchButton.Text = "Search";
            searchButton.Location = new Point(150, 140);
            searchButton.Click += SearchButton_Click;
            this.Controls.Add(searchButton);

            resultsListBox = new ListBox();
            resultsListBox.Location = new Point(20, 180);
            resultsListBox.Size = new Size(540, 200);
            resultsListBox.DoubleClick += ResultsListBox_DoubleClick; // Add DoubleClick event
            this.Controls.Add(resultsListBox);
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                searchPathTextBox.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            resultsListBox.Items.Clear();

            string directoryPath = searchPathTextBox.Text.Trim(); // Use the selected path

            if (!Directory.Exists(directoryPath))
            {
                MessageBox.Show("Directory does not exist.");
                return;
            }

            List<FileInfo> files = new List<FileInfo>();
            try
            {
                files.AddRange(GetFiles(directoryPath, new[] { ".jpg", ".jpeg", ".png" }));
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show($"Access to some directories was denied: {ex.Message}");
            }

            if (!string.IsNullOrEmpty(sizeTextBox.Text) && long.TryParse(sizeTextBox.Text, out long sizeInKB))
            {
                files = files.Where(f => f.Length <= sizeInKB * 1024).ToList();
            }

            DateTime selectedDate = dateModifiedPicker.Value.Date;
            files = files.Where(f => f.LastWriteTime.Date == selectedDate).ToList();

            foreach (FileInfo file in files)
            {
                resultsListBox.Items.Add($"{file.Name} - {file.Length / 1024} KB - {file.LastWriteTime}");
            }

            if (resultsListBox.Items.Count == 0)
            {
                MessageBox.Show("No matching images found.");
            }
        }

        private IEnumerable<FileInfo> GetFiles(string path, string[] extensions)
        {
            List<FileInfo> files = new List<FileInfo>();
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                files.AddRange(directoryInfo.GetFiles().Where(f => extensions.Contains(f.Extension.ToLower())));

                foreach (var directory in directoryInfo.GetDirectories())
                {
                    files.AddRange(GetFiles(directory.FullName, extensions));
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Ignore directories that cannot be accessed
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }

            return files;
        }

        private void ResultsListBox_DoubleClick(object sender, EventArgs e)
        {
            if (resultsListBox.SelectedItem != null)
            {
                string selectedItem = resultsListBox.SelectedItem.ToString();
                string fileName = selectedItem.Split('-')[0].Trim();
                string directoryPath = searchPathTextBox.Text.Trim(); // Use the selected path
                string filePath = Path.Combine(directoryPath, fileName);
                if (File.Exists(filePath))
                {
                    System.Diagnostics.Process.Start(filePath);
                }
                else
                {
                    MessageBox.Show("File not found.");
                }
            }
        }
    }
}
