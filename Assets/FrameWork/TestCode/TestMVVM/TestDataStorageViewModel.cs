using System.Collections;
using System.Collections.Generic;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UnityEngine;

public class TestDataStorageViewModel : ObservableObject
{
    public int IntValue
    {
        get
        {
            var testDataStorage = TOONIPLAY.GameManager.GetDataStorage<TestDataStorage>();
            return testDataStorage.intValue;
        }
        set
        {
            var testDataStorage = TOONIPLAY.GameManager.GetDataStorage<TestDataStorage>();
            SetProperty(ref testDataStorage.intValue, value);
        }
    }
    
    public ICommand IncreaseIntValueCommand { get; }

    public TestDataStorageViewModel()
    {
        IncreaseIntValueCommand = new RelayCommand(IncreaseIntValue);
    }

    private void IncreaseIntValue() => IntValue++;
}
