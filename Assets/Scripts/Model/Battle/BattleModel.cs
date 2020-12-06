﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using Main.Data;

namespace Main.Model.Battle
{
    public class BattleModel
    {
        // HP
        IntReactiveProperty hp = new IntReactiveProperty(100);
        // MP
        IntReactiveProperty mp = new IntReactiveProperty(0);
        // 経過ターン数
        IntReactiveProperty turn = new IntReactiveProperty(0);
        // 火属性ダメージ半減バリア
        IntReactiveProperty fireBarrior = new IntReactiveProperty(0);
        // 水属性ダメージ半減バリア
        IntReactiveProperty waterBarrior = new IntReactiveProperty(0);
        // 雷属性ダメージ半減バリア
        IntReactiveProperty thunderBarrior = new IntReactiveProperty(0);

        // プレイヤーの属性
        public AttributeType PlayerAttribute { get; private set; }
        // このターンに使用した枚数
        public int UsedHandCount { get; private set; } = 0;

        /// <summary>
        /// セットアップ
        /// </summary>
        public void Setup()
        {}

        // void SetDeck(DeckData deckData)
        // {
        //     deckData.cardList.ForEach(card => deck.AddRange(Enumerable.Repeat(card.cardData, card.count)));
        //     deck = deck.OrderBy(_ => Guid.NewGuid()).ToList();
        // }

        /// <summary>
        /// ターン開始
        /// </summary>
        public void TurnStart()
        {
            // ターンカウント
            turn.Value++;
            // MP回復
            mp.Value = Mathf.Min(100, mp.Value + Mathf.Min(5, turn.Value) * 10);
            // 使用枚数のリセット
            UsedHandCount = 0;
            // バリアのターン数消費
            fireBarrior.Value = Mathf.Max(0, fireBarrior.Value - 1);
            waterBarrior.Value = Mathf.Max(0, waterBarrior.Value - 1);
            thunderBarrior.Value = Mathf.Max(0, thunderBarrior.Value - 1);
        }

        /// <summary>
        /// HPを増減する
        /// </summary>
        public void AddHP(int value)
        {
            hp.Value += value;
        }

        /// <summary>
        /// MPを消費する
        /// </summary>
        public void UseMp(int value)
        {
            mp.Value -= value;
        }

        /// <summary>
        /// プレイヤーの属性を変更する
        /// </summary>
        public void SetPlayerAttribure(AttributeType type)
        {
            PlayerAttribute = type;
        }

        /// <summary>
        /// 攻撃力を計算する
        /// </summary>
        public float CulcAttack(int attack, AttributeType type)
        {
            return type == PlayerAttribute ? attack * 1.25f : attack;
        }

        /// <summary>
        /// HPを取得する
        /// </summary>
        public IReadOnlyReactiveProperty<int> GetHP()
        {
            return hp;
        }

        /// <summary>
        /// MPを取得する
        /// </summary>
        public IReadOnlyReactiveProperty<int> GetMP()
        {
            return mp;
        }

        /// <summary>
        /// 経過ターン数を取得する
        /// </summary>
        public IReadOnlyReactiveProperty<int> GetTurn()
        {
            return turn;
        }

        /// <summary>
        /// 火属性ダメージ半減バリアの状態を取得する
        /// </summary>
        public IReadOnlyReactiveProperty<int> GetFireBarrior()
        {
            return fireBarrior;
        }

        /// <summary>
        /// 水属性ダメージ半減バリアの状態を取得する
        /// </summary>
        public IReadOnlyReactiveProperty<int> GetWaterBarrior()
        {
            return waterBarrior;
        }

        /// <summary>
        /// 雷属性ダメージ半減バリアの状態を取得する
        /// </summary>
        public IReadOnlyReactiveProperty<int> GetThunderBarrior()
        {
            return thunderBarrior;
        }
    }
}