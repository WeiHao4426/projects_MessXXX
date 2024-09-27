close all;
clear all;
clc;
I = imread('yuanshi.jpg');
I=rgb2gray(I);

subplot(131),imshow(I);
title('原始图像');


% 函数fft2()用于计算二维傅立叶变换
% 函数fftshift()是对函数fft2()作傅里叶变换后得到的频谱进行平移,将变换后的图像频谱中心从矩阵的原点移到矩阵的中心
% 作二维傅里叶变换前一定要用函数im2double()把原始图像的数据类型由uint8转化为double类型
% 否则会因为unit8数据类型只能表示0-255的整数而出现数据截断,进而出现错误结果
s=fftshift(fft2(im2double(I)));
[a,b]=size(s);
a0=round(a/2);
b0=round(b/2);
d0=230; % 将理想低通滤波器的截止频率D0设置为50
for i=1:a %双重for循环计算频率点(i,j)与频域中心的距离D(i,j)=sqrt((i-round(a/2)^2+(j-round(b/2)^2))
    for j=1:b 
        distance=sqrt((i-a0)^2+(j-b0)^2);
        if distance<=d0  % 根据理想低通滤波器产生公式,当D(i,j)<=D0,置为1
            h=1;
        else
            h=0;        % 根据理想低通滤波器产生公式,当D(i,j)>D0,置为0
        end
        s(i,j)=h*s(i,j);% 频域图像乘以滤波器的系数
    end
end
% real函数取元素的实部
s=real(ifft2(ifftshift(s)));% 最后进行二维傅里叶反变换转换为时域图像
subplot(133),imshow(s,[]);
title('理想低通滤波所得图像'); 
