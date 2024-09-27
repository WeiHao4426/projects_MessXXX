
#define WIN32_LEAN_AND_MEAN
#include <stdio.h>
#include <stdlib.h>
#include <windows.h>
#include <time.h>
#include "MtVerify.h"
#include "ReaderAndWriter.h"


int PASCAL WinMain(HINSTANCE, HINSTANCE, LPSTR, int);
BOOL InitApplication(HINSTANCE);
BOOL InitInstance(HINSTANCE, int);

extern HWND   	hWndMain;			// Main Window Handle                             


#define P_DELAY	 rand()/25*10


int Readercount;                       //读者数量
int readerstate[CCounter];             //读者状态
int writerstate[CCounter];             //写者状态
int resourcestate[CCounter];           //资源状态
CRITICAL_SECTION  RP_Write;            
HANDLE count;                          //用于定义 readcount 的资源
#undef PostMessage
#define PostMessage SendMessage

DWORD WINAPI ReaderThread(LPVOID pVoid)//读者线程函数
{
	int ReaderNum = (int)pVoid;  // 获取线程数       
	DWORD result;
	//随机化随机数生成器
	srand((unsigned)time(NULL) * (ReaderNum + 1));
	readerstate[ReaderNum] = resting;        //读者休息
	Sleep(P_DELAY);
	for (;;)
	{// 等待资源可用
		readerstate[ReaderNum] = waiting;    //读者等待
		PostMessage(hWndMain, WM_FORCE_REPAINT, 0, 0);
		result = WaitForSingleObject(count, INFINITE);  //获取资源
		if (result == WAIT_OBJECT_0)
			Readercount += 1;
		if (Readercount == 1)
			EnterCriticalSection(&RP_Write);  //获取临界区
		MTVERIFY(ReleaseMutex(count));        // 释放资源
		resourcestate[ReaderNum] = read;
		readerstate[ReaderNum] = reading;     //读者正在读
		PostMessage(hWndMain, WM_FORCE_REPAINT, 0, 0);
		Sleep(P_DELAY / 4);

		result = WaitForSingleObject(count, INFINITE); //获取资源
		if (result == WAIT_OBJECT_0)
			Readercount -= 1;
		if (Readercount == 0)
			LeaveCriticalSection(&RP_Write);     //离开临界区
		MTVERIFY(ReleaseMutex(count));           //释放资源	
		readerstate[ReaderNum] = resting;        //读者休息
		resourcestate[ReaderNum] = UNUSED;
		PostMessage(hWndMain, WM_FORCE_REPAINT, 0, 0);
		Sleep(P_DELAY);
	}
	return 0;
}
DWORD WINAPI WriterThread(LPVOID pVoid)//写者线程函数
{
	int writerNum = (int)pVoid;   //获取写者线程的编号
   //随机数生成器
	srand((unsigned)time(NULL) * (writerNum + 1));
	writerstate[writerNum] = resting;  //写者休息
	Sleep(P_DELAY);
	for (;;)
	{ 
		writerstate[writerNum] = waiting;   //写者等待
		PostMessage(hWndMain, WM_FORCE_REPAINT, 0, 0);
		EnterCriticalSection(&RP_Write);   // 进入临界区
		writerstate[writerNum] = writing;  // 写者正在写
		resourcestate[writerNum] = write;
		PostMessage(hWndMain, WM_FORCE_REPAINT, 0, 0);
		Sleep(P_DELAY / 4);
		LeaveCriticalSection(&RP_Write);   //离开临界区
		writerstate[writerNum] = resting;  // 写者休息
		resourcestate[writerNum] = UNUSED;
		PostMessage(hWndMain, WM_FORCE_REPAINT, 0, 0);
		Sleep(P_DELAY);
	}
	return 0;
}

int ReaderAndWriter(void)  //主函数
{
	HANDLE hThread[CCounter];
	HANDLE hThread1[CCounter];
	DWORD dwThreadId;
	int i;
	Readercount = 0;
	resourcestate[CCounter] = UNUSED;
	count = CreateMutex(NULL, FALSE, NULL);//创建互斥量
	MTVERIFY(count != NULL);
	InitializeCriticalSection(&RP_Write);  //初始化临界区
	for (i = 0; i < CCounter; i++)
	{//初始化读者和写者状态
		readerstate[i] = resting;
		writerstate[i] = resting;
	}

	for (i = 0; i < CCounter; i++)
	{   //创建读者和写者线程
		MTVERIFY(hThread[i] = CreateThread(NULL, 0, ReaderThread, (LPVOID)i, 0, &dwThreadId));//创建读者线程
		MTVERIFY(hThread1[i] = CreateThread(NULL, 0, WriterThread, (LPVOID)i, 0, &dwThreadId));//创建写者线程
	}
	return 0;
}