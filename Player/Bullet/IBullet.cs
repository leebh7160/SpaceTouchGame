
using Unity.VisualScripting;
using UnityEngine;

internal interface IBullet
{
    //�Ѿ� ����Ʈ �־��ֱ�
    //(�÷��̾�� �ְ��ִ� �� �ٲٱ�)
    void Bullet_SetEffect();

    //�Ѿ� �� ����
    void Bullet_CountUp();

    //�Ѿ� ���� ����� �� ����Ʈ ����
    void Bullet_Bust();

    //void Bullet_TriggerActive(Collider2D collision, string tag);//�Ѿ� Ʈ���� ���� �� �ൿ
}
