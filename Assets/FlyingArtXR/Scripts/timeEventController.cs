using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;




[Serializable]
public enum StandardTimeEvent
{
    system,
    specified,
    hybrid
}

//[RequireComponent(typeof(SpecifyTime))]
public class timeEventController : MonoBehaviour
{
    //public enum StandardTimeEvent
    //{
    //    system,
    //    specified,
    //    hybrid
    //}
    private GUIStyle guiStyle = new GUIStyle();
    //public currentTime cT;
    private SpecifyTime sT;

    [Tooltip("현재기준인지 알람기준인지 선택")]
    public StandardTimeEvent standard = StandardTimeEvent.system;

    //시스템 날짜 받아오면서 0시0분0초로 초기화
    private DateTime TimeStandard = DateTime.Today;
    private int nextEventTime;
    private int endEvevtTime, min = int.MaxValue;

    //이벤트 생성 시간 리스트
    public List<TimeEventAttribute> eventList = new List<TimeEventAttribute>();
    //다음이벤트
    public TimeEventAttribute temp_nextEvent;
    public List<ParticleSystem> par;
    private ParticleSystem.Particle[] m_Particles;
    private bool IsKillParticle = false;


    [Tooltip("화면에 시간 표시")]
    public bool isGuiDebug = false;
    [Tooltip("이벤트 체크 타임")]
    public float interval = 0.25f;
    [HideInInspector]
    public int totalCurrentSeconds;
    //임의의 변수
    private float nextTime = 0;
    private int savedTimeData;

    //public bool isShowing = false;

    private void Awake()
    {
        savedTimeData = PlayerPrefs.GetInt("timeStandard");
        if (standard==StandardTimeEvent.hybrid)
        {
            switch (savedTimeData)
            {
                case 0:
                    standard = StandardTimeEvent.system;
                    print($"시간기준은 = " + standard);
                    break;
                case 1:
                    standard = StandardTimeEvent.specified;
                    print($"시간기준은 = " + standard);
                    break;
                case 2:
                    standard = StandardTimeEvent.hybrid;
                    print($"시간기준은 = " + standard);
                    break;
                default:
                    standard = StandardTimeEvent.system;
                    break;
            }

        }

        //sT = this.GetComponent<SpecifyTime>();
        sT = GameObject.FindObjectOfType<SpecifyTime>();

        //타이머이벤트리스트생성
        //eventList.AddRange(GameObject.FindObjectsOfType<TimeEventAttribute>());
        eventList.AddRange(this.GetComponentsInChildren<TimeEventAttribute>());


    }
    private void OnEnable()
    {
        //일단오브젝트 숨기기
        for (int i = 0; i < eventList.Count; i++)
        {
            eventList[i].eventPrefab.SetActive(false);
        }
        //기간기준가져오기
        currentTime();

        approximationTime(eventList.ToArray());

    }
    private void Start()
    {

    }

    private void Update()
    {


        if (Time.time >= nextTime)
        {
            currentTime();
            approximationTime(eventList.ToArray());
            nextTime += interval;

        }
        if (temp_nextEvent != null)
        {
            if (totalCurrentSeconds > nextEventTime && totalCurrentSeconds < endEvevtTime && !temp_nextEvent.eventPrefab.activeSelf)
            {
                temp_nextEvent.eventPrefab.SetActive(true);

                print("show");
            }

            //파티클 서서히 사라지게 하기위해  3 먼저 처리.
            if (totalCurrentSeconds > endEvevtTime - 4 && temp_nextEvent.eventPrefab.activeSelf && par[0].particleCount > 1 && !IsKillParticle)
            {
                if (par != null)
                    ReadyKillParticl();
            }

            if (totalCurrentSeconds > endEvevtTime - 1 && temp_nextEvent.eventPrefab.activeSelf && !IsKillParticle)
            {
                print("hide");
                temp_nextEvent.eventPrefab.SetActive(false);
            }

        }

    }


