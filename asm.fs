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
\ \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


( UTILITIES ) \ ------------------------------------------------------------------------------

: @INDEX ( uindex - copcode ) CELLS + @ ;

: ?IN-SIGNED-8BIT-RANGE
    DUP -128 >= SWAP $FF <= AND ;

: ?OPERAND-IN-ZP-RANGE ( operand -- finrange )     \ distinguishes between zp and absolute addressing modes
    ?IN-SIGNED-8BIT-RANGE ;

: ?OPERAND-IN-IMM-RANGE ( operand -- finrange )    \ registers can only hold 8 bits
    ?IN-SIGNED-8BIT-RANGE ;

( ADDRESSING MODE WORDS ) \ ------------------------------------------------------------------

: ?VALID-MODE @INDEX 0= INVERT ;

: MODE-DEFAULT ( noperand caddr -- coperand copcode )
    SWAP DUP
      ?OPERAND-IN-ZP-RANGE
        IF SWAP DUP 2 ?VALID-MODE
                        IF 2 @INDEX
                        ELSE ." ZP INVALID" ENDIF
        ELSE SWAP DUP 5 ?VALID-MODE
                          IF 5 @INDEX
                          ELSE ." ABS INVALID" ENDIF
        ENDIF ;

: MODE-IMM
    SWAP DUP ?OPERAND-IN-IMM-RANGE
               IF SWAP 1 @INDEX
               ELSE . ." OUT OF BOUNDS FOR IMM ADDR MODE" DROP ENDIF ;

: MODE-ZP-X SWAP DUP
              ?OPERAND-IN-ZP-RANGE
                IF SWAP 3 @INDEX 
                ELSE . ." OUT OF BOUNDS FOR ZP,X ADDR MODE " ENDIF ;

: MODE-ZP-Y SWAP DUP
              ?OPERAND-IN-ZP-RANGE
                IF SWAP 4 @INDEX 
                ELSE . ." OUT OF BOUNDS FOR ZP,Y ADDR MODE " ENDIF ;

: MODE-ABS-X 6 @INDEX ;

: MODE-ABS-Y 7 @INDEX ;

( CONSTANTS )
VARIABLE ADDR-MODE  \ set by words: (ACC # ,X ,Y (,X) (,Y)) and reset by all opcode words
' MODE-DEFAULT ADDR-MODE ! 

( OPCODE CONSTRUCTOR )
: OPCODE   CREATE
             ,  \ INDY
             ,  \ INDX
             ,  \ ABSY
             ,  \ ABSX
             ,  \ ABS
             ,  \ ZPY
             ,  \ ZPX
             ,  \ ZP
             ,  \ IMM
             ,  \ ACC


DOES> ADDR-MODE PERFORM  \ call function pointed to by ADDR-MODE,
      ['] MODE-DEFAULT ADDR-MODE ! ;

( OPCODES )
$B1 $A1 $B9 $BD $AD 00 $B5 $A5 $A9 $00 OPCODE LDA
: BRK $00 ; \ BRK simply pushes its byte (as do all opcodes which use only implied addressing)
            \ and so its code $00 is repurposed in OPCODE-defined words to signify an invalid addr mode
            
: CLC $18 ;
: CLD $D8 ;
: CLI $58 ;
: CLV $B8 ;
: DEX $CA ;
: DEY $88 ;
: INX $E8 ;
: INY $C8 ;
: NOP $EA ;
: PHA $48 ;
: PHP $08 ;
: PLA $68 ;
: PLP $28 ;
: RTI $40 ;
: RTS $60 ;
: SEC $38 ;
: SED $F8 ;
: SEI $78 ;
: TAX $AA ;
: TAY $A8 ;
: TSX $BA ;
: TXA $8A ;
: TXS $9A ;
: TYA $98 ;

( VARIABLES )

( IMPLICIT )          \ some modes don't require an associated word
( ACCUMULATOR )
  : ACC   1 ADDR-MODE ! ;
( IMMEDIATE )
  : #     ['] MODE-IMM ADDR-MODE ! ;
( ZERO PAGE )
( ZERO PAGE X )
( ZERO PAGE Y )
( RELATIVE )
( ABSOLUTE )
( ABSOLUTE X )
( ABSOLUTE Y )
( ,X )
  : ,X    
          DUP ?OPERAND-IN-ZP-RANGE
          IF ['] MODE-ZP-X ADDR-MODE !
          ELSE ['] MODE-ABS-X ADDR-MODE !
          ENDIF ;
( ,Y )
  : ,Y    
          DUP ?OPERAND-IN-ZP-RANGE
          IF ['] MODE-ZP-Y ADDR-MODE !
          ELSE ['] MODE-ABS-Y ADDR-MODE !
          ENDIF ;

( INDEXED INDIRECT )
  : (,X)  5 ADDR-MODE ! ;
( INDIRECT INDEXED )
  : (,Y)  6 ADDR-MODE ! ;
