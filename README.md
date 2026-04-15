[![Review Assignment Due Date](https://classroom.github.com/assets/deadline-readme-button-22041afd0340ce965d47ae6ef1cefeee28c7c493a6346c4f15d667ab976d596c.svg)](https://classroom.github.com/a/ozVFrFMv)
# CSCI 1260 — Project

## Project Instructions
The rules of mine sweeper are simple, First you pick tile that you think is not a bomb.
then once some tiles are cleared you will see either a clear box or a number. 
these numbers mean how many bombs are adjencent to that tile.
If you think that found a bomb you flag it.
You keep doing this until you flag all the mines.

## Board Sizes
8x8 with 10 mines
12x12 with 25 mines
16x16 with 40 mines

choose your board in the main menu

## input commands
r = Reveal
f = Flag
q = quit

to reveal a tile you would use the command "r row col" 
and the same for flag "f row col"
if i wanted to reveal a tile in row 7 and column 9 I would use "r 7 9"

## Seed
enter any number to use a deterministic seed
leave blank for a random seed

## Board Symbols
'#' = Hidden tile
'f' = Flagged tile
'.' = Revealed safe tile
'1-8' = Revealed tile with adjencent mine(s)
'b' = Bomb