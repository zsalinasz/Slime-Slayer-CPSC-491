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
        return true;
    }
    else
    {
        return false;
    }
}

void enemy::PrintBio()
{
    std::cout << "Health: " << health << std::endl;
    std::cout << "Position: (" << position.x << ", " << position.y << ")" << std::endl;
}

std::string enemy::get_state_str()
{
 switch (currentState)
 {
  case AIState::Idle: return "Idle";
      break;
  case AIState::Chase: return "Chase";
      break;
  case AIState::Attack: return "Attack";
      break;
  case AIState::Recovery: return "Recovery";
      break;
  case AIState::HitReaction: return "HitReaction";
      break;
  case AIState::Dead: return "Dead";
      break;
 }
}

float enemy::get_detectionRange()
{
    return detectionRange;
}
float enemy::get_attackRange()
{
    return attackRange;
}
void enemy::set_state(AIState state)
{
    currentState = state;
}
AIState enemy::get_state()
{
    return currentState;
}

void enemy::move_enemy(int dx, int dy)
{
    position.x += dx;
    position.y += dy;
}
int enemy::get_attackDamage()
{
    return attackDamage;
}
Vector2 enemy::get_position()
{
    return position;
}
