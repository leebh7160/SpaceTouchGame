
using Unity.VisualScripting;
using UnityEngine;

internal interface IBullet
{
    //총알 이펙트 넣어주기
    //(플레이어로 넣고있는 것 바꾸기)
    void Bullet_SetEffect();

    //총알 수 세기
    void Bullet_CountUp();

    //총알 벽에 닿았을 시 이펙트 실행
    void Bullet_Bust();

    //void Bullet_TriggerActive(Collider2D collision, string tag);//총알 트리거 진행 시 행동
}
