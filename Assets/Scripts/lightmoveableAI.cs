using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class lightmoveableAI : MonoBehaviour
{
    public NavMeshAgent nma;
    public Transform[] points;         // Ѳ��·����
        private int index = 0;             // ��ǰĿ�������
    
        public float speed = 1.0f;         // �ƶ��ٶ�
        public float rotationSpeed = 5f;   // ��ת�ٶȣ�����ת��ƽ���ȣ�
        private Vector3 target;            // ��ǰĿ�������
    
        private bool isGoingToLight = false;   // �Ƿ�����ǰ���ƹ�
        public Transform lightTrans;           // �ƹ��λ��
    
        public Animator pointLightAnimator;    // �ƹ�� Animator
        private string flashStateName = "LightBlink";  // ��˸������״̬��
    
        private void Start()
        {
            target = points[index].position;
        }
    
        private void Update()
        {
            // ��鶯����ǰ�Ƿ�����˸״̬
            AnimatorStateInfo animState = pointLightAnimator.GetCurrentAnimatorStateInfo(0);
            if (animState.IsName(flashStateName))
            {
                isGoingToLight = true;
            }
    
            if (!isGoingToLight)
            {
                MoveTowards(target);
    
                if (Vector3.Distance(transform.position, target) < 0.1f)
                {
                    index++;
                    if (index >= points.Length)
                    {
                        index = 0;
                    }
                    target = points[index].position;
                }
            }
            else
            {
                MoveTowards(lightTrans.position);
            }
        }
    
        private void MoveTowards(Vector3 destination)
        {
            /*Vector3 direction = destination - transform.position;
    
            if (direction != Vector3.zero)
            {
                // ƽ����ת����Ŀ�귽��
                Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }*/
    
            // ��Ŀ���ƶ�
            //transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
            
            nma.SetDestination(destination);
        }
}
