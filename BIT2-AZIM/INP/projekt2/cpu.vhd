-- cpu.vhd: Simple 8-bit CPU (BrainLove interpreter)
-- Copyright (C) 2017 Brno University of Technology,
--                    Faculty of Information Technology
-- Author(s): Hynek Bernard (xberna16)
--

library ieee;
use ieee.std_logic_1164.all;
use ieee.std_logic_arith.all;
use ieee.std_logic_unsigned.all;

-- ----------------------------------------------------------------------------
--                        Entity declaration
-- ----------------------------------------------------------------------------
entity cpu is
 port (
   CLK   : in std_logic;  -- hodinovy signal
   RESET : in std_logic;  -- asynchronni reset procesoru
   EN    : in std_logic;  -- povoleni cinnosti procesoru
 
   -- synchronni pamet ROM
   CODE_ADDR : out std_logic_vector(11 downto 0); -- adresa do pameti
   CODE_DATA : in std_logic_vector(7 downto 0);   -- CODE_DATA <- rom[CODE_ADDR] pokud CODE_EN='1'
   CODE_EN   : out std_logic;                     -- povoleni cinnosti
   
   -- synchronni pamet RAM
   DATA_ADDR  : out std_logic_vector(9 downto 0); -- adresa do pameti
   DATA_WDATA : out std_logic_vector(7 downto 0); -- mem[DATA_ADDR] <- DATA_WDATA pokud DATA_EN='1'
   DATA_RDATA : in std_logic_vector(7 downto 0);  -- DATA_RDATA <- ram[DATA_ADDR] pokud DATA_EN='1'
   DATA_RDWR  : out std_logic;                    -- cteni z pameti (DATA_RDWR='0') / zapis do pameti (DATA_RDWR='1')
   DATA_EN    : out std_logic;                    -- povoleni cinnosti
   
   -- vstupni port
   IN_DATA   : in std_logic_vector(7 downto 0);   -- IN_DATA obsahuje stisknuty znak klavesnice pokud IN_VLD='1' a IN_REQ='1'
   IN_VLD    : in std_logic;                      -- data platna pokud IN_VLD='1'
   IN_REQ    : out std_logic;                     -- pozadavek na vstup dat z klavesnice
   
   -- vystupni port
   OUT_DATA : out  std_logic_vector(7 downto 0);  -- zapisovana data
   OUT_BUSY : in std_logic;                       -- pokud OUT_BUSY='1', LCD je zaneprazdnen, nelze zapisovat,  OUT_WE musi byt '0'
   OUT_WE   : out std_logic                       -- LCD <- OUT_DATA pokud OUT_WE='1' a OUT_BUSY='0'
 );
end cpu;


-- ----------------------------------------------------------------------------
--                      Architecture declaration
-- ----------------------------------------------------------------------------
architecture behavioral of cpu is

 -- zde dopiste potrebne deklarace signalu
	signal PC	: std_logic_vector(11 downto 0);
	signal PTR	: std_logic_vector(9 downto 0);
	signal CNT: std_logic_vector(7 downto 0);
	type fsm_state is (
	sbegin,
	sdecode,
	sfetch0,
	sfetch1,
	shalt,
	snext,
	sprev,
	sinc0,
	sinc1,
	sinc2,
	sdec0,
	sdec1,
	sdec2,
	sprint,
	sgetc0,
	sgetc1,
	swhile0,
	swhile1,
	swhile2,
	swhile3,
	swhileend0,
	swhileend1,
	swhileend2,
	swhileend3,
	sbreak,
	ssav,
	sret,
	sskip
	);
	signal nstate: fsm_state;
	signal pstate: fsm_state;
	
	signal PCinc: std_logic;
	signal PCdec: std_logic;
	
	signal PTRinc: std_logic;
	signal PTRdec: std_logic;
	
	signal mp1: std_logic_vector(1 downto 0);
	
	
	signal CNTinc: std_logic;
	signal CNTdec: std_logic;
	
