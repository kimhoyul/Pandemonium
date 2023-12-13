using UnityEngine;

namespace TOONIPLAY
{
    public class EmptyScene : MonoBehaviour
    {
        private void Start()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(1);
        }
    }
}
