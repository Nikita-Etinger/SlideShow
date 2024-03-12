using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


//делал с закосом под видео формат просто по кадрам 
//потому счетчик в виде времени, а не кадров 
namespace slideshow
{
    public partial class Q1 : Form
    {
        List<Bitmap> images =new List<Bitmap>();
        Timer timer = new Timer();
        int indexPic = 0;
        public Q1()
        {
            InitializeComponent();
            timer.Interval = 40;
            timer.Tick += next_Click;
            trackBar1.MouseDown += trackBar1_MouseDownHandler;
            numericUpDown1.Value = 25;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = false;

        }

        private void trackBar1_MouseDownHandler(object sender, MouseEventArgs e)
        {
            if (timer.Enabled)
            {
                timer.Stop();
            }
        }

        private void next_Click(object sender, EventArgs e)
        {
            nextPicture();
        }

        private void nextPicture()
        {
            if (images.Count == 0)
            {
                return;
            }
            else
            {
                indexPic++;
                if (indexPic >= images.Count)
                {
                    indexPic = 0;
                }
                trackBar1.Value = indexPic;
                updateTime();

            }
        }

        private void updateTime()
        {
            {
                double framesPerSecond = (double)numericUpDown1.Value; 
                double totalSeconds = images.Count / framesPerSecond; // Общее время в секундах

                double currentSeconds = indexPic / framesPerSecond; // Текущее время в секундах


                TimeSpan totalTimeSpan = TimeSpan.FromSeconds(totalSeconds);
                TimeSpan currentTimeSpan = TimeSpan.FromSeconds(currentSeconds);

                label2.Text = currentTimeSpan.ToString(@"hh\:mm\:ss"); // Текущее время
                label3.Text = totalTimeSpan.ToString(@"hh\:mm\:ss"); // Всего времени

            }
        }
        private async void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            folder.SelectedPath = Environment.CurrentDirectory; 
            if (folder.ShowDialog() == DialogResult.OK)
            {
                if (images.Any())
                {
                    images.ForEach(img => img.Dispose()); 
                    images.Clear();
                }

                if (!Directory.Exists(folder.SelectedPath))
                {
                    MessageBox.Show("Выбранной папки не существует");
                    return;
                }

                
                string[] imageExtensions = { ".bmp", ".jpeg", ".jpg", ".png" };

                await Task.Run(() =>
                {
                    DirectoryInfo directory = new DirectoryInfo(folder.SelectedPath);
                    IEnumerable<FileInfo> files = directory.EnumerateFiles();

                    foreach (var file in files)
                    {
                        string extension = Path.GetExtension(file.FullName).ToLowerInvariant();
                        if (imageExtensions.Contains(extension))
                        {
                            using (Bitmap originalImage = new Bitmap(file.FullName))
                            {
                                Bitmap resizedImage = new Bitmap(originalImage, pictureBox1.Size);
                                images.Add(resizedImage);
                            }
                        }
                    }
                });

                if (images.Count == 0)
                {
                    MessageBox.Show("Картинок нет");
                }
                else
                {
                    trackBar1.Maximum = images.Count-1;
                    MessageBox.Show("OK");
                    button2.Enabled = true;
                    button4.Enabled = true;
                    button5.Enabled = true;
                }
            }
        }



        private void button2_Click(object sender, EventArgs e)
        {
            if (images.Count == 0)
            {
                MessageBox.Show("Картинок нет");

            }
            else timer.Start();
            button3.Enabled = true;
            button2.Enabled = false;
            button4.Enabled = false; 
            button5.Enabled = false; 
        }

        private void button3_Click(object sender, EventArgs e)
        {
            timer.Stop();
            button3.Enabled = false;
            button2.Enabled = true;
            button4.Enabled = true;
            button5.Enabled = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (images.Count == 0)
            {
                return;
            }
            else
            {

                indexPic++;
                
                if (indexPic >= images.Count)
                {
                    indexPic = 0;
                    trackBar1.Value = 0;
                }
                pictureBox1.Image = images[indexPic];
                trackBar1.Value++;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (images.Count == 0)
            {
                return;
            }
            else
            {
                indexPic--;
                
                if (indexPic < 0)
                {
                    indexPic = images.Count-1;
                    trackBar1.Value =trackBar1.Maximum;
                }
                pictureBox1.Image = images[indexPic];
                trackBar1.Value--;
            }
        }

        private void trackBar1_MouseCaptureChanged(object sender, EventArgs e)
        {
            
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            indexPic = trackBar1.Value;
            pictureBox1.Image = images[indexPic];
            updateTime();
        }

        private void trackBar1_KeyDown(object sender, KeyEventArgs e)
        {
            
        }
        private void trackBar1_MouseDown(object sender, MouseEventArgs e)
        {
            timer.Stop();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            int x = (int)1000/ (int)numericUpDown1.Value;
            timer.Interval= x;
            updateTime();
        }
    }
}
