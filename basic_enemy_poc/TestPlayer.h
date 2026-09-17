#pragma once
#include "vec2.h"

class TestPlayer
{
public:
    Vector2 position;
    int Health;
    bool alive;
    
    TestPlayer()
    {}
    TestPlayer(int x, int y, int health)
    {
        position.set(x, y);
        Health = health;
        alive = true;
    }
    void TakeDamage(int damage)
    {
        if (Health - damage > 0)
        {
            Health -= damage;
        }
        else if (alive)
        {
            Health = 0;
            alive = false;
        }
    }

};
