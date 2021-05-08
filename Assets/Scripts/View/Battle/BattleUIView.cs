using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Main.Data.Battle;

namespace Main.View.Battle
{
    public class BattleUIView : MonoBehaviour
    {
        [SerializeField] Button turnEndButton;
        [SerializeField] Image myHPBar;
        [SerializeField] TextMeshProUGUI myHPText;
        [SerializeField] Image myMPBar;
        [SerializeField] TextMeshProUGUI myMPText;
        [SerializeField] Image enemyHPBar;
        [SerializeField] TextMeshProUGUI enemyHPText;
        [SerializeField] Image enemyMPBar;
        [SerializeField] TextMeshProUGUI enemyMPText;
        [SerializeField] TextMeshProUGUI winText;
        [SerializeField] TextMeshProUGUI loseText;

        Vector2 myHPBarSize;
        Vector2 myMPBarSize;
        Vector2 enemyHPBarSize;
        Vector2 enemyMPBarSize;

        // ボタンクリック時イベント
        Subject<ButtonType> OnClick = new Subject<ButtonType>();

        /// <summary>
        /// セットアップ
        /// </summary>
        public void Setup()
        {
            myHPBarSize = myHPBar.rectTransform.sizeDelta;
            myMPBarSize = myMPBar.rectTransform.sizeDelta;
            enemyHPBarSize = enemyHPBar.rectTransform.sizeDelta;
            enemyMPBarSize = enemyMPBar.rectTransform.sizeDelta;

            winText.gameObject.SetActive(false);
            loseText.gameObject.SetActive(false);

            turnEndButton.OnClickAsObservable().Subscribe(_ => OnClick.OnNext(ButtonType.TurnEnd)).AddTo(this);
        }

        /// <summary>
        /// ボタンの表示/非表示設定
        /// </summary>
        public void SetButtonActive(ButtonType type, bool active)
        {
            switch (type)
            {
                case ButtonType.TurnEnd:
                    turnEndButton.gameObject.SetActive(active);
                    break;
            }
        }

        /// <summary>
        /// 自分のHPを設定
        /// </summary>
        public void SetMyHP(int value)
        {
            myHPBar.rectTransform.DOSizeDelta(new Vector2(myHPBarSize.x * value / 100, myHPBarSize.y), 0.3f);
            myHPText.text = $"HP {value} / 100";
        }

        /// <summary>
        /// 自分のMPを設定
        /// </summary>
        public void SetMyMP(int value, int maxValue)
        {
            myMPBar.rectTransform.DOSizeDelta(new Vector2(myMPBarSize.x * value / 100, myMPBarSize.y), 0.3f);
            myMPText.text = $"MP {value} / {maxValue}";
        }

        /// <summary>
        /// 相手のHPを設定
        /// </summary>
        public void SetEnemyHP(int value)
        {
            enemyHPBar.rectTransform.DOSizeDelta(new Vector2(enemyHPBarSize.x * value / 100, enemyHPBarSize.y), 0.3f);
            enemyHPText.text = $"HP {value} / 100";
        }

        /// <summary>
        /// 相手のMPを設定
        /// </summary>
        public void SetEnemyMP(int value, int maxValue)
        {
            enemyMPBar.rectTransform.DOSizeDelta(new Vector2(enemyMPBarSize.x * value / 100, enemyMPBarSize.y), 0.3f);
            enemyMPText.text = $"MP {value} / {maxValue}";
        }

        /// <summary>
        /// 勝利時アニメーション
        /// </summary>
        public async UniTask Win()
        {
            winText.gameObject.SetActive(true);
            winText.color = new Color(1f, 1f, 1f, 0f);
            winText.rectTransform.localScale = Vector3.one * 3f;

            winText.DOColor(Color.white, 0.5f).SetEase(Ease.InQuart);
            winText.rectTransform.DOScale(Vector3.one * 0.6f, 0.4f).SetEase(Ease.InQuad);

            await UniTask.Delay(400);

            winText.rectTransform.DOScale(Vector3.one, 0.1f);

            await UniTask.Delay(100);
        }

        /// <summary>
        /// 敗北時アニメーション
        /// </summary>
        public async UniTask Lose()
        {
            loseText.gameObject.SetActive(true);

            loseText.color = new Color(1f, 1f, 1f, 0f);

            loseText.DOColor(Color.white, 0.5f);

            await UniTask.Delay(500);
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
