#include "enemy.h"

enemy::enemy()
{
    std::cout << "enemy created.\n";
    type = EnemyType::BasicMelee;
    health = 10;
    position.set(10, 10);
    movementSpeed = 5.0f;
    attackDamage = 5;
    detectionRange = 10.0f;
    attackRange = 5.0f;
    currentState = AIState::Idle;
}
enemy::~enemy()
{
    std::cout << "enemy destroyed.\n";
}

void enemy::TakeDamage(int dmg)
{
    if (health - dmg > 0)
    {
        health -= dmg;
    }
    else
    {
        std::cout << "took damage to 0" << std::endl;
        health = 0;
        currentState = AIState::Dead;
    }
}
bool enemy::IsDead()
{
    if (currentState == AIState::Dead)
    {
        std::cout << "IsDead called, returns true.\n";
        return true;
    }
    else
    {
        std::cout << "IsDead called, returns false.\n";
        return false;
    }
}

void enemy::PrintBio()
{
    std::cout << "Health: " << health << std::endl;
    std::cout << "Position: (" << position.x << ", " << position.y << ")" << std::endl;
}
