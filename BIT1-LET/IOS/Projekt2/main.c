/*Hynek Bernard 2017
*/
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <time.h>
#include <errno.h>
#include <fcntl.h>
#include <unistd.h>
#include <semaphore.h>
#include <signal.h>
#include <sys/ipc.h>
#include <sys/mman.h>
#include <sys/sem.h>
#include <sys/shm.h>
#include <sys/types.h>
#include <sys/wait.h>

int params[5]; 
FILE *outFile; //vystup
int val = 0, dread = 0, acnt = 0, ccnt = 0;
sem_t *buff, *endSem, *exitSem, *adult, *child, *prior;
pid_t process;
const char *ERR = "OK";
int cntr,adltCnt,chldCnt,remAdlts =0;
int *counter = NULL;
int *adultCount = NULL;
int *childCount = NULL;
int *remAdults= NULL;
int *remChilds= NULL;
int *adults = NULL;
int *childs = NULL;


void help();
int clearMem();
int getMem(int adultC, int childC);
int saveParams(int cntofparams, const char *locparams[]);
void sigcatch();
int closeSem()
{
    return 0;
}
void outPrint(char *proces, int poradi, char *akce)
{
  sem_wait(buff);

  if (strcmp (akce, "waiting") == 0)
  {
    fprintf(outFile, "%-3d     : %s %-3d   : %s : %-3d : %-3d\n", ++*counter, proces, poradi, akce, (*adultCount), (*childCount));
  }
  else
  {
    fprintf(outFile, "%-3d     : %s %-3d   : %s\n", ++*counter, proces, poradi, akce);
  }

  sem_post(buff);

  return;
}

int adultProc(int order)
{
    
    outPrint("A", order, "started");
    sem_wait(prior);
    /*printf("%d : A %d : started\n",++*counter,order);
    fflush(stdout);
    printf("%d : A %d : enter\n",++*counter,order);
    fflush(stdout);*/
    ++*adultCount;
    outPrint("A", order, "enter");
    sem_post(child);sem_post(child);sem_post(child);
    sem_post(prior);
    sem_post(adult);
    
    if (params[4]>0)
    usleep((rand()%params[4])*1000);

    sem_wait(prior);
    outPrint("A", order, "trying to leave");
    /*printf("%d : A %d : trying to leave\n",++*counter,order);
    fflush(stdout);*/
    //semafor sem nepustit nic dokud nezjistím možnost odchodu
    int first =1;
    while(*childCount > ((*(adultCount)-1)*3))
    {
        if (first)
        {
            /*printf("%d : A %d : waiting : %d : %d\n",++*counter,order,*adultCount,*childCount);
            fflush(stdout);*/
            outPrint("A", order, "waiting");
            sem_post(prior);
            first--;
        }
        sem_wait(adult);
    }
    outPrint("A", order, "leave");
    /*printf("%d : A %d : leave\n",++*counter,order);
    fflush(stdout);*/
    --*adultCount;
    if(first)
    {
        sem_post(prior);
    }
    --*remAdults;
    sem_post(endSem);
    sem_wait(exitSem);
    /*printf("%d : A %d : finished\n",++*counter, order);
    fflush(stdout);*/
    outPrint("A", order, "finished");
    clearMem();
    exit(0);
}

int childProc(int order)
{
    sem_wait(prior);
    outPrint("C", order, "started");
    /*printf("%d : C %d :\t started\n",++*counter,order);
    fflush(stdout);*/
    int first =1;
    fflush(stdout);
    while (*childCount+1> (*adultCount*3))
    {
        if(*remAdults==0)
            break;
        if (first)
        {
            outPrint("C", order, "waiting");
            /*printf("%d :\t C %d : waiting : %d : %d\n",++*counter,order,*adultCount,*childCount);
            fflush(stdout);*/
            sem_post(prior);
            first--;
        }
        sem_wait(child);
    }
    if (first)
        sem_post(prior);

    sem_wait(prior);
    outPrint("C", order, "enter");
    /*printf("%d : C %d : enter\n",++*counter,order);
    fflush(stdout);*/
    ++*childCount;
    sem_post(prior);
    if (params[5]>0)
    usleep((rand()%params[5])*1000);
    sem_wait(prior);
    outPrint("C", order, "trying to leave");
    --*childCount;
    outPrint("C", order, "leave");
    --*remChilds;
    sem_post(prior);


    sem_post(child);
    sem_post(adult);


    sem_post(endSem);
    sem_wait(exitSem);
    /*printf("%d : C %d : finished\n", ++*counter, order);*/
    outPrint("C", order, "finished");
    clearMem();
    exit(0);
}

