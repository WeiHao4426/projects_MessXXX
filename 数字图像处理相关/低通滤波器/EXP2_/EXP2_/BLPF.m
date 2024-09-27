function ProcessedPic = BLPF(pic,d0,n)
I = imread(pic);
I=rgb2gray(I);
% ����fft2()���ڼ����ά����Ҷ��??% ����fftshift()�ǶԺ���fft2()������Ҷ�任��õ���Ƶ�׽���ƽ��,���任���ͼ��Ƶ�����ĴӾ����ԭ���Ƶ����������
% ����ά����Ҷ�任ǰһ��Ҫ�ú���im2double()��ԭʼͼ�������������uint8ת��Ϊdouble����
% �������Ϊunit8��������ֻ�ܱ�ʾ0-255������??�������ݽض�,��??���ִ�����
s=fftshift(fft2(im2double(I)));
[N1,N2]=size(s);%���ά����Ҷ�任��ͼ���??n1=round(N1/2);
n2=round(N2/2);
for i=1:N1      %˫��forѭ������Ƶ��??i,j)��Ƶ�����ĵľ���D(i,j)=sqrt((i-round(N1/2)^2+(j-round(N2/2)^2))
    for j=1:N2 
        distance=sqrt((i-n1)^2+(j-n2)^2);
        if distance==0 
            h=0; 
        else
            h=1/(1+(distance/d0)^(2*n));% ���ݰ�����˹��??�˲�����ʽΪ1/(1+[D(i,j)/D0]^2n)
        end
        s(i,j)=h*s(i,j);% Ƶ��ͼ������˲�����ϵ��
    end
end
% real����ȡԪ�ص�ʵ��
ProcessedPic=real(ifft2(ifftshift(s)));% ????���ж�ά����Ҷ���任ת��Ϊʱ��ͼ??end