begin

 -- zde dopiste vlastni VHDL kod
 
 
	--CNT
	
	CNTBEH: process(RESET,CLK)
	begin
		if(RESET = '1') then
			CNT <= ("00000000");
		elsif (CLK'event) and (CLK='1') then
			if(CNTinc = '1') then
				CNT <= CNT + '1';
			elsif (CNTdec = '1') then
				CNT <= CNT - '1';
			end if;
		end if;
	end process;
 
 
	--PC
	
	PCBEH: process(RESET,CLK)
	begin
		if(RESET = '1') then
			PC <= ("000000000000");
		elsif (CLK'event) and (CLK='1') then
			if(PCinc = '1') then
				PC <= PC + '1';
			elsif(PCdec = '1') then
				PC <= PC - '1';
			end if;		
		end if;
	CODE_ADDR <= PC;
	end process;
	
	--PTR
	
	PTRBEH: process(RESET,CLK)
	begin
		if(RESET = '1') then
			PTR <= ("0000000000");
		elsif (CLK'event) and (CLK='1') then
			if(PTRinc = '1') then
				PTR <= PTR + '1';
			elsif (PTRdec = '1') then
				PTR <= PTR - '1';
			end if;
		end if;
	DATA_ADDR <= PTR;
	end process;
 
	--MP1
	
	MP1BEH: process(RESET,CLK, mp1)
	begin
		case mp1 is
			when "00" => DATA_WDATA <= IN_DATA;
			when "01" => DATA_WDATA <= DATA_RDATA + 1;
			when "10" => DATA_WDATA <= DATA_RDATA - 1;
			when others => DATA_WDATA <= (others => '0');
		end case;
	end process;
			
 
	--pstate změna
	fsm_pstate: process (RESET,CLK)
	begin
		if (RESET='1') then
			pstate <= sbegin;
		elsif (CLK'event) and (CLK='1') then
			if (EN = '1') then
				pstate<=nstate;
			end if;
		end if;
	end process;
	
	fsm_nstate: process (pstate, CODE_DATA, DATA_RDATA, IN_DATA, IN_VLD, OUT_BUSY)
	begin
	--init
	DATA_EN <= '0';
	DATA_RDWR <= '0';
	CODE_EN <= '0';
	IN_REQ <= '0';
	OUT_WE <= '0';
	PCinc <= '0';
	PCdec <= '0';
	PTRinc <= '0';
	PTRdec <= '0';
	CNTinc <= '0';
	CNTdec <= '0';
	--PC <= "000000000000";
	--PTR <= "0000000000";
	mp1 <= "11";
	
		
		case pstate is
			--SBEGIN
			when sbegin =>
				nstate <= sfetch0;
			--SFETCH
			when sfetch0 =>
				CODE_EN <= '1';
				nstate <= sfetch1;
			when sfetch1 =>
				CODE_EN <= '1';
				nstate <= sdecode;
			--SDECODE
			when sdecode =>
				case (CODE_DATA) is
					when X"3E" => -->
					nstate <= snext;
					when X"3C" =>--<
					nstate <= sprev;
					when X"2B" =>--+
					nstate <= sinc0;
					when X"2D" =>-- -
					nstate <= sdec0;
					when X"2E" =>--.
					nstate <= sprint;
					when X"2C" =>--,
					nstate <= sgetc0;
					when X"5B" =>--[
					nstate <= swhile0;
					when X"5D" =>--]
					nstate <= swhileend0;
					when X"7E" => --~
					nstate <= sbreak;
					when X"00" =>--end
					nstate <=shalt;
					when others =>
					nstate <= sskip;
				end case;
			--SINC	
			when sinc0 =>
				PCinc <= '1';
				DATA_EN <= '1';
				DATA_RDWR <= '0';
				nstate <= sinc1;
			when sinc1 =>
				DATA_EN <= '1';
				nstate <= sinc2;
			when sinc2 =>
				DATA_EN <= '1';
				DATA_RDWR <= '1';
				mp1 <= "01";
				nstate <=sfetch0;
			--SDEC
			when sdec0 =>
				PCinc <= '1';
				DATA_EN <= '1';
				DATA_RDWR <= '0';
				nstate <= sdec1;
			when sdec1 =>
				DATA_EN <= '1';
				nstate <=sdec2;
			
			when sdec2 =>
				DATA_EN <= '1';
				DATA_RDWR <= '1';
				mp1 <= "10";
				nstate <=sfetch0;
			--SSKIP	
			when sskip =>
				PCinc <= '1';
				nstate <=sfetch0;
			--SNEXT
			when snext =>
				PCinc <= '1';
				PTRinc <= '1';
				DATA_EN <= '1';
				nstate <= sfetch0;
			--SPREV
			when sprev =>
				PCinc <= '1';
				PTRdec <= '1';
				DATA_EN <= '1';
				nstate <= sfetch0;
			--SPRINT
			when sprint =>
				DATA_EN <= '1';
				if(OUT_BUSY = '0') then
					nstate <= sfetch0;
					OUT_DATA <= DATA_RDATA;
					DATA_RDWR <= '0';
					PCinc <= '1';
					OUT_WE <= '1';
				else
					nstate <= sprint;
				end if;
			--SGETC
			when sgetc0 =>
				IN_REQ <= '1';
				--DATA_EN <= '1';
				if(IN_VLD = '1') then
					PCinc <='1';
					DATA_EN <= '1';
					--DATA_RDWR <= '1';
					--DATA_WDATA <= IN_DATA;
					nstate <= sgetc1;
				else
					nstate <=sgetc0;
				end if;
			when sgetc1 =>
			DATA_EN <= '1';
			DATA_RDWR <= '1';
			mp1 <= "00";
			nstate <=sfetch0;
			
			--SWHILE
			when swhile0 =>
			PCinc <='1';
			CODE_EN <= '1';
			DATA_EN <= '1';
			nstate <=swhile1;
			
			when swhile1 =>
			if(DATA_RDATA = "00000000") then
				CODE_EN <= '1';
				PCinc <= '1';
				nstate <= swhile2;
			else
			nstate <= sfetch0;
			end if;
			
			when swhile2 =>
			CODE_EN <= '1';
			nstate <=swhile3;
			
			when swhile3 =>
			CODE_EN <= '1';
			PCinc <= '1';
			if (CODE_DATA = X"5B") then
				CNTinc <= '1';
				nstate <=swhile2;
			elsif(CODE_DATA = X"5D") and (CNT = "00000000") then
				nstate <= sfetch0;
			elsif(CODE_DATA = X"5D") and (CNT /= "00000000") then
				nstate <= swhile2;
				CNTdec <= '1';
			else
				nstate <= swhile2;
			end if;
			
			--SWHILEEND
			when swhileend0 =>
			PCdec <='1';
			DATA_EN <= '1';
			CODE_EN <= '1';
			nstate <=swhileend1;
			
			when swhileend1 =>
			if(DATA_RDATA /= "00000000") then
				CODE_EN <= '1';
				PCdec <= '1';
				nstate <= swhileend2;
			else
			PCinc <='1';
			nstate <= sskip;
			end if;
			when swhileend2 =>
			CODE_EN <= '1';
			nstate <=swhileend3;
			when swhileend3 =>
			CODE_EN <= '1';
			PCdec <= '1';
			if (CODE_DATA = X"5D") then
				CNTinc <= '1';
				nstate <=swhileend2;
			elsif(CODE_DATA = X"5B") and (CNT = "00000000") then
				nstate <= sfetch0;
				PCdec<= '0';
				PCinc<='1';
			elsif(CODE_DATA = X"5B") and (CNT /= "00000000") then
				nstate <= swhileend2;
				CNTdec <= '1';
			else
				nstate <= swhileend2;
			end if;
			
			--SBREAK
			when sbreak =>
			PCinc <='1';
			CODE_EN <= '1';
			nstate <=swhile2;
			
			--SHALT
			when shalt =>
				nstate <= shalt;
			
			when others =>
			
		end case;
	end process;
end behavioral;
 
