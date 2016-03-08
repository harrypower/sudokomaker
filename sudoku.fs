#! /usr/bin/gforth

\    Copyright (C) 2016  Philip K. Smith
\    This program is free software: you can redistribute it and/or modify
\    it under the terms of the GNU General Public License as published by
\    the Free Software Foundation, either version 3 of the License, or
\    (at your option) any later version.

\    This program is distributed in the hope that it will be useful,
\    but WITHOUT ANY WARRANTY; without even the implied warranty of
\    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
\    GNU General Public License for more details.

\    You should have received a copy of the GNU General Public License
\    along with this program.  If not, see <http://www.gnu.org/licenses/>.
\
\ This code simply makes a sudoku!

require random.fs

9 9 * cell * constant datasize

0 value sudokulist \ making space for the sudoku data
here to sudokulist
datasize allot
sudokulist datasize erase \ ensure starting with no values in the sudoku

: horzaddr ( nhorz nindex -- naddr ) \ calculate the address of horizontal data
   swap 9 * + ;

: vertaddr ( nvert nindex -- naddr ) \ caluclate the address of vertical data
   9 * + ;

: celladdr ( ncell nindex -- naddr ) \ calculate the address of a cell data
   swap case
      0 of 0 endof
      1 of 3 endof
      2 of 6 endof
      3 of 27 endof
      4 of 30 endof
      5 of 33 endof
      6 of 54 endof
      7 of 57 endof
      8 of 60 endof
      \ default case:  should not ever get here
      100 throw \ only numbers 0 to 8 allowed
   endcase swap  ( ncell-start nindex )
   case
      0 of 0 endof
      1 of 1 endof
      2 of 2 endof
      3 of 9 endof
      4 of 10 endof
      5 of 11 endof
      6 of 18 endof
      7 of 19 endof
      8 of 20 endof
      \ default case: should not ever get here
      101 throw \ only numbers 0 to 8 allowed
   endcase ( ncell-start nadjusted-index )
   + ;

: horztocell ( nhorz nindex -- ncell ) \ calculate ncell value from horizontal and index values
   3 / swap
   case
      0 of 0 + endof
      1 of 0 + endof
      2 of 0 + endof
      3 of 3 + endof
      4 of 3 + endof
      5 of 3 + endof
      6 of 6 + endof
      7 of 6 + endof
      8 of 6 + endof
      \ default case: should not ever get here
      102 throw \ only numbers 0 to 8 allowed
   endcase ;

\ loop through workingindex and workinghorizontal from 0 to 8 and when done the sudoku is done
\ in the loop get a random number then test horizontal vertical and cells for conflict
\ if no conflict place the number into the workingindex workinghorizontal location and continute to next loop
\ if a conflict get another random number and repeat the conflict test.  Eventualy a number will be found to work with all three
\ test plains .

: testhorz ( ntest nhorz  -- nflag ) \ look through horizontal items for ntest if not found then nflag is false
   \ nflag is true if ntest value is found
   { tvalue horz }
   try
      9 0 do horz i horzaddr sudokulist + @ tvalue = if true throw then loop
      false
   restore
   endtry ;

: testvert ( ntest nvert -- nflag ) \ look through vertical items for ntest if not found then nflag is false
   \ nflag is true if ntest value is found
   { tvalue vert }
   try
      9 0 do vert i vertaddr sudokulist + @ tvalue = if true throw then loop
      false
   restore
   endtry ;

: testcells ( ntest ncell -- nflag ) \ look through cell items for ntest if not found nflag is false
   \ nflag is true if ntest is found
   { tvalue ncell }
   try
      9 0 do ncell i celladdr sudokulist + @ tvalue = if true throw then loop
      false
   restore
   endtry ;

: nextguess ( -- nguess ) \ simply produce a number guess for sudoku 1 to 9
   9 random 1 + ;

: sudoku! ( nvalue nhorz nindex -- ) \ store nvalue in sudokulist at nhorz nindex location
   horzaddr cell * sudokulist + ! ;

: sudoku@ ( nhorz nindex -- nvalue ) \ retrieve nvalue of the current nhorz nindex location of the sudoku
   horzaddr cell * sudokulist + @ ;

: makesudoku ( -- ) \ make the sudokulist that will be a solvable sudoku
   0 { theguess }
   9 0 do \ remember j this will be horizontal
      9 0 do \ remember i this will be index
         begin
            nextguess to theguess
            theguess j testhorz false =
            if theguess i testvert false =
               if theguess j i horztocell testcells false =
                  if theguess j i sudoku! then
               then
            then
            j i sudoku@ \ if 0 is in sudokulist then nothing was stored there yet so repeat until something stored
         until
      loop
   loop ;
