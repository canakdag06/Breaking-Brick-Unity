using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ParticleType
{
    BrickHit,
    BrickBreak,
    UnBreakableHit
}


public class ParticlePool : MonoBehaviour
{
    public static ParticlePool Instance { get; private set; }

    [System.Serializable]
    public class ParticleEntry
    {
        public ParticleType type;
        public GameObject prefab;
        public int poolSize = 10;
    }

    [SerializeField] private List<ParticleEntry> particleEntries;

    private Dictionary<ParticleType, Queue<GameObject>> poolDictionary;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        poolDictionary = new Dictionary<ParticleType, Queue<GameObject>>();

        foreach (var entry in particleEntries)
        {
            Queue<GameObject> queue = new Queue<GameObject>();

            for (int i = 0; i < entry.poolSize; i++)
            {
                GameObject obj = Instantiate(entry.prefab, transform);
                obj.SetActive(false);
                queue.Enqueue(obj);
            }
            poolDictionary.Add(entry.type, queue);
        }
    }


    public void Play(ParticleType type, Vector3 pos, Color color)
    {
        if(!poolDictionary.ContainsKey(type))
        {
            Debug.LogError("Particle type not found in pool: " + type);
            return;
        }

        GameObject obj = poolDictionary[type].Dequeue();
        obj.transform.position = pos;
        obj.SetActive(true);

        var ps = obj.GetComponent<ParticleSystem>();
        var main = ps.main;
        main.startColor = color;
        ps.Play();

        float duration = obj.GetComponent<ParticleSystem>()?.main.duration ?? 1f;
        //StartCoroutine(DisableAfter(obj, duration));

        poolDictionary[type].Enqueue(obj);
    }

    private IEnumerator DisableAfter(GameObject obj, float time)
    {
        yield return new WaitForSeconds(time);
        obj.SetActive(false);
    }
}
