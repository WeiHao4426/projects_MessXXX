import threading
import random
import time
import tkinter as tk
from tkinter import messagebox # 弹窗库


class Philosopher(threading.Thread): # 哲学家类
    def __init__(self, name, left_chopstick, right_chopstick, mode, timeout):
        threading.Thread.__init__(self)
        self.name = name
        self.left_chopstick = left_chopstick
        self.right_chopstick = right_chopstick
        self.mode = mode
        self.timeout = timeout

    def run(self):
        while True:
            self.think()
            self.get_chopsticks()
            self.eat()
            self.put_chopsticks()

    def think(self):
        print(f"{self.name} is thinking...")
        if self.timeout:
            time.sleep(random.uniform(1, 3))
        else:
            time.sleep(random.uniform(1, 10))
        # self.canvas.itemconfigure(self.name, fill='green')

    def get_chopsticks(self):
        print(f"{self.name} is getting chopsticks...")
        if self.mode:
            while True:
                if self.left_chopstick.acquire(timeout=1):
                    # print(f"{self.name} get left chopstick...")
                    # self.canvas.itemconfigure(str(int(self.name[12:])), fill='red')
                    # print(str(int(self.name[12:])))
                    locked = self.right_chopstick.acquire(timeout=1)
                    # self.canvas.itemconfigure(str(int(self.name[12:]) + 1), fill='red')
                    if locked:
                        # print(f"{self.name} get right chopstick...")
                        break
                    else:
                        print(f"{self.name} release left chopstick...")
                        # self.canvas.itemconfigure(str(int(self.name[12:])), fill='green')
                    self.left_chopstick.release()
                    print(f"{self.name} is waiting for chopsticks...")
        else:
            while True:
                if self.left_chopstick.acquire(timeout=1):
                    print(f"{self.name} get left chopstick...")
                    # self.canvas.itemconfigure(str(int(self.name[12:])), fill='red')
                    locked = self.right_chopstick.acquire(timeout=1)
                    if locked:
                        print(f"{self.name} get right chopstick...")
                        # self.canvas.itemconfigure(str(int(self.name[12:]) + 1), fill='red')
                        break
                    print(f"{self.name} is waiting for right chopstick...")
        print(f"{self.name} got both chopsticks.")
        # self.canvas.itemconfigure(self.name, fill='red')

    def eat(self):
        print(f"{self.name} is eating...")
        time.sleep(random.uniform(2, 3))
        print(f"{self.name} finished eating.")

    def put_chopsticks(self):
        print(f"{self.name} is putting chopsticks...")
        # self.canvas.itemconfigure(self.name, fill='black')
        self.right_chopstick.release()
        # self.canvas.itemconfigure(str(int(self.name[12:])), fill='green')
        # print(str(int(self.name[12:])))
        self.left_chopstick.release()
        # self.canvas.itemconfigure(str(int(self.name[12:]) + 1), fill='green')


def start():
    mode = messagebox.askyesno("选择运行模式", "是否采用防死锁方式运行？")
    if mode:
        timeout = None
    else:
        timeout = messagebox.askyesno('选择运行时长', '是为短，否为长')
    chopsticks = [threading.Lock() for _ in range(5)]
    philosophers = [Philosopher(f"Philosopher {i + 1}", chopsticks[i], chopsticks[(i + 1) % 5], mode, timeout)
                    for i in range(5)]
    for philosopher in philosophers:
        philosopher.start()


# # GUI界面
# root = tk.Tk()
# root.geometry('800x500')
# canvas = tk.Canvas(root, width=800, height=500)
# canvas.pack()
#
# # 绘制公共资源
# #canvas.create_oval(250, 125, 600, 375, width=2, tags=f"Philosopher")
#
# # 绘制哲学家
# for i in range(5):
#     canvas.create_rectangle(200 + i * 100, 50, 250 + i * 100, 100, fill='black', tags=(f"Philosopher {i + 1}",))
#     canvas.create_oval(150 + i * 100, 150, 200 + i * 100, 200, fill='black', tags=str(i + 1))
#     #canvas.create_line(225 + i * 100, 100, 425, 250, fill="", width=3, tags=('philosopherline' + str(i + 1),))
#     #canvas.create_text(225 + i * 100, 75, text='philosopher ' + str(i + 1))

if __name__ == "__main__":
    root1 = tk.Tk()
    root1.withdraw()
    start()

# root.mainloop()
