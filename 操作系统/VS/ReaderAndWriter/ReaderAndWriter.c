/*
 * ReaderAndWriter.c
 *
 * Sample code for "Multithreading Applications in Win32"
 * This sample is discussed in Chapter 4.
 *
 * Graphically demonstrates the problem of the
 * ReaderAndWriter.
 */

#define WIN32_LEAN_AND_MEAN
#include <stdio.h>
#include <stdlib.h>
#include <windows.h>
#include <windowsx.h>
#include <string.h>
#include <math.h>
#include "ReaderAndWriter.h"

HINSTANCE 	hInst;			// Application Instance Handle
HWND   	hWndMain;       	// Main Window Handle                             
HBITMAP	hbmpOffscreen;      //bitmap handle

LRESULT CALLBACK WndProc(HWND, UINT, WPARAM, LPARAM);//���ڹ���
void RenderOffscreen(HDC hDestDC);//���ƺ���
extern int Readercount;//������
extern int readerstate[CCounter];//����״̬
extern int writerstate[CCounter];//д��״̬
extern int resourcestate[CCounter];//��Դ״̬
extern HANDLE count;//�ź���


LRESULT CALLBACK WndProc(HWND hWnd, UINT message,
	WPARAM wParam, LPARAM lParam)//���ڹ���
{
	PAINTSTRUCT ps;
	HDC hdc;

	switch (message)
	{
	case WM_COMMAND:
		switch (wParam)
		{
		case CM_EXIT:
			PostMessage(hWndMain, WM_CLOSE, 0, 0L);
			break;
		}
		break;

	case WM_FORCE_REPAINT://ǿ���ػ�
	{
		MSG msg;

		InvalidateRect(hWndMain, NULL, TRUE);
		while (PeekMessage(&msg, hWndMain, WM_FORCE_REPAINT, WM_FORCE_REPAINT, TRUE))//�����Ϣ����
			;
	}
	break;

	case WM_PAINT:

		hdc = BeginPaint(hWndMain, &ps);

		RenderOffscreen(hdc);

		EndPaint(hWndMain, &ps);
		break;

	case WM_CLOSE:
		return DefWindowProc(hWndMain, message, wParam, lParam);//�رմ���

	case WM_DESTROY:
		PostQuitMessage(0);
		break;

	default:
		return DefWindowProc(hWnd, message, wParam, lParam);
	}
	return (0);
}

BOOL CreateOffscreen()//����λͼ
{
	HWND hwndScreen = GetDesktopWindow();
	HDC hdc = GetDC(hwndScreen);

	int	nWidth = GetSystemMetrics(SM_CXSCREEN);
	int	nHeight = GetSystemMetrics(SM_CYSCREEN);

	hbmpOffscreen = CreateCompatibleBitmap(hdc, nWidth, nHeight);

	ReleaseDC(hwndScreen, hdc);

	if (!hbmpOffscreen)
		return FALSE;
	else
		return TRUE;
}

