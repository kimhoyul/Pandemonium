using System.Collections;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

public class MVVMImage : MonoBehaviour
{
    [SerializeField] private string propertyName;
    [SerializeField] Sprite emptyImage;
    [SerializeField] Color emptyColor;
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

    private void Awake()
    {
        if (gameObject.TryGetComponent<Image>(out var image))
        {
            if (emptyImage == null)
                emptyImage = image.sprite;
            if (emptyColor == Color.clear)
                emptyColor = image.color;
        }
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

                UpdateImage(_viewModelInstance);
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
            UpdateImage(sender);
    }

    private void UpdateImage(object sender)
    {
        var type = sender.GetType();
        var property = type.GetProperty(propertyName);
        if (property == null)
            return;

        if (gameObject.TryGetComponent<Image>(out var image))
        {
            if (property.GetValue(sender) is Sprite spriteSender)
                image.sprite = spriteSender;

            else if (property.GetValue(sender) is Color colorSender)
            {
                if (colorSender != Color.clear)
                    image.color = colorSender;
                else
                    image.color = emptyColor;
            }

            else
            {
                image.sprite = emptyImage;
                image.color = emptyColor;
            }
        }
    }
}
