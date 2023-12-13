using System.Collections.Generic;
using UnityEngine;


namespace TOONIPLAY
{
    public class CharacterManager : TSingletonMonoBehaviour<CharacterManager>
    {
        private GameObject _characterRootPrefab;
        private List<GameObject> _presetModelPrefabs;

        private readonly Dictionary<string, TooniCharacter> _characterMap = new();

        public TooniCharacter AddCharacter(string uid,
                                           GameObject characterId,
                                           IPeerHandler parentPeer,
                                           TooniCharacter.InputType inputType,
                                           Vector3 position,
                                           Quaternion rotation)
        {
            var characterController = CreateCharacter(characterId, position, rotation);

            if (characterController == null)
                return null;

            characterController.parentPeer = parentPeer;
            characterController.CurrentInputType = inputType;

            switch (inputType)
            {
                case TooniCharacter.InputType.Human:
                    {
                        characterController.gameObject.name = $"Player({uid})";
                        characterController.gameObject.layer = LayerMask.NameToLayer("Player");
                        //characterController.SetCamera(Camera.main);
                        //CameraManager.Instance.SetCamera(CameraManager.CameraType.ThirdPerson, characterController.CameraTargetHandler.transform, characterController.CameraTargetHandler.transform);
                    }

                    break;

                case TooniCharacter.InputType.AI:
                    {
                        characterController.gameObject.name = $"AI Player({uid})";
                        characterController.gameObject.layer = LayerMask.NameToLayer("HideCharacter");
                    }

                    break;

                case TooniCharacter.InputType.Network:
                    {
                        characterController.gameObject.name = $"Network Player({uid})";
                        characterController.gameObject.layer = LayerMask.NameToLayer("HideCharacter");
                    }

                    break;

                case TooniCharacter.InputType.None:
                default:
                    GameObject.Destroy(characterController.gameObject);
                    characterController = null;

                    Debug.LogError("지원하지 않는 타입");
                    break;
            }

            if (characterController == null)
                return characterController;

            if (_characterMap.ContainsKey(uid))
            {
                _characterMap.Remove(uid);
            }

            _characterMap.Add(uid, characterController);

            return characterController;
        }

        public void DeleteCharacter(string uid)
        {
            if (_characterMap.TryGetValue(uid, out var deletedCharacter))
            {
                GameObject.Destroy(deletedCharacter.gameObject);

                _characterMap.Remove(uid);
            }
        }

        public void DeleteAllCharacter()
        {
            foreach (var element in _characterMap)
            {
                if (element.Value != null)
                    GameObject.Destroy(element.Value.gameObject);
            }

            _characterMap.Clear();
        }

        public TooniCharacter GetCharacterController(string uid)
        {
            return _characterMap.TryGetValue(uid, out var characterController) ? characterController : null;
        }

        public TooniCharacter GetMainCharacterController()
        {
            foreach (var iter in _characterMap)
            {
                if (iter.Value.CurrentInputType != TooniCharacter.InputType.Network)
                    return iter.Value;
            }

            return null;
        }

        private TooniCharacter CreateCharacter(GameObject modelPrefab, Vector3 position, Quaternion rotation)
        {
            var newCharRoot = GameObject.Instantiate(_characterRootPrefab, position, rotation);
            if (newCharRoot == null)
                return null;

            var newModel = GameObject.Instantiate(modelPrefab, newCharRoot.transform, false);
            if (newModel == null)
            {
                GameObject.Destroy(newCharRoot);
                return null;
            }

            var characterRoot = newCharRoot.GetComponent<BaseCharacterController>();
            if (characterRoot == null)
                return newCharRoot.GetComponent<TooniCharacter>();

            if (!newModel.TryGetComponent<CharacterModelConfig>(out var characterModelConfig))
            {
                // NOTE(jaeil) : 만약 modelRoot에 BaseCharacterController는 있지만 CharacterModelConfig이 없는 경우는 없어야 합니다.
                GameObject.Destroy(newModel);
                GameObject.Destroy(newCharRoot);

                return null;
            }
            
            characterRoot.SetAnimator(newModel.GetComponent<Animator>());

            characterModelConfig.Character = characterRoot;
            characterRoot.CameraTargetHandler.transform.localPosition = characterModelConfig.EyePosition;
            characterRoot.characterModel = newModel;

            return newCharRoot.GetComponent<TooniCharacter>();
        }

        public void ApplySetting(CharacterSettingSO characterSetting)
        {
            _characterRootPrefab = characterSetting.characterRootPrefab;
            _presetModelPrefabs = characterSetting.modelPrefabs;
        }
    }
}
