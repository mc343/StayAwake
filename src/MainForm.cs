using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace StayAwake
{
    public class MainForm : Form
    {
        // Windows API imports
        [DllImport("kernel32.dll")]
        private static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

        [DllImport("user32.dll")]
        private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        private const uint MOUSEEVENTF_MOVE = 0x0001;
        private const uint KEYEVENTF_KEYUP = 0x0002;
        private const byte VK_SHIFT = 0x10;

        [Flags]
        private enum EXECUTION_STATE : uint
        {
            ES_AWAYMODE_REQUIRED = 0x00000040,
            ES_CONTINUOUS = 0x80000000,
            ES_DISPLAY_REQUIRED = 0x00000002,
            ES_SYSTEM_REQUIRED = 0x00000001
        }

        // Controls
        private Panel? headerPanel;
        private Label? titleLabel;
        private Panel? mainPanel;
        private GroupBox? durationGroup;
        private ComboBox? durationCombo;
        private NumericUpDown? customHours;
        private NumericUpDown? customMinutes;
        private Button? startStopBtn;
        private GroupBox? statusGroup;
        private Label? statusLabel;
        private Label? timeRemainingLabel;
        private Label? activityCountLabel;
        private ProgressBar? progress;
        private GroupBox? settingsGroup;
        private Label? sensitivityLabel;
        private TrackBar? sensitivityTrackBar;
        private CheckBox? minimizeToTrayCheck;
        private CheckBox? enableHotkeyCheck;
        private GroupBox? scheduleGroup;
        private CheckBox? enableScheduleCheck;
        private DateTimePicker? scheduleStart;
        private DateTimePicker? scheduleEnd;
        private GroupBox? logGroup;
        private ListBox? activityLog;
        private NotifyIcon? trayIcon;
        private ContextMenuStrip? trayMenu;

        // State
        private bool isRunning = false;
        private DateTime? startTime;
        private DateTime? endTime;
        private DateTime? scheduleStartTime;
        private DateTime? scheduleEndTime;
        private System.Windows.Forms.Timer? activityTimer;
        private System.Windows.Forms.Timer? uiUpdateTimer;
        private readonly Random random = new Random();
        private int activityCount = 0;
        private List<string> logEntries = new List<string>();

        // Sensitivity settings (pixels to move, interval in ms)
        private readonly (int pixels, int intervalMs, bool useKeypress)[] sensitivityLevels = new (int, int, bool)[]
        {
            (1, 45000, false),   // Subtle - 1px, 45s, no keypress
            (2, 30000, false),   // Medium - 2px, 30s, no keypress
            (5, 20000, true),    // Noticeable - 5px, 20s, with keypress
        };

        public MainForm()
        {
            InitializeComponents();
            InitializeTrayIcon();
            RegisterHotKey();
        }

        private void InitializeComponents()
        {
            // Form properties
            this.Text = "StayAwake - Keep Your System Awake";
            this.Size = new Size(550, 720);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(240, 240, 245);
            this.Font = new Font("Segoe UI", 9F);

            // Header Panel
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(65, 105, 225)  // Royal Blue
            };

            titleLabel = new Label
            {
                Text = "⏰ StayAwake",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 15)
            };

            headerPanel.Controls.Add(titleLabel);

            // Main Panel with scroll
            mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(15),
                AutoScroll = true
            };

            // Duration Selection Group
            durationGroup = CreateGroupBox("⏱ Duration", 10, 10, 500, 120);

            var durationPresetLabel = new Label
            {
                Text = "Preset:",
                Location = new Point(15, 30),
                Size = new Size(60, 20)
            };

            durationCombo = new ComboBox
            {
                Location = new Point(80, 28),
                Size = new Size(150, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            durationCombo.Items.AddRange(new object[] { "30 minutes", "1 hour", "2 hours", "4 hours", "8 hours", "Unlimited", "Custom" });
            durationCombo.SelectedIndex = 1; // Default to 1 hour
            durationCombo.SelectedIndexChanged += DurationCombo_SelectedIndexChanged;

            var customLabel = new Label
            {
                Text = "Custom:",
                Location = new Point(15, 70),
                Size = new Size(60, 20),
                Enabled = false
            };

            customHours = new NumericUpDown
            {
                Location = new Point(80, 68),
                Size = new Size(60, 25),
                Minimum = 0,
                Maximum = 23,
                Value = 1,
                Enabled = false
            };

            var hoursLabel = new Label { Text = "hrs", Location = new Point(145, 70), Size = new Size(30, 20) };

            customMinutes = new NumericUpDown
            {
                Location = new Point(180, 68),
                Size = new Size(60, 25),
                Minimum = 0,
                Maximum = 59,
                Value = 0,
                Enabled = false
            };

            var minutesLabel = new Label { Text = "min", Location = new Point(245, 70), Size = new Size(30, 20) };

            durationGroup.Controls.AddRange(new Control[] { durationPresetLabel, durationCombo, customLabel, customHours, hoursLabel, customMinutes, minutesLabel });

            // Start/Stop Button
            startStopBtn = new Button
            {
                Text = "▶ START",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Size = new Size(500, 50),
                Location = new Point(10, 140),
                BackColor = Color.FromArgb(34, 197, 94),  // Green
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            startStopBtn.FlatAppearance.BorderSize = 0;
            startStopBtn.Click += StartStop_Click;

            // Status Group
            statusGroup = CreateGroupBox("📊 Status", 10, 200, 500, 150);

            statusLabel = new Label
            {
                Text = "● Stopped",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(156, 163, 175),  // Gray
                Location = new Point(15, 25),
                Size = new Size(200, 25)
            };

            timeRemainingLabel = new Label
            {
                Text = "Time Remaining: --:--:--",
                Location = new Point(15, 55),
                Size = new Size(300, 20)
            };

            activityCountLabel = new Label
            {
                Text = "Activities Performed: 0",
                Location = new Point(15, 80),
                Size = new Size(300, 20)
            };

            progress = new ProgressBar
            {
                Location = new Point(15, 105),
                Size = new Size(470, 20),
                Style = ProgressBarStyle.Continuous,
                Value = 0
            };

            statusGroup.Controls.AddRange(new Control[] { statusLabel, timeRemainingLabel, activityCountLabel, progress });

            // Settings Group
            settingsGroup = CreateGroupBox("⚙ Settings", 10, 360, 500, 140);

            sensitivityLabel = new Label
            {
                Text = "Activity Level: Subtle",
                Location = new Point(15, 25),
                Size = new Size(200, 20)
            };

            sensitivityTrackBar = new TrackBar
            {
                Location = new Point(15, 45),
                Size = new Size(300, 45),
                Minimum = 0,
                Maximum = 2,
                Value = 0,
                TickStyle = TickStyle.Both
            };
            sensitivityTrackBar.ValueChanged += SensitivityTrackBar_ValueChanged;

            var sensitivityDescLabel = new Label
            {
                Text = "Subtle ← → Medium ← → Noticeable",
                Location = new Point(15, 85),
                Size = new Size(300, 20),
                Font = new Font("Segoe UI", 7F)
            };

            minimizeToTrayCheck = new CheckBox
            {
                Text = "Minimize to system tray when running",
                Location = new Point(330, 25),
                Size = new Size(160, 25),
                Checked = true
            };

            enableHotkeyCheck = new CheckBox
            {
                Text = "Enable Hotkey (Ctrl+Shift+A)",
                Location = new Point(330, 50),
                Size = new Size(160, 25),
                Checked = true
            };

            settingsGroup.Controls.AddRange(new Control[] { sensitivityLabel, sensitivityTrackBar, sensitivityDescLabel, minimizeToTrayCheck, enableHotkeyCheck });

            // Schedule Group
            scheduleGroup = CreateGroupBox("📅 Schedule (Optional)", 10, 510, 500, 100);

            enableScheduleCheck = new CheckBox
            {
                Text = "Enable automatic schedule",
                Location = new Point(15, 25),
                Size = new Size(200, 25),
                Checked = false
            };
            enableScheduleCheck.CheckedChanged += EnableScheduleCheck_CheckedChanged;

            var scheduleLabel = new Label
            {
                Text = "From:",
                Location = new Point(15, 55),
                Size = new Size(40, 20),
                Enabled = false
            };

            scheduleStart = new DateTimePicker
            {
                Location = new Point(55, 53),
                Size = new Size(100, 25),
                Format = DateTimePickerFormat.Time,
                ShowUpDown = true,
                Value = DateTime.Today.AddHours(9),  // 9 AM
                Enabled = false
            };

            var toLabel = new Label { Text = "To:", Location = new Point(165, 55), Size = new Size(30, 20), Enabled = false };

            scheduleEnd = new DateTimePicker
            {
                Location = new Point(200, 53),
                Size = new Size(100, 25),
                Format = DateTimePickerFormat.Time,
                ShowUpDown = true,
                Value = DateTime.Today.AddHours(17),  // 5 PM
                Enabled = false
            };

            scheduleGroup.Controls.AddRange(new Control[] { enableScheduleCheck, scheduleLabel, scheduleStart, toLabel, scheduleEnd });

            // Log Group
            logGroup = CreateGroupBox("📝 Activity Log", 10, 620, 500, 120);

            activityLog = new ListBox
            {
                Location = new Point(15, 25),
                Size = new Size(470, 80),
                Font = new Font("Consolas", 8F),
                ScrollAlwaysVisible = true,
                HorizontalScrollbar = true
            };

            logGroup.Controls.Add(activityLog);

            // Add all to main panel
            mainPanel.Controls.AddRange(new Control[] { durationGroup, startStopBtn, statusGroup, settingsGroup, scheduleGroup, logGroup });

            // Add panels to form
            this.Controls.Add(mainPanel);
            this.Controls.Add(headerPanel);

            // Timers
            activityTimer = new System.Windows.Forms.Timer();
            activityTimer.Tick += PerformActivity;

            uiUpdateTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            uiUpdateTimer.Tick += UpdateUI;
            uiUpdateTimer.Start();

            // Form events
            this.Resize += MainForm_Resize;
            this.FormClosing += MainForm_FormClosing;
        }

        private GroupBox CreateGroupBox(string text, int x, int y, int width, int height)
        {
            return new GroupBox
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(width, height),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(55, 65, 81)
            };
        }

        private void InitializeTrayIcon()
        {
            trayMenu = new ContextMenuStrip();
            var showItem = new ToolStripMenuItem("Show", null, ShowWindow);
            var toggleItem = new ToolStripMenuItem("Start/Stop", null, (s, e) => StartStop_Click(s, e));
            var exitItem = new ToolStripMenuItem("Exit", null, ExitApplication);
            trayMenu.Items.AddRange(new ToolStripItem[] { showItem, toggleItem, exitItem });

            trayIcon = new NotifyIcon
            {
                Icon = SystemIcons.Application,
                Text = "StayAwake - Stopped",
                Visible = true,
                ContextMenuStrip = trayMenu
            };
            trayIcon.DoubleClick += (s, e) => ShowWindow(s, e);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.Shift | Keys.A) && enableHotkeyCheck.Checked)
            {
                StartStop_Click(null, null);
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void RegisterHotKey()
        {
            // HotKey is handled via ProcessCmdKey for simplicity
        }

        private void DurationCombo_SelectedIndexChanged(object? sender, EventArgs e)
        {
            bool isCustom = durationCombo?.SelectedItem?.ToString() == "Custom";

            customHours!.Enabled = isCustom;
            customMinutes!.Enabled = isCustom;

            // Reset numeric controls when switching presets
            if (!isCustom)
            {
                customHours.Value = 1;
                customMinutes.Value = 0;
            }
        }

        private void SensitivityTrackBar_ValueChanged(object? sender, EventArgs e)
        {
            var levels = new[] { "Subtle", "Medium", "Noticeable" };
            sensitivityLabel!.Text = $"Activity Level: {levels[sensitivityTrackBar!.Value]}";
        }

        private void EnableScheduleCheck_CheckedChanged(object? sender, EventArgs e)
        {
            bool enabled = enableScheduleCheck.Checked;

            scheduleStart!.Enabled = enabled;
            scheduleEnd!.Enabled = enabled;

            if (enabled)
            {
                AddLog($"Schedule set: {scheduleStart.Value:HH:mm} to {scheduleEnd.Value:HH:mm}");
            }
        }

        private void StartStop_Click(object? sender, EventArgs? e)
        {
            if (isRunning)
            {
                Stop();
            }
            else
            {
                Start();
            }
        }

        private void Start()
        {
            // Calculate end time
            string selectedDuration = durationCombo!.SelectedItem!.ToString()!;

            if (selectedDuration == "Unlimited")
            {
                endTime = null;
            }
            else if (selectedDuration == "Custom")
            {
                endTime = DateTime.Now.AddHours((double)customHours!.Value).AddMinutes((double)customMinutes!.Value);
            }
            else
            {
                // Parse preset
                var durationMap = new Dictionary<string, TimeSpan>
                {
                    ["30 minutes"] = TimeSpan.FromMinutes(30),
                    ["1 hour"] = TimeSpan.FromHours(1),
                    ["2 hours"] = TimeSpan.FromHours(2),
                    ["4 hours"] = TimeSpan.FromHours(4),
                    ["8 hours"] = TimeSpan.FromHours(8)
                };
                endTime = DateTime.Now + durationMap[selectedDuration];
            }

            isRunning = true;
            startTime = DateTime.Now;
            activityCount = 0;

            // Set activity prevention
            SetThreadExecutionState(
                EXECUTION_STATE.ES_CONTINUOUS |
                EXECUTION_STATE.ES_SYSTEM_REQUIRED |
                EXECUTION_STATE.ES_DISPLAY_REQUIRED |
                EXECUTION_STATE.ES_AWAYMODE_REQUIRED
            );

            // Start activity timer
            var (pixels, intervalMs, _) = sensitivityLevels[sensitivityTrackBar!.Value];
            activityTimer!.Interval = intervalMs + random.Next(5000);
            activityTimer.Start();

            // Update UI
            UpdateUIState(true);
            AddLog($"Started at {DateTime.Now:HH:mm:ss}");

            if (minimizeToTrayCheck.Checked)
            {
                this.WindowState = FormWindowState.Minimized;
            }
        }

        private void Stop()
        {
            isRunning = false;
            startTime = null;

            // Restore normal sleep behavior
            SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);

            // Stop timers
            activityTimer!.Stop();

            // Update UI
            UpdateUIState(false);
            AddLog($"Stopped at {DateTime.Now:HH:mm:ss}");
        }

        private void PerformActivity(object? sender, EventArgs e)
        {
            if (!isRunning) return;

            var (pixels, intervalMs, useKeypress) = sensitivityLevels[sensitivityTrackBar!.Value];

            // Move mouse randomly
            int deltaX = random.Next(-pixels, pixels + 1);
            int deltaY = random.Next(-pixels, pixels + 1);
            mouse_event(MOUSEEVENTF_MOVE, (uint)Math.Abs(deltaX), (uint)Math.Abs(deltaY), 0, 0);

            // Optionally press shift key
            if (useKeypress && random.Next(2) == 0)
            {
                keybd_event(VK_SHIFT, 0, 0, 0);  // Press
                Thread.Sleep(50);
                keybd_event(VK_SHIFT, 0, KEYEVENTF_KEYUP, 0);  // Release
            }

            activityCount++;

            // Randomize next interval
            activityTimer!.Interval = intervalMs + random.Next(10000);

            AddLog($"Activity #{activityCount} at {DateTime.Now:HH:mm:ss} (dX: {deltaX}, dY: {deltaY})");
        }

        private void UpdateUI(object? sender, EventArgs e)
        {
            // Update time remaining
            if (isRunning && endTime.HasValue)
            {
                var remaining = endTime.Value - DateTime.Now;
                if (remaining <= TimeSpan.Zero)
                {
                    Stop();
                    AddLog("Time elapsed - automatically stopped");
                    return;
                }
                timeRemainingLabel!.Text = $"Time Remaining: {remaining.Hours:D2}:{remaining.Minutes:D2}:{remaining.Seconds:D2}";

                // Update progress bar
                if (startTime.HasValue)
                {
                    var totalDuration = endTime.Value - startTime.Value;
                    var elapsed = DateTime.Now - startTime.Value;
                    var progressValue = (int)((elapsed.TotalMilliseconds / totalDuration.TotalMilliseconds) * 100);
                    progress!.Value = Math.Min(100, Math.Max(0, progressValue));
                }
            }
            else if (isRunning && !endTime.HasValue)
            {
                timeRemainingLabel!.Text = "Time Remaining: Unlimited";
                progress!.Value = 100;
            }
            else
            {
                timeRemainingLabel!.Text = "Time Remaining: --:--:--";
                progress!.Value = 0;
            }

            activityCountLabel!.Text = $"Activities Performed: {activityCount}";

            // Check schedule
            if (enableScheduleCheck.Checked && isRunning)
            {
                var now = DateTime.Now;
                var startTime = scheduleStart!.Value;
                var endTimeSchedule = scheduleEnd!.Value;

                var current = TimeSpan.FromHours(now.Hour) + TimeSpan.FromMinutes(now.Minute);
                var start = TimeSpan.FromHours(startTime.Hour) + TimeSpan.FromMinutes(startTime.Minute);
                var end = TimeSpan.FromHours(endTimeSchedule.Hour) + TimeSpan.FromMinutes(endTimeSchedule.Minute);

                if (current < start || current > end)
                {
                    if (isRunning)
                    {
                        Stop();
                        AddLog("Outside schedule hours - automatically stopped");
                    }
                }
            }
        }

        private void UpdateUIState(bool running)
        {
            if (running)
            {
                startStopBtn!.Text = "⏹ STOP";
                startStopBtn.BackColor = Color.FromArgb(239, 68, 68);  // Red
                statusLabel!.Text = "● Running";
                statusLabel.ForeColor = Color.FromArgb(34, 197, 94);  // Green
                trayIcon!.Text = "StayAwake - Running";
                durationCombo!.Enabled = false;
                customHours!.Enabled = false;
                customMinutes!.Enabled = false;
            }
            else
            {
                startStopBtn!.Text = "▶ START";
                startStopBtn.BackColor = Color.FromArgb(34, 197, 94);  // Green
                statusLabel!.Text = "● Stopped";
                statusLabel.ForeColor = Color.FromArgb(156, 163, 175);  // Gray
                trayIcon!.Text = "StayAwake - Stopped";
                durationCombo!.Enabled = true;
                DurationCombo_SelectedIndexChanged(null, null);
            }
        }

        private void AddLog(string message)
        {
            var entry = $"[{DateTime.Now:HH:mm:ss}] {message}";
            logEntries.Add(entry);

            if (activityLog!.InvokeRequired)
            {
                activityLog.Invoke(new Action(() => AddLogToUI(entry)));
            }
            else
            {
                AddLogToUI(entry);
            }
        }

        private void AddLogToUI(string entry)
        {
            activityLog!.Items.Insert(0, entry);
            if (activityLog.Items.Count > 50) // Keep last 50 entries
            {
                activityLog.Items.RemoveAt(activityLog.Items.Count - 1);
            }
        }

        private void ShowWindow(object? sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.Activate();
        }

        private void MainForm_Resize(object? sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized && minimizeToTrayCheck.Checked && isRunning)
            {
                this.Hide();
                trayIcon!.ShowBalloonTip(2000, "StayAwake", "Running in background. Double-click to open.", ToolTipIcon.Info);
            }
        }

        private void ExitApplication(object? sender, EventArgs e)
        {
            if (isRunning)
            {
                Stop();
            }
            trayIcon?.Dispose();
            Application.Exit();
        }

        private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (isRunning)
            {
                var result = MessageBox.Show(
                    "StayAwake is currently running. Do you want to stop and exit?",
                    "Confirm Exit",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
                Stop();
            }

            // Restore normal sleep behavior
            SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);
            trayIcon?.Dispose();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);
                trayIcon?.Dispose();
                activityTimer?.Dispose();
                uiUpdateTimer?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
