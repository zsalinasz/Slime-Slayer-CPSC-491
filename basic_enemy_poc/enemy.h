#pragma once
#include <iostream>
#include <string>
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
    std::string get_state_str();
    float get_detectionRange();
    float get_attackRange();
    void set_state(AIState state);
    AIState get_state();
    void move_enemy(int dx, int dy);
    int get_attackDamage();
    Vector2 get_position();
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
