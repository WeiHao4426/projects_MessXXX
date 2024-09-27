close all;
clear all;
clc;
I = imread('yuanshi.jpg');
I=rgb2gray(I);

subplot(131),imshow(I);
title('ԭʼͼ��');


% ����fft2()���ڼ����ά����Ҷ�任
% ����fftshift()�ǶԺ���fft2()������Ҷ�任��õ���Ƶ�׽���ƽ��,���任���ͼ��Ƶ�����ĴӾ����ԭ���Ƶ����������
% ����ά����Ҷ�任ǰһ��Ҫ�ú���im2double()��ԭʼͼ�������������uint8ת��Ϊdouble����
% �������Ϊunit8��������ֻ�ܱ�ʾ0-255���������������ݽض�,�������ִ�����
s=fftshift(fft2(im2double(I)));
[a,b]=size(s);
a0=round(a/2);
b0=round(b/2);
d0=230; % �������ͨ�˲����Ľ�ֹƵ��D0����Ϊ50
for i=1:a %˫��forѭ������Ƶ�ʵ�(i,j)��Ƶ�����ĵľ���D(i,j)=sqrt((i-round(a/2)^2+(j-round(b/2)^2))
    for j=1:b 
        distance=sqrt((i-a0)^2+(j-b0)^2);
        if distance<=d0  % ���������ͨ�˲���������ʽ,��D(i,j)<=D0,��Ϊ1
            h=1;
        else
            h=0;        % ���������ͨ�˲���������ʽ,��D(i,j)>D0,��Ϊ0
        end
        s(i,j)=h*s(i,j);% Ƶ��ͼ������˲�����ϵ��
    end
end
% real����ȡԪ�ص�ʵ��
s=real(ifft2(ifftshift(s)));% �����ж�ά����Ҷ���任ת��Ϊʱ��ͼ��
subplot(133),imshow(s,[]);
title('�����ͨ�˲�����ͼ��'); 
