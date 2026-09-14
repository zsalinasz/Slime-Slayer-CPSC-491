#pragma once
#include <iostream>
#include "enemy_type.h"
#include "vec2.h"
#include "AIState.h"

class enemy
{
public:
    enemy();
    ~enemy();
    void TakeDamage(int dmg);
    bool IsDead();
    void PrintBio();
    
private:
    EnemyType type;
    int health;
    int maxHealth;
    Vector2 position;
    float movementSpeed;
    int attackDamage;
    float detectionRange;
    float attackRange;
    AIState currentState;
};