    /// </summary>
    /// <param name = "가까운 이벤트나 진행중인 이벤트 " ></ param >
    private void approximationTime(TimeEventAttribute[] eventlist)
    {
        int eventlistT = 0;
        for (int i = 0; i < eventlist.Length; i++)
        {
            eventlistT = eventlist[i].secondForEvent;
            //Debug.Log($"리스트({eventlist[i].name})");
            //Debug.Log($"현재시간({totalCurrentSeconds})은 이벤트시작시간{eventlistT} 보다 크다는 비교는{totalCurrentSeconds > eventlistT}");
            if (totalCurrentSeconds > eventlistT && totalCurrentSeconds < eventlist[i].secondForEndEvent)
            {
                min = Mathf.Abs(eventlistT - totalCurrentSeconds);
                //Debug.Log($"최소값은 {min}");
                temp_nextEvent = eventlist[i];
                //Debug.Log("탬프에 리스느(" + eventlist[i].name + ")에 넣기");
                nextEventTime = eventlistT;
                endEvevtTime = eventlist[i].secondForEndEvent;
            }
            if (temp_nextEvent != null)
                StartCoroutine(GetParticlList());
        }
    }

    IEnumerator GetParticlList()
    {
        yield return new WaitForEndOfFrame();
        par.Clear();
        for (int j = 0; j < temp_nextEvent.transform.GetChild(0).childCount; j++)
        {
            if (temp_nextEvent.transform.GetChild(0).GetChild(j).GetComponent<ParticleSystem>() != null)
                par.Add(temp_nextEvent.transform.GetChild(0).GetChild(j).GetComponent<ParticleSystem>());
        }
    }

    private void ReadyKillParticl()
    {
        IsKillParticle = true;
        if (par != null)
        {
            for (int i = 0; i < par.Count; i++)
            {
                float eRate = 0;
                var emission = par[i].emission;
                emission.rateOverTime = eRate;
                StartCoroutine(KillParticle(par[i]));
            }

        }
    }
    //파티클 수가 0이면 오브젝트를 끈다.
    IEnumerator KillParticle(ParticleSystem m_system)
    {

        InitializeIfNeeded(m_system);
        int numParticlesAlive = m_system.GetParticles(m_Particles);

        while (m_system.particleCount > 1)
        {
            for (int i = 0; i < numParticlesAlive; i++)
            {
                if (m_Particles[i].remainingLifetime > 2) m_Particles[i].remainingLifetime = 2;
                else continue;

                m_Particles[i].remainingLifetime -= i * 0.1f;
                m_system.SetParticles(m_Particles);
            }

            yield return new WaitForEndOfFrame();
        }
        yield return new WaitUntil(() => m_system.particleCount < 1);
        print("here");
        m_system.gameObject.SetActive(false);
        IsKillParticle = false;
    }


    private void InitializeIfNeeded(ParticleSystem m_System)
    {
        if (m_System == null)
            m_System = GetComponent<ParticleSystem>();

        if (m_Particles == null || m_Particles.Length < m_System.main.maxParticles)
            m_Particles = new ParticleSystem.Particle[m_System.main.maxParticles];
    }

    private int currentTime()
    {
        switch (standard)
        {
            case StandardTimeEvent.system:
                totalCurrentSeconds = (DateTime.Now.Hour * 60 * 60) + (DateTime.Now.Minute * 60) + DateTime.Now.Second;
                break;
            case StandardTimeEvent.specified:
                totalCurrentSeconds = (int)sT.totalGameSeconds;
                break;
            case StandardTimeEvent.hybrid:
                totalCurrentSeconds = (DateTime.Now.Hour * 60 * 60) + (DateTime.Now.Minute * 60) + DateTime.Now.Second;
                break;
        }

        return totalCurrentSeconds;
    }


















    void OnGUI()
    {
        guiStyle.fontSize = 40;
        guiStyle.normal.textColor = Color.white;

        int endminutes = endEvevtTime / 60;

        if (isGuiDebug)
        {
            if (standard == StandardTimeEvent.specified)
                GUI.Label(new Rect(100, 40, 200, 40), "설정된 시간 : " + sT.time, guiStyle);
            else if (standard == StandardTimeEvent.hybrid)
                GUI.Label(new Rect(100, 40, 200, 40), "설정된 시간 : " + sT.time, guiStyle);
            else
                GUI.Label(new Rect(100, 40, 200, 40), "설정된 시간 : " + DateTime.Now, guiStyle);

            GUI.Label(new Rect(100, 80, 200, 40), "현재 시간초 : " + totalCurrentSeconds.ToString(), guiStyle);
            GUI.Label(new Rect(100, 160, 200, 40), "다음이벤트 시간초 : " + nextEventTime.ToString(), guiStyle);
            GUI.Label(new Rect(100, 200, 200, 40), "이벤트종료 시간초 : " + endminutes.ToString() + "분" + endEvevtTime.ToString() + "초/", guiStyle);

        }
    }
}