int main(int argc, char const *argv[])
{
    if(saveParams(argc, argv) == 1)
    {
        fprintf(stderr, "%s \n", ERR);
        help();
        return 1;
    }
    pid_t adultHandler, childHandler; 
    signal(SIGTERM, sigcatch);

    //Alokace a inicializace paměti
    if (getMem(params[0],params[1]) == 1)
    {
        fprintf(stderr, "Alokace pameti se nezdarila");
        return 2;
    }

    
    *remAdults=params[0];
    *remChilds=params[1];
    if (( adultHandler=fork()) < 0)
    {
      fprintf(stderr, "Nepodarilo se vytvorit adultHandler\n");
      process = adultHandler;
      clearMem();
      return 2;
    }
    else if (adultHandler == 0)
    {
        for (int i = 0; i < params[0]; ++i)
        {
            /*ADULTI*/
            if (params[2]>0)
                usleep((rand()%params[2])*1000);
            adults[i] = fork();
            if (adults[i] ==0)
            {
                adultProc(i+1);
            }
        }
        sem_post(endSem);
        sem_wait(exitSem);
        clearMem();
        return 0;
    }
    if (( childHandler=fork()) < 0) 
    {
      fprintf(stderr, "Nepodarilo se vytvorit childHandler\n");
      process = childHandler;
      clearMem();
      return 2;
    }
    else if (childHandler == 0)
    {
        for (int i = 0; i < params[1]; ++i)
        {
            /*DĚTI*/
            if (params[3]>0)
                usleep((rand()%params[3])*1000);
            childs[i] = fork();
            if (childs[i] ==0)
            {
                childProc(i+1);
            }
        }
        sem_post(endSem);
        sem_wait(exitSem);
        clearMem();
        return 0;
    }

    for (int i = 0; i < (params[0] + params[1])+2; i++)
    {
        sem_wait(endSem);
    }
    for(int i = 0; i < (params[0] + params[1])+3; i++)
    {
        sem_post(exitSem);
    }
    if(*remAdults == 0 && *childCount == 0 && *adultCount == 0 && *remChilds == 0)
    {
        clearMem();
    }
    else
    {
        printf("Chyba pri uzavirani procesu remAdults=%d childCount=%d adultCount=%d remChilds=%d\n",
            *remAdults,*childCount,*adultCount,*remChilds);
    }

    return 0;
}

void sigcatch(int signal)
{
    signal+=1;
    closeSem();
    clearMem();
    kill(process, SIGTERM);
    kill(getppid(), SIGTERM);
    exit(EXIT_FAILURE);
}

