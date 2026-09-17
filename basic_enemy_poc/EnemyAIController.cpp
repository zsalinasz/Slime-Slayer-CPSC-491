#include "EnemyAIController.h"

EnemyAIController::EnemyAIController()
{
    TestPlayer nPlayer;
    target = nPlayer;
    target.Health = 10;
    target.position.x = 22;
    target.position.y = 22;
    target.alive = true;
}

void EnemyAIController::Update()
{
    std::cout << "Enemy at (" << Enemy.get_position().x << ", " << Enemy.get_position().y << ")\n";
    std::cout << "Player at (" << target.position.x << ", " << target.position.y << ")\n";
    std::cout << "Player Health: " << target.Health << "\n";
    std::cout << "Enemy state: " << Enemy.get_state_str() << std::endl;
    
    //Simulate target for this test
    target.position.x -= 1;
    target.position.y -= 1;
    
    //Process Transition Conditions
    
    if (DetectPlayer())
    {
        if (PlayerInRange())
        {
            ChangeState(AIState::Attack);
            std::cout << "Enemy transitions to attack mode.\n";
        }
        else
        {
            ChangeState(AIState::Chase);
            std::cout << "Enemy chases player.\n";
        }
    }
    else
    {
        std::cout << "Enemy reverts to idle state.\n";
        ChangeState(AIState::Idle);
    }
    
    //Process State behavior
    
    if (Enemy.get_state() == AIState::Chase)
    {
        // calculate displacement vector
        float dx = Enemy.get_position().x - target.position.x;
        float dy = Enemy.get_position().y - target.position.y;
        float dist = sqrt((dx*dx) + (dy*dy));
        // make unit
        dx /= dist;
        dy /= dist;
        // multiply speeed
        dx *= 3;
        dy *= 3;
        //make positive if negative
        dx = abs(dx);
        dy = abs(dy);
        
        std::cout << "Disp: " << dx << " " << dy << std::endl;
        
        //approach player.
        Enemy.move_enemy(dx, dy);
    }
    
    if (Enemy.get_state() == AIState::Attack)
    {
        std::cout << "Enemy attacks player.";
        target.TakeDamage(Enemy.get_attackDamage());
    }
    
}
void EnemyAIController::ChangeState(AIState state)
{
    Enemy.set_state(state);
}
bool EnemyAIController::DetectPlayer()
{
    if (!target.alive)
    {
        std::cout << "Target is dead.\n";
        return false;
    }
    //returns true if player is detected
    float dx = Enemy.get_position().x - target.position.x;
    float dy = Enemy.get_position().y - target.position.y;
    float dist = sqrt((dx*dx) + (dy*dy));
    if (dist <= Enemy.get_detectionRange())
    {
        return true;
        std::cout << "detect is true\n";
    }
    else
    {
        std::cout << "Player dist is: " << dist << std::endl;
        return false;
    }
}
bool EnemyAIController::PlayerInRange()
{
    //returns true if player is in range
    float dx = Enemy.get_position().x - target.position.x;
    float dy = Enemy.get_position().y - target.position.y;
    float dist = sqrt((dx*dx) + (dy*dy));
    if (dist <= Enemy.get_attackRange())
    {
        return true;
    }
    else
    {
        return false;
    }
}
