-- fsm.vhd: Finite State Machine
-- Author(s): 
--
library ieee;
use ieee.std_logic_1164.all;
-- ----------------------------------------------------------------------------
--                        Entity declaration
-- ----------------------------------------------------------------------------
entity fsm is
port(
   CLK         : in  std_logic;
   RESET       : in  std_logic;

   -- Input signals
   KEY         : in  std_logic_vector(15 downto 0);
   CNT_OF      : in  std_logic;

   -- Output signals
   FSM_CNT_CE  : out std_logic;
   FSM_MX_MEM  : out std_logic;
   FSM_MX_LCD  : out std_logic;
   FSM_LCD_WR  : out std_logic;
   FSM_LCD_CLR : out std_logic
);
end entity fsm;

-- ----------------------------------------------------------------------------
--                      Architecture declaration
-- ----------------------------------------------------------------------------
architecture behavioral of fsm is
   type t_state is (TEST1, PRINT_MESSAGE, ERR, FINISH, TEST10, TEST11, TEST12, TEST13, TEST14, TEST15, TEST16, TEST17, TEST18,
    TEST19, TEST20, TEST21, TEST22, TEST23, TEST24, TEST25, TEST26, TEST27, TEST28, SUCC); 
   signal present_state, next_state : t_state;

begin
-- -------------------------------------------------------
sync_logic : process(RESET, CLK)
begin
   if (RESET = '1') then
      present_state <= TEST1;
   elsif (CLK'event AND CLK = '1') then
      present_state <= next_state;
   end if;
end process sync_logic;

-- -------------------------------------------------------
next_state_logic : process(present_state, KEY, CNT_OF)
begin
   case (present_state) is
   -- - - - - - - - - - - - - - - - - - - - - - -
   when TEST1 =>
      next_state <= TEST1;
      if (KEY(7) = '1') then
         next_state <= TEST10;
      elsif (KEY(3) = '1') then
         next_state <= TEST20;
      elsif (KEY(14 downto 0) /= "000000000000000") then
         next_state <= ERR;
      elsif (KEY(15) = '1') then
         next_state <= PRINT_MESSAGE; 
      end if;
   -- - - - - - - - - - - - - - - - - - - - - - -
   when ERR =>
   
      next_state <= present_state;
      if (KEY(14 downto 0) /= "000000000000000") then
        next_state <= ERR;
      elsif (KEY(15) = '1') then
         next_state <= PRINT_MESSAGE; 
      end if;
   -- - - - - - - - - - - - - - - - - - - - - - -
   when TEST10 =>
      next_state <= present_state;
      if (KEY(7) = '1') then
        next_state <= TEST11;
      elsif (KEY(14 downto 0) /= "000000000000000") then
        next_state <= ERR;
      elsif (KEY(15) = '1') then
         next_state <= PRINT_MESSAGE; 
      end if;
   -- - - - - - - - - - - - - - - - - - - - - - -
   when TEST11 =>
      next_state <= present_state;
      if (KEY(1) = '1') then
        next_state <= TEST12;
      elsif (KEY(14 downto 0) /= "000000000000000") then
        next_state <= ERR;
      elsif (KEY(15) = '1') then
         next_state <= PRINT_MESSAGE; 
      end if;
   -- - - - - - - - - - - - - - - - - - - - - - -
   when TEST12 =>
      next_state <= present_state;
      if (KEY(3) = '1') then
        next_state <= TEST13;
      elsif (KEY(14 downto 0) /= "000000000000000") then
        next_state <= ERR;
      elsif (KEY(15) = '1') then
         next_state <= PRINT_MESSAGE; 
      end if;
   -- - - - - - - - - - - - - - - - - - - - - - -
   when TEST13 =>
      next_state <= present_state;
      if (KEY(5) = '1') then
        next_state <= TEST14;
      elsif (KEY(14 downto 0) /= "000000000000000") then
        next_state <= ERR;
      elsif (KEY(15) = '1') then
         next_state <= PRINT_MESSAGE; 
      end if;
   -- - - - - - - - - - - - - - - - - - - - - - -
   when TEST14 =>
      next_state <= present_state;
      if (KEY(0) = '1') then
        next_state <= TEST15;
      elsif (KEY(14 downto 0) /= "000000000000000") then
        next_state <= ERR;
      elsif (KEY(15) = '1') then
         next_state <= PRINT_MESSAGE; 
      end if;
   -- - - - - - - - - - - - - - - - - - - - - - -
    when TEST15 =>
      next_state <= present_state;
      if (KEY(6) = '1') then
        next_state <= TEST16;
      elsif (KEY(14 downto 0) /= "000000000000000") then
        next_state <= ERR;
      elsif (KEY(15) = '1') then
         next_state <= PRINT_MESSAGE; 
      end if;
   -- - - - - - - - - - - - - - - - - - - - - - -
   when TEST16 => 
      next_state <= present_state;
      if (KEY(7) = '1') then
        next_state <= TEST17;
      elsif (KEY(14 downto 0) /= "000000000000000") then
        next_state <= ERR;
      elsif (KEY(15) = '1') then
         next_state <= PRINT_MESSAGE; 
      end if;
   -- - - - - - - - - - - - - - - - - - - - - - -
   when TEST17 =>
      next_state <= present_state;
      if (KEY(0) = '1') then
        next_state <= TEST18;
      elsif (KEY(14 downto 0) /= "000000000000000") then
        next_state <= ERR;
      elsif (KEY(15) = '1') then
         next_state <= PRINT_MESSAGE; 
      end if;
   -- - - - - - - - - - - - - - - - - - - - - - -
   when TEST18 =>
      next_state <= present_state;
      if (KEY(4) = '1') then
        next_state <= TEST19;
      elsif (KEY(14 downto 0) /= "000000000000000") then
        next_state <= ERR;
      elsif (KEY(15) = '1') then
         next_state <= PRINT_MESSAGE; 
      end if;
   -- - - - - - - - - - - - - - - - - - - - - - -

   when TEST19 =>
      next_state <= present_state;
      if (KEY(15) = '1') then
        next_state <= SUCC;
      elsif (KEY(14 downto 0) /= "000000000000000") then
        next_state <= ERR;
      end if;
   -- - - - - - - - - - - - - - - - - - - - - - -
   when TEST20 =>
      next_state <= present_state;
      if (KEY(8) = '1') then
        next_state <= TEST21;
      elsif (KEY(14 downto 0) /= "000000000000000") then
        next_state <= ERR;
      elsif (KEY(15) = '1') then
         next_state <= PRINT_MESSAGE; 
      end if;
   -- - - - - - - - - - - - - - - - - - - - - - -
   when TEST21 =>
      next_state <= present_state;
      if (KEY(5) = '1') then
        next_state <= TEST22;
      elsif (KEY(14 downto 0) /= "000000000000000") then
        next_state <= ERR;
      elsif (KEY(15) = '1') then
         next_state <= PRINT_MESSAGE; 
      end if;
   -- - - - - - - - - - - - - - - - - - - - - - -
   when TEST22 =>
      next_state <= present_state;
      if (KEY(6) = '1') then
        next_state <= TEST23;
      elsif (KEY(14 downto 0) /= "000000000000000") then
        next_state <= ERR;
      elsif (KEY(15) = '1') then
         next_state <= PRINT_MESSAGE; 
      end if;
   -- - - - - - - - - - - - - - - - - - - - - - -
   when TEST23 =>
      next_state <= present_state;
      if (KEY(7) = '1') then
        next_state <= TEST24;
      elsif (KEY(14 downto 0) /= "000000000000000") then
        next_state <= ERR;
      elsif (KEY(15) = '1') then
         next_state <= PRINT_MESSAGE; 
      end if;
   -- - - - - - - - - - - - - - - - - - - - - - -
   when TEST24 =>
      next_state <= present_state;
      if (KEY(5) = '1') then
        next_state <= TEST25;
      elsif (KEY(14 downto 0) /= "000000000000000") then
        next_state <= ERR;
      elsif (KEY(15) = '1') then
         next_state <= PRINT_MESSAGE; 
      end if;
   -- - - - - - - - - - - - - - - - - - - - - - -
    when TEST25 =>
      next_state <= present_state;
      if (KEY(3) = '1') then
        next_state <= TEST26;
      elsif (KEY(14 downto 0) /= "000000000000000") then
        next_state <= ERR;
      elsif (KEY(15) = '1') then
         next_state <= PRINT_MESSAGE; 
      end if;
   -- - - - - - - - - - - - - - - - - - - - - - -
   when TEST26 => 
      next_state <= present_state;
      if (KEY(3) = '1') then
        next_state <= TEST27;
      elsif (KEY(14 downto 0) /= "000000000000000") then
        next_state <= ERR;
      elsif (KEY(15) = '1') then
         next_state <= PRINT_MESSAGE; 
      end if;
   -- - - - - - - - - - - - - - - - - - - - - - -
   when TEST27 =>
      next_state <= present_state;
      if (KEY(5) = '1') then
        next_state <= TEST28;
      elsif (KEY(14 downto 0) /= "000000000000000") then
        next_state <= ERR;
      elsif (KEY(15) = '1') then
         next_state <= PRINT_MESSAGE; 
      end if;
   -- - - - - - - - - - - - - - - - - - - - - - -
   when TEST28 =>
      next_state <= present_state;
      if (KEY(2) = '1') then
        next_state <= TEST19;
      elsif (KEY(14 downto 0) /= "000000000000000") then
        next_state <= ERR;
      elsif (KEY(15) = '1') then
         next_state <= PRINT_MESSAGE; 
      end if;
   -- - - - - - - - - - - - - - - - - - - - - - -
   when SUCC =>
      next_state <= SUCC;
      if (CNT_OF = '1') then
        next_state <= FINISH;
      end if;
   -- - - - - - - - - - - - - - - - - - - - - - -
   when PRINT_MESSAGE =>
      next_state <= PRINT_MESSAGE;
      if (CNT_OF = '1') then
         next_state <= FINISH;
      end if;
   -- - - - - - - - - - - - - - - - - - - - - - -
   when FINISH =>
      next_state <= FINISH;
      if (KEY(15) = '1') then
         next_state <= TEST1; 
      end if;
   -- - - - - - - - - - - - - - - - - - - - - - -
   when others =>
      next_state <= TEST1;
   end case;
end process next_state_logic;

-- -------------------------------------------------------
output_logic : process(present_state, KEY)
begin
   FSM_CNT_CE     <= '0';
   FSM_MX_MEM     <= '0';   
   FSM_MX_LCD     <= '0';
   FSM_LCD_WR     <= '0';
   FSM_LCD_CLR    <= '0';

   case (present_state) is
   -- - - - - - - - - - - - - - - - - - - - - - -
   when TEST1 =>
      if (KEY(14 downto 0) /= "000000000000000") then
         FSM_LCD_WR     <= '1';
      end if;
      if (KEY(15) = '1') then
         FSM_LCD_CLR    <= '1';
      end if;
   -- - - - - - - - - - - - - - - - - - - - - - -
   when PRINT_MESSAGE =>
      FSM_CNT_CE     <= '1';
      FSM_MX_MEM     <= '0';
      FSM_MX_LCD     <= '1';
      FSM_LCD_WR     <= '1';
   -- - - - - - - - - - - - - - - - - - - - - - -
   when SUCC =>
      FSM_CNT_CE     <= '1';
      FSM_MX_MEM     <= '1'; 
      FSM_MX_LCD     <= '1';
      FSM_LCD_WR     <= '1';
   -- - - - - - - - - - - - - - - - - - - - - - -
   when FINISH =>
      if (KEY(15) = '1') then
         FSM_LCD_CLR    <= '1';
      end if;
   -- - - - - - - - - - - - - - - - - - - - - - -
   when others =>
   if (KEY(14 downto 0) /= "000000000000000") then
         FSM_LCD_WR     <= '1';
      end if;
      if (KEY(15) = '1') then
         FSM_LCD_CLR    <= '1';
      end if;
   end case;
end process output_logic;

end architecture behavioral;

