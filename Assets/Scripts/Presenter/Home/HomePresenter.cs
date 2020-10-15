using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Main.Service;
using Main.View.Home;
using Main.Data;

namespace Main.Presenter.Home
{
    public class HomePresenter : MonoBehaviour
    {
        [SerializeField] HomeUIView uiView;

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
            uiView.SetUp();
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
            uiView.OnClickAsObservable().Subscribe(OnClick).AddTo(this);
        }

        /// <summary>
        /// Update, FixedUpdateのイベントの設定
        /// </summary>
        void SetUpdateEvents()
        {
        }

        void OnClick(ButtonType type)
        {
            switch (type)
            {
                case ButtonType.Play:
                    break;
                case ButtonType.Setting:
                    break;
            }
        }
    }
}
