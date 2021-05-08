using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Main.Data.Home;

namespace Main.View.Home
{
    public class HomeUIView : MonoBehaviour
    {
        [SerializeField] Button playButton;
        [SerializeField] Button settingButton;

        // ボタンクリック時イベント
        Subject<ButtonType> OnClick = new Subject<ButtonType>();

        /// <summary>
        /// セットアップ
        /// </summary>
        public void SetUp()
        {
            playButton.OnClickAsObservable().Subscribe(_ => OnClick.OnNext(ButtonType.Play)).AddTo(this);
            settingButton.OnClickAsObservable().Subscribe(_ => OnClick.OnNext(ButtonType.Setting)).AddTo(this);
        }

        /// <summary>
        /// ボタンクリック時イベントを取得
        /// </summary>
        public IObservable<ButtonType> OnClickAsObservable()
        {
            return OnClick;
        }
    }
}
