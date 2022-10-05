using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

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

    //이벤트최초시간과 마지막시간
    public int firstEventTime;
    public int finalEventTime;
    //드론쇼 시작 시간
    [Header("이벤트시작시간")]
    public int es_hours, es_minutes, es_seconds;
    //public bool isShowing = false;

    private TcpClient socketConnection;
    private Thread clientReceiveThread;
    public string serverIp = "flyingart-server.8hlab.com";
    string serverMessage;
    string playMode;


    private void Awake()
    {
        savedTimeData = 1; // PlayerPrefs.GetInt("timeStandard");
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
        if (standard==StandardTimeEvent.hybrid)
        {
            totalCurrentSeconds = 0;

        }


        totalCurrentSeconds = 0;

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
        ConnectToTcpServer();
    }
    private void ConnectToTcpServer()
    {
        try
        {
            clientReceiveThread = new Thread(new ThreadStart(ListenForData));
            clientReceiveThread.IsBackground = true;
            clientReceiveThread.Start();
        }
        catch (Exception e)
        {
            Debug.Log("On client connect exception " + e);
        }
    }

    private void ListenForData()
    {
        try
        {
            socketConnection = new TcpClient(this.serverIp, 8090);
            Byte[] bytes = new Byte[1024];
            while (true)
            {
                // Get a stream object for reading 				
                using (NetworkStream stream = socketConnection.GetStream())
                {
                    int length;
                    // Read incomming stream into byte arrary. 					
                    while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        var incommingData = new byte[length];
                        Array.Copy(bytes, 0, incommingData, 0, length);
                        // Convert byte array to string message. 						
                        serverMessage = Encoding.ASCII.GetString(incommingData);


                        if ("PLAY".Equals(serverMessage))
                        {
                            totalCurrentSeconds = 0;
                            sT.totalGameSeconds = 0;
                        }

                        if ("STOP".Equals(serverMessage))
                        {
                            totalCurrentSeconds = 60 * 60 * 12;
                            sT.totalGameSeconds = totalCurrentSeconds;
                        }

                        Debug.Log("server message received as: " + serverMessage);
                    }
                }
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }


    private void Update()
    {
        //interval 마다 이벤트 체크..
        if (Time.time >= nextTime)
        {
            currentTime();
            approximationTime(eventList.ToArray());
            nextTime += interval;

        }

        /*
        if(temp_nextEvent == null)
        {
            if (totalCurrentSeconds < firstEventTime) GameObject.Find("--Event_Start----").transform.GetChild(0).gameObject.SetActive(true);
            else GameObject.Find("--Event_Start----").transform.GetChild(0).gameObject.SetActive(false);

            if (totalCurrentSeconds > finalEventTime) GameObject.Find("--Event_End----").transform.GetChild(0).gameObject.SetActive(true);
            else GameObject.Find("--Event_End----").transform.GetChild(0).gameObject.SetActive(false);

        }
        */

        if (temp_nextEvent != null)
        {

            if (totalCurrentSeconds > nextEventTime && totalCurrentSeconds < endEvevtTime && !temp_nextEvent.eventPrefab.activeSelf)
            {
                temp_nextEvent.eventPrefab.SetActive(true);
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
        firstEventTime = eventList[0].secondForEvent;
        print($"first event time is = {eventList[0].secondForEvent}");
        finalEventTime = eventList[eventList.Count - 1].secondForEndEvent;
        print($"final event time is = {eventList[eventList.Count - 1].secondForEndEvent}");
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
                totalCurrentSeconds = (int)sT.totalGameSeconds;
                //totalCurrentSeconds = (DateTime.Now.Hour * 60 * 60) + (DateTime.Now.Minute * 60) + DateTime.Now.Second;
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
            GUI.Label(new Rect(100, 200, 200, 40), "설정된 스피드 : " +  sT.speed, guiStyle);

            //GUI.Label(new Rect(100, 200, 200, 40), "이벤트종료 시간초 : " +(nextEventTime / 60) / 60 % 24 + "시"+ endminutes.ToString() + "분" + endEvevtTime.ToString() + "초/", guiStyle);

        }
    }
}

