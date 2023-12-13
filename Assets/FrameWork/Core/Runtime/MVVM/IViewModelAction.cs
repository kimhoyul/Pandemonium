using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IViewModelAction
{
    public Action<ObservableObject> Action { get; set; }

}
