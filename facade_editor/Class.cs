using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using facade_editor.Properties;

namespace facade_editor
{
    public partial class Form1 : Form
    {
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
        private Timer timer1;
        int click =0;
        private void tabPage4_Click(object sender, EventArgs e)
        {
            click++;
            if (click == 5)
            {
                tabPage4.Controls.Add(superSecretPictureBox);
                superSecretPictureBox.BackColor = Color.Transparent;
                superSecretPictureBox.Location = new Point(185, 95);
                superSecretPictureBox.Name = "superSecretPictureBox";
                superSecretPictureBox.Size = new Size(200, 210);
                superSecretPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                superSecretPictureBox.TabIndex = 0;
                superSecretPictureBox.TabStop = false;
                superSecretPictureBox.Show();
                timer1 = new Timer(components);
                timer1.Tick += new EventHandler(timer1_Tick);
                timer1.Interval = 20;
                timer1.Enabled = true;
                creditLabel.Text = "Special thanks to the Façade developers,\r\nand of course, thanks for this funny Swedish man\r\nfor inspiring me with his Façade corruption streams to make this ";
            }
            
            if(click == 6)
            {
                superSecretPictureBox.Hide();
                timer1.Enabled = false;
                timer1.Dispose();
                click = 0;
                creditLabel.Text = "Special thanks to the Façade developers";
            }
        }

        Bitmap RotateImage(Bitmap rotateMe, float angle)
        {

            var bmp = new Bitmap(rotateMe.Width + (rotateMe.Width / 2), rotateMe.Height + (rotateMe.Height / 2));

            using (Graphics g = Graphics.FromImage(bmp))
                g.DrawImageUnscaled(rotateMe, (rotateMe.Width / 4), (rotateMe.Height / 4), bmp.Width, bmp.Height);
            rotateMe = bmp;

            Bitmap rotatedImage = new Bitmap(rotateMe.Width, rotateMe.Height);

            using (Graphics g = Graphics.FromImage(rotatedImage))
            {
                g.TranslateTransform(rotateMe.Width / 2, rotateMe.Height / 2);  
                g.RotateTransform(angle);                                        
                g.TranslateTransform(-rotateMe.Width / 2, -rotateMe.Height / 2); 
                g.DrawImage(rotateMe, new Point(0, 0));                          
            }
            bmp.Dispose();
            rotateMe.Dispose();
            return rotatedImage;

        }

        int rotate = 0;
        string directionLR = "right";
        string directionUD = "up";
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (rotate == 360)
                rotate = 0;
            superSecretPictureBox.Image = RotateImage(Resources.VargFren, rotate);
            rotate+=5;
            if (directionLR == "right")
                superSecretPictureBox.Location = new Point(superSecretPictureBox.Location.X+5, superSecretPictureBox.Location.Y);
            if (directionLR == "left")
                superSecretPictureBox.Location = new Point(superSecretPictureBox.Location.X - 5, superSecretPictureBox.Location.Y);
            if(directionUD == "up")
                superSecretPictureBox.Location = new Point(superSecretPictureBox.Location.X, superSecretPictureBox.Location.Y+5);
            if (directionUD == "down")
                superSecretPictureBox.Location = new Point(superSecretPictureBox.Location.X, superSecretPictureBox.Location.Y - 5);
            if (superSecretPictureBox.Location.X+(superSecretPictureBox.Size.Width/4) <= 0)
                directionLR = "right";
            if (superSecretPictureBox.Size.Width + superSecretPictureBox.Location.X >= tabControl1.Size.Width)
                directionLR = "left";

            if (superSecretPictureBox.Location.Y + (superSecretPictureBox.Size.Height / 4) <= 0)
                directionUD = "up";
            if (superSecretPictureBox.Size.Height + superSecretPictureBox.Location.Y >=tabControl1.Size.Height)
                directionUD = "down";
            //superSecretPictureBox.Size = new Size(superSecretPictureBox.Size.Width+1, superSecretPictureBox.Size.Height);
        }
        

    }
}
