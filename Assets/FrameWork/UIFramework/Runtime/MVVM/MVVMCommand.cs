using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TOONIPLAY
{
    public class MVVMCommand : MonoBehaviour
    {
        [SerializeField] private string commandPropertyName;

        private bool isBounded;
        private ObservableObject viewModelInstance;

        public void SetViewModelInstance(ObservableObject newViewModelInstance)
        {
            viewModelInstance = newViewModelInstance;
            if (viewModelInstance != null && gameObject.TryGetComponent<Button>(out var button))
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => OnCommandListener(viewModelInstance));
                isBounded = true;
            }
        }

        private void OnEnable()
        {
            if (!isBounded)
                StartCoroutine(CoActivateViewModeInstance());
        }

        private IEnumerator CoActivateViewModeInstance()
        {
            var accessor = gameObject.GetComponentInParent<IViewModelAccessor>();
            if (accessor == null)
                yield break;

            while (viewModelInstance == null)
            {
                viewModelInstance = accessor.ViewModelInstance;
                if (viewModelInstance != null && gameObject.TryGetComponent<Button>(out var button))
                {
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() => OnCommandListener(viewModelInstance));
                    isBounded = true;
                }

                yield return null;
            }
        }

        private void OnCommandListener(ObservableObject viewModelInstance)
        {
            var type = viewModelInstance.GetType();
            var command = type.GetProperty(commandPropertyName);

            var commandInstance = command.GetValue(viewModelInstance);
            var execute = command.PropertyType.GetMethod("Execute");
            execute.Invoke(commandInstance, new object[] { null });
        }
    }
}
