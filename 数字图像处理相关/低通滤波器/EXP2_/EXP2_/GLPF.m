function ProcessedPic = GLPF(pic,d0)
I = imread(pic);
I=rgb2gray(I);
% 函数fft2()用于计算二维傅立叶变换
% 函数fftshift()是对函数fft2()作傅里叶变换后得到的频谱进行平移,将变换后的图像频谱中心从矩阵的原点移到矩阵的中心
% 作二维傅里叶变换前一定要用函数im2double()把原始图像的数据类型由uint8转化为double类型
% 否则会因为unit8数据类型只能表示0-255的整数而出现数据截断,进而出现错误结果
s=fftshift(fft2(im2double(I)));
[a,b]=size(s);
a0=round(a/2);
b0=round(b/2);
for i=1:a
    for j=1:b
        distance=sqrt((i-a0)^2+(j-b0)^2);    % 根据高斯低通滤波器公式H(u,v)=e^-[D^2(u,v)/2*D0^2] 
        h=exp(-(distance*distance)/(2*(d0^2))); % exp表示以e为底的指数函数
        s(i,j)=h*s(i,j);% 频域图像乘以滤波器的系数
    end
end
ProcessedPic=real(ifft2(ifftshift(s)));% 最后进行二维傅里叶反变换转换为时域图像
end
