//Hynek Bernard, xberna16 IMP-2018

/* Header file with all the essential definitions for a given type of MCU */
#include "MK60D10.h"

/* Macros for bit-level registers manipulation */
#define GPIO_PIN_MASK	0x1Fu
#define GPIO_PIN(x)		(((1)<<(x & GPIO_PIN_MASK)))

/* Constants specifying delay loop duration */
#define	DELAY_T 200000

/* Mapping of LEDs and buttons to specific port pins: */
#define LED_D9  0x20 // Port B, bit 5
#define LED_D12 0x4 // Port B, bit 2
#define LED_D10 0x10 // Port B, bit 4

#define BTN_SW3 0x1000 // Port E, bit 12
#define BTN_SW5 0x4000000 // Port E, bit 26

int isPaused = 1;//program se zapíná pozastavený
int entropy = 0;//s entropy se nepoèítá, musí se zapnout tlaèítkem SW5

void delay(long long bound) {

	long long i;
	for (i = 0; i < bound; i++)
		__NOP();
		;
}
void PORTE_IRQHandler(void) {
	delay(500000);//adekvátní èas pro stisk tlaèítka
	if (PORTE_ISFR == BTN_SW3 && (GPIOE_PDIR & BTN_SW3)) {//pozastavení generace
		isPaused = !isPaused;
		GPIOB_PDOR ^= LED_D12;
	}
	if (PORTE_ISFR == BTN_SW5 && (GPIOE_PDIR & BTN_SW5)) {//povolení plnìní entropy registru shiftováním pøedchozího výsledku
		entropy = !entropy;
		GPIOB_PDOR ^= LED_D10;
	}
	PORTE_ISFR = ~0;
}
void MCUInit(void) {
	MCG_C4 |= ( MCG_C4_DMX32_MASK | MCG_C4_DRST_DRS(0x01));
	SIM_CLKDIV1 |= SIM_CLKDIV1_OUTDIV1(0x00);
	WDOG_STCTRLH &= ~WDOG_STCTRLH_WDOGEN_MASK;
}
void UARTInit(void) {

	UART5->C2 &= ~(0x8 | 0x4);//nulování transmitteru a receiveru
	UART5->BDH = 0x00;
	UART5->BDL = 0x1A;//rychlost 115 200Bd, 1stop
	UART5->C1 = 0x0;
	UART5->C3 = 0x0;
	UART5->MA1 = 0x0;
	UART5->MA2 = 0x0;
	UART5->BDH = 0x0;
	UART5->C4 = 0xF;//oversampling 16, no match address

	UART5->S2;
	UART5->C2 = 0x8;//zapneme vysílaè

}

void PortsInit(void) {
	/* Turn on all port clocks */
	SIM->SCGC5 = SIM_SCGC5_PORTB_MASK | SIM_SCGC5_PORTE_MASK;
	SIM->SCGC1 = SIM_SCGC1_UART5_MASK;
	SIM->SCGC3 = SIM_SCGC3_RNGA_MASK;

	/* Set corresponding PTB pins (connected to LED's) for GPIO functionality */
	PORTB->PCR[5] = PORT_PCR_MUX(0x01); // D9
	PORTB->PCR[4] = PORT_PCR_MUX(0x01); // D10
	PORTB->PCR[2] = PORT_PCR_MUX(0x01); // D12

	PORTE->PCR[12] = PORT_PCR_MUX(0x01); // SW3
	PORTE->PCR[12] = ( PORT_PCR_ISF(0x01) /* Nuluj ISF (Interrupt Status Flag) */
	| PORT_PCR_IRQC(0x0A) /* Interrupt enable on failing edge */
	| PORT_PCR_MUX(0x01) /* Pin Mux Control to GPIO */
	| PORT_PCR_PE(0x01) /* Pull resistor enable... */
	| PORT_PCR_PS(0x01));/* ...select Pull-Up */
	PORTE->PCR[26] = PORT_PCR_MUX(0x01); // SW5
	PORTE->PCR[26] = ( PORT_PCR_ISF(0x01) /* Nuluj ISF (Interrupt Status Flag) */
	| PORT_PCR_IRQC(0x0A) /* Interrupt enable on failing edge */
	| PORT_PCR_MUX(0x01) /* Pin Mux Control to GPIO */
	| PORT_PCR_PE(0x01) /* Pull resistor enable... */
	| PORT_PCR_PS(0x01));/* ...select Pull-Up */

	/* Change corresponding PTB port pins as outputs */
	PTB->PDDR = GPIO_PDDR_PDD(0x3C);
	PTB->PDOR |= GPIO_PDOR_PDO(0x3C); // turn all LEDs OFF

	PORTE->PCR[8] = PORT_PCR_MUX(0x03); //UART5 Tx

	NVIC_ClearPendingIRQ(PORTE_IRQn);//vyèistit pøerušení

	NVIC_EnableIRQ(PORTE_IRQn);//povolit pøerušení portE

}
void printInt(unsigned int nrToPrint) {
	if (entropy) {
		RNG->ER = nrToPrint << (nrToPrint % 10) + ((nrToPrint % 100) % 10);
		RNG->ER = nrToPrint << ((nrToPrint % 1000) % 100);
		RNG->ER = nrToPrint << (((nrToPrint % 1000) % 100) % 10);
		RNG->ER = nrToPrint << ((((nrToPrint % 10000) % 1000) % 100) % 10);
	}

	char output[11];
	int pos = 0;
	while (nrToPrint > 9) {
		output[pos++] = nrToPrint % 10 + 0x30;
		nrToPrint /= 10;
	}
	while (!(UART5->S1 & 0x80) && !(UART5->S1 & 0x40))
		;
	UART5->D = nrToPrint + 0x30;
	if (pos == 0) {
		return;
	}
	pos--;
	for (; pos > -1; pos--) {
		while (!(UART5->S1 & 0x80) && !(UART5->S1 & 0x40))
			;
		UART5->D = output[pos];
	}
}

void RNGInit() {
	RNG_CR = 0xB;
}

int main(void) {
	MCUInit();
	PortsInit();
	UARTInit();
	RNGInit();
	delay(50000);
	while (1) {

		while (isPaused)
			;
		printInt(RNG->OR);
		GPIOB_PDOR ^= LED_D9;
		while (!(UART5->S1 & 0x80) && !(UART5->S1 & 0x40))
			;
		UART5->D = ' ';
		GPIOB_PDOR ^= LED_D9;

	}

	return 0;
}
