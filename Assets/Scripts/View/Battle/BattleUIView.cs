using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Main.Data.Battle;

namespace Main.View.Battle
{
    public class BattleUIView : MonoBehaviour
    {
        [SerializeField] Button turnEndButton;

        Subject<ButtonType> OnClick = new Subject<ButtonType>();

        public void Setup()
        {
            turnEndButton.OnClickAsObservable().Subscribe(_ => OnClick.OnNext(ButtonType.TurnEnd)).AddTo(this);
        }

        public void SetButtonActive(ButtonType type, bool active)
        {
            switch (type)
            {
                case ButtonType.TurnEnd:
                    turnEndButton.gameObject.SetActive(active);
                    break;
            }
        }

        public IObservable<ButtonType> OnClickAsObservable()
        {
            return OnClick;
        }
    }
}
