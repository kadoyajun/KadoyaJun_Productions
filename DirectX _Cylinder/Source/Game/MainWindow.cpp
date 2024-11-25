#include "Game.h"
#include <system_error>

MainWindow::MainWindow(const WindowSettings& settings)
{
	HINSTANCE hInstance = GetModuleHandle(NULL);
	const wchar_t CLASS_NAME[] = L"GameWindow";
	WNDCLASSEX wndClass = {};
	wndClass.cbSize = sizeof(WNDCLASSEX);
	wndClass.lpfnWndProc = WindowProc;
	wndClass.hInstance = hInstance;
	wndClass.hCursor = LoadCursor(NULL, IDC_ARROW);
	wndClass.hbrBackground = (HBRUSH)COLOR_BACKGROUND;
	wndClass.lpszClassName = CLASS_NAME;
	if (!RegisterClassEx(&wndClass)) {
		throw std::system_error(GetLastError(), std::system_category(), __FUNCTION__);
	}

	// ウィンドウサイズの計算
	RECT rect = { 0, 0, settings.screenWidth, settings.screenHeight };
	AdjustWindowRectEx(&rect, WS_OVERLAPPEDWINDOW, FALSE, 0);

	handle = CreateWindowEx(
		0, CLASS_NAME, settings.title.c_str(), WS_OVERLAPPEDWINDOW,
		CW_USEDEFAULT, CW_USEDEFAULT,
		(rect.right - rect.left), (rect.bottom - rect.top),
		NULL, NULL, hInstance, NULL);
	if (handle == NULL) {
		throw std::system_error(GetLastError(), std::system_category(), __FUNCTION__);
	}

	ShowWindow(handle, SW_SHOWNORMAL);
	if (!UpdateWindow(handle)) {
		throw std::system_error(GetLastError(), std::system_category(), __FUNCTION__);
	}

	this->settings = settings;
}
//ウィンドウの幅、高さ、ハンドルの取得
int MainWindow::GetWidth()const
{
	return settings.screenWidth;
}
int MainWindow::GetHeight()const
{
	return settings.screenHeight;
}
HWND MainWindow::GetHandle()const
{
	return handle;
}

LRESULT MainWindow::WindowProc(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam)
{
	switch (uMsg) {
	case WM_CLOSE:
		if (MessageBox(hWnd, L"ウィンドウを閉じますか？", L"メッセージ", MB_OKCANCEL) == IDOK) {
			DestroyWindow(hWnd);
		}
		return 0;

	case WM_DESTROY:
		PostQuitMessage(0);
		return 0;
	}

	return DefWindowProc(hWnd, uMsg, wParam, lParam);
}
