( SIMPLE 6502 "PARSER" and "OPCODE EMITTER" for FORTH)
( CURRENTLY ONLY HANDLES LDA )
\ EXAMPLES:
\           $2A # LDA    -- immediate
\           $2A LDA      -- zero page
\           $2A ,X LDA   -- zero page,x
\           $2A ,Y LDA   -- zero page,y (invalid addressing mode for LDA)
\           $1234 LDA    -- absolute
\           $2A (,X) LDA -- indexed indirect
\           $2A (,Y) LDA -- indirect indexed
\ -----------------------------------------------------------------------

( CONSTANTS )
( OPCODE ADDR MODES )
( LDA )
$A9 CONSTANT LDA-IMM $A5 CONSTANT LDA-ZP $B5 CONSTANT LDA-ZP,X $AD CONSTANT LDA-ABS
$BD CONSTANT LDA-ABS,X $B9 CONSTANT LDA-ABS,Y $A1 CONSTANT LDA-IND,X $B1 CONSTANT LDA-IND,Y

( VARIABLES )
VARIABLE ADDR-MODE \ set by words: (ACC # ,X ,Y (,X) (,Y)) and reset by all opcode words
0 ADDR-MODE !

( IMPLICIT )          \ some modes don't require an associated word
( ZERO PAGE )
( ZERO PAGE X )
( ZERO PAGE Y )
( ABSOLUTE )
( ABSOLUTE X )
( ABSOLUTE Y )
( ACCUMULATOR )
  : ACC   1 ADDR-MODE ! ;
( IMMEDIATE )
  : #     2 ADDR-MODE ! ;
( ,X )
  : ,X    3 ADDR-MODE ! ;
( ,Y )
  : ,Y    4 ADDR-MODE ! ;
( INDEXED INDIRECT )
  : (,X)  5 ADDR-MODE ! ;
( INDIRECT INDEXED )
  : (,Y)  6 ADDR-MODE ! ;

: ?<=$FF ( op -- op finrange ) DUP $FF <= ;
: ?OPERAND-IN-ZP-RANGE   ?<=$FF ; \ distinguishes between zp and absolute addressing modes
: ?OPERAND-IN-IMM-RANGE  ?<=$FF ; \ registers can only hold 8 bits

( LDA )
  : LDA   ADDR-MODE @
          CASE
            0 OF ?OPERAND-IN-ZP-RANGE IF LDA-ZP ELSE LDA-ABS ENDIF ENDOF \ needs abs range check <=$FFFF
            1 OF ." INCOMPATIBLE ADDR MODE FOR LDA: ACC" ENDOF
            2 OF ?OPERAND-IN-IMM-RANGE IF LDA-IMM ELSE . ." OUT OF RANGE FOR IMM ADDR MODE" ENDIF ENDOF
            3 OF ?OPERAND-IN-ZP-RANGE IF LDA-ZP,X ELSE LDA-ABS,X ENDIF ENDOF \ also needs abs range check
            4 OF ." INCOMPATIBLE ADDR MODE FOR LDA: ZP,Y" ENDOF
            5 OF ?OPERAND-IN-ZP-RANGE IF LDA-IND,X ELSE . ." OUT OF RANGE FOR ZP ADDR MODE" ENDIF ENDOF
            6 OF ?OPERAND-IN-ZP-RANGE IF LDA-IND,Y ELSE . ." OUT OF RANGE FOR ZP ADDR MODE" ENDIF ENDOF
          ENDCASE
          0 ADDR-MODE ! ;
