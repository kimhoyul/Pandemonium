using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class MVVMActiveObject : MonoBehaviour
{
    [SerializeField] private string propertyName;
    private ObservableObject _viewModelInstance;
    public bool isReverse;

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

    private void Start()
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

                UpdateGameObject(_viewModelInstance);
            }

            yield return null;
        }
    }

    private void OnDestroy()
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
            UpdateGameObject(sender);
    }

    private void UpdateGameObject(object sender)
    {
        var type = sender.GetType();
        var property = type.GetProperty(propertyName);
        if (property == null)
            return;

        if (property.GetValue(sender) is bool boolSender)
            gameObject.SetActive(boolSender ^ isReverse);
    }
}
