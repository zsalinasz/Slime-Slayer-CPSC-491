#include <iostream>
#include "enemy.h"

int main()
{
    std::cout << "Hello.\n";
    
    AIState st = AIState::Idle;
    if (st == AIState::Idle)
    {
        std::cout << "test works.\n";
    }
    else
    {
        std::cout << "TEST doesn't work.\n";
    }
    
    
    enemy en;
    int c = 0;
    while (!en.IsDead())
    {
     en.PrintBio();
     en.TakeDamage(1);
     c++;
     if ( c >= 12 )
         break;
    }
    if (en.IsDead()) 
    {
        std::cout << "Enemy died.\n";
    }

    return 0;
}
