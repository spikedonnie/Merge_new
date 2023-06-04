using Spine.Unity.AttachmentTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EPoolName
{
    Melee_1,
    Melee_2,
    Melee_3,
    Melee_4,
    Arrow_1,
    Arrow_2,
    Arrow_3,
    Magic_1,
    Magic_2,
    Magic_3,
    Magic_4,
    Character,
    MergeEffect,
    HeroHit,
    Coin,
    Projectile,
    JumpEffect,
}

public enum EUIPoolName
{
    //사용할 풀종류의 이름을 정하세요.
    HpBar,

    HeroName
}

public class ObjectPool : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public EPoolName poolName;
        public int quantity;
        public GameObject prefab;
        public int layer;

        [HideInInspector]
        public Transform poolTr;

        public Queue<GameObject> poolingObjectQueue = new Queue<GameObject>();
    }

    [System.Serializable]
    public class UIPool
    {
        public EUIPoolName poolName;
        public int quantity;
        public GameObject prefab;
        public Queue<GameObject> poolingObjectQueue = new Queue<GameObject>();
    }

    [SerializeField] public Pool[] pools;
    [SerializeField] public UIPool[] uIPools;
    [SerializeField] private Transform uiPoolParent = null;

    public static ObjectPool instance;

    public List<GameObject> normalMonsterGameObjectList;
    public List<GameObject> uniqueEnemyGameObjectList;


    #region internal

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        normalMonsterGameObjectList = new List<GameObject>(Utils.gameResources.normalMonsterList.Count);
        uniqueEnemyGameObjectList = new List<GameObject>(Utils.gameResources.uniqueMonsterList.Count);

        MakeMonsterPool();
        CreatePoolsTr();
        CreateUIPool();


    }

    
    private void MakeMonsterPool()
    {
        GameObject poolTr2 = new GameObject("CommonEnemy");
        poolTr2.transform.SetParent(this.transform);

        for (int i = 0; i < Utils.gameResources.normalMonsterList.Count; i++)
        {
            GameObject obj = Instantiate(Utils.GetNormalMonsterName(i));
            obj.transform.SetParent(poolTr2.transform);

            obj.SetActive(false);
            normalMonsterGameObjectList.Add(obj);
        }

        GameObject poolTr3 = new GameObject("UniqueEnemy");
        poolTr3.transform.SetParent(this.transform);

        for (int i = 0; i < Utils.gameResources.uniqueMonsterList.Count; i++)
        {
            GameObject obj = Instantiate(Utils.GetUniqueMonsterName(i));
            obj.transform.SetParent(poolTr3.transform);

            obj.SetActive(false);
            uniqueEnemyGameObjectList.Add(obj);
        }

    }

    public GameObject GetNormalMonster(int index)
    {
        normalMonsterGameObjectList[index].SetActive(true);
        return normalMonsterGameObjectList[index];
    }

    public GameObject GetUniqueMonster(int index)
    {
        uniqueEnemyGameObjectList[index].SetActive(true);
        return uniqueEnemyGameObjectList[index];
    }

    public void ReturnNormalMonster(int index)
    {
        normalMonsterGameObjectList[index].SetActive(false);
    }

    public void ReturnUniqueMonster(int index)
    {
        uniqueEnemyGameObjectList[index].SetActive(false);
    }

    // CreatePoolsTr 풀이 될 부모객체를 생성
    private void CreatePoolsTr()
    {
        for (int i = 0; i < pools.Length; i++)
        {
            GameObject poolTr = new GameObject(pools[i].poolName.ToString());
            pools[i].poolTr = poolTr.transform;
            poolTr.transform.SetParent(this.transform);
            PooiInitialize(pools[i], pools[i].quantity);
        }
    }

    private void CreateUIPool()
    {
        for (int i = 0; i < uIPools.Length; i++)
        {
            PooiInitialize(uIPools[i], uIPools[i].quantity);
        }
    }

    //풀의 오브젝트 큐 리스트에 원하는 수량만큼 Init시킴
    private void PooiInitialize(Pool pool, int initCount)
    {
        for (int i = 0; i < initCount; i++)
        {
            pool.poolingObjectQueue.Enqueue(CreateNewObject(pool));
        }
    }

    private void PooiInitialize(UIPool pool, int initCount)
    {
        for (int i = 0; i < initCount; i++)
        {
            pool.poolingObjectQueue.Enqueue(CreateNewObject(pool));
        }
    }

    //풀에 들어갈 객체들을 생성하여 반환
    private GameObject CreateNewObject(Pool pool)
    {
        var newObj = Instantiate(pool.prefab);
        newObj.gameObject.SetActive(false);
        newObj.transform.SetParent(pool.poolTr);
        newObj.layer = pool.layer;
        return newObj;
    }

    private GameObject CreateNewObject(UIPool pool)
    {
        var newObj = Instantiate(pool.prefab);
        newObj.gameObject.SetActive(false);
        newObj.transform.SetParent(uiPoolParent);
        return newObj;
    }

    //풀타입에 따른 풀을 찾아옵니다
    private Pool FindPoolByPoolType(EPoolName poolType)
    {
        for (int i = 0; i < pools.Length; i++)
        {
            if (pools[i].poolName == poolType)
            {
                return pools[i];
            }
        }
        return null;
    }

    private UIPool FindPoolByPoolType(EUIPoolName poolType)
    {
        for (int i = 0; i < pools.Length; i++)
        {
            if (uIPools[i].poolName == poolType)
            {
                return uIPools[i];
            }
        }
        return null;
    }

    #endregion internal

    public GameObject GetObject(EPoolName poolName)
    {
        Pool currentPool = FindPoolByPoolType(poolName);
        if (currentPool == null) Debug.LogError($"<color=red>[{poolName}]</color>의 풀을 찾지 못했습니다. 이름을 확인해주세요  </color>");

        if (currentPool.poolingObjectQueue.Count > 0)
        {
            var obj = currentPool.poolingObjectQueue.Dequeue();
            //obj.transform.SetParent(null);
            obj.gameObject.SetActive(true);
            return obj;
        }
        else
        {
            var newObj = CreateNewObject(currentPool);
            newObj.gameObject.SetActive(true);
            //newObj.transform.SetParent(null);
            return newObj;
        }
    }

    public GameObject GetObject(EUIPoolName poolName)
    {
        UIPool currentPool = FindPoolByPoolType(poolName);
        if (currentPool == null) Debug.LogError($"<color=red>[{poolName}]</color>의 풀을 찾지 못했습니다. 이름을 확인해주세요  </color>");

        if (currentPool.poolingObjectQueue.Count > 0)
        {
            var obj = currentPool.poolingObjectQueue.Dequeue();
            obj.gameObject.SetActive(true);
            return obj;
        }
        else
        {
            var newObj = CreateNewObject(currentPool);
            newObj.gameObject.SetActive(true);
            return newObj;
        }
    }

    public void ReturnObject(GameObject obj, EPoolName poolName)
    {
        Pool currentPool = FindPoolByPoolType(poolName);
        if (currentPool == null) Debug.LogError($"<color=red>[{poolName}]</color>의 풀을 찾지 못했습니다. 이름을 확인해주세요.  </color>");
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(currentPool.poolTr.transform);
        currentPool.poolingObjectQueue.Enqueue(obj);
    }

    public void ReturnObject(GameObject obj, EUIPoolName poolName)
    {
        UIPool currentPool = FindPoolByPoolType(poolName);
        if (currentPool == null) Debug.LogError($"<color=red>[{poolName}]</color>의 풀을 찾지 못했습니다. 이름을 확인해주세요.  </color>");
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(uiPoolParent);
        currentPool.poolingObjectQueue.Enqueue(obj);
    }

    //딜레이 리턴용 메소드 오버라이딩
    public void ReturnObject(GameObject obj, float delay, EPoolName poolType)
    {
        StartCoroutine(Co_ReturnObject(obj, delay, poolType));
    }

    private IEnumerator Co_ReturnObject(GameObject obj, float delay, EPoolName poolType)
    {
        yield return new WaitForSeconds(delay);
        ReturnObject(obj, poolType);
    }
}