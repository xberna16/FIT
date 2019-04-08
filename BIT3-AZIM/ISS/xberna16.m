clear
[data,fs]=audioread("xberna16.wav");
reader = fopen('xberna16.txt','r');
xberna16 = fscanf(reader,'%d');
fclose(reader);
%% prvni ukol
fprintf('1.\n');
fs
delkaveVzorcich = length(data)
delkavs = length(data)/fs
pocetSymbolu = delkaveVzorcich/16

%% druhy ukol
out = zeros(pocetSymbolu,1);
fprintf('2.\n');
for i=0:pocetSymbolu-1
    if data((i*16)+8) > 0
        out(i+1,1)= 1;
    end 
end 
%whos data
vypocetSpravne = isequal(xberna16,out)
t=1:length(data)/100;t=t/fs;
f = figure('Visible', 'off');
plot(t,data(1:(length(data)/100)))
hold on
stem(t(8:16:end),out(1:20,1))
plot(t,zeros(320,1),'--k');
hold off
print(f, '-depsc', 'img/2.eps');
close(f);
%% treti ukol
fprintf('3.\n');
B = [0.0192 -0.0185 -0.0185 0.0192];
A = [1.0000 -2.8870 2.7997 -0.9113];
if isstable(B,A)
	fprintf('Filtr je stabilní.\n');
else
	fprintf('Filtr není stabilní.\n');
end
f = figure('Visible', 'off');
zplane(B, A);
hold on;
plot(t,zeros(320,1),'--k');
hold off;
xlabel('Reálná èást');
ylabel('Imaginární èást');
print(f, '-depsc', 'img/3.eps');
close(f);
%% ctvrty ukol
fprintf('4.\n');
t = (0 : 255) / 256 * fs / 2;
f = figure('Visible', 'off');
plot(t,abs(freqz(B,A,256)));
hold on;
plot(t,zeros(length(t),1),'--k');
hold off;
print(f, '-depsc', 'img/4.eps');
close(f);
fprintf('Dolní propus, mezní frekvence je 500Hz (vizuální analýza)\n');
%% paty ukol
fprintf('5.\n');
t = 0:(delkaveVzorcich/100)-1 /fs;
ss = filter(B,A,data);
f = figure('Visible', 'off');
plot(t,data(1:320));
hold on;
plot(t,zeros(320,1),'--k');
plot(t,ss(1:320));
hold off;
print(f, '-depsc', 'img/5.eps');
close(f);
%% sesty ukol
fprintf('6.\n');
sssdecoded = zeros(pocetSymbolu,1);
ssshift = ss(17:length(ss));%zpoždìní o 16 vzorkù od oka
for i=0:pocetSymbolu-2
    if ssshift((i*16)+8) > 0
        sssdecoded(i+1,1)= 1;
    end 
end 
f = figure('Visible', 'off');
plot(t,data(1:(length(data)/100)))
hold on;
plot(t,ssshift(1:320));
plot(t,zeros(320,1),'--k');
plot(t,ss(1:320));
stem(t(8:16:end),sssdecoded(1:20,1))
hold off;
print(f, '-depsc', 'img/6.eps');
close(f);
%% sedmy ukol
fprintf('7.\n');
xorout = xor(sssdecoded,out);
pocetChyb = 0;
for i=1:length(xorout)
    if xorout(i,1) == 1
        pocetChyb = pocetChyb + 1;
    end
end
pocetChyb
chybovost = pocetChyb/pocetSymbolu
%% osmy ukol
fprintf('8.\n');
fq = (0:delkaveVzorcich/2-1)/delkaveVzorcich*fs;
sfft=abs(fft(data));
ssfft=abs(fft(ss));
f = figure('Visible', 'off');
plot(fq,sfft(1:length(sfft)/2));
hold on;
plot(fq,ssfft(1:length(ssfft)/2));
hold off;
print(f, '-depsc', 'img/8.eps');
close(f);
%% devaty ukol 
fprintf('9.\n');
f = figure('Visible', 'off');
pomocna = unique(data);
musibyt1=0;
pravdepodobnost = histc(data,pomocna)/delkaveVzorcich;
for i=1:length(pravdepodobnost)
    musibyt1= musibyt1+ pravdepodobnost(i);
end
musibyt1
plot(pomocna,pravdepodobnost);
print(f, '-depsc', 'img/9.eps');
close(f);
%% desaty ukol
fprintf('10.\n');
f = figure('Visible', 'off');
R = zeros(100,1);
for i = -50:50
    sum=0;
    for j= 1:delkaveVzorcich
        if i+j>0 && i+j< delkaveVzorcich+1
            sum= sum+ data(j)* data(i+j);
        end
    end
    R(i+51) = sum/delkaveVzorcich;
end
plot(-50:50,R);
print(f, '-depsc', 'img/10.eps');
close(f);
%% jedenacty ukol
fprintf('11.\n');
fprintf('R[0] = %f, R[1]= %f, R[16]= %f\n', R(51),R(52),R(51+16));
%% dvanacty a trinacty ukol
fprintf('12. a 13.\n');
f = figure('Visible', 'off');
x= linspace(min(data),max(data),100);
[h,p,r] = hist2opt(data(1:delkaveVzorcich-1), data(2:delkaveVzorcich),x);
imagesc(x,x,p);
axis xy;
colorbar;
print(f, '-depsc', 'img/12.eps');
close(f);
%% ctrnacty ukol
fprintf('14.\n');
fprintf('Návrat hist2opt = %f, R[1] = %f, Jejich rozdíl je %f\n',r, R(52),r-R(52));
