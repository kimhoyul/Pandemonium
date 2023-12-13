using System.Collections;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using TMPro;
using UnityEngine;

namespace TOONIPLAY
{
    public class MVVMText : MonoBehaviour
    {
        [SerializeField] private string propertyName;

        private ObservableObject _viewModelInstance;

        public void SetViewModelInstance(ObservableObject newViewModelInstance)
        {
            if (_viewModelInstance != null)
            {
                _viewModelInstance.PropertyChanged -= OnPropertyChanged;
                _viewModelInstance = null;
            }

            _viewModelInstance = newViewModelInstance;
            if (_viewModelInstance != null)
                _viewModelInstance.PropertyChanged += OnPropertyChanged;
        }

        private void OnEnable()
        {
            StartCoroutine(CoActivateViewModelInstance());
        }

        private IEnumerator CoActivateViewModelInstance()
        {
            var accessor = gameObject.GetComponentInParent<IViewModelAccessor>();
            if (accessor == null)
                yield break;
            
            while (_viewModelInstance == null)
            {
                _viewModelInstance = accessor.ViewModelInstance;
                if (_viewModelInstance != null)
                {
                    _viewModelInstance.PropertyChanged += OnPropertyChanged;

                    UpdateText(_viewModelInstance);
                }

                yield return null;
            }
        }
        
        private void OnDisable()
        {
            if (_viewModelInstance != null)
            {
                _viewModelInstance.PropertyChanged -= OnPropertyChanged;
                _viewModelInstance = null;
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender == null)
                return;

            if (propertyName.Equals(e.PropertyName))
                UpdateText(sender);
        }

        private void UpdateText(object sender)
        {
            var type = sender.GetType();
            var property = type.GetProperty(propertyName);
            if (property == null)
                return;

            if (gameObject.TryGetComponent<TMP_Text>(out var text))
                text.text = property.GetValue(sender).ToString();
        }
    }
}
