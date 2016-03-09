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

warnings off

require random.fs
utime drop seed !

9 9 * cell * constant datasize

0 value sudokulist \ making space for the sudoku data
here to sudokulist
datasize allot
: clearsudoku ( -- )
   sudokulist datasize erase ; \ ensure starting with no values in the sudoku
clearsudoku

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
      \ 100 throw \ only numbers 0 to 8 allowed
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
      \ 101 throw \ only numbers 0 to 8 allowed
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
      \ 102 throw \ only numbers 0 to 8 allowed
   endcase ;

: testhorz ( ntest nhorz  -- nflag ) \ look through horizontal items for ntest if not found then nflag is false
   \ nflag is true if ntest value is found
   { tvalue horz }
   try
      9 0 do horz i horzaddr cell * sudokulist + @ tvalue = if true throw then loop
      false
   restore
   endtry ;

: testvert ( ntest nvert -- nflag ) \ look through vertical items for ntest if not found then nflag is false
   \ nflag is true if ntest value is found
   { tvalue vert }
   try
      9 0 do vert i vertaddr cell * sudokulist + @ tvalue = if true throw then loop
      false
   restore
   endtry ;

: testcells ( ntest ncell -- nflag ) \ look through cell items for ntest if not found nflag is false
   \ nflag is true if ntest is found
   { tvalue ncell }
   try
      9 0 do ncell i celladdr cell * sudokulist + @ tvalue = if true throw then loop
      false
   restore
   endtry ;

: nextguess ( -- nguess ) \ simply produce a number guess for sudoku 1 to 9
   9 random 1 + ;

: sudoku! ( nvalue nhorz nindex -- ) \ store nvalue in sudokulist at nhorz nindex location
   horzaddr cell * sudokulist + ! ;

: sudoku@ ( nhorz nindex -- nvalue ) \ retrieve nvalue of the current nhorz nindex location of the sudoku
   horzaddr cell * sudokulist + @ ;

: clearhorizontal ( nhorz -- ) \ simply put 0 into all horizontal data for nhorz
   { nhorz }
   9 0 do 0 nhorz i sudoku! loop ;

: doahorziontal ( nhorz nindex -- nindex1 )  \ inside part of makesudoku
   \ finds a working horizontal set of sudoku number to keep in solution
   0 0 { nhorz nindex theguess failtimes }
   begin
      nextguess to theguess
      theguess nhorz testhorz false =
      if theguess nindex testvert false =
         if theguess nhorz nindex horztocell testcells false =
            if theguess nhorz nindex sudoku! then
         then
      then
      nhorz nindex sudoku@ 0 =
      if \ solution not found this time
         failtimes 1+ to failtimes
         failtimes 20 >
         if \ bail from this failed horizontal solution
            -1 nhorz clearhorizontal \ now clear the current horizontal
            true
         else
            false \ try again for solution
         then
      else
         nindex true \ solution found return
      then
   until ;

: doindex ( nhorz -- ) \ intermediate part of makesudoku
   0 { nhorz nindex }
   begin
      nhorz nindex doahorziontal to nindex nindex 1+ to nindex
      nindex 9 >=
   until ;

: makesudoku ( -- ) \ make the sudokulist that will be a solvable sudoku
      clearsudoku cr
      9 0 do \ remember i this will be horizontal
         i doindex
      loop ;

: displaysudoku ( -- ) \ display current sudoku
   page
   9 0 do
      9 0 do
         j i sudoku@ i j at-xy .
      loop
   loop ;

: easy 30 50 ;
: medium 25 50 ;
: hard 20 50 ;
: showall 40 40 ;
: displaychance ( nchance nrnd -- nflag ) \ nflag is true or false based on the chance of random draw
   random > ;

: sudoku ( nchance nrnd -- ) \ display the sudoku
   { nchance nrnd }
   page
   9 0 do
      9 0 do
         nchance nrnd displaychance
         if  j i sudoku@ i j at-xy . else i j at-xy ." -" then
      loop
   loop ;

: startsudoku ( -- )
   makesudoku
   s" SUDOKU MAKER" type cr
   s" To display the sudoku type one of the following:" type cr
   s" easy sudoku     - to see an easy sudoku!" type cr
   s" medium sudoku   - to see a medium sudoku!" type cr
   s" hard sudoku     - to see a hard sudoku!" type cr
   s" showall sudoku  - to see the sudoku solution!" type cr
   s" makesudoku      - to create a new sudoku and delete the old one from memory!" type cr
   s" startsudoku     - to see this message again and create a new sudoku!" type cr ;

:noname startsudoku ; is bootmessage
