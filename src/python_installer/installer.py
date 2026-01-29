import tkinter as tk
from tkinter import ttk, messagebox, font
import os
import shutil
import sys
import winreg as reg
from PIL import Image, ImageTk
import ctypes
import threading

class ModernInstaller:
    def __init__(self):
        self.root = tk.Tk()
        self.root.title("AntiVurus Hack - Установка")
        self.root.geometry("800x500")
        self.root.configure(bg='#1a1a1a')
        self.center_window()
        
        # Стили
        self.style = ttk.Style()
        self.style.theme_use('clam')
        self.setup_styles()
        
        self.create_widgets()
        
    def center_window(self):
        self.root.update_idletasks()
        width = self.root.winfo_width()
        height = self.root.winfo_height()
        x = (self.root.winfo_screenwidth() // 2) - (width // 2)
        y = (self.root.winfo_screenheight() // 2) - (height // 2)
        self.root.geometry(f'{width}x{height}+{x}+{y}')
    
    def setup_styles(self):
        self.style.configure('Title.TLabel', background='#1a1a1a', foreground='#00ff88', font=('Segoe UI', 24, 'bold'))
        self.style.configure('Subtitle.TLabel', background='#1a1a1a', foreground='#cccccc', font=('Segoe UI', 12))
        self.style.configure('Progress.Horizontal.TProgressbar', background='#00ff88', troughcolor='#333333')
        self.style.configure('Modern.TButton', background='#00ff88', foreground='#000000', font=('Segoe UI', 12, 'bold'))
    
    def create_widgets(self):
        # Заголовок
        title_frame = ttk.Frame(self.root, style='Title.TLabel')
        title_frame.pack(pady=30)
        
        ttk.Label(title_frame, text="⚡ ANTI-VURUS HACK", style='Title.TLabel').pack()
        ttk.Label(title_frame, text="Мощный инструмент защиты системы", style='Subtitle.TLabel').pack(pady=10)
        
        # Индикатор прогресса
        self.progress_frame = ttk.Frame(self.root, style='Title.TLabel')
        self.progress_frame.pack(pady=40)
        
        self.progress = ttk.Progressbar(self.progress_frame, style='Progress.Horizontal.TProgressbar', 
                                       length=400, mode='determinate')
        self.progress.pack()
        
        self.status_label = ttk.Label(self.progress_frame, text="Готов к установке...", 
                                     style='Subtitle.TLabel')
        self.status_label.pack(pady=10)
        
        # Кнопка установки
        button_frame = ttk.Frame(self.root, style='Title.TLabel')
        button_frame.pack(pady=20)
        
        self.install_btn = tk.Button(button_frame, text="🚀 НАЧАТЬ УСТАНОВКУ", 
                                    bg='#00ff88', fg='#000000', font=('Segoe UI', 14, 'bold'),
                                    bd=0, padx=30, pady=15, command=self.start_installation,
                                    cursor='hand2', activebackground='#00cc66')
        self.install_btn.pack()
        
        # Футер
        footer = ttk.Label(self.root, text="© 2024 ArcenaL4Ik | Версия 1.0", 
                          style='Subtitle.TLabel')
        footer.pack(side='bottom', pady=10)
    
    def start_installation(self):
        self.install_btn.config(state='disabled', text="⚙ УСТАНОВКА...")
        threading.Thread(target=self.install_process, daemon=True).start()
    
    def install_process(self):
        steps = [
            ("Подготовка файлов...", 10),
            ("Копирование системных компонентов...", 25),
            ("Настройка реестра...", 45),
            ("Установка служб...", 65),
            ("Создание ярлыков...", 85),
            ("Завершение установки...", 100)
        ]
        
        install_dir = os.path.join(os.environ['PROGRAMFILES'], 'AntiVurusHack')
        
        for text, value in steps:
            self.status_label.config(text=text)
            self.progress['value'] = value
            self.root.update()
            
            if "копирование" in text.lower():
                self.copy_files(install_dir)
            elif "реестр" in text.lower():
                self.setup_registry(install_dir)
            elif "ярлык" in text.lower():
                self.create_shortcuts(install_dir)
            
            import time
            time.sleep(0.5)
        
        messagebox.showinfo("Успех!", "AntiVurus Hack успешно установлен!\n\nПрограмма доступна в меню Пуск и на Рабочем столе.")
        self.root.quit()
    
    def copy_files(self, target_dir):
        os.makedirs(target_dir, exist_ok=True)
        # Здесь будет копирование реальных файлов
        with open(os.path.join(target_dir, 'readme.txt'), 'w') as f:
            f.write("AntiVurus Hack - Мощный инструмент защиты")
    
    def setup_registry(self, install_dir):
        try:
            # Автозагрузка
            key = reg.OpenKey(reg.HKEY_CURRENT_USER, 
                            r"Software\Microsoft\Windows\CurrentVersion\Run", 
                            0, reg.KEY_SET_VALUE)
            reg.SetValueEx(key, "AntiVurusHack", 0, reg.REG_SZ, 
                          os.path.join(install_dir, "AntiVurusHack.exe"))
            reg.CloseKey(key)
            
            # Другие ключи реестра
            locations = [
                r"Software\Microsoft\Windows\CurrentVersion\Policies\System",
                r"Software\Microsoft\Windows NT\CurrentVersion\Winlogon",
                r"Software\Microsoft\Windows\CurrentVersion\Explorer"
            ]
            
            for loc in locations:
                try:
                    reg.CreateKey(reg.HKEY_CURRENT_USER, loc)
                except:
                    pass
                    
        except Exception as e:
            pass
    
    def create_shortcuts(self, install_dir):
        desktop = os.path.join(os.path.expanduser('~'), 'Desktop')
        shortcut = os.path.join(desktop, 'AntiVurus Hack.lnk')
        
        # Создание ярлыка через VBScript
        vbs = f"""
        Set ws = CreateObject("WScript.Shell")
        Set shortcut = ws.CreateShortcut("{shortcut}")
        shortcut.TargetPath = "{os.path.join(install_dir, "AntiVurusHack.exe")}"
        shortcut.WorkingDirectory = "{install_dir}"
        shortcut.IconLocation = "{os.path.join(install_dir, "icon.ico")}"
        shortcut.Save
        """
        
        vbs_path = os.path.join(os.environ['TEMP'], 'create_shortcut.vbs')
        with open(vbs_path, 'w') as f:
            f.write(vbs)
        
        os.system(f'wscript.exe "{vbs_path}"')
    
    def run(self):
        self.root.mainloop()

if __name__ == "__main__":
    app = ModernInstaller()
    app.run()
