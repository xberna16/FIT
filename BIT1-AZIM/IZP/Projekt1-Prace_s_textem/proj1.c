/***********************************************************************
* FILENAME:proj1.c
* DESCRIPTION:Program pro práci s textem. 1.Projekt VUT FIT
* AUTHOR:Hynek Bernard 
* DATE: 25.10.2016
***********************************************************************/
#include <stdio.h>
#include <stdlib.h>
#include <ctype.h>

int compareStr(char *s1,char *s2) //porovna stringy (strcmp) --Funkcni
{
char *p1 = (char *)s1;
char *p2 = (char *)s2;
        while(*p1 != '\0'){
			if (*p2 == '\0')
				return 1;
			if (*p2<*p1)
				return -1;
			if (*p1<*p2)
				return 1;
			p1++;
			p2++;
		}
		if (*p2 != '\0')
			return -1;
		return 0;
}
int isNumber(char number[])
{
	int retNum = 0;
	for (int i = 0; number[i]!= '\0';i++)
	{
		if(number[i] > '9' || number[i] < '0')
			return 0;
		if(retNum == 0){
			retNum = number[i]-'0';
		}
		else{
			retNum = retNum * 10 +number[i]-'0';
		}
	}
	return retNum;
}
void printHelp()// Vypise napovedu a ukončí program
{
	printf("Program pro praci s textem.\nArgumenty ktere lze pouzit jsou: [-s M] [-n N] | -x | -S 0<N<200| -r |\n");
	exit(0);
}
int takeParams(int argc, char* argv[], int *N, int *M) //zpracuje parametry programu
{
	if (argc ==1)//pokud nejsou argumenty
		return 1;
	int done=0;//počet platných argumentů- nesmí být větší jako 1
	int retParam=1;//to co funkce vrátí
	for(int i=1; i<argc;i++){
		if (isNumber(argv[i])>0){
		printHelp();
		}
		if (compareStr(argv[i],"-s")==0){
			if(i+1<argc)
				if(isNumber(argv[++i]))
					*M = isNumber(argv[i]);
				else
					printHelp();
			else{
				printHelp();
			}
			retParam = 1;
			done++;
			continue;
		}
		if (compareStr(argv[i],"-n")==0){
			if(i+1<argc)
				if(isNumber(argv[++i]))
					*N = isNumber(argv[i]);
				else
					printHelp();
			else{
				printHelp();
			}
			retParam = 1;
			done++;
			continue;
		}
		if (compareStr(argv[i],"-x")==0){
			retParam = 2;
			done++;
			continue;
		}
		if (compareStr(argv[i],"-S")==0){
			if(i+1<argc){
				*N = isNumber(argv[++i]);
				if(*N>=200 || *N<=0)
					printHelp();
			}
			else{
				printHelp();
			}
			done++;
			retParam = 3;
			continue;
		}
		if (compareStr(argv[i],"-r")==0){
			done++;
			retParam = 4;
			continue;
		}
		printHelp();
	}
	if ((done>1 && retParam !=1) || (done>2 && retParam ==1))
		printHelp();
	return retParam;
}
void clearArray(char arrayToClear[])
{
	for(unsigned int i = 0; i< ((sizeof(*arrayToClear))/(sizeof(char)));i++)
	{
		arrayToClear[i]=0;
	}
}

int main(int argc, char* argv[])
        {
			static int maxChars=16;
			int N=0; //N ze zadani
			int M=0; //M ze zadani
			int addressIn=0;
			char printc=0;
			char c;
			char d='x';
			char inputChars[maxChars];
			int endOfStream=0;
			int byteCount=0;
			int parameters=takeParams(argc,argv,&N,&M);//zjisti kombinaci parametru
			switch(parameters)
			{
				case 1:					
					while (M>0)
					{
						c=getchar();
						M--;
						addressIn++;
						if(c==EOF)
							exit(0);
					}
					while(endOfStream==0)
					{
						clearArray(inputChars);
						for(int i = 0;i<maxChars;i++)//naplní pole znaky ze vstupu
						{
							c=getchar();
							if(i==0)
								printf("%08x  ",addressIn);//vytiskne adresu prvního bajtu
							if(c!=EOF && endOfStream==0){//přičte adresu a vytiskne znak v hexa
								inputChars[i]=c;
								byteCount++;
								addressIn++;
								printf("%02x ",c);
								if(i+1 == (maxChars/2))//vytiskne mezeru v polovině
									printf(" ");
								if (byteCount== N)
								{
									endOfStream=1;
								}
							}
							else
							{
								printf("   ");
								if(i+1 == (maxChars/2))
									printf(" ");
								inputChars[i]='\0';
								endOfStream=1;
							}
						}
						printf(" |");
						for(int i = 0;i<maxChars;i++)//vytiskne poslední část výstupu v ASCII hodnotách
						{
							if(isprint(inputChars[i]))
								printf("%c",inputChars[i]);
							else
							{
								if(inputChars[i]!='\0')
									printf(".");
								else
									printf(" ");
							}
								
						}
						printf("|\n");
					}
				break;
				
				case 2:
					while((c=getchar())!=EOF)//tiskne dokud není konec vstupu
						printf("%02x",c);
					printf("\n");
				break;
				case 3:
					if (N<=0||N>=200)
						printHelp();
					while(endOfStream ==0)
					{
						for(int i = 0;i<N;i++)//Tiskne dokud nevytiskne délku znaků N
						{
							c=getchar();
							if(c!=EOF && endOfStream==0){
								if(isprint(c) || isblank(c)){
									printf("%c",c);
								}
								else{
									for(int j = 0;j<i;j++)//pokud je nesplněná podmínka tisku, umaže počet znaků který se řídí proměnnou i
										printf("\b");
									break;
								}
							}
							else
							{
								endOfStream=1;
								break;
							}
						}
						if (endOfStream==1)
							break;
						if(isprint(c) || isblank(c)){
						}
						else
							continue;						
						while((c=getchar()) !=EOF && endOfStream==0) // tiskne dokud je splněna podmínka tisku
						{
							if(isprint(c) || isblank(c))
									printf("%c",c);
							else
							{
								printf("\n"); //vytiskne odřádkování a začne další kolo prvního cyklu
								break;
							}
						}
						if(c==EOF)
							endOfStream=1;
					}
				break;
				case 4:
					while((c=getchar())!=EOF){
						if(isblank(c))
							continue;
						while((d=getchar())!=EOF){//plní dokud není použitelný znak
							if(isblank(d))
								continue;
							else
								break;
							}
						if (d==EOF || d==10)
						{
							break;
						}
						if(c-'0'<=9 && c-'0'>=0)// pokud je znak číslo, počítá s číslem
							printc= (c-'0')*16;
						else{//zjistí jestli je písmeno malé, nebo velké a započítá
							printc= c-'A'>=6? (c-'a'+10)*16:(c-'A'+10)*16;
						}
						if ((printc/16)>16)
							printHelp();
						if(d-'0'<=9 && d-'0'>=0)
							printc+= d-'0';
						else{
							printc+= d-'A'>=6? d-'a'+10:d-'A'+10;
						}
						printf("%c",printc);
					}
					printf("\n");
				break;
			}
			
        return 0;
        }


