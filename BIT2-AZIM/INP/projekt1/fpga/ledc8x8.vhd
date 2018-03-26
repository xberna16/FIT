library IEEE;
use IEEE.std_logic_1164.all;
use IEEE.std_logic_arith.all;
use IEEE.std_logic_unsigned.all;

entity ledc8x8 is
port ( -- Sem doplnte popis rozhrani obvodu.
        ROW     : out std_logic_vector (0 to 7);
        LED     : out std_logic_vector (0 to 7);
        RESET   : in std_logic; 
        SMCLK   : in std_logic
);
end ledc8x8;

architecture main of ledc8x8 is

    -- Sem doplnte definice vnitrnich signalu.
    signal counter: std_logic_vector(7 downto 0);
    signal ce: std_logic;
    signal led_ver: std_logic_vector(7 downto 0);
    signal row_hor: std_logic_vector(7 downto 0);
    
begin

    -- Sem doplnte popis obvodu. Doporuceni: pouzivejte zakladni obvodove prvky
    -- (multiplexory, registry, dekodery,...), jejich funkce popisujte pomoci
    -- procesu VHDL a propojeni techto prvku, tj. komunikaci mezi procesy,
    -- realizujte pomoci vnitrnich signalu deklarovanych vyse.

    -- DODRZUJTE ZASADY PSANI SYNTETIZOVATELNEHO VHDL KODU OBVODOVYCH PRVKU,
    -- JEZ JSOU PROBIRANY ZEJMENA NA UVODNICH CVICENI INP A SHRNUTY NA WEBU:
    -- http://merlin.fit.vutbr.cz/FITkit/docs/navody/synth_templates.html.

    -- Nezapomente take doplnit mapovani signalu rozhrani na piny FPGA
    -- v souboru ledc8x8.ucf.

    
    gener: process(SMCLK, RESET) 
    begin
        if RESET = '1' then 
            counter <= "00000000";
        elsif SMCLK'event and SMCLK = '1' then 
            counter <= counter + 1;
        end if;
    end process gener;
    ce <= '1' when counter = X"FF" else '0';

    

    row_rotate: process(RESET, ce, SMCLK)
    begin   
        if RESET = '1' then 
            row_hor <= "10000000"; 
        elsif (SMCLK'event and SMCLK = '1' and ce = '1') then
            row_hor <= row_hor(0) & row_hor(7 downto 1);
        end if;
    end process row_rotate;

    
  
    row_decoder: process(row_hor)
    begin
        case row_hor is
            when "10000000" => led_ver <= "01101111";
            when "01000000" => led_ver <= "01101111";
            when "00100000" => led_ver <= "00001111";
            when "00010000" => led_ver <= "01100001";
            when "00001000" => led_ver <= "01100110";
            when "00000100" => led_ver <= "11110001";
            when "00000010" => led_ver <= "11110110";
            when "00000001" => led_ver <= "11110001";
            when others =>     led_ver <= "11111111";
        end case;
    end process row_decoder;
                                  
    ROW <= row_hor;
    LED <= led_ver;

end main;