void RenderOffscreen(HDC hDestDC)
{
	HDC hdc = hDestDC;
	int err = GetLastError();
	HBITMAP hOldBitmap = SelectObject(hdc, hbmpOffscreen);
	RECT rect;
	HPEN hPen;
	int i;
	long CenterX, CenterY;

	hPen = SelectObject(hdc, CreatePen(PS_SOLID, 3, 0L));

	GetClientRect(hWndMain, &rect);

	//Draw the resource
	CenterX = (rect.right - rect.left) / 2;
	CenterY = (rect.bottom - rect.top) / 2;
	Ellipse(hdc, CenterX - 80, CenterY - 80, CenterX + 80, CenterY + 80);
	DeleteObject(SelectObject(hdc, hPen));

	//draw the reader
	for (i = 0; i < CCounter; i++)//���ƶ���
	{
		/* select a pen for each reader */
		switch (readerstate[i])
		{
		case resting://��Ϣ
			hPen = SelectObject(hdc, CreatePen(PS_SOLID, 3, RGB(0, 0, 0)));//��ɫ
			break;

		case waiting://�ȴ�
			hPen = SelectObject(hdc, CreatePen(PS_SOLID, 3, RGB(0, 255, 0)));//��ɫ
			break;
		case reading://��ȡ
			hPen = SelectObject(hdc, CreatePen(PS_SOLID, 3, RGB(255, 0, 0)));//��ɫ

		default:
			hPen = SelectObject(hdc, CreatePen(PS_SOLID, 3, 0L));
		}

		/* Draw the reader */
		Rectangle(hdc, CenterX + 120 * (i - 2) - 40, CenterY - 200, CenterX + 120 * (i - 2) + 40, CenterY - 150);

		//draw the right of reading
		if (resourcestate[i] == read)
		{
			MoveToEx(hdc, CenterX, CenterY, NULL);
			LineTo(hdc, CenterX + 120 * (i - 2), CenterY - 175);
		}
		DeleteObject(SelectObject(hdc, hPen));

	}

	//draw the writer
	for (i = 0; i < CCounter; i++)//����д��
	{
		/* select a pen for each reader */
		switch (writerstate[i])
		{
		case resting:
			hPen = SelectObject(hdc, CreatePen(PS_SOLID, 3, RGB(0, 0, 0)));
			break;

		case waiting:
			hPen = SelectObject(hdc, CreatePen(PS_SOLID, 3, RGB(0, 255, 0)));
			break;
		case writing:
			hPen = SelectObject(hdc, CreatePen(PS_SOLID, 3, RGB(255, 0, 0)));
			break;

		default:
			hPen = SelectObject(hdc, CreatePen(PS_SOLID, 3, 0L));
		}

		/* Draw the writer */
		Rectangle(hdc, CenterX + 120 * (i - 2) - 40, CenterY + 150, CenterX + 120 * (i - 2) + 40, CenterY + 200);

		//draw the right of writing
		if (resourcestate[i] == write)
		{
			MoveToEx(hdc, CenterX, CenterY, NULL);
			LineTo(hdc, CenterX + 120 * (i - 2), CenterY + 175);
		}
		DeleteObject(SelectObject(hdc, hPen));

	}

	BitBlt(hDestDC,
		rect.left,
		rect.top,
		rect.right - rect.left,
		rect.bottom - rect.top,
		hdc,
		rect.left,
		rect.top,
		SRCCOPY
	);
	GetLastError();

	SelectObject(hdc, hOldBitmap);

	//	DeleteDC(hWndMain, hdc);
}//���ƺ���//���ƶ��ߺ�д��//������Դ

/********************************************************************/

BOOL InitApplication(HINSTANCE hInstance)
{
	WNDCLASS  wc;

	wc.style = CS_HREDRAW | CS_VREDRAW;
	wc.lpfnWndProc = WndProc;
	wc.cbClsExtra = 0;
	wc.cbWndExtra = 0;
	wc.hInstance = hInstance;
	wc.hIcon = LoadIcon(NULL, IDI_APPLICATION);
	wc.hCursor = LoadCursor(NULL, IDC_ARROW);
	wc.hbrBackground = (HBRUSH)(COLOR_WINDOW + 1);
	wc.lpszMenuName = "ReaderAndWriter";
	wc.lpszClassName = "ReaderAndWriterClass";

	RegisterClass(&wc);

	return TRUE;
}//��ʼ��Ӧ�ó���


/********************************************************************/


BOOL InitInstance(HINSTANCE hInstance, int nCmdShow)
{
	// 	int ret;

	hInst = hInstance;

	hWndMain = CreateWindow(
		"ReaderAndWriterClass",
		"ReaderAndWriter",
		WS_OVERLAPPEDWINDOW,
		CW_USEDEFAULT,
		CW_USEDEFAULT,
		450,
		450,
		NULL, NULL, hInstance, NULL);

	if (!hWndMain)
		return FALSE;

	ShowWindow(hWndMain, nCmdShow);
	UpdateWindow(hWndMain);

	if (!CreateOffscreen())
		PostQuitMessage(1);
	// Start the threads
	ReaderAndWriter();

	return TRUE;
}

/********************************************************************/

int PASCAL WinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance,
	LPSTR lpCmdLine, int nCmdShow)
{
	MSG msg;
	if (!hPrevInstance)
		if (!InitApplication(hInstance))
			return (FALSE);

	if (!InitInstance(hInstance, nCmdShow))
		return (FALSE);

	while (GetMessage(&msg, NULL, 0, 0))//��ȡ��Ϣ
	{   //Processes messages
		TranslateMessage(&msg);
		DispatchMessage(&msg);
	}
	CloseHandle(count);
	return (msg.wParam);
}//������