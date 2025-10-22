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
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        private ListBox listBox;
        private Label infoLabel;
        private Button okButton;
        private Button controlButton;
        private Button refreshButton;
        private System.Windows.Forms.Timer refreshTimer;

        private WindowHandler handler;

        public NameGetter1()
        {
            this.Text = "Open Windows List";
            this.Width = 400;
            this.Height = 550;

            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 5
            };

            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 65));
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 10));
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 10));
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 10));
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 5));

            listBox = new ListBox { Dock = DockStyle.Fill };
            infoLabel = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Italic)
            };
            okButton = new Button { Text = "OK", Dock = DockStyle.Fill };
            controlButton = new Button { Text = "Window Control", Dock = DockStyle.Fill };
            refreshButton = new Button { Text = "Refresh (Manual)", Dock = DockStyle.Fill };

            panel.Controls.Add(listBox, 0, 0);
            panel.Controls.Add(infoLabel, 0, 1);
            panel.Controls.Add(okButton, 0, 2);
            panel.Controls.Add(controlButton, 0, 3);
            panel.Controls.Add(refreshButton, 0, 4);

            this.Controls.Add(panel);

            handler = new WindowHandler(UpdateLabel);

            refreshTimer = new System.Windows.Forms.Timer();
            refreshTimer.Interval = 3000;
            refreshTimer.Tick += (s, e) => LoadWindowList();
            refreshTimer.Start();

            this.Load += (s, e) => LoadWindowList();
            listBox.SelectedIndexChanged += ListBox_SelectedIndexChanged;
            okButton.Click += OkButton_Click;
            controlButton.Click += ControlButton_Click;
            refreshButton.Click += (s, e) => LoadWindowList();
        }

        private void LoadWindowList()
        {
            string? previouslySelected = listBox.SelectedItem as string;
            listBox.Items.Clear();

            EnumWindows((hWnd, lParam) =>
            {
                if (IsWindowVisible(hWnd))
                {
                    int length = GetWindowTextLength(hWnd);
                    if (length > 0)
                    {
                        StringBuilder builder = new StringBuilder(length + 1);
                        GetWindowText(hWnd, builder, builder.Capacity);
                        string windowTitle = builder.ToString();

                        if (!string.IsNullOrEmpty(windowTitle) && windowTitle != this.Text)
                            listBox.Items.Add(windowTitle);
                    }
                }
                return true;
            }, IntPtr.Zero);

            if (previouslySelected != null && listBox.Items.Contains(previouslySelected))
                listBox.SelectedItem = previouslySelected;
        }

        private void ListBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (listBox.SelectedItem is string selectedTitle)
                handler.OnWindowSelected(selectedTitle);
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

        private void UpdateLabel(string message)
        {
            infoLabel.Text = message;
        }
    }
}
