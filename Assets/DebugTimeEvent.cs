using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTimeEvent : MonoBehaviour
{
    //public currentTime cT;
    private SpecifyTime sT;

    //시스템 날짜 받아오면서 0시0분0초로 초기화
    private int nextEventTime;
    private int endEvevtTime, min = int.MaxValue;

    //이벤트 생성 시간 리스트
    public List<TimeEventAttribute> eventList = new List<TimeEventAttribute>();
    //다음이벤트
    public TimeEventAttribute temp_nextEvent;
    public List<ParticleSystem> par;
    public bool IsKillParticle = false;



    public float interval = 0.25f;

    //임의의 변수
    private float nextTime = 0;

    public int totalCurrentSeconds;

    private void Awake()
    {
        sT = GameObject.FindObjectOfType<SpecifyTime>();

        eventList.AddRange(this.GetComponentsInChildren<TimeEventAttribute>());
  
    }
    private void Start()
    {
        //일단오브젝트 숨기기
        for (int i = 0; i < eventList.Count; i++)
        {
            eventList[i].eventPrefab.SetActive(false);
        }
        //기간기준가져오기
        //currentTime();
        totalCurrentSeconds = (int)sT.totalGameSeconds;
        approximationTime(eventList.ToArray());

    }
    private void Update()
    {
        //if (Time.time >= nextTime)
        //{
        //    print("check");
        //    totalCurrentSeconds = (int)sT.totalGameSeconds;
        //    approximationTime(eventList.ToArray());
        //    nextTime += interval;
        //}
        //if (temp_nextEvent != null)
        //{
        //    if (totalCurrentSeconds > nextEventTime && totalCurrentSeconds < endEvevtTime && !temp_nextEvent.eventPrefab.activeSelf)
        //    {
        //        temp_nextEvent.eventPrefab.SetActive(true);

        //        print("show");
        //    }


        //    if (totalCurrentSeconds > endEvevtTime - 1 && temp_nextEvent.eventPrefab.activeSelf && !IsKillParticle)
        //    {
        //        print("hide");
        //        print($"현재시간보다 종료시간보다 크다");
        //        temp_nextEvent.eventPrefab.SetActive(false);
        //        //approximationTime(eventList.ToArray());
        //    }

        //}

    }



    private void approximationTime(TimeEventAttribute[] eventlist)
    {
        int eventlistT = 0;
        for (int i = 0; i < eventlist.Length; i++)
        {
            eventlistT = eventlist[i].secondForEvent;
            endEvevtTime = eventlist[i].secondForEndEvent;
            Debug.Log($"리스트({eventlist[i].name})");
            Debug.Log($"현재시간({totalCurrentSeconds})은 이벤트시작시간{eventlistT} 보다 크다는 비교는{totalCurrentSeconds > eventlistT}");
            if (totalCurrentSeconds > eventlistT && totalCurrentSeconds < endEvevtTime)
            {
                    min = Mathf.Abs(eventlistT - totalCurrentSeconds);
                    Debug.Log($"최소값은 {min}");
                    temp_nextEvent = eventlist[i];
                    Debug.Log("탬프에 리스느(" + eventlist[i].name + ")에 넣기");
                    nextEventTime = eventlistT;
                    endEvevtTime = eventlist[i].secondForEndEvent;

            }
    
        }

    }
}
