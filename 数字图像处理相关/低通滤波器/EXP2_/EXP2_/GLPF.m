function ProcessedPic = GLPF(pic,d0)
I = imread(pic);
I=rgb2gray(I);
% ����fft2()���ڼ����ά����Ҷ�任
% ����fftshift()�ǶԺ���fft2()������Ҷ�任��õ���Ƶ�׽���ƽ��,���任���ͼ��Ƶ�����ĴӾ����ԭ���Ƶ����������
% ����ά����Ҷ�任ǰһ��Ҫ�ú���im2double()��ԭʼͼ�������������uint8ת��Ϊdouble����
% �������Ϊunit8��������ֻ�ܱ�ʾ0-255���������������ݽض�,�������ִ�����
s=fftshift(fft2(im2double(I)));
[a,b]=size(s);
a0=round(a/2);
b0=round(b/2);
for i=1:a
    for j=1:b
        distance=sqrt((i-a0)^2+(j-b0)^2);    % ���ݸ�˹��ͨ�˲�����ʽH(u,v)=e^-[D^2(u,v)/2*D0^2] 
        h=exp(-(distance*distance)/(2*(d0^2))); % exp��ʾ��eΪ�׵�ָ������
        s(i,j)=h*s(i,j);% Ƶ��ͼ������˲�����ϵ��
    end
end
ProcessedPic=real(ifft2(ifftshift(s)));% �����ж�ά����Ҷ���任ת��Ϊʱ��ͼ��
end
