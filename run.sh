#!/bin/bash

rm *.out

echo "Compile C code"
g++ -g -c -m64 -o main.o main.cpp
g++ -g -c -m64 -o enemy.o enemy.cpp
g++ -g -c -m64 -o enemy_type.o enemy_type.cpp
g++ -g -c -m64 -o AIState.o AIState.cpp
g++ -g -c -m64 -o vec2.o vec2.cpp
echo "Link files"
g++ -g -m64 -o main.out main.o enemy.o enemy_type.o AIState.o vec2.o
echo "Running main..."
./main.out

echo "Bash script terminating."

