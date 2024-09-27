
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


int Readercount;                       //��������
int readerstate[CCounter];             //����״̬
int writerstate[CCounter];             //д��״̬
int resourcestate[CCounter];           //��Դ״̬
CRITICAL_SECTION  RP_Write;            
HANDLE count;                          //���ڶ��� readcount ����Դ
#undef PostMessage
#define PostMessage SendMessage

DWORD WINAPI ReaderThread(LPVOID pVoid)//�����̺߳���
{
	int ReaderNum = (int)pVoid;  // ��ȡ�߳���       
	DWORD result;
	//����������������
	srand((unsigned)time(NULL) * (ReaderNum + 1));
	readerstate[ReaderNum] = resting;        //������Ϣ
	Sleep(P_DELAY);
	for (;;)
	{// �ȴ���Դ����
		readerstate[ReaderNum] = waiting;    //���ߵȴ�
		PostMessage(hWndMain, WM_FORCE_REPAINT, 0, 0);
		result = WaitForSingleObject(count, INFINITE);  //��ȡ��Դ
		if (result == WAIT_OBJECT_0)
			Readercount += 1;
		if (Readercount == 1)
			EnterCriticalSection(&RP_Write);  //��ȡ�ٽ���
		MTVERIFY(ReleaseMutex(count));        // �ͷ���Դ
		resourcestate[ReaderNum] = read;
		readerstate[ReaderNum] = reading;     //�������ڶ�
		PostMessage(hWndMain, WM_FORCE_REPAINT, 0, 0);
		Sleep(P_DELAY / 4);

		result = WaitForSingleObject(count, INFINITE); //��ȡ��Դ
		if (result == WAIT_OBJECT_0)
			Readercount -= 1;
		if (Readercount == 0)
			LeaveCriticalSection(&RP_Write);     //�뿪�ٽ���
		MTVERIFY(ReleaseMutex(count));           //�ͷ���Դ	
		readerstate[ReaderNum] = resting;        //������Ϣ
		resourcestate[ReaderNum] = UNUSED;
		PostMessage(hWndMain, WM_FORCE_REPAINT, 0, 0);
		Sleep(P_DELAY);
	}
	return 0;
}
DWORD WINAPI WriterThread(LPVOID pVoid)//д���̺߳���
{
	int writerNum = (int)pVoid;   //��ȡд���̵߳ı��
   //�����������
	srand((unsigned)time(NULL) * (writerNum + 1));
	writerstate[writerNum] = resting;  //д����Ϣ
	Sleep(P_DELAY);
	for (;;)
	{ 
		writerstate[writerNum] = waiting;   //д�ߵȴ�
		PostMessage(hWndMain, WM_FORCE_REPAINT, 0, 0);
		EnterCriticalSection(&RP_Write);   // �����ٽ���
		writerstate[writerNum] = writing;  // д������д
		resourcestate[writerNum] = write;
		PostMessage(hWndMain, WM_FORCE_REPAINT, 0, 0);
		Sleep(P_DELAY / 4);
		LeaveCriticalSection(&RP_Write);   //�뿪�ٽ���
		writerstate[writerNum] = resting;  // д����Ϣ
		resourcestate[writerNum] = UNUSED;
		PostMessage(hWndMain, WM_FORCE_REPAINT, 0, 0);
		Sleep(P_DELAY);
	}
	return 0;
}

int ReaderAndWriter(void)  //������
{
	HANDLE hThread[CCounter];
	HANDLE hThread1[CCounter];
	DWORD dwThreadId;
	int i;
	Readercount = 0;
	resourcestate[CCounter] = UNUSED;
	count = CreateMutex(NULL, FALSE, NULL);//����������
	MTVERIFY(count != NULL);
	InitializeCriticalSection(&RP_Write);  //��ʼ���ٽ���
	for (i = 0; i < CCounter; i++)
	{//��ʼ�����ߺ�д��״̬
		readerstate[i] = resting;
		writerstate[i] = resting;
	}

	for (i = 0; i < CCounter; i++)
	{   //�������ߺ�д���߳�
		MTVERIFY(hThread[i] = CreateThread(NULL, 0, ReaderThread, (LPVOID)i, 0, &dwThreadId));//���������߳�
		MTVERIFY(hThread1[i] = CreateThread(NULL, 0, WriterThread, (LPVOID)i, 0, &dwThreadId));//����д���߳�
	}
	return 0;
}