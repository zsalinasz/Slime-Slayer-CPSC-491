#include <iostream>
#include "enemy.h"
#include "EnemyAIController.h"

int main()
{   
    enemy en;
    EnemyAIController controller;
    controller.Enemy = en;
    
    
    for (int i = 0; controller.target.alive && !controller.Enemy.IsDead(); i++)
    {
     controller.Update();
     if (i >= 30)
         break;
    }
    if (en.IsDead()) 
    {
        std::cout << "Enemy died.\n";
    }
    if (!controller.target.alive)
    {
        std::cout << "\nPlayer was killed by enemy.\n";
    }

    return 0;
}
