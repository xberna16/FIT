/***********************************************************************
* FILENAME:proj2.c
* DESCRIPTION:Program pro iteraèní výpoèty. 2.Projekt VUT FIT
* AUTHOR:Hynek Bernard 
* DATE: 27.11.2016
***********************************************************************/
#include <stdio.h>
#include <math.h>
#include <string.h>
#include <stdlib.h>
#include <ctype.h>

void printHelp()
{
	printf("Program pro iteraèní výpoèty, pro správné spuštìní použijte\n./proj2 --log X N \n nebo \n./proj2 --pow X Y N \n");
}
double cfrac_log(double x, unsigned int n)
{
	//x=(1+((x+1)*(x-1)))/(1-((x+1)*(x-1)));
	x=(x-1)/(1+x);
	double vysledek=0;
	vysledek= ((n*2)-1)-(((n*n)*(x*x))/(((n+1)*(n+1))*(x*x)));
	//n--;
	while(n>=1){
		vysledek = ((n*2-1)-((n*n)*(x*x))/vysledek);
		n--;
	}
	return (2*x)/vysledek;

}
double taylor_log(double x, unsigned int n)
{
	double vysledek=0;
	int jmenovatel=1;
	double citatel=-(1-x);
	double y = 1-x;
	if(x>0.0 && x<=1.0)
	{
		x=1-x;
		while(n-->0)
		{
			vysledek -= citatel/jmenovatel;
			citatel*=y;
			jmenovatel++;
		}
		return -vysledek;
	}
	else
	{
		if(x<=0.0)
		{
			printHelp();
			return 666;
		}
		citatel = (x-1)/x;
		while(n-->0)
		{
			vysledek +=citatel/jmenovatel;
			citatel *=(x-1)/x;
			jmenovatel++;
		}
		return vysledek;
	}
	return 666.0;
}
double taylor_pow(double x, double y, unsigned int n)
{
	if(x==1||y==0)
		return 1.0;
	int zaklad=n;
	double vysledek = 1;
	int i = 1;
	//n--;
	double citatel = 1;
	double jmenovatel = 1;
	while (n>0){
		citatel *= y*taylor_log(x,zaklad);
		jmenovatel = jmenovatel*i;
		vysledek += citatel/jmenovatel;
		n--;
		i++;
	} 
	return vysledek;
}
double taylorcf_pow(double x, double y, unsigned int n)
{
	if(x==1||y==0)
		return 1.0;
	int zaklad=n;
	double vysledek = 1;
	int i = 1;
	n--;
	double citatel = 1;
	double jmenovatel = 1;
	while (n>0){
		citatel *= y*cfrac_log(x,zaklad);
		jmenovatel = jmenovatel*i;
		vysledek += citatel/jmenovatel;
		n--;
		i++;
	} 
	return vysledek;
}

int isNumber(char* string)
{
	int point = 0;
	if (*string == '-')
		string++;
	do {
		if (*string<'0' && *string > '9')
		{
			if(*string == '.'&& point ==0)
				point++;
			else
				return 0;
		}
	}while(*++string !='\0');
	return 1;
}
int main(int argc,char* argv[])
{
	double X = 0.0;
	double Y = 0.0;
	unsigned int N = 0;
	if(argc >1 && argc<6)
	{
		if(strcmp(argv[1],"--log")==0){
			if(argc==4)
			{
				if (isNumber(argv[2]))
				{
					X = strtod(argv[2],NULL);
					if (isNumber(argv[3]))
						N = strtol(argv[3],NULL,10);
				}
				if (N>0)
				{
					printf("       log(%g) = %.12g\n cfrac_log(%g) = %.12g\ntaylor_log(%g) = %.12g\n--------------------\n",X,log(X),X,cfrac_log(X,N),X,taylor_log(X,N));
				}
				else
				{
					printHelp();
				}
			}
			else
				printHelp();
		}
		else if(strcmp(argv[1],"--pow")==0){
			if(argc==5)
			{
				if (isNumber(argv[2])&& isNumber(argv[3]) && isNumber(argv[4]))
				{
				X = strtod(argv[2],NULL);
				Y = strtod(argv[3],NULL);
				N = strtol(argv[4],NULL,10);
				printf("       pow(%G,%g) = %.12g\n  taylor_pow(%g,%g) = %.12g\ntaylorcf_pow(%g,%g) = %.12g\n--------------------\n",X,Y,pow(X,Y),X,Y,taylor_pow(X,Y,N),X,Y,taylorcf_pow(X,Y,N));
				}
				else
					printHelp();
			}
			else
				printHelp();
		}
		else
			printHelp();
	}
	else
		printHelp();
}
