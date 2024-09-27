function DMax = GetDMax(pic)
I = imread(pic);
I=rgb2gray(I);
s=fftshift(fft2(I));
[a,b]=size(s);
a0=round(a/2);
b0=round(b/2);
DMax=sqrt((a-a0)^2+(b-b0)^2);
 