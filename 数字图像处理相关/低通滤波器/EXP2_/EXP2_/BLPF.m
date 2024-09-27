function ProcessedPic = BLPF(pic,d0,n)
I = imread(pic);
I=rgb2gray(I);
% 函数fft2()用于计算二维傅立叶变??% 函数fftshift()是对函数fft2()作傅里叶变换后得到的频谱进行平移,将变换后的图像频谱中心从矩阵的原点移到矩阵的中心
% 作二维傅里叶变换前一定要用函数im2double()把原始图像的数据类型由uint8转化为double类型
% 否则会因为unit8数据类型只能表示0-255的整数??出现数据截断,进??出现错误结果
s=fftshift(fft2(im2double(I)));
[N1,N2]=size(s);%求二维傅里叶变换后图像大??n1=round(N1/2);
n2=round(N2/2);
for i=1:N1      %双重for循环计算频率??i,j)与频域中心的距离D(i,j)=sqrt((i-round(N1/2)^2+(j-round(N2/2)^2))
    for j=1:N2 
        distance=sqrt((i-n1)^2+(j-n2)^2);
        if distance==0 
            h=0; 
        else
            h=1/(1+(distance/d0)^(2*n));% 根据巴特沃斯低??滤波器公式为1/(1+[D(i,j)/D0]^2n)
        end
        s(i,j)=h*s(i,j);% 频域图像乘以滤波器的系数
    end
end
% real函数取元素的实部
ProcessedPic=real(ifft2(ifftshift(s)));% ????进行二维傅里叶反变换转换为时域图??end

