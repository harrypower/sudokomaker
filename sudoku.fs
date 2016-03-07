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

require objects.fs
require random.fs

variable workinghorizontal
variable avertical
variable acell
9 9 * cell * constant datasize

here constant sudokudata \ making space for the sudoku data
datasize allot
sudokudata datasize erase \ ensure starting with no values in the sudoku

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
