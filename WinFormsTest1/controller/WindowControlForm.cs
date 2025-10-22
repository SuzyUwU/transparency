using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace controller
{
    public class WindowControlForm : Form
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string? lpClassName, string lpWindowName);

        private Label titleLabel;
        private CheckBox pinCheckBox;
        private CheckBox clickThroughCheckBox;
        private TrackBar transparencySlider;
        private Label transparencyLabel;
        private Button applyButton;

        private IntPtr targetHandle;

        public WindowControlForm(string windowName)
        {
            this.Text = $"Control: {windowName}";
            this.Width = 350;
            this.Height = 250;
            this.TopMost = true;

            targetHandle = FindWindow(null, windowName);

            titleLabel = new Label
            {
                Text = $"Window: {windowName}",
                Dock = DockStyle.Top,
                Height = 40,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            pinCheckBox = new CheckBox
            {
                Text = "Always on Top (Pin)",
                Dock = DockStyle.Top, 
                AutoSize = false,
                Height = 30
            };

            clickThroughCheckBox = new CheckBox
            {
                Text = "Click-Through",
                Dock = DockStyle.Top,
                AutoSize = false,
                Height = 30
            };

            transparencyLabel = new Label
            {
                Text = "Transparency: 0%",
                Dock = DockStyle.Top,
                Height = 25,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                AutoSize = false
            };

            transparencySlider = new TrackBar
            {
                Minimum = 0,
                Maximum = 100,
                Value = 0,
                TickFrequency = 10,
                Dock = DockStyle.Top,
                Height = 50
            };

            applyButton = new Button
            {
                Text = "Apply",
                Dock = DockStyle.Bottom,
                Height = 40
            };

            transparencySlider.Scroll += (s, e) =>
            {
                transparencyLabel.Text = $"Transparency: {transparencySlider.Value}%";
            };

            applyButton.Click += ApplyButton_Click;

            this.Controls.Add(applyButton);
            this.Controls.Add(transparencySlider);
            this.Controls.Add(transparencyLabel);
            this.Controls.Add(clickThroughCheckBox);
            this.Controls.Add(pinCheckBox);
            this.Controls.Add(titleLabel);
        }

        private void ApplyButton_Click(object? sender, EventArgs e)
        {
            if (targetHandle == IntPtr.Zero)
            {
                MessageBox.Show("Window not found.", "Error");
                return;
            }
            byte alpha = (byte)(255 - (transparencySlider.Value * 255 / 100));

            WindowUtility.SetAlwaysOnTop(targetHandle, pinCheckBox.Checked);
            WindowUtility.SetClickThrough(targetHandle, clickThroughCheckBox.Checked);
            WindowUtility.SetTransparency(targetHandle, alpha);

            MessageBox.Show($"Settings applied! Transparency set to {transparencySlider.Value}%", "Success");
        }
    }
}
