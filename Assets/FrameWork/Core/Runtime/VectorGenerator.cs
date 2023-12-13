using System.Collections.Generic;
using UnityEngine;


namespace TOONIPLAY.Utilities
{
    public enum SideType
    {
        None,
        Top,
        Down,
        Left,
        Right
    }

    public static class VectorGenerator
    {
        /// <summary>
        /// 무작위 3차원 방향 벡터를 생성한다.
        /// </summary>
        /// <returns></returns>
        public static Vector3 GetRandomDirection()
        {
            var randomX = Random.Range(-1f, 1f);
            var randomY = Random.Range(-1f, 1f);
            var randomZ = Random.Range(-1f, 1f);

            var randomDirection = new Vector3(randomX, randomY, randomZ);

            return randomDirection.normalized;
        }

        /// <summary>
        /// 플레이어 중심으로 작은 원과 큰 원 사이의 임의의 영역 중 어느 한 곳을 반환한다.
        /// </summary>
        /// <param name="size"></param>
        /// <param name="center"></param>
        /// <param name="smallRadius"></param>
        /// <param name="bigRadius"></param>
        /// <returns></returns>
        public static Vector3 GetRandomPointInCircle(Vector3 center, float smallRadius, float bigRadius)
        {
            var angle = Random.Range(0f, 2f * Mathf.PI);
            var distance = Mathf.Sqrt(Random.Range(smallRadius * smallRadius, bigRadius * bigRadius));

            var x = center.x + distance * Mathf.Cos(angle);
            var z = center.z + distance * Mathf.Sin(angle);

            return new Vector3(x, 0, z);
        }

        /// <summary>
        /// 플레이어 위치를 중점으로 하는 원 위의 점을 동일한 간격으로 생성한다.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="count"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static Vector3[] GetPointInCircleByCount(Vector3 center, int count, float distance)
        {
            List<Vector3> result = new List<Vector3>();

            var angle = (Mathf.PI * 2f) / count;

            for (int i = 0; i < count; i++)
            {
                var x = center.x + distance * Mathf.Cos(angle * i);
                var z = center.z + distance * Mathf.Sin(angle * i);

                result.Add(new Vector3(x, 0, z));
            }

            return result.ToArray();
        }

        /// <summary>
        /// 플레이어 중심으로 직사각형의 임의의 영역 중 어느 한 곳을 반환한다.
        /// </summary>
        /// <returns></returns>
        public static Vector3 GetRandomPointInRect(Vector3 center, float width, float height, float widthRanVal, float heightRanVal)
        {
            int sign = Random.Range(0, 2) * 2 - 1; // 0부터 1까지의 정수를 반환

            return new Vector3(center.x + (sign * width) + (sign * Random.Range(0, widthRanVal)),
                               center.y,
                               center.z + (sign * height) + (sign * Random.Range(0, heightRanVal)));
        }

        /// <summary>
        /// 스크린 화면 내의 랜덤한 좌표를 반환하는 함수
        /// </summary>
        /// <param name="pos">현재 캐릭터의 위치</param>
        /// <returns></returns>
        public static Vector3 GetRandomScreenWorldPoint()
        {
            var mainCamera = Camera.main;
            if (mainCamera == null)
                return Vector3.zero;

            var worldPointLeftBottom = CalculateCameraRayToWorldPoint(0, 0);
            var worldPointRightTop = CalculateCameraRayToWorldPoint(Screen.width, Screen.height);

            var x = Random.Range(worldPointLeftBottom.x, worldPointRightTop.x);
            var z = Random.Range(worldPointLeftBottom.z, worldPointRightTop.z);

            return new Vector3(x, 0f, z); 
        }
        
        /// <summary>
        /// 스크린 화면 내의 외각 부분의 좌표를 반환하는 함수
        /// </summary>
        /// <returns></returns>
        public static Vector3 GetRandomScreenOutlineWorldPoint(SideType sideType = SideType.None)
        {
            var mainCamera = Camera.main;
            if (mainCamera == null)
                return Vector3.zero;

            var worldPointLeftBottom = CalculateCameraRayToWorldPoint(0, 0);
            var worldPointLeftTop = CalculateCameraRayToWorldPoint(0, Screen.height);
            var worldPointRightBottom = CalculateCameraRayToWorldPoint(Screen.width, 0);
            var worldPointRightTop = CalculateCameraRayToWorldPoint(Screen.width, Screen.height);

            var area = Random.Range(0, 4);

            switch (sideType)
            {
                case SideType.None:
                    {
                        switch (area)
                        {
                            case 0: // 상
                                {
                                    var randomX = Random.Range(worldPointLeftTop.x, worldPointRightTop.x);
                                    return new Vector3(randomX, 0f, worldPointLeftTop.z);
                                }
                            case 1: // 하
                                {
                                    var randomX = Random.Range(worldPointLeftBottom.x, worldPointRightBottom.x);
                                    return new Vector3(randomX, 0f, worldPointLeftTop.z);
                                }
                            case 2: // 좌
                                {
                                    var randomZ = Random.Range(worldPointLeftBottom.z, worldPointLeftTop.z);
                                    return new Vector3(worldPointLeftTop.x, 0f, randomZ);
                                }
                            case 3:// 우
                                {
                                    var randomZ = Random.Range(worldPointRightBottom.z, worldPointRightTop.z);
                                    return new Vector3(worldPointRightTop.x, 0f, randomZ);
                                }
                        }
                    }
                    break;
                case SideType.Top:
                    {
                        var randomX = Random.Range(worldPointLeftTop.x, worldPointRightTop.x);
                        return new Vector3(randomX, 0f, worldPointLeftTop.z);
                    }
                case SideType.Down:
                    {
                        var randomX = Random.Range(worldPointLeftBottom.x, worldPointRightBottom.x);
                        return new Vector3(randomX, 0f, worldPointLeftTop.z);
                    }
                case SideType.Left:
                    {
                        var randomZ = Random.Range(worldPointLeftBottom.z, worldPointLeftTop.z);
                        return new Vector3(worldPointLeftTop.x, 0f, randomZ);
                    }
                case SideType.Right:
                    {
                        var randomZ = Random.Range(worldPointRightBottom.z, worldPointRightTop.z);
                        return new Vector3(worldPointRightTop.x, 0f, randomZ);
                    }
                default: { }
                    break;
            }

            return Vector3.zero;
        }

        static Vector3 CalculateCameraRayToWorldPoint(int x, int y)
        {
            var vec = new Vector3(x, y, 0f);
            var ray = Camera.main.ScreenPointToRay(vec);

            Physics.Raycast(ray, out var result, Camera.main.farClipPlane, LayerMask.GetMask("Ground"));

            if (result.collider != null)
            {
                return result.point;
            }
            else
            {
                return Vector3.zero;
            }
        }
    }
}