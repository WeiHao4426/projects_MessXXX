/*
 * Mutex.c
 *
 * Sample code for "Multithreading Applications in Win32"
 * This sample is discussed in Chapter 4.
 *
 * Graphically demonstrates the problem of the
 * dining philosophers.
 *
 * This version uses mutexes with WaitForSingleObject(),
 * which can cause deadlock, and WaitForMultipleObjects(),
 * which always works properly.
 */

#define WIN32_LEAN_AND_MEAN
#include <stdio.h>
#include <stdlib.h>
#include <windows.h>
#include <time.h>
#include "MtVerify.h"
#include "dining.h"


int PASCAL WinMain(HINSTANCE, HINSTANCE, LPSTR, int);
BOOL InitApplication(HINSTANCE);
BOOL InitInstance(HINSTANCE, int);

extern HWND   	hWndMain;			// Main Window Handle                             
extern BOOL bWaitMultiple;
extern BOOL bFastFood;

#define P_DELAY		bFastFood ? rand()/25 : ((rand()%5)+1)*1000 //控制进程时间

int gDinerState[PHILOSOPHERS];
int gChopstickState[PHILOSOPHERS];

HANDLE gchopStick[PHILOSOPHERS];	//每个哲学家和他的邻居之间有 1 根筷子

#undef PostMessage
#define PostMessage SendMessage

DWORD WINAPI PhilosopherThread(LPVOID pVoid)
{
	HANDLE myChopsticks[2];
	int iPhilosopher = (int) pVoid;
	int iLeftChopstick = iPhilosopher;
	int iRightChopstick = iLeftChopstick + 1;
	DWORD result;

	if (iRightChopstick > PHILOSOPHERS-1)  //筷子编号过了5就使它为0
		iRightChopstick = 0;

    //Randomize the random number generator
	srand( (unsigned)time( NULL ) * (iPhilosopher + 1) );

	// remember handles for my chopsticks
	myChopsticks[0] = gchopStick[iLeftChopstick];      //定义哲学家的左右筷子
	myChopsticks[1] = gchopStick[iRightChopstick];

	gDinerState[iPhilosopher] = RESTING;	//需要筷子

    Sleep(P_DELAY);

	for(;;)
	{
		if (bWaitMultiple == FALSE)
		{
			// 等到两根筷子都可用
			gDinerState[iPhilosopher] = WAITING;	//需要筷子

            PostMessage(hWndMain, WM_FORCE_REPAINT,0 ,0); 
			result = WaitForSingleObject(gchopStick[iLeftChopstick], INFINITE);
			MTVERIFY(result == WAIT_OBJECT_0);
			gChopstickState[iLeftChopstick] = iPhilosopher;
			Sleep(P_DELAY/4);

			gDinerState[iPhilosopher] = WAITING;	//需要筷子
            PostMessage(hWndMain, WM_FORCE_REPAINT,0 ,0);
			result = WaitForSingleObject(gchopStick[iRightChopstick], INFINITE);
			MTVERIFY(result == WAIT_OBJECT_0);
			gChopstickState[iRightChopstick] = iPhilosopher;
		}
		else
		{
			// 两个筷子都可用
			gDinerState[iPhilosopher] = WAITING;	//wants chopsticks
			PostMessage(hWndMain, WM_FORCE_REPAINT,0 ,0);//发送消息
			result = WaitForMultipleObjects(2, myChopsticks, TRUE, INFINITE);//等待两个筷子都可用
			MTVERIFY(result >= WAIT_OBJECT_0 && result < WAIT_OBJECT_0 + 2);
			gChopstickState[iLeftChopstick] = iPhilosopher;//筷子被占用
			gChopstickState[iRightChopstick] = iPhilosopher;
		}

		// 哲学家现在可以吃了
		gDinerState[iPhilosopher] = EATING;	//正在吃
		PostMessage(hWndMain, WM_FORCE_REPAINT,0 ,0);
        Sleep(P_DELAY);

		// 放下筷子
		gDinerState[iPhilosopher] = RESTING;	//休息
		gChopstickState[iRightChopstick] = UNUSED;
		gChopstickState[iLeftChopstick] = UNUSED;
		PostMessage(hWndMain, WM_FORCE_REPAINT,0 ,0);
		MTVERIFY( ReleaseMutex(gchopStick[iLeftChopstick]) ); //释放筷子资源
		MTVERIFY( ReleaseMutex(gchopStick[iRightChopstick]) );

		// sleep
        Sleep(P_DELAY);

	} // end for

	return 0;
}

int Diner(void)
{
	HANDLE hThread[PHILOSOPHERS];
	DWORD dwThreadId;
	int i;

	for (i=0; i < PHILOSOPHERS; i++)//初始化筷子和哲学家状态
	{
		//将 chopsitcks 初始化为未使用
		gChopstickState[i] = UNUSED;
		// 初始化 diner 状态表
		gDinerState[i] = 0;
		// 哲学家们准备吃饭
		gchopStick[i] = CreateMutex(NULL, FALSE, NULL); //建立互斥量
		MTVERIFY(gchopStick[i] != NULL);
	}

	for (i = 0; i < PHILOSOPHERS; i++)
		MTVERIFY( hThread[i] = CreateThread(NULL, 0, PhilosopherThread, (LPVOID) i, 0, &dwThreadId ));//启动进程

	return 0;
}
