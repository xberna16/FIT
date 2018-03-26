/**
 * Kostra programu pro 3. projekt IZP 2015/16
 *
 * Jednoducha shlukova analyza
 * Complete linkage
 * http://is.muni.cz/th/172767/fi_b/5739129/web/web/clsrov.html
 */
#include <stdio.h>
#include <stdlib.h>
#include <assert.h>
#include <string.h>
#include <math.h> // sqrtf
#include <limits.h> // INT_MAX

/*****************************************************************
 * Ladici makra. Vypnout jejich efekt lze definici makra
 * NDEBUG, napr.:
 *   a) pri prekladu argumentem prekladaci -DNDEBUG
 *   b) v souboru (na radek pred #include <assert.h>
 *      #define NDEBUG
 */
#ifdef NDEBUG
#define debug(s)
#define dfmt(s, ...)
#define dint(i)
#define dfloat(f)
#else

// vypise ladici retezec
#define debug(s) printf("- %s\n", s)

// vypise formatovany ladici vystup - pouziti podobne jako printf
#define dfmt(s, ...) printf(" - "__FILE__":%u: "s"\n",__LINE__,__VA_ARGS__)

// vypise ladici informaci o promenne - pouziti dint(identifikator_promenne)
#define dint(i) printf(" - " __FILE__ ":%u: " #i " = %d\n", __LINE__, i)

// vypise ladici informaci o promenne typu float - pouziti
// dfloat(identifikator_promenne)
#define dfloat(f) printf(" - " __FILE__ ":%u: " #f " = %g\n", __LINE__, f)

#endif

/*****************************************************************
 * Deklarace potrebnych datovych typu:
 *
 * TYTO DEKLARACE NEMENTE
 *
 *   struct obj_t - struktura objektu: identifikator a souradnice
 *   struct cluster_t - shluk objektu:
 *      pocet objektu ve shluku,
 *      kapacita shluku (pocet objektu, pro ktere je rezervovano
 *          misto v poli),
 *      ukazatel na pole shluku.
 */

struct obj_t {
    int id;
    float x;
    float y;
};

struct cluster_t {
    int size;
    int capacity;
    struct obj_t *obj;
};

/*****************************************************************
 * Deklarace potrebnych funkci.
 *
 * PROTOTYPY FUNKCI NEMENTE
 *
 * IMPLEMENTUJTE POUZE FUNKCE NA MISTECH OZNACENYCH 'TODO'
 *
 */

/*
 Inicializace shluku 'c'. Alokuje pamet pro cap objektu (kapacitu).
 Ukazatel NULL u pole objektu znamena kapacitu 0.
*/
void init_cluster(struct cluster_t *c, int cap)
{
    assert(c != NULL);
    assert(cap >= 0);
	// TODO-DONE
	if (cap!=0)
	{
		c->obj = malloc(sizeof(struct obj_t) * cap);
	}
	else
	{
		c = NULL;
	}
	c->capacity=cap;
	c->size = 0;
}

/*
 Odstraneni vsech objektu shluku a inicializace na prazdny shluk.
 */
void clear_cluster(struct cluster_t *c)
{
    // TODO-DONE
	/*for(int i = 0; i<c->size;i++)
	{
		free(&c->obj[i]);
	}*/
	free(c->obj);
	c->capacity=0;
	c->size=0;
	c->obj =NULL;
}

/// Chunk of cluster objects. Value recommended for reallocation.
const int CLUSTER_CHUNK = 10;

/*
 Zmena kapacity shluku 'c' na kapacitu 'new_cap'.
 */
struct cluster_t *resize_cluster(struct cluster_t *c, int new_cap)
{
    // TUTO FUNKCI NEMENTE
    assert(c);
    assert(c->capacity >= 0);
    assert(new_cap >= 0);

    if (c->capacity >= new_cap)
        return c;

    size_t size = sizeof(struct obj_t) * new_cap;

    void *arr = realloc(c->obj, size);
    if (arr == NULL)
        return NULL;

    c->obj = arr;
    c->capacity = new_cap;
    return c;
}

/*
 Prida objekt 'obj' na konec shluku 'c'. Rozsiri shluk, pokud se do nej objekt
 nevejde.
 */
