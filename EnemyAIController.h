#pragma once
#include <cmath>
#include "enemy.h"
#include "TestPlayer.h"

class EnemyAIController
{
 public:
     enemy Enemy;
     TestPlayer target;
     
 public:
     EnemyAIController();
     void Update();
     void ChangeState(AIState state);
     bool DetectPlayer();
     bool PlayerInRange();
};
