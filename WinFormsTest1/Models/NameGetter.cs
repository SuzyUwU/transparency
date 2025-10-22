using System;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Variable;
using Models;
using controller;

namespace Models
{
    public class NameGetter1 : Form
    {
        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        private ListBox listBox;
        private Label infoLabel;
        private Button okButton;
        private WindowHandler handler;
        private Button controlButton;

        public NameGetter1()
        {
            this.Text = "Open Windows List";
            this.Width = 400;
            this.Height = 550;

            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 3
            };
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 80));
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 10));
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 10));

            listBox = new ListBox { Dock = DockStyle.Fill };
            infoLabel = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Italic)
            };
            okButton = new Button
            {
                Text = "OK",
                Dock = DockStyle.Fill
            };
            controlButton = new Button
            {
                Text = "Window Control",
                Dock = DockStyle.Fill
            };
            panel.RowCount = 4;
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 10));
            panel.Controls.Add(controlButton, 0, 3);

            controlButton.Click += ControlButton_Click;


            panel.Controls.Add(listBox, 0, 0);
            panel.Controls.Add(infoLabel, 0, 1);
            panel.Controls.Add(okButton, 0, 2);
            this.Controls.Add(panel);

            handler = new WindowHandler(UpdateLabel);

            this.Load += (s, e) => RefreshWindowList();
            listBox.SelectedIndexChanged += ListBox_SelectedIndexChanged;
            okButton.Click += OkButton_Click;
        }

        private void RefreshWindowList()
        {
            listBox.Items.Clear();

            EnumWindows((hWnd, lParam) =>
            {
                if (IsWindowVisible(hWnd))
                {
                    StringBuilder buffer = new StringBuilder(256);
                    GetWindowText(hWnd, buffer, buffer.Capacity);
                    string title = buffer.ToString();

                    if (!string.IsNullOrWhiteSpace(title))
                        listBox.Items.Add(title);
                }
                return true;
            }, IntPtr.Zero);
        }

        private void ListBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (listBox.SelectedItem is string selectedTitle)
            {
                handler.OnWindowSelected(selectedTitle);
            }
        }

        private void OkButton_Click(object? sender, EventArgs e)
        {
            if (listBox.SelectedItem is string selectedTitle)
            {
                AppData.SelectedWindow = selectedTitle;
                MessageBox.Show($"Saved: {selectedTitle}", "Info");
            }
            else
            {
                MessageBox.Show("Please select a window first.", "Warning");
            }
        }

        private void UpdateLabel(string message)
        {
            infoLabel.Text = message;
        }
        private void ControlButton_Click(object? sender, EventArgs e)
        {
            if (listBox.SelectedItem is string selectedTitle)
            {
                var controlForm = new WindowControlForm(selectedTitle);
                controlForm.Show();
            }
            else
            {
                MessageBox.Show("Please select a window first.", "Warning");
            }
        }
    }
}