void append_cluster(struct cluster_t *c, struct obj_t obj)
{
    // TODO-DONE
	if(c->size==c->capacity)
	{
		c = resize_cluster(c,c->capacity+1);
	}
	//*(c->obj+c->size++) = obj;
	c->obj[c->size++]=obj;
}

/*
 Seradi objekty ve shluku 'c' vzestupne podle jejich identifikacniho cisla.
 */
void sort_cluster(struct cluster_t *c);

/*
 Do shluku 'c1' prida objekty 'c2'. Shluk 'c1' bude v pripade nutnosti rozsiren.
 Objekty ve shluku 'c1' budou serazny vzestupne podle identifikacniho cisla.
 Shluk 'c2' bude nezmenen.
 */
void merge_clusters(struct cluster_t *c1, struct cluster_t *c2)
{
	int i=0;
    assert(c1 != NULL);
    assert(c2 != NULL);
    // TODO-DONE
	while((c1->size + c2->size) > c1->capacity + i)
	{
		i++;
	}
	if (i>0)
	{
		c1 = resize_cluster(c1,c1->capacity +i);
	}
	for(int j = 0;j<c2->size;j++)
	{
		c1->obj[c1->size++]=c2->obj[j];
	}
	sort_cluster(c1);
}

/**********************************************************************/
/* Prace s polem shluku */

/*
 Odstrani shluk z pole shluku 'carr'. Pole shluku obsahuje 'narr' polozek
 (shluku). Shluk pro odstraneni se nachazi na indexu 'idx'. Funkce vraci novy
 pocet shluku v poli.
*/
int remove_cluster(struct cluster_t *carr, int narr, int idx)
{
    assert(idx < narr);
    assert(narr > 0);
    // TODO-DONE
	struct cluster_t tempClust;
	tempClust = carr[narr-1];
	carr[narr-1]=carr[idx];
	carr[idx]=tempClust;
	clear_cluster(&carr[narr-1]);
	if(narr==1)
	{
		return --narr;
	}
	carr = realloc(carr,sizeof(struct cluster_t)*--narr);
	if (carr==NULL)exit(0);
	assert(carr!=NULL);
	return narr;
}

/*
 Pocita Euklidovskou vzdalenost mezi dvema objekty.
 */
float obj_distance(struct obj_t *o1, struct obj_t *o2)
{
    assert(o1 != NULL);
    assert(o2 != NULL);

    // TODO-DONE
	float vysledek;
	vysledek = sqrt(abs((o1->x - o2->x)*(o1->x - o2->x)+(o1->y - o2->y)*(o1->y - o2->y)));
	return vysledek;
	
}

/*
 Pocita vzdalenost dvou shluku.
*/
float cluster_distance(struct cluster_t *c1, struct cluster_t *c2)
{
    assert(c1 != NULL);
    assert(c1->size > 0);
    assert(c2 != NULL);
    assert(c2->size > 0);

    // TODO-DONE
	double localMax=0, max=0, distance;
	for(int i = 0; i< c1->size;i++)
	{
		for(int j = 0; j< c2->size;j++)
		{
			distance = obj_distance(&(c1->obj[i]),&(c2->obj[j]));
			if (localMax<distance)
			{
				localMax = distance;
			}
		}
		if (localMax>max)
		{
			max = localMax;
		}
	}
	return max;
}

/*
 Funkce najde dva nejblizsi shluky. V poli shluku 'carr' o velikosti 'narr'
 hleda dva nejblizsi shluky. Nalezene shluky identifikuje jejich indexy v poli
 'carr'. Funkce nalezene shluky (indexy do pole 'carr') uklada do pameti na
 adresu 'c1' resp. 'c2'.
*/
void find_neighbours(struct cluster_t *carr, int narr, int *c1, int *c2)
{
    assert(narr > 0);

    // TODO-DONE
	double minLength=INFINITY, distance;
	for(int i = 0;i<narr;i++)
	{
		for(int j = i+1; j<narr;j++)
		{
			distance = cluster_distance(&(carr[i]),&(carr[j]));
			if (distance<minLength)
			{
				minLength=distance;
				*c1=i;
				*c2=j;
			}
		}
	}
}

