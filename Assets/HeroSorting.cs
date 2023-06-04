using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroSorting : MonoBehaviour
{
    public Transform[] targetPositions; // 미리 정해진 위치들을 저장하는 배열
    private List<Vector3> visitedPositions = new List<Vector3>(); // 이미 방문한 위치들을 저장하는 리스트
    private int currentTargetIndex; // 현재 이동할 위치의 인덱스



    public void MoveToNextPosition(List<Character> heroList)
    {
        currentTargetIndex = 0;
        visitedPositions.Clear();
        // Check if there are still unvisited target positions
        while (currentTargetIndex < targetPositions.Length)
        {
            // heroList의 요소 수가 targetIndex보다 작으면 종료합니다.
            if (currentTargetIndex >= heroList.Count)
            {
                //Debug.Log("heroList 요소 수가 targetIndex보다 작습니다.");
                break;
            }

            Vector3 nextPosition = targetPositions[currentTargetIndex].position;

            // Check if the next position has already been visited by another character
            if (!visitedPositions.Contains(nextPosition))
            {
                // Move the character to the next position
                heroList[currentTargetIndex].ForceMove(nextPosition);

                // Add the position to the list of visited positions
                visitedPositions.Add(nextPosition);

            }

            // Otherwise, increment the target index and continue with the loop
            currentTargetIndex++;
        }

        // If all target positions have been visited, stop moving and return
        //Debug.Log("모든 위치를 방문했습니다.");
    }

}

