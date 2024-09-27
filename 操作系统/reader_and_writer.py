import time
import threading
from threading import Semaphore
import random
from tkinter import *

Rcount = 0  # 进行的读者队列数量
Wcount = 0  # 进行的写者队列数量
Wmutex = Semaphore(1)  # 对临界资源Wcount的互斥访问
Rmutex = Semaphore(1)  # 对临界资源Rcount的互斥访问
File = Semaphore(1)  # 解决“写着与写着”和“写着与第一个读者”的互斥问题
Wirte = Semaphore(1)  # 表示写者进程


def reader(i): # 读者进程
    while 1:
        canvas.itemconfigure('reader' + str(i - 1), fill='green')
        print('reader' + str(i) + ' waiting to read\n', end='')
        Wirte.acquire()
        Rmutex.acquire()
        global Rcount
        if Rcount == 0:
            File.acquire()
        Rcount += 1
        Rmutex.release()
        Wirte.release()
        canvas.itemconfigure('reader' + str(i - 1), fill='red')
        canvas.itemconfigure('readerline' + str(i - 1), fill='red')
        print('reader' + str(i) + ' reading\n', end='')
        time.sleep(random.randint(1, 5))
        Rmutex.acquire()
        Rcount -= 1
        if Rcount == 0:
            File.release()
        Rmutex.release()
        canvas.itemconfigure('reader' + str(i - 1), fill='black')
        canvas.itemconfigure('readerline' + str(i - 1), fill='')
        print('reader' + str(i) + ' finish read\n', end='')
        time.sleep(random.randint(4, 8))


def writer(i): # 写者进程
    while 1:
        canvas.itemconfigure('writer' + str(i - 1), fill='green')
        print('writer' + str(i) + ' waiting to write\n', end='')
        Wmutex.acquire()
        global Wcount
        if Wcount == 0:
            Wirte.acquire()
        Wcount += 1
        Wmutex.release()
        File.acquire()
        print('writer' + str(i) + ' writing\n', end='')
        canvas.itemconfigure('writer' + str(i - 1), fill='red')
        canvas.itemconfigure('writerline' + str(i - 1), fill='red')
        time.sleep(random.randint(1, 3))
        File.release()
        Wmutex.acquire()
        Wcount -= 1
        if Wcount == 0:
            Wirte.release()
        Wmutex.release()
        canvas.itemconfigure('writer' + str(i - 1), fill='black')
        canvas.itemconfigure('writerline' + str(i - 1), fill='')
        print('writer' + str(i) + ' finish write\n', end='')
        time.sleep(random.randint(10, 15))


# GUI界面
root = Tk()
root.geometry('800x500')
canvas = Canvas(root, width=800, height=500)
canvas.pack()

# 绘制公共资源
canvas.create_oval(250, 125, 600, 375, width=2)

# 绘制读者
for i in range(5):
    canvas.create_rectangle(200 + i * 100, 50, 250 + i * 100, 100, fill='black', tags=('reader' + str(i),))
    canvas.create_line(225 + i * 100, 100, 425, 250, fill="", width=3, tags=('readerline' + str(i),))
    canvas.create_text(225 + i * 100, 75, text='Reader ' + str(i + 1))

# 绘制写者
for i in range(5):
    canvas.create_rectangle(200 + i * 100, 400, 250 + i * 100, 450, fill='black', tags=('writer' + str(i),))
    canvas.create_line(225 + i * 100, 400, 425, 250, fill="", width=3, tags=('writerline' + str(i),))
    canvas.create_text(225 + i * 100, 425, text='Writer ' + str(i + 1))

if __name__ == '__main__':
    # times = 10
    rwlist = [1, 1, 1, 1, 1, 0, 0, 0, 0, 0]
    # for _ in range(times):
    # rwlist.append(random.randint(0, 1))
    print(rwlist)
    print('其中1表示读者进程，0表示写者进程')
    rindex = 1
    windex = 1
    for i in rwlist:
        if i == 1:
            t = threading.Thread(target=reader, args=(rindex,))
            rindex += 1
            t.start()
        else:
            t = threading.Thread(target=writer, args=(windex,))
            windex += 1
            t.start()

root.mainloop()
