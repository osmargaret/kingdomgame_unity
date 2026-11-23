using UnityEngine;
using KingdomQuest.Bridge;

namespace KingdomQuest.Adventures
{
    /// <summary>
    /// Example 3D mission controller for Scene_TheExodus (Exodus 14:21).
    /// Assign left/right Red Sea water wall Transforms in the Unity Inspector.
    /// </summary>
    public class ExodusMissionController : MonoBehaviour
    {
        [Header("Scene References")]
        public Transform leftWaterWall;
        public Transform rightWaterWall;
        public float partDistance = 12f;
        public float partSpeed = 3f;

        private bool isParted;

        public void TriggerPartRedSea()
        {
            isParted = true;
            KingdomBridge.Instance?.SendMissionCompleted(
                missionId: "exodus-ch2-red-sea",
                collectibleId: "moses",
                scriptureRef: "Exodus 14:21",
                xpEarned: 40
            );
        }

        private void Update()
        {
            if (!isParted) return;

            if (leftWaterWall != null)
            {
                var targetLeft = new Vector3(-partDistance, leftWaterWall.position.y, leftWaterWall.position.z);
                leftWaterWall.position = Vector3.MoveTowards(leftWaterWall.position, targetLeft, partSpeed * Time.deltaTime);
            }

            if (rightWaterWall != null)
            {
                var targetRight = new Vector3(partDistance, rightWaterWall.position.y, rightWaterWall.position.z);
                rightWaterWall.position = Vector3.MoveTowards(rightWaterWall.position, targetRight, partSpeed * Time.deltaTime);
            }
        }
    }
}

