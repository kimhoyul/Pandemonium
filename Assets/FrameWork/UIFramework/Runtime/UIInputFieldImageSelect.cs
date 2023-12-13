using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TOONIPLAY
{
    [RequireComponent(typeof(TMP_InputField))]
    public class UIInputFieldImageSelect : MonoBehaviour
    {
        [SerializeField] private Sprite _selectedSprite;

        private TMP_InputField _inputField;
        private Sprite _originalSprite;

        private void Awake()
        {
            _inputField = GetComponent<TMP_InputField>();
            _originalSprite = _inputField.image.sprite;
        }

        private void OnEnable()
        {
            if (_inputField != null)
            {
                _inputField.onSelect.AddListener(OnSelected);
                _inputField.onDeselect.AddListener(OnDeselected);
            }
        }

        private void OnDisable()
        {
            if (_inputField != null)
            {
                _inputField.onSelect.RemoveListener(OnSelected);
                _inputField.onDeselect.RemoveListener(OnDeselected);
            }
        }

        private void OnSelected(string value)
        {
            _inputField.image.overrideSprite = _selectedSprite;
        }

        private void OnDeselected(string value)
        {
            _inputField.image.overrideSprite = _originalSprite;
        }
    }
}
