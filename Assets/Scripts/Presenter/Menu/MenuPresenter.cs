using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Main.Service;
using Main.View.Menu;
using Main.Data.Menu;

namespace Main.Presenter.Menu
{
    public class MenuPresenter : MonoBehaviour
    {
        [SerializeField] MenuUIView uiView;
        /// <summary>
        /// 初期設定
        /// </summary>
        void Start()
        {
            SetUpModels();

            SetUpViews();

            Bind();

            SetEvents();

            SetUpdateEvents();
        }

        /// <summary>
        /// Modelの設定
        /// </summary>
        void SetUpModels()
        {
        }


        /// <summary>
        /// Viewの設定
        /// </summary>
        void SetUpViews()
        {
            // uiViewのセットアップ
            uiView.Setup();
        }

        /// <summary>
        /// Modelの監視
        /// </summary>
        void Bind()
        {
        }

        /// <summary>
        /// Viewのイベントの設定
        /// </summary>
        void SetEvents()
        {
            // uiViewの監視
            uiView.OnClickAsObservable().Subscribe(OnClick).AddTo(this);
        }

        /// <summary>
        /// Update, FixedUpdateのイベントの設定
        /// </summary>
        void SetUpdateEvents()
        {
        }

        /// <summary>
        /// ボタンクリック時
        /// </summary>
        async void OnClick(ButtonType type)
        {
            switch (type)
            {
                case ButtonType.BattleMenu:
                    await uiView.ChangeMenu(MenuType.Battle);
                    break;
                case ButtonType.DeckMenu:
                    await uiView.ChangeMenu(MenuType.Deck);
                    break;
                case ButtonType.CreateMenu:
                    await uiView.ChangeMenu(MenuType.Create);
                    break;
            }
        }
    }
}