int getMem( int adultC, int childC)
{
    
    int result=0;
    if((prior = mmap(NULL, sizeof(sem_t), PROT_READ | PROT_WRITE, MAP_SHARED | MAP_ANONYMOUS, -1, 0)) == MAP_FAILED)
        result=1;
    if((adult = mmap(NULL, sizeof(sem_t), PROT_READ | PROT_WRITE, MAP_SHARED | MAP_ANONYMOUS, -1, 0)) == MAP_FAILED)
        result=1;
    if((child = mmap(NULL, sizeof(sem_t), PROT_READ | PROT_WRITE, MAP_SHARED | MAP_ANONYMOUS, -1, 0)) == MAP_FAILED)
        result=1;
    if((buff = mmap(NULL, sizeof(sem_t), PROT_READ | PROT_WRITE, MAP_SHARED | MAP_ANONYMOUS, -1, 0)) == MAP_FAILED)
        result=1;
    if((endSem = mmap(NULL, sizeof(sem_t), PROT_READ | PROT_WRITE, MAP_SHARED | MAP_ANONYMOUS, -1, 0)) == MAP_FAILED)
        result=1;
    if((exitSem = mmap(NULL, sizeof(sem_t), PROT_READ | PROT_WRITE, MAP_SHARED | MAP_ANONYMOUS, -1, 0)) == MAP_FAILED)
        result=1;
    
    
    if((counter = mmap(NULL, sizeof(int), PROT_READ | PROT_WRITE, MAP_SHARED | MAP_ANONYMOUS, -1, 0)) == MAP_FAILED)
        result=1;
    *counter = 0;
    if((adultCount = mmap(NULL, sizeof(int), PROT_READ | PROT_WRITE, MAP_SHARED | MAP_ANONYMOUS, -1, 0)) == MAP_FAILED)
        result=1;
    if((childCount = mmap(NULL, sizeof(int), PROT_READ | PROT_WRITE, MAP_SHARED | MAP_ANONYMOUS, -1, 0)) == MAP_FAILED)
        result=1;
    if((remAdults = mmap(NULL, sizeof(int), PROT_READ | PROT_WRITE, MAP_SHARED | MAP_ANONYMOUS, -1, 0)) == MAP_FAILED)
        result=1;
    if((remChilds = mmap(NULL, sizeof(int), PROT_READ | PROT_WRITE, MAP_SHARED | MAP_ANONYMOUS, -1, 0)) == MAP_FAILED)
        result=1;
    if((adults = mmap(NULL, sizeof(pid_t) * adultC, PROT_READ | PROT_WRITE, MAP_SHARED | MAP_ANONYMOUS, -1, 0)) == MAP_FAILED)
        result=1;
    if((childs = mmap(NULL, sizeof(pid_t) * childC, PROT_READ | PROT_WRITE, MAP_SHARED | MAP_ANONYMOUS, -1, 0)) == MAP_FAILED)
        result=1;


    sem_init(prior, 1, 1);
    sem_init(child, 1, 0);
    sem_init(buff, 1, 1);
    sem_init(adult, 1, 1);
    sem_init(endSem, 1, 0);
    sem_init(exitSem, 1, 0);
    outFile = fopen("proj2.out", "w+");
    setbuf(outFile,NULL);
    return result;

}

int saveParams(int cntofparams, const char *locparams[])
{
    if (cntofparams != 7)
    {
        ERR = "Špatný počet parametrů\n";
        return 1;
    }
    int tmp = 1;
    while(tmp <= 6)
    {
        params[tmp-1] = strtol(locparams[tmp], NULL, 10);

        if(((params[tmp - 1] < 0) || params[tmp -1] > 5000))
        {
            ERR = "Spatny parametr\n";
            return 1;
        }
        tmp++;
    }
    return 0;
}

void help()
{
	fprintf(stderr, "A : pocet 'adult' procesu\nC : pocet 'child' procesu\n"
	"AGT : maximalni doba generace procesu 'adult'\n"
    "CGT : maximalni doba generace procesu 'child'\n"
	"AWT : max. cas, po kterou proces 'adult' simuluje cinnost v centru\n"
	"CWT : max. cas, po kterou proces 'child' simuluje cinnost v centru\n");
}

int clearMem()
{
  int test = 0, result = 0;

  if ((test = sem_destroy(adult)) == -1)
    {
    result = 1;
    }
  if ((test = sem_destroy(child)) == -1)
    {
    result = 1;
    }
  if ((test = sem_destroy(buff)) == -1)
    {
    result = 1;
    }
  if ((test = sem_destroy(prior)) == -1)
    {
        result = 1;
    }

  if (outFile != stdout)
  {
    fclose(outFile);
  }
  
  return result;    
}