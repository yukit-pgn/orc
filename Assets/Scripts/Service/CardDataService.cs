using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using Main.Data;

namespace Main.Service
{
    public class CardDataService : SingletonMonoBehaviour<CardDataService>
    {
        static readonly string conditionListKey = "ConditionList";
        static readonly string effectListKey = "EffectList";

        // 条件リスト
        List<ConditionData> conditionList = null;
        // 効果リスト
        List<EffectData> effectList = null;

        protected override async void Awake()
        {
            base.Awake();

            var conditionCSV = await Addressables.LoadAssetAsync<TextAsset>(conditionListKey);
            var effectCSV = await Addressables.LoadAssetAsync<TextAsset>(effectListKey);
            conditionList = CSVtoConditionList(conditionCSV);
            effectList = CSVtoEffectList(effectCSV);
        }

        /// <summary>
        /// CSVから条件データを読み込む
        /// </summary>
        List<ConditionData> CSVtoConditionList(TextAsset textAsset)
        {
            return textAsset.text.Split(new string[] { "\r\n" }, StringSplitOptions.None)
            .Where(line => line.Length == 1 || (line.Length >= 2 && line.Substring(0, 2) != "##"))
            .Select(line => 
            {
                var l = line.Split(',');
                return new ConditionData(int.Parse(l[0]), int.Parse(l[3]), l[2], l[1]);
            })
            .ToList();
        }

        /// <summary>
        /// CSVから効果データを読み込む
        /// </summary>
        List<EffectData> CSVtoEffectList(TextAsset textAsset)
        {
            return textAsset.text.Split(new string[] { "\r\n" }, StringSplitOptions.None)
            .Where(line => line.Length == 1 || (line.Length >= 2 && line.Substring(0, 2) != "##"))
            .Select(line => 
            {
                var l = line.Split(',');
                return new EffectData(int.Parse(l[0]), int.Parse(l[3]), l[2], l[1]);
            })
            .ToList();
        }

        /// <summary>
        /// IDから条件を取得
        /// </summary>
        async UniTask<ConditionData> GetConditionData(int id)
        {
            // 条件リストが読み込まれるまで待つ
            await UniTask.WaitWhile(() => conditionList == null);

            return conditionList.First(c => c.id == id);
        }

        /// <summary>
        /// IDから効果を取得
        /// </summary>
        async UniTask<EffectData> GetEffectData(int id)
        {
            // 条件リストが読み込まれるまで待つ
            await UniTask.WaitWhile(() => effectList == null);

            return effectList.First(c => c.id == id);
        }

        /// <summary>
        /// IDから条件の説明を取得
        /// </summary>
        public async UniTask<string> GetConditionExplanation(int id)
        {
            return (await GetConditionData(id)).explanation;
        }

        /// <summary>
        /// IDから条件のスプライトを取得
        /// </summary>
        public async UniTask<Sprite> GetConditionSprite(int id)
        {
            return await Addressables.LoadAssetAsync<Sprite>("Condition/" + id.ToString() + ".png");
        }

        /// <summary>
        /// IDから効果の説明を取得
        /// </summary>
        public async UniTask<string> GetEffectExplanation(int id)
        {
            return (await GetEffectData(id)).explanation;
        }

        public async UniTask<Sprite> GetEffectSprite(int id)
        {
            return await Addressables.LoadAssetAsync<Sprite>("Effect/" + id.ToString() + ".png");
        }

        /// <summary>
        /// CardDataから名前を取得
        /// </summary>
        public async UniTask<string> GetCardName(CardData cardData)
        {
            var name = (await GetEffectData(cardData.effect1ID)).name;
            if (cardData.conditionID != 0)
            {
                name = (await GetConditionData(cardData.conditionID)).name + "・" + name;
            }
            if (cardData.effect2ID != 0)
            {
                name += "&" + (await GetEffectData(cardData.effect2ID)).name;
            }
            return name;
        }

        /// <summary>
        /// CardDataからコストを取得
        /// </summary>
        public async UniTask<int> GetCardCost(CardData cardData)
        {
            int cost = -10 + (await GetEffectData(cardData.effect1ID)).cost;
            if (cardData.conditionID != 0)
            {
                cost -= (await GetConditionData(cardData.conditionID)).cost;
            }
            if (cardData.effect2ID != 0)
            {
                cost += (await GetEffectData(cardData.effect2ID)).cost;
            }
            return Mathf.Max(cost, 0);
        }
    }
}