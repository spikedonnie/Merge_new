using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    private Vector3 _target;
    [SerializeField] Character slot;

    MergeController mergeController;

    private void Start()
    {
        mergeController = GameController.instance.mergeController;
    }

    private void Update()
    {
        if (GameController.instance.IsGameStop) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameController.instance.EndGamePopUp();
        }


        if (EventSystem.current.IsPointerOverGameObject() == false)
        {
            if (Input.GetMouseButtonDown(0))
            {
                SendRayCast();
            }
            //잡고 있는중
            if (Input.GetMouseButton(0))
            {
                OnItemSelected();
            }

            if (Input.GetMouseButtonUp(0))
            {
                //Drop item
                ButtonUp();
            }
        }
    }

    private void SendRayCast()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), transform.forward);

        if (hit)
        {
            if (hit.collider.tag == "Character")
            {
                slot = hit.transform.GetComponent<Character>();
                slot.sortingLayer.sortingOrder = 100;
                slot.isSelected = true;
                //자동 머지가 진행 중이거나 아직 생성중이라면 슬롯 지정이 불가
                if (slot.IsGoingMerge || !slot.isStopSpawnMove || slot.isAutoSelected)
                {
                    slot.isSelected = false;
                    slot.ReturnLayerOder();
                    slot = null;
                }

            }
        }
 
    }

    private void OnItemSelected()
    {
        if (slot != null && slot.isStopSpawnMove)
        {
            //자동 머지가 진행중이라면 슬롯 지정 풀림
            if (slot.IsGoingMerge || slot.isAutoSelected || !slot.isStopSpawnMove)
            {
                slot.isSelected = false;
                slot.ReturnLayerOder();
                slot = null;
                return;
            }

            _target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _target.z = 0;
            if (_target.x <= Define.MIN_X)
                _target.x = Define.MIN_X;
            if (_target.x >= Define.MAX_X)
                _target.x = Define.MAX_X;
            if (_target.y <= Define.MIN_Y)
                _target.y = Define.MIN_Y;
            if (_target.y >= Define.MAX_Y)
                _target.y = Define.MAX_Y;

            _target.y = _target.y - 0.7f;
            slot.isSelected = true;

            slot.GrabAndMove(_target);

        }
    }

    private void ButtonUp()
    {
        if (slot != null)
        {
            //영웅 최데레벨이면 리턴
            if (mergeController.CheckHeroMaxLevel(slot.heroLv)) 
            {
                slot.isSelected = false;
                slot = null;
                return;
            }

            //만약 영웅이 잠김 상태라면 넘어감
            if (GameController.instance.CardManager.GetHeroLockState(slot.heroType))
            {
                slot.isSelected = false;
                slot = null;
                //UIController.instance.SendPopupMessage(EnumDefine.AlarmTYPE.HERO_LOCK);

                return;
            }


            //Slot과 같은 등급의 영웅이 모여 있는 리스트를 생성
            List<Character> equalGradeHeroList = mergeController.GetChractersByType(slot.heroType);
            //Debug.Log($"같은 등급 영웅 : {equalGradeHeroList.Count} 개 찾음");
            //같은 레벨의 가장 가까운 용병을 찾고
            Character target = GetNearTarget(equalGradeHeroList);

            if (target != null)
            {
                //Debug.Log("가까이에 같은 레벨의 용병이 있음");
                //영웅 오브젝트 풀로 돌려보내기
                target.DisableHero();
                //영웅 레벨 업
                slot.LevelUpHero();
                //돌려보낸 영웅 리스트에서 제거
                mergeController.RemoveCharacterList(target);


            }

            slot.isSelected = false;
            slot = null;


        }
    }

    //범위 내 가까운 영웅 찾기
    private Character GetNearTarget(List<Character> heroList)
    {
        float dist = 0.7f;//반경값이 들어가야함

        Character target = null;

        foreach (Character hero in heroList)
        {
            //이동 중이거나 자동 합치기중이라면 다음 영웅 검색
            if (hero.IsGoingMerge || hero.isAutoSelected || !hero.isStopSpawnMove || hero.isSelected)
            {
                continue;
            }

            float d = Vector2.Distance(slot.transform.position, hero.transform.position);
            //계산된 거리가 반경안에 들어오면
            if (d <= dist)
            {
                //반경을 더욱 좁힌다
                dist = d;
                target = hero;
            }
        }
        

        return target;

    }

    


}