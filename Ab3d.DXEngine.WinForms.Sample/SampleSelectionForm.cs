using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows.Forms;

namespace Ab3d.DXEngine.WinForms.Sample
{
    public partial class SampleSelectionForm : Form
    {
        private Form _shownForm;

        public SampleSelectionForm()
        {
            InitializeComponent();

            AddInfoIcon(button1, "This sample shows how to use Ab3d.DXEngine with a ElementHost control that provides WPF interop to WinForms.\r\nWith ElementHost it is possible to render WPF controls on top of 3D scene and use the same samples as for the WPF samples project (except the part that is defines in XAML is here defined in code).\r\n\r\nFor more info see comments in DXViewportViewElementHostSample.cs");

            AddInfoIcon(button3, "This sample shows how to use Ab3d.DXEngine with a DXViewportView control and a RenderForm.\r\nUsing RenderForm is the simplest usage of the Ab3d.DXEngine in WinForms. There the 3D scene is rendered to the whole content of the Form. Note that no other WPF or WinForms object can be rendered on top of the 3D scene.\r\n\r\nFor more info see comments in DXViewportViewRenderFormSample.cs");
            AddInfoIcon(button2, "This sample shows how to use Ab3d.DXEngine with a DXViewportView control and a RenderControl.\r\nUsing RenderControl instead RenderForm allows showing 3D scene only on part of the Form.\r\n\r\nFor more info see comments in DXViewportViewRenderControlSample.cs");
            AddInfoIcon(button4, "This samples shows an advanced usage of Ab3d.DXEngine that is used without DXViewportView control\r\nbut instead a lower level DXScene objects is created to render the DirectX content to the RenderForm.\r\n\r\nFor more info see comments in RenderFormSample.cs");
            AddInfoIcon(button5, "This samples shows an advanced usage of Ab3d.DXEngine that is used without DXViewportView control\r\nbut instead a lower level DXScene objects is created to render the DirectX content to the PictureBox.\r\n\r\nFor more info see comments in PictureBoxSample.cs");


            var sampleSolutionPath = System.IO.Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                @"..\..\..\Ab3d.DXEngine MAIN SAMPLES.sln");

            if (System.IO.File.Exists(sampleSolutionPath))
            {
                var linkLabel = new LinkLabel()
                {
                    Text = "Click here to open the WPF samples solution.",
                    Width = label1.Width,
                    Font = label1.Font
                };

                linkLabel.Location = new Point(label1.Location.X, label1.Location.Y + label1.Height + 10);

                linkLabel.Click += delegate(object sender, EventArgs args)
                {
                    try
                    {
                        // For CORE3 project we need to set UseShellExecute to true,
                        // otherwise a "The specified executable is not a valid application for this OS platform" exception is thrown.
                        System.Diagnostics.Process.Start(new ProcessStartInfo(sampleSolutionPath) { UseShellExecute = true });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error opening samples solution:\r\n" + ex.Message);
                    }
                };

                this.Controls.Add(linkLabel);

                this.Height += linkLabel.Height;
            }


            this.Closing += delegate(object sender, CancelEventArgs args)
            {
                CloseShownForm();
            };
        }


        private void button1_Click(object sender, EventArgs e)
        {
            CloseShownForm();

            _shownForm      = new DXViewportViewElementHostSample();
            _shownForm.Size = new Size(1200, 700);
            _shownForm.Closing += delegate (object o, CancelEventArgs args)
            {
                button1.Enabled = true;
            };

            _shownForm.Show();

            button1.Enabled = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            Application.DoEvents();

            using (var game = new DXViewportViewRenderFormSample())
            {
                game.Run();
            }

            button2.Enabled = true;
        }


        private void button3_Click(object sender, EventArgs e)
        {
            CloseShownForm();

            _shownForm      = new DXViewportViewRenderControlSample();
            _shownForm.Size = new Size(1200, 700);
            _shownForm.Closing += delegate (object o, CancelEventArgs args)
            {
                button3.Enabled = true;
            };

            _shownForm.Show();

            button3.Enabled = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            CloseShownForm();

            button4.Enabled = false;
            Application.DoEvents();

            using (var game = new RenderFormSample())
            {
                game.Run();
            }

            button4.Enabled = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            CloseShownForm();

            _shownForm      = new PictureBoxSample();
            _shownForm.Size = new Size(1200, 700);
            _shownForm.Closing += delegate (object o, CancelEventArgs args)
            {
                button5.Enabled = true;
            };

            _shownForm.Show();

            button5.Enabled = false;
        }


        private void CloseShownForm()
        {
            if (_shownForm == null)
                return;

            _shownForm.Close();
            _shownForm = null;
        }

        private void AddInfoIcon(Control attachedControl, string infoText)
        {
            var pictureBox = new PictureBox();
            pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox.Width = 24;
            pictureBox.Height = 24;
            pictureBox.Location = new Point(attachedControl.Location.X + attachedControl.Width - pictureBox.Width, attachedControl.Location.Y + (attachedControl.Height - pictureBox.Height) / 2);
            pictureBox.Image = SystemIcons.Information.ToBitmap();

            attachedControl.Width -= 24 + 3;


            ToolTip toolTip1 = new ToolTip();

            toolTip1.AutoPopDelay = 60000;
            toolTip1.InitialDelay = 500;
            toolTip1.ReshowDelay  = 500;
            toolTip1.ShowAlways = true;

            toolTip1.SetToolTip(attachedControl, infoText);
            toolTip1.SetToolTip(pictureBox, infoText);

            this.Controls.Add(pictureBox);
        }
    }
}
