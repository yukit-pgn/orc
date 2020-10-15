using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Main.Data;

namespace Main.View.Home
{
    public class HomeUIView : MonoBehaviour
    {
        [SerializeField] Button playButton;
        [SerializeField] Button settingButton;

        Subject<ButtonType> OnClick = new Subject<ButtonType>();

        public void SetUp()
        {
            playButton.OnClickAsObservable().Subscribe(_ => OnClick.OnNext(ButtonType.Play));
            settingButton.OnClickAsObservable().Subscribe(_ => OnClick.OnNext(ButtonType.Setting));
        }

        public IObservable<ButtonType> OnClickAsObservable()
        {
            return OnClick;
        }
    }
}
