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

    [Tooltip("����������� �˶��������� ����")]
    public StandardTimeEvent standard = StandardTimeEvent.system;

    //�ý��� ��¥ �޾ƿ��鼭 0��0��0�ʷ� �ʱ�ȭ
    private DateTime TimeStandard = DateTime.Today;
    private int nextEventTime;
    private int endEvevtTime, min = int.MaxValue;

    //�̺�Ʈ ���� �ð� ����Ʈ
    public List<TimeEventAttribute> eventList = new List<TimeEventAttribute>();
    //�����̺�Ʈ
    public TimeEventAttribute temp_nextEvent;
    public List<ParticleSystem> par;
    private ParticleSystem.Particle[] m_Particles;
    private bool IsKillParticle = false;


    [Tooltip("ȭ�鿡 �ð� ǥ��")]
    public bool isGuiDebug = false;
    [Tooltip("�̺�Ʈ üũ Ÿ��")]
    public float interval = 0.25f;
    [HideInInspector]
    public int totalCurrentSeconds;
    //������ ����
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
                    print($"�ð������� = " + standard);
                    break;
                case 1:
                    standard = StandardTimeEvent.specified;
                    print($"�ð������� = " + standard);
                    break;
                case 2:
                    standard = StandardTimeEvent.hybrid;
                    print($"�ð������� = " + standard);
                    break;
                default:
                    standard = StandardTimeEvent.system;
                    break;
            }

        }

        //sT = this.GetComponent<SpecifyTime>();
        sT = GameObject.FindObjectOfType<SpecifyTime>();

        //Ÿ�̸��̺�Ʈ����Ʈ����
        //eventList.AddRange(GameObject.FindObjectsOfType<TimeEventAttribute>());
        eventList.AddRange(this.GetComponentsInChildren<TimeEventAttribute>());


    }
    private void OnEnable()
    {
        //�ϴܿ�����Ʈ �����
        for (int i = 0; i < eventList.Count; i++)
        {
            eventList[i].eventPrefab.SetActive(false);
        }
        //�Ⱓ���ذ�������
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

            //��ƼŬ ������ ������� �ϱ�����  3 ���� ó��.
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
    /// <param name = "����� �̺�Ʈ�� �������� �̺�Ʈ " ></ param >
    private void approximationTime(TimeEventAttribute[] eventlist)
    {
        int eventlistT = 0;
        for (int i = 0; i < eventlist.Length; i++)
        {
            eventlistT = eventlist[i].secondForEvent;
            //Debug.Log($"����Ʈ({eventlist[i].name})");
            //Debug.Log($"����ð�({totalCurrentSeconds})�� �̺�Ʈ���۽ð�{eventlistT} ���� ũ�ٴ� �񱳴�{totalCurrentSeconds > eventlistT}");
            if (totalCurrentSeconds > eventlistT && totalCurrentSeconds < eventlist[i].secondForEndEvent)
            {
                min = Mathf.Abs(eventlistT - totalCurrentSeconds);
                //Debug.Log($"�ּҰ��� {min}");
                temp_nextEvent = eventlist[i];
                //Debug.Log("������ ������(" + eventlist[i].name + ")�� �ֱ�");
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
    //��ƼŬ ���� 0�̸� ������Ʈ�� ����.
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
                GUI.Label(new Rect(100, 40, 200, 40), "������ �ð� : " + sT.time, guiStyle);
            else if (standard == StandardTimeEvent.hybrid)
                GUI.Label(new Rect(100, 40, 200, 40), "������ �ð� : " + sT.time, guiStyle);
            else
                GUI.Label(new Rect(100, 40, 200, 40), "������ �ð� : " + DateTime.Now, guiStyle);

            GUI.Label(new Rect(100, 80, 200, 40), "���� �ð��� : " + totalCurrentSeconds.ToString(), guiStyle);
            GUI.Label(new Rect(100, 160, 200, 40), "�����̺�Ʈ �ð��� : " + nextEventTime.ToString(), guiStyle);
            GUI.Label(new Rect(100, 200, 200, 40), "�̺�Ʈ���� �ð��� : " + endminutes.ToString() + "��" + endEvevtTime.ToString() + "��/", guiStyle);

        }
    }
}