// pomocna funkce pro razeni shluku
static int obj_sort_compar(const void *a, const void *b)
{
    // TUTO FUNKCI NEMENTE
    const struct obj_t *o1 = a;
    const struct obj_t *o2 = b;
    if (o1->id < o2->id) return -1;
    if (o1->id > o2->id) return 1;
    return 0;
}

/*
 Razeni objektu ve shluku vzestupne podle jejich identifikatoru.
*/
void sort_cluster(struct cluster_t *c)
{
    // TUTO FUNKCI NEMENTE
    qsort(c->obj, c->size, sizeof(struct obj_t), &obj_sort_compar);
}

/*
 Tisk shluku 'c' na stdout.
*/
void print_cluster(struct cluster_t *c)
{
    // TUTO FUNKCI NEMENTE
    for (int i = 0; i < c->size; i++)
    {
        if (i) putchar(' ');
        printf("%d[%g,%g]", c->obj[i].id, c->obj[i].x, c->obj[i].y);
    }
    putchar('\n');
}

/*
 Ze souboru 'filename' nacte objekty. Pro kazdy objekt vytvori shluk a ulozi
 jej do pole shluku. Alokuje prostor pro pole vsech shluku a ukazatel na prvni
 polozku pole (ukalazatel na prvni shluk v alokovanem poli) ulozi do pameti,
 kam se odkazuje parametr 'arr'. Funkce vraci pocet nactenych objektu (shluku).
 V pripade nejake chyby uklada do pameti, kam se odkazuje 'arr', hodnotu NULL.
*/
int load_clusters(char *filename, struct cluster_t **arr)
{
    assert(arr != NULL);

    // TODO-DONE
	char line[1000];
	int count, lineNr=1;
	FILE *fr = fopen(filename,"r");
	if (fr==NULL) {
		fprintf(stderr,"Soubor neexistuje");
		exit(0);
	}
		
	fscanf(fr,"%s",line);
	if(strncmp("count=",line,6)){
		fprintf(stderr,"Soubor musí mít formát count=N");
		exit(0);
	}
	count = (int)strtol(&line[6],NULL,10);
	if (count == 0)exit(0);
	*arr = (struct cluster_t*) malloc(count* sizeof(struct cluster_t));
	if(*arr==NULL) exit(5);
	while(lineNr++<count+1)
	{
		struct obj_t tempObj;
		init_cluster((&(*arr)[lineNr-2]),1);
		int a,b,c;
		if(fscanf(fr,"%d %d %d\n",&a,&b,&c)==EOF){
			fprintf(stderr,"Počet objektů nesouhlasí s count=N");
			exit(0);
		}
			
		tempObj.id=a;
		tempObj.x=b;
		tempObj.y=c;
		append_cluster(&(*arr)[lineNr-2],tempObj);
	}
	fclose(fr);
	return count;
}

/*
 Tisk pole shluku. Parametr 'carr' je ukazatel na prvni polozku (shluk).
 Tiskne se prvnich 'narr' shluku.
*/
void print_clusters(struct cluster_t *carr, int narr)
{
    printf("Clusters:\n");
    for (int i = 0; i < narr; i++)
    {
        printf("cluster %d: ", i);
        print_cluster(&carr[i]);
    }
}
void clear_clusters(struct cluster_t *carr, int* narr)
{
	for (int i = 0; i<*narr;i++)
	{
		clear_cluster(&carr[i]);
	}
	narr = 0;
}

int main(int argc, char *argv[])
{
    struct cluster_t *clusters;
	int param;
	if (argc==3)
	{
		param = (int) strtol(argv[2], NULL, 10);
		if (param <= 0)
		{
			fprintf(stderr,"Správné použití parametrů je : ./proj3 filename [N>0]");
			exit(0);
		}
			
	}
	else
	{
		if (argc == 2)
			param = 1;
		else
		{
			fprintf(stderr,"Správné použití parametrů je : ./proj3 filename [N>0]");
			exit(0);
		}
	}
	int n = load_clusters(argv[1],&clusters);	
	for(int i =0;n>param;i++)
	{
		int a,b;
		find_neighbours(clusters,n,&a,&b);
		merge_clusters(&clusters[a],&clusters[b]);
		n=remove_cluster(clusters,n,b);
	}
	print_clusters(&(*clusters),n);
	clear_clusters(clusters,&n);
	free(clusters);
}
